using KirokuG2;
using KirokuG2.Internal;

namespace Murcex.Vyudro.Module.Client.Models
{
	internal class NoLogger : IKLog
	{
		public void Dispose()
		{ }

		public void Error(string data)
		{ }

		public void Info(string data)
		{ }

		public void Metric(string key, bool value)
		{ }

		public void Metric(string key, double value)
		{ }

		public void Metric(string key, float value)
		{ }

		public void Metric(string key, int value)
		{ }

		public void Metric(string key, string value)
		{ }

		public KBlock NewBlock(string name)
		{
			return null;
		}

		public void Trace(string data)
		{ }
	}
}
