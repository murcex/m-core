using KirokuG2;
using Murcex.PlyQor.Internal.Retention.Core;
using Murcex.PlyQor.Internal.Retention.Operations;

namespace Murcex.PlyQor.Internal.Retention
{
	public class ContainerRetentionManager
	{
		public static bool Initialize(Dictionary<string, Dictionary<string, string>> config)
		{
			return Initializer.Execute(config);
		}

		public static void Execute(IKLog klog)
		{
			var globalOperationCount = 0;
			var redlineCount = 0;

			klog.Metric("Container-Count", Configuration.Containers.Count);

			foreach (var container in Configuration.Containers)
			{
				var containerName = container.Key;

				// counters
				var dataOpPassCounter = 0;
				var dataOpFailCounter = 0;
				var tagOpPassCounter = 0;
				var tagOpFailCounter = 0;
				var dataDeleteCounter = 0;
				var tagDeleteCounter = 0;

				var ids = GetId.Execute(containerName);

				if (ids.Count == Configuration.Top)
				{
					redlineCount++;
					klog.Error($"Redline: {containerName}");
				}

				foreach (var id in ids)
				{
					var dataDeleteResult = DeleteId.Execute(containerName, id, true);

					var tagDeleteResult = DeleteId.Execute(containerName, id, false);

					if (dataDeleteResult.result)
					{
						dataDeleteCounter = +dataDeleteResult.recordCount;
						dataOpPassCounter++;
					}
					else
					{
						dataOpFailCounter++;
					}

					if (tagDeleteResult.result)
					{
						tagDeleteCounter = +tagDeleteResult.recordCount;
						tagOpPassCounter++;
					}
					else
					{
						tagOpFailCounter++;
					}
				}

				klog.Trace($"Container: {containerName}");
				klog.Trace($"Records: {ids.Count}");
				klog.Trace($"Data Operation Pass: {dataOpPassCounter}");
				klog.Trace($"Data Operation Fail: {dataOpFailCounter}");
				klog.Trace($"Tag Operation Pass: {tagOpPassCounter}");
				klog.Trace($"Tag Operation Fail: {tagOpFailCounter}");
				klog.Trace($"Data Delete Count: {dataDeleteCounter}");
				klog.Trace($"Tag Delete Count: {tagDeleteCounter}");

				globalOperationCount += ids.Count;
			}

			if (redlineCount > 0)
			{
				klog.Error($"Redline: {redlineCount}");
			}

			klog.Metric("Retention-Operations", globalOperationCount);
			klog.Metric("Retention-Redline", redlineCount);
		}
	}
}
