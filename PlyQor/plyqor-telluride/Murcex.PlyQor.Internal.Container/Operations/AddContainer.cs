using Murcex.PlyQor.Internal.Container.Model;
using Murcex.PlyQor.Internal.Container.Storage;
using System.Text.RegularExpressions;

namespace Murcex.PlyQor.Internal.Container.Operations
{
	public class AddContainer
	{
		const string namePattern = @"^[a-zA-Z0-9_-]+$";

		public static bool Execute(PlyQorContainer container)
		{
			// Validate container name
			if (string.IsNullOrWhiteSpace(container.Name))
			{
				throw new ArgumentException("Container name cannot be null, empty, or whitespace.", nameof(container.Name));
			}

			if (container.Name.Length > 30)
			{
				throw new ArgumentException("Container name cannot exceed 30 characters.", nameof(container.Name));
			}
;
			if (!Regex.IsMatch(container.Name, namePattern))
			{
				throw new ArgumentException("Container name can only contain letters (a-z, A-Z), numbers (0-9), underscores (_), and hyphens (-).", nameof(container.Name));
			}

			// Retrieve existing containers
			var containers = DownloadContainers.Execute();

			// Check for duplicate container name (case-insensitive)
			if (containers.Any(c => string.Equals(c.Name, container.Name, StringComparison.OrdinalIgnoreCase)))
			{
				throw new ArgumentException($"A container named '{container.Name}' already exists.", nameof(container.Name));
			}

			// Add and persist the new container
			containers.Add(container);

			new UploadContainers().Execute(containers);

			return true;
		}
	}
}
