using BackupBaseDeDados.Utilidades.Database;

namespace BackupBaseDeDados.Utilidades.Backup;

public interface IRetryPolicy
{
    Task ExecuteAsync(Func<Task> action);
}

public class ExponentialBackoffRetryPolicy : IRetryPolicy
{
    private readonly int _maxRetries;
    
    public ExponentialBackoffRetryPolicy(int maxRetries) => _maxRetries = maxRetries;

    public async Task ExecuteAsync(Func<Task> action)
    {
        for (int retry = 0; retry < _maxRetries; retry++)
        {
            try
            {
                await action();
                return;
            }
            catch
            {
                if (retry == _maxRetries - 1) throw;
                await Task.Delay((int)Math.Pow(2, retry) * 1000);
            }
        }
    }
}

public class ResilientBackupService : IBackupService
{
    private readonly IBackupService _inner;
    private readonly IRetryPolicy _retryPolicy;

    public ResilientBackupService(IBackupService inner, IRetryPolicy retryPolicy)
    {
        _inner = inner;
        _retryPolicy = retryPolicy;
    }

    public Task CreateBackupAsync(string backupPath) => 
        _retryPolicy.ExecuteAsync(() => _inner.CreateBackupAsync(backupPath));
}
