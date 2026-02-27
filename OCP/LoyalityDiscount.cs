namespace OCP;

public class LoyalityDiscount:IDiscount
{
    public decimal Apply(decimal price)
    {
        return price * 0.85m;
    }
}