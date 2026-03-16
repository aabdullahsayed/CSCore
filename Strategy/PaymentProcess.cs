namespace Strategy;

public class PaymentProcess
{
    private  IPaymentStrategy _paymentStrategy;

    public PaymentProcess(IPaymentStrategy paymentStrategy)
    {
        _paymentStrategy = paymentStrategy;
    }


    public void SetPaymentStrategy(IPaymentStrategy paymentStrategy)
    {
        _paymentStrategy = paymentStrategy;
    }

    public void ProcessPayment(decimal amount)
    {
        _paymentStrategy.pay(amount);
    }
    
    
}