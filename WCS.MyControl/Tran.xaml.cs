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
    /// Tran.xaml 的交互逻辑
    /// </summary>
    public partial class Tran : UserControl
    {
        public Tran()
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
        /// 宽度
        /// </summary>
        public double TranWidth
        {
            get
            {
                return this.tranGrid.Width;
            }
            set
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.tranGrid.Width = value;
                });
            }
        }

        /// <summary>
        /// 宽度
        /// </summary>
        public double TranHeight
        {
            get
            {
                return this.tranGrid.Height;
            }
            set
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.tranGrid.Height = value;
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

        private LocDirection lightDirection;
        /// <summary>
        /// 方向
        /// </summary>
        public LocDirection LightDirection
        {
            get
            {
                return lightDirection;
            }
            set
            {
                lightDirection = value;
                SetToLightDirection();
            }
        }

        /// <summary>
        /// 设定方向
        /// </summary>
        public void SetToLightDirection()
        {
            this.Dispatcher.Invoke(() =>
            {
                switch (this.LightDirection)
                {
                    case LocDirection.None:
                        this.recNorth.Visibility = Visibility.Hidden;
                        this.recSouth.Visibility = Visibility.Hidden;
                        break;
                    case LocDirection.North:
                        this.recNorth.Visibility = Visibility.Visible;
                        this.recSouth.Visibility = Visibility.Hidden;
                        break;
                    case LocDirection.South:
                        this.recNorth.Visibility = Visibility.Hidden;
                        this.recSouth.Visibility = Visibility.Visible;
                        break;
                    case LocDirection.NorthAndSouth:
                        this.recNorth.Visibility = Visibility.Visible;
                        this.recSouth.Visibility = Visibility.Visible;
                        break;
                    default:
                        this.recNorth.Visibility = Visibility.Hidden;
                        this.recSouth.Visibility = Visibility.Hidden;
                        break;
                }
            });
        }
    }
}
