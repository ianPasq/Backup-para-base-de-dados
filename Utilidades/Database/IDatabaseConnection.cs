namespace BackupBaseDeDados.Utilidades.Database;

public interface IDatabaseConnection
{
    // Add required database connection methods
    void Connect();
    void Disconnect();
    Task ExecuteCommandAsync(string command);
    Task CreateBackupAsync(string backupPath);
    // ... outros membros ...
}