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
    public partial class MainWindow : Window
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
            if (rule != null)
            {
                List<dynamic> prdtList= adi.GetMultiData("olddb",rule.KeyTable);
                foreach (IDictionary<string, object> singleprdt in prdtList)
                {
                    DeProduct pd = new DeProduct();
                    
                    pd.prdt_no = rule.getstringbyrule("PRDT_NO", singleprdt);
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
            //循环现有产品

        }
       
    }
}
