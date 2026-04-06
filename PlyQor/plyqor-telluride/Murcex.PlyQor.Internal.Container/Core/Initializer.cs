namespace Murcex.PlyQor.Internal.Container.Core
{
	public class Initializer
	{
		public static bool Execute(Dictionary<string, Dictionary<string, string>> config)
		{
			var containerManagerCfg = config["container-manager"];

			return Configuration.Load(containerManagerCfg);
		}

		public static bool Execute(string databaseConnection)
		{
			return Configuration.Load(databaseConnection);
		}
	}
}
