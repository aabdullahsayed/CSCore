using PaymentSystem.Interfaces;

namespace PaymentSystem.Discounts;

public class NoDiscount:IDiscountStrategy
{
    public decimal ApplyDiscount(decimal amount)
    {
        return amount;
    }
}