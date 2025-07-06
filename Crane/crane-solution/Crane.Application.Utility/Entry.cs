using Crane.Internal.Engine;
using Crane.Internal.Engine.Components;
using Crane.Internal.Engine.Interface;

namespace Crane.Application.Utility
{
	public class Entry
	{
		static void Main(string[] args)
		{
			ICraneLogger logger = new CraneLogger();
			ICraneConsole console = new CraneConsole(new CraneRedactor());
			ICraneFileManager fileManager = new CraneFileManager();
			ICraneTaskManager taskManager = new CraneTaskManager();

			CraneApplication app = new(logger, console, fileManager, taskManager);

			app.Execute(args);
		}
	}
}