using Murcex.PlyQor.Internal.Container.Storage;

namespace Murcex.PlyQor.Internal.Container.Operations
{
	public class GetRetentionPolicies
	{
		public static Dictionary<string, int> Execute()
		{
			var containers = DownloadContainers.Execute();

			return containers
			.Where(container => container.Retention > 0)
			.ToDictionary(container => container.Name, container => container.Retention);
		}
	}
}
