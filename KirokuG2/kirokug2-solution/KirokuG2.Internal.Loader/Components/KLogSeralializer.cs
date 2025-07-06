using KirokuG2.Internal.Loader.Interface;

namespace KirokuG2.Internal.Loader.Components
{
	public class KLogSeralializer : IKLogSeralializer
	{
		public KLogSeralializer()
		{

		}

		public Dictionary<string, List<string>> DeseralizalizeLogSet(string rawLog)
		{
			var rawLogAsLines = ConvertToLines(rawLog);

			Dictionary<string, List<string>> logs = new();

			var isFirst = true;
			var isFirstMultiLog = true;
			var tempIndex = string.Empty;
			var tempLogs = new List<string>();
			foreach (var line in rawLogAsLines)
			{
				if (isFirst)
				{
					if (line.StartsWith("##multi-log-start", StringComparison.OrdinalIgnoreCase))
					{
						isFirst = false;

						continue;
					}
					else
					{
						logs["1"] = rawLogAsLines;

						return logs;
					}
					;
				}
				else
				{
					if (line.StartsWith("#index=") || line.StartsWith("##multi-log-end"))
					{
						if (!isFirstMultiLog)
						{
							// add log to log set
							logs[new string(tempIndex)] = new List<string>(tempLogs);

							// clean old temp log
							tempLogs.Clear();
						}
						else
						{
							isFirstMultiLog = false;
						}

						// find index
						tempIndex = GetIndex(line);
					}
					else
					{
						// add line as new document
						tempLogs.Add(line);
					}
				}
			}

			return logs;
		}

		/// <summary>
		/// Convert KLOG from a single string to list of strings for processing
		/// </summary>
		private static List<string> ConvertToLines(string input)
		{
			List<string> lines = new();

			var line = string.Empty;
			using (var string_reader = new StringReader(input))
			{
				while ((line = string_reader.ReadLine()) != null)
				{
					lines.Add(line);
				}
			}

			return lines;
		}

		private static string GetIndex(string line)
		{
			return line.Replace("#index=", "").Trim();
		}
	}
}
