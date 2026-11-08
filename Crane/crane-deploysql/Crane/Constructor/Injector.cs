using System;
using Microsoft.Data.SqlClient;

namespace Crane
{
    /// <summary>
    /// 
    /// </summary>
	class Injector
    {
        public static string Execute(string articleCase)
        {
            using (SqlConnection connection = new SqlConnection(Global.SQLConnectionString))
            {
                // Apply SQL Article to SQL Instance
                SqlCommand command = new SqlCommand(articleCase, connection);

                try
                {
                    // Execute SQL
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    // Return Results to Console
                    if (reader.RecordsAffected == -1)
                    {
                        Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("\t\t<!> Success");
                        Console.ResetColor();
                        Log.Info("|-> Success");
                    }
                    if (reader.RecordsAffected > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("\t\t<!> Success (Rows Affected: {0})", reader.RecordsAffected);
                        Console.ResetColor();
                        Log.Info($"|-> Success (Rows Affected: {reader.RecordsAffected})");
                    }
                    if (reader.RecordsAffected == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("\t\t<!> Failure");
                        Console.ResetColor();
                        Log.Error("|-> Failure");
                    }

                    return "Success";
                }

                catch (Exception e)
                {
                    // Check Exception for 'IF EXISTS'
                    var check = e.ToString();
                    if (check.Contains("There is already an object named"))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\t\t<!> Success (Object Exists)");
                        Console.ResetColor();
                        Log.Info("|-> Success (Object Exists)");

                        return "Success (Object Exists)";
                    }

                    // Return All Other Exceptions
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\t\tResult: Failure (SQL Exception)");
                        Console.ResetColor();
                        Log.Error($"\r\n|-> Failure (SQL Exception): {e}");

                        return $"Failure (SQL Exception): {e}";
                    }
                }
            }
        }
    }
}
