namespace Strategy;

public interface IPaymentStrategy
{
    void pay(decimal amount);
}