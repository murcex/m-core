using Crane.Internal.Engine.Interface;
using Crane.Internal.Engine.Model;
using Crane.Internal.Engine.Task.SQLDatabaseDeployment;
using Crane.Internal.Engine.Task.SQLDatabaseDeployment.Internal;

namespace Crane.Internal.Engine.Components
{
	public class CraneTaskManager : ICraneTaskManager
	{
		public (bool result, string message) Execute(ICraneLogger logger, ICraneConsole console, Dictionary<string, Dictionary<string, string>> craneTask)
		{
			// get crane task type
			string? type;
			if (craneTask.TryGetValue("task", out var scriptHeader))
			{
				type = scriptHeader["type"];

				if (string.IsNullOrEmpty(type))
				{
					logger.Error($"crane_error=crane_task_type_value_empty");
					throw new CraneException();
				}
			}
			else
			{
				logger.Error($"crane_error=crane_task_type_key_empty");
				throw new CraneException();
			}

			// ---
			// match and execute crane task type with ICraneTaskType
			// ---

			if (string.Equals("deploy-sql-database", type, StringComparison.OrdinalIgnoreCase))
			{

				return new SQLDatabaseDeploymentType().Execute(logger, console, craneTask, new Dictionary<string, object>()
				{
					{ "sql_access", new SQLAccess() }
				});
			}

			logger.Error($"crane_error=crane_task_type_no_match,type={type}");
			throw new CraneException();
		}
	}
}
