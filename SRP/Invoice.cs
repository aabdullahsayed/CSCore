namespace SRP;

public class Invoice
{
 
    public int Number { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }

    public Invoice(int Number, string Name, decimal Amount)
    {
        if (Amount < 0) throw new Exception("Amount can not be less than zero");
        this.Number = Number;
        this.Name = Name;
        this.Amount = Amount;
    }
    
}