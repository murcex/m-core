using Microsoft.Data.SqlClient;
using Murcex.PlyQor.Internal.Container.Core;
using Murcex.PlyQor.Internal.Container.Model;
using System.Data;
using System.Text.Json;

namespace Murcex.PlyQor.Internal.Container.Storage
{
	/// <summary>
	/// Serializes a list of PlyQorContainer objects and uploads them to the database as a JSON string.
	/// </summary>
	public class UploadContainers
	{
		/// <summary>
		/// Serializes the containers and inserts/updates them in the database.
		/// </summary>
		/// <param name="containers">List of PlyQorContainer objects to upload.</param>
		public void Execute(List<PlyQorContainer> containers)
		{
			// Build the dictionary for serialization
			var containerDict = new Dictionary<string, Dictionary<string, string>>();

			foreach (var container in containers)
			{
				var tokens = new List<string>();
				if (!string.IsNullOrEmpty(container.PrimaryToken))
				{
					tokens.Add(container.PrimaryToken);
				}

				if (!string.IsNullOrEmpty(container.SecondaryToken))
				{
					tokens.Add(container.SecondaryToken);
				}

				var jsonTokens = JsonSerializer.Serialize(tokens);

				containerDict[container.Name] = new Dictionary<string, string>
				{
					{ "Retention", container.Retention.ToString() },
					{ "Tokens", jsonTokens }
				};
			}

			var json = JsonSerializer.Serialize(containerDict);

			json = json.Replace("u0022", "\""); // Escape backslashes for SQL compatibility

			// Insert/update the JSON string in the database
			try
			{
				using var connection = new SqlConnection(Configuration.DatabaseConnection);
				using var cmd = new SqlCommand(Configuration.UpsertStoredProcedure, connection)
				{
					CommandType = CommandType.StoredProcedure,
					CommandTimeout = 0
				};

				// Add all required parameters including the timestamp
				cmd.Parameters.AddWithValue("@dt_timestamp", DateTime.UtcNow); // Provide current timestamp
				cmd.Parameters.AddWithValue(Configuration.ParameterId, Configuration.ContainersId);
				cmd.Parameters.AddWithValue(Configuration.ParameterData, json);

				connection.Open();
				cmd.ExecuteNonQuery();
			}
			catch (SqlException sqlEx)
			{
				throw new Exception($"Database error in UploadContainers: {sqlEx.Message}", sqlEx);
			}
			catch (Exception ex)
			{
				throw new Exception($"UploadContainers Exception: {ex.Message}", ex);
			}
		}
	}
}
