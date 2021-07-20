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
    /// Store.xaml 的交互逻辑
    /// </summary>
    public partial class Store : UserControl
    {
        public Store()
        {
            InitializeComponent();

            this.BinColor = Colors.Brown;
        }

        private int columnCount;

        public int ColumnCount
        {
            get
            {
                return columnCount;
            }
            set
            {
                columnCount = value;
                DrawBin();
            }
        }

        private Color binColor;

        public Color BinColor
        {
            get
            {
                return binColor;
            }
            set
            {
                binColor = value;
                DrawBin();
            }
        }

        private void DrawBin()
        {
            gridBin.Children.Clear();
            gridBin.RowDefinitions.Clear();

            for (int i = 0; i < ColumnCount; i++)
            {
                var row = new RowDefinition();
                row.Height = new GridLength(1, GridUnitType.Star);
                gridBin.RowDefinitions.Add(row);
            }

            for (int i = 0; i < ColumnCount; i++)
            {
                var bin = new Bin();
                bin.Margin = new Thickness(1);
                gridBin.Children.Add(bin);
                Grid.SetRow(bin, i);
                Grid.SetColumn(bin, 0);
            }
        }
    }
}
