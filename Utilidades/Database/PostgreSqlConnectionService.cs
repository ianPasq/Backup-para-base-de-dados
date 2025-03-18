using System.Diagnostics;
using Npgsql;

namespace BackupBaseDeDados.Utilidades.Database;

public class PostgreSqlConnectionService : IDatabaseConnection
{
    private readonly NpgsqlConnection _connection;
    private readonly string _host;
    private readonly string _user;
    private readonly string _password;
    private readonly string _database;

    public PostgreSqlConnectionService(string host, string database, string user, string password)
    {
        _connection = new NpgsqlConnection($"Host={host};Database={database};Username={user};Password={password};");
        _host = host;
        _user = user;
        _password = password;
        _database = database;
    }

    public void Connect() => _connection.Open();
    public void Disconnect() => _connection.Close();
    
    public async Task ExecuteCommandAsync(string command)
    {
        using var cmd = new NpgsqlCommand(command, _connection);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task CreateBackupAsync(string backupPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(backupPath)!);
        var processInfo = new ProcessStartInfo
        {
            FileName = "pg_dump",
            Arguments = $"-h {_host} -U {_user} -Fc {_database} -f \"{backupPath}\"",
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        processInfo.EnvironmentVariables["PGPASSWORD"] = _password;

        using var process = new Process { StartInfo = processInfo };
        process.Start();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync();
            throw new Exception($"PostgreSQL backup failed: {error}");
        }
    }
}