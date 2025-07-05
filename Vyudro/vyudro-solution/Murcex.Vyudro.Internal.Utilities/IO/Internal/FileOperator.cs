namespace Murcex.Vyudro.Internal.Utilities.IO.Internal
{
	public class FileOperator
	{
		/// <summary>
		/// The options for the file manager.
		/// </summary>
		private static FileManagerOptions _options = new();

		/// <summary>
		/// Indicates whether the root directory is set.
		/// </summary>
		private static bool _hasRootDirectory;

		/// <summary>
		/// The root directory path.
		/// </summary>
		private static string _rootDirectory = string.Empty;

		/// <summary>
		/// Gets a value indicating whether the root directory is set.
		/// </summary>
		public static bool HasRootDirectory => _hasRootDirectory;

		/// <summary>
		/// Gets the root directory.
		/// </summary>
		public static string RootDirectory => _rootDirectory;

		/// <summary>
		/// Sets the options for the file manager.
		/// </summary>
		/// <param name="options">The FileManagerOptions object containing the options.</param>
		/// <returns>True if the options were set successfully, otherwise false.</returns>
		public static bool SetOptions(FileManagerOptions options)
		{
			_options = options;

			_hasRootDirectory = !string.IsNullOrEmpty(_options.RootDirectory);

			if (_hasRootDirectory)
			{
				_rootDirectory = _options.RootDirectory;
			}

			return true;
		}

		/// <summary>
		/// Checks if a file exists.
		/// </summary>
		/// <param name="file">The path to the file.</param>
		/// <returns>True if the file exists, otherwise false.</returns>
		public static bool FileExists(string file)
		{
			try
			{
				return File.Exists(file);
			}
			catch (Exception ex)
			{
				return ThrowException(ex);
			}
		}

		/// <summary>
		/// Reads the contents of a file.
		/// </summary>
		/// <param name="file">The path to the file.</param>
		/// <returns>A list of strings representing the lines of the file.</returns>
		public static List<string> ReadFile(string file)
		{
			try
			{
				string[]? lines = File.ReadAllLines(file);
				return lines?.ToList() ?? new List<string>();
			}
			catch (Exception ex)
			{
				ThrowException(ex);
				return new List<string>();
			}
		}

		/// <summary>
		/// Creates a new file with the specified contents.
		/// </summary>
		/// <param name="path">The path to the file.</param>
		/// <param name="contents">The contents of the file.</param>
		/// <returns>True if the file was created successfully, otherwise false.</returns>
		public static bool CreateFile(string path, List<string> contents)
		{
			try
			{
				File.WriteAllLines(path, contents);
				return true;
			}
			catch (Exception ex)
			{
				return ThrowException(ex);
			}
		}

		/// <summary>
		/// Appends the specified contents to an existing file.
		/// </summary>
		/// <param name="file">The path to the file.</param>
		/// <param name="contents">The contents to append.</param>
		/// <returns>True if the contents were appended successfully, otherwise false.</returns>
		public static bool UpdateFile(string file, List<string> contents)
		{
			try
			{
				File.AppendAllLines(file, contents);
				return true;
			}
			catch (Exception ex)
			{
				return ThrowException(ex);
			}
		}

		/// <summary>
		/// Deletes a file.
		/// </summary>
		/// <param name="file">The path to the file.</param>
		/// <returns>True if the file was deleted successfully, otherwise false.</returns>
		public static bool DeleteFile(string file)
		{
			try
			{
				File.Delete(file);
				return true;
			}
			catch (Exception ex)
			{
				return ThrowException(ex);
			}
		}

		/// <summary>
		/// Throws an exception or writes an error message based on the options.
		/// </summary>
		/// <param name="ex">The exception to handle.</param>
		/// <returns>True if the exception was thrown, otherwise false.</returns>
		private static bool ThrowException(Exception ex)
		{
			return _options.ThrowException ? throw ex : false;
		}
	}
}
