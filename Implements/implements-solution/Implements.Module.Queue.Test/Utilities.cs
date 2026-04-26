using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Implements.Module.Queue.Test
{
    public class Utilities
    {
        public static List<string> SampleGenerator(int count)
        {
            List<string> samples = new();
            int sample = 0;
            while (sample < count)
            {
                samples.Add($"test_{sample}");
                sample++;
            }

            return samples;
        }

        public static Action<string> CreateTestLogger(List<string> tracker)
        {
            return (string msg) =>
            {
                tracker.Add(msg);
            };
        }

        public static Action<List<string>> CreateTestAction(List<string> tracker, Batch batch)
        {
            return (List<string> objs) =>
            {
                var currentBatch = GetBatch(batch);
                foreach (var item in objs)
                {
                    tracker.Add($"B{currentBatch}-{item}");
                }
            };
        }

        public static Action<List<string>> CreateExceptionTestAction(List<string> tracker, Batch batch)
        {
            return (List<string> objs) =>
            {
                throw new Exception("Test Exception");
            };
        }

        private static int GetBatch(Batch batch)
        {
            return batch.Next();
        }

        public static bool CheckTrackerContains(List<string> input, List<string> output)
        {
            return output.All(x => input.Contains(x.Split("-")[1]) == true);
        }

        public static async Task EnqueueAsync(QueueManager<string> queue, string sample, int delay)
        {
            await Task.Delay(delay);

            queue.Enqueue(sample);
        }
    }
}
