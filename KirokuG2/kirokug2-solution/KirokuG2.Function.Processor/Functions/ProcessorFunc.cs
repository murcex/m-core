namespace KirokuG2.Processor.Functions
{
	using KirokuG2.Loader;
	using Microsoft.Azure.Functions.Worker;
	using Microsoft.Extensions.Logging;
	using System;

	public class ProcessorFunc
	{
		private readonly ILogger<ProcessorFunc> logger;

		public ProcessorFunc(ILogger<ProcessorFunc> logger)
		{
			this.logger = logger;
		}

		[Function("Kiroku-Processor")]
		public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, FunctionContext functionContext)
		{
			using (var klog = KManager.NewInstance(functionContext.FunctionDefinition.Name))
			{
				try
				{
					KLoaderManager.ProcessLogs(klog);
				}
				catch (Exception ex)
				{
					klog.Error($"ProcessorLogs EXCEPTION: {ex}");
				}
			}
		}
	}
}
