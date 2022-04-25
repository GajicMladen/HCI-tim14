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
using Separator = LiveCharts.Wpf.Separator;
using System.Windows.Input;
using System.Text.RegularExpressions;
//using System.Data.Linq;

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
        public static List<SMAdata> SMAdata { get; set; }
        public static List<StockData> StockData { get; set; }

        private string _dataPeriod;
        public string DataPeriod
        {
            set
            {
                if (_dataPeriod != value)
                {
                    _dataPeriod = value;
                }
            }
            get
            {
                return _dataPeriod;
            }
        }

        private string _symbol = "";
        public string Symbol
        {
            set
            {
                if (_symbol != value)
                {
                    _symbol = value;
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
                }

            }
            get
            {
                return _interval;
            }
        }

        private int _timePeriod = 10;

        public int TimePeriod
        {
            set
            {
                if (_timePeriod != value)
                {
                    _timePeriod = value;
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

            cartesianChart1.AxisY.Clear();
            cartesianChart1.AxisY.Add(
                new Axis
                {
                    MinValue = 0
                }
            );

            symbol_cmbx.Items.Add("IBM");
            symbol_cmbx.Items.Add("TSCO.LON");
            symbol_cmbx.Items.Add("SHOP.TRT");
            symbol_cmbx.Items.Add("GPV.TRV");
            symbol_cmbx.Items.Add("DAI.DEX");
            symbol_cmbx.Items.Add("RELIANCE.BSE");

            interval_cmbx.Items.Add("1min");
            interval_cmbx.Items.Add("5min");
            interval_cmbx.Items.Add("15min");
            interval_cmbx.Items.Add("30min");
            interval_cmbx.Items.Add("60min");
            interval_cmbx.Items.Add("daily");
            interval_cmbx.Items.Add("weekly");
            interval_cmbx.Items.Add("monthly");
            interval_cmbx.SelectedIndex = 6;

            cartesianChart1.DisableAnimations = true;
            cartesianChart2.DisableAnimations = true;

            //================= API for SMA ===============
            Thread t = new Thread(getAndDisplayData);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void getAndDisplayData()
        {

            //dodati jos neophodnih parametara
            try
            {
                hideErrAPImsg();
                startLoadingAnimation();
                SMAdata = getSMAdataFromAPI(Symbol, Interval, TimePeriod.ToString(), SeriesType);
                StockData = getOhclFromAPI(Symbol);
                FilterData(SMAdata, StockData);

                displayMetaData();
                displaySMAdataInLineChart(SMAdata);
                displayStockDataInOhclChart(StockData);

                stopLoadingAnimation();
            }
            catch {
                errAPI();
            }
            
        }

        private void hideErrAPImsg() {

            errAPI_lbl.Dispatcher.Invoke(() =>
            {
                errAPI_lbl.Visibility = Visibility.Hidden;

            });
            errtAPI_lbl.Dispatcher.Invoke(() =>
            {
                errtAPI_lbl.Visibility = Visibility.Hidden;

            });
        }

        private void errAPI()
        {
            errAPI_lbl.Dispatcher.Invoke(() =>
            {
                errAPI_lbl.Visibility = Visibility.Visible;

            });
            errtAPI_lbl.Dispatcher.Invoke(() =>
            {
                errtAPI_lbl.Visibility = Visibility.Visible;

            });
            loadingChart2_img.Dispatcher.Invoke(()=> {
                loadingChart2_img.Visibility = Visibility.Hidden;
            });

        }

        /// <summary>
        /// Funkcija koja vraca listu SMA objekata sa API-ja u zavisnosti od prosledjenih parametara 
        /// </summary>
        /// TODO : dodati sve neophodne parametre i dinamicki kreirati QUERY_URL
        public List<SMAdata> getSMAdataFromAPI(string symbol, string interval, string timePeriod, string series_type)
        {
            string QUERY_URL = $"https://www.alphavantage.co/query?function=SMA&symbol={symbol}&interval={interval}&time_period={timePeriod}&series_type={series_type}&apikey=DEC66JZYOJHHO5PC";
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
            try
            {
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
            }
            catch {
                errorFromAPI();
            }
            return MySMAs;
        }

        private void errorFromAPI() {



        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
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

        private void FilterData(List<SMAdata> _SMAdata, List<StockData> _stockData)
        {
            List<SMAdata> newSMAdata = new List<SMAdata>();
            List<StockData> newStockData = new List<StockData>();

            if (DataPeriod.Contains("1 year"))
            {
                var now = DateTime.Now.AddYears(-1);
                foreach (SMAdata sma in _SMAdata)
                {
                    if (DateTime.Compare(now, DateTime.Parse(sma.DateTime)) < 0)
                    {
                        newSMAdata.Add(sma);
                    }
                }
                foreach (StockData stock in _stockData)
                {
                    if (DateTime.Compare(now, DateTime.Parse(stock.DateTime)) < 0)
                    {
                        newStockData.Add(stock);
                    }
                }
                SMAdata = newSMAdata;
                StockData = newStockData;


            }
            else if (DataPeriod.Contains("2 year"))
            {
                var now = DateTime.Now.AddYears(-2);
                foreach (SMAdata sma in _SMAdata)
                {
                    if (DateTime.Compare(now, DateTime.Parse(sma.DateTime)) < 0)
                    {
                        newSMAdata.Add(sma);
                    }
                }
                foreach (StockData stock in _stockData)
                {
                    if (DateTime.Compare(now, DateTime.Parse(stock.DateTime)) < 0)
                    {
                        newStockData.Add(stock);
                    }
                }
                SMAdata = newSMAdata;
                StockData = newStockData;
            }
        }
        private void displaySMAdataInLineChart(List<SMAdata> MySMAs)
        {
            List<string> labele2 = new List<string>();
            List<double> valuesForChartSMA = new List<double>();
            
            foreach (SMAdata sma in MySMAs)
            {
                valuesForChartSMA.Add(sma.SMAvalue);
                labele2.Add(sma.DateTime);
            }
            
            cartesianChart2.Dispatcher.Invoke(() => {

                var seriesSMA = new LiveCharts.Wpf.LineSeries()
                {
                    Values = new LiveCharts.ChartValues<double>(valuesForChartSMA),
                    PointGeometry = System.Windows.Media.Geometry.Empty,
                    Title = "SMA value"

                };

                int step = labele2.Count / 5;
                LiveCharts.Wpf.Axis ax = new LiveCharts.Wpf.Axis()
                {
                    Title = "Date time",
                    FontSize = 10,
                    Separator = new Separator { Step = step, IsEnabled = false },
                    LabelsRotation = 18,
                    ShowLabels = true,
                    Labels = labele2,
                };



                cartesianChart2.Series.Clear();
                cartesianChart2.Series.Add(seriesSMA);

                cartesianChart2.AxisX.Clear();
                cartesianChart2.AxisX.Add(ax);

            }, System.Windows.Threading.DispatcherPriority.ContextIdle);

        }


        private void displayStockDataInOhclChart(List<StockData> stockData)
        {
            List<OhlcPoint> ohclPoints = new List<OhlcPoint>();

            List<string> labele1 = new List<string>();
            foreach (StockData stock in stockData)
            {
                ohclPoints.Add(new OhlcPoint(stock.Open, stock.High, stock.Low, stock.Close));
                labele1.Add(stock.DateTime);
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
                int step = labele1.Count / 5;
                LiveCharts.Wpf.Axis ax = new LiveCharts.Wpf.Axis()
                {
                    Title = "Date time",
                    FontSize = 10,
                    Separator = new Separator { Step = step, IsEnabled = false },
                    LabelsRotation = 18,
                    ShowLabels = true,
                    Labels = labele1,
                };
                cartesianChart1.AxisX.Clear();
                cartesianChart1.AxisX.Add(ax);

                OnPropertyChanged("SeriesCollection");
            }, System.Windows.Threading.DispatcherPriority.ContextIdle);
        }

        private void displayMetaData()
        {
            OnPropertyChanged("Symbol");
            OnPropertyChanged("Indicator");
            OnPropertyChanged("LastRefreshed");
            OnPropertyChanged("Interval");
            OnPropertyChanged("TimePeriod");
            OnPropertyChanged("SeriesType");
            OnPropertyChanged("TimeZone");
        }

        private void startLoadingAnimation()
        {

            cartesianChart2.Dispatcher.Invoke(() => {
                cartesianChart2.Visibility = Visibility.Hidden;
            });

            cartesianChart1.Dispatcher.Invoke(() => {
                cartesianChart1.Visibility = Visibility.Hidden;
            });
            loadingChart2_img.Dispatcher.Invoke(() => {

                loadingChart2_img.Visibility = Visibility.Visible;
            });

        }

        private void stopLoadingAnimation()
        {

            cartesianChart1.Dispatcher.Invoke(() => {
                cartesianChart1.Visibility = Visibility.Visible;
            });
            cartesianChart2.Dispatcher.Invoke(() => {
                cartesianChart2.Visibility = Visibility.Visible;
            });
            loadingChart2_img.Dispatcher.Invoke(() => {

                loadingChart2_img.Visibility = Visibility.Hidden;
            });

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(getAndDisplayData);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();


        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Symbol = symbol_cmbx.SelectedItem.ToString();
        }

        private void interval_cmbx_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Interval = interval_cmbx.SelectedItem.ToString();
        }

        private void TableView_Click(object sender, RoutedEventArgs e)
        {
            TableViewWindow tableViewWindow = new TableViewWindow(Symbol, Indicator, LastRefreshed, Interval, SeriesType, TimePeriod.ToString(), TimeZone);
            tableViewWindow.Show();
        }

        private void SeriesTypeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var RadioButton = (RadioButton)sender;

            SeriesType = RadioButton.Content.ToString();
        }

        private void time_period_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            int value;
            bool correct = int.TryParse(time_period_tb.Text.ToString(), out value);

            if (correct)
            {
                TimePeriod = value;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var RadioButton = (RadioButton)sender;

            DataPeriod = RadioButton.Content.ToString();
        }
    }
}

