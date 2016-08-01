using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataMigration.SqlConnect;
using System.Data.OleDb;
using Dapper;
using DataMigration.AbsDAO;
using DataMigration.Model;
using DataMigration.Functions;
using DataMigration.trade.DE;
using System.ComponentModel;

namespace DataMigration
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, ICRUD
    {
        private AbsDAOIMP adi;
        private BindingList<DeProduct> LPrdtDe;
        
        public MainWindow()
        {
            InitializeComponent();
            adi =  AbsDAOIMP.GetDaoInstance();
            LPrdtDe = new BindingList<DeProduct>();
            SqlConnectSuit.GetConnect("olddb", "Provider=MSDAORA;  user id=hshtdb; password=hshtdb; Data Source=(DESCRIPTION =(ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(Host = 10.1.5.7)(Port = 1521)))(CONNECT_DATA =(SERVICE_NAME = ORCL))); Persist Security Info=False;");
            SqlConnectSuit.GetConnect("newdb", "Provider=MSDAORA;  user id=newcoredb; password=newcoredb; Data Source=(DESCRIPTION =(ADDRESS_LIST =(ADDRESS = (PROTOCOL = TCP)(Host = 10.1.5.8)(Port = 1521)))(CONNECT_DATA =(SERVICE_NAME = ORCL))); Persist Security Info=False;");

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LPrdtDe.Clear();
            RuleManager rm = RuleManager.GetRuleManagerInstance();
            Rule rule=rm.GetRuleByName("PrdtDe");
            List<int> prdtuse = new List<int>();
            if (rule != null)
            {
                
                List<dynamic> prdtList= adi.GetMultiData("olddb",rule.KeyTable);
                foreach (IDictionary<string, object> singleprdt in prdtList)
                {
                    DeProduct pd = new DeProduct();
                    
                    pd.prdt_no = rule.getstringbyrule("PRDT_NO", singleprdt);
                    prdtuse.Add(Int32.Parse(pd.prdt_no));
                    //minint转换函数会使用到。设置为字段名称
                    DataBus.SetData("PRDT_NO", prdtuse);
                    pd.filler = rule.getstringbyrule("FILLER", singleprdt); 
                    //de_Base_parm
                    pd.Cbase.prdt_type = rule.getstringbyrule("PRDT_TYPE", singleprdt); 
                    pd.Cbase.prdt_knd = rule.getstringbyrule("PRDT_KND", singleprdt);
                    pd.Cbase.cif_type = rule.getstringbyrule("CIF_TYPE", singleprdt);
                    pd.Cbase.beg_date = long.Parse(rule.getstringbyrule("BEG_DATE", singleprdt));
                    pd.Cbase.end_date = long.Parse(rule.getstringbyrule("END_DATE", singleprdt));
                    pd.Cbase.term_type = rule.getstringbyrule("TERM_TYPE", singleprdt);
                    pd.Cbase.term = long.Parse(rule.getstringbyrule("TERM", singleprdt));
                    pd.Cbase.prdt_sts = rule.getstringbyrule("PRDT_STS", singleprdt);
                    pd.Cbase.prdt_ind = rule.getstringbyrule("PRDT_IND", singleprdt);
                    //de_rate_parm
                    pd.Crate.prdt_flt_type =rule.getstringbyrule("PRDT_FLT_TYPE", singleprdt);
                    pd.Crate.rate_chg_term = rule.getstringbyrule("RATE_CHG_TERM", singleprdt);
                    pd.Crate.rate_chg_type = rule.getstringbyrule("RATE_CHG_TYPE", singleprdt);
                    pd.Crate.rate_no = rule.getstringbyrule("RATE_NO", singleprdt);
                    pd.Crate.rate_rule_sys = rule.getstringbyrule("RATE_RULE_SYS", singleprdt);
                    pd.Crate.rate_type = rule.getstringbyrule("RATE_TYPE", singleprdt);
                    pd.Crate.ratio = double.Parse(rule.getstringbyrule("RATIO", singleprdt));
                    pd.Crate.rule_cal_type = rule.getstringbyrule("RULE_CAL_TYPE", singleprdt);
                    LPrdtDe.Add(pd);
                }
                DataBus.RemoveData("PRDT_NO");
            }
            dg_main.ItemsSource = LPrdtDe;
        }

        private void Btn_modify_Click(object sender, RoutedEventArgs e)
        {
            DEInfo depage = new DEInfo(dg_main.SelectedItem as DeProduct);
            depage.ShowDialog();
            //修改相应删除
            LPrdtDe.FindandReplace((DeProduct)dg_main.SelectedItem, depage.Prdt);
            
        }

        private void Btn_sync_Click(object sender, RoutedEventArgs e)
        {
            //查询现有的产品名称，如果相同产品名称的存在提示是否继续移植。
            List<dynamic> existprdt = adi.GetMultiData("newdb", "prdt_parm", new Dictionary<string, string> { { "GROUP_NO", "DE_BASE_PARM" } });

            //循环现有产品
            foreach (DeProduct dp in LPrdtDe)
            {
                if (existprdt.Exists(x => x.FILLER.Equals(dp.filler)))
                {
                   MessageBoxResult mbr= MessageBox.Show("该产品" + dp.filler + "已经存在，是否继续移植", "错误",MessageBoxButton.YesNo,MessageBoxImage.Information);
                   if (mbr == MessageBoxResult.No)
                   {
                       continue;
                   }
                   //删除现有产品配置
                   
                   
                }
                //增加新产品

            }
        }
        



        public void ADD(DeProduct dp)
        {
            //de_Base_parm
            //de_rate_parm
            //prdt_parm
            adi.SqlAdd("newdb","PRDT_PARM",new Dictionary<string,string>{{"PRDT_NO",dp.prdt_no},{"GROUP_NO","DE_BASE_PARM"},{"GROUP_MO","基本属性"},
            {"BEG_DATE",dp.Cbase._beg_date.ToString()},{"END_DATE",dp.Cbase.end_date.ToString()},
            {"STS","0"},{"FILLER",dp.filler}});
            
           
        }

        public void Delete(DeProduct dp)
        {
            
        }
    }
}
