namespace Strategy;

public class CardPayment:IPaymentStrategy
{
    public void pay(decimal amount)
    {
        Console.WriteLine(amount+" Pay with Card");
    }
}