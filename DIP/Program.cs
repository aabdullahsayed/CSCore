namespace DIP;

class Program
{
    static void Main(string[] args)
    {
        IDatabase database = new MySqlDatabase();
        IDatabase database2 = new FileDatabase();
        BudgetReport budgetReport = new BudgetReport(database);
        BudgetReport budgetReport2 = new BudgetReport(database2);
        
        budgetReport.GenerateReport();
        budgetReport2.GenerateReport();
        
    }
}

/*
 
 Dependency Inversion Principle

High-level classes shouldn’t depend on low-level classes.
Both should depend on abstractions. 
Abstractions shouldn’t depend on details. 
Details should depend on abstractions.

*/

