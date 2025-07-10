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
			string data = string.Empty;

			try
			{
				using (var connection = new SqlConnection(Configuration.DatabaseConnection))
				{
					var cmd = new SqlCommand(Configuration.SelectStoredProcedure, connection);

					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue(Configuration.ParameterId, Configuration.ContainersId);

					cmd.CommandTimeout = 0;

					connection.Open();

					var reader = cmd.ExecuteReader();
					while (reader.Read())
					{
						data = (string)reader[Configuration.ParameterData];
					}

					return data;
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"SelectContainerConfig Exception: {ex}");
			}
		}

		/// <summary>
		/// Create a local copy of the PlyQor container config from the serialized Json string.
		/// </summary>
		private static List<PlyQorContainer> CreateContainerConfig(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return new List<PlyQorContainer>();
			}

			var containers_config = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(input);

			List<PlyQorContainer> plyQorContainers = new List<PlyQorContainer>();
			if (containers_config != null)
			{
				foreach (var container in containers_config)
				{
					var plyqorContainer = new PlyQorContainer();

					plyqorContainer.Name = container.Key;

					var retention = string.Empty;

					plyqorContainer.Retention = int.Parse(container.Value["Retention"]);

					var tokens = JsonSerializer.Deserialize<List<string>>(container.Value["Tokens"]);

					plyqorContainer.PrimaryToken = tokens != null && tokens.Count > 0 && !string.IsNullOrEmpty(tokens[0]) ? tokens[0] : "null";

					plyqorContainer.SecondaryToken = tokens != null && tokens.Count > 1 && !string.IsNullOrEmpty(tokens[1]) ? tokens[1] : "null";

					plyQorContainers.Add(plyqorContainer);
				}
			}

			return plyQorContainers;
		}
	}
}
