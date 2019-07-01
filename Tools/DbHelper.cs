using DeploySQL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace DeploySQL.Tools
{
    public class DbHelper
    {
        public static void executeSqlFile(string connStr, string script)
        {
            // split script on GO command
            var commandStrings = Regex.Split(script, @"^\s*GO\s*$",
                         RegexOptions.Multiline | RegexOptions.IgnoreCase);

            RetryHelper.DoAction(() =>
            {
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    foreach (string commandString in commandStrings)
                    {
                        if (commandString.Trim() == "")
                            continue;

                        using (var command = new SqlCommand(commandString, conn))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }, TimeSpan.FromMilliseconds(500), Configs.cfg.DbRetryCount);
        }

        public static DataTable querySqlFile(string connStr, string TSQL)
        {
            var DT = new DataTable();
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var MyCommand = new SqlCommand(TSQL, conn)
                {
                    CommandTimeout = 0
                };

                using (SqlDataAdapter da = new SqlDataAdapter(MyCommand))
                {
                    da.Fill(DT);
                }
            }

            return DT;
        }
    }
}