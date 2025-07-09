namespace Crane.Internal.Engine.Interface
{
	public interface ICraneConsole
	{
		void Starter();
		void GeneralConformation(ICraneLogger logger);
		public void TaskConfirmation(ICraneLogger logger, Dictionary<string, Dictionary<string, string>> collection);
		public void Close();
	}
}