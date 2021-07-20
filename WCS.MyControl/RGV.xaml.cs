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

namespace WCS.MyControl
{
    /// <summary>
    /// RGV.xaml 的交互逻辑
    /// </summary>
    public partial class RGV : UserControl
    {
        //控件单击事件
        public event Action<string> Click;

        public RGV()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 站台编号
        /// </summary>
        public string LocPlcNo
        {
            get
            {
                return this.locPlcNo.Text.ToString();
            }
            set
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.locPlcNo.Text = value;
                });
            }
        }

        /// <summary>
        /// 站台号
        /// </summary>
        public string LocNo
        {
            get;
            set;
        }      
      

        /// <summary>
        /// 设置载货标识
        /// </summary>
        public void SetToLoad(int value)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (value == 1)
                {
                    this.locStatus.Fill = CustomSolidBrush.PaleGreen;
                }
                else
                {
                    this.locStatus.Fill = CustomSolidBrush.LightGray;
                }
            });
        }

        /// <summary>
        /// 设置空闲标识
        /// </summary>
        public void SetToFree(int value)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (value == 1)
                {
                    this.locStatus.Fill = CustomSolidBrush.LightBlue;
                }
                else
                {
                    this.locStatus.Fill = CustomSolidBrush.LightGray;
                }
            });
        }

        /// <summary>
        /// 设置请求标识
        /// </summary>
        public void SetToRequest(int value)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (value == 1)
                {
                    this.locStatus.Fill = CustomSolidBrush.LimeGreen;
                }
            });
        }

        /// <summary>
        /// 设置故障标识
        /// </summary>
        public void SetToFault(int value)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (value == 1)
                {
                    this.locStatus.Fill = CustomSolidBrush.Red;
                }
            });
        }

        /// <summary>
        /// 设置自动标识
        /// </summary>
        public void SetToAuto(int value)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (value == 0)
                {
                    this.locAuto.Visibility = Visibility.Visible;
                }
                else
                {
                    this.locAuto.Visibility = Visibility.Hidden;
                }
            });
        }



        /// <summary>
        /// 设置禁用标识
        /// </summary>
        public void SetToEnable(int value)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (value == 0)
                {
                    this.locStatus.Fill = new SolidColorBrush(Colors.White);
                }
                else
                {
                    this.locStatus.Fill = new SolidColorBrush(Colors.LightGray);
                }
            });
        }              

        private void tranGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Click?.Invoke(this.LocNo);
        }
    }
}
