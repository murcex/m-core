namespace Crane.Internal.Engine.Interface
{
	public interface ICraneRedactor
	{
		Dictionary<string, Dictionary<string, string>> Execute(Dictionary<string, string> taskCfg, Dictionary<string, Dictionary<string, string>> taskParameters);
	}
}