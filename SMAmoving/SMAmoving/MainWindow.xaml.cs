using System;
using System.Windows;
using System.Net;
using System.Web.Script.Serialization;
using System.Collections.Generic;

namespace SMAmoving
{
    public partial class MainWindow : Window
    {        
        public static List<StockData> MyStocks = new List<StockData>();

        public MainWindow()
        {
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
            //====================TEST dijagram 1================================


            //================= API ===============
            string QUERY_URL = "https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=IBM&apikey=demo";
            Uri queryUri = new Uri(QUERY_URL);
            
            using (WebClient client = new WebClient())
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                dynamic json_data = js.Deserialize(client.DownloadString(queryUri), typeof(object));
                convertJsonToStockData(json_data);
               
            }

        }

        /// <summary>
        /// Funkcija koja popunjava staticku listu MyStocks sa prosledjenim JSON stringom 
        /// </summary>
        private static void convertJsonToStockData(dynamic json_data) {

            MyStocks.Clear();
            
            int i = 0;
            foreach (Dictionary<string, object> stocks in json_data.Values)
            {
                //Prvi (0) deo JSON stringa sadrzi opste informacije o dobavljenim podacima
                //A drugi (1) deo sadrzi same podatke!  
                if (i == 1)
                {
                    foreach (string key in stocks.Keys)
                    {

                        Dictionary<string, object> stock = (Dictionary<string, object>)stocks[key];
                        MyStocks.Add(new StockData
                        {
                            name = key,
                            open = double.Parse(stock["1. open"].ToString().Replace(".", ",")),
                            high = double.Parse(stock["2. high"].ToString().Replace(".", ",")),
                            low = double.Parse(stock["3. low"].ToString().Replace(".", ",")),
                            close = double.Parse(stock["4. close"].ToString().Replace(".", ",")),
                            volume = double.Parse(stock["5. volume"].ToString().Replace(".", ","))
                        });

                    }
                }
                i++;
            }
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
        //====================TEST dijagram 1================================
        
    }
}
