using Murcex.Vyudro.Internal.Utilities.IO.Internal;

namespace Murcex.Vyudro.Internal.Utilities.IO
{
	public static class FileManager
	{
		/// <summary>
		/// Sets the options for the file manager.
		/// </summary>
		/// <param name="options">The options to be set.</param>
		/// <returns>True if the options are successfully set, otherwise false.</returns>
		public static bool SetOptions(FileManagerOptions options)
		{
			return FileOperator.SetOptions(options);
		}

		/// <summary>
		/// Checks if a file exists at the specified target path.
		/// </summary>
		/// <param name="target">The target path of the file.</param>
		/// <param name="root">Indicates whether the target path is relative to the root directory.</param>
		/// <returns>True if the file exists, otherwise false.</returns>
		public static bool FileExists(string target, bool root = false)
		{
			target = WithRootDirectory(target, root);

			return FileOperator.FileExists(target);
		}

		/// <summary>
		/// Reads the contents of a file with the specified target path.
		/// </summary>
		/// <param name="target">The target path of the file.</param>
		/// <param name="root">Indicates whether the target path is relative to the root directory.</param>
		/// <returns>A list of strings representing the contents of the file.</returns>
		public static List<string> ReadFile(string target, bool root = false)
		{
			target = WithRootDirectory(target, root);

			return FileOperator.ReadFile(target);
		}

		/// <summary>
		/// Creates a file with the specified target path and contents.
		/// </summary>
		/// <param name="target">The target path of the file.</param>
		/// <param name="contents">The contents of the file.</param>
		/// <param name="root">Indicates whether the target path is relative to the root directory.</param>
		/// <returns>True if the file is successfully created, otherwise false.</returns>
		public static bool CreateFile(string target, List<string> contents, bool root = false)
		{
			target = WithRootDirectory(target, root);

			return FileOperator.CreateFile(target, contents);
		}

		/// <summary>
		/// Updates the file with the specified target path and contents.
		/// </summary>
		/// <param name="target">The target path of the file.</param>
		/// <param name="contents">The updated contents of the file.</param>
		/// <param name="root">Indicates whether the target path is relative to the root directory.</param>
		/// <returns>True if the file is successfully updated, otherwise false.</returns>
		public static bool UpdateFile(string target, List<string> contents, bool root = false)
		{
			target = WithRootDirectory(target, root);

			return FileOperator.UpdateFile(target, contents);
		}

		/// <summary>
		/// Deletes the file with the specified target path.
		/// </summary>
		/// <param name="target">The target path of the file.</param>
		/// <param name="root">Indicates whether the target path is relative to the root directory.</param>
		/// <returns>True if the file is successfully deleted, otherwise false.</returns>
		public static bool DeleteFile(string target, bool root = false)
		{
			target = WithRootDirectory(target, root);

			return FileOperator.DeleteFile(target);
		}

		private static string WithRootDirectory(string target, bool root)
		{
			if (root)
			{
				return target;
			}
			else
			{
				if (FileOperator.HasRootDirectory)
				{
					return Path.Combine(FileOperator.RootDirectory, target);
				}
				else
				{
					return target;
				}
			}
		}
	}
}
