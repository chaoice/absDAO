using DataMigration.Model;
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
using System.Windows.Shapes;

namespace DataMigration.trade.DE
{
    /// <summary>
    /// DEInfo.xaml 的交互逻辑
    /// </summary>
    public partial class DEInfo : Window
    {
        public DeProduct Prdt;
        public DeProduct Prdt_active;
        public DEInfo(DeProduct de)
        {
            InitializeComponent();
            this.Prdt = de;
            this.Prdt_active = (DeProduct)de.Clone();
            this.DataContext = Prdt_active;
        }

        private void Btn_save_Click(object sender, RoutedEventArgs e)
        {
            this.Prdt = (DeProduct)Prdt_active.Clone();
        }

        private void Btn_restore_Click(object sender, RoutedEventArgs e)
        {
            this.Prdt_active = (DeProduct)Prdt.Clone();
            this.DataContext = Prdt_active;
        }

        private void Btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
