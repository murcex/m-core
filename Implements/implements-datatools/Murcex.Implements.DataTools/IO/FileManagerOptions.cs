using System.Reflection;

namespace Murcex.Implements.DataTools.IO
{
	public class FileManagerOptions
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FileManagerOptions"/> class.
		/// </summary>
		public FileManagerOptions()
		{
			ThrowException = false;
			RootDirectory = string.Empty;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to throw an exception when an error occurs.
		/// </summary>
		public bool ThrowException { get; set; }

		/// <summary>
		/// Gets or sets the root directory.
		/// </summary>
		public string RootDirectory { get; set; }

		/// <summary>
		/// Sets the root directory based on the executing assembly location.
		/// </summary>
		public void GetExecutingAssembly()
		{
			RootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
		}

		/// <summary>
		/// Gets the current directory and sets it as the root directory.
		/// </summary>
		public void GetCurrentDirectory()
		{
			RootDirectory = Directory.GetCurrentDirectory();
		}

		/// <summary>
		/// Sets the root directory to the specified value.
		/// </summary>
		/// <param name="rootDirectory">The root directory.</param>
		public void SetRootDirectory(string rootDirectory)
		{
			RootDirectory = rootDirectory;
		}
	}
}
