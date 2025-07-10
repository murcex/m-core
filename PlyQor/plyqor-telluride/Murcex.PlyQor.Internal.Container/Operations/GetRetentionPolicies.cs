using Murcex.PlyQor.Internal.Container.Storage;

namespace Murcex.PlyQor.Internal.Container.Operations
{
	public class GetRetentionPolicies
	{
		public static Dictionary<string, int> Execute()
		{
			var containers = DownloadContainers.Execute();

			Dictionary<string, int> result = new();
			foreach (var container in containers)
			{
				if (container.Retention > 0)
				{
					result.Add(container.Name, container.Retention);
				}
			}

			return result;
		}
	}
}
