namespace KirokuG2.Processor.Functions
{
	using KirokuG2.Processor.Core;
	using Microsoft.Azure.Functions.Worker;
	using Microsoft.Data.SqlClient;
	using Microsoft.Extensions.Logging;
	using System;
	using System.Collections.Generic;

	public class MaintenanceFunc
	{
		private readonly ILogger<MaintenanceFunc> logger;

		private static readonly List<string> tables = new List<string>()
		{
			"Activation",
			"Block",
			"Critical",
			"Error",
			"Instance",
			"Metric",
			"Quarantine"
		};

		public MaintenanceFunc(ILogger<MaintenanceFunc> logger)
		{
			this.logger = logger;
		}

		[Function("Kiroku-Maintenance")]
		public void Run([TimerTrigger("0 0 12 * * *")] TimerInfo myTimer, FunctionContext functionContext)
		{
			using (var klog = KManager.NewInstance(functionContext.FunctionDefinition.Name))
			{
				try
				{
					//foreach list execute sql query
					foreach (var table in tables)
					{
						using (var block = klog.NewBlock($"{table}Retention"))
						{
							// sql query
							var query = $"DELETE FROM [tbl_KirokuG2_{table}] WHERE [dt_session] < DATEADD(day,-7,GETDATE())";

							using (var connection = new SqlConnection(Configuration.Database))
							{
								connection.Open();

								using (var command = new SqlCommand(query, connection))
								{
									command.CommandTimeout = 0;

									var reader = command.ExecuteReader();

									var recordCount = reader.RecordsAffected;

									klog.Info($"{block.Tag}@{table}={recordCount}");

									while (reader.Read())
									{ }
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					klog.Error(ex.ToString());
				}
			}
		}
	}
}
