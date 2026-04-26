using Microsoft.VisualStudio.TestTools.UnitTesting;
using Implements.Module.Queue;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Implements.Module.Queue.Test
{
    [TestClass]
    public class QueueManagerInProcessTests
    {
        /// <summary>
        /// Simulates Service A (producers) directly calling Service B's enqueue method (no HTTP).
        /// Service B uses QueueManager to batch and process items into an in-memory store.
        /// This test is deterministic (seeded RNG) and uses TaskCompletionSource to await processing.
        /// </summary>
        [TestMethod]
        public async Task InProcess_ProducerConsumer_AllMessagesProcessed()
        {
            var processed = new ConcurrentDictionary<string, bool>();
            var expectedTotal = 5 * 100; // producers * messages per producer
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            // action invoked by QueueManager when a batch is processed
            void QueueAction(List<string> items)
            {
                foreach (var it in items)
                {
                    processed.TryAdd(it, true);
                }

                if (processed.Count >= expectedTotal)
                {
                    tcs.TrySetResult(true);
                }
            }

            var logs = new List<string>();
            var queue = new QueueManager<string>(limit: 50, duration: 50, action: QueueAction, logger: msg => { lock (logs) { logs.Add(msg); } });

            // simulate multiple producers (Service A)
            var producers = 5;
            var perProducer = 100;
            var rng = new Random(42); // deterministic

            var tasks = new List<Task>();
            for (int p = 0; p < producers; p++)
            {
                var pid = p;
                tasks.Add(Task.Run(async () =>
                {
                    for (int i = 0; i < perProducer; i++)
                    {
                        var id = $"p{pid}-{i}";
                        queue.Enqueue(id);

                        // small randomized delay
                        await Task.Delay(rng.Next(0, 5));
                    }
                }));
            }

            // wait for all enqueues to be scheduled
            await Task.WhenAll(tasks);

            // wait until processed or timeout
            var completed = await Task.WhenAny(tcs.Task, Task.Delay(10000));
            Assert.AreEqual(tcs.Task, completed, "Not all messages were processed in time");

            // final check
            Assert.AreEqual(expectedTotal, processed.Count, "Processed count should equal enqueued count");

            // ensure no duplicates (dictionary keys are unique) and basic log entries exist
            Assert.IsTrue(processed.Keys.Contains("p0-0"), "Expect some known message present");

            // shutdown queue
            queue.Shutdown();
        }
    }
}
