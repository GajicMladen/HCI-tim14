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
        public TableViewWindow()
        {
            InitializeComponent();
            tableView.ItemsSource = MainWindow.SMAdata; 
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
