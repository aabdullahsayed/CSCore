using PaymentSystem.Interfaces;

namespace PaymentSystem.Services;

public class InvoiceService
{
    private readonly IDiscountStrategy _discountStrategy;

    public InvoiceService(IDiscountStrategy discountStrategy)
    {
        _discountStrategy = discountStrategy;
    }
    
    
    
    
}