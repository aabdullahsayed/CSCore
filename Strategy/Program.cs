namespace Strategy;

class Program
{
    static void Main(string[] args)
    {

        PaymentProcess process = new PaymentProcess(new CardPayment());
        process.ProcessPayment(500);
        
        process.SetPaymentStrategy(new BkashPayment());
        process.ProcessPayment(1000);
        
        process.SetPaymentStrategy(new NagadPayment());
        process.ProcessPayment(4000);

    }
}

/*
Encapsulate what varies
Favor composition over inheritance
Program to interfaces, not implementations
 */
