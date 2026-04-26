using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Implements.Module.Queue.Test
{
	[TestClass]
	public class Index
	{
		/// <summary>
		/// This test checks the behavior of the queue when a single duration is specified.
		/// </summary>
		[TestMethod]
		public async Task Duration_Single()
		{
			var cfg = new QueueTestConfig(10, 5000, 5, 10, 10000);

			var results = await InternalExecutorAsync(cfg);

			Assert.IsTrue(Utilities.CheckTrackerContains(results.samples, results.objTracker));

			//var records = QueueLoglizer.Execute(results.logTracker);
		}

		/// <summary>
		/// This test checks the behavior of the queue when multiple durations are specified.
		/// </summary>
		/// <returns></returns>
		[TestMethod]
		public async Task Duration_Multi()
		{
			var cfg = new QueueTestConfig(10, 5500, 1000, 10, 10000);

			var results = await InternalExecutorAsync(cfg);

			Assert.IsTrue(Utilities.CheckTrackerContains(results.samples, results.objTracker));

			//var records = QueueLoglizer.Execute(results.logTracker);
		}

		/// <summary>
		/// This test checks the behavior of the queue when a single limit is specified.
		/// </summary>
		/// <returns></returns>
		[TestMethod]
		public async Task Limit_Single()
		{
			var cfg = new QueueTestConfig(5, 1000, 9, 10, 10000);

			var results = await InternalExecutorAsync(cfg);

			Assert.IsTrue(Utilities.CheckTrackerContains(results.samples, results.objTracker));

			//var records = QueueLoglizer.Execute(results.logTracker);
		}

		/// <summary>
		/// This test checks the behavior of the queue when multiple limits are specified.
		/// </summary>
		/// <returns></returns>
		[TestMethod]
		public async Task Limit_Multi()
		{
			var cfg = new QueueTestConfig(5, 1000, 11, 10, 10000);

			var results = await InternalExecutorAsync(cfg);

			Assert.IsTrue(Utilities.CheckTrackerContains(results.samples, results.objTracker));

			//var records = QueueLoglizer.Execute(results.logTracker);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[TestMethod]
		public async Task Load_Blast()
		{
			var cfg = new QueueTestConfig(10, 5000, 10000, 10, 10000);

			var results = await InternalExecutorAsync(cfg);

			Assert.IsTrue(Utilities.CheckTrackerContains(results.samples, results.objTracker));

			//var records = QueueLoglizer.Execute(results.logTracker);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[TestMethod]
		public async Task Load_Burst()
		{
			var cfg = new QueueTestConfig(10, 5000, 2500, 10, 10000);

			var results = await InternalBurstExecutorAsync(cfg);

			Assert.IsTrue(Utilities.CheckTrackerContains(results.samples, results.objTracker));

			//var records = QueueLoglizer.Execute(results.logTracker);

			var test = 0;
		}

		/// <summary>
		/// This test checks the behavior of the queue under high load.
		/// </summary>
		/// <returns></returns>
		[TestMethod]
		public async Task Load_Multi()
		{
			var cfg = new QueueTestConfig(100, 5000, 1000, 5000, 10000);

			var results = await InternalRandomExecutorAsync(cfg);

			Assert.IsTrue(Utilities.CheckTrackerContains(results.samples, results.objTracker), $"{results.samples}:{results.objTracker}");

			//var records = QueueLoglizer.Execute(results.logTracker);
		}

		/// <summary>
		/// This test checks the behavior of the queue when an exception occurs.
		/// </summary>
		/// <returns></returns>
		[TestMethod]
		public async Task Standard_Exception()
		{
			var cfg = new QueueTestConfig(10, 5000, 5, 10, 10000);

			var results = await InternalExceptionExecutorAsync(cfg);

			var records = QueueLoglizer.Execute(results.logTracker);

			Assert.IsTrue(records.Any(record => record.Key == "action_exception"));
		}

		/// <summary>
		/// This test checks the behavior of the queue when it is closed and empty.
		/// </summary>
		[TestMethod]
		public async Task Standard_Shutdown()
		{
		}

		/// <summary>
		/// This test checks the behavior of the queue when it is closed and empty.
		/// </summary>
		[TestMethod]
		public async Task Standard_IsActive()
		{
		}

		/// <summary>
		/// Executes the queue test asynchronously.
		/// </summary>
		/// <param name="config">The configuration for the queue test.</param>
		/// <returns>A tuple containing the generated samples, object tracker, and log tracker.</returns>
		private async Task<(List<string> samples, List<string> objTracker, List<string> logTracker)> InternalExecutorAsync(QueueTestConfig config)
		{
			var samples = Utilities.SampleGenerator(config.SampleSize);

			var objTracker = new List<string>();
			var batch = new Batch();
			var logTracker = new List<string>();

            var queue = new QueueManager<string>(config.Limit, config.Duration, Utilities.CreateTestAction(objTracker, batch), Utilities.CreateTestLogger(logTracker));

			foreach (var sample in samples)
			{
				queue.Enqueue(sample);

				await Task.Delay(config.EnqueueDelay);
			}

			await Task.Delay(config.DrainDelay);

			return (samples, objTracker, logTracker);
		}

		private async Task<(List<string> samples, List<string> objTracker, List<string> logTracker)> InternalRandomExecutorAsync(QueueTestConfig config)
		{
			var samples = Utilities.SampleGenerator(config.SampleSize);

			var objTracker = new List<string>();
			var batch = new Batch();
			var logTracker = new List<string>();

            var queue = new QueueManager<string>(config.Limit, config.Duration, Utilities.CreateTestAction(objTracker, batch), Utilities.CreateTestLogger(logTracker));

			var totalSamples = samples.Count;
			var groupSizes = new List<int>();

			// Calculate the sizes of the three groups
			var remainingSamples = totalSamples;
			while (remainingSamples > 0)
			{
				var maxSize = Math.Min(remainingSamples, 100);
				var minSize = Math.Min(maxSize, 50);
				var groupSize = new Random().Next(minSize, maxSize + 1);
				groupSizes.Add(groupSize);
				remainingSamples -= groupSize;
			}

			// Break the samples into three groups
			var groups = new List<List<string>>();
			var currentIndex = 0;
			foreach (var groupSize in groupSizes)
			{
				var group = samples.GetRange(currentIndex, groupSize);
				groups.Add(group);
				currentIndex += groupSize;
			}

			// Run each group through the existing foreach loop
			var tasks = new List<Task>();
			foreach (var group in groups)
			{
				foreach (var sample in group)
				{
					var randomDelay = new Random().Next(1, config.EnqueueDelay + 1);
					tasks.Add(Utilities.EnqueueAsync(queue, sample, randomDelay));
				}
			}

			await Task.WhenAll(tasks);

			await Task.Delay(config.DrainDelay);

			return (samples, objTracker, logTracker);
		}

		/// <summary>
		/// Executes the queue test asynchronously.
		/// </summary>
		/// <param name="config">The configuration for the queue test.</param>
		/// <returns>A tuple containing the generated samples, object tracker, and log tracker.</returns>
		private async Task<(List<string> samples, List<string> objTracker, List<string> logTracker)> InternalBurstExecutorAsync(QueueTestConfig config)
		{
			var objTracker = new List<string>();
			var batch = new Batch();
			var logTracker = new List<string>();

            var queue = new QueueManager<string>(config.Limit, config.Duration, Utilities.CreateTestAction(objTracker, batch), Utilities.CreateTestLogger(logTracker));

			var samples = new List<string>();

			var cycles = 1;
			while (cycles < 4)
			{
				var cycleSamples = Utilities.SampleGenerator(config.SampleSize);
				samples.AddRange(cycleSamples);

				foreach (var sample in cycleSamples)
				{
					queue.Enqueue(sample);

					await Task.Delay(config.EnqueueDelay);
				}

				await Task.Delay(config.DrainDelay);
				cycles++;
			}

			return (samples, objTracker, logTracker);
		}

		private async Task<(List<string> samples, List<string> objTracker, List<string> logTracker)> InternalExceptionExecutorAsync(QueueTestConfig config)
		{
			var samples = Utilities.SampleGenerator(config.SampleSize);

			var objTracker = new List<string>();
			var batch = new Batch();
			var logTracker = new List<string>();

            var queue = new QueueManager<string>(config.Limit, config.Duration, Utilities.CreateExceptionTestAction(objTracker, batch), Utilities.CreateTestLogger(logTracker));

			foreach (var sample in samples)
			{
				queue.Enqueue(sample);

				await Task.Delay(config.EnqueueDelay);
			}

			await Task.Delay(config.DrainDelay);

			return (samples, objTracker, logTracker);
		}
	}
}