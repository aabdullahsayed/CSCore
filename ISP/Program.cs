namespace ISP;

class Program
{
    static void Main(string[] args)
    {
        IPrinter a = new BasicPrinter();
        MultiFunctionPrinter m = new MultiFunctionPrinter();
        
        a.print();
        m.scan();
        m.print();
        m.fax();
    }
}
//Interface Segregation Principle : Clients shouldn’t be forced to depend on methods they do not use.