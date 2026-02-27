namespace OCP;

public class DiscountCalculator
{

    public static decimal Calculate(decimal price, IDiscount discount)
    {
        return discount.Apply(price);
    }

}