using PaymentSystem.Interfaces;

namespace PaymentSystem.Payments;

public class CreditCardPayment:IPaymentMethod
{
    public decimal pay(decimal amount)
    {
        Console.WriteLine("Payment With Card");
        return amount;
    }
}