using Crane.Internal.Engine.Interface;

namespace Crane.Internal.Test.Mock
{
	public class TestLogger : ICraneLogger
	{
		private List<string> _logs;

		private bool _status;

		public TestLogger()
		{
			this._logs = new List<string>();
			this._status = false;
		}

		public TestLogger(List<string> logs)
		{
			_logs = logs;
			_status = false;
		}

		public void Info(string input)
		{
			Console.WriteLine(input);
			_logs.Add(input);
		}

		public void Success(string input)
		{
			Console.WriteLine(input);
			_logs.Add(input);
		}

		public void Error(string input)
		{
			Console.WriteLine(input);
			_logs.Add(input);
		}

		public void Enable(string rootPath)
		{
			Console.WriteLine(rootPath);
			_logs.Add(rootPath);
		}

		public bool Enabled()
		{
			return _status;
		}
	}
}
