using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.OleDb;
using DataMigration.SqlConnect;

namespace DataMigration.AbsDAO
{
    
    public class AbsDAOIMP
    {
        private List<RawData> RawdataList;
        private static AbsDAOIMP _daoInstance;
        private AbsDAOIMP()
        {
            RawdataList = new List<RawData>();
        }
        public static AbsDAOIMP GetDaoInstance()
        {
            if (_daoInstance == null)
            {
                _daoInstance = new AbsDAOIMP();
            }
            return _daoInstance;
        }

        public List<dynamic> GetMultiData(string database,string tablename, Dictionary<string,string> conditons=null)
        {
            
            var data = RawdataList.Find(x => x.Table_name.Equals(database+"."+tablename));
            if (data == null)
            {
                data = new RawData(database,tablename);
                RawdataList.Add(data);
            }
            data.lastvisit = DateTime.Now;
            if (RawdataList.Count > 10)
            {
                RawdataList.Remove(RawdataList.OrderBy(x => x.lastvisit).First());
            }
            List<dynamic> target=data.AllData;
            if (conditons != null && conditons.Count > 0)
            {
                foreach (KeyValuePair<string, string> kvp in conditons)
                {
                    target = target.FindAll(x => ((IDictionary<string, object>)x)[kvp.Key.ToUpper()].ToString().Equals(kvp.Value.ToUpper()));
                }
                return target;
            }
            
            else
            {
                return data.AllData;
            }
            
            
        }
        public string GetSingleData(string database,string tablename,string column,Dictionary<string,string> conditions=null)
        {
            string sReturn = string.Empty;
            var data = RawdataList.Find(x => x.Table_name.Equals(database+"."+tablename));
            if (data == null)
            {
                data = new RawData(database,tablename);
                RawdataList.Add(data);
            }
            data.lastvisit = DateTime.Now;
            //如果数据超出10个表，将最近不用的表移除。
            if (RawdataList.Count > 10)
            {
                RawdataList.Remove(RawdataList.OrderBy(x => x.lastvisit).First());
            }
            List<dynamic> target = data.AllData;
            if (conditions != null && conditions.Count > 0)
            {
                foreach (KeyValuePair<string, string> kvp in conditions)
                {
                    target = target.FindAll(x => ((IDictionary<string, object>)x)[kvp.Key.ToUpper()].ToString().Equals(kvp.Value.ToUpper()));
                }
            }
            if (target != null&&target.Count>0)
            {
                sReturn = ((IDictionary<string, object>)target.First())[column.ToUpper()].ToString();
            }
            return sReturn;
        }
        /// <summary>
        /// 插入表table_name
        /// </summary>
        /// <param name="database"></param>
        /// <param name="table_name"></param>
        /// <param name="data"></param>
        public void SqlAdd(string database,string table_name,Dictionary<string,string> data)
        {

            string sqlstring="insert into "+table_name+" ",
                sqlkeys=string.Empty,
                sqlvalues=string.Empty;
            foreach(KeyValuePair<string,string> kvp in data)
            {
                sqlkeys+=kvp.Key+",";
                sqlvalues+="'"+kvp.Value+"'"+",";
            }
            sqlkeys.TrimEnd(',');
            sqlvalues.TrimEnd(',');
            sqlstring=string.Format("{0}({1}) values ({2})");

            SqlConnectSuit.GetConnect(database).Execute(sqlstring);
        }
        /// <summary>
        /// 删除表table_name条件是data
        /// </summary>
        /// <param name="database"></param>
        /// <param name="table_name"></param>
        /// <param name="data"></param>
        public void SqlDel(string database, string table_name, Dictionary<string, string> data)
        {

            string sqlstring = "delete from " + table_name + " where ";
            foreach (KeyValuePair<string, string> kvp in data)
            {
                sqlstring+=kvp.Key+"='"+kvp.Value+"' and";
            }
            sqlstring=sqlstring.Take(sqlstring.Count() - 4).ToString();

            SqlConnectSuit.GetConnect(database).Execute(sqlstring);
        }
    }
    public class RawData
    {
        public string Table_name;
        public List<dynamic> AllData;
        public DateTime lastvisit=DateTime.Now;
        public RawData(string database,string tablename)
        {
            Table_name =database+"."+ tablename;
            AllData=SqlConnectSuit.GetConnect(database).Query("select * from "+tablename).ToList<dynamic>();
        }
    }
   
}
