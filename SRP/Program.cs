namespace SRP;

internal class Program
{
    static void Main(string[] args)
    {
        Invoice i = new Invoice(100, "Abdullah", 599);
        InvoicePrinter pr = new InvoicePrinter();
        pr.Print(i);
    }
}
//A class should have just one reason to change