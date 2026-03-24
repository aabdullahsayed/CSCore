namespace DIP;

public class MySqlDatabase:IDatabase
{
    public string Read()
    {
        return "Data From MySql";
    }
}