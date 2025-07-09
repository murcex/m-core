using Crane.Internal.Engine.Task.SQLDatabaseDeployment;
using Crane.Internal.Test.Core;
using Crane.Internal.Test.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Crane.Internal.Test.CraneTypes
{
	[TestClass]
	public class TestSQLDatabaseDeployment
	{
		private string craneTestDir;

		readonly string database = "TestDB";
		readonly string account = "TestAccount";
		readonly string login = "TestUser";
		readonly string key = "TestKey";

		[TestInitialize]
		public void Execute()
		{
			Setup.Basic();

			craneTestDir = Path.Combine(Setup.CraneTestRoot, "types", "sqldatabasedeployment");

			Directory.SetCurrentDirectory(craneTestDir);

			// Database
			Setup.CreateTestFile("Database\\proc\\proc.sql", "testproc");
			Setup.CreateTestFile("Database\\security\\security.sql", "testsecurity");
			Setup.CreateTestFile("Database\\table\\table.sql", "testtable");
			Setup.CreateTestFile("Database\\view\\view.sql", "testview");

			// Database-2
			Setup.CreateTestFile("Database-2\\proc\\proc.sql", "exception-2");
			Setup.CreateTestFile("Database-2\\security\\security.sql", "testsecurity");
			Setup.CreateTestFile("Database-2\\table\\table.sql", "exception-1");
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void TestSQLDatabaseDeploymentType_Remote()
		{
			var local = bool.FalseString;
			var storage = Path.Combine(craneTestDir, "Database");
			var connectionString = $"Server=tcp:{account},1433; Initial Catalog={database}; Persist Security Info=False; User ID={login}; Password={key}; MultipleActiveResultSets=False;";

			var exceptedSqlObj = new List<string>()
			{
				connectionString,
				"testtable",
				"testproc",
				"testview",
				"testsecurity"
			};

			// dict of script
			Dictionary<string, Dictionary<string, string>> cfg = new();
			Dictionary<string, string> jobCfg = new()
			{
				["local"] = local,
				["database"] = database,
				["account"] = account,
				["login"] = login,
				["key"] = key,
				["storage"] = storage
			};
			cfg["task"] = jobCfg;

			var deployedSqlObj = new Dictionary<string, string>();

			Dictionary<string, object> parameters = new()
			{
				{ "sql_access", new TestSQLAccess(deployedSqlObj) }
			};

			var logRecords = new Dictionary<string, string>();

			var logger = new TestLogger();

			var console = new TestConsole();

			SQLDatabaseDeploymentType job = new();

			job.Execute(logger, console, cfg, parameters);

			Assert.IsTrue(exceptedSqlObj.All(x => deployedSqlObj.ContainsValue(x)));
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void TestSQLDatabaseDeploymentType_Local()
		{
			var local = bool.TrueString;
			var storage = Path.Combine(craneTestDir, "Database");
			var connectionString = $"Integrated Security=SSPI; Persist Security Info=False; Initial Catalog={database}; Data Source=localhost; Encrypt=False;";

			var exceptedSqlObj = new List<string>()
			{
				connectionString,
				"testtable",
				"testproc",
				"testview",
				"testsecurity"
			};

			// dict of script
			Dictionary<string, Dictionary<string, string>> cfg = new();
			Dictionary<string, string> jobCfg = new()
			{
				["local"] = local,
				["database"] = database,
				["storage"] = storage
			};
			cfg["task"] = jobCfg;

			var deployedSqlObj = new Dictionary<string, string>();

			Dictionary<string, object> parameters = new()
			{
				{ "sql_access", new TestSQLAccess(deployedSqlObj) }
			};

			var logRecords = new Dictionary<string, string>();

			var logger = new TestLogger();

			var console = new TestConsole();

			SQLDatabaseDeploymentType job = new();

			job.Execute(logger, console, cfg, parameters);

			Assert.IsTrue(exceptedSqlObj.All(x => deployedSqlObj.ContainsValue(x)));
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void TestSQLDatabaseDeploymentType_MissingEntry()
		{
			var test_parameters = new List<(string local, string remove, bool result)>()
			{
				// script cfg
				(string.Empty, "script-null", false),
				(string.Empty, "script-empty", false),

				// job cfg
				(string.Empty, "task-null", false),
				(string.Empty, "task-empty", false),

				// local
				(bool.TrueString, "local", true),
				(bool.TrueString, "database", false),
				(bool.TrueString, "account", true),
				(bool.TrueString, "login", true),
				(bool.TrueString, "key", true),
				(bool.TrueString, "storage", false),

				// remote
				(bool.FalseString, "local", false),
				(bool.FalseString, "database", false),
				(bool.FalseString, "account",  false),
				(bool.FalseString, "login", false),
				(bool.FalseString, "key", false),
				(bool.FalseString, "storage", false)
			};

			foreach (var param in test_parameters)
			{
				var local = param.local;
				var storage = Path.Combine(craneTestDir, "Database");

				var connectionString = string.Empty;
				if (string.Equals(bool.TrueString, local, StringComparison.OrdinalIgnoreCase))
				{
					connectionString = $"Integrated Security=SSPI; Persist Security Info=False; Initial Catalog={database}; Data Source=localhost; Encrypt=False;";
				}
				else
				{
					connectionString = $"Server=tcp:{account},1433; Initial Catalog={database}; Persist Security Info=False; User ID={login}; Password={key}; MultipleActiveResultSets=False;";
				}

				var exceptedSqlObj = new List<string>()
				{
					connectionString,
					"testtable",
					"testproc",
					"testview",
					"testsecurity"
				};

				// dict of script
				Dictionary<string, Dictionary<string, string>> cfg = new();
				Dictionary<string, string> jobCfg = new()
				{
					["local"] = local,
					["database"] = database,
					["account"] = account,
					["login"] = login,
					["key"] = key,
					["storage"] = storage
				};

				if (string.Equals("script-null", param.remove, StringComparison.OrdinalIgnoreCase))
				{
					cfg = null;
				}
				else if (string.Equals("script-empty", param.remove, StringComparison.OrdinalIgnoreCase))
				{
					cfg.Clear();
				}
				else if (string.Equals("tasl-null", param.remove, StringComparison.OrdinalIgnoreCase))
				{
					cfg["task"] = null;
				}
				else if (string.Equals("tasl-empty", param.remove, StringComparison.OrdinalIgnoreCase))
				{
					cfg["task"] = new Dictionary<string, string>();
				}
				else
				{
					jobCfg.Remove(param.remove);

					cfg["task"] = jobCfg;
				}

				var deployedSqlObj = new Dictionary<string, string>();

				Dictionary<string, object> parameters = new()
				{
					{ "sql_access", new TestSQLAccess(deployedSqlObj) }
				};

				var logger = new TestLogger();

				var console = new TestConsole();

				SQLDatabaseDeploymentType job = new();

				var test_result = false;
				try
				{
					job.Execute(logger, console, cfg, parameters);

					if (exceptedSqlObj.All(x => deployedSqlObj.ContainsValue(x)))
					{
						test_result = true;
					}
					else
					{
						test_result = false;
					}
				}
				catch
				{
					test_result = false;
				}

				Assert.AreEqual(test_result, param.result, $"local={param.local}, param={param.remove}, result={param.result}");

				logger.Info("-----");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void TestSQLDatabaseDeploymentType_MissingEntryValue()
		{
			var test_parameters = new List<(string local, string remove, bool result)>()
			{
				// script cfg
				(string.Empty, "script-null", false),
				(string.Empty, "script-empty", false),

				// job cfg
				(string.Empty, "task-null", false),
				(string.Empty, "task-empty", false),

				// local
				(bool.TrueString, "local", true),
				(bool.TrueString, "database", false),
				(bool.TrueString, "account", true),
				(bool.TrueString, "login", true),
				(bool.TrueString, "key", true),
				(bool.TrueString, "storage", false),

				// remote
				(bool.FalseString, "local", false),
				(bool.FalseString, "database", false),
				(bool.FalseString, "account",  false),
				(bool.FalseString, "login", false),
				(bool.FalseString, "key", false),
				(bool.FalseString, "storage", false)
			};

			var logger = new TestLogger();

			var console = new TestConsole();

			foreach (var param in test_parameters)
			{
				logger.Info($"parameter: {param.local} : {param.remove} : {param.result}");

				var local = param.local;
				var storage = Path.Combine(craneTestDir, "Database");

				var connectionString = string.Empty;
				if (string.Equals(bool.TrueString, local, StringComparison.OrdinalIgnoreCase))
				{
					connectionString = $"Integrated Security=SSPI; Persist Security Info=False; Initial Catalog={database}; Data Source=localhost; Encrypt=False;";
				}
				else
				{
					connectionString = $"Server=tcp:{account},1433; Initial Catalog={database}; Persist Security Info=False; User ID={login}; Password={key}; MultipleActiveResultSets=False;";
				}

				var exceptedSqlObj = new List<string>()
				{
					connectionString,
					"testtable",
					"testproc",
					"testview",
					"testsecurity"
				};

				// dict of script
				Dictionary<string, Dictionary<string, string>> cfg = new();
				Dictionary<string, string> jobCfg = new()
				{
					["local"] = local,
					["database"] = database,
					["account"] = account,
					["login"] = login,
					["key"] = key,
					["storage"] = storage
				};

				if (string.Equals("script-null", param.remove, StringComparison.OrdinalIgnoreCase))
				{
					cfg = null;
				}
				else if (string.Equals("script-empty", param.remove, StringComparison.OrdinalIgnoreCase))
				{
					cfg.Clear();
				}
				else if (string.Equals("task-null", param.remove, StringComparison.OrdinalIgnoreCase))
				{
					cfg["task"] = null;
				}
				else if (string.Equals("task-empty", param.remove, StringComparison.OrdinalIgnoreCase))
				{
					cfg["task"] = new Dictionary<string, string>();
				}
				else
				{
					jobCfg[param.remove] = string.Empty;

					cfg["task"] = jobCfg;
				}

				var deployedSqlObj = new Dictionary<string, string>();

				Dictionary<string, object> parameters = new()
				{
					{ "sql_access", new TestSQLAccess(deployedSqlObj) }
				};

				SQLDatabaseDeploymentType job = new();

				var test_result = false;
				try
				{
					job.Execute(logger, console, cfg, parameters);

					if (exceptedSqlObj.All(x => deployedSqlObj.ContainsValue(x)))
					{
						test_result = true;
					}
					else
					{
						test_result = false;
					}
				}
				catch
				{
					test_result = false;
				}

				Assert.AreEqual(test_result, param.result, $"local={param.local}, param={param.remove}, result={param.result}");

				logger.Info("-----");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[TestMethod]
		public void TestSQLDatabaseDeploymentType_Failures()
		{
			var local = bool.FalseString;
			var storage = Path.Combine(craneTestDir, "Database-2");
			var connectionString = $"Server=tcp:{account},1433; Initial Catalog={database}; Persist Security Info=False; User ID={login}; Password={key}; MultipleActiveResultSets=False;";

			var exceptedSqlObj = new List<string>()
			{
				connectionString,
				"exception-1",
				"exception-2",
				"testsecurity"
			};

			// dict of script
			Dictionary<string, Dictionary<string, string>> cfg = new();
			Dictionary<string, string> jobCfg = new()
			{
				["local"] = local,
				["database"] = database,
				["account"] = account,
				["login"] = login,
				["key"] = key,
				["storage"] = storage
			};
			cfg["task"] = jobCfg;

			var deployedSqlObj = new Dictionary<string, string>();

			Dictionary<string, object> parameters = new()
			{
				{ "sql_access", new TestSQLAccess(deployedSqlObj) }
			};

			var logRecords = new Dictionary<string, string>();

			var logger = new TestLogger();

			var console = new TestConsole();

			SQLDatabaseDeploymentType job = new();

			job.Execute(logger, console, cfg, parameters);

			Assert.IsTrue(exceptedSqlObj.All(x => deployedSqlObj.ContainsValue(x)));
		}
	}
}