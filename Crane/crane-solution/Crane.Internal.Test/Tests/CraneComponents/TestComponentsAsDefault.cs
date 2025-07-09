using Crane.Internal.Engine.Components;
using Crane.Internal.Engine.Interface;
using Crane.Internal.Test.Core;
using Crane.Internal.Test.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Crane.Internal.Test.Tests.CraneComponents
{
	[TestClass]
	public class TestComponentsAsDefault
	{
		string craneTestDir = Path.Combine(Setup.CraneTestRoot, "local");

		[TestInitialize]
		public void Execute()
		{
			Setup.Basic();

			Directory.SetCurrentDirectory(craneTestDir);

			// local\task.ini
			Setup.CreateTestFile("Config.ini", "");

			// tasks\task.ini
			Setup.CreateTestFile("test-task.ini", $"[task]\r\nid=default-test-task\r\ntype=test");

			Setup.CleanDirectory();
		}

		/// <summary>
		/// Test basic crane execution using default current executing directory
		/// (1) Load "test-task.ini" from local/current directory
		/// (2) Set Logger to local/current directory
		/// </summary>
		[TestMethod]
		public void TestCraneFileManager_Default()
		{
			List<string> logs = new();
			ICraneLogger logger = new TestLogger(logs);

			CraneFileManager fileManager = new();

			var cfg = fileManager.LoadCraneConfig(logger);

			Assert.IsNotNull(cfg);
			Assert.AreEqual(0, cfg.Count);

			var loggerFilePath = fileManager.GetCraneLoggerFilePath(logger, cfg);

			logger.Enable(loggerFilePath);

			Assert.AreEqual(Directory.GetCurrentDirectory(), loggerFilePath);

			var scriptCfg = fileManager.LoadCraneTask(logger, cfg, "test-task.ini");

			var craneCfgHeader = scriptCfg["task"];

			var craneScriptId = craneCfgHeader["id"];

			var craneScriptType = craneCfgHeader["type"];

			Assert.AreEqual("default-test-task", craneScriptId);
			Assert.AreEqual("test", craneScriptType);
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void TestLogger_Off_Default()
		{
			var testLogPathway = Directory.GetCurrentDirectory();
			Setup.CleanDirectory();
			var dir = new DirectoryInfo(testLogPathway);

			ICraneLogger logger = new CraneLogger();

			var infoId = Guid.NewGuid().ToString();
			var errorId = Guid.NewGuid().ToString();

			logger.Info(infoId);
			logger.Error(errorId);

			var beforeFiles = dir.GetFiles();

			logger.Enable(testLogPathway);
			var status = logger.Enabled();

			var afterFiles = dir.GetFiles();
			var logFile = string.Empty;
			foreach (var file in afterFiles)
			{
				if (!string.Equals("Config.ini", file.Name, StringComparison.OrdinalIgnoreCase) && !string.Equals("test-task.ini", file.Name, StringComparison.OrdinalIgnoreCase))
				{
					logFile = file.Name;
					break;
				}
			}

			var logContents = File.ReadAllLines(logFile);

			foreach (var entry in logContents)
			{
				Console.WriteLine($"entry: {entry}");
			}

			var infoCheck = logContents.Any(x => x.Contains(infoId));
			var errorCheck = logContents.Any(y => y.Contains(errorId));

			Assert.AreEqual(2, beforeFiles.Length);
			Assert.AreEqual(3, afterFiles.Length);
			Assert.IsTrue(status, "logger status should be true (enabled)");
			Assert.IsTrue(infoCheck, $"log contents should contain info id {infoId}");
			Assert.IsTrue(errorCheck, $"log contents should container error id {errorId}");
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void TestLogger_On_Default()
		{
			var testLogPathway = Directory.GetCurrentDirectory();
			Setup.CleanDirectory();
			var dir = new DirectoryInfo(testLogPathway);

			ICraneLogger logger = new CraneLogger(testLogPathway);

			var infoId = Guid.NewGuid().ToString();
			var errorId = Guid.NewGuid().ToString();

			logger.Info(infoId);
			logger.Error(errorId);

			var status = logger.Enabled();

			var afterFiles = dir.GetFiles();
			var logFile = string.Empty;
			foreach (var file in afterFiles)
			{
				if (!string.Equals("Config.ini", file.Name, StringComparison.OrdinalIgnoreCase) && !string.Equals("test-task.ini", file.Name, StringComparison.OrdinalIgnoreCase))
				{
					logFile = file.Name;
					break;
				}
			}

			var logContents = File.ReadAllLines(logFile);

			foreach (var entry in logContents)
			{
				Console.WriteLine($"entry: {entry}");
			}

			var infoCheck = logContents.Any(x => x.Contains(infoId));
			var errorCheck = logContents.Any(y => y.Contains(errorId));

			Assert.IsTrue(status, "logger status should be true (enabled)");
			Assert.IsTrue(infoCheck, $"log contents should contain info id {infoId}");
			Assert.IsTrue(errorCheck, $"log contents should container error id {errorId}");
		}
	}
}
