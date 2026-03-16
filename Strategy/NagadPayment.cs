namespace Strategy;

public class NagadPayment:IPaymentStrategy
{
    public void pay(decimal amount)
    {
        Console.WriteLine(amount+" Pay with Nagad");
    }
}