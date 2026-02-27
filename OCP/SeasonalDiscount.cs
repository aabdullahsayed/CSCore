namespace OCP;

public class SeasonalDiscount:IDiscount
{
    public decimal Apply(decimal price)
     {

         return price * 0.9m;

     }
}