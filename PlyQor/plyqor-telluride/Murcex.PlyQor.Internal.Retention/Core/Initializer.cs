namespace Murcex.PlyQor.Internal.Retention.Core
{
	internal class Initializer
	{
		public static bool Execute(Dictionary<string, Dictionary<string, string>> config)
		{
			var retentionManagerConfig = config["retention-manager"];

			return Configuration.Load(retentionManagerConfig);
		}
	}
}
