using Murcex.PlyQor.Internal.Container.Model;
using Murcex.PlyQor.Internal.Container.Storage;

namespace Murcex.PlyQor.Internal.Container.Operations
{
	public class GetContainer
	{
		public static PlyQorContainer Execute(string containerName)
		{
			var containerConfigs = DownloadContainers.Execute();

			var container = containerConfigs.FirstOrDefault(c => c.Name == containerName);

			if (container == null)
			{
				container = new PlyQorContainer();
			}

			return container;
		}
	}
}
