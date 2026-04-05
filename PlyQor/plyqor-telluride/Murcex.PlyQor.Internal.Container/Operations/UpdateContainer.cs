using Murcex.PlyQor.Internal.Container.Model;
using Murcex.PlyQor.Internal.Container.Storage;

namespace Murcex.PlyQor.Internal.Container.Operations
{
	public class UpdateContainer
	{
		public static bool Execute(PlyQorContainer container)
		{
			// download containers
			var existingContainers = DownloadContainers.Execute();

			// update container
			var existingContainer = existingContainers.FirstOrDefault(c => c.Name.Equals(container.Name, StringComparison.OrdinalIgnoreCase));
			if (existingContainer is not null)
			{
				// Update properties except Name
				foreach (var prop in typeof(PlyQorContainer).GetProperties())
				{
					if (prop.Name.Equals("Name", StringComparison.OrdinalIgnoreCase)) continue;

					var newValue = prop.GetValue(container);
					prop.SetValue(existingContainer, newValue);
				}
			}

			// upload updated containers
			var uploadContainers = new UploadContainers();

			uploadContainers.Execute(existingContainers);

			return true;
		}
	}
}
