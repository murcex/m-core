namespace Crane.Internal.Engine.Interface
{
	public interface ICraneLogger
	{
		public void Info(string input);
		public void Success(string input);
		public void Error(string input);
		public void Enable(string rootPath);
		public bool Enabled();
	}
}