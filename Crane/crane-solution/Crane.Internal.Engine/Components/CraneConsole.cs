using Crane.Internal.Engine.Interface;
using Crane.Internal.Engine.Model;

namespace Crane.Internal.Engine.Components
{
	public class CraneConsole : ICraneConsole
	{
		readonly ICraneRedactor redactor;

		public CraneConsole(ICraneRedactor radactor)
		{
			this.redactor = radactor;
		}

		public void Starter()
		{
			Console.Clear();

			Console.WriteLine("Starting Crane\r\n");
		}

		public void GeneralConformation(ICraneLogger logger)
		{
			Console.WriteLine($"Continue? Y/N");
			var deployCheck = Console.ReadLine();

			if (!string.Equals("Y", deployCheck, StringComparison.OrdinalIgnoreCase))
			{
				logger.Info($"conformation_check=fail");
				throw new CraneException();
			}

			logger.Info($"conformation_check=pass");
			Console.Clear();
			Console.WriteLine("Executing Crane Task\r\n");
		}

		public void TaskConfirmation(ICraneLogger logger, Dictionary<string, Dictionary<string, string>> collection)
		{
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

			string? confirm;
			if (taskCfg.TryGetValue("crane_confirm", out confirm))
			{
				if (string.IsNullOrEmpty(confirm))
				{
					confirm = "true";
				}
			}
			else
			{
				confirm = "true";
			}
			var isConfirm = string.Equals(bool.TrueString, confirm, StringComparison.OrdinalIgnoreCase);

			if (isConfirm)
			{
				var consoleClone = redactor.Execute(taskCfg, collection);

				Console.WriteLine("Task Configuration:");
				foreach (var group in consoleClone)
				{
					foreach (var item in group.Value)
					{
						Console.WriteLine($"{group.Key};{item.Key}={item.Value}");
					}
				}

				Console.WriteLine($"\r\nContinue? Y/N");
				var deployCheck = Console.ReadLine();

				if (!string.Equals("Y", deployCheck, StringComparison.OrdinalIgnoreCase))
				{
					logger.Error($"task_conformation_check=fail");
					throw new CraneException();
				}

				logger.Info($"task_conformation_check=pass");
				Console.Clear();
				Console.WriteLine("Executing Crane Task\r\n");
			}
			else
			{
				logger.Info("task_comformation_check=skip");
			}
		}

		public void Close()
		{
			Console.WriteLine($"\r\nEnding Crane");

			Environment.Exit(0);
		}
	}
}
