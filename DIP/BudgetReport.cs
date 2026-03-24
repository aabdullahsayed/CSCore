namespace DIP;

public class BudgetReport
{
    private readonly IDatabase _database;
    
    public BudgetReport(IDatabase database)
    {
        _database = database;
    }

    public void GenerateReport()
    {
        Console.WriteLine(_database.Read());
    }
    
    
}