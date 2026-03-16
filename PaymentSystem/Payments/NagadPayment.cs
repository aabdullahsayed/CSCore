using PaymentSystem.Interfaces;

namespace PaymentSystem.Payments;

public class NagadPayment:IPaymentMethod
{
    public decimal pay(decimal amount)
    {
        Console.WriteLine("Payment With Nagad");
        return amount;
    }
}