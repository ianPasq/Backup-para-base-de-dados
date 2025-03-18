using System.Diagnostics;
using MySqlConnector;

namespace BackupBaseDeDados.Utilidades.Database;

public class MySqlConnectionService : IDatabaseConnection
{
    private readonly MySqlConnection _connection;
    private readonly string _user;
    private readonly string _password;
    private readonly string _database;

    public MySqlConnectionService(string host, string database, string user, string password)
    {
        _connection = new MySqlConnection($"Server={host};Database={database};Uid={user};Pwd={password};");
        _user = user;
        _password = password;
        _database = database;
    }

    public void Connect() => _connection.Open();
    public void Disconnect() => _connection.Close();
    
    public async Task ExecuteCommandAsync(string command)
    {
        using var cmd = new MySqlCommand(command, _connection);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task CreateBackupAsync(string backupPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(backupPath)!);
        var processInfo = new ProcessStartInfo
        {
            FileName = "mysqldump",
            Arguments = $"-u {_user} -p{_password} {_database} --result-file=\"{backupPath}\"",
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = processInfo };
        process.Start();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync();
            throw new Exception($"MySQL backup failed: {error}");
        }
    }
}
