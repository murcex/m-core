﻿namespace PlyQor.Engine.Components.Storage.Internals
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.SqlClient;
    using PlyQor.Engine.Core;
    using PlyQor.Models;
    using PlyQor.Resources;

    class SelectRetentionStroage
    {
        public static List<string> Execute()
        {
            List<string> ids = new List<string>();

            try
            {
                using (var connection = new SqlConnection(Configuration.DatabaseConnection))
                {
                    var cmd = new SqlCommand(SqlColumns.SelectRetentionStroage, connection);

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    connection.Open();

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var id = (string)reader[SqlColumns.Id];

                        ids.Add(id);
                    }

                    return ids;
                }
            }
            catch (Exception ex)
            {
                throw new JavelinException(StatusCode.ERR010, ex);
            }
        }
    }
}
