namespace BackupBaseDeDados.Configs;

public record DatabaseSettings(
    string Type,
    string Host,
    string DatabaseName,
    string Username,
    string Password);

public record StorageSettings(
    string LocalPath,
    string CloudPath,
    bool EnableEncryption);

public record NotificationSettings(
    string SlackWebhookUrl,
    string EmailServiceUrl);

public enum ExitCode
{
    Success = 0,
    ConfigFileMissing = 1,
    DatabaseConnectionFailed = 2,
    UnhandledError = 99
}
