namespace Strategy;

public class BkashPayment:IPaymentStrategy
{
    public void pay(decimal amount)
    {
        Console.WriteLine(amount+" Pay with Bkash");
    }
}