using PaymentSystem.Interfaces;

namespace PaymentSystem.Services;

public class NotificationService
{
    private readonly INotification _notification;

    public NotificationService(INotification notification)
    {
        _notification = notification;
    }

    public void SendNotification(string msg)
    {
        string s = _notification.notify(msg);
        Console.WriteLine(s);
    }
}