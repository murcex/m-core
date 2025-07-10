using Murcex.PlyQor.Internal.Container;

namespace Murcex.PlyQor.Internal.Retention.Core
{
	public class Configuration
	{
		public static string DatabaseConnection => _databaseConnection;

		public static Dictionary<string, int> Containers => _containers;

		public static int Top => 500;

		private static string _databaseConnection = string.Empty;

		private static Dictionary<string, int> _containers = new();

		public static bool Load(Dictionary<string, string> config)
		{
			foreach (var entry in config)
			{
				switch (entry.Key)
				{
					case "database-access":
						_databaseConnection = config["database-access"];
						break;
					default:
						throw new Exception("Invalid config type");
				}
			}

			return LoadContainers();
		}

		private static bool LoadContainers()
		{
			PlyQorContainerManager.Initialize(_databaseConnection);

			_containers = PlyQorContainerManager.GetContainerRetention();

			return true;
		}
	}
}
