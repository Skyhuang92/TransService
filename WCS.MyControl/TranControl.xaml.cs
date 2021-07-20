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
    /// TranControl.xaml 的交互逻辑
    /// </summary>
    public partial class TranControl : UserControl
    {
        //控件单击事件
        public event Action<string> Click;

        public TranControl()
        {
            InitializeComponent();
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

        private void tranGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Click?.Invoke(this.LocNo);
        }
    }
}
