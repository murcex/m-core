using System.Reflection;

namespace Murcex.Vyudro.Module.Client.Managers
{
	public class ResourceManager
	{
		public static Dictionary<string, string> LoadFiles(string folder, bool image = false)
		{
			var files = new Dictionary<string, string>();

			// get all files inside Base Directory\Resources\<Folder>\*.*
			var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var parentDir = Directory.GetParent(currentDir).FullName;

			var filesDir = Path.Combine(parentDir, "Resources", folder);
			if (Directory.Exists(filesDir))
			{
				var filePaths = Directory.GetFiles(filesDir, "*.*", SearchOption.TopDirectoryOnly);

				// foreach file in filePaths add to files as file name, string of contents
				foreach (var filePath in filePaths)
				{
					var fileName = Path.GetFileName(filePath);
					string fileContent;

					if (image)
					{
						byte[] fileBytes = File.ReadAllBytes(filePath);

						// Determine MIME type from file extension
						string extension = Path.GetExtension(filePath).ToLowerInvariant();
						string mimeType = extension switch
						{
							".png" => "image/png",
							".jpg" => "image/jpeg",
							".jpeg" => "image/jpeg",
							".gif" => "image/gif",
							".bmp" => "image/bmp",
							".svg" => "image/svg+xml",
							_ => "application/octet-stream"
						};

						string base64String = Convert.ToBase64String(fileBytes);
						fileContent = $"data:{mimeType};base64,{base64String}";
					}
					else
					{
						fileContent = File.ReadAllText(filePath); // for HTML or text-based files
					}

					files[fileName] = fileContent;
				}
			}

			return files;
		}
	}
}
