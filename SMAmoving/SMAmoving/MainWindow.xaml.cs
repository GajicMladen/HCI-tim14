using System;
using System.Windows;
using System.Net;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System.Linq;
using System.Windows.Media;
using System.Data.Linq;

namespace SMAmoving
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SeriesCollection SeriesCollection { get; set; }

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

        public static List<SMAdata> SMAdata { get; set; }
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            cartesianChart1.AxisY.Clear();
            cartesianChart1.AxisY.Add(
                new Axis
                {
                    MinValue = 0
                }
            );

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

            cartesianChart1.DisableAnimations = true;
            cartesianChart2.DisableAnimations = true;
            
            //================= API for SMA ===============
            Thread t = new Thread(getAndDisplayData);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            
        }

        private void getAndDisplayData() {

            startLoadingAnimation();

            //dodati jos neophodnih parametara
            SMAdata = getSMAdataFromAPI(Symbol, "1min");
            List<StockData> StockData = getOhclFromAPI(Symbol);

            displaySMAdataInLineChart(SMAdata);
            displayStockDataInOhclChart(StockData);

            stopLoadingAnimation();
            
        }

        /// <summary>
        /// Funkcija koja vraca listu SMA objekata sa API-ja u zavisnosti od prosledjenih parametara 
        /// </summary>
        /// TODO : dodati sve neophodne parametre i dinamicki kreirati QUERY_URL
        public List<SMAdata> getSMAdataFromAPI(string symbol, string interval)
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

        private List<StockData> getOhclFromAPI(string symbol)
        {
            string QUERY_URL = $"https://www.alphavantage.co/query?function=TIME_SERIES_WEEKLY&symbol={symbol}&apikey=7NHB0CFU57FSN2R2";
            Uri queryUri = new Uri(QUERY_URL);
            using (WebClient client = new WebClient())
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                dynamic json_data = js.Deserialize(client.DownloadString(queryUri), typeof(object));
                return convertJsonToStockData(json_data);
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

        private List<StockData> convertJsonToStockData(dynamic json_data)
        {
            List<StockData> StockData = new List<StockData>();

            int i = 0;
            foreach (Dictionary<string, object> stocks in json_data.Values)
            {
                if (i == 1)
                {
                    foreach (string key in stocks.Keys)
                    {

                        Dictionary<string, object> stock = (Dictionary<string, object>)stocks[key];
                        StockData.Add(new StockData
                        {
                            DateTime = key,
                            Open = double.Parse(stock["1. open"].ToString().Replace(".", ",")),
                            High = double.Parse(stock["2. high"].ToString().Replace(".", ",")),
                            Low = double.Parse(stock["3. low"].ToString().Replace(".", ",")),
                            Close = double.Parse(stock["4. close"].ToString().Replace(".", ",")),
                            Volume = double.Parse(stock["5. volume"].ToString().Replace(".", ","))
                        });

                    }
                }
                i++;
            }

            return StockData;
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

        private void displayStockDataInOhclChart(List<StockData> stockData)
        {
            List<OhlcPoint> ohclPoints = new List<OhlcPoint>();
            foreach (StockData stock in stockData)
            {
                ohclPoints.Add(new OhlcPoint(stock.Open, stock.High, stock.Low, stock.Close));
            }
            cartesianChart1.Dispatcher.Invoke(() =>
            {
                SeriesCollection = new SeriesCollection
                {
                    new OhlcSeries()
                    {
                        Values = new ChartValues<OhlcPoint>(ohclPoints)
                    },
                };
                OnPropertyChanged("SeriesCollection");
            }, System.Windows.Threading.DispatcherPriority.ContextIdle);
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
}
