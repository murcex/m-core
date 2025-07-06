using Crane.Internal.Engine.Interface;

namespace Crane.Internal.Test.Mock
{
	public class TestConsole : ICraneConsole
	{
		private List<string> _tracker;

		public TestConsole()
		{
			_tracker = new List<string>();
		}

		public TestConsole(List<string> consoleTracker)
		{
			_tracker = consoleTracker;
		}

		public void Close()
		{
			_tracker.Add("Close");
		}

		public void GeneralConformation(ICraneLogger logger)
		{
			_tracker.Add("GeneralConformation");
		}

		public void Starter()
		{
			_tracker.Add("Starter");
		}

		public void TaskConfirmation(ICraneLogger logger, Dictionary<string, Dictionary<string, string>> collection)
		{
			_tracker.Add("TaskConfirmation");
		}
	}
}
