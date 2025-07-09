namespace Crane.Internal.Engine.Interface
{
	public interface ICraneTaskManager
	{
		(bool result, string message) Execute(ICraneLogger logger, ICraneConsole console, Dictionary<string, Dictionary<string, string>> craneTask);
	}
}