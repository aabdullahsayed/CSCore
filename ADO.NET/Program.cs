using System;
using Microsoft.Data.SqlClient;
namespace ADO.NET;
class Program
{
    static void Main(string[] args)
    {
        string connectionString =
            "Server=Localhost;Database=test_db;" +
            "Trusted_Connection=True;TrustServerCertificate=True";
        string query = "SELECT * FROM doctors";
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Connected");
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine(
                        reader["Id"] + " " +
                        reader["Name"] + " " +
                        reader["Department"]);
                }
                reader.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}