using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Media.Animation;
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
                MessageBox.Show("Error: Neue Werte nicht übernommen");
                GlobalVariables.ForecastCorrect = false;
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
                GlobalVariables.ForecastCorrect = true;
                XMLImage.Visibility = Visibility.Visible;
                XMLPath.Visibility = Visibility.Visible;
                XMLLabel.Visibility = Visibility.Visible;
                XmlUpload.Visibility = Visibility.Visible;
                Okay1.Visibility = Visibility.Visible;
                DoubleAnimation animation = new DoubleAnimation(0,TimeSpan.FromSeconds(3));
                Okay1.BeginAnimation(Image.OpacityProperty, animation);
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
            bool? result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                try
                {

                    XmlDocument doc = new XmlDocument();
                    doc.Load(filename);
                    XmlNodeList nodes = doc.SelectNodes("//completedorders");
                    for (int i = nodes.Count - 1; i >= 0; i--)
                    {
                        nodes[i].ParentNode.RemoveChild(nodes[i]);
                    }
                    doc.Save(filename);

                    GlobalVariables.InputXML = XDocument.Load(filename);
                    XMLPath.Text = filename;
                    GlobalVariables.XMLCorrect = true;
                    CalculationStart.Visibility = Visibility.Visible;
                    Okay2.Visibility = Visibility.Visible;
                    DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(3));
                    Okay2.BeginAnimation(Image.OpacityProperty, animation);
                }
                catch (XmlException exception)
                {
                    XMLPath.Text = null;
                    MessageBox.Show("Your XML was probably bad...");
                    GlobalVariables.XMLCorrect = false;
                    CalculationStart.Visibility = Visibility.Hidden;
                }
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
            //&& (int.Parse(e.Text)%10==0);
        }

        private void CalculationStart_OnClick(object sender, RoutedEventArgs e)
        {
            if (GlobalVariables.XMLCorrect && GlobalVariables.ForecastCorrect)
            {
                GlobalVariables.InputDataSetWithoutOldBatchCalc = DataTableStuff.ReadXMLtoDataSet(XMLPath.Text);
                GridOldStock.DataContext = GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables[2].DefaultView;
                Programmplannung(GlobalVariables.InputDataSetWithoutOldBatchCalc);
            }
            else
            {
                MessageBox.Show("Mistakes");
            }
        }

        #region Data

        private void Programmplannung(DataSet data)
        {
            //GeplanterVerkauf
            ChildBikeOrderP1.Text = GlobalVariables.SaleChildBikeN.ToString();
            FemaleBikeOrderP2.Text = GlobalVariables.SaleFemaleBikeN.ToString();
            MaleBikeOrderP3.Text = GlobalVariables.SaleMaleBikeN.ToString();
            //Geplanter Sicherheitsbestand
            #region SafetyStock
            //P1
            ChildBikeSafetyP1.Text = GlobalVariables.StockChildBike.ToString();
            ChildBikeSafetyE26.Text = ChildBikeSafetyP1.Text;
            ChildBikeSafetyE51.Text = ChildBikeSafetyP1.Text;
            ChildBikeSafetyE16.Text = ChildBikeSafetyP1.Text;
            ChildBikeSafetyE17.Text = ChildBikeSafetyP1.Text;
            ChildBikeSafetyE50.Text = ChildBikeSafetyP1.Text;
            ChildBikeSafetyE4.Text = ChildBikeSafetyP1.Text;
            ChildBikeSafetyE10.Text = ChildBikeSafetyP1.Text;
            ChildBikeSafetyE49.Text = ChildBikeSafetyP1.Text;
            ChildBikeSafetyE7.Text = ChildBikeSafetyP1.Text;
            ChildBikeSafetyE13.Text = ChildBikeSafetyP1.Text;
            ChildBikeSafetyE18.Text = ChildBikeSafetyP1.Text;
            //P2
            FemaleBikeSafetyP2.Text = GlobalVariables.StockFemaleBike.ToString();
            FemaleBikeSafetyE26.Text = FemaleBikeSafetyP2.Text;
            FemaleBikeSafetyE56.Text = FemaleBikeSafetyP2.Text;
            FemaleBikeSafetyE16.Text = FemaleBikeSafetyP2.Text;
            FemaleBikeSafetyE17.Text = FemaleBikeSafetyP2.Text;
            FemaleBikeSafetyE55.Text = FemaleBikeSafetyP2.Text;
            FemaleBikeSafetyE5.Text = FemaleBikeSafetyP2.Text;
            FemaleBikeSafetyE11.Text = FemaleBikeSafetyP2.Text;
            FemaleBikeSafetyE54.Text = FemaleBikeSafetyP2.Text;
            FemaleBikeSafetyE8.Text = FemaleBikeSafetyP2.Text;
            FemaleBikeSafetyE14.Text = FemaleBikeSafetyP2.Text;
            FemaleBikeSafetyE19.Text = FemaleBikeSafetyP2.Text;
            //P3
            MaleBikeSafetyP3.Text = GlobalVariables.StockMaleBike.ToString();
            MaleBikeSafetyE26.Text = MaleBikeSafetyP3.Text;
            MaleBikeSafetyE31.Text = MaleBikeSafetyP3.Text;
            MaleBikeSafetyE16.Text = MaleBikeSafetyP3.Text;
            MaleBikeSafetyE17.Text = MaleBikeSafetyP3.Text;
            MaleBikeSafetyE30.Text = MaleBikeSafetyP3.Text;
            MaleBikeSafetyE6.Text = MaleBikeSafetyP3.Text;
            MaleBikeSafetyE12.Text = MaleBikeSafetyP3.Text;
            MaleBikeSafetyE29.Text = MaleBikeSafetyP3.Text;
            MaleBikeSafetyE9.Text = MaleBikeSafetyP3.Text;
            MaleBikeSafetyE15.Text = MaleBikeSafetyP3.Text;
            MaleBikeSafetyE20.Text = MaleBikeSafetyP3.Text;
            #endregion SafetyStock
            //Stock
            #region Stock
            //P123
            DataRow[] results = data.Tables[2].Select("id = '16'");
            int E16 = int.Parse(results[0].ItemArray[1].ToString());
            int E16P1 = 0;
            int E16P2 = 0;
            int E16P3 = 0;
            while(E16>0)
            {
                if (E16 > 0)
                {
                    E16P1 = E16P1 + 10;
                    E16 = E16 - 10;
                }
                if (E16 > 0)
                {
                    E16P2 = E16P2 + 10;
                    E16 = E16 - 10;
                }
                if (E16 > 0)
                {
                    E16P3 = E16P3 + 10;
                    E16 = E16 - 10;
                }
            }
            ChildBikeStockE16.Text = E16P1.ToString();
            FemaleBikeStockE16.Text = E16P2.ToString();
            MaleBikeStockE16.Text = E16P3.ToString();

            results = data.Tables[2].Select("id = '17'");
            int E17 = int.Parse(results[0].ItemArray[1].ToString());
            int E17P1 = 0;
            int E17P2 = 0;
            int E17P3 = 0;
            while (E17 > 0)
            {
                if (E17 > 0)
                {
                    E17P1 = E17P1 + 10;
                    E17 = E17 - 10;
                }
                if (E17 > 0)
                {
                    E17P2 = E17P2 + 10;
                    E17 = E17 - 10;
                }
                if (E17 > 0)
                {
                    E17P3 = E17P3 + 10;
                    E17 = E17 - 10;
                }
            }
            ChildBikeStockE17.Text = E17P1.ToString();
            FemaleBikeStockE17.Text = E17P2.ToString();
            MaleBikeStockE17.Text = E17P3.ToString();

            results = data.Tables[2].Select("id = '26'");
            int E26 = int.Parse(results[0].ItemArray[1].ToString());
            int E26P1 = 0;
            int E26P2 = 0;
            int E26P3 = 0;
            while (E26 > 0)
            {
                if (E26 > 0)
                {
                    E26P1 = E26P1 + 10;
                    E26 = E26 - 10;
                }
                if (E26 > 0)
                {
                    E26P2 = E26P2 + 10;
                    E26 = E26 - 10;
                }
                if (E26 > 0)
                {
                    E26P3 = E26P3 + 10;
                    E26 = E26 - 10;
                }
            }
            ChildBikeStockE26.Text = E26P1.ToString();
            FemaleBikeStockE26.Text = E26P2.ToString();
            MaleBikeStockE26.Text = E26P3.ToString();
            //P1
            results = data.Tables[2].Select("id = '1'");
            ChildBikeStockP1.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '51'");
            ChildBikeStockE51.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '50'");
            ChildBikeStockE50.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '4'");
            ChildBikeStockE4.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '10'");
            ChildBikeStockE10.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '49'");
            ChildBikeStockE49.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '7'");
            ChildBikeStockE7.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '13'");
            ChildBikeStockE13.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '18'");
            ChildBikeStockE18.Text = results[0].ItemArray[1].ToString();
            //P2
            results = data.Tables[2].Select("id = '2'");
            FemaleBikeStockP2.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '56'");
            FemaleBikeStockE56.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '55'");
            FemaleBikeStockE55.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '5'");
            FemaleBikeStockE5.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '11'");
            FemaleBikeStockE11.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '54'");
            FemaleBikeStockE54.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '8'");
            FemaleBikeStockE8.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '14'");
            FemaleBikeStockE14.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '19'");
            FemaleBikeStockE19.Text = results[0].ItemArray[1].ToString();
            //P3
            results = data.Tables[2].Select("id = '3'");
            MaleBikeStockP3.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '31'");
            MaleBikeStockE31.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '30'");
            MaleBikeStockE30.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '6'");
            MaleBikeStockE6.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '12'");
            MaleBikeStockE12.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '29'");
            MaleBikeStockE29.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '9'");
            MaleBikeStockE9.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '15'");
            MaleBikeStockE15.Text = results[0].ItemArray[1].ToString();
            results = data.Tables[2].Select("id = '20'");
            MaleBikeStockE20.Text = results[0].ItemArray[1].ToString();
            #endregion Stock
            //WarteSchlange
            #region Waitlist
            //P123
            ChildBikeWaitlistE16.Text = "0";
            FemaleBikeWaitlistE16.Text = "0";
            MaleBikeWaitlistE16.Text = "0";
            results = data.Tables[10].Select("item = '16'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                int WE16 = Int32.Parse(results[0].ItemArray[5].ToString());
                int WE16P1 = 0;
                int WE16P2 = 0;
                int WE16P3 = 0;
                while (WE16 > 0)
                {
                    if (WE16 > 0)
                    {
                        WE16P1 = WE16P1 + 10;
                        WE16 = WE16 - 10;
                    }
                    if (WE16 > 0)
                    {
                        WE16P2 = WE16P2 + 10;
                        WE16 = WE16 - 10;
                    }
                    if (WE16 > 0)
                    {
                        WE16P3 = WE16P3 + 10;
                        WE16 = WE16 - 10;
                    }
                }
                ChildBikeWaitlistE16.Text = WE16P1.ToString();
                FemaleBikeWaitlistE16.Text = WE16P2.ToString();
                MaleBikeWaitlistE16.Text = WE16P3.ToString();
            }
            ChildBikeWaitlistE17.Text = "0";
            FemaleBikeWaitlistE17.Text = "0";
            MaleBikeWaitlistE17.Text = "0";
            results = data.Tables[10].Select("item = '17'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                int WE17 = Int32.Parse(results[0].ItemArray[5].ToString());
                int WE17P1 = 0;
                int WE17P2 = 0;
                int WE17P3 = 0;
                while (WE17 > 0)
                {
                    if (WE17 > 0)
                    {
                        WE17P1 = WE17P1 + 10;
                        WE17 = WE17 - 10;
                    }
                    if (WE17 > 0)
                    {
                        WE17P2 = WE17P2 + 10;
                        WE17 = WE17 - 10;
                    }
                    if (WE17 > 0)
                    {
                        WE17P3 = WE17P3 + 10;
                        WE17 = WE17 - 10;
                    }
                }
                ChildBikeWaitlistE17.Text = WE17P1.ToString();
                FemaleBikeWaitlistE17.Text = WE17P2.ToString();
                MaleBikeWaitlistE17.Text = WE17P3.ToString();
            }
            ChildBikeWaitlistE26.Text = "0";
            FemaleBikeWaitlistE26.Text = "0";
            MaleBikeWaitlistE26.Text = "0";
            results = data.Tables[10].Select("item = '26'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                int WE26 = Int32.Parse(results[0].ItemArray[5].ToString());
                int WE26P1 = 0;
                int WE26P2 = 0;
                int WE26P3 = 0;
                while (WE26 > 0)
                {
                    if (WE26 > 0)
                    {
                        WE26P1 = WE26P1 + 10;
                        WE26 = WE26 - 10;
                    }
                    if (WE26 > 0)
                    {
                        WE26P2 = WE26P2 + 10;
                        WE26 = WE26 - 10;
                    }
                    if (WE26 > 0)
                    {
                        WE26P3 = WE26P3 + 10;
                        WE26 = WE26 - 10;
                    }
                }
                ChildBikeWaitlistE26.Text = WE26P1.ToString();
                FemaleBikeWaitlistE26.Text = WE26P2.ToString();
                MaleBikeWaitlistE26.Text = WE26P3.ToString();
            }
            //P1
            ChildBikeWaitlistP1.Text = "0";
            results = data.Tables[10].Select("item = '1'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                ChildBikeWaitlistP1.Text = results[0].ItemArray[5].ToString();
            }
            ChildBikeWaitlistE51.Text = "0";
            results = data.Tables[10].Select("item = '51'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                ChildBikeWaitlistE51.Text = results[0].ItemArray[5].ToString();
            }
            ChildBikeWaitlistE50.Text = "0";
            results = data.Tables[10].Select("item = '50'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                ChildBikeWaitlistE50.Text = results[0].ItemArray[5].ToString();
            }
            ChildBikeWaitlistE4.Text = "0";
            results = data.Tables[10].Select("item = '4'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                ChildBikeWaitlistE4.Text = results[0].ItemArray[5].ToString();
            }
            ChildBikeWaitlistE10.Text = "0";
            results = data.Tables[10].Select("item = '10'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                ChildBikeWaitlistE10.Text = results[0].ItemArray[5].ToString();
            }
            ChildBikeWaitlistE49.Text = "0";
            results = data.Tables[10].Select("item = '49'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                ChildBikeWaitlistE49.Text = results[0].ItemArray[5].ToString();
            }
            ChildBikeWaitlistE7.Text = "0";
            results = data.Tables[10].Select("item = '7'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                ChildBikeWaitlistE7.Text = results[0].ItemArray[5].ToString();
            }
            ChildBikeWaitlistE13.Text = "0";
            results = data.Tables[10].Select("item = '13'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                ChildBikeWaitlistE13.Text = results[0].ItemArray[5].ToString();
            }
            ChildBikeWaitlistE18.Text = "0";
            results = data.Tables[10].Select("item = '18'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                ChildBikeWaitlistE18.Text = results[0].ItemArray[5].ToString();
            }
            //P2
            FemaleBikeWaitlistP2.Text = "0";
            results = data.Tables[10].Select("item = '2'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                FemaleBikeWaitlistP2.Text = results[0].ItemArray[5].ToString();
            }
            FemaleBikeWaitlistE56.Text = "0";
            results = data.Tables[10].Select("item = '56'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                FemaleBikeWaitlistE56.Text = results[0].ItemArray[5].ToString();
            }
            FemaleBikeWaitlistE55.Text = "0";
            results = data.Tables[10].Select("item = '55'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                FemaleBikeWaitlistE55.Text = results[0].ItemArray[5].ToString();
            }
            FemaleBikeWaitlistE5.Text = "0";
            results = data.Tables[10].Select("item = '5'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                FemaleBikeWaitlistE5.Text = results[0].ItemArray[5].ToString();
            }
            FemaleBikeWaitlistE11.Text = "0";
            results = data.Tables[10].Select("item = '11'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                FemaleBikeWaitlistE11.Text = results[0].ItemArray[5].ToString();
            }
            FemaleBikeWaitlistE54.Text = "0";
            results = data.Tables[10].Select("item = '54'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                FemaleBikeWaitlistE54.Text = results[0].ItemArray[5].ToString();
            }
            FemaleBikeWaitlistE8.Text = "0";
            results = data.Tables[10].Select("item = '8'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                FemaleBikeWaitlistE8.Text = results[0].ItemArray[5].ToString();
            }
            FemaleBikeWaitlistE14.Text = "0";
            results = data.Tables[10].Select("item = '14'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                FemaleBikeWaitlistE14.Text = results[0].ItemArray[5].ToString();
            }
            FemaleBikeWaitlistE19.Text = "0";
            results = data.Tables[10].Select("item = '19'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                FemaleBikeWaitlistE19.Text = results[0].ItemArray[5].ToString();
            }
            //P3
            MaleBikeWaitlistP3.Text = "0";
            results = data.Tables[10].Select("item = '3'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                MaleBikeWaitlistP3.Text = results[0].ItemArray[5].ToString();
            }
            MaleBikeWaitlistE31.Text = "0";
            results = data.Tables[10].Select("item = '31'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                MaleBikeWaitlistE31.Text = results[0].ItemArray[5].ToString();
            }
            MaleBikeWaitlistE30.Text = "0";
            results = data.Tables[10].Select("item = '30'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                MaleBikeWaitlistE30.Text = results[0].ItemArray[5].ToString();
            }
            MaleBikeWaitlistE6.Text = "0";
            results = data.Tables[10].Select("item = '6'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                MaleBikeWaitlistE6.Text = results[0].ItemArray[5].ToString();
            }
            MaleBikeWaitlistE12.Text = "0";
            results = data.Tables[10].Select("item = '12'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                MaleBikeWaitlistE12.Text = results[0].ItemArray[5].ToString();
            }
            MaleBikeWaitlistE29.Text = "0";
            results = data.Tables[10].Select("item = '29'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                MaleBikeWaitlistE29.Text = results[0].ItemArray[5].ToString();
            }
            MaleBikeWaitlistE9.Text = "0";
            results = data.Tables[10].Select("item = '9'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                MaleBikeWaitlistE9.Text = results[0].ItemArray[5].ToString();
            }
            MaleBikeWaitlistE15.Text = "0";
            results = data.Tables[10].Select("item = '15'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                MaleBikeWaitlistE15.Text = results[0].ItemArray[5].ToString();
            }
            MaleBikeWaitlistE20.Text = "0";
            results = data.Tables[10].Select("item = '20'");
            if (results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
            {
                MaleBikeWaitlistE20.Text = results[0].ItemArray[5].ToString();
            }
            #endregion Waitlist
            //InBearbeitung
            #region InProduction
            //P123
            ChildBikeInProductionE16.Text = "0";
            FemaleBikeInProductionE16.Text = "0";
            MaleBikeInProductionE16.Text = "0";
            results = data.Tables[7].Select("item = '16'");
            if (results.Length > 0)
            {
                int E16inP = ((results.Length) * 10);
                int E16inP1 = 0;
                int E16inP2 = 0;
                int E16inP3 = 0;
                while (E16inP > 0)
                {
                    if (E16inP > 0)
                    {
                        E16inP1 = E16inP1 + 10;
                        E16inP = E16inP - 10;
                    }
                    if (E16inP > 0)
                    {
                        E16inP2 = E16inP2 + 10;
                        E16inP = E16inP - 10;
                    }
                    if (E16inP > 0)
                    {
                        E16inP3 = E16inP3 + 10;
                        E16inP = E16inP - 10;
                    }
                }
                ChildBikeInProductionE16.Text = E16inP1.ToString();
                FemaleBikeInProductionE16.Text = E16inP2.ToString();
                MaleBikeInProductionE16.Text = E16inP3.ToString();
            }
            ChildBikeInProductionE17.Text = "0";
            FemaleBikeInProductionE17.Text = "0";
            MaleBikeInProductionE17.Text = "0";
            results = data.Tables[7].Select("item = '17'");
            if (results.Length > 0)
            {
                int E17inP = ((results.Length) * 10);
                int E17inP1 = 0;
                int E17inP2 = 0;
                int E17inP3 = 0;
                while (E17inP > 0)
                {
                    if (E17inP > 0)
                    {
                        E17inP1 = E17inP1 + 10;
                        E17inP = E17inP - 10;
                    }
                    if (E17inP > 0)
                    {
                        E17inP2 = E17inP2 + 10;
                        E17inP = E17inP - 10;
                    }
                    if (E17inP > 0)
                    {
                        E17inP3 = E17inP3 + 10;
                        E17inP = E17inP - 10;
                    }
                }
                ChildBikeInProductionE17.Text = E17inP1.ToString();
                FemaleBikeInProductionE17.Text = E17inP2.ToString();
                MaleBikeInProductionE17.Text = E17inP3.ToString();
            }
            ChildBikeInProductionE26.Text = "0";
            FemaleBikeInProductionE26.Text = "0";
            MaleBikeInProductionE26.Text = "0";
            results = data.Tables[7].Select("item = '26'");
            if (results.Length > 0)
            {
                int E26inP = ((results.Length) * 10);
                int E26inP1 = 0;
                int E26inP2 = 0;
                int E26inP3 = 0;
                while (E26inP > 0)
                {
                    if (E26inP > 0)
                    {
                        E26inP1 = E26inP1 + 10;
                        E26inP = E26inP - 10;
                    }
                    if (E26inP > 0)
                    {
                        E26inP2 = E26inP2 + 10;
                        E26inP = E26inP - 10;
                    }
                    if (E26inP > 0)
                    {
                        E26inP3 = E26inP3 + 10;
                        E26inP = E26inP - 10;
                    }
                }
                ChildBikeInProductionE26.Text = E26inP1.ToString();
                FemaleBikeInProductionE26.Text = E26inP2.ToString();
                MaleBikeInProductionE26.Text = E26inP3.ToString();
            }
            //P1
            ChildBikeInProductionP1.Text = "0";
            results = data.Tables[7].Select("item = '1'");
            if (results.Length > 0)
            {
                ChildBikeInProductionP1.Text = ((results.Length) * 10).ToString();
            }
            ChildBikeInProductionE51.Text = "0";
            results = data.Tables[7].Select("item = '51'");
            if (results.Length > 0)
            {
                ChildBikeInProductionE51.Text = ((results.Length) * 10).ToString();
            }
            ChildBikeInProductionE50.Text = "0";
            results = data.Tables[7].Select("item = '50'");
            if (results.Length > 0)
            {
                ChildBikeInProductionE50.Text = ((results.Length) * 10).ToString();
            }
            ChildBikeInProductionE4.Text = "0";
            results = data.Tables[7].Select("item = '4'");
            if (results.Length > 0)
            {
                ChildBikeInProductionE4.Text = ((results.Length) * 10).ToString();
            }
            ChildBikeInProductionE10.Text = "0";
            results = data.Tables[7].Select("item = '10'");
            if (results.Length > 0)
            {
                ChildBikeInProductionE10.Text = ((results.Length) * 10).ToString();
            }
            ChildBikeInProductionE49.Text = "0";
            results = data.Tables[7].Select("item = '49'");
            if (results.Length > 0)
            {
                ChildBikeInProductionE49.Text = ((results.Length) * 10).ToString();
            }
            ChildBikeInProductionE7.Text = "0";
            results = data.Tables[7].Select("item = '7'");
            if (results.Length > 0)
            {
                ChildBikeInProductionE7.Text = ((results.Length) * 10).ToString();
            }
            ChildBikeInProductionE13.Text = "0";
            results = data.Tables[7].Select("item = '13'");
            if (results.Length > 0)
            {
                ChildBikeInProductionE13.Text = ((results.Length) * 10).ToString();
            }
            ChildBikeInProductionE18.Text = "0";
            results = data.Tables[7].Select("item = '18'");
            if (results.Length > 0)
            {
                ChildBikeInProductionE18.Text = ((results.Length) * 10).ToString();
            }
            //P2
            FemaleBikeInProductionP2.Text = "0";
            results = data.Tables[7].Select("item = '2'");
            if (results.Length > 0)
            {
                FemaleBikeInProductionP2.Text = ((results.Length)*10).ToString();
            }
            FemaleBikeInProductionE56.Text = "0";
            results = data.Tables[7].Select("item = '56'");
            if (results.Length > 0)
            {
                FemaleBikeInProductionE56.Text = ((results.Length) * 10).ToString();
            }
            FemaleBikeInProductionE55.Text = "0";
            results = data.Tables[7].Select("item = '55'");
            if (results.Length > 0)
            {
                FemaleBikeInProductionE55.Text = ((results.Length) * 10).ToString();
            }
            FemaleBikeInProductionE5.Text = "0";
            results = data.Tables[7].Select("item = '5'");
            if (results.Length > 0)
            {
                FemaleBikeInProductionE5.Text = ((results.Length) * 10).ToString();
            }
            FemaleBikeInProductionE11.Text = "0";
            results = data.Tables[7].Select("item = '11'");
            if (results.Length > 0)
            {
                FemaleBikeInProductionE11.Text = ((results.Length) * 10).ToString();
            }
            FemaleBikeInProductionE54.Text = "0";
            results = data.Tables[7].Select("item = '54'");
            if (results.Length > 0)
            {
                FemaleBikeInProductionE54.Text = ((results.Length) * 10).ToString();
            }
            FemaleBikeInProductionE8.Text = "0";
            results = data.Tables[7].Select("item = '8'");
            if (results.Length > 0)
            {
                FemaleBikeInProductionE8.Text = ((results.Length) * 10).ToString();
            }
            FemaleBikeInProductionE14.Text = "0";
            results = data.Tables[7].Select("item = '14'");
            if (results.Length > 0)
            {
                FemaleBikeInProductionE14.Text = ((results.Length) * 10).ToString();
            }
            FemaleBikeInProductionE19.Text = "0";
            results = data.Tables[7].Select("item = '19'");
            if (results.Length > 0)
            {
                FemaleBikeInProductionE19.Text = ((results.Length) * 10).ToString();
            }
            //P3
            MaleBikeInProductionP3.Text = "0";
            results = data.Tables[7].Select("item = '3'");
            if (results.Length > 0)
            {
                MaleBikeInProductionP3.Text = ((results.Length) * 10).ToString();
            }
            MaleBikeInProductionE31.Text = "0";
            results = data.Tables[7].Select("item = '31'");
            if (results.Length > 0)
            {
                MaleBikeInProductionE31.Text = ((results.Length) * 10).ToString();
            }
            MaleBikeInProductionE30.Text = "0";
            results = data.Tables[7].Select("item = '30'");
            if (results.Length > 0)
            {
                MaleBikeInProductionE30.Text = ((results.Length) * 10).ToString();
            }
            MaleBikeInProductionE6.Text = "0";
            results = data.Tables[7].Select("item = '6'");
            if (results.Length > 0)
            {
                MaleBikeInProductionE6.Text = ((results.Length) * 10).ToString();
            }
            MaleBikeInProductionE12.Text = "0";
            results = data.Tables[7].Select("item = '12'");
            if (results.Length > 0)
            {
                MaleBikeInProductionE12.Text = ((results.Length) * 10).ToString();
            }
            MaleBikeInProductionE29.Text = "0";
            results = data.Tables[7].Select("item = '29'");
            if (results.Length > 0)
            {
                MaleBikeInProductionE29.Text = ((results.Length) * 10).ToString();
            }
            MaleBikeInProductionE9.Text = "0";
            results = data.Tables[7].Select("item = '9'");
            if (results.Length > 0)
            {
                MaleBikeInProductionE9.Text = ((results.Length) * 10).ToString();
            }
            MaleBikeInProductionE15.Text = "0";
            results = data.Tables[7].Select("item = '15'");
            if (results.Length > 0)
            {
                MaleBikeInProductionE15.Text = ((results.Length) * 10).ToString();
            }
            MaleBikeInProductionE20.Text = "0";
            results = data.Tables[7].Select("item = '20'");
            if (results.Length > 0)
            {
                MaleBikeInProductionE20.Text = ((results.Length) * 10).ToString();
            }
            #endregion InProduction

            //All data ready: Calculations
            #region ProgrammplannungKalkulation
            //P1
            //1. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.P1Produktionsauftrag = int.Parse(ChildBikeOrderP1.Text) + int.Parse(ChildBikeSafetyP1.Text) -
                                                   int.Parse(ChildBikeStockP1.Text) -
                                                   int.Parse(ChildBikeWaitlistP1.Text) -
                                                   int.Parse(ChildBikeInProductionP1.Text);
            ChildBikePlannedProductionP1.Text = GlobalVariables.P1Produktionsauftrag.ToString();
            ChildBikeOrderE26.Text = ChildBikePlannedProductionP1.Text;
            ChildBikeOrderE51.Text = ChildBikePlannedProductionP1.Text;
            ChildBikePassedWaitlistE26.Text = ChildBikeWaitlistP1.Text;
            ChildBikePassedWaitlistE51.Text = ChildBikeWaitlistP1.Text;
            //2. und 3. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.P1E26Produktionsauftrag = int.Parse(ChildBikeOrderE26.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE26.Text) +
                                                      int.Parse(ChildBikeSafetyE26.Text) -
                                                      int.Parse(ChildBikeStockE26.Text) -
                                                      int.Parse(ChildBikeWaitlistE26.Text) -
                                                      int.Parse(ChildBikeInProductionE26.Text);
            ChildBikePlannedProductionE26.Text = GlobalVariables.P1E26Produktionsauftrag.ToString();
            GlobalVariables.E51Produktionsauftrag = int.Parse(ChildBikeOrderE51.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE51.Text) +
                                                      int.Parse(ChildBikeSafetyE51.Text) -
                                                      int.Parse(ChildBikeStockE51.Text) -
                                                      int.Parse(ChildBikeWaitlistE51.Text) -
                                                      int.Parse(ChildBikeInProductionE51.Text);
            ChildBikePlannedProductionE51.Text = GlobalVariables.E51Produktionsauftrag.ToString();
            ChildBikeOrderE16.Text = ChildBikePlannedProductionE51.Text;
            ChildBikeOrderE17.Text = ChildBikePlannedProductionE51.Text;
            ChildBikeOrderE50.Text = ChildBikePlannedProductionE51.Text;
            ChildBikePassedWaitlistE16.Text = ChildBikeWaitlistE51.Text;
            ChildBikePassedWaitlistE17.Text = ChildBikeWaitlistE51.Text;
            ChildBikePassedWaitlistE50.Text = ChildBikeWaitlistE51.Text;
            //4.-6. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.P1E16Produktionsauftrag = int.Parse(ChildBikeOrderE16.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE16.Text) +
                                                      int.Parse(ChildBikeSafetyE16.Text) -
                                                      int.Parse(ChildBikeStockE16.Text) -
                                                      int.Parse(ChildBikeWaitlistE16.Text) -
                                                      int.Parse(ChildBikeInProductionE16.Text);
            ChildBikePlannedProductionE16.Text = GlobalVariables.P1E16Produktionsauftrag.ToString();
            GlobalVariables.P1E17Produktionsauftrag = int.Parse(ChildBikeOrderE17.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE17.Text) +
                                                      int.Parse(ChildBikeSafetyE17.Text) -
                                                      int.Parse(ChildBikeStockE17.Text) -
                                                      int.Parse(ChildBikeWaitlistE17.Text) -
                                                      int.Parse(ChildBikeInProductionE17.Text);
            ChildBikePlannedProductionE17.Text = GlobalVariables.P1E17Produktionsauftrag.ToString();
            GlobalVariables.E50Produktionsauftrag = int.Parse(ChildBikeOrderE50.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE50.Text) +
                                                      int.Parse(ChildBikeSafetyE50.Text) -
                                                      int.Parse(ChildBikeStockE50.Text) -
                                                      int.Parse(ChildBikeWaitlistE50.Text) -
                                                      int.Parse(ChildBikeInProductionE50.Text);
            ChildBikePlannedProductionE50.Text = GlobalVariables.E50Produktionsauftrag.ToString();
            ChildBikeOrderE4.Text = ChildBikePlannedProductionE50.Text;
            ChildBikeOrderE10.Text = ChildBikePlannedProductionE50.Text;
            ChildBikeOrderE49.Text = ChildBikePlannedProductionE50.Text;
            ChildBikePassedWaitlistE4.Text = ChildBikeWaitlistE50.Text;
            ChildBikePassedWaitlistE10.Text = ChildBikeWaitlistE50.Text;
            ChildBikePassedWaitlistE49.Text = ChildBikeWaitlistE50.Text;
            //7.-9. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.E4Produktionsauftrag = int.Parse(ChildBikeOrderE4.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE4.Text) +
                                                      int.Parse(ChildBikeSafetyE4.Text) -
                                                      int.Parse(ChildBikeStockE4.Text) -
                                                      int.Parse(ChildBikeWaitlistE4.Text) -
                                                      int.Parse(ChildBikeInProductionE4.Text);
            ChildBikePlannedProductionE4.Text = GlobalVariables.E4Produktionsauftrag.ToString();
            GlobalVariables.E10Produktionsauftrag = int.Parse(ChildBikeOrderE10.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE10.Text) +
                                                      int.Parse(ChildBikeSafetyE10.Text) -
                                                      int.Parse(ChildBikeStockE10.Text) -
                                                      int.Parse(ChildBikeWaitlistE10.Text) -
                                                      int.Parse(ChildBikeInProductionE10.Text);
            ChildBikePlannedProductionE10.Text = GlobalVariables.E10Produktionsauftrag.ToString();
            GlobalVariables.E49Produktionsauftrag = int.Parse(ChildBikeOrderE49.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE49.Text) +
                                                      int.Parse(ChildBikeSafetyE49.Text) -
                                                      int.Parse(ChildBikeStockE49.Text) -
                                                      int.Parse(ChildBikeWaitlistE49.Text) -
                                                      int.Parse(ChildBikeInProductionE49.Text);
            ChildBikePlannedProductionE49.Text = GlobalVariables.E49Produktionsauftrag.ToString();
            ChildBikeOrderE7.Text = ChildBikePlannedProductionE49.Text;
            ChildBikeOrderE13.Text = ChildBikePlannedProductionE49.Text;
            ChildBikeOrderE18.Text = ChildBikePlannedProductionE49.Text;
            ChildBikePassedWaitlistE7.Text = ChildBikeWaitlistE49.Text;
            ChildBikePassedWaitlistE13.Text = ChildBikeWaitlistE49.Text;
            ChildBikePassedWaitlistE18.Text = ChildBikeWaitlistE49.Text;
            //10.-12. Zeile ausrechnen
            GlobalVariables.E7Produktionsauftrag = int.Parse(ChildBikeOrderE7.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE7.Text) +
                                                      int.Parse(ChildBikeSafetyE7.Text) -
                                                      int.Parse(ChildBikeStockE7.Text) -
                                                      int.Parse(ChildBikeWaitlistE7.Text) -
                                                      int.Parse(ChildBikeInProductionE7.Text);
            ChildBikePlannedProductionE7.Text = GlobalVariables.E7Produktionsauftrag.ToString();
            GlobalVariables.E13Produktionsauftrag = int.Parse(ChildBikeOrderE13.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE13.Text) +
                                                      int.Parse(ChildBikeSafetyE13.Text) -
                                                      int.Parse(ChildBikeStockE13.Text) -
                                                      int.Parse(ChildBikeWaitlistE13.Text) -
                                                      int.Parse(ChildBikeInProductionE13.Text);
            ChildBikePlannedProductionE13.Text = GlobalVariables.E13Produktionsauftrag.ToString();
            GlobalVariables.E18Produktionsauftrag = int.Parse(ChildBikeOrderE18.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE18.Text) +
                                                      int.Parse(ChildBikeSafetyE18.Text) -
                                                      int.Parse(ChildBikeStockE18.Text) -
                                                      int.Parse(ChildBikeWaitlistE18.Text) -
                                                      int.Parse(ChildBikeInProductionE18.Text);
            ChildBikePlannedProductionE18.Text = GlobalVariables.E18Produktionsauftrag.ToString();
            //P2
            //1. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.P2Produktionsauftrag = int.Parse(FemaleBikeOrderP2.Text) + int.Parse(FemaleBikeSafetyP2.Text) -
                                                   int.Parse(FemaleBikeStockP2.Text) -
                                                   int.Parse(FemaleBikeWaitlistP2.Text) -
                                                   int.Parse(FemaleBikeInProductionP2.Text);
            FemaleBikePlannedProductionP2.Text = GlobalVariables.P2Produktionsauftrag.ToString();
            FemaleBikeOrderE26.Text = FemaleBikePlannedProductionP2.Text;
            FemaleBikeOrderE56.Text = FemaleBikePlannedProductionP2.Text;
            FemaleBikePassedWaitlistE26.Text = FemaleBikeWaitlistP2.Text;
            FemaleBikePassedWaitlistE56.Text = FemaleBikeWaitlistP2.Text;
            //2. und 3. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.P2E26Produktionsauftrag = int.Parse(FemaleBikeOrderE26.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE26.Text) +
                                                      int.Parse(FemaleBikeSafetyE26.Text) -
                                                      int.Parse(FemaleBikeStockE26.Text) -
                                                      int.Parse(FemaleBikeWaitlistE26.Text) -
                                                      int.Parse(FemaleBikeInProductionE26.Text);
            FemaleBikePlannedProductionE26.Text = GlobalVariables.P2E26Produktionsauftrag.ToString();
            GlobalVariables.E56Produktionsauftrag = int.Parse(FemaleBikeOrderE56.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE56.Text) +
                                                      int.Parse(FemaleBikeSafetyE56.Text) -
                                                      int.Parse(FemaleBikeStockE56.Text) -
                                                      int.Parse(FemaleBikeWaitlistE56.Text) -
                                                      int.Parse(FemaleBikeInProductionE56.Text);
            FemaleBikePlannedProductionE56.Text = GlobalVariables.E56Produktionsauftrag.ToString();
            FemaleBikeOrderE16.Text = FemaleBikePlannedProductionE56.Text;
            FemaleBikeOrderE17.Text = FemaleBikePlannedProductionE56.Text;
            FemaleBikeOrderE55.Text = FemaleBikePlannedProductionE56.Text;
            FemaleBikePassedWaitlistE16.Text = FemaleBikeWaitlistE56.Text;
            FemaleBikePassedWaitlistE17.Text = FemaleBikeWaitlistE56.Text;
            FemaleBikePassedWaitlistE55.Text = FemaleBikeWaitlistE56.Text;
            //4.-6. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.P2E16Produktionsauftrag = int.Parse(FemaleBikeOrderE16.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE16.Text) +
                                                      int.Parse(FemaleBikeSafetyE16.Text) -
                                                      int.Parse(FemaleBikeStockE16.Text) -
                                                      int.Parse(FemaleBikeWaitlistE16.Text) -
                                                      int.Parse(FemaleBikeInProductionE16.Text);
            FemaleBikePlannedProductionE16.Text = GlobalVariables.P2E16Produktionsauftrag.ToString();
            GlobalVariables.P2E17Produktionsauftrag = int.Parse(FemaleBikeOrderE17.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE17.Text) +
                                                      int.Parse(FemaleBikeSafetyE17.Text) -
                                                      int.Parse(FemaleBikeStockE17.Text) -
                                                      int.Parse(FemaleBikeWaitlistE17.Text) -
                                                      int.Parse(FemaleBikeInProductionE17.Text);
            FemaleBikePlannedProductionE17.Text = GlobalVariables.P2E17Produktionsauftrag.ToString();
            GlobalVariables.E55Produktionsauftrag = int.Parse(FemaleBikeOrderE55.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE55.Text) +
                                                      int.Parse(FemaleBikeSafetyE55.Text) -
                                                      int.Parse(FemaleBikeStockE55.Text) -
                                                      int.Parse(FemaleBikeWaitlistE55.Text) -
                                                      int.Parse(FemaleBikeInProductionE55.Text);
            FemaleBikePlannedProductionE55.Text = GlobalVariables.E55Produktionsauftrag.ToString();
            FemaleBikeOrderE5.Text = FemaleBikePlannedProductionE55.Text;
            FemaleBikeOrderE11.Text = FemaleBikePlannedProductionE55.Text;
            FemaleBikeOrderE54.Text = FemaleBikePlannedProductionE55.Text;
            FemaleBikePassedWaitlistE5.Text = FemaleBikeWaitlistE55.Text;
            FemaleBikePassedWaitlistE11.Text = FemaleBikeWaitlistE55.Text;
            FemaleBikePassedWaitlistE54.Text = FemaleBikeWaitlistE55.Text;
            //7.-9. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.E5Produktionsauftrag = int.Parse(FemaleBikeOrderE5.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE5.Text) +
                                                      int.Parse(FemaleBikeSafetyE5.Text) -
                                                      int.Parse(FemaleBikeStockE5.Text) -
                                                      int.Parse(FemaleBikeWaitlistE5.Text) -
                                                      int.Parse(FemaleBikeInProductionE5.Text);
            FemaleBikePlannedProductionE5.Text = GlobalVariables.E5Produktionsauftrag.ToString();
            GlobalVariables.E11Produktionsauftrag = int.Parse(FemaleBikeOrderE11.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE11.Text) +
                                                      int.Parse(FemaleBikeSafetyE11.Text) -
                                                      int.Parse(FemaleBikeStockE11.Text) -
                                                      int.Parse(FemaleBikeWaitlistE11.Text) -
                                                      int.Parse(FemaleBikeInProductionE11.Text);
            FemaleBikePlannedProductionE11.Text = GlobalVariables.E11Produktionsauftrag.ToString();
            GlobalVariables.E54Produktionsauftrag = int.Parse(FemaleBikeOrderE54.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE54.Text) +
                                                      int.Parse(FemaleBikeSafetyE54.Text) -
                                                      int.Parse(FemaleBikeStockE54.Text) -
                                                      int.Parse(FemaleBikeWaitlistE54.Text) -
                                                      int.Parse(FemaleBikeInProductionE54.Text);
            FemaleBikePlannedProductionE54.Text = GlobalVariables.E54Produktionsauftrag.ToString();
            FemaleBikeOrderE8.Text = FemaleBikePlannedProductionE54.Text;
            FemaleBikeOrderE14.Text = FemaleBikePlannedProductionE54.Text;
            FemaleBikeOrderE19.Text = FemaleBikePlannedProductionE54.Text;
            FemaleBikePassedWaitlistE8.Text = FemaleBikeWaitlistE54.Text;
            FemaleBikePassedWaitlistE14.Text = FemaleBikeWaitlistE54.Text;
            FemaleBikePassedWaitlistE19.Text = FemaleBikeWaitlistE54.Text;
            //10.-12. Zeile ausrechnen
            GlobalVariables.E8Produktionsauftrag = int.Parse(FemaleBikeOrderE8.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE8.Text) +
                                                      int.Parse(FemaleBikeSafetyE8.Text) -
                                                      int.Parse(FemaleBikeStockE8.Text) -
                                                      int.Parse(FemaleBikeWaitlistE8.Text) -
                                                      int.Parse(FemaleBikeInProductionE8.Text);
            FemaleBikePlannedProductionE8.Text = GlobalVariables.E8Produktionsauftrag.ToString();
            GlobalVariables.E14Produktionsauftrag = int.Parse(FemaleBikeOrderE14.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE14.Text) +
                                                      int.Parse(FemaleBikeSafetyE14.Text) -
                                                      int.Parse(FemaleBikeStockE14.Text) -
                                                      int.Parse(FemaleBikeWaitlistE14.Text) -
                                                      int.Parse(FemaleBikeInProductionE14.Text);
            FemaleBikePlannedProductionE14.Text = GlobalVariables.E14Produktionsauftrag.ToString();
            GlobalVariables.E19Produktionsauftrag = int.Parse(FemaleBikeOrderE19.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE19.Text) +
                                                      int.Parse(FemaleBikeSafetyE19.Text) -
                                                      int.Parse(FemaleBikeStockE19.Text) -
                                                      int.Parse(FemaleBikeWaitlistE19.Text) -
                                                      int.Parse(FemaleBikeInProductionE19.Text);
            FemaleBikePlannedProductionE19.Text = GlobalVariables.E19Produktionsauftrag.ToString();
            //P3
            //1. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.P3Produktionsauftrag = int.Parse(MaleBikeOrderP3.Text) + int.Parse(MaleBikeSafetyP3.Text) -
                                                   int.Parse(MaleBikeStockP3.Text) -
                                                   int.Parse(MaleBikeWaitlistP3.Text) -
                                                   int.Parse(MaleBikeInProductionP3.Text);
            MaleBikePlannedProductionP3.Text = GlobalVariables.P3Produktionsauftrag.ToString();
            MaleBikeOrderE26.Text = MaleBikePlannedProductionP3.Text;
            MaleBikeOrderE31.Text = MaleBikePlannedProductionP3.Text;
            MaleBikePassedWaitlistE26.Text = MaleBikeWaitlistP3.Text;
            MaleBikePassedWaitlistE31.Text = MaleBikeWaitlistP3.Text;
            //2. und 3. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.P3E26Produktionsauftrag = int.Parse(MaleBikeOrderE26.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE26.Text) +
                                                      int.Parse(MaleBikeSafetyE26.Text) -
                                                      int.Parse(MaleBikeStockE26.Text) -
                                                      int.Parse(MaleBikeWaitlistE26.Text) -
                                                      int.Parse(MaleBikeInProductionE26.Text);
            MaleBikePlannedProductionE26.Text = GlobalVariables.P3E26Produktionsauftrag.ToString();
            GlobalVariables.E31Produktionsauftrag = int.Parse(MaleBikeOrderE31.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE31.Text) +
                                                      int.Parse(MaleBikeSafetyE31.Text) -
                                                      int.Parse(MaleBikeStockE31.Text) -
                                                      int.Parse(MaleBikeWaitlistE31.Text) -
                                                      int.Parse(MaleBikeInProductionE31.Text);
            MaleBikePlannedProductionE31.Text = GlobalVariables.E31Produktionsauftrag.ToString();
            MaleBikeOrderE16.Text = MaleBikePlannedProductionE31.Text;
            MaleBikeOrderE17.Text = MaleBikePlannedProductionE31.Text;
            MaleBikeOrderE30.Text = MaleBikePlannedProductionE31.Text;
            MaleBikePassedWaitlistE16.Text = MaleBikeWaitlistE31.Text;
            MaleBikePassedWaitlistE17.Text = MaleBikeWaitlistE31.Text;
            MaleBikePassedWaitlistE30.Text = MaleBikeWaitlistE31.Text;
            //4.-6. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.P3E16Produktionsauftrag = int.Parse(MaleBikeOrderE16.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE16.Text) +
                                                      int.Parse(MaleBikeSafetyE16.Text) -
                                                      int.Parse(MaleBikeStockE16.Text) -
                                                      int.Parse(MaleBikeWaitlistE16.Text) -
                                                      int.Parse(MaleBikeInProductionE16.Text);
            MaleBikePlannedProductionE16.Text = GlobalVariables.P3E16Produktionsauftrag.ToString();
            GlobalVariables.P3E17Produktionsauftrag = int.Parse(MaleBikeOrderE17.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE17.Text) +
                                                      int.Parse(MaleBikeSafetyE17.Text) -
                                                      int.Parse(MaleBikeStockE17.Text) -
                                                      int.Parse(MaleBikeWaitlistE17.Text) -
                                                      int.Parse(MaleBikeInProductionE17.Text);
            MaleBikePlannedProductionE17.Text = GlobalVariables.P3E17Produktionsauftrag.ToString();
            GlobalVariables.E30Produktionsauftrag = int.Parse(MaleBikeOrderE30.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE30.Text) +
                                                      int.Parse(MaleBikeSafetyE30.Text) -
                                                      int.Parse(MaleBikeStockE30.Text) -
                                                      int.Parse(MaleBikeWaitlistE30.Text) -
                                                      int.Parse(MaleBikeInProductionE30.Text);
            MaleBikePlannedProductionE30.Text = GlobalVariables.E30Produktionsauftrag.ToString();
            MaleBikeOrderE6.Text = MaleBikePlannedProductionE30.Text;
            MaleBikeOrderE12.Text = MaleBikePlannedProductionE30.Text;
            MaleBikeOrderE29.Text = MaleBikePlannedProductionE30.Text;
            MaleBikePassedWaitlistE6.Text = MaleBikeWaitlistE30.Text;
            MaleBikePassedWaitlistE12.Text = MaleBikeWaitlistE30.Text;
            MaleBikePassedWaitlistE29.Text = MaleBikeWaitlistE30.Text;
            //7.-9. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.E6Produktionsauftrag = int.Parse(MaleBikeOrderE6.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE6.Text) +
                                                      int.Parse(MaleBikeSafetyE6.Text) -
                                                      int.Parse(MaleBikeStockE6.Text) -
                                                      int.Parse(MaleBikeWaitlistE6.Text) -
                                                      int.Parse(MaleBikeInProductionE6.Text);
            MaleBikePlannedProductionE6.Text = GlobalVariables.E6Produktionsauftrag.ToString();
            GlobalVariables.E12Produktionsauftrag = int.Parse(MaleBikeOrderE12.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE12.Text) +
                                                      int.Parse(MaleBikeSafetyE12.Text) -
                                                      int.Parse(MaleBikeStockE12.Text) -
                                                      int.Parse(MaleBikeWaitlistE12.Text) -
                                                      int.Parse(MaleBikeInProductionE12.Text);
            MaleBikePlannedProductionE12.Text = GlobalVariables.E12Produktionsauftrag.ToString();
            GlobalVariables.E29Produktionsauftrag = int.Parse(MaleBikeOrderE29.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE29.Text) +
                                                      int.Parse(MaleBikeSafetyE29.Text) -
                                                      int.Parse(MaleBikeStockE29.Text) -
                                                      int.Parse(MaleBikeWaitlistE29.Text) -
                                                      int.Parse(MaleBikeInProductionE29.Text);
            MaleBikePlannedProductionE29.Text = GlobalVariables.E29Produktionsauftrag.ToString();
            MaleBikeOrderE9.Text = MaleBikePlannedProductionE29.Text;
            MaleBikeOrderE15.Text = MaleBikePlannedProductionE29.Text;
            MaleBikeOrderE20.Text = MaleBikePlannedProductionE29.Text;
            MaleBikePassedWaitlistE9.Text = MaleBikeWaitlistE29.Text;
            MaleBikePassedWaitlistE15.Text = MaleBikeWaitlistE29.Text;
            MaleBikePassedWaitlistE20.Text = MaleBikeWaitlistE29.Text;
            //10.-12. Zeile ausrechnen
            GlobalVariables.E9Produktionsauftrag = int.Parse(MaleBikeOrderE9.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE9.Text) +
                                                      int.Parse(MaleBikeSafetyE9.Text) -
                                                      int.Parse(MaleBikeStockE9.Text) -
                                                      int.Parse(MaleBikeWaitlistE9.Text) -
                                                      int.Parse(MaleBikeInProductionE9.Text);
            MaleBikePlannedProductionE9.Text = GlobalVariables.E9Produktionsauftrag.ToString();
            GlobalVariables.E15Produktionsauftrag = int.Parse(MaleBikeOrderE15.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE15.Text) +
                                                      int.Parse(MaleBikeSafetyE15.Text) -
                                                      int.Parse(MaleBikeStockE15.Text) -
                                                      int.Parse(MaleBikeWaitlistE15.Text) -
                                                      int.Parse(MaleBikeInProductionE15.Text);
            MaleBikePlannedProductionE15.Text = GlobalVariables.E15Produktionsauftrag.ToString();
            GlobalVariables.E20Produktionsauftrag = int.Parse(MaleBikeOrderE20.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE20.Text) +
                                                      int.Parse(MaleBikeSafetyE20.Text) -
                                                      int.Parse(MaleBikeStockE20.Text) -
                                                      int.Parse(MaleBikeWaitlistE20.Text) -
                                                      int.Parse(MaleBikeInProductionE20.Text);
            MaleBikePlannedProductionE20.Text = GlobalVariables.E20Produktionsauftrag.ToString();


            //Produktionsaufträge für von allen Fahrrädern verwendeten Teilen ausrechnen
            GlobalVariables.E16Produktionsauftrag = GlobalVariables.P1E16Produktionsauftrag +
                                                    GlobalVariables.P2E16Produktionsauftrag +
                                                    GlobalVariables.P3E16Produktionsauftrag;
            GlobalVariables.E17Produktionsauftrag = GlobalVariables.P1E17Produktionsauftrag +
                                                    GlobalVariables.P2E17Produktionsauftrag +
                                                    GlobalVariables.P3E17Produktionsauftrag;
            GlobalVariables.E26Produktionsauftrag = GlobalVariables.P1E26Produktionsauftrag +
                                                    GlobalVariables.P2E26Produktionsauftrag +
                                                    GlobalVariables.P3E26Produktionsauftrag;
            #endregion ProgrammplannungKalkulation

            //Datagrid ProdOrders
            #region DataTable
            
            GlobalVariables.dtProdOrder.Clear();
            if(!GlobalVariables.dtProdOrder.Columns.Contains("Item"))
                GlobalVariables.dtProdOrder.Columns.Add("Item");
            if (!GlobalVariables.dtProdOrder.Columns.Contains("Amount"))
                GlobalVariables.dtProdOrder.Columns.Add("Amount");

            //Enter all Rows
            DataRow P1P = GlobalVariables.dtProdOrder.NewRow();
            P1P["Item"] = "1";
            if(int.Parse(ChildBikePlannedProductionP1.Text) > 0)
                P1P["Amount"] = int.Parse(ChildBikePlannedProductionP1.Text);
            else
                P1P["Amount"] = 0;

            DataRow P2P = GlobalVariables.dtProdOrder.NewRow();
            P2P["Item"] = "2";
            if (int.Parse(FemaleBikePlannedProductionP2.Text) > 0)
                P2P["Amount"] = int.Parse(FemaleBikePlannedProductionP2.Text);
            else
                P2P["Amount"] = 0;
            
            DataRow P3P = GlobalVariables.dtProdOrder.NewRow();
            P3P["Item"] = "3";
            if (int.Parse(MaleBikePlannedProductionP3.Text) > 0)
                P3P["Amount"] = int.Parse(MaleBikePlannedProductionP3.Text);
            else
                P3P["Amount"] = 0;

            DataRow E26P = GlobalVariables.dtProdOrder.NewRow();
            E26P["Item"] = "26";
            if (int.Parse(GlobalVariables.E26Produktionsauftrag.ToString()) > 0)
                E26P["Amount"] = GlobalVariables.E26Produktionsauftrag;
            else
                E26P["Amount"] = 0;

            DataRow E16P = GlobalVariables.dtProdOrder.NewRow();
            E16P["Item"] = "16";
            if (int.Parse(GlobalVariables.E16Produktionsauftrag.ToString()) > 0)
                E16P["Amount"] = GlobalVariables.E16Produktionsauftrag;
            else
                E16P["Amount"] = 0;

            DataRow E17P = GlobalVariables.dtProdOrder.NewRow();
            E17P["Item"] = "17";
            if (int.Parse(GlobalVariables.E17Produktionsauftrag.ToString()) > 0)
                E17P["Amount"] = GlobalVariables.E17Produktionsauftrag;
            else
                E17P["Amount"] = 0;

            DataRow E51P = GlobalVariables.dtProdOrder.NewRow();
            E51P["Item"] = "51";
            if (int.Parse(GlobalVariables.E51Produktionsauftrag.ToString()) > 0)
                E51P["Amount"] = GlobalVariables.E51Produktionsauftrag;
            else
                E51P["Amount"] = 0;

            DataRow E56P = GlobalVariables.dtProdOrder.NewRow();
            E56P["Item"] = "56";
            if (int.Parse(GlobalVariables.E56Produktionsauftrag.ToString()) > 0)
                E56P["Amount"] = GlobalVariables.E56Produktionsauftrag;
            else
                E56P["Amount"] = 0;

            DataRow E31P = GlobalVariables.dtProdOrder.NewRow();
            E31P["Item"] = "31";
            if (int.Parse(GlobalVariables.E31Produktionsauftrag.ToString()) > 0)
                E31P["Amount"] = GlobalVariables.E31Produktionsauftrag;
            else
                E31P["Amount"] = 0;

            DataRow E50P = GlobalVariables.dtProdOrder.NewRow();
            E50P["Item"] = "50";
            if (int.Parse(GlobalVariables.E50Produktionsauftrag.ToString()) > 0)
                E50P["Amount"] = GlobalVariables.E50Produktionsauftrag;
            else
                E50P["Amount"] = 0;

            DataRow E55P = GlobalVariables.dtProdOrder.NewRow();
            E55P["Item"] = "55";
            if (int.Parse(GlobalVariables.E55Produktionsauftrag.ToString()) > 0)
                E55P["Amount"] = GlobalVariables.E55Produktionsauftrag;
            else
                E55P["Amount"] = 0;

            DataRow E30P = GlobalVariables.dtProdOrder.NewRow();
            E30P["Item"] = "30";
            if (int.Parse(GlobalVariables.E30Produktionsauftrag.ToString()) > 0)
                E30P["Amount"] = GlobalVariables.E30Produktionsauftrag;
            else
                E30P["Amount"] = 0;

            DataRow E49P = GlobalVariables.dtProdOrder.NewRow();
            E49P["Item"] = "49";
            if (int.Parse(GlobalVariables.E49Produktionsauftrag.ToString()) > 0)
                E49P["Amount"] = GlobalVariables.E49Produktionsauftrag;
            else
                E49P["Amount"] = 0;

            DataRow E54P = GlobalVariables.dtProdOrder.NewRow();
            E54P["Item"] = "54";
            if (int.Parse(GlobalVariables.E54Produktionsauftrag.ToString()) > 0)
                E54P["Amount"] = GlobalVariables.E54Produktionsauftrag;
            else
                E54P["Amount"] = 0;

            DataRow E29P = GlobalVariables.dtProdOrder.NewRow();
            E29P["Item"] = "29";
            if (int.Parse(GlobalVariables.E29Produktionsauftrag.ToString()) > 0)
                E29P["Amount"] = GlobalVariables.E29Produktionsauftrag;
            else
                E29P["Amount"] = 0;

            DataRow E18P = GlobalVariables.dtProdOrder.NewRow();
            E18P["Item"] = "18";
            if (int.Parse(GlobalVariables.E18Produktionsauftrag.ToString()) > 0)
                E18P["Amount"] = GlobalVariables.E18Produktionsauftrag;
            else
                E18P["Amount"] = 0;

            DataRow E19P = GlobalVariables.dtProdOrder.NewRow();
            E19P["Item"] = "19";
            if (int.Parse(GlobalVariables.E19Produktionsauftrag.ToString()) > 0)
                E19P["Amount"] = GlobalVariables.E19Produktionsauftrag;
            else
                E19P["Amount"] = 0;

            DataRow E20P = GlobalVariables.dtProdOrder.NewRow();
            E20P["Item"] = "20";
            if (int.Parse(GlobalVariables.E20Produktionsauftrag.ToString()) > 0)
                E20P["Amount"] = GlobalVariables.E20Produktionsauftrag;
            else
                E20P["Amount"] = 0;

            DataRow E13P = GlobalVariables.dtProdOrder.NewRow();
            E13P["Item"] = "13";
            if (int.Parse(GlobalVariables.E13Produktionsauftrag.ToString()) > 0)
                E13P["Amount"] = GlobalVariables.E13Produktionsauftrag;
            else
                E13P["Amount"] = 0;

            DataRow E14P = GlobalVariables.dtProdOrder.NewRow();
            E14P["Item"] = "14";
            if (int.Parse(GlobalVariables.E14Produktionsauftrag.ToString()) > 0)
                E14P["Amount"] = GlobalVariables.E14Produktionsauftrag;
            else
                E14P["Amount"] = 0;

            DataRow E15P = GlobalVariables.dtProdOrder.NewRow();
            E15P["Item"] = "15";
            if (int.Parse(GlobalVariables.E15Produktionsauftrag.ToString()) > 0)
                E15P["Amount"] = GlobalVariables.E15Produktionsauftrag;
            else
                E15P["Amount"] = 0;

            DataRow E10P = GlobalVariables.dtProdOrder.NewRow();
            E10P["Item"] = "10";
            if (int.Parse(GlobalVariables.E10Produktionsauftrag.ToString()) > 0)
                E10P["Amount"] = GlobalVariables.E10Produktionsauftrag;
            else
                E10P["Amount"] = 0;

            DataRow E11P = GlobalVariables.dtProdOrder.NewRow();
            E11P["Item"] = "11";
            if (int.Parse(GlobalVariables.E11Produktionsauftrag.ToString()) > 0)
                E11P["Amount"] = GlobalVariables.E11Produktionsauftrag;
            else
                E11P["Amount"] = 0;

            DataRow E12P = GlobalVariables.dtProdOrder.NewRow();
            E12P["Item"] = "12";
            if (int.Parse(GlobalVariables.E12Produktionsauftrag.ToString()) > 0)
                E12P["Amount"] = GlobalVariables.E12Produktionsauftrag;
            else
                E12P["Amount"] = 0;

            DataRow E7P = GlobalVariables.dtProdOrder.NewRow();
            E7P["Item"] = "7";
            if (int.Parse(GlobalVariables.E7Produktionsauftrag.ToString()) > 0)
                E7P["Amount"] = GlobalVariables.E7Produktionsauftrag;
            else
                E7P["Amount"] = 0;

            DataRow E8P = GlobalVariables.dtProdOrder.NewRow();
            E8P["Item"] = "8";
            if (int.Parse(GlobalVariables.E8Produktionsauftrag.ToString()) > 0)
                E8P["Amount"] = GlobalVariables.E8Produktionsauftrag;
            else
                E8P["Amount"] = 0;

            DataRow E9P = GlobalVariables.dtProdOrder.NewRow();
            E9P["Item"] = "9";
            if (int.Parse(GlobalVariables.E9Produktionsauftrag.ToString()) > 0)
                E9P["Amount"] = GlobalVariables.E9Produktionsauftrag;
            else
                E9P["Amount"] = 0;

            DataRow E4P = GlobalVariables.dtProdOrder.NewRow();
            E4P["Item"] = "4";
            if (int.Parse(GlobalVariables.E4Produktionsauftrag.ToString()) > 0)
                E4P["Amount"] = GlobalVariables.E4Produktionsauftrag;
            else
                E4P["Amount"] = 0;

            DataRow E5P = GlobalVariables.dtProdOrder.NewRow();
            E5P["Item"] = "5";
            if (int.Parse(GlobalVariables.E5Produktionsauftrag.ToString()) > 0)
                E5P["Amount"] = GlobalVariables.E5Produktionsauftrag;
            else
                E5P["Amount"] = 0;

            DataRow E6P = GlobalVariables.dtProdOrder.NewRow();
            E6P["Item"] = "6";
            if (int.Parse(GlobalVariables.E6Produktionsauftrag.ToString()) > 0)
                E6P["Amount"] = GlobalVariables.E6Produktionsauftrag;
            else
                E6P["Amount"] = 0;


            //int test = int.Parse(P1P[1].ToString());
            //MessageBox.Show(test.ToString());
            //MIst

            //GlobalVariables.dtProdOrder.Rows.Add(P1P);
            //GlobalVariables.dtProdOrder.Rows.Add(P2P);
            //GlobalVariables.dtProdOrder.Rows.Add(P3P);

            //GlobalVariables.dtProdOrder.Rows.Add(E51P);
            //GlobalVariables.dtProdOrder.Rows.Add(E56P);
            //GlobalVariables.dtProdOrder.Rows.Add(E31P);

            //GlobalVariables.dtProdOrder.Rows.Add(E50P);
            //GlobalVariables.dtProdOrder.Rows.Add(E55P);
            //GlobalVariables.dtProdOrder.Rows.Add(E30P);

            //GlobalVariables.dtProdOrder.Rows.Add(E49P);
            //GlobalVariables.dtProdOrder.Rows.Add(E54P);
            //GlobalVariables.dtProdOrder.Rows.Add(E29P);

            //GlobalVariables.dtProdOrder.Rows.Add(E26P);

            //GlobalVariables.dtProdOrder.Rows.Add(E18P);
            //GlobalVariables.dtProdOrder.Rows.Add(E19P);
            //GlobalVariables.dtProdOrder.Rows.Add(E20P);

            //GlobalVariables.dtProdOrder.Rows.Add(E16P);
            //GlobalVariables.dtProdOrder.Rows.Add(E17P);

            //GlobalVariables.dtProdOrder.Rows.Add(E13P);
            //GlobalVariables.dtProdOrder.Rows.Add(E14P);
            //GlobalVariables.dtProdOrder.Rows.Add(E15P);

            //GlobalVariables.dtProdOrder.Rows.Add(E10P);
            //GlobalVariables.dtProdOrder.Rows.Add(E11P);
            //GlobalVariables.dtProdOrder.Rows.Add(E12P);

            //GlobalVariables.dtProdOrder.Rows.Add(E7P);
            //GlobalVariables.dtProdOrder.Rows.Add(E8P);
            //GlobalVariables.dtProdOrder.Rows.Add(E9P);

            //GlobalVariables.dtProdOrder.Rows.Add(E4P);
            //GlobalVariables.dtProdOrder.Rows.Add(E5P);
            //GlobalVariables.dtProdOrder.Rows.Add(E6P);

            List<DataRow> P1P2P3 = new List<DataRow> { P1P,P2P, P3P};
            List<DataRow> E51E56E31 = new List<DataRow> { E51P, E56P, E31P };
            List<DataRow> E50E55E30 = new List<DataRow> { E50P, E55P, E30P };
            List<DataRow> E49E54E29 = new List<DataRow> { E49P, E54P, E29P };

            List<DataRow> E18E19E20 = new List<DataRow> { E18P, E19P, E20P };
            List<DataRow> E16E17 = new List<DataRow> { E16P, E17P };

            List<DataRow> E13E14E15 = new List<DataRow> { E13P, E14P, E15P };
            List<DataRow> E10E11E12 = new List<DataRow> { E10P, E11P, E12P };
            List<DataRow> E7E8E9 = new List<DataRow> { E7P, E8P, E9P };
            List<DataRow> E4E5E6 = new List<DataRow> { E4P, E5P, E6P };



            //Order Rows according to Settings
            #region Order
            if (BikesFirst.IsChecked)
            {
                if (High.IsChecked)
                {
                    var R123 = P1P2P3.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R123)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R515631 = E51E56E31.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R515631)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R505530 = E50E55E30.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R505530)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R495429 = E49E54E29.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R495429)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    GlobalVariables.dtProdOrder.Rows.Add(E26P);
                    var R181920 = E18E19E20.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R181920)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R16R17 = E16E17.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R16R17)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R131415 = E13E14E15.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R131415)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R101112 = E10E11E12.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R101112)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R789 = E7E8E9.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R789)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R456 = E4E5E6.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R456)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }

                }
                else
                {
                    var R123 = P1P2P3.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R123)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R515631 = E51E56E31.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R515631)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R505530 = E50E55E30.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R505530)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R495429 = E49E54E29.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R495429)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    GlobalVariables.dtProdOrder.Rows.Add(E26P);
                    var R181920 = E18E19E20.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R181920)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R16R17 = E16E17.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R16R17)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R131415 = E13E14E15.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R131415)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R101112 = E10E11E12.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R101112)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R789 = E7E8E9.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R789)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R456 = E4E5E6.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R456)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                }
            }
            else
            {
                if (High.IsChecked)
                {
                    var R456 = E4E5E6.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R456)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R789 = E7E8E9.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R789)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R101112 = E10E11E12.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R101112)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R131415 = E13E14E15.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R131415)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R16R17 = E16E17.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R16R17)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R181920 = E18E19E20.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R181920)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    GlobalVariables.dtProdOrder.Rows.Add(E26P);
                    var R495429 = E49E54E29.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R495429)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R505530 = E50E55E30.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R505530)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R515631 = E51E56E31.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R515631)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R123 = P1P2P3.OrderByDescending(itemArray => itemArray[1]);
                    foreach (DataRow x in R123)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                }
                else
                {
                    var R456 = E4E5E6.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R456)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R789 = E7E8E9.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R789)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R101112 = E10E11E12.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R101112)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R131415 = E13E14E15.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R131415)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R16R17 = E16E17.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R16R17)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R181920 = E18E19E20.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R181920)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    GlobalVariables.dtProdOrder.Rows.Add(E26P);
                    var R495429 = E49E54E29.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R495429)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R505530 = E50E55E30.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R505530)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R515631 = E51E56E31.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R515631)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                    var R123 = P1P2P3.OrderBy(itemArray => itemArray[1]);
                    foreach (DataRow x in R123)
                    {
                        GlobalVariables.dtProdOrder.Rows.Add(x);
                    }
                }
            }
            #endregion Order




            GridProductionOrders.DataContext = GlobalVariables.dtProdOrder.DefaultView;
            #endregion DataTable
        }


        private void ProgrammplannungRepeat(object sender, RoutedEventArgs e)
        {
            #region RepeatProgrammplannungKalkulation
            //All data ready: Calculations
            //P1
            //1. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.P1Produktionsauftrag = int.Parse(ChildBikeOrderP1.Text) + int.Parse(ChildBikeSafetyP1.Text) -
                                                   int.Parse(ChildBikeStockP1.Text) -
                                                   int.Parse(ChildBikeWaitlistP1.Text) -
                                                   int.Parse(ChildBikeInProductionP1.Text);
            ChildBikePlannedProductionP1.Text = GlobalVariables.P1Produktionsauftrag.ToString();
            ChildBikeOrderE26.Text = ChildBikePlannedProductionP1.Text;
            ChildBikeOrderE51.Text = ChildBikePlannedProductionP1.Text;
            ChildBikePassedWaitlistE26.Text = ChildBikeWaitlistP1.Text;
            ChildBikePassedWaitlistE51.Text = ChildBikeWaitlistP1.Text;
            //2. und 3. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.P1E26Produktionsauftrag = int.Parse(ChildBikeOrderE26.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE26.Text) +
                                                      int.Parse(ChildBikeSafetyE26.Text) -
                                                      int.Parse(ChildBikeStockE26.Text) -
                                                      int.Parse(ChildBikeWaitlistE26.Text) -
                                                      int.Parse(ChildBikeInProductionE26.Text);
            ChildBikePlannedProductionE26.Text = GlobalVariables.P1E26Produktionsauftrag.ToString();
            GlobalVariables.E51Produktionsauftrag = int.Parse(ChildBikeOrderE51.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE51.Text) +
                                                      int.Parse(ChildBikeSafetyE51.Text) -
                                                      int.Parse(ChildBikeStockE51.Text) -
                                                      int.Parse(ChildBikeWaitlistE51.Text) -
                                                      int.Parse(ChildBikeInProductionE51.Text);
            ChildBikePlannedProductionE51.Text = GlobalVariables.E51Produktionsauftrag.ToString();
            ChildBikeOrderE16.Text = ChildBikePlannedProductionE51.Text;
            ChildBikeOrderE17.Text = ChildBikePlannedProductionE51.Text;
            ChildBikeOrderE50.Text = ChildBikePlannedProductionE51.Text;
            ChildBikePassedWaitlistE16.Text = ChildBikeWaitlistE51.Text;
            ChildBikePassedWaitlistE17.Text = ChildBikeWaitlistE51.Text;
            ChildBikePassedWaitlistE50.Text = ChildBikeWaitlistE51.Text;
            //4.-6. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.P1E16Produktionsauftrag = int.Parse(ChildBikeOrderE16.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE16.Text) +
                                                      int.Parse(ChildBikeSafetyE16.Text) -
                                                      int.Parse(ChildBikeStockE16.Text) -
                                                      int.Parse(ChildBikeWaitlistE16.Text) -
                                                      int.Parse(ChildBikeInProductionE16.Text);
            ChildBikePlannedProductionE16.Text = GlobalVariables.P1E16Produktionsauftrag.ToString();
            GlobalVariables.P1E17Produktionsauftrag = int.Parse(ChildBikeOrderE17.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE17.Text) +
                                                      int.Parse(ChildBikeSafetyE17.Text) -
                                                      int.Parse(ChildBikeStockE17.Text) -
                                                      int.Parse(ChildBikeWaitlistE17.Text) -
                                                      int.Parse(ChildBikeInProductionE17.Text);
            ChildBikePlannedProductionE17.Text = GlobalVariables.P1E17Produktionsauftrag.ToString();
            GlobalVariables.E50Produktionsauftrag = int.Parse(ChildBikeOrderE50.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE50.Text) +
                                                      int.Parse(ChildBikeSafetyE50.Text) -
                                                      int.Parse(ChildBikeStockE50.Text) -
                                                      int.Parse(ChildBikeWaitlistE50.Text) -
                                                      int.Parse(ChildBikeInProductionE50.Text);
            ChildBikePlannedProductionE50.Text = GlobalVariables.E50Produktionsauftrag.ToString();
            ChildBikeOrderE4.Text = ChildBikePlannedProductionE50.Text;
            ChildBikeOrderE10.Text = ChildBikePlannedProductionE50.Text;
            ChildBikeOrderE49.Text = ChildBikePlannedProductionE50.Text;
            ChildBikePassedWaitlistE4.Text = ChildBikeWaitlistE50.Text;
            ChildBikePassedWaitlistE10.Text = ChildBikeWaitlistE50.Text;
            ChildBikePassedWaitlistE49.Text = ChildBikeWaitlistE50.Text;
            //7.-9. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.E4Produktionsauftrag = int.Parse(ChildBikeOrderE4.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE4.Text) +
                                                      int.Parse(ChildBikeSafetyE4.Text) -
                                                      int.Parse(ChildBikeStockE4.Text) -
                                                      int.Parse(ChildBikeWaitlistE4.Text) -
                                                      int.Parse(ChildBikeInProductionE4.Text);
            ChildBikePlannedProductionE4.Text = GlobalVariables.E4Produktionsauftrag.ToString();
            GlobalVariables.E10Produktionsauftrag = int.Parse(ChildBikeOrderE10.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE10.Text) +
                                                      int.Parse(ChildBikeSafetyE10.Text) -
                                                      int.Parse(ChildBikeStockE10.Text) -
                                                      int.Parse(ChildBikeWaitlistE10.Text) -
                                                      int.Parse(ChildBikeInProductionE10.Text);
            ChildBikePlannedProductionE10.Text = GlobalVariables.E10Produktionsauftrag.ToString();
            GlobalVariables.E49Produktionsauftrag = int.Parse(ChildBikeOrderE49.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE49.Text) +
                                                      int.Parse(ChildBikeSafetyE49.Text) -
                                                      int.Parse(ChildBikeStockE49.Text) -
                                                      int.Parse(ChildBikeWaitlistE49.Text) -
                                                      int.Parse(ChildBikeInProductionE49.Text);
            ChildBikePlannedProductionE49.Text = GlobalVariables.E49Produktionsauftrag.ToString();
            ChildBikeOrderE7.Text = ChildBikePlannedProductionE49.Text;
            ChildBikeOrderE13.Text = ChildBikePlannedProductionE49.Text;
            ChildBikeOrderE18.Text = ChildBikePlannedProductionE49.Text;
            ChildBikePassedWaitlistE7.Text = ChildBikeWaitlistE49.Text;
            ChildBikePassedWaitlistE13.Text = ChildBikeWaitlistE49.Text;
            ChildBikePassedWaitlistE18.Text = ChildBikeWaitlistE49.Text;
            //10.-12. Zeile ausrechnen
            GlobalVariables.E7Produktionsauftrag = int.Parse(ChildBikeOrderE7.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE7.Text) +
                                                      int.Parse(ChildBikeSafetyE7.Text) -
                                                      int.Parse(ChildBikeStockE7.Text) -
                                                      int.Parse(ChildBikeWaitlistE7.Text) -
                                                      int.Parse(ChildBikeInProductionE7.Text);
            ChildBikePlannedProductionE7.Text = GlobalVariables.E7Produktionsauftrag.ToString();
            GlobalVariables.E13Produktionsauftrag = int.Parse(ChildBikeOrderE13.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE13.Text) +
                                                      int.Parse(ChildBikeSafetyE13.Text) -
                                                      int.Parse(ChildBikeStockE13.Text) -
                                                      int.Parse(ChildBikeWaitlistE13.Text) -
                                                      int.Parse(ChildBikeInProductionE13.Text);
            ChildBikePlannedProductionE13.Text = GlobalVariables.E13Produktionsauftrag.ToString();
            GlobalVariables.E18Produktionsauftrag = int.Parse(ChildBikeOrderE18.Text) +
                                                      int.Parse(ChildBikePassedWaitlistE18.Text) +
                                                      int.Parse(ChildBikeSafetyE18.Text) -
                                                      int.Parse(ChildBikeStockE18.Text) -
                                                      int.Parse(ChildBikeWaitlistE18.Text) -
                                                      int.Parse(ChildBikeInProductionE18.Text);
            ChildBikePlannedProductionE18.Text = GlobalVariables.E18Produktionsauftrag.ToString();
            //P2
            //1. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.P2Produktionsauftrag = int.Parse(FemaleBikeOrderP2.Text) + int.Parse(FemaleBikeSafetyP2.Text) -
                                                   int.Parse(FemaleBikeStockP2.Text) -
                                                   int.Parse(FemaleBikeWaitlistP2.Text) -
                                                   int.Parse(FemaleBikeInProductionP2.Text);
            FemaleBikePlannedProductionP2.Text = GlobalVariables.P2Produktionsauftrag.ToString();
            FemaleBikeOrderE26.Text = FemaleBikePlannedProductionP2.Text;
            FemaleBikeOrderE56.Text = FemaleBikePlannedProductionP2.Text;
            FemaleBikePassedWaitlistE26.Text = FemaleBikeWaitlistP2.Text;
            FemaleBikePassedWaitlistE56.Text = FemaleBikeWaitlistP2.Text;
            //2. und 3. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.P2E26Produktionsauftrag = int.Parse(FemaleBikeOrderE26.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE26.Text) +
                                                      int.Parse(FemaleBikeSafetyE26.Text) -
                                                      int.Parse(FemaleBikeStockE26.Text) -
                                                      int.Parse(FemaleBikeWaitlistE26.Text) -
                                                      int.Parse(FemaleBikeInProductionE26.Text);
            FemaleBikePlannedProductionE26.Text = GlobalVariables.P2E26Produktionsauftrag.ToString();
            GlobalVariables.E56Produktionsauftrag = int.Parse(FemaleBikeOrderE56.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE56.Text) +
                                                      int.Parse(FemaleBikeSafetyE56.Text) -
                                                      int.Parse(FemaleBikeStockE56.Text) -
                                                      int.Parse(FemaleBikeWaitlistE56.Text) -
                                                      int.Parse(FemaleBikeInProductionE56.Text);
            FemaleBikePlannedProductionE56.Text = GlobalVariables.E56Produktionsauftrag.ToString();
            FemaleBikeOrderE16.Text = FemaleBikePlannedProductionE56.Text;
            FemaleBikeOrderE17.Text = FemaleBikePlannedProductionE56.Text;
            FemaleBikeOrderE55.Text = FemaleBikePlannedProductionE56.Text;
            FemaleBikePassedWaitlistE16.Text = FemaleBikeWaitlistE56.Text;
            FemaleBikePassedWaitlistE17.Text = FemaleBikeWaitlistE56.Text;
            FemaleBikePassedWaitlistE55.Text = FemaleBikeWaitlistE56.Text;
            //4.-6. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.P2E16Produktionsauftrag = int.Parse(FemaleBikeOrderE16.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE16.Text) +
                                                      int.Parse(FemaleBikeSafetyE16.Text) -
                                                      int.Parse(FemaleBikeStockE16.Text) -
                                                      int.Parse(FemaleBikeWaitlistE16.Text) -
                                                      int.Parse(FemaleBikeInProductionE16.Text);
            FemaleBikePlannedProductionE16.Text = GlobalVariables.P2E16Produktionsauftrag.ToString();
            GlobalVariables.P2E17Produktionsauftrag = int.Parse(FemaleBikeOrderE17.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE17.Text) +
                                                      int.Parse(FemaleBikeSafetyE17.Text) -
                                                      int.Parse(FemaleBikeStockE17.Text) -
                                                      int.Parse(FemaleBikeWaitlistE17.Text) -
                                                      int.Parse(FemaleBikeInProductionE17.Text);
            FemaleBikePlannedProductionE17.Text = GlobalVariables.P2E17Produktionsauftrag.ToString();
            GlobalVariables.E55Produktionsauftrag = int.Parse(FemaleBikeOrderE55.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE55.Text) +
                                                      int.Parse(FemaleBikeSafetyE55.Text) -
                                                      int.Parse(FemaleBikeStockE55.Text) -
                                                      int.Parse(FemaleBikeWaitlistE55.Text) -
                                                      int.Parse(FemaleBikeInProductionE55.Text);
            FemaleBikePlannedProductionE55.Text = GlobalVariables.E55Produktionsauftrag.ToString();
            FemaleBikeOrderE5.Text = FemaleBikePlannedProductionE55.Text;
            FemaleBikeOrderE11.Text = FemaleBikePlannedProductionE55.Text;
            FemaleBikeOrderE54.Text = FemaleBikePlannedProductionE55.Text;
            FemaleBikePassedWaitlistE5.Text = FemaleBikeWaitlistE55.Text;
            FemaleBikePassedWaitlistE11.Text = FemaleBikeWaitlistE55.Text;
            FemaleBikePassedWaitlistE54.Text = FemaleBikeWaitlistE55.Text;
            //7.-9. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.E4Produktionsauftrag = int.Parse(FemaleBikeOrderE5.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE5.Text) +
                                                      int.Parse(FemaleBikeSafetyE5.Text) -
                                                      int.Parse(FemaleBikeStockE5.Text) -
                                                      int.Parse(FemaleBikeWaitlistE5.Text) -
                                                      int.Parse(FemaleBikeInProductionE5.Text);
            FemaleBikePlannedProductionE5.Text = GlobalVariables.E4Produktionsauftrag.ToString();
            GlobalVariables.E11Produktionsauftrag = int.Parse(FemaleBikeOrderE11.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE11.Text) +
                                                      int.Parse(FemaleBikeSafetyE11.Text) -
                                                      int.Parse(FemaleBikeStockE11.Text) -
                                                      int.Parse(FemaleBikeWaitlistE11.Text) -
                                                      int.Parse(FemaleBikeInProductionE11.Text);
            FemaleBikePlannedProductionE11.Text = GlobalVariables.E11Produktionsauftrag.ToString();
            GlobalVariables.E54Produktionsauftrag = int.Parse(FemaleBikeOrderE54.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE54.Text) +
                                                      int.Parse(FemaleBikeSafetyE54.Text) -
                                                      int.Parse(FemaleBikeStockE54.Text) -
                                                      int.Parse(FemaleBikeWaitlistE54.Text) -
                                                      int.Parse(FemaleBikeInProductionE54.Text);
            FemaleBikePlannedProductionE54.Text = GlobalVariables.E54Produktionsauftrag.ToString();
            FemaleBikeOrderE8.Text = FemaleBikePlannedProductionE54.Text;
            FemaleBikeOrderE14.Text = FemaleBikePlannedProductionE54.Text;
            FemaleBikeOrderE19.Text = FemaleBikePlannedProductionE54.Text;
            FemaleBikePassedWaitlistE8.Text = FemaleBikeWaitlistE54.Text;
            FemaleBikePassedWaitlistE14.Text = FemaleBikeWaitlistE54.Text;
            FemaleBikePassedWaitlistE19.Text = FemaleBikeWaitlistE54.Text;
            //10.-12. Zeile ausrechnen
            GlobalVariables.E8Produktionsauftrag = int.Parse(FemaleBikeOrderE8.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE8.Text) +
                                                      int.Parse(FemaleBikeSafetyE8.Text) -
                                                      int.Parse(FemaleBikeStockE8.Text) -
                                                      int.Parse(FemaleBikeWaitlistE8.Text) -
                                                      int.Parse(FemaleBikeInProductionE8.Text);
            FemaleBikePlannedProductionE8.Text = GlobalVariables.E8Produktionsauftrag.ToString();
            GlobalVariables.E14Produktionsauftrag = int.Parse(FemaleBikeOrderE14.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE14.Text) +
                                                      int.Parse(FemaleBikeSafetyE14.Text) -
                                                      int.Parse(FemaleBikeStockE14.Text) -
                                                      int.Parse(FemaleBikeWaitlistE14.Text) -
                                                      int.Parse(FemaleBikeInProductionE14.Text);
            FemaleBikePlannedProductionE14.Text = GlobalVariables.E14Produktionsauftrag.ToString();
            GlobalVariables.E19Produktionsauftrag = int.Parse(FemaleBikeOrderE19.Text) +
                                                      int.Parse(FemaleBikePassedWaitlistE19.Text) +
                                                      int.Parse(FemaleBikeSafetyE19.Text) -
                                                      int.Parse(FemaleBikeStockE19.Text) -
                                                      int.Parse(FemaleBikeWaitlistE19.Text) -
                                                      int.Parse(FemaleBikeInProductionE19.Text);
            FemaleBikePlannedProductionE19.Text = GlobalVariables.E19Produktionsauftrag.ToString();
            //P3
            //1. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.P3Produktionsauftrag = int.Parse(MaleBikeOrderP3.Text) + int.Parse(MaleBikeSafetyP3.Text) -
                                                   int.Parse(MaleBikeStockP3.Text) -
                                                   int.Parse(MaleBikeWaitlistP3.Text) -
                                                   int.Parse(MaleBikeInProductionP3.Text);
            MaleBikePlannedProductionP3.Text = GlobalVariables.P3Produktionsauftrag.ToString();
            MaleBikeOrderE26.Text = MaleBikePlannedProductionP3.Text;
            MaleBikeOrderE31.Text = MaleBikePlannedProductionP3.Text;
            MaleBikePassedWaitlistE26.Text = MaleBikeWaitlistP3.Text;
            MaleBikePassedWaitlistE31.Text = MaleBikeWaitlistP3.Text;
            //2. und 3. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.P3E26Produktionsauftrag = int.Parse(MaleBikeOrderE26.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE26.Text) +
                                                      int.Parse(MaleBikeSafetyE26.Text) -
                                                      int.Parse(MaleBikeStockE26.Text) -
                                                      int.Parse(MaleBikeWaitlistE26.Text) -
                                                      int.Parse(MaleBikeInProductionE26.Text);
            MaleBikePlannedProductionE26.Text = GlobalVariables.P3E26Produktionsauftrag.ToString();
            GlobalVariables.E31Produktionsauftrag = int.Parse(MaleBikeOrderE31.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE31.Text) +
                                                      int.Parse(MaleBikeSafetyE31.Text) -
                                                      int.Parse(MaleBikeStockE31.Text) -
                                                      int.Parse(MaleBikeWaitlistE31.Text) -
                                                      int.Parse(MaleBikeInProductionE31.Text);
            MaleBikePlannedProductionE31.Text = GlobalVariables.E31Produktionsauftrag.ToString();
            MaleBikeOrderE16.Text = MaleBikePlannedProductionE31.Text;
            MaleBikeOrderE17.Text = MaleBikePlannedProductionE31.Text;
            MaleBikeOrderE30.Text = MaleBikePlannedProductionE31.Text;
            MaleBikePassedWaitlistE16.Text = MaleBikeWaitlistE31.Text;
            MaleBikePassedWaitlistE17.Text = MaleBikeWaitlistE31.Text;
            MaleBikePassedWaitlistE30.Text = MaleBikeWaitlistE31.Text;
            //4.-6. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.P3E16Produktionsauftrag = int.Parse(MaleBikeOrderE16.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE16.Text) +
                                                      int.Parse(MaleBikeSafetyE16.Text) -
                                                      int.Parse(MaleBikeStockE16.Text) -
                                                      int.Parse(MaleBikeWaitlistE16.Text) -
                                                      int.Parse(MaleBikeInProductionE16.Text);
            MaleBikePlannedProductionE16.Text = GlobalVariables.P3E16Produktionsauftrag.ToString();
            GlobalVariables.P3E17Produktionsauftrag = int.Parse(MaleBikeOrderE17.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE17.Text) +
                                                      int.Parse(MaleBikeSafetyE17.Text) -
                                                      int.Parse(MaleBikeStockE17.Text) -
                                                      int.Parse(MaleBikeWaitlistE17.Text) -
                                                      int.Parse(MaleBikeInProductionE17.Text);
            MaleBikePlannedProductionE17.Text = GlobalVariables.P3E17Produktionsauftrag.ToString();
            GlobalVariables.E30Produktionsauftrag = int.Parse(MaleBikeOrderE30.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE30.Text) +
                                                      int.Parse(MaleBikeSafetyE30.Text) -
                                                      int.Parse(MaleBikeStockE30.Text) -
                                                      int.Parse(MaleBikeWaitlistE30.Text) -
                                                      int.Parse(MaleBikeInProductionE30.Text);
            MaleBikePlannedProductionE30.Text = GlobalVariables.E30Produktionsauftrag.ToString();
            MaleBikeOrderE6.Text = MaleBikePlannedProductionE30.Text;
            MaleBikeOrderE12.Text = MaleBikePlannedProductionE30.Text;
            MaleBikeOrderE29.Text = MaleBikePlannedProductionE30.Text;
            MaleBikePassedWaitlistE6.Text = MaleBikeWaitlistE30.Text;
            MaleBikePassedWaitlistE12.Text = MaleBikeWaitlistE30.Text;
            MaleBikePassedWaitlistE29.Text = MaleBikeWaitlistE30.Text;
            //7.-9. Zeile ausrechnen und Warteschlange und geplante Produktion durchreichen
            GlobalVariables.E4Produktionsauftrag = int.Parse(MaleBikeOrderE6.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE6.Text) +
                                                      int.Parse(MaleBikeSafetyE6.Text) -
                                                      int.Parse(MaleBikeStockE6.Text) -
                                                      int.Parse(MaleBikeWaitlistE6.Text) -
                                                      int.Parse(MaleBikeInProductionE6.Text);
            MaleBikePlannedProductionE6.Text = GlobalVariables.E4Produktionsauftrag.ToString();
            GlobalVariables.E12Produktionsauftrag = int.Parse(MaleBikeOrderE12.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE12.Text) +
                                                      int.Parse(MaleBikeSafetyE12.Text) -
                                                      int.Parse(MaleBikeStockE12.Text) -
                                                      int.Parse(MaleBikeWaitlistE12.Text) -
                                                      int.Parse(MaleBikeInProductionE12.Text);
            MaleBikePlannedProductionE12.Text = GlobalVariables.E12Produktionsauftrag.ToString();
            GlobalVariables.E29Produktionsauftrag = int.Parse(MaleBikeOrderE29.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE29.Text) +
                                                      int.Parse(MaleBikeSafetyE29.Text) -
                                                      int.Parse(MaleBikeStockE29.Text) -
                                                      int.Parse(MaleBikeWaitlistE29.Text) -
                                                      int.Parse(MaleBikeInProductionE29.Text);
            MaleBikePlannedProductionE29.Text = GlobalVariables.E29Produktionsauftrag.ToString();
            MaleBikeOrderE9.Text = MaleBikePlannedProductionE29.Text;
            MaleBikeOrderE15.Text = MaleBikePlannedProductionE29.Text;
            MaleBikeOrderE20.Text = MaleBikePlannedProductionE29.Text;
            MaleBikePassedWaitlistE9.Text = MaleBikeWaitlistE29.Text;
            MaleBikePassedWaitlistE15.Text = MaleBikeWaitlistE29.Text;
            MaleBikePassedWaitlistE20.Text = MaleBikeWaitlistE29.Text;
            //10.-12. Zeile ausrechnen
            GlobalVariables.E8Produktionsauftrag = int.Parse(MaleBikeOrderE9.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE9.Text) +
                                                      int.Parse(MaleBikeSafetyE9.Text) -
                                                      int.Parse(MaleBikeStockE9.Text) -
                                                      int.Parse(MaleBikeWaitlistE9.Text) -
                                                      int.Parse(MaleBikeInProductionE9.Text);
            MaleBikePlannedProductionE9.Text = GlobalVariables.E8Produktionsauftrag.ToString();
            GlobalVariables.E15Produktionsauftrag = int.Parse(MaleBikeOrderE15.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE15.Text) +
                                                      int.Parse(MaleBikeSafetyE15.Text) -
                                                      int.Parse(MaleBikeStockE15.Text) -
                                                      int.Parse(MaleBikeWaitlistE15.Text) -
                                                      int.Parse(MaleBikeInProductionE15.Text);
            MaleBikePlannedProductionE15.Text = GlobalVariables.E15Produktionsauftrag.ToString();
            GlobalVariables.E20Produktionsauftrag = int.Parse(MaleBikeOrderE20.Text) +
                                                      int.Parse(MaleBikePassedWaitlistE20.Text) +
                                                      int.Parse(MaleBikeSafetyE20.Text) -
                                                      int.Parse(MaleBikeStockE20.Text) -
                                                      int.Parse(MaleBikeWaitlistE20.Text) -
                                                      int.Parse(MaleBikeInProductionE20.Text);
            MaleBikePlannedProductionE20.Text = GlobalVariables.E20Produktionsauftrag.ToString();


            //Produktionsaufträge für von allen Fahrrädern verwendeten Teilen ausrechnen
            GlobalVariables.E16Produktionsauftrag = GlobalVariables.P1E16Produktionsauftrag +
                                                    GlobalVariables.P2E16Produktionsauftrag +
                                                    GlobalVariables.P3E16Produktionsauftrag;
            GlobalVariables.E17Produktionsauftrag = GlobalVariables.P1E17Produktionsauftrag +
                                                    GlobalVariables.P2E17Produktionsauftrag +
                                                    GlobalVariables.P3E17Produktionsauftrag;
            GlobalVariables.E26Produktionsauftrag = GlobalVariables.P1E26Produktionsauftrag +
                                                    GlobalVariables.P2E26Produktionsauftrag +
                                                    GlobalVariables.P3E26Produktionsauftrag;
            #endregion RepeatProgrammplannungKalkulation
        }

        #endregion Data

        private void DeleteOrder_OnClick(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)GridProductionOrders.SelectedItems[0];
            row.Delete();
        }


        private void AddOrder_OnClick(object sender, RoutedEventArgs e)
        {
            if (AddItemBox.Text.Length>0 && AddAmountBox.Text.Length>0)
            {
                string ItemID;
                string amount;
                ItemID = AddItemBox.Text;
                amount = AddAmountBox.Text;
                DataRow AddRow = GlobalVariables.dtProdOrder.NewRow();
                AddRow["Item"] = ItemID;
                AddRow["Amount"] = amount;
                GlobalVariables.dtProdOrder.Rows.Add(AddRow);
            }
            else
            {
                MessageBox.Show("Fehler");
            }
            
        }

        private void Up_OnClick(object sender, RoutedEventArgs e)
        {
            int CurrentRow = GridProductionOrders.SelectedIndex;
            int old = CurrentRow;
            DataRow row = GlobalVariables.dtProdOrder.Rows[old];
            DataRow row2 = GlobalVariables.dtProdOrder.NewRow();
            row2.ItemArray = row.ItemArray;

            if (old - 1 >= 0)
            {
                GlobalVariables.dtProdOrder.Rows.Remove(GlobalVariables.dtProdOrder.Rows[old]);
                GlobalVariables.dtProdOrder.Rows.InsertAt(row2, old - 1);
                this.GridProductionOrders.SelectedIndex = old - 1;
            }
            else
            {
                MessageBox.Show("Really");
            }
        }

        private void Down_OnClick(object sender, RoutedEventArgs e)
        {
            int CurrentRow = GridProductionOrders.SelectedIndex;
            int old = CurrentRow;
            if (old<GlobalVariables.dtProdOrder.Rows.Count-1)
            {
                DataRow row = GlobalVariables.dtProdOrder.Rows[old];
                DataRow row2 = GlobalVariables.dtProdOrder.NewRow();
                row2.ItemArray = row.ItemArray;

                if (old + 1 <= GlobalVariables.dtProdOrder.Rows.Count)
                {
                    GlobalVariables.dtProdOrder.Rows.Remove(GlobalVariables.dtProdOrder.Rows[old]);
                    GlobalVariables.dtProdOrder.Rows.InsertAt(row2, old + 1);
                    this.GridProductionOrders.SelectedIndex = old + 1;
                }
                else
                {
                    MessageBox.Show("Really");
                }
            }
            else
            {
                MessageBox.Show("Really");
            }

        }

        private void ProdCalcSettings_OnClick(object sender, RoutedEventArgs e)
        {
            // Uncheck each item
            foreach (MenuItem item in ProdCalcSettings.Items)
            {
                item.IsChecked = false;
            }

            MenuItem mi = sender as MenuItem;
            mi.IsChecked = true;
        }

        private void HighOrLowSettings_OnClick(object sender, RoutedEventArgs e)
        {
            // Uncheck each item
            foreach (MenuItem item in HighOrLow.Items)
            {
                item.IsChecked = false;
            }

            MenuItem mi = sender as MenuItem;
            mi.IsChecked = true;
        }
    }
}
