using PaymentSystem.Interfaces;

namespace PaymentSystem.Notifications;

public class EmailNotification:INotification
{
    public string notify(string msg)
    {
        return msg;
    }
}