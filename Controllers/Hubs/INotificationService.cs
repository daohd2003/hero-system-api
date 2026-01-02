namespace Controllers.Hubs
{
    public interface INotificationService
    {
        Task SendSystemNotificationAsync(string userId, string message);
    }
}
