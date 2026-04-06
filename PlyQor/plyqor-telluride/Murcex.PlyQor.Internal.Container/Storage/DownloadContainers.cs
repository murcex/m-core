using Microsoft.Data.SqlClient;
using Murcex.PlyQor.Internal.Container.Core;
using Murcex.PlyQor.Internal.Container.Model;
using System.Data;
using System.Text.Json;

namespace Murcex.PlyQor.Internal.Container.Storage
{
	public class DownloadContainers
	{
		public static List<PlyQorContainer> Execute()
		{
			var cfg = SelectContainerConfig();

			return CreateContainerConfig(cfg);
		}

		// get containers from storage
		/// <summary>
		/// Query the PlyQor System container for current container config.
		/// </summary>
		private static string SelectContainerConfig()
		{
			try
			{
				using var connection = new SqlConnection(Configuration.DatabaseConnection);
				using var cmd = new SqlCommand(Configuration.SelectStoredProcedure, connection)
				{
					CommandType = CommandType.StoredProcedure,
					CommandTimeout = 0
				};

				cmd.Parameters.AddWithValue(Configuration.ParameterId, Configuration.ContainersId);

				connection.Open();

				using var reader = cmd.ExecuteReader();
				if (reader.Read())
				{
					var value = reader[Configuration.ParameterData];
					return value is string str ? str : string.Empty;
				}

				return string.Empty;
			}
			catch (SqlException sqlEx)
			{
				throw new Exception($"Database error in SelectContainerConfig: {sqlEx.Message}", sqlEx);
			}
			catch (Exception ex)
			{
				throw new Exception($"SelectContainerConfig Exception: {ex.Message}", ex);
			}
		}

		/// <summary>
		/// Create a local copy of the PlyQor container config from the serialized Json string.
		/// </summary>
		private static List<PlyQorContainer> CreateContainerConfig(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				throw new ArgumentException("Input string for container config is null or empty.");
			}

			var containers_config = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(input);

			if (containers_config == null)
			{
				throw new InvalidOperationException("Deserialized container config is null.");
			}

			List<PlyQorContainer> plyQorContainers = new List<PlyQorContainer>();
			foreach (var container in containers_config)
			{
				var plyqorContainer = new PlyQorContainer
				{
					Name = container.Key,
					Retention = int.Parse(container.Value["Retention"].ToString())
				};

				var jsonTokens = container.Value["Tokens"];

				if (string.IsNullOrEmpty(jsonTokens))
				{
					plyqorContainer.PrimaryToken = string.Empty;
					plyqorContainer.SecondaryToken = string.Empty;
					plyQorContainers.Add(plyqorContainer);
					continue;
				}

				var tokens = JsonSerializer.Deserialize<List<string>>(jsonTokens);

				plyqorContainer.PrimaryToken = tokens != null && tokens.Count > 0 && !string.IsNullOrEmpty(tokens[0]) ? tokens[0] : string.Empty;
				plyqorContainer.SecondaryToken = tokens != null && tokens.Count > 1 && !string.IsNullOrEmpty(tokens[1]) ? tokens[1] : string.Empty;

				plyQorContainers.Add(plyqorContainer);
			}

			return plyQorContainers;
		}
	}
}
