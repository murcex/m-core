using Crane.Internal.Engine.Interface;

namespace Crane.Internal.Engine.Components
{
	public class CraneRedactor : ICraneRedactor
	{
		public Dictionary<string, Dictionary<string, string>> Execute(Dictionary<string, string> taskCfg, Dictionary<string, Dictionary<string, string>> taskParameters)
		{
			bool isRedact = false;
			List<string> redactItems = new List<string>();
			if (taskCfg.TryGetValue("crane_redact", out var redact))
			{
				if (string.IsNullOrEmpty(redact))
				{
					isRedact = false;
				}

				redactItems = redact.Split(',').ToList();
				isRedact = true;
			}
			else
			{
				isRedact = false;
			}

			Dictionary<string, Dictionary<string, string>> consoleClone = new();
			foreach (var group in taskParameters)
			{
				Dictionary<string, string> groupClone = new();
				foreach (var item in group.Value)
				{
					if (isRedact)
					{
						if (redactItems.Any(x => string.Equals(x, item.Key)))
						{
							groupClone.Add(item.Key, "*redacted*");

							continue;
						}
					}
					groupClone.Add(item.Key, item.Value);
				}
				consoleClone.Add(group.Key, groupClone);
			}

			return consoleClone;
		}
	}
}
