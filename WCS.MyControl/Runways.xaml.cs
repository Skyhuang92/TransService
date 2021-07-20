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
    /// Runways.xaml 的交互逻辑
    /// </summary>
    public partial class Runways : UserControl
    {
        public Runways()
        {
            InitializeComponent();
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
                DrawRunways();
            }
        }

        private void DrawRunways()
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
                var way = new Runway();
                way.Margin = new Thickness(0);
                gridBin.Children.Add(way);
                Grid.SetRow(way, i);
                Grid.SetColumn(way, 0);
            }
        }
    }
}
