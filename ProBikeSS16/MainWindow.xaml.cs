﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml;
using System.Xml.Linq;
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
                GlobalVariables.StockChildBike = SafetyStockChildBike.Value;
                GlobalVariables.StockFemaleBike = SafetyStockFemaleBike.Value;
                GlobalVariables.StockMaleBike = SafetyStockMaleBike.Value;
                GlobalVariables.SaleChildBikeN = SalesChildBikeN.Value;
                GlobalVariables.SaleChildBikeN1 = SalesChildBikeN1.Value;
                GlobalVariables.SaleChildBikeN2 = SalesChildBikeN2.Value;
                GlobalVariables.SaleChildBikeN3 = SalesChildBikeN3.Value;
                GlobalVariables.SaleFemaleBikeN = SalesFemaleBikeN.Value;
                GlobalVariables.SaleFemaleBikeN1 = SalesFemaleBikeN1.Value;
                GlobalVariables.SaleFemaleBikeN2 = SalesFemaleBikeN2.Value;
                GlobalVariables.SaleFemaleBikeN3 = SalesFemaleBikeN3.Value;
                GlobalVariables.SaleMaleBikeN = SalesMaleBikeN.Value;
                GlobalVariables.SaleMaleBikeN1 = SalesMaleBikeN1.Value;
                GlobalVariables.SaleMaleBikeN2 = SalesMaleBikeN2.Value;
                GlobalVariables.SaleMaleBikeN3 = SalesMaleBikeN3.Value;
                MessageBox.Show(GlobalVariables.StockChildBike.Value.ToString()+" Success");
                
            }

        }
        #endregion

        private void XmlUpload_OnClick(object sender, RoutedEventArgs e)
        {
            

            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".xml";
            dlg.Filter = "xml files (.xml)|*.xml";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                try
                {
                    GlobalVariables.InputXML = XDocument.Load(filename);
                    XMLPath.Text = filename;
                }
                catch (XmlException exception)
                {
                    XMLPath.Text = null;
                    MessageBox.Show("Your XML was probably bad...");
                }
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
