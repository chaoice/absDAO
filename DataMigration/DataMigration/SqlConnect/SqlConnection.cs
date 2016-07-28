using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.OleDb;

namespace DataMigration.SqlConnect
{
    public class SqlConnectSuit
    {
        private static SqlConnectSuit _sql_connect;
        private List<ConnectionCase> _sql_connection;

        private SqlConnectSuit()
        {
            _sql_connection = new List<ConnectionCase>();
        }

        public static OleDbConnection GetConnect(string name, string dbstring = null)
        {
            if (_sql_connect == null)
            {
                _sql_connect = new SqlConnectSuit();
            }
            if (!_sql_connect._sql_connection.Exists(x => x._name.Equals(name)))
            {
                _sql_connect._sql_connection.Add(new ConnectionCase(name, dbstring));
            }
            return _sql_connect._sql_connection.Find(x => x._name.Equals(name))._conn;
        }
       

    }
    public class ConnectionCase
    {
        public string _name;
        private string _connectstring;
        public OleDbConnection _conn;
        public ConnectionCase(string name,string connectstring)
        {
            _name = name;
            _connectstring = connectstring;
            _conn = new OleDbConnection(_connectstring);
            _conn.Open();
        }
    }

}
