using Implements.Function.Queue.Target.Core;
using Implements.Module.Queue;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Implements.Function.Queue.Target.Components
{
	public class QueueProcessor
	{
        public static void Execute(List<string> items)
		{
			foreach (var item in items)
			{
				try
				{
                    var id = item.ToString();

					var result = SQLStorage.UpdateRecord(id);

					//Console.WriteLine($"Updating {id} => {result}");

					Thread.Sleep(1000);
				}
				catch { }
			}
		}

		public static void Logger(string message)
		{
			var record = QueueLoglizer.Execute(message);

			switch (record.Type)
			{
				case 1:
					Console.WriteLine($"LOG | Timestamp: {record.Timestamp} => Key: {record.Key}");
					break;
				case 2:
					Console.WriteLine($"LOG | Timestamp: {record.Timestamp} => Key: {record.Key}, Value: {record.Value}");
					break;
				case 3:
					Console.WriteLine($"LOG | Timestamp: {record.Timestamp} => Instance: {record.Instance}, Key: {record.Key}, Value: {record.Value}");
					break;
			}
		}
	}
}
