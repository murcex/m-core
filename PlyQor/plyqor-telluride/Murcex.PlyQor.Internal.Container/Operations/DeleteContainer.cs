using Murcex.PlyQor.Internal.Container.Model;
using Murcex.PlyQor.Internal.Container.Storage;

namespace Murcex.PlyQor.Internal.Container.Operations
{
	public class DeleteContainer
	{
		public static bool Execute(PlyQorContainer container)
		{
			// download containers
			var existingContainers = DownloadContainers.Execute();

			// find and remove container
			var containerToRemove = existingContainers.FirstOrDefault(c => c.Name.Equals(container.Name, StringComparison.OrdinalIgnoreCase));
			if (containerToRemove is not null)
			{
				existingContainers.Remove(containerToRemove);
			}
			else
			{
				return false; // container not found
			}

			// upload updated containers
			var uploadContainers = new UploadContainers();

			uploadContainers.Execute(existingContainers);

			return true;
		}
	}
}
