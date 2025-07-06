using Crane.Internal.Engine.Interface;
using Crane.Internal.Engine.Model;
using Crane.Internal.Engine.Task.SQLDatabaseDeployment.Internal;

namespace Crane.Internal.Engine.Task.SQLDatabaseDeployment
{
	public class SQLDatabaseDeploymentType : ICraneTaskType
	{
		public (bool result, string message) Execute(ICraneLogger logger, ICraneConsole console, Dictionary<string, Dictionary<string, string>> collection, Dictionary<string, object> parameters)
		{
			logger.Info($"crane_operation=sql-database-deployment");

			var database = string.Empty;
			var account = string.Empty;
			var login = string.Empty;
			var key = string.Empty;
			var storage = string.Empty;

			// get all the values from cfg
			if (collection == null)
			{
				logger.Error($"invalid_parameter=collection_null");
				throw new CraneException();
			}
			if (collection.Count == 0)
			{
				logger.Error($"invalid_parameter=collection_empty");
				throw new CraneException();
			}

			var taskCfg = collection["task"];

			if (taskCfg == null)
			{
				logger.Error($"invalid_parameter=task_config_null");
				throw new CraneException();
			}
			if (taskCfg.Count == 0)
			{
				logger.Error($"invalid_parameter=task_config_empty");
				throw new CraneException();
			}

			if (parameters == null)
			{
				logger.Error($"invalid_parameter=task_parameter_empty");
				throw new CraneException();
			}

			ISQLAccess sqlAccess;
			if (parameters.TryGetValue("sql_access", out var sqlAccessOjb))
			{
				sqlAccess = (ISQLAccess)sqlAccessOjb;
			}
			else
			{
				logger.Error($"invalid_parameter=sql_access_null");
				throw new CraneException();
			}

			string? local;
			// local
			if (taskCfg.TryGetValue("local", out local))
			{
				if (string.IsNullOrEmpty(local))
				{
					local = "true";
				}
			}
			else
			{
				local = "true";
			}
			var isLocal = string.Equals(bool.TrueString, local, StringComparison.OrdinalIgnoreCase);

			// database
			if (taskCfg.TryGetValue("database", out database))
			{
				if (string.IsNullOrEmpty(database))
				{
					logger.Error($"invalid_parameter=database");
					throw new CraneException();
				}
			}
			else
			{
				logger.Error($"invalid_parameter=database");
				throw new CraneException();
			}

			// account
			if (taskCfg.TryGetValue("account", out account))
			{
				if (!isLocal && string.IsNullOrEmpty(account))
				{
					throw new CraneException();
				}
			}
			else
			{
				if (!isLocal)
				{
					logger.Error($"invalid_parameter=account");
					throw new CraneException();
				}
			}

			// login
			if (taskCfg.TryGetValue("login", out login))
			{
				if (!isLocal && string.IsNullOrEmpty(login))
				{
					throw new CraneException();
				}
			}
			else
			{
				if (!isLocal)
				{
					logger.Error($"invalid_parameter=login");
					throw new CraneException();
				}
			}

			// key
			if (taskCfg.TryGetValue("key", out key))
			{
				if (!isLocal && string.IsNullOrEmpty(key))
				{
					throw new CraneException();
				}
			}
			else
			{
				if (!isLocal)
				{
					logger.Error($"invalid_parameter=key");
					throw new CraneException();
				}
			}

			// storage
			if (taskCfg.TryGetValue("storage", out storage))
			{
				if (!isLocal && string.IsNullOrEmpty(storage))
				{
					throw new CraneException();
				}
			}
			else
			{
				logger.Error($"invalid_parameter=storage");
				throw new CraneException();
			}

			// build connection string
			string? connectionString;
			if (isLocal)
			{
				// local conn
				connectionString = $"Integrated Security=SSPI; Persist Security Info=False; Initial Catalog={database}; Data Source=localhost; Encrypt=False;";
			}
			else
			{
				// azure conn
				connectionString = $"Server=tcp:{account},1433; Initial Catalog={database}; Persist Security Info=False; User ID={login}; Password={key}; MultipleActiveResultSets=False;";
			}

			if (isLocal)
			{
				logger.Info($"type=local;database={database}");
			}
			else
			{
				logger.Info($"type=remote;account={account};database={database};login={login}");
			}

			// set sql credentials
			sqlAccess.SetCredentials(connectionString);

			int success = 0;
			int error = 0;

			console.TaskConfirmation(logger, collection);

			// foreach enum type
			foreach (var sqlObjectType in Enum.GetNames(typeof(SqlObjectType)))
			{
				try
				{
					var directory = Path.Combine(storage, sqlObjectType);

					if (!Directory.Exists(directory))
					{
						logger.Info($"sql_object_missing={sqlObjectType}");
						continue;
					}

					string[] fileEntries = Directory.GetFiles(directory, "*.sql");

					logger.Info($"executing_sql_object={sqlObjectType};count={fileEntries.Length}");

					foreach (var fileName in fileEntries)
					{
						try
						{
							var sqlScript = File.ReadAllText(fileName);

							var result = sqlAccess.Execute(sqlScript);

							if (result.result)
							{
								// console green
								logger.Info($"file={Path.GetFileName(fileName)};result={result.code};message={result.message}");
								success++;
							}
							else
							{
								// console red
								logger.Error($"file={Path.GetFileName(fileName)};result={result.code};message={result.message}");
								error++;
							}
						}
						catch (Exception e)
						{
							logger.Error($"crane_task_exception=read_execute_payload_exception,message={e}");
							error++;
						}
					}
				}
				catch (Exception e)
				{
					logger.Error($"crane_task_exception=read_directory_exception,message={e}");
					error++;
				}
			}

			logger.Info($"total={success + error};success={success};error={error}");

			bool taskResult;
			string taskMessage;
			if (error > 0)
			{
				taskResult = false;
				taskMessage = $"{error} ERRORS DETECTED";
			}
			else
			{
				taskResult = true;
				taskMessage = "SUCCESS";
			}

			return (taskResult, taskMessage);
		}
	}
}
