using Microsoft.VisualStudio.TestTools.UnitTesting;
using Implements.Module.Queue;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System;

namespace Implements.Module.Queue.Test
{
    [TestClass]
    public class QueueManagerAdditionalTests
    {
        private Action<string> CreateLogger(List<string> log)
        {
            return msg =>
            {
                lock (log)
                {
                    log.Add(msg);
                }
            };
        }

        [TestMethod]
        public async Task Shutdown_Prevents_Further_Enqueue_And_Cancels_Pending()
        {
            var logs = new List<string>();
            var tcs = new TaskCompletionSource<bool>();

            void Action(List<string> items)
            {
                tcs.TrySetResult(true);
            }

            var q = new QueueManager<string>(limit: 10, duration: 500, action: Action, logger: CreateLogger(logs));

            q.Enqueue("a");

            // shutdown immediately
            var first = q.Shutdown();
            Assert.IsTrue(first);

            // subsequent shutdown returns false
            Assert.IsFalse(q.Shutdown());

            // further enqueues should be rejected
            Assert.IsFalse(q.Enqueue("b"));

            // wait a short time to see if action fired (it should not)
            var completed = await Task.WhenAny(tcs.Task, Task.Delay(300));
            Assert.AreNotEqual(tcs.Task, completed, "Action should not have executed after shutdown");
        }

        [TestMethod]
        public async Task IsActive_Reflects_Trigger_Lifecycle()
        {
            var logs = new List<string>();
            var tcs = new TaskCompletionSource<bool>();

            void Action(List<string> items)
            {
                // signal that action executed
                tcs.TrySetResult(true);
            }

            var q = new QueueManager<string>(limit: 100, duration: 200, action: Action, logger: CreateLogger(logs));

            Assert.IsFalse(q.IsActive());

            q.Enqueue("x");

            Assert.IsTrue(q.IsActive(), "IsActive should be true immediately after first enqueue starting the delay");

            // wait for processing to complete
            var completed = await Task.WhenAny(tcs.Task, Task.Delay(2000));
            Assert.AreEqual(tcs.Task, completed, "Action did not execute in time");

            // allow small margin for state reset
            await Task.Delay(50);

            Assert.IsFalse(q.IsActive(), "IsActive should be false after processing");
        }

        [TestMethod]
        public async Task Processing_Guard_Prevents_Concurrent_Action_Execution()
        {
            var logs = new List<string>();
            var inAction = 0;
            var reentryTcs = new TaskCompletionSource<bool>();
            var doneCount = 0;
            var doneTcs = new TaskCompletionSource<bool>();

            void Action(List<string> items)
            {
                if (Interlocked.Exchange(ref inAction, 1) == 1)
                {
                    reentryTcs.TrySetResult(true);
                }

                // simulate work
                Thread.Sleep(200);

                Interlocked.Exchange(ref inAction, 0);

                if (Interlocked.Increment(ref doneCount) >= 1)
                {
                    doneTcs.TrySetResult(true);
                }
            }

            var q = new QueueManager<string>(limit: 2, duration: 1000, action: Action, logger: CreateLogger(logs));

            // enqueue many items concurrently to attempt to trigger concurrent executions
            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() => q.Enqueue(Guid.NewGuid().ToString())));
            }

            await Task.WhenAll(tasks);

            // wait until at least two batches processed or timeout
            var completed = await Task.WhenAny(doneTcs.Task, Task.Delay(5000));
            Assert.AreEqual(doneTcs.Task, completed, "Batches did not complete in time");

            // ensure no reentry detected
            Assert.IsFalse(reentryTcs.Task.IsCompleted, "Action was re-entered concurrently");
        }

        [TestMethod]
        public async Task Enqueue_During_Processing_Items_Are_Not_Lost()
        {
            var logs = new List<string>();
            var processed = new List<string>();
            var processingTcs = new TaskCompletionSource<bool>();
            var finishTcs = new TaskCompletionSource<bool>();

            void Action(List<string> items)
            {
                // first time signal
                if (!processingTcs.Task.IsCompleted)
                {
                    processingTcs.TrySetResult(true);
                }

                // simulate longer processing to allow concurrent enqueues
                Thread.Sleep(200);

                lock (processed)
                {
                    processed.AddRange(items);
                }

                // when processed size grows beyond 5 signal finish
                if (processed.Count >= 5)
                {
                    finishTcs.TrySetResult(true);
                }
            }

            var q = new QueueManager<string>(limit: 2, duration: 1000, action: Action, logger: CreateLogger(logs));

            // prime two items to trigger immediate processing
            q.Enqueue("a");
            q.Enqueue("b");

            // wait until action starts
            await Task.WhenAny(processingTcs.Task, Task.Delay(1000));

            // while processing, enqueue more items
            q.Enqueue("c");
            q.Enqueue("d");
            q.Enqueue("e");

            // wait for finish
            var completed = await Task.WhenAny(finishTcs.Task, Task.Delay(5000));
            Assert.AreEqual(finishTcs.Task, completed, "Not all items were processed in time");

            // verify no loss
            Assert.AreEqual(5, processed.Count);
            CollectionAssert.AreEquivalent(new List<string> { "a", "b", "c", "d", "e" }, processed);
        }

        [TestMethod]
        public async Task Generic_Type_Int_Handled_Correctly()
        {
            var logs = new List<string>();
            var processed = new List<int>();
            var tcs = new TaskCompletionSource<bool>();

            void Action(List<int> items)
            {
                lock (processed)
                {
                    processed.AddRange(items);
                }

                tcs.TrySetResult(true);
            }

            var q = new QueueManager<int>(limit: 10, duration: 100, action: Action, logger: CreateLogger(logs));

            q.Enqueue(1);
            q.Enqueue(2);

            var completed = await Task.WhenAny(tcs.Task, Task.Delay(2000));
            Assert.AreEqual(tcs.Task, completed, "Int action did not execute in time");

            Assert.IsTrue(processed.Contains(1) && processed.Contains(2));
        }

        [TestMethod]
        public async Task TokenDisposal_NoExceptions_OnManyTriggers()
        {
            var logs = new List<string>();
            var processed = new List<string>();

            void Action(List<string> items)
            {
                lock (processed)
                {
                    processed.AddRange(items);
                }
                // small work
                Thread.Sleep(10);
            }

            var q = new QueueManager<string>(limit: 10, duration: 200, action: Action, logger: CreateLogger(logs));

            var iterations = 200;
            var perIter = 10;
            for (int i = 0; i < iterations; i++)
            {
                for (int j = 0; j < perIter; j++)
                {
                    q.Enqueue($"m-{i}-{j}");
                }

                // small pause to allow triggers and token replacement
                await Task.Delay(5);
            }

            // allow time for processing
            await Task.Delay(3000);

            // ensure no exception logs were captured
            Assert.IsFalse(logs.Any(l => l.Contains("async_trigger_exception") || l.Contains("trigger_exception") || l.Contains("action_exception")), "No exception logs expected");

            // verify processed count equals enqueued
            Assert.AreEqual(iterations * perIter, processed.Count, "All enqueued items should be processed");
        }

        [TestMethod]
        [TestCategory("Integration")]
        [Ignore("Integration test - long running")]
        public async Task Integration_Stress_LongRunning()
        {
            var logs = new List<string>();
            var processed = new List<string>();

            void Action(List<string> items)
            {
                lock (processed)
                {
                    processed.AddRange(items);
                }
                Thread.Sleep(1);
            }

            var q = new QueueManager<string>(limit: 100, duration: 100, action: Action, logger: CreateLogger(logs));

            var total = 20000;
            var tasks = new List<Task>();
            for (int i = 0; i < total; i++)
            {
                var s = $"x-{i}";
                tasks.Add(Task.Run(() => q.Enqueue(s)));
            }

            await Task.WhenAll(tasks);

            // wait long enough for processing
            await Task.Delay(15000);

            Assert.AreEqual(total, processed.Count, "All items should be processed in stress test");
        }

    }
}
