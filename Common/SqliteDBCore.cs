using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Common
{
    public class SqliteDBCore
    {

        //private static readonly string _connstr= ConfigurationManager.ConnectionStrings["connstr"].ToString();

        //public DapperDataCore(string conn)
        //{
        //    _connstr = conn;
        //}


        public virtual string ConnectionString
        {
            get
            {
                throw new Exception("未重写连接字符串.");
            }
        }

        public long Insert<T>(T inertItem) where T : class
        {
            using (SQLiteConnection cn = new SQLiteConnection(ConnectionString))
            {
                cn.Open();
                var id = cn.Insert(inertItem);
                cn.Close();
                return id;
            }
        }

        public bool TransactionInsert<T>(List<T> listT, Action<int> callBack = null) where T : class
        {
            using (SQLiteConnection cn = new SQLiteConnection(ConnectionString))
            {
                cn.Open();
                var trans = cn.BeginTransaction();
                try
                {
                    var id = cn.Insert(listT, trans);
                    trans.Commit();
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    return false;
                }

                cn.Close();
                return true;
            }
        }

        public bool Update<T>(T newItem) where T : class
        {
            using (SQLiteConnection cn = new SQLiteConnection(ConnectionString))
            {
                cn.Open();
                cn.UpdateAsync(newItem).Wait();
                cn.Close();
                return true;

            }
        }

        public bool Delete<T>(T deleteItem) where T : class
        {
            using (SQLiteConnection cn = new SQLiteConnection(ConnectionString))
            {
                cn.Open();
                cn.Delete(deleteItem);
                cn.Close();
                return true;
            }
        }

        public bool Delete(string sql)
        {
            using (SQLiteConnection cn = new SQLiteConnection(ConnectionString))
            {
                cn.Open();
                cn.ExecuteScalar(sql);
                cn.Close();
                return true;
            }
        }

        public T Get<T>(int primaryKeyId) where T : class
        {
            using (SQLiteConnection cn = new SQLiteConnection(ConnectionString))
            {
                cn.Open();
                var item = cn.Get<T>(primaryKeyId);
                cn.Close();
                return item;
            }
        }

        public T Get<T>(string primaryKeyId) where T : class
        {
            using (SQLiteConnection cn = new SQLiteConnection(ConnectionString))
            {
                cn.Open();
                var item = cn.Get<T>(primaryKeyId);
                cn.Close();
                return item;
            }
        }

        public IList<T> QueryForList<T>(string sql)
        {
            using (SQLiteConnection cn = new SQLiteConnection(ConnectionString))
            {
                cn.Open();
                var item = cn.Query<T>(sql).ToList();
                cn.Close();
                return item;
            }
        }

        public DataTable QueryForDataTable(string sql)
        {
            using (SQLiteConnection cn = new SQLiteConnection(ConnectionString))
            {
                cn.Open();
                var reader = cn.ExecuteReader(sql);
                DataTable dt = new DataTable();
                dt.Load(reader);
                cn.Close();
                return dt;
            }
        }

        public void ExecSql(string sql)
        {
            using (SQLiteConnection cn = new SQLiteConnection(ConnectionString))
            {
                cn.Open();
                cn.ExecuteScalar(sql);
                cn.Close();
            }
        }

    }
}
