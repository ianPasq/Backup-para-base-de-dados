namespace BackupBaseDeDados.Utilidades.Notifications;

public interface INotificationService
{
    Task SendNotification(string message);
}

public class SlackNotificationService : INotificationService
{
    private readonly string _webhookUrl;

    public SlackNotificationService(string webhookUrl)
    {
        _webhookUrl = webhookUrl;
    }

    public async Task SendNotification(string message)
    {
        // adicionar slack notifications 
    }
}
