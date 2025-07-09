using Crane.Internal.Engine.Interface;
using Crane.Internal.Engine.Model;
using Implements.Configuration;

namespace Crane.Internal.Engine.Components
{
	public class CraneFileManager : ICraneFileManager
	{
		public Dictionary<string, Dictionary<string, string>> LoadCraneConfig(ICraneLogger logger)
		{
			// local config.ini
			var configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Config.ini");

			// file exist
			if (!File.Exists(configFilePath))
			{
				Dictionary<string, Dictionary<string, string>> defaultCfg = new()
				{
					{ "crane", new Dictionary<string, string>() }
				};

				logger.Info($"crane_config=default");
			}

			// read all lines
			var lines = File.ReadAllLines(configFilePath).ToList();

			// de-serial lines to cfg and return
			using ConfigurationUtility configurationUtility = new();
			return configurationUtility.Deserialize(lines);
		}

		public string GetCraneLoggerFilePath(ICraneLogger logger, Dictionary<string, Dictionary<string, string>> cfg)
		{
			var output = GetCraneConfigurationValue(logger, cfg, "log");

			if (!output.result || string.IsNullOrEmpty(output.craneValue))
			{
				return Directory.GetCurrentDirectory();
			}
			else
			{
				return output.craneValue;
			}
		}

		public bool CheckForConformation(ICraneLogger logger, Dictionary<string, Dictionary<string, string>> cfg)
		{
			var output = GetCraneConfigurationValue(logger, cfg, "confirm");

			if (!output.result || string.IsNullOrEmpty(output.craneValue))
			{
				return true;
			}
			else
			{
				if (bool.TryParse(output.craneValue, out bool result))
				{
					return result;
				}
				else
				{
					return true;
				}
			}
		}

		public Dictionary<string, Dictionary<string, string>> LoadCraneTask(ICraneLogger logger, Dictionary<string, Dictionary<string, string>> cfg, string name)
		{
			// file path
			var output = GetCraneConfigurationValue(logger, cfg, "task");

			var root = Directory.GetCurrentDirectory();
			if (output.result || !string.IsNullOrEmpty(output.craneValue))
			{
				root = output.craneValue;
			}

			// file exist
			var taskFilePath = Path.Combine(root, name);

			if (!File.Exists(taskFilePath))
			{
				logger.Error($"crane_error=task_directory_does_not_exist,task_directory={taskFilePath}");
				throw new CraneException();
			}

			// read all lines
			var lines = File.ReadAllLines(taskFilePath).ToList();

			// de-serial lines to cfg
			using ConfigurationUtility configurationUtility = new();
			var taskCfg = configurationUtility.Deserialize(lines);

			// check type name
			if (taskCfg.TryGetValue("task", out var taskHeader))
			{

				if (taskHeader.TryGetValue("type", out var taskType))
				{
					if (string.IsNullOrEmpty(taskType))
					{
						logger.Error($"crane_error=crane_task_type_value_empty");
						throw new CraneException();
					}
				}
				else
				{
					logger.Error("crane_error=crane_task_type_key_empty");
					throw new CraneException();
				}
			}
			else
			{
				logger.Error($"crane_error=crane_config_index_missing");
				throw new CraneException();
			}

			return taskCfg;
		}

		private (bool result, string craneValue) GetCraneConfigurationValue(ICraneLogger logger, Dictionary<string, Dictionary<string, string>> cfg, string craneKey)
		{
			if (cfg == null)
			{
				logger.Error("crane_error=config_dictionary_null");
				throw new CraneException();
			}

			bool result = false;
			string craneValue = string.Empty;
			if (cfg.TryGetValue("crane", out var craneCfg))
			{
				if (craneCfg == null)
				{
					logger.Error($"crane_error=crane_config_null");
					throw new CraneException();
				}

				result = craneCfg.TryGetValue(craneKey, out craneValue);
			}

			return (result, craneValue);
		}
	}
}
