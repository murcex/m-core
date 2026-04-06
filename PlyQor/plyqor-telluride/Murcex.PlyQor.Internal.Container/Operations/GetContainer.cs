using Murcex.PlyQor.Internal.Container.Model;
using Murcex.PlyQor.Internal.Container.Storage;

namespace Murcex.PlyQor.Internal.Container.Operations
{
	public class GetContainer
	{
		public static PlyQorContainer Execute(string containerName)
		{
			if (string.IsNullOrWhiteSpace(containerName))
			{
				throw new ArgumentException("Container name must not be null or empty.", nameof(containerName));
			}

			var containerConfigs = DownloadContainers.Execute();

			var container = containerConfigs?.FirstOrDefault(c => string.Equals(c.Name, containerName, StringComparison.OrdinalIgnoreCase));

			return container ?? new PlyQorContainer();
		}
	}
}
