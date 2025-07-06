using Microsoft.Data.SqlClient;

namespace Crane.Internal.Engine.Task.SQLDatabaseDeployment.Internal
{
	public class SQLAccess : ISQLAccess
	{
		private string? _connectionString;

		public bool SetCredentials(string connectionString)
		{
			_connectionString = connectionString;

			return true;
		}

		public (bool result, string code, string message) Execute(string sqlCmd)
		{
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				// Apply SQL Article to SQL Instance
				SqlCommand command = new SqlCommand(sqlCmd, connection);

				bool result = false;
				string code = string.Empty;
				string message = string.Empty;

				try
				{
					// Execute SQL
					connection.Open();
					SqlDataReader reader = command.ExecuteReader();

					// Return Results to Console
					if (reader.RecordsAffected == -1)
					{
						//Console.ForegroundColor = ConsoleColor.Green; 
						//Console.WriteLine("\t\t<!> Success");
						//Console.ResetColor();
						//Log.Info("|-> Success");
						result = true;
						code = "success";
						message = "rows_affected=-1";
					}
					if (reader.RecordsAffected > 0)
					{
						//Console.ForegroundColor = ConsoleColor.Green;
						//Console.WriteLine("\t\t<!> Success (Rows Affected: {0})", reader.RecordsAffected);
						//Console.ResetColor();
						//Log.Info($"|-> Success (Rows Affected: {reader.RecordsAffected})");
						result = true;
						code = "success";
						message = $"rows_affected={reader.RecordsAffected}";
					}
					if (reader.RecordsAffected == 0)
					{
						//Console.ForegroundColor = ConsoleColor.Red; 
						//Console.WriteLine("\t\t<!> Failure");
						//Console.ResetColor();
						//Log.Error("|-> Failure");
						result = false;
						code = "records_affected_zero";
						message = "rows_affected=0";
					}

					//return "Success";
				}
				catch (Exception e)
				{
					// Check Exception for 'IF EXISTS'
					var check = e.ToString();
					if (check.Contains("There is already an object named"))
					{
						//Console.ForegroundColor = ConsoleColor.Green;
						//Console.WriteLine("\t\t<!> Success (Object Exists)");
						//Console.ResetColor();
						//Log.Info("|-> Success (Object Exists)");

						//return "Success (Object Exists)";
						result = true;
						code = "object_exists";
						message = "success";
					}

					// Return All Other Exceptions
					else
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("\t\tResult: Failure (SQL Exception)");
						Console.ResetColor();
						//Log.Error($"\r\n|-> Failure (SQL Exception): {e.ToString()}");

						//return $"Failure (SQL Exception): {e}";
						result = false;
						code = "sql_exception";
						message = $"{e}";
					}
				}

				return (result, code, message);
			}
		}
	}
}
