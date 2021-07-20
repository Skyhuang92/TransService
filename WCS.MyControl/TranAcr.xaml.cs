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
    /// TranAcr.xaml 的交互逻辑
    /// </summary>
    public partial class TranAcr : UserControl
    {
        public TranAcr()
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

        private LocDirection locDirection;
        /// <summary>
        /// 方向
        /// </summary>
        public LocDirection LocDirection
        {
            get
            {
                return locDirection;
            }
            set
            {
                locDirection = value;
                SetToDirection();
            }
        }

        /// <summary>
        /// 设定方向
        /// </summary>
        public void SetToDirection()
        {
            this.Dispatcher.Invoke(() =>
            {
                switch (this.LocDirection)
                {
                    case LocDirection.None:
                        this.baNorth.Visibility = Visibility.Hidden;
                        this.baSouth.Visibility = Visibility.Hidden;
                        break;
                    case LocDirection.North:
                        this.baNorth.Visibility = Visibility.Visible;
                        this.baSouth.Visibility = Visibility.Hidden;
                        break;
                    case LocDirection.South:
                        this.baNorth.Visibility = Visibility.Hidden;
                        this.baSouth.Visibility = Visibility.Visible;
                        break;
                    case LocDirection.NorthAndSouth:
                        this.baNorth.Visibility = Visibility.Visible;
                        this.baSouth.Visibility = Visibility.Visible;
                        break;
                    default:
                        break;
                }
            });
        }
    }
}
