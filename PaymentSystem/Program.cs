using PaymentSystem.Discounts;
using PaymentSystem.Interfaces;
using PaymentSystem.Notifications;
using PaymentSystem.Payments;
using PaymentSystem.Services;

namespace PaymentSystem;

class Program
{
    static void Main(string[] args)
    {

        IPaymentMethod paymentMethod = new BkashPayment();
        IDiscountStrategy discountStrategy = new PercentageDiscount(10);
        PaymentService paymentService = new PaymentService(paymentMethod,discountStrategy);
        paymentService.MakePayment(1000);
        INotification notification = new EmailNotification();
        NotificationService notificationService = new NotificationService(notification);
        notificationService.SendNotification("New Email from Bkash");
        

    }
}