namespace SensorApp
{
    using KirokuG2;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using Sensor;
    using System;

    public class SensorFunc
    {
        [Function("Sensor-Scanner")]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
        {
            try
            {
                using (var klog = KManager.NewInstance("Sensor-Scanner"))
                {
                    try
                    {
                        var result = SensorManager.Execute(klog);
                    }
                    catch (Exception ex)
                    {
                        klog.Error(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                KManager.Critical(ex.ToString());
            }
        }
    }
}
