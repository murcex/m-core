﻿namespace PlyQor.Engine.Components.Storage.Internals
{
    using Microsoft.Data.SqlClient;
    using PlyQor.Engine.Core;
    using PlyQor.Models;
    using PlyQor.Resources;
    using System;

    class UpdateTagStorage
    {
        public static int Execute(
            string container,
            string oldindex,
            string newindex)
        {
            try
            {
                using (var connection = new SqlConnection(Configuration.DatabaseConnection))
                {
                    var cmd = new SqlCommand(SqlValues.UpdateTagStorage, connection);

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue(SqlValues.Container, container);
                    cmd.Parameters.AddWithValue(SqlValues.OldData, oldindex);
                    cmd.Parameters.AddWithValue(SqlValues.NewData, newindex);

                    cmd.CommandTimeout = 0;

                    connection.Open();

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    { }

                    var recordCount = reader.RecordsAffected;

                    return recordCount;
                }
            }
            catch (Exception ex)
            {
                if (ex is SqlException)
                {
                    SqlExceptionCheck.Execute(ex);
                }

                throw new PlyQorException(StatusCode.ERR010, ex);
            }
        }
    }
}
