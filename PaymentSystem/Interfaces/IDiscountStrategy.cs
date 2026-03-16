namespace PaymentSystem.Interfaces;

public interface IDiscountStrategy
{

    decimal ApplyDiscount(decimal amount);

}