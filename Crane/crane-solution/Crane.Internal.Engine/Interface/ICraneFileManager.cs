namespace Crane.Internal.Engine.Interface
{
	public interface ICraneFileManager
	{
		string GetCraneLoggerFilePath(ICraneLogger logger, Dictionary<string, Dictionary<string, string>> cfg);
		Dictionary<string, Dictionary<string, string>> LoadCraneConfig(ICraneLogger logger);
		bool CheckForConformation(ICraneLogger logger, Dictionary<string, Dictionary<string, string>> cfg);
		Dictionary<string, Dictionary<string, string>> LoadCraneTask(ICraneLogger logger, Dictionary<string, Dictionary<string, string>> cfg, string name);
	}
}