using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DataMigration
{
    public class RuleManager
    {
        private static RuleManager _ruleManager;
        List<Rule> LRule;
        public Rule GetRuleByName(string name)
        {
            Rule r_rule = LRule.Find(x => x.RuleName.Equals(name));
            return r_rule;

        }
        private RuleManager()
        {
            LRule = new List<Rule>();
            XmlDocument xd = new XmlDocument();
            xd.Load("Migration.xml");
            foreach (XmlNode xn in xd.SelectSingleNode("//root").ChildNodes)
            {
                Rule tmprule = new Rule() { RuleName = xn.Name, KeyTable = xn.Attributes["maintable"].Value };
                foreach (XmlNode xnchild in xn.ChildNodes)
                {
                    if (xnchild.NodeType == XmlNodeType.Element)
                    {
                        ColumnRule cr = new ColumnRule()
                        {
                            defaultvalue = xnchild.Attributes["default"] == null ? string.Empty : xnchild.Attributes["default"].Value,
                            rule = xnchild.Attributes["rule"] == null ? string.Empty : xnchild.Attributes["rule"].Value,
                            TargetColumnName = xnchild.Name,
                            SourceColumnName = xnchild.Attributes["sourcecolumn"] == null ? string.Empty : xnchild.Attributes["sourcecolumn"].Value,
                            SourceTable = xnchild.Attributes["sourcetable"] == null ? tmprule.KeyTable : xnchild.Attributes["sourcetable"].Value,
                            MianKey = xnchild.Attributes["mainkey"] == null ? string.Empty : xnchild.Attributes["mainkey"].Value,
                            SourceKey = xnchild.Attributes["sourcekey"] == null ? string.Empty : xnchild.Attributes["sourcekey"].Value,
                        };
                        tmprule.LColumnRule.Add(cr);
                    }
                }
                LRule.Add(tmprule);
            }
        }
        public static RuleManager GetRuleManagerInstance()
        {
            if(_ruleManager==null)
            {
                _ruleManager=new RuleManager();
            }
            return _ruleManager;
        }
    }
    public class Rule 
    {
        public string RuleName ;
        public string KeyTable;
        public List<ColumnRule> LColumnRule;
        public Rule()
        {
            LColumnRule = new List<ColumnRule>();
        }
        public ColumnRule GetColumnRulebyString(string columnname)
        {
            return LColumnRule.Find(x => x.TargetColumnName.Equals(columnname));
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ColumnRule
    {
        //映射老表名
        public string SourceTable;
        //映射老表字段名
        public string SourceColumnName;
        //映射老表主键字段名
        public string SourceKey;
        //映射老表主表键值
        public string MianKey;
        //转换规则，只有老表字段名设置时可用
        public string rule;
        //默认值
        public string defaultvalue;
        //新表字段名
        public string TargetColumnName;
        public ColumnRule()
        {
            
        }
    }
}
