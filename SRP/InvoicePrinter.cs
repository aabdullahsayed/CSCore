namespace SRP;

public class InvoicePrinter
{

    public void Print(Invoice invoice)
    {
        Console.WriteLine(invoice.Name+" "+invoice.Number+" "+invoice.Amount);
    }
}