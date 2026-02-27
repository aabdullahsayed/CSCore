namespace OCP;

class Program
{
    static void Main(string[] args)
    {
        decimal price = 1000;

        IDiscount l = new LoyalityDiscount();
        IDiscount s = new SeasonalDiscount();

        decimal a = DiscountCalculator.Calculate(price, l);
        decimal b = DiscountCalculator.Calculate(price, s);

        Console.WriteLine(a+" "+b);

    }
}
/*

Open/Closed Principle
Classes should be open for extension but closed for
modification.

*/
