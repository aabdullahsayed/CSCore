using PaymentSystem.Interfaces;

namespace PaymentSystem.Discounts;

public class PercentageDiscount: IDiscountStrategy
{
    private readonly decimal _percent;

    public PercentageDiscount(decimal percentage)
    {
        _percent = percentage;
    }
    public decimal ApplyDiscount(decimal amount)
    {

        return amount - (amount * _percent / 100);
    }
}