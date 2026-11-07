namespace KirokuG2.Injektr.Functions
{
	using Microsoft.Azure.Functions.Worker;
	using Microsoft.Extensions.Logging;
	using System;

	public class AuditFunc
	{
		private readonly ILogger<AuditFunc> logger;

		public AuditFunc(ILogger<AuditFunc> logger)
		{
			this.logger = logger;
		}

		[Function("Kiroku-Audit")]
		public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, FunctionContext functionContext)
		{
			try
			{
				using (var klog = KManager.NewInstance(functionContext.FunctionDefinition.Name))
				{
					try
					{
						// test Info
						klog.Info("testing info");

						// test metric (double)
						klog.Metric("test metric", 99.99);

						// test block
						using (var block = klog.NewBlock("TestBlock"))
						{
							// test getting block tag
							klog.Info($"doing stuff inside the block {block.Tag}");
						}

						// test nested klog
						TestMethod(klog);

					}
					catch (Exception ex)
					{
						klog.Error(ex.ToString());
					}
				}
			}
			catch (Exception ex)
			{
				KManager.Critical($"setup failed: {ex}");
			}
		}

		private static void TestMethod(IKLog kLog)
		{
			kLog.Info("Test Method Info");
		}
	}
}
