namespace PaymentSystem.Interfaces;

public interface IPaymentMethod
{
    decimal pay(decimal amount);
}