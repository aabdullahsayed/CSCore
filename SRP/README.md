## Single Responsiblity Principle

`A class should have just one reason to change`

**Primary Goal** : To reduce complexity as a program grows.

The Problem without SRP is that when one class does many jobs, a change to any of those jobs you to open and edit that class and every edit is a chance to accidentally break the other jobs living in the same file.

**Before:** Violating SRP

```c#
using System;

public class Employee
{
    public string Name {get; set;}
    public string Role {get; set;}
    
    public Employee(string name, string role)
    {
        Name = name;
        Role = role;
    }
    
    public string GetEmployeeData()
    {
        return $"{Name} - {Role}";
    }
    
    // VIOLATION: Reporting behavior is mixed with data management
    public void PrintTimeSheetReport()
    {
        Console.WriteLine("Timesheet Report");
        Console.WriteLine("Employee: "+Name);
        Console.WriteLine("Hours logged: 40");
    }
    
    
}


```
**After**: Following SRP

```c#

using System;

// Responsibility 1: Managing Employee Data
public class Employee
{
    public string Name { get; private set; }
    public string Role { get; private set; }

    public Employee(string name, string role)
    {
        Name = name;
        Role = role;
    }

    public string GetName()
    {
        return Name;
    }
}

// Responsibility 2: Managing and Printing Reports
public class TimeSheetReport
{
    public void PrintReport(Employee employee)
    {
        Console.WriteLine("Timesheet Report");
        Console.WriteLine("Employee: "+Name);
        Console.WriteLine("Hours logged: 40");
    }
}

// Usage:
public class Program
{
    public static void Main()
    {
        Employee alice = new Employee("Alice", "Developer");
        TimeSheetReport reportGenerator = new TimeSheetReport();
        
        reportGenerator.PrintReport(alice);
    }
}
```