namespace ISP;

public class MultiFunctionPrinter:IScanner,IPrinter,IFax
{


    public void scan()
    {
        Console.WriteLine("Scan your document");
    }
    
    public void print()
    {
        Console.WriteLine("Print your document");
    }

    public void fax()
    {
        Console.WriteLine("Fax your document");
    }
}