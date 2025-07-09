using Crane.Internal.Engine.Task.SQLDatabaseDeployment.Internal;

namespace Crane.Internal.Test.Mock
{
	public class TestSQLAccess : ISQLAccess
	{
		private Dictionary<string, string> _data;

		public TestSQLAccess(Dictionary<string, string> data)
		{
			_data = data;
		}

		public bool SetCredentials(string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString))
			{
				return false;
			}

			_data.TryAdd("connectionString", connectionString);

			return true;
		}

		public (bool result, string code, string message) Execute(string sqlCmd)
		{
			if (string.IsNullOrEmpty(sqlCmd))
			{
				throw new Exception("sqlCmd NullOrEmpty");
			}

			if (string.Equals("exception-1", sqlCmd, StringComparison.OrdinalIgnoreCase))
			{
				var id = Guid.NewGuid().ToString();

				_data.TryAdd(id, sqlCmd);

				return (true, "object_exists", id);
			}
			else if (string.Equals("exception-2", sqlCmd, StringComparison.OrdinalIgnoreCase))
			{
				var id = Guid.NewGuid().ToString();

				_data.TryAdd(id, sqlCmd);

				return (false, "sql_exception", id);
			}
			else
			{
				var id = Guid.NewGuid().ToString();

				_data.TryAdd(id, sqlCmd);

				return (true, "success", id);
			}
		}
	}
}
