## Dependency Inversion Principle
High-level classes shouldn’t depend on low-level classes. Both should depend on abstractions. Abstractions
shouldn’t depend on details. Details should depend on
abstractions.


Without Dependency Inversion Principle
```c#
class Program
{
static void Main(string[] args)
{

        BudgetReport b = new BudgetReport();
        b.GenerateReport();
    }
}


//High-level depends on abstraction	❌
//Low-level depends on abstraction	❌
//Easy to replace database	❌
//Easy to test	❌
//Open for extension	❌

public class MysqlData
{
    public string Read()
    {
        return "Data From mysql Database";
    }
}

// ❌ PROBLEM HERE:
// This is a LOW-LEVEL module (database access).
// It is concrete and tightly coupled to MySQL.
// No abstraction (interface) exists.


public class BudgetReport
{
    private readonly MysqlData _mysqlData;
    
    // ❌ ISSUE #1:
    // High-level module (BudgetReport)
    // depends directly on a LOW-LEVEL concrete class (MysqlData)
    public BudgetReport()
    {
        // ❌ ISSUE #2:
        // BudgetReport is RESPONSIBLE for creating its dependency
        // This causes tight coupling and violates DIP
        _mysqlData = new MysqlData();
    }

    public void GenerateReport()
    {
        string data = _mysqlData.Read();

        // ❌ ISSUE #3:
        // If tomorrow data comes from:
        // - PostgreSQL 
        // - API
        // - File
        // - Mock for testing
        // You MUST modify this class
        
        
        Console.WriteLine("Budget Report "+data);
    }
    
    
}

```
