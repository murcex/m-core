using Implements.Configuration;

namespace Crane.Internal.Test.Core
{
	public class Setup
	{
		// get config.ini
		private static string root;

		public static string CraneTestRoot => root;

		private static List<string> core = new()
		{
			"external",
			"failure",
			"local",
			"types"
		};

		private static List<string> types = new()
		{
			"sqldatabasedeployment"
		};

		// check dirs
		public static void Basic()
		{
			if (string.IsNullOrWhiteSpace(root))
			{
				root = LoadTestConfig();
			}

			if (!Directory.Exists(root))
			{
				throw new Exception($"root directory doesn't exist");
			}

			foreach (string dir in core)
			{
				var checkDir = Path.Combine(root, dir);

				if (!Directory.Exists(checkDir))
				{
					Directory.CreateDirectory(checkDir);
				}
			}

			foreach (string dir in types)
			{
				var checkDir = Path.Combine(root, "types", dir);

				if (!Directory.Exists(checkDir))
				{
					Directory.CreateDirectory(checkDir);
				}
			}
		}

		private static string LoadTestConfig()
		{
			// local config.ini
			var configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Config.ini");

			// file exist
			if (!File.Exists(configFilePath))
			{
				throw new Exception($"crane test Config.ini not found");
			}

			// read all lines
			var lines = File.ReadAllLines(configFilePath).ToList();

			// de-serial lines to cfg and return
			using ConfigurationUtility configurationUtility = new();
			var config = configurationUtility.Deserialize(lines);

			if (config == null || config.Count == 0)
			{
				throw new Exception($"crane test config dictionary is null or empty");
			}

			if (config.TryGetValue("crane-test", out var craneTestCfg))
			{
				if (craneTestCfg.TryGetValue("root", out var rootDir))
				{
					if (string.IsNullOrEmpty(rootDir))
					{
						throw new Exception($"crane test config root value is null or empty");
					}

					return rootDir;
				}
				else
				{
					throw new Exception($"crane test config doesn't contain root key");
				}
			}
			else
			{
				throw new Exception($"crane test config doesn't contain [crane-test] group");
			}
		}

		public static void CreateTestFile(string file, string contents)
		{
			var root = Directory.GetCurrentDirectory();

			var compoents = file.Split("\\").ToList();

			if (compoents.Count > 1)
			{
				_ = compoents.Remove(compoents[compoents.Count - 1]);

				var testFilepath = string.Empty;
				foreach (var compoent in compoents)
				{
					if (string.IsNullOrEmpty(testFilepath))
					{
						testFilepath = compoent;
					}
					else
					{
						testFilepath = Path.Combine(testFilepath, compoent);
					}

					var testFilePath_1 = Path.Combine(root, testFilepath);

					if (!Directory.Exists(testFilePath_1))
					{
						Directory.CreateDirectory(testFilePath_1);
					}
				}
			}

			file = Path.Combine(root, file);

			File.WriteAllText(file, contents);
		}

		public static void CleanDirectory(string pathway)
		{
			pathway = Path.Combine(Directory.GetCurrentDirectory(), pathway);

			var dir = new DirectoryInfo(pathway);

			foreach (FileInfo file in dir.GetFiles())
			{
				file.Delete();
			}
			foreach (var nestedDir in dir.GetDirectories())
			{
				nestedDir.Delete(true);
			}
		}

		public static void CleanDirectory()
		{
			var pathway = Directory.GetCurrentDirectory();

			var files = new DirectoryInfo(pathway).GetFiles();

			foreach (var file in files)
			{
				if (!string.Equals("Config.ini", file.Name, StringComparison.OrdinalIgnoreCase) && !string.Equals("test-task.ini", file.Name, StringComparison.OrdinalIgnoreCase))
				{
					file.Delete();
				}
			}
		}
	}
}
