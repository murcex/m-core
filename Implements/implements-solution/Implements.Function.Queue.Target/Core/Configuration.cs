using Implements.Function.Queue.Target.Components;
using Implements.Module.Queue;
using System.Collections.Generic;

namespace Implements.Function.Queue.Target.Core
{
    public class Configuration
    {
        private static QueueManager<string> _queue;

		private static string _database;

		private static string _token;

        public static QueueManager<string> Queue => _queue;

		public static string Database => _database;

		public static string Token => _token;

		public static bool Load(Dictionary<string, string> configuration)
		{
			foreach (var kvp in configuration)
			{
				Sort(kvp.Key, kvp.Value);
			}

			PostLoad();

			return true;
		}

		private static string Sort(string key, string value) => key.ToUpper() switch
		{
			"DATABASE" => _database = value,
			"TOKEN" => _token = value,
			_ => null,
		};

		private static bool PostLoad()
		{
            _queue = new QueueManager<string>(2, 10000, QueueProcessor.Execute, QueueProcessor.Logger);

			return true;
		}
	}
}
