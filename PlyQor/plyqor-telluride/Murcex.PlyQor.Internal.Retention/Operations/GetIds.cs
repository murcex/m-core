using Microsoft.Data.SqlClient;
using Murcex.PlyQor.Internal.Retention.Core;

namespace Murcex.PlyQor.Internal.Retention.Operations
{
	public class GetId
	{
		public static List<string> Execute(string container)
		{
			List<string> ids = new List<string>();

			try
			{
				var query = $"SELECT TOP ({Configuration.Top}) [nvc_id] FROM [tbl_PlyQor_Data] WHERE [nvc_container] = '{container}' AND [dt_timestamp] < DATEADD(DAY,-1,GETDATE())";

				using (var connection = new SqlConnection(Configuration.DatabaseConnection))
				{
					var cmd = new SqlCommand(query, connection);

					cmd.CommandTimeout = 0;

					connection.Open();

					var reader = cmd.ExecuteReader();
					while (reader.Read())
					{
						var id = (string)reader["nvc_id"];

						ids.Add(id);
					}

					return ids;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());

				return new List<string>();
			}
		}
	}
}
