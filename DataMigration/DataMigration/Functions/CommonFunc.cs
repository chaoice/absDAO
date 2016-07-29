using DataMigration.AbsDAO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DataMigration.Functions
{
    
    public   class CommonFunc
    {
        /// <summary>
        /// 取prdt_parm里group_no=de_base_parm，里rule="minint(1000,1999,PRDT_PARM,PRDT_NO，GROUP_NO,DE_BASE_PARM)"
        /// </summary>
        /// <param name="start_no"></param>
        /// <param name="end_no"></param>
        /// <param name="table_name"></param>
        /// <param name="column_name"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static int minint(string parm)
            //public static int minint(int start_no,int end_no, string table_name, string column_name,Dictionary<string,string> conditions=null)
        {
            string[] parms=parm.Split(',');
            int start_no = Int32.Parse(parms[0]);
            int end_no = Int32.Parse(parms[1]);
            string table_name = parms[2];
            string column_name = parms[3];
            Dictionary<string, string> conditions = new Dictionary<string, string>();
            for (int i = 4; i < parms.Count(); i+=2)
            {
                conditions.Add(parms[i], parms[i + 1]);
            }
            var target=AbsDAOIMP.GetDaoInstance().GetMultiData("newdb",table_name, conditions);
            for(int  i = start_no;i<=end_no;i++)
            {
                if (!target.Exists(x => Int32.Parse(((IDictionary<string, object>)x)[table_name].ToString()) == i))
                {
                    return i;
                }
            }
            return -1;

        }
        /// <summary>
        /// 扩展list<t> 方法，集合中查找并替换为新对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">list集合</param>
        /// <param name="sour">集合中对象</param>
        /// <param name="Des">新对象</param>
        public static void FindandReplace<T>(this BindingList<T> list,T sour,T Des)
        {
            int index = list.ToList().FindIndex(x => x.Equals(sour));
            list.RemoveAt(index);
            list.Insert(index, Des);
        }
        /// <summary>
        /// 序列化方式进行深层拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">被复制类型</param>
        /// <returns>复制后的新对象</returns>
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
      
        
        /// <summary>
        /// 根据字段映射规则取当前主表记录对应的字段值
        /// </summary>
        /// <param name="rule">规则</param>
        /// <param name="columnname">列名</param>
        /// <param name="singleprdt">单个产品</param>
        /// <returns></returns>
        public static string getstringbyrule(this Rule rule, string columnname, IDictionary<string, object> singleprdt)
        {
            
            string rString = "";
            ColumnRule cr = rule.GetColumnRulebyString(columnname);
            if (!string.IsNullOrEmpty(cr.SourceColumnName))
            {
                //从老表中获取
                if (cr.SourceTable.Equals(rule.KeyTable))
                {
                    //取主表数据
                    rString = singleprdt[cr.SourceColumnName].ToString();
                }
                else
                {
                    //取其他表数据
                    rString = AbsDAOIMP.GetDaoInstance().GetSingleData("olddb", cr.SourceTable, cr.SourceColumnName, new Dictionary<string, string> { { cr.SourceKey, singleprdt[cr.MianKey].ToString() } });
                }
                if (!string.IsNullOrEmpty(cr.rule))
                {
                    //使用转换规则
                    string[] parms = cr.rule.Split(new char[] { '(', ')' });
                    Assembly sb=Assembly.GetExecutingAssembly();
                    var obj=sb.CreateInstance("DataMigration.Functions.CommonFunc");
                    object[] paras = new object[] { parms[1] };
                    var ind=typeof(CommonFunc).GetMethod(parms[0]).Invoke(obj, paras);
                    
                }

            }
            else if (!string.IsNullOrEmpty(cr.defaultvalue))
            {
                //默认值
                rString = cr.defaultvalue;
            }
            else
            {
                //tbd
                rString = "";
            }
            return rString;
        }
        /// <summary>
        /// 简单的根据使用系统、基准利率，浮动类型，浮动值在新核心里找利率规则，不存在就新增。函数返回利率规则码。
        /// </summary>
        /// <param name="rrs">适用系统</param>
        /// <param name="rate_no">利率代码</param>
        /// <param name="rct">多条利率规则</param>
        /// <param name="pft">产品利率浮动类型</param>
        /// <param name="ratio">产品利率浮动值</param>
        /// <returns>产品利率编号</returns>
        public static string GetDataCodebyRule(string rrs,string rate_no,string rct="1",string pft="1",double ratio=0)
        {
            string Sre_code = string.Empty;
            var adi = AbsDAOIMP.GetDaoInstance();
            List<dynamic> prdtrate=new List<dynamic>();
            if(!ratio.Equals(0))
            {
               prdtrate= adi.GetMultiData("newdb","PRDT_RATECTL_PARM",new Dictionary<string,string>{{"rate_no",rate_no},{"flt_type",pft},{"rate_ratio",ratio.ToString()}});
            }
            else
            {
                prdtrate = adi.GetMultiData("newdb", "PRDT_RATECTL_PARM", new Dictionary<string,string>{{"rate_no",rate_no}});
            }
            
            List<dynamic> raterule=new List<dynamic>();
            raterule = adi.GetMultiData("newdb", "RATE_RULE_DEF", new Dictionary<string, string> { { "rate_rule_sys", rrs },{"rule_cal_type",rct} });
            Sre_code=((IDictionary<string, object>)(raterule.Except(raterule.Except(prdtrate)).First()))["rate_rule_code"].ToString();
            if (string.IsNullOrEmpty(Sre_code))
            {
                //没有找到需要新增rate_rule_def prdt_ratectl_parm。
                //查找rate_rule_code最大可用值，并赋值给Sre_code。
                //插入rate_rule_def与prdt_ratectl_parm。
                
            }
            return Sre_code;
        }
    }
}
