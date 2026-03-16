using PaymentSystem.Interfaces;

namespace PaymentSystem.Payments;

public class BkashPayment:IPaymentMethod
{
    public decimal pay(decimal amount)
    {
        return amount;
    }
}