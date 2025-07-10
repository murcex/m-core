namespace Murcex.PlyQor.Internal.Container.Core
{
	public class Configuration
	{
		public static string DatabaseConnection => _databaseConnection;

		public static string UpsertStoredProcedure { get; } = "usp_PlyQor_Data_UpsertSystem";

		public static string SelectStoredProcedure { get; } = "usp_PlyQor_Data_SelectSystem";

		public static string Admin { get; } = "admin";

		public static string Database { get; } = "database";

		public static string ParameterTimeStamp { get; } = "dt_timestamp";

		public static string ParameterId { get; } = "nvc_id";

		public static string ParameterData { get; } = "nvc_data";

		public static string ContainersId { get; } = "CONTAINERS";

		private static string _databaseConnection = string.Empty;

		public static bool Load(Dictionary<string, string> config)
		{
			foreach (var entry in config)
			{
				switch (entry.Key)
				{
					case "connection-string":
						_databaseConnection = config["connection-string"];
						break;
					default:
						throw new Exception("Invalid config type");
				}
			}

			return true;
		}

		public static bool Load(string databaseConnection)
		{
			_databaseConnection = databaseConnection;

			return true;
		}
	}
}
