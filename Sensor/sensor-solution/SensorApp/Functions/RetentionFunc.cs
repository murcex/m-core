namespace SensorApp
{
    using System;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Sensor;
    using Sensor.Function.Core;
    using KirokuG2;

    public static class RetentionFunc
    {
        [FunctionName("Sensor-Retention")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            //try
            //{
            //    Setup.Execute();

            //    using (var klog = KManager.NewInstance("Sensor-Retention"))
            //    {
            //        var result = SensorManager.Retention(klog);
            //    }
            //}
            //catch (Exception ex)
            //{
            //   KManager.Critical($"Sensor Retention Exception: {ex}");
            //}
        }
    }
}
