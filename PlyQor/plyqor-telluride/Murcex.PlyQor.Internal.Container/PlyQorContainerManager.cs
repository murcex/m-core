using Murcex.PlyQor.Internal.Container.Core;
using Murcex.PlyQor.Internal.Container.Model;

namespace Murcex.PlyQor.Internal.Container
{
	public class PlyQorContainerManager
	{
		public static bool Initialize(Dictionary<string, Dictionary<string, string>> config)
		{
			return Initializer.Execute(config);
		}

		public static bool Initialize(string databaseConnection)
		{
			return Initializer.Execute(databaseConnection);
		}

		public static List<string> ListContainers()
		{
			return Operations.ListContainers.Execute();
		}

		public static PlyQorContainer GetContainer(string containerName)
		{
			return Operations.GetContainer.Execute(containerName);
		}

		public static Dictionary<string, int> GetContainerRetention()
		{
			return Operations.GetRetentionPolicies.Execute();
		}

		public static string GetToken(string containerName)
		{
			return Operations.GetToken.Execute(containerName);
		}

		public static bool AddContainer(PlyQorContainer container)
		{
			return Operations.AddContainer.Execute(container);
		}

		public static bool UpdateContainer(PlyQorContainer container)
		{
			return Operations.UpdateContainer.Execute(container);
		}

		public static bool DeleteContainer(PlyQorContainer container)
		{
			return Operations.DeleteContainer.Execute(container);
		}
	}
}
