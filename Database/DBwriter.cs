using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace EscapeFromTheWoods
{
    public class DBwriter
    {
        private string connectionString;

        public DBwriter(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private SqlConnection getConnection()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            return connection;
        }
        #region OLD
        public void WriteWoodRecords(List<DBWoodRecord> data)
        {
            SqlConnection connection = getConnection();
            string query = "INSERT INTO dbo.WoodRecords (woodID,treeID,x,y) VALUES(@woodID,@treeID,@x,@y)";
            using (SqlCommand command = connection.CreateCommand())
            {
                connection.Open();
                try
                {
                    command.Parameters.Add(new SqlParameter("@woodID", SqlDbType.Int));
                    command.Parameters.Add(new SqlParameter("@treeID", SqlDbType.Int));
                    command.Parameters.Add(new SqlParameter("@x", SqlDbType.Int));
                    command.Parameters.Add(new SqlParameter("@y", SqlDbType.Int));

                    command.CommandText = query;
                    foreach (var x in data)
                    {
                        command.Parameters["@woodID"].Value = x.woodID;
                        command.Parameters["@treeID"].Value = x.treeID;
                        command.Parameters["@x"].Value = x.x;
                        command.Parameters["@y"].Value = x.y;
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public void WriteMonkeyRecords(List<DBMonkeyRecord> data)
        {
            SqlConnection connection = getConnection();
            string query = "INSERT INTO dbo.MonkeyRecords (monkeyID,monkeyName,woodID, seqNr,treeID,x,y) VALUES(@monkeyID,@monkeyName,@woodID,@seqNr,@treeID,@x,@y)";
            using (SqlCommand command = connection.CreateCommand())
            {
                connection.Open();
                try
                {
                    command.Parameters.Add(new SqlParameter("@monkeyID", SqlDbType.Int));
                    command.Parameters.Add(new SqlParameter("@monkeyName", SqlDbType.NVarChar));
                    command.Parameters.Add(new SqlParameter("@woodID", SqlDbType.Int));
                    command.Parameters.Add(new SqlParameter("@seqNr", SqlDbType.Int));
                    command.Parameters.Add(new SqlParameter("@treeID", SqlDbType.Int));
                    command.Parameters.Add(new SqlParameter("@x", SqlDbType.Int));
                    command.Parameters.Add(new SqlParameter("@y", SqlDbType.Int));

                    command.CommandText = query;
                    foreach (var x in data) {
                        command.Parameters["@monkeyID"].Value = x.monkeyID;
                        command.Parameters["@monkeyName"].Value = x.monkeyName;
                        command.Parameters["@woodID"].Value = x.woodID;
                        command.Parameters["@seqNr"].Value = x.seqNr;
                        command.Parameters["@treeID"].Value = x.treeID;
                        command.Parameters["@x"].Value = x.x;
                        command.Parameters["@y"].Value = x.y;
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        #endregion

        #region REFACTORED / ASYNC
        public async Task AsyncWriteWoodRecords(List<DBWoodRecord> data)
        {
            string query = "INSERT INTO dbo.WoodRecords (woodID, treeID, x, y) VALUES (@woodID, @treeID, @x, @y)";
            using (SqlConnection connection = getConnection())
            {
                await connection.OpenAsync();
                using (SqlCommand command = connection.CreateCommand())
                {
                    try
                    {
                        command.CommandText = query;
                        command.Parameters.Add(new SqlParameter("@woodID", SqlDbType.Int));
                        command.Parameters.Add(new SqlParameter("@treeID", SqlDbType.Int));
                        command.Parameters.Add(new SqlParameter("@x", SqlDbType.Int));
                        command.Parameters.Add(new SqlParameter("@y", SqlDbType.Int));

                        foreach (var x in data)
                        {
                            command.Parameters["@woodID"].Value = x.woodID;
                            command.Parameters["@treeID"].Value = x.treeID;
                            command.Parameters["@x"].Value = x.x;
                            command.Parameters["@y"].Value = x.y;
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    finally
                    {
                        await connection.CloseAsync();
                    }
                }
            }
        }
        public async Task AsyncWriteMonkeyRecords(List<DBMonkeyRecord> data)
        {
            string query = "INSERT INTO dbo.MonkeyRecords (monkeyID,monkeyName,woodID, seqNr,treeID,x,y) VALUES(@monkeyID,@monkeyName,@woodID,@seqNr,@treeID,@x,@y)";
            using (SqlConnection connection = getConnection())
            {
                await connection.OpenAsync();
                using (SqlCommand command = connection.CreateCommand())
                {
                    try
                    {
                        command.Parameters.Add(new SqlParameter("@monkeyID", SqlDbType.Int));
                        command.Parameters.Add(new SqlParameter("@monkeyName", SqlDbType.NVarChar));
                        command.Parameters.Add(new SqlParameter("@woodID", SqlDbType.Int));
                        command.Parameters.Add(new SqlParameter("@seqNr", SqlDbType.Int));
                        command.Parameters.Add(new SqlParameter("@treeID", SqlDbType.Int));
                        command.Parameters.Add(new SqlParameter("@x", SqlDbType.Int));
                        command.Parameters.Add(new SqlParameter("@y", SqlDbType.Int));

                        command.CommandText = query;
                        foreach (var x in data)
                        {
                            command.Parameters["@monkeyID"].Value = x.monkeyID;
                            command.Parameters["@monkeyName"].Value = x.monkeyName;
                            command.Parameters["@woodID"].Value = x.woodID;
                            command.Parameters["@seqNr"].Value = x.seqNr;
                            command.Parameters["@treeID"].Value = x.treeID;
                            command.Parameters["@x"].Value = x.x;
                            command.Parameters["@y"].Value = x.y;
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    finally
                    {
                        await connection.CloseAsync();
                    }
                }
            }
        }
        #endregion
    }
}
