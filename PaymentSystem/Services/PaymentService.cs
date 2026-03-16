using PaymentSystem.Interfaces;

namespace PaymentSystem.Services;

public class PaymentService
{
    private readonly IPaymentMethod _paymentMethod;
    private readonly IDiscountStrategy _discountStrategy;

    public PaymentService(IPaymentMethod paymentMethod, IDiscountStrategy discountStrategy)
    {
        _paymentMethod = paymentMethod;
        _discountStrategy = discountStrategy;
    }

    public void MakePayment(decimal amount)
    {
        decimal FinalAmount = _discountStrategy.ApplyDiscount(amount);
        _paymentMethod.pay(FinalAmount);
        Console.WriteLine("PAID: "+ FinalAmount);
    }
    
}