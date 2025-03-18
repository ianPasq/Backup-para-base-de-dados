using BackupBaseDeDados.Utilidades.Database;

namespace BackupBaseDeDados.Utilidades.Backup;


public interface IBackupService
{
    Task CreateBackupAsync(string backupPath);
}

public class BackupService : IBackupService
{
    private readonly IDatabaseConnection _connection;

    public BackupService(IDatabaseConnection connection)
    {
        _connection = connection;
    }

    public async Task CreateBackupAsync(string backupPath)
    {
        await _connection.CreateBackupAsync(backupPath);
        // implementar Backup
    }
}
