using Crane.Internal.Engine.Components;
using Crane.Internal.Engine.Interface;
using Crane.Internal.Test.Core;
using Crane.Internal.Test.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Crane.Internal.Test.Tests.CraneComponents
{
	[TestClass]
	public class TestComponentsAsExternal
	{
		private string craneTestDir;

		[TestInitialize]
		public void Execute()
		{
			Setup.Basic();

			craneTestDir = Path.Combine(Setup.CraneTestRoot, "external");

			Directory.SetCurrentDirectory(craneTestDir);

			// config.ini
			Setup.CreateTestFile("Config.ini", $"[crane]\r\ntask={Path.Combine(craneTestDir, "tasks")}\r\nlog={Path.Combine(craneTestDir, "logs")}");

			// logs\_
			Setup.CreateTestFile("logs\\_", $"");

			// tasks\test-task.ini
			Setup.CreateTestFile("tasks\\test-task.ini", $"[task]\r\nid=external-test-task\r\ntype=test");

			// log-test\_
			Setup.CreateTestFile("log-test\\_", $"");
			Setup.CleanDirectory("log-test");
		}

		/// <summary>
		/// Test basic crane execution with user provided / external task directory
		/// (1) Load "test-task.ini" from external directory
		/// (2) Set Logger to external directory
		/// </summary>
		[TestMethod]
		public void TestCraneFileManager_External()
		{
			List<string> logs = new();
			ICraneLogger logger = new TestLogger(logs);

			CraneFileManager fileManager = new();

			var cfg = fileManager.LoadCraneConfig(logger);

			Assert.IsNotNull(cfg);
			Assert.AreEqual(1, cfg.Count);

			var loggerFilePath = fileManager.GetCraneLoggerFilePath(logger, cfg);

			logger.Enable(loggerFilePath);

			Assert.AreEqual(Path.Combine(Directory.GetCurrentDirectory(), "logs"), loggerFilePath);

			var scriptCfg = fileManager.LoadCraneTask(logger, cfg, "test-task.ini");

			var craneCfgHeader = scriptCfg["task"];

			var craneScriptId = craneCfgHeader["id"];

			var craneScriptType = craneCfgHeader["type"];

			Assert.AreEqual("external-test-task", craneScriptId);
			Assert.AreEqual("test", craneScriptType);
		}

		/// <summary>
		/// Test that the logger will use the current executing directory when enabled/forced without a user provided / external directory
		/// * This would occur in the event the logger throw exception before the log path could be aquired from the Config.ini
		/// </summary>
		[TestMethod]
		public void TestLogger_Off_External()
		{
			var testLogPathway = Path.Combine(Directory.GetCurrentDirectory(), "log-test");
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
			var logFile = afterFiles[0].FullName;
			var logContents = File.ReadAllLines(logFile);

			var infoCheck = logContents.Any(x => x.Contains(infoId));
			var errorCheck = logContents.Any(y => y.Contains(errorId));

			Assert.AreEqual(0, beforeFiles.Length);
			Assert.AreEqual(1, afterFiles.Length);
			Assert.IsTrue(status);
			Assert.IsTrue(infoCheck);
			Assert.IsTrue(errorCheck);
		}

		/// <summary>
		/// Test that the logger will use the user provided / external directory when provided
		/// * This will only occur if the log=<directory> is provided in the Config.ini
		/// </summary>
		[TestMethod]
		public void TestLogger_On_External()
		{
			var testLogPathway = Path.Combine(Directory.GetCurrentDirectory(), "log-test");
			Setup.CleanDirectory(testLogPathway);
			var dir = new DirectoryInfo(testLogPathway);

			ICraneLogger logger = new CraneLogger(testLogPathway);

			var infoId = Guid.NewGuid().ToString();
			var errorId = Guid.NewGuid().ToString();

			logger.Info(infoId);
			logger.Error(errorId);

			var status = logger.Enabled();

			var afterFiles = dir.GetFiles();
			var logFile = afterFiles[0].FullName;
			var logContents = File.ReadAllLines(logFile);

			var infoCheck = logContents.Any(x => x.Contains(infoId));
			var errorCheck = logContents.Any(y => y.Contains(errorId));

			Assert.AreEqual(1, afterFiles.Length);
			Assert.IsTrue(status);
			Assert.IsTrue(infoCheck);
			Assert.IsTrue(errorCheck);
		}
	}
}
