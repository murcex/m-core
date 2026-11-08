namespace SensorApp
{
    using KirokuG2;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using Sensor;
    using System;

    public class RetentionFunc
    {
        [Function("Sensor-Retention")]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
        {
            try
            {
                using (var klog = KManager.NewInstance("Sensor-Retention"))
                {
                    var result = SensorManager.Retention(klog);
                }
            }
            catch (Exception ex)
            {
                KManager.Critical($"Sensor Retention Exception: {ex}");
            }
        }
    }
}
