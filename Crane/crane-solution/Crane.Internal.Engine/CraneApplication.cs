using Crane.Internal.Engine.Interface;
using Crane.Internal.Engine.Model;

namespace Crane.Internal.Engine
{
	public class CraneApplication
	{
		readonly ICraneLogger _logger;
		readonly ICraneFileManager _fileManager;
		readonly ICraneTaskManager _taskManager;
		readonly ICraneConsole _craneConsole;

		public CraneApplication(ICraneLogger logger, ICraneConsole console, ICraneFileManager fileManger, ICraneTaskManager taskManager)
		{
			_logger = logger;
			_fileManager = fileManger;
			_taskManager = taskManager;
			_craneConsole = console;
		}

		public void Execute(string[] args)
		{
			try
			{
				_craneConsole.Starter();

				var taskFile = string.Empty;
				if (args.Length > 0)
				{
					taskFile = args[0];

					if (string.IsNullOrEmpty(taskFile))
					{
						_logger.Error($"crane_error=task_arg_empty");
						throw new CraneException();
					}
				}
				else
				{
					_logger.Error($"crane_error=arg_count_zero");
					throw new CraneException();
				}

				// load Config.ini
				var craneCfg = _fileManager.LoadCraneConfig(_logger);

				// setup logger
				var loggerFilePath = _fileManager.GetCraneLoggerFilePath(_logger, craneCfg);

				_logger.Enable(loggerFilePath);

				// read task cfg
				var taskCfg = _fileManager.LoadCraneTask(_logger, craneCfg, taskFile);

				// console conformation
				if (_fileManager.CheckForConformation(_logger, craneCfg))
				{
					_craneConsole.GeneralConformation(_logger);
				}

				// switch on type and execute task
				var taskResult = _taskManager.Execute(_logger, _craneConsole, taskCfg);

				if (taskResult.result)
				{
					_logger.Success($"crane_task_complete={taskResult.message}");
				}
				else
				{
					_logger.Error($"crane_task_complete={taskResult.message}");
				}

				_craneConsole.Close();
			}
			catch (CraneException craneEx)
			{
				if (!_logger.Enabled())
				{
					_logger.Enable(Directory.GetCurrentDirectory());
				}

				_logger.Error($"general_exception={craneEx}");

				_craneConsole.Close();
			}
			catch (Exception ex)
			{
				if (!_logger.Enabled())
				{
					_logger.Enable(Directory.GetCurrentDirectory());
				}

				_logger.Error($"general_exception={ex}");

				_craneConsole.Close();
			}
		}
	}
}
