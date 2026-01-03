namespace Controllers.Hubs
{
    public interface IChatClient
    {
        Task ReceiveFeedMessage(string user, string message, DateTime time);
        Task ReceivePrivateMessage(string senderName, string message, DateTime time);
        Task ReceiveSystemNotification(string message);
    }
}
