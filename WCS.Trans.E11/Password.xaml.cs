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

namespace WCS.Trans
{
    /// <summary>
    /// Password.xaml 的交互逻辑
    /// </summary>
    public partial class Password : Window
    {
        private string strPassword = string.Empty;

        public Password(string password)
        {
            InitializeComponent();

            strPassword = password;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Password))
            {
                MessageBox.Show("请输入密码！");
                return;
            }
            if (txtPassword.Password != strPassword)
            {
                MessageBox.Show("密码错误！");
                return;
            }
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
