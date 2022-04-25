using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SMAmoving
{
    /// <summary>
    /// Interaction logic for TableViewWindow.xaml
    /// </summary>
    public partial class TableViewWindow : Window
    {
        public string SymbolLabel { get; set; }
        public string IndicatorLabel { get; set; }
        public string LastRefreshedLabel { get; set; }
        public string IntervalLabel { get; set; }
        public string SeriesTypeLabel { get; set; }
        public string TimePeriodLabel { get; set; }
        public string TimezoneLabel { get; set; }
        
        public TableViewWindow(string symbol, string indicator, string lastRefreshed, string interval, string seriesType, string timePeriod, string timezone)
        {
            SymbolLabel = "Symbol: " + symbol;
            IndicatorLabel = "Indicator: " + indicator;
            LastRefreshedLabel = "Last Refreshed: " + lastRefreshed;
            IntervalLabel = "Interval: " + interval;
            SeriesTypeLabel = "Series Type: " + seriesType;
            TimePeriodLabel = "Time Period: " + timePeriod;
            TimezoneLabel = "Timezone: " + timezone;
            DataContext = this;
            InitializeComponent();
            tableView.ItemsSource = MainWindow.SMAdata;
            tableView.MinColumnWidth = 420;
            Console.WriteLine("stop");
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
