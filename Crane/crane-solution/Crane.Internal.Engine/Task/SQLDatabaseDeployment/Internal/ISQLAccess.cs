namespace Crane.Internal.Engine.Task.SQLDatabaseDeployment.Internal
{
	public interface ISQLAccess
	{
		public bool SetCredentials(string connectionString);

		public (bool result, string code, string message) Execute(string sqlCmd);
	}
}
