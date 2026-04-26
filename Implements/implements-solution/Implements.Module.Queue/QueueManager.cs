using System;
using System;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Implements.Module.Queue
{
	/// <summary>
	// This class represents a queue manager that allows enqueueing objects and executing actions on the enqueued objects based on certain triggers.
	/// </summary>
	public class QueueManager<T>
	{
		///
		/// --- Queue Configuration ---
		///

		/// <summary>
		/// Represents the queue that stores the enqueued items.
		/// </summary>
		private readonly ConcurrentQueue<T> _queue;

		/// <summary>
		/// Represents the maximum number of items allowed in the queue.
		/// </summary>
		private readonly int _limit;

		/// <summary>
		/// Represents the duration in milliseconds after which the queue processor will be triggered.
		/// </summary>
		private readonly int _duration;

		/// <summary>
		/// Represents the action to be executed on the items in the queue.
		/// </summary>
		private readonly Action<List<T>> _action;

		/// <summary>
		/// Represents the optional logger function to log messages.
		/// </summary>
		private readonly Action<string> _logger;

		///
		/// --- State Management ---
		///

		/// <summary>
		/// Represents the state of the queue manager indicating if it is active or not.
		/// 0 = inactive, 1 = active. Uses Interlocked for atomic reads/writes.
		/// </summary>
		private int _active;

		/// <summary>
		/// Represents the state of the queue manager indicating if it is currently processing items.
		/// 0 = not processing, 1 = processing. Uses Interlocked for atomic guarding.
		/// </summary>
		private int _processing;

		/// <summary>
		/// Represents the cancellation token source used to cancel the queue processor.
		/// Access to replacing/disposing the token is protected by `_stateLock`.
		/// </summary>
		private CancellationTokenSource _token;

		/// <summary>
		/// Represents the flag indicating if the queue manager has been shut down.
		/// 0 = running, 1 = shutdown. Uses Interlocked for atomic transition.
		/// </summary>
		private int _shutdown;

		/// <summary>
		/// Lock object to protect compound state transitions (token replacement and active flag changes).
		/// </summary>
		private readonly object _stateLock = new();

		/// <summary>
		/// Initializes a new instance of the QueueManager class.
		/// </summary>
		/// <param name="limit">The maximum number of items allowed in the queue.</param>
		/// <param name="duration">The duration in milliseconds after which the queue processor will be triggered.</param>
		/// <param name="action">The action to be executed on the items in the queue.</param>
		/// <param name="logger">The optional logger function to log messages.</param>
		public QueueManager(int limit, int duration, Action<List<T>> action, Action<string>? logger = null)
		{
			_queue = new ConcurrentQueue<T>();
			_limit = limit;
			_duration = duration;
			_action = action ?? throw new ArgumentNullException(nameof(action));
			_logger = logger ?? (_ => { });
			_active = 0;
			_processing = 0;
			_shutdown = 0;
			_token = new CancellationTokenSource();
		}

		/// <summary>
		/// Enqueues an object to the queue.
		/// </summary>
		/// <param name="obj">The object to enqueue.</param>
		/// <returns>True if the object was successfully enqueued, false otherwise.</returns>
		public bool Enqueue(T obj)
		{
			if (Interlocked.CompareExchange(ref _shutdown, 0, 0) == 1)
			{
				return false;
			}

			_queue.Enqueue(obj);

			_logger($"t={DateTime.UtcNow},k=add_item,v={_queue.Count}");

			// fast-path: if active, check for limit trigger
			if (Interlocked.CompareExchange(ref _active, 0, 0) == 1)
			{
				if (_queue.Count >= _limit)
				{
					CancellationTokenSource tokenToCancel;
					lock (_stateLock)
					{
						tokenToCancel = _token;
					}

					try { tokenToCancel?.Cancel(); } catch (ObjectDisposedException) { }

					var id = GetInstanceId();
					Task.Run(async () =>
					{
						try { await Trigger(id).ConfigureAwait(false); }
						catch (Exception ex) { _logger($"t={DateTime.UtcNow},i={id},k=trigger_exception,v={ex}"); }
					});

					_logger($"t={DateTime.UtcNow},i={id},k=queue_limit_triggered,v={_queue.Count}");
				}
			}
			else
			{
				lock (_stateLock)
				{
					if (Interlocked.CompareExchange(ref _shutdown, 0, 0) == 1)
					{
						return false;
					}

					var previous = _token;
					_token = new CancellationTokenSource();
					try { previous?.Dispose(); } catch { }

					var id = GetInstanceId();
					Interlocked.Exchange(ref _active, 1);

					Task.Run(async () =>
					{
						try { await AsyncTrigger(id, _token.Token).ConfigureAwait(false); }
						catch (Exception ex) { _logger($"t={DateTime.UtcNow},i={id},k=async_trigger_exception,v={ex}"); }
					});

					_logger($"t={DateTime.UtcNow},k=queue_async_triggered,v={id}");
				}
			}

			return true;
		}

		/// <summary>
		/// Shuts down the queue manager.
		/// </summary>
		/// <returns>True if the queue manager was successfully shut down, false otherwise.</returns>
		public bool Shutdown()
		{
			if (Interlocked.Exchange(ref _shutdown, 1) == 1)
			{
				return false;
			}

			// cancel and dispose current token safely
			CancellationTokenSource tokenToCancel;
			lock (_stateLock)
			{
				tokenToCancel = _token;
				_token = new CancellationTokenSource();
				Interlocked.Exchange(ref _active, 0);
			}

			try { tokenToCancel?.Cancel(); } catch (ObjectDisposedException) { }
			try { tokenToCancel?.Dispose(); } catch { }

			return true;
		}

		/// <summary>
		/// Checks if the queue manager is active.
		/// </summary>
		/// <returns>True if the queue manager is active, false otherwise.</returns>
		public bool IsActive()
		{
			return Interlocked.CompareExchange(ref _active, 0, 0) == 1;
		}

		/// <summary>
		/// Executes the trigger for a specific ID.
		/// </summary>
		/// <param name="id">The ID of the trigger.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		private async Task Trigger(string id)
		{
			await Task.Run(() => ExecuteTrigger(id, TriggerType.Limit)).ConfigureAwait(false);
		}

		/// <summary>
		/// Asynchronously triggers the execution of the queue processor after a specified duration.
		/// </summary>
		/// <param name="id">The ID of the trigger.</param>
		/// <param name="token">The cancellation token.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		private async Task AsyncTrigger(string id, CancellationToken token)
		{
			try
			{
				await Task.Delay(_duration, token).ConfigureAwait(false);
			}
			catch (TaskCanceledException)
			{
				// cancelled by limit or shutdown
				return;
			}

			ExecuteTrigger(id, TriggerType.Duration);
		}

		/// <summary>
		/// Executes the trigger for a specific ID.
		/// </summary>
		/// <param name="id">The ID of the trigger.</param>
		/// <param name="type">The type of the trigger.</param>
		private void ExecuteTrigger(string id, TriggerType type)
		{
			_logger($"t={DateTime.UtcNow},i={id},k=execute_queue_processor,v={type}");

			// acquire processing guard
			if (Interlocked.CompareExchange(ref _processing, 1, 0) == 1)
			{
				_logger($"t={DateTime.UtcNow},i={id},k=processor_status,v=locked");
				return;
			}

			_logger($"t={DateTime.UtcNow},i={id},k=processor_status,v=locking");

            // Process batches until queue is empty. This avoids a race where
            // triggers arriving while processing return early and leave items unprocessed.
            int totalProcessed = 0;
            try
            {
                while (true)
                {
                    List<T> objs = new List<T>();
                    while (_queue.TryDequeue(out var obj))
                    {
                        objs.Add(obj);
                    }

                    if (objs.Count == 0)
                    {
                        break;
                    }

                    totalProcessed += objs.Count;

                    _logger($"t={DateTime.UtcNow},i={id},k=processor_action_count,v={objs.Count}");
                    _logger($"t={DateTime.UtcNow},i={id},k=processor_queue_count,v={_queue.Count}");

                    try
                    {
                        _logger($"t={DateTime.UtcNow},i={id},k=action_status,v=executing");
                        _action(objs);
                        _logger($"t={DateTime.UtcNow},i={id},k=action_status,v=completed");
                    }
                    catch (Exception ex)
                    {
                        var data = ex.ToString().Replace(",", "").Replace("=", "");
                        _logger($"t={DateTime.UtcNow},i={id},k=action_status,v=exception");
                        _logger($"t={DateTime.UtcNow},i={id},k=action_exception,v={data}");
                    }
                }
            }
            finally
            {
                // reset active and replace token under lock once processing fully complete
                CancellationTokenSource previous;
                lock (_stateLock)
                {
                    Interlocked.Exchange(ref _active, 0);
                    previous = _token;
                    _token = new CancellationTokenSource();
                }

                try { previous?.Dispose(); } catch { }

                _logger($"t={DateTime.UtcNow},i={id},k=processor_status,v=unlocked");
                _logger($"t={DateTime.UtcNow},i={id},k=processor_action_total,v={totalProcessed}");

                // release processing flag after all batches processed
                Interlocked.Exchange(ref _processing, 0);
            }
		}

		/// <summary>
		/// Generates a unique instance ID.
		/// </summary>
		/// <returns>The generated instance ID.</returns>
		private static string GetInstanceId()
		{
			return Guid.NewGuid().ToString().Split('-')[0].ToUpper();
		}
	}

	/// <summary>
	/// Represents the type of trigger for the queue manager.
	/// </summary>
	enum TriggerType
	{
		Limit,
		Duration
	}
}
