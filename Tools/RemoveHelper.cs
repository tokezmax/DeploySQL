using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace DeploySQL.Tools
{
    public class RemoveHelper
    {
        public static string GetAllDropSpScript = @"select
'DROP PROCEDURE ['+SPECIFIC_SCHEMA+'].['+SPECIFIC_NAME+']' AS script,
'['+SPECIFIC_SCHEMA+'].['+SPECIFIC_NAME+']' AS rName
from information_schema.routines
where routine_type = 'PROCEDURE'
and Left(Routine_Name, 3) NOT IN ('sp_', 'xp_', 'ms_')
order by SPECIFIC_CATALOG,SPECIFIC_SCHEMA,SPECIFIC_NAME";

        public static string GetAllDropTableScript = @"SELECT
'DROP TABLE ['+TABLE_SCHEMA+'].['+TABLE_NAME+']' AS script,
'['+TABLE_SCHEMA+'].['+TABLE_NAME+']' AS rName
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
order by TABLE_CATALOG,TABLE_SCHEMA,TABLE_NAME";

        public static void removeAllSp(string connStr)
        {
            var dt = DbHelper.querySqlFile(connStr, GetAllDropSpScript);
            if (null == dt || dt.Rows.Count <= 0)
                return;

            var OutputMsg = "({0}/{1}){2}";
            var ExecutedFileCount = 1;
            var allCount = dt.Rows.Count;
            foreach (DataRow row in dt.Rows)
            {
                var sql = row["script"].ToString();
                DbHelper.executeSqlFile(connStr, sql);
                LogHelper.doLog(string.Format(OutputMsg, ExecutedFileCount, allCount, row["rName"].ToString()));
                ExecutedFileCount++;
            }
        }

        public static void removeAllTable(string connStr)
        {
            var dt = DbHelper.querySqlFile(connStr, GetAllDropTableScript);
            if (null == dt || dt.Rows.Count <= 0)
                return;

            var OutputMsg = "({0}/{1}){2}";
            var ExecutedFileCount = 1;
            var allCount = dt.Rows.Count;
            foreach (DataRow row in dt.Rows)
            {
                var sql = row["script"].ToString();
                DbHelper.executeSqlFile(connStr, sql);
                LogHelper.doLog(string.Format(OutputMsg, ExecutedFileCount, allCount, row["rName"].ToString()));
                ExecutedFileCount++;
            }
        }
    }
}