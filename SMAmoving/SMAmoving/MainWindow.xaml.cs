using System;
using System.Windows;
using System.Net;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SMAmoving
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        private string _symbol = "IBM";
        public string Symbol
        {
            set
            {
                if (_symbol != value)
                {
                    _symbol = value;
                    OnPropertyChanged();
                }
                
            }
            get
            {
                return _symbol;
            }
        }

        private string _indicator = "";
        public string Indicator
        {
            set
            {
                if (_indicator != value)
                {
                    _indicator = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return _indicator;
            }
        }
        
        private string _lastRefreshed = "";
        public string LastRefreshed
        {
            set
            {
                if (_lastRefreshed != value)
                {
                    _lastRefreshed = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return _lastRefreshed;
            }
        }
        
        private string _interval = "";
        public string Interval
        {
            set
            {
                if (_interval != value)
                {
                    _interval = value;
                    OnPropertyChanged();
                }

            }
            get
            {
                return _interval;
            }
        }

        private int _timePeriod = 0;

        public int TimePeriod
        {
            set
            {
                if (_timePeriod != value)
                {
                    _timePeriod = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return _timePeriod;
            }
        }

        private string _seriesType = "";
        public string SeriesType
        {
            set
            {
                if (_seriesType != value)
                {
                    _seriesType = value;
                    OnPropertyChanged();
                }

            }
            get
            {
                return _seriesType;
            }
        }

        private string _timeZone = "";
        public string TimeZone
        {
            set
            {
                if (_timeZone != value)
                {
                    _timeZone = value;
                    OnPropertyChanged();
                }

            }
            get
            {
                return _timeZone;
            }
        }


        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            //====================TEST dijagram 1=================================
            int pointCount = 200;
            double[] ys1 = RandomWalk(pointCount);
            double[] ys2 = RandomWalk(pointCount);

            // create series and populate them with data
            var series1 = new LiveCharts.Wpf.LineSeries()
            {
                Title = "Group A",
                Values = new LiveCharts.ChartValues<double>(ys1),
            };

            var series2 = new LiveCharts.Wpf.LineSeries()
            {
                Title = "Group B",
                Values = new LiveCharts.ChartValues<double>(ys2),
            };

            // display the series in the chart control
            cartesianChart1.Series.Clear();
            cartesianChart1.Series.Add(series1);
            cartesianChart1.Series.Add(series2);
            //=========================================================

            symbol_cmbx.Items.Add("IBM");
            symbol_cmbx.Items.Add("TSCO.LON");
            symbol_cmbx.Items.Add("Shopify Inc (Canada - Toronto Stock Exchange)");

            interval_cmbx.Items.Add("1min");
            interval_cmbx.Items.Add("5min");
            interval_cmbx.Items.Add("15min");
            interval_cmbx.Items.Add("30min");
            interval_cmbx.Items.Add("60min");
            interval_cmbx.Items.Add("daily");
            interval_cmbx.Items.Add("weekly");
            interval_cmbx.Items.Add("monthly");

            cartesianChart2.DisableAnimations = true;
            
            //================= API for SMA ===============
            Thread t = new Thread(getAndDisplayData);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            
        }

        private void getAndDisplayData() {

            startLoadingAnimation();

            //dodati jos neophodnih parametara
            List<SMAdata> SMAdata = getSMAdataFromAPI(Symbol, "1min");
            
            displaySMAdataInLineChart(SMAdata);

            stopLoadingAnimation();
            
        }

        /// <summary>
        /// Funkcija koja vraca listu SMA objekata sa API-ja u zavisnosti od prosledjenih parametara 
        /// </summary>
        /// TODO : dodati sve neophodne parametre i dinamicki kreirati QUERY_URL
        private List<SMAdata> getSMAdataFromAPI(string symbol, string interval)
        {
            string QUERY_URL = $"https://www.alphavantage.co/query?function=SMA&symbol={symbol}&interval=weekly&time_period=10&series_type=open&apikey=DEC66JZYOJHHO5PC";
            Uri queryUri = new Uri(QUERY_URL);
            using (WebClient client = new WebClient())
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                dynamic json_data = js.Deserialize(client.DownloadString(queryUri), typeof(object));
                return convertJsonToSMAdata(json_data);
            }
        }

        /// <summary>
        /// Funkcija koja vraca listu SMA objekata od prosledjenog JSON stringa 
        /// </summary>
        private List<SMAdata> convertJsonToSMAdata(dynamic json_data)
        {
            List<SMAdata> MySMAs = new List<SMAdata>();
            MySMAs.Clear();

            int i = 0;
            foreach (Dictionary<string, object> stocks in json_data.Values)
            {
                if (i == 0)
                {
                    foreach (string key in stocks.Keys)
                    {
                        if (key.Contains("Symbol"))
                        {
                            Symbol = (string)stocks[key];
                        }
                        else if (key.Contains("Indicator"))
                        {
                            Indicator = (string)stocks[key];
                        }
                        else if (key.Contains("Last Refreshed"))
                        {
                            LastRefreshed = (string)stocks[key];
                        }
                        else if (key.Contains("Interval"))
                        {
                            Interval = (string)stocks[key];
                        }
                        else if (key.Contains("Time Period"))
                        {
                            TimePeriod = (Int32)stocks[key];
                        }
                        else if (key.Contains("Series Type"))
                        {
                            SeriesType = (string)stocks[key];
                        }
                        else if (key.Contains("Time Zone"))
                        {
                            TimeZone = (string)stocks[key];
                        }
                    }

                }
                if (i == 1)
                {
                    foreach (string key in stocks.Keys)
                    {

                        Dictionary<string, object> stock = (Dictionary<string, object>)stocks[key];
                        MySMAs.Add(new SMAdata
                        {
                            DateTime = key,
                            SMAvalue = double.Parse(stock["SMA"].ToString().Replace(".", ","))
                        });

                    }
                }
                i++;
            }

            return MySMAs;
        }

        private void displaySMAdataInLineChart(List<SMAdata> MySMAs) {

            List<double> valuesForChartSMA = new List<double>();
            foreach (SMAdata sma in MySMAs)
            {
                valuesForChartSMA.Add(sma.SMAvalue);
            }

            cartesianChart2.Dispatcher.Invoke(() => {

                var seriesSMA = new LiveCharts.Wpf.LineSeries()
                {
                    Title = "Group A",
                    Values = new LiveCharts.ChartValues<double>(valuesForChartSMA),
                    PointGeometry = System.Windows.Media.Geometry.Empty
                };


                cartesianChart2.Series.Clear();
                cartesianChart2.Series.Add(seriesSMA);

            },System.Windows.Threading.DispatcherPriority.ContextIdle);

        }

        private void startLoadingAnimation()
        {

            cartesianChart2.Dispatcher.Invoke(()=> {
                cartesianChart2.Visibility = Visibility.Hidden;
            });
            loadingChart2_img.Dispatcher.Invoke(() => {
                
                loadingChart2_img.Visibility = Visibility.Visible;
            });

        }

        private void stopLoadingAnimation()
        {

            cartesianChart2.Dispatcher.Invoke(() => {
                cartesianChart2.Visibility = Visibility.Visible;
            });
            loadingChart2_img.Dispatcher.Invoke(() => {

                loadingChart2_img.Visibility = Visibility.Hidden;
            });

        }
       
        //====================TEST dijagram 1================================

        private Random rand = new Random(0);

        private double[] RandomWalk(int points = 5, double start = 100, double mult = 50)
        {
            // return an array of difting random numbers
            double[] values = new double[points];
            values[0] = start;
            for (int i = 1; i < points; i++)
                values[i] = values[i - 1] + (rand.NextDouble() - .5) * mult;
            return values;
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Symbol = symbol_cmbx.SelectedItem.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(getAndDisplayData);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
           

        }

        private void interval_cmbx_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
        //====================TEST dijagram 1================================

    }
}
