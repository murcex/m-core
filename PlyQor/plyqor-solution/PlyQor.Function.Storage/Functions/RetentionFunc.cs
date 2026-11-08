using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using PlyQor.Engine;

namespace PlyQor.Functions
{
    public class RetentionFunc
    {
        [Function("Retention")]
        public void Run([TimerTrigger("0 0 1 * * *")] TimerInfo myTimer)
        {
            PlyQorManager.Retention();
        }
    }
}
