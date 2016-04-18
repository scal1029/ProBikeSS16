using System;
using System.Collections.Generic;
using System.Globalization;
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
using ProBikeSS16;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace ProBikeSS16
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            foreach (MenuItem item in menuItemLanguages.Items)
            {
                if (item.Tag.ToString().Equals(CultureInfo.CurrentUICulture.Name))
                    item.IsChecked = true;
            }
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void LanguageHeader_Click(object sender, RoutedEventArgs e)
        {
            // Uncheck each item
            foreach (MenuItem item in menuItemLanguages.Items)
            {
                item.IsChecked = false;
            }

            MenuItem mi = sender as MenuItem;
            mi.IsChecked = true;
            App.Instance.SwitchLanguage(mi.Tag.ToString());        
        }

        #region Save ForecastAndSales
        public int? StockChildBike;
        public int? StockMaleBike;
        public int? StockFemaleBike;
        public int? SaleChildBikeN;
        public int? SaleChildBikeN1;
        public int? SaleChildBikeN2;
        public int? SaleChildBikeN3;
        public int? SaleFemaleBikeN;
        public int? SaleFemaleBikeN1;
        public int? SaleFemaleBikeN2;
        public int? SaleFemaleBikeN3;
        public int? SaleMaleBikeN;
        public int? SaleMaleBikeN1;
        public int? SaleMaleBikeN2;
        public int? SaleMaleBikeN3;

        private void SaveSafetyStockAndSales_OnClick(object sender, RoutedEventArgs e)
        {
            if (!SafetyStockChildBike.Value.HasValue || !SafetyStockFemaleBike.Value.HasValue || !SafetyStockMaleBike.Value.HasValue || !SalesChildBikeN.Value.HasValue ||
                !SalesChildBikeN1.Value.HasValue || !SalesChildBikeN2.Value.HasValue || !SalesChildBikeN3.Value.HasValue || !SalesFemaleBikeN.Value.HasValue ||
                !SalesFemaleBikeN1.Value.HasValue || !SalesFemaleBikeN2.Value.HasValue || !SalesFemaleBikeN3.Value.HasValue || !SalesMaleBikeN.Value.HasValue ||
                !SalesMaleBikeN1.Value.HasValue || !SalesMaleBikeN2.Value.HasValue || !SalesMaleBikeN3.Value.HasValue)
            {
                MessageBox.Show("Error");
            }
            else
            {
                StockChildBike = SafetyStockChildBike.Value;
                StockFemaleBike = SafetyStockFemaleBike.Value;
                StockMaleBike = SafetyStockMaleBike.Value;
                SaleChildBikeN = SalesChildBikeN.Value;
                SaleChildBikeN1 = SalesChildBikeN1.Value;
                SaleChildBikeN2 = SalesChildBikeN2.Value;
                SaleChildBikeN3 = SalesChildBikeN3.Value;
                SaleFemaleBikeN = SalesFemaleBikeN.Value;
                SaleFemaleBikeN1 = SalesFemaleBikeN1.Value;
                SaleFemaleBikeN2 = SalesFemaleBikeN2.Value;
                SaleFemaleBikeN3 = SalesFemaleBikeN3.Value;
                SaleMaleBikeN = SalesMaleBikeN.Value;
                SaleMaleBikeN1 = SalesMaleBikeN1.Value;
                SaleMaleBikeN2 = SalesMaleBikeN2.Value;
                SaleMaleBikeN3 = SalesMaleBikeN3.Value;
                MessageBox.Show(StockChildBike.Value.ToString()+" Success");
                
            }

        }
        #endregion 
    }
}
