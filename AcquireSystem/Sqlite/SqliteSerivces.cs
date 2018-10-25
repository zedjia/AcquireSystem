using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;

namespace AcquireSystem.Sqlite
{
    public class SqliteSerivces: SqliteDBCore
    {
        private const string SQLITEDBPATH = "./Sqlite/db.sqlite";
        private SQLiteConnection connection;

        public override string ConnectionString
        {
            get
            {
                return $"Data Source={SQLITEDBPATH};Version=3;";
            }
        }

        public SqliteSerivces()
        {
            connection= new SQLiteConnection(ConnectionString);
        }

        public bool SqlTest()
        {
            try
            {
                var data=connection.ExecuteScalar("select count(1) from Test");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }





    }
}
