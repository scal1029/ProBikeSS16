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
using System.Collections.ObjectModel;
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
                GlobalVariables.factory.initStorage(GlobalVariables.InputDataSetWithoutOldBatchCalc);
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

            DataTable Prognose = new DataTable();
            Prognose.Clear();
            if(!Prognose.Columns.Contains("article"))
            {
                Prognose.Columns.Add("article", typeof(int));
                Prognose.Columns.Add("quantity", typeof(int));
            }

            Prognose.Rows.Add(1, GlobalVariables.SaleChildBikeN);
            Prognose.Rows.Add(2, GlobalVariables.SaleFemaleBikeN);
            Prognose.Rows.Add(3, GlobalVariables.SaleMaleBikeN);

            Sellwish.DataContext = Prognose.DefaultView;

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
            //results = data.Tables[10].Select("item = '16'");
            try
            {
                results = data.Tables["waitinglist"].Select("item = '16'");
            }
            catch (Exception)
            {
                results = null;
            }
            
            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
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
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
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
            }
            ChildBikeWaitlistE17.Text = "0";
            FemaleBikeWaitlistE17.Text = "0";
            MaleBikeWaitlistE17.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '17'");
            }
            catch (Exception)
            {
                
            }
            
            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
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
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
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
            }
            ChildBikeWaitlistE26.Text = "0";
            FemaleBikeWaitlistE26.Text = "0";
            MaleBikeWaitlistE26.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '26'");
            }
            catch (Exception)
            {
                
            }

            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
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
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
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
            }
            //P1
            ChildBikeWaitlistP1.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '1'");
            }
            catch (Exception)
            {
                
            }

            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    ChildBikeWaitlistP1.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    ChildBikeWaitlistP1.Text = results[0].ItemArray[5].ToString();
                }
            }
            ChildBikeWaitlistE51.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '51'");
            }
            catch (Exception)
            {
                
            }

            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    ChildBikeWaitlistE51.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    ChildBikeWaitlistE51.Text = results[0].ItemArray[5].ToString();
                }
            }
            ChildBikeWaitlistE50.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '50'");
            }
            catch (Exception)
            {
                
            }
            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    ChildBikeWaitlistE50.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    ChildBikeWaitlistE50.Text = results[0].ItemArray[5].ToString();
                }
            }
            ChildBikeWaitlistE4.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '4'");
            }
            catch (Exception)
            {
                
            }
            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    ChildBikeWaitlistE4.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    ChildBikeWaitlistE4.Text = results[0].ItemArray[5].ToString();
                }
            }
            ChildBikeWaitlistE10.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '10'");
            }
            catch (Exception)
            {
                
            }

            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    ChildBikeWaitlistE10.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    ChildBikeWaitlistE10.Text = results[0].ItemArray[5].ToString();
                }
            }
            ChildBikeWaitlistE49.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '49'");
            }
            catch (Exception)
            {
                
            }

            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    ChildBikeWaitlistE49.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    ChildBikeWaitlistE49.Text = results[0].ItemArray[5].ToString();
                }
            }
            ChildBikeWaitlistE7.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '7'");
            }
            catch (Exception)
            {
                
            }

            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    ChildBikeWaitlistE7.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    ChildBikeWaitlistE7.Text = results[0].ItemArray[5].ToString();
                }
            }
            ChildBikeWaitlistE13.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '13'");
            }
            catch (Exception)
            {
                
            }

            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    ChildBikeWaitlistE13.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    ChildBikeWaitlistE13.Text = results[0].ItemArray[5].ToString();
                }
            }
            ChildBikeWaitlistE18.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '18'");
            }
            catch (Exception)
            {

            }

            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    ChildBikeWaitlistE18.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0 )
                {
                    ChildBikeWaitlistE18.Text = results[0].ItemArray[5].ToString();
                }
            }
            //P2
            FemaleBikeWaitlistP2.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '2'");
            }
            catch (Exception)
            {

            }

            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    FemaleBikeWaitlistP2.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    FemaleBikeWaitlistP2.Text = results[0].ItemArray[5].ToString();
                }
            }
            FemaleBikeWaitlistE56.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '56'");
            }
            catch (Exception)
            {
                
            }

            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    FemaleBikeWaitlistE56.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    FemaleBikeWaitlistE56.Text = results[0].ItemArray[5].ToString();
                }
            }
            FemaleBikeWaitlistE55.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '55'");
            }
            catch (Exception)
            {
                
            }

            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    FemaleBikeWaitlistE55.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    FemaleBikeWaitlistE55.Text = results[0].ItemArray[5].ToString();
                }
            }
            FemaleBikeWaitlistE5.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '5'");
            }
            catch (Exception)
            {
                
            }
            
            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    FemaleBikeWaitlistE5.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    FemaleBikeWaitlistE5.Text = results[0].ItemArray[5].ToString();
                }
            }
            FemaleBikeWaitlistE11.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '11'");
            }
            catch (Exception)
            {
                
            }
            
            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    FemaleBikeWaitlistE11.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    FemaleBikeWaitlistE11.Text = results[0].ItemArray[5].ToString();
                }
            }
            FemaleBikeWaitlistE54.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '54'");
            }
            catch (Exception)
            {
                
            }
            
            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    FemaleBikeWaitlistE54.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    FemaleBikeWaitlistE54.Text = results[0].ItemArray[5].ToString();
                }
            }
            FemaleBikeWaitlistE8.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '8'");
            }
            catch (Exception)
            {
      
            }
            
            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    FemaleBikeWaitlistE8.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    FemaleBikeWaitlistE8.Text = results[0].ItemArray[5].ToString();
                }
            }
            FemaleBikeWaitlistE14.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '14'");
            }
            catch (Exception)
            {
           
            }
            
            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    FemaleBikeWaitlistE14.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    FemaleBikeWaitlistE14.Text = results[0].ItemArray[5].ToString();
                }
            }
            FemaleBikeWaitlistE19.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '19'");
            }
            catch (Exception)
            {
               
            }
            
            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    FemaleBikeWaitlistE19.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    FemaleBikeWaitlistE19.Text = results[0].ItemArray[5].ToString();
                }
            }
            //P3
            MaleBikeWaitlistP3.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '3'");
            }
            catch (Exception)
            {
                
            }
            
            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    MaleBikeWaitlistP3.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    MaleBikeWaitlistP3.Text = results[0].ItemArray[5].ToString();
                }
            }
            MaleBikeWaitlistE31.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '31'");
            }
            catch (Exception)
            {
               
            }
            
            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    MaleBikeWaitlistE31.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    MaleBikeWaitlistE31.Text = results[0].ItemArray[5].ToString();
                }
            }
            MaleBikeWaitlistE30.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '30'");
            }
            catch (Exception)
            {
                
            }
            
            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    MaleBikeWaitlistE30.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    MaleBikeWaitlistE30.Text = results[0].ItemArray[5].ToString();
                }
            }
            MaleBikeWaitlistE6.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '6'");
            }
            catch (Exception)
            {
                
            }
            
            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    MaleBikeWaitlistE6.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    MaleBikeWaitlistE6.Text = results[0].ItemArray[5].ToString();
                }
            }
            MaleBikeWaitlistE12.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '12'");
            }
            catch (Exception)
            {
                
            }
            
            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    MaleBikeWaitlistE12.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    MaleBikeWaitlistE12.Text = results[0].ItemArray[5].ToString();
                }
            }
            MaleBikeWaitlistE29.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '29'");
            }
            catch (Exception)
            {
                
            }
            
            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    MaleBikeWaitlistE29.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    MaleBikeWaitlistE29.Text = results[0].ItemArray[5].ToString();
                }
            }
            MaleBikeWaitlistE9.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '9'");
            }
            catch (Exception)
            {
                
            }
            
            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    MaleBikeWaitlistE9.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    MaleBikeWaitlistE9.Text = results[0].ItemArray[5].ToString();
                }
            }
            MaleBikeWaitlistE15.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '15'");
            }
            catch (Exception)
            {
                
               
            }
            
            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    MaleBikeWaitlistE15.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    MaleBikeWaitlistE15.Text = results[0].ItemArray[5].ToString();
                }
            }
            MaleBikeWaitlistE20.Text = "0";
            try
            {
                results = data.Tables["waitinglist"].Select("item = '20'");
            }
            catch (Exception)
            {
                ;
            }
            
            try
            {
                if (results != null && results.Length > 0 && results[0].ItemArray[8] == DBNull.Value)
                {
                    MaleBikeWaitlistE20.Text = results[0].ItemArray[5].ToString();
                }
            }
            catch (Exception)
            {
                if (results != null && results.Length > 0)
                {
                    MaleBikeWaitlistE20.Text = results[0].ItemArray[5].ToString();
                }
            }
            #endregion Waitlist
            //InBearbeitung
            #region InProduction
            //P123
            ChildBikeInProductionE16.Text = "0";
            FemaleBikeInProductionE16.Text = "0";
            MaleBikeInProductionE16.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '16'");
            }
            catch (Exception)
            {
                
            }
            
            if (results != null && results.Length > 0)
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
            try
            {
                results = data.Tables["workplace"].Select("item = '17'");
            }
            catch (Exception)
            {

            }
            
            if (results != null && results.Length > 0)
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
            try
            {
                results = data.Tables["workplace"].Select("item = '26'");
            }
            catch (Exception)
            {
                
                
            }
            
            if (results != null && results.Length > 0)
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
            try
            {
                results = data.Tables["workplace"].Select("item = '1'");
            }
            catch (Exception)
            {
                
                
            }
            
            if (results != null && results.Length > 0)
            {
                ChildBikeInProductionP1.Text = ((results.Length) * 10).ToString();
            }
            ChildBikeInProductionE51.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '51'");
            }
            catch (Exception)
            {
                
            }
            
            if (results != null && results.Length > 0)
            {
                ChildBikeInProductionE51.Text = ((results.Length) * 10).ToString();
            }
            ChildBikeInProductionE50.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '50'");
            }
            catch (Exception)
            {
                
            }
            
            if (results != null && results.Length > 0)
            {
                ChildBikeInProductionE50.Text = ((results.Length) * 10).ToString();
            }
            ChildBikeInProductionE4.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '4'");
            }
            catch (Exception)
            {
                
            }
            
            if (results != null && results.Length > 0)
            {
                ChildBikeInProductionE4.Text = ((results.Length) * 10).ToString();
            }
            ChildBikeInProductionE10.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '10'");
            }
            catch (Exception)
            {
                
            }
            
            if (results != null && results.Length > 0)
            {
                ChildBikeInProductionE10.Text = ((results.Length) * 10).ToString();
            }
            ChildBikeInProductionE49.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '49'");
            }
            catch (Exception)
            {
                
            }
            
            if (results != null && results.Length > 0)
            {
                ChildBikeInProductionE49.Text = ((results.Length) * 10).ToString();
            }
            ChildBikeInProductionE7.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '7'");
            }
            catch (Exception)
            {
                
            }
            
            if (results != null && results.Length > 0)
            {
                ChildBikeInProductionE7.Text = ((results.Length) * 10).ToString();
            }
            ChildBikeInProductionE13.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '13'");
            }
            catch (Exception)
            {
                
            }
            
            if (results != null && results.Length > 0)
            {
                ChildBikeInProductionE13.Text = ((results.Length) * 10).ToString();
            }
            ChildBikeInProductionE18.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '18'");
            }
            catch (Exception)
            {
                
            }
            
            if (results != null && results.Length > 0)
            {
                ChildBikeInProductionE18.Text = ((results.Length) * 10).ToString();
            }
            //P2
            FemaleBikeInProductionP2.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '2'");
            }
            catch (Exception)
            {
                
            }

            if (results != null && results.Length > 0)
            {
                FemaleBikeInProductionP2.Text = ((results.Length)*10).ToString();
            }
            FemaleBikeInProductionE56.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '56'");
            }
            catch (Exception)
            {
                
            }

            if (results != null && results.Length > 0)
            {
                FemaleBikeInProductionE56.Text = ((results.Length) * 10).ToString();
            }
            FemaleBikeInProductionE55.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '55'");
            }
            catch (Exception)
            {
                
            }
            
            if (results != null && results.Length > 0)
            {
                FemaleBikeInProductionE55.Text = ((results.Length) * 10).ToString();
            }
            FemaleBikeInProductionE5.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '5'");
            }
            catch (Exception)
            {
                
            }
            
            if (results != null && results.Length > 0)
            {
                FemaleBikeInProductionE5.Text = ((results.Length) * 10).ToString();
            }
            FemaleBikeInProductionE11.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '11'");
            }
            catch (Exception)
            {
                
            }
            
            if (results != null && results.Length > 0)
            {
                FemaleBikeInProductionE11.Text = ((results.Length) * 10).ToString();
            }
            FemaleBikeInProductionE54.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '54'");
            }
            catch (Exception)
            {
                
            }
            
            if (results != null && results.Length > 0)
            {
                FemaleBikeInProductionE54.Text = ((results.Length) * 10).ToString();
            }
            FemaleBikeInProductionE8.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '8'");
            }
            catch (Exception)
            {
                
            }
            
            if (results != null && results.Length > 0)
            {
                FemaleBikeInProductionE8.Text = ((results.Length) * 10).ToString();
            }
            FemaleBikeInProductionE14.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '14'");
            }
            catch (Exception)
            {
                
            }
            
            if (results != null && results.Length > 0)
            {
                FemaleBikeInProductionE14.Text = ((results.Length) * 10).ToString();
            }
            FemaleBikeInProductionE19.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '19'");
            }
            catch (Exception)
            {
                
            }
            
            if (results != null && results.Length > 0)
            {
                FemaleBikeInProductionE19.Text = ((results.Length) * 10).ToString();
            }
            //P3
            MaleBikeInProductionP3.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '3'");
            }
            catch (Exception)
            {
                
            }
            
            if (results != null && results.Length > 0)
            {
                MaleBikeInProductionP3.Text = ((results.Length) * 10).ToString();
            }
            MaleBikeInProductionE31.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '31'");
            }
            catch (Exception)
            {
                
            }
            
            if (results != null && results.Length > 0)
            {
                MaleBikeInProductionE31.Text = ((results.Length) * 10).ToString();
            }
            MaleBikeInProductionE30.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '30'");
            }
            catch (Exception)
            {
                
                
            }
            
            if (results != null && results.Length > 0)
            {
                MaleBikeInProductionE30.Text = ((results.Length) * 10).ToString();
            }
            MaleBikeInProductionE6.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '6'");
            }
            catch (Exception)
            {
                
            }
            
            if (results != null && results.Length > 0)
            {
                MaleBikeInProductionE6.Text = ((results.Length) * 10).ToString();
            }
            MaleBikeInProductionE12.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '12'");
            }
            catch (Exception)
            {
                
                
            }
            
            if (results != null && results.Length > 0)
            {
                MaleBikeInProductionE12.Text = ((results.Length) * 10).ToString();
            }
            MaleBikeInProductionE29.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '29'");
            }
            catch (Exception)
            {
                
                
            }
            
            if (results != null && results.Length > 0)
            {
                MaleBikeInProductionE29.Text = ((results.Length) * 10).ToString();
            }
            MaleBikeInProductionE9.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '9'");
            }
            catch (Exception)
            {
                
                
            }
            
            if (results != null && results.Length > 0)
            {
                MaleBikeInProductionE9.Text = ((results.Length) * 10).ToString();
            }
            MaleBikeInProductionE15.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '15'");
            }
            catch (Exception)
            {
                
                
            }
            
            if (results != null && results.Length > 0)
            {
                MaleBikeInProductionE15.Text = ((results.Length) * 10).ToString();
            }
            MaleBikeInProductionE20.Text = "0";
            try
            {
                results = data.Tables["workplace"].Select("item = '20'");
            }
            catch (Exception)
            {
                
                
            }
            
            if (results != null && results.Length > 0)
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

            //1. Arbeitsplätze initialisieren und zu Liste hinzufügen
            Arbeitsplatzprototyp A1 = new Arbeitsplatzprototyp(1);
            Arbeitsplatzprototyp A2 = new Arbeitsplatzprototyp(2);
            Arbeitsplatzprototyp A3 = new Arbeitsplatzprototyp(3);
            Arbeitsplatzprototyp A4 = new Arbeitsplatzprototyp(4);
            Arbeitsplatzprototyp A6 = new Arbeitsplatzprototyp(6);
            Arbeitsplatzprototyp A7 = new Arbeitsplatzprototyp(7);
            Arbeitsplatzprototyp A8 = new Arbeitsplatzprototyp(8);
            Arbeitsplatzprototyp A9 = new Arbeitsplatzprototyp(9);
            Arbeitsplatzprototyp A10 = new Arbeitsplatzprototyp(10);
            Arbeitsplatzprototyp A11 = new Arbeitsplatzprototyp(11);
            Arbeitsplatzprototyp A12 = new Arbeitsplatzprototyp(12);
            Arbeitsplatzprototyp A13 = new Arbeitsplatzprototyp(13);
            Arbeitsplatzprototyp A14 = new Arbeitsplatzprototyp(14);
            Arbeitsplatzprototyp A15 = new Arbeitsplatzprototyp(15);
            GlobalVariables.AlleArbeitsplätze.Clear();
            GlobalVariables.AlleArbeitsplätze.Add(1,A1);
            GlobalVariables.AlleArbeitsplätze.Add(2,A2);
            GlobalVariables.AlleArbeitsplätze.Add(3,A3);
            GlobalVariables.AlleArbeitsplätze.Add(4,A4);
            GlobalVariables.AlleArbeitsplätze.Add(6,A6);
            GlobalVariables.AlleArbeitsplätze.Add(7,A7);
            GlobalVariables.AlleArbeitsplätze.Add(8,A8);
            GlobalVariables.AlleArbeitsplätze.Add(9,A9);
            GlobalVariables.AlleArbeitsplätze.Add(10,A10);
            GlobalVariables.AlleArbeitsplätze.Add(11,A11);
            GlobalVariables.AlleArbeitsplätze.Add(12,A12);
            GlobalVariables.AlleArbeitsplätze.Add(13,A13);
            GlobalVariables.AlleArbeitsplätze.Add(14,A14);
            GlobalVariables.AlleArbeitsplätze.Add(15,A15);

            //2. Arbeitsstationen initialisieren und zu Kette machen
            //Arbeitsstationen P123
            //E26
            Dictionary<int,int> TeileE26_1 = new Dictionary<int, int>();
            TeileE26_1.Add(44, 2);
            TeileE26_1.Add(48, 2);
            ArbeitsstationPrototyp E261 = new ArbeitsstationPrototyp("Teil26Station1Arbeitsplatz7",A7,30,2,TeileE26_1,"Init",26);
            Dictionary<int, int> TeileE26_2 = new Dictionary<int, int>();
            TeileE26_2.Add(47, 1);
            ArbeitsstationPrototyp E262 = new ArbeitsstationPrototyp("Teil26Station2Arbeitsplatz15", A15, 15, 3, TeileE26_2, "Init",26);
            //E16
            Dictionary<int, int> TeileE16_1 = new Dictionary<int, int>();
            TeileE16_1.Add(28, 1);
            ArbeitsstationPrototyp E161 = new ArbeitsstationPrototyp("Teil16Station1Arbeitsplatz6", A6, 15, 2, TeileE16_1, "Init", 16);
            Dictionary<int, int> TeileE16_2 = new Dictionary<int, int>();
            TeileE16_2.Add(24, 1);
            TeileE16_2.Add(40, 1);
            TeileE16_2.Add(41, 1);
            TeileE16_2.Add(42, 2);
            ArbeitsstationPrototyp E162 = new ArbeitsstationPrototyp("Teil16Station2Arbeitsplatz14", A14, 0, 3, TeileE16_2, "Init", 16);
            //E17
            Dictionary<int, int> TeileE17_1 = new Dictionary<int, int>();
            TeileE17_1.Add(43, 1);
            TeileE17_1.Add(44, 1);
            TeileE17_1.Add(45, 1);
            TeileE17_1.Add(46, 1);
            ArbeitsstationPrototyp E171 = new ArbeitsstationPrototyp("Teil17Station1Arbeitsplatz15", A15, 15, 3, TeileE17_1, "Init", 17);
            //P1
            //E13
            Dictionary<int, int> TeileE13_1 = new Dictionary<int, int>();
            TeileE13_1.Add(39, 1);
            ArbeitsstationPrototyp E131 = new ArbeitsstationPrototyp("Teil13Station1Arbeitsplatz13", A13, 0, 2, TeileE13_1, "Init",13);
            Dictionary<int, int> TeileE13_2 = new Dictionary<int, int>();
            //TeileE13_2.Add(X, X);
            ArbeitsstationPrototyp E132 = new ArbeitsstationPrototyp("Teil13Station2Arbeitsplatz12", A12, 0, 3, TeileE13_2, "Init", 13);
            Dictionary<int, int> TeileE13_3 = new Dictionary<int, int>();
            //TeileE13_3.Add(X, X);
            ArbeitsstationPrototyp E133 = new ArbeitsstationPrototyp("Teil13Station3Arbeitsplatz8", A8, 15, 1, TeileE13_3, "Init", 13);
            Dictionary<int, int> TeileE13_4 = new Dictionary<int, int>();
            //TeileE13_4.Add(X, X);
            ArbeitsstationPrototyp E134 = new ArbeitsstationPrototyp("Teil13Station4Arbeitsplatz7", A7, 20, 2, TeileE13_4, "Init", 13);
            Dictionary<int, int> TeileE13_5 = new Dictionary<int, int>();
            TeileE13_5.Add(32, 1);
            ArbeitsstationPrototyp E135 = new ArbeitsstationPrototyp("Teil13Station5Arbeitsplatz9", A9, 15, 3, TeileE13_5, "Init", 13);
            //E18
            Dictionary<int, int> TeileE18_1 = new Dictionary<int, int>();
            TeileE18_1.Add(28, 3);
            ArbeitsstationPrototyp E181 = new ArbeitsstationPrototyp("Teil18Station1Arbeitsplatz6", A6, 15, 3, TeileE18_1, "Init", 18);
            Dictionary<int, int> TeileE18_2 = new Dictionary<int, int>();
            //TeileE18_2.Add(X, X);
            ArbeitsstationPrototyp E182 = new ArbeitsstationPrototyp("Teil18Station2Arbeitsplatz8", A8, 20, 3, TeileE18_2, "Init", 18);
            Dictionary<int, int> TeileE18_3 = new Dictionary<int, int>();
            TeileE18_3.Add(59, 2);
            ArbeitsstationPrototyp E183 = new ArbeitsstationPrototyp("Teil18Station3Arbeitsplatz7", A7, 20, 2, TeileE18_3, "Init", 18);
            Dictionary<int, int> TeileE18_4 = new Dictionary<int, int>();
            TeileE18_4.Add(32, 1);
            ArbeitsstationPrototyp E184 = new ArbeitsstationPrototyp("Teil18Station4Arbeitsplatz9", A9, 15, 2, TeileE18_4, "Init", 18);
            //E7
            Dictionary<int, int> TeileE7_1 = new Dictionary<int, int>();
            TeileE7_1.Add(52, 1);
            TeileE7_1.Add(53, 36);
            ArbeitsstationPrototyp E71 = new ArbeitsstationPrototyp("Teil7Station1Arbeitsplatz10", A10, 20, 4, TeileE7_1, "Init", 7);
            Dictionary<int, int> TeileE7_2 = new Dictionary<int, int>();
            TeileE7_2.Add(35, 2);
            TeileE7_2.Add(37, 1);
            TeileE7_2.Add(38, 1);
            ArbeitsstationPrototyp E72 = new ArbeitsstationPrototyp("Teil7Station2Arbeitsplatz11", A11, 20, 3, TeileE7_2, "Init", 7);
            //E4
            Dictionary<int, int> TeileE4_1 = new Dictionary<int, int>();
            TeileE4_1.Add(52, 1);
            TeileE4_1.Add(53, 36);
            ArbeitsstationPrototyp E41 = new ArbeitsstationPrototyp("Teil4Station1Arbeitsplatz10", A10, 20, 4, TeileE4_1, "Init", 4);
            Dictionary<int, int> TeileE4_2 = new Dictionary<int, int>();
            TeileE4_2.Add(35, 2);
            TeileE4_2.Add(36, 1);
            ArbeitsstationPrototyp E42 = new ArbeitsstationPrototyp("Teil4Station2Arbeitsplatz11", A11, 10, 3, TeileE4_2, "Init", 4);
            //E10
            Dictionary<int, int> TeileE10_1 = new Dictionary<int, int>();
            TeileE10_1.Add(39, 1);
            ArbeitsstationPrototyp E101 = new ArbeitsstationPrototyp("Teil10Station1Arbeitsplatz13", A13, 0, 2, TeileE10_1, "Init", 10);
            Dictionary<int, int> TeileE10_2 = new Dictionary<int, int>();
            //TeileEX_X.Add(X, X);
            ArbeitsstationPrototyp E102 = new ArbeitsstationPrototyp("Teil10Station2Arbeitsplatz12", A12, 0, 3, TeileE10_2, "Init", 10);
            Dictionary<int, int> TeileE10_3 = new Dictionary<int, int>();
            //TeileEX_X.Add(X, X);
            ArbeitsstationPrototyp E103 = new ArbeitsstationPrototyp("Teil10Station3Arbeitsplatz8", A8, 15, 1, TeileE10_3, "Init", 10);
            Dictionary<int, int> TeileE10_4 = new Dictionary<int, int>();
            //TeileEX_X.Add(X, X);
            ArbeitsstationPrototyp E104 = new ArbeitsstationPrototyp("Teil10Station4Arbeitsplatz7", A7, 20, 2, TeileE10_4, "Init", 10);
            Dictionary<int, int> TeileE10_5 = new Dictionary<int, int>();
            TeileE10_5.Add(32, 1);
            ArbeitsstationPrototyp E105 = new ArbeitsstationPrototyp("Teil10Station5Arbeitsplatz9", A9, 15, 3, TeileE10_5, "Init", 10);
            //E49
            Dictionary<int, int> TeileE49_1 = new Dictionary<int, int>();
            TeileE49_1.Add(13, 1);
            TeileE49_1.Add(18, 1);
            TeileE49_1.Add(7, 1);
            TeileE49_1.Add(24, 2);
            TeileE49_1.Add(25, 2);
            ArbeitsstationPrototyp E491 = new ArbeitsstationPrototyp("Teil49Station1Arbeitsplatz1", A1, 20, 6, TeileE49_1, "Init", 49);
            //E50
            Dictionary<int, int> TeileE50_1 = new Dictionary<int, int>();
            TeileE50_1.Add(49, 1);
            TeileE50_1.Add(4, 1);
            TeileE50_1.Add(10, 1);
            TeileE50_1.Add(24, 2);
            TeileE50_1.Add(25, 2);
            ArbeitsstationPrototyp E501 = new ArbeitsstationPrototyp("Teil50Station1Arbeitsplatz2", A2, 30, 5, TeileE50_1, "Init", 50);
            //E51
            Dictionary<int, int> TeileE51_1 = new Dictionary<int, int>();
            TeileE51_1.Add(17, 1);
            TeileE51_1.Add(16, 1);
            TeileE51_1.Add(50, 1);
            TeileE51_1.Add(24, 1);
            TeileE51_1.Add(27, 1);
            ArbeitsstationPrototyp E511 = new ArbeitsstationPrototyp("Teil51Station1Arbeitsplatz3", A3, 20, 5, TeileE51_1, "Init", 51);
            //P1
            Dictionary<int, int> TeileE1_1 = new Dictionary<int, int>();
            TeileE1_1.Add(51, 1);
            TeileE1_1.Add(26, 1);
            TeileE1_1.Add(21, 1);
            TeileE1_1.Add(24, 1);
            TeileE1_1.Add(27, 1);
            ArbeitsstationPrototyp E11 = new ArbeitsstationPrototyp("Teil1Station1Arbeitsplatz4", A4, 30, 6, TeileE1_1, "Init", 1);
            //P2
            //E14
            Dictionary<int, int> TeileE14_1 = new Dictionary<int, int>();
            TeileE14_1.Add(39, 1);
            ArbeitsstationPrototyp E141 = new ArbeitsstationPrototyp("Teil14Station1Arbeitsplatz14", A14, 0, 2, TeileE14_1, "Init", 14);
            Dictionary<int, int> TeileE14_2 = new Dictionary<int, int>();
            //TeileE14_2.Add(X, X);
            ArbeitsstationPrototyp E142 = new ArbeitsstationPrototyp("Teil14Station2Arbeitsplatz12", A12, 0, 3, TeileE14_2, "Init", 14);
            Dictionary<int, int> TeileE14_3 = new Dictionary<int, int>();
            //TeileE14_3.Add(X, X);
            ArbeitsstationPrototyp E143 = new ArbeitsstationPrototyp("Teil14Station3Arbeitsplatz8", A8, 15, 2, TeileE14_3, "Init", 14);
            Dictionary<int, int> TeileE14_4 = new Dictionary<int, int>();
            //TeileE14_4.Add(X, X);
            ArbeitsstationPrototyp E144 = new ArbeitsstationPrototyp("Teil14Station4Arbeitsplatz7", A7, 20, 2, TeileE14_4, "Init", 14);
            Dictionary<int, int> TeileE14_5 = new Dictionary<int, int>();
            TeileE14_5.Add(32, 1);
            ArbeitsstationPrototyp E145 = new ArbeitsstationPrototyp("Teil14Station5Arbeitsplatz9", A9, 15, 3, TeileE14_5, "Init", 14);
            //E19
            Dictionary<int, int> TeileE19_1 = new Dictionary<int, int>();
            TeileE19_1.Add(28, 4);
            ArbeitsstationPrototyp E191 = new ArbeitsstationPrototyp("Teil19Station1Arbeitsplatz6", A6, 15, 3, TeileE19_1, "Init", 19);
            Dictionary<int, int> TeileE19_2 = new Dictionary<int, int>();
            //TeileE19_2.Add(X, X);
            ArbeitsstationPrototyp E192 = new ArbeitsstationPrototyp("Teil19Station2Arbeitsplatz8", A8, 25, 3, TeileE19_2, "Init", 19);
            Dictionary<int, int> TeileE19_3 = new Dictionary<int, int>();
            TeileE19_3.Add(59, 2);
            ArbeitsstationPrototyp E193 = new ArbeitsstationPrototyp("Teil19Station3Arbeitsplatz7", A7, 20, 2, TeileE19_3, "Init", 19);
            Dictionary<int, int> TeileE19_4 = new Dictionary<int, int>();
            TeileE19_4.Add(32, 1);
            ArbeitsstationPrototyp E194 = new ArbeitsstationPrototyp("Teil19Station4Arbeitsplatz9", A9, 20, 2, TeileE19_4, "Init", 19);
            //E8
            Dictionary<int, int> TeileE8_1 = new Dictionary<int, int>();
            TeileE8_1.Add(57, 1);
            TeileE8_1.Add(58, 36);
            ArbeitsstationPrototyp E81 = new ArbeitsstationPrototyp("Teil8Station1Arbeitsplatz10", A10, 20, 4, TeileE8_1, "Init", 8);
            Dictionary<int, int> TeileE8_2 = new Dictionary<int, int>();
            TeileE8_2.Add(35, 2);
            TeileE8_2.Add(37, 1);
            TeileE8_2.Add(38, 1);
            ArbeitsstationPrototyp E82 = new ArbeitsstationPrototyp("Teil8Station2Arbeitsplatz11", A11, 20, 3, TeileE8_2, "Init", 8);
            //E5
            Dictionary<int, int> TeileE5_1 = new Dictionary<int, int>();
            TeileE5_1.Add(57, 1);
            TeileE5_1.Add(58, 36);
            ArbeitsstationPrototyp E51 = new ArbeitsstationPrototyp("Teil5Station1Arbeitsplatz10", A10, 20, 4, TeileE4_1, "Init", 5);
            Dictionary<int, int> TeileE5_2 = new Dictionary<int, int>();
            TeileE5_2.Add(35, 2);
            TeileE5_2.Add(36, 1);
            ArbeitsstationPrototyp E52 = new ArbeitsstationPrototyp("Teil5Station2Arbeitsplatz11", A11, 10, 3, TeileE5_2, "Init", 5);
            //E11
            Dictionary<int, int> TeileE11_1 = new Dictionary<int, int>();
            TeileE11_1.Add(39, 1);
            ArbeitsstationPrototyp E111 = new ArbeitsstationPrototyp("Teil11Station1Arbeitsplatz13", A13, 0, 2, TeileE11_1, "Init", 11);
            Dictionary<int, int> TeileE11_2 = new Dictionary<int, int>();
            //TeileEX_X.Add(X, X);
            ArbeitsstationPrototyp E112 = new ArbeitsstationPrototyp("Teil11Station2Arbeitsplatz12", A12, 0, 3, TeileE11_2, "Init", 11);
            Dictionary<int, int> TeileE11_3 = new Dictionary<int, int>();
            //TeileEX_X.Add(X, X);
            ArbeitsstationPrototyp E113 = new ArbeitsstationPrototyp("Teil11Station3Arbeitsplatz8", A8, 15, 2, TeileE11_3, "Init", 11);
            Dictionary<int, int> TeileE11_4 = new Dictionary<int, int>();
            //TeileEX_X.Add(X, X);
            ArbeitsstationPrototyp E114 = new ArbeitsstationPrototyp("Teil11Station4Arbeitsplatz7", A7, 20, 2, TeileE11_4, "Init", 11);
            Dictionary<int, int> TeileE11_5 = new Dictionary<int, int>();
            TeileE11_5.Add(32, 1);
            ArbeitsstationPrototyp E115 = new ArbeitsstationPrototyp("Teil11Station5Arbeitsplatz9", A9, 15, 3, TeileE11_5, "Init", 11);
            //E54
            Dictionary<int, int> TeileE54_1 = new Dictionary<int, int>();
            TeileE54_1.Add(14, 1);
            TeileE54_1.Add(19, 1);
            TeileE54_1.Add(8, 1);
            TeileE54_1.Add(24, 2);
            TeileE54_1.Add(25, 2);
            ArbeitsstationPrototyp E541 = new ArbeitsstationPrototyp("Teil54Station1Arbeitsplatz1", A1, 20, 6, TeileE54_1, "Init", 54);
            //E55
            Dictionary<int, int> TeileE55_1 = new Dictionary<int, int>();
            TeileE55_1.Add(54, 1);
            TeileE55_1.Add(5, 1);
            TeileE55_1.Add(11, 1);
            TeileE55_1.Add(24, 2);
            TeileE55_1.Add(25, 2);
            ArbeitsstationPrototyp E551 = new ArbeitsstationPrototyp("Teil55Station1Arbeitsplatz2", A2, 30, 5, TeileE55_1, "Init", 55);
            //E56
            Dictionary<int, int> TeileE56_1 = new Dictionary<int, int>();
            TeileE56_1.Add(17, 1);
            TeileE56_1.Add(16, 1);
            TeileE56_1.Add(55, 1);
            TeileE56_1.Add(24, 1);
            TeileE56_1.Add(27, 1);
            ArbeitsstationPrototyp E561 = new ArbeitsstationPrototyp("Teil56Station1Arbeitsplatz3", A3, 20, 6, TeileE56_1, "Init", 56);
            //P2
            Dictionary<int, int> TeileE2_1 = new Dictionary<int, int>();
            TeileE2_1.Add(56, 1);
            TeileE2_1.Add(26, 1);
            TeileE2_1.Add(22, 1);
            TeileE2_1.Add(24, 1);
            TeileE2_1.Add(27, 1);
            ArbeitsstationPrototyp E21 = new ArbeitsstationPrototyp("Teil2Station1Arbeitsplatz4", A4, 20, 7, TeileE2_1, "Init", 2);


            //P3
            //E15
            Dictionary<int, int> TeileE15_1 = new Dictionary<int, int>();
            TeileE15_1.Add(39, 1);
            ArbeitsstationPrototyp E151 = new ArbeitsstationPrototyp("Teil15Station1Arbeitsplatz15", A15, 0, 2, TeileE15_1, "Init", 15);
            Dictionary<int, int> TeileE15_2 = new Dictionary<int, int>();
            //TeileE15_2.Add(X, X);
            ArbeitsstationPrototyp E152 = new ArbeitsstationPrototyp("Teil15Station2Arbeitsplatz12", A12, 0, 3, TeileE15_2, "Init", 15);
            Dictionary<int, int> TeileE15_3 = new Dictionary<int, int>();
            //TeileE15_3.Add(X, X);
            ArbeitsstationPrototyp E153 = new ArbeitsstationPrototyp("Teil15Station3Arbeitsplatz8", A8, 15, 2, TeileE15_3, "Init", 15);
            Dictionary<int, int> TeileE15_4 = new Dictionary<int, int>();
            //TeileE15_4.Add(X, X);
            ArbeitsstationPrototyp E154 = new ArbeitsstationPrototyp("Teil15Station4Arbeitsplatz7", A7, 20, 2, TeileE15_4, "Init", 15);
            Dictionary<int, int> TeileE15_5 = new Dictionary<int, int>();
            TeileE15_5.Add(32, 1);
            ArbeitsstationPrototyp E155 = new ArbeitsstationPrototyp("Teil15Station5Arbeitsplatz9", A9, 15, 3, TeileE15_5, "Init", 15);
            //E20
            Dictionary<int, int> TeileE20_1 = new Dictionary<int, int>();
            TeileE20_1.Add(28, 5);
            ArbeitsstationPrototyp E201 = new ArbeitsstationPrototyp("Teil20Station1Arbeitsplatz6", A6, 15, 3, TeileE20_1, "Init", 20);
            Dictionary<int, int> TeileE20_2 = new Dictionary<int, int>();
            //TeileE20_2.Add(X, X);
            ArbeitsstationPrototyp E202 = new ArbeitsstationPrototyp("Teil20Station2Arbeitsplatz8", A8, 20, 3, TeileE20_2, "Init", 20);
            Dictionary<int, int> TeileE20_3 = new Dictionary<int, int>();
            TeileE20_3.Add(59, 2);
            ArbeitsstationPrototyp E203 = new ArbeitsstationPrototyp("Teil20Station3Arbeitsplatz7", A7, 20, 2, TeileE20_3, "Init", 20);
            Dictionary<int, int> TeileE20_4 = new Dictionary<int, int>();
            TeileE20_4.Add(32, 1);
            ArbeitsstationPrototyp E204 = new ArbeitsstationPrototyp("Teil20Station4Arbeitsplatz9", A9, 15, 2, TeileE20_4, "Init", 20);
            //E9
            Dictionary<int, int> TeileE9_1 = new Dictionary<int, int>();
            TeileE9_1.Add(33, 1);
            TeileE9_1.Add(34, 36);
            ArbeitsstationPrototyp E91 = new ArbeitsstationPrototyp("Teil9Station1Arbeitsplatz10", A10, 20, 4, TeileE9_1, "Init", 9);
            Dictionary<int, int> TeileE9_2 = new Dictionary<int, int>();
            TeileE9_2.Add(35, 2);
            TeileE9_2.Add(37, 1);
            TeileE9_2.Add(38, 1);
            ArbeitsstationPrototyp E92 = new ArbeitsstationPrototyp("Teil9Station2Arbeitsplatz11", A11, 20, 3, TeileE9_2, "Init", 9);
            //E6
            Dictionary<int, int> TeileE6_1 = new Dictionary<int, int>();
            TeileE6_1.Add(33, 1);
            TeileE6_1.Add(34, 36);
            ArbeitsstationPrototyp E61 = new ArbeitsstationPrototyp("Teil6Station1Arbeitsplatz10", A10, 20, 4, TeileE4_1, "Init", 6);
            Dictionary<int, int> TeileE6_2 = new Dictionary<int, int>();
            TeileE6_2.Add(35, 2);
            TeileE6_2.Add(36, 1);
            ArbeitsstationPrototyp E62 = new ArbeitsstationPrototyp("Teil6Station2Arbeitsplatz11", A11, 20, 3, TeileE6_2, "Init", 6);
            //E12
            Dictionary<int, int> TeileE12_1 = new Dictionary<int, int>();
            TeileE12_1.Add(39, 1);
            ArbeitsstationPrototyp E121 = new ArbeitsstationPrototyp("Teil12Station1Arbeitsplatz13", A13, 0, 2, TeileE12_1, "Init", 12);
            Dictionary<int, int> TeileE12_2 = new Dictionary<int, int>();
            //TeileEX_X.Add(X, X);
            ArbeitsstationPrototyp E122 = new ArbeitsstationPrototyp("Teil12Station2Arbeitsplatz12", A12, 0, 3, TeileE12_2, "Init", 12);
            Dictionary<int, int> TeileE12_3 = new Dictionary<int, int>();
            //TeileEX_X.Add(X, X);
            ArbeitsstationPrototyp E123 = new ArbeitsstationPrototyp("Teil12Station3Arbeitsplatz8", A8, 15, 2, TeileE12_3, "Init", 12);
            Dictionary<int, int> TeileE12_4 = new Dictionary<int, int>();
            //TeileEX_X.Add(X, X);
            ArbeitsstationPrototyp E124 = new ArbeitsstationPrototyp("Teil12Station4Arbeitsplatz7", A7, 20, 2, TeileE12_4, "Init", 12);
            Dictionary<int, int> TeileE12_5 = new Dictionary<int, int>();
            TeileE12_5.Add(32, 1);
            ArbeitsstationPrototyp E125 = new ArbeitsstationPrototyp("Teil12Station5Arbeitsplatz9", A9, 15, 3, TeileE12_5, "Init", 12);
            //E29
            Dictionary<int, int> TeileE29_1 = new Dictionary<int, int>();
            TeileE29_1.Add(15, 1);
            TeileE29_1.Add(20, 1);
            TeileE29_1.Add(9, 1);
            TeileE29_1.Add(24, 2);
            TeileE29_1.Add(25, 2);
            ArbeitsstationPrototyp E291 = new ArbeitsstationPrototyp("Teil29Station1Arbeitsplatz1", A1, 20, 6, TeileE29_1, "Init", 29);
            //E30
            Dictionary<int, int> TeileE30_1 = new Dictionary<int, int>();
            TeileE30_1.Add(29, 1);
            TeileE30_1.Add(6, 1);
            TeileE30_1.Add(12, 1);
            TeileE30_1.Add(24, 2);
            TeileE30_1.Add(25, 2);
            ArbeitsstationPrototyp E301 = new ArbeitsstationPrototyp("Teil30Station1Arbeitsplatz2", A2, 20, 5, TeileE30_1, "Init", 30);
            //E31
            Dictionary<int, int> TeileE31_1 = new Dictionary<int, int>();
            TeileE31_1.Add(17, 1);
            TeileE31_1.Add(16, 1);
            TeileE31_1.Add(30, 1);
            TeileE31_1.Add(24, 1);
            TeileE31_1.Add(27, 1);
            ArbeitsstationPrototyp E311 = new ArbeitsstationPrototyp("Teil31Station1Arbeitsplatz3", A3, 20, 6, TeileE31_1, "Init", 31);
            //P3
            Dictionary<int, int> TeileE3_1 = new Dictionary<int, int>();
            TeileE3_1.Add(31, 1);
            TeileE3_1.Add(26, 1);
            TeileE3_1.Add(23, 1);
            TeileE3_1.Add(24, 1);
            TeileE3_1.Add(27, 1);
            ArbeitsstationPrototyp E31 = new ArbeitsstationPrototyp("Teil3Station1Arbeitsplatz4", A4, 30, 7, TeileE3_1, "Init", 3);

            //P123
            List<ArbeitsstationPrototyp> KetteE26 = new List<ArbeitsstationPrototyp> {E261, E262};
            List<ArbeitsstationPrototyp> KetteE16 = new List<ArbeitsstationPrototyp> { E161, E162 };
            List<ArbeitsstationPrototyp> KetteE17 = new List<ArbeitsstationPrototyp> { E171 };
            //P1
            List<ArbeitsstationPrototyp> KetteE13 = new List<ArbeitsstationPrototyp> { E131, E132, E133, E134, E135 };
            List<ArbeitsstationPrototyp> KetteE18 = new List<ArbeitsstationPrototyp> { E181, E182, E183, E184 };
            List<ArbeitsstationPrototyp> KetteE7 = new List<ArbeitsstationPrototyp> { E71, E72 };
            List<ArbeitsstationPrototyp> KetteE4 = new List<ArbeitsstationPrototyp> { E41, E42 };
            List<ArbeitsstationPrototyp> KetteE10 = new List<ArbeitsstationPrototyp> { E101, E102, E103, E104, E105 };
            List<ArbeitsstationPrototyp> KetteE49 = new List<ArbeitsstationPrototyp> { E491 };
            List<ArbeitsstationPrototyp> KetteE50 = new List<ArbeitsstationPrototyp> { E501 };
            List<ArbeitsstationPrototyp> KetteE51 = new List<ArbeitsstationPrototyp> { E511 };
            List<ArbeitsstationPrototyp> KetteE1 = new List<ArbeitsstationPrototyp> { E11 };
            //P2
            List<ArbeitsstationPrototyp> KetteE14 = new List<ArbeitsstationPrototyp> { E141, E142, E143, E144, E145 };
            List<ArbeitsstationPrototyp> KetteE19 = new List<ArbeitsstationPrototyp> { E191, E192, E193, E194 };
            List<ArbeitsstationPrototyp> KetteE8 = new List<ArbeitsstationPrototyp> { E81, E82 };
            List<ArbeitsstationPrototyp> KetteE5 = new List<ArbeitsstationPrototyp> { E51, E52 };
            List<ArbeitsstationPrototyp> KetteE11 = new List<ArbeitsstationPrototyp> { E111, E112, E113, E114, E115 };
            List<ArbeitsstationPrototyp> KetteE54 = new List<ArbeitsstationPrototyp> { E541 };
            List<ArbeitsstationPrototyp> KetteE55 = new List<ArbeitsstationPrototyp> { E551 };
            List<ArbeitsstationPrototyp> KetteE56 = new List<ArbeitsstationPrototyp> { E561 };
            List<ArbeitsstationPrototyp> KetteE2 = new List<ArbeitsstationPrototyp> { E21 };
            //P3
            List<ArbeitsstationPrototyp> KetteE15 = new List<ArbeitsstationPrototyp> { E151, E152, E153, E154, E155 };
            List<ArbeitsstationPrototyp> KetteE20 = new List<ArbeitsstationPrototyp> { E201, E202, E203, E204 };
            List<ArbeitsstationPrototyp> KetteE9 = new List<ArbeitsstationPrototyp> { E91, E92 };
            List<ArbeitsstationPrototyp> KetteE6 = new List<ArbeitsstationPrototyp> { E61, E62 };
            List<ArbeitsstationPrototyp> KetteE12 = new List<ArbeitsstationPrototyp> { E121, E122, E123, E124, E125 };
            List<ArbeitsstationPrototyp> KetteE29 = new List<ArbeitsstationPrototyp> { E291 };
            List<ArbeitsstationPrototyp> KetteE30 = new List<ArbeitsstationPrototyp> { E301 };
            List<ArbeitsstationPrototyp> KetteE31 = new List<ArbeitsstationPrototyp> { E311 };
            List<ArbeitsstationPrototyp> KetteE3 = new List<ArbeitsstationPrototyp> { E31 };


            //3. Produzierbare Teile Initialisieren und zu Liste hinzufügen
            //P123
            TeilPrototyp Teil26 = new TeilPrototyp(26, KetteE26);
            TeilPrototyp Teil16 = new TeilPrototyp(16, KetteE16);
            TeilPrototyp Teil17 = new TeilPrototyp(17, KetteE17);
            GlobalVariables.AlleTeile.Add(Teil26);
            GlobalVariables.AlleTeile.Add(Teil16);
            GlobalVariables.AlleTeile.Add(Teil17);
            //P1
            TeilPrototyp Teil13 = new TeilPrototyp(13, KetteE13);
            TeilPrototyp Teil18 = new TeilPrototyp(18, KetteE18);
            TeilPrototyp Teil7 = new TeilPrototyp(7, KetteE7);
            TeilPrototyp Teil4 = new TeilPrototyp(4, KetteE4);
            TeilPrototyp Teil10 = new TeilPrototyp(10, KetteE10);
            TeilPrototyp Teil49 = new TeilPrototyp(49, KetteE49);
            TeilPrototyp Teil50 = new TeilPrototyp(50, KetteE50);
            TeilPrototyp Teil51 = new TeilPrototyp(51, KetteE51);
            TeilPrototyp Teil1 = new TeilPrototyp(1, KetteE1);
            GlobalVariables.AlleTeile.Add(Teil13);
            GlobalVariables.AlleTeile.Add(Teil18);
            GlobalVariables.AlleTeile.Add(Teil7);
            GlobalVariables.AlleTeile.Add(Teil4);
            GlobalVariables.AlleTeile.Add(Teil10);
            GlobalVariables.AlleTeile.Add(Teil49);
            GlobalVariables.AlleTeile.Add(Teil50);
            GlobalVariables.AlleTeile.Add(Teil51);
            GlobalVariables.AlleTeile.Add(Teil1);
            //P2
            TeilPrototyp Teil14 = new TeilPrototyp(14, KetteE14);
            TeilPrototyp Teil19 = new TeilPrototyp(19, KetteE19);
            TeilPrototyp Teil8 = new TeilPrototyp(8, KetteE8);
            TeilPrototyp Teil5 = new TeilPrototyp(5, KetteE5);
            TeilPrototyp Teil11 = new TeilPrototyp(11, KetteE11);
            TeilPrototyp Teil54 = new TeilPrototyp(54, KetteE54);
            TeilPrototyp Teil55 = new TeilPrototyp(55, KetteE55);
            TeilPrototyp Teil56 = new TeilPrototyp(56, KetteE56);
            TeilPrototyp Teil2 = new TeilPrototyp(2, KetteE2);
            GlobalVariables.AlleTeile.Add(Teil14);
            GlobalVariables.AlleTeile.Add(Teil19);
            GlobalVariables.AlleTeile.Add(Teil8);
            GlobalVariables.AlleTeile.Add(Teil5);
            GlobalVariables.AlleTeile.Add(Teil11);
            GlobalVariables.AlleTeile.Add(Teil54);
            GlobalVariables.AlleTeile.Add(Teil55);
            GlobalVariables.AlleTeile.Add(Teil56);
            GlobalVariables.AlleTeile.Add(Teil2);
            //P3
            TeilPrototyp Teil15 = new TeilPrototyp(15, KetteE15);
            TeilPrototyp Teil20 = new TeilPrototyp(20, KetteE20);
            TeilPrototyp Teil9 = new TeilPrototyp(9, KetteE9);
            TeilPrototyp Teil6 = new TeilPrototyp(6, KetteE6);
            TeilPrototyp Teil12 = new TeilPrototyp(12, KetteE12);
            TeilPrototyp Teil29 = new TeilPrototyp(29, KetteE29);
            TeilPrototyp Teil30 = new TeilPrototyp(30, KetteE30);
            TeilPrototyp Teil31 = new TeilPrototyp(31, KetteE31);
            TeilPrototyp Teil3 = new TeilPrototyp(3, KetteE3);
            GlobalVariables.AlleTeile.Add(Teil15);
            GlobalVariables.AlleTeile.Add(Teil20);
            GlobalVariables.AlleTeile.Add(Teil9);
            GlobalVariables.AlleTeile.Add(Teil6);
            GlobalVariables.AlleTeile.Add(Teil12);
            GlobalVariables.AlleTeile.Add(Teil29);
            GlobalVariables.AlleTeile.Add(Teil30);
            GlobalVariables.AlleTeile.Add(Teil31);
            GlobalVariables.AlleTeile.Add(Teil3);


            

            //Produktionsauftragsliste erstellen(ohne in Bearbeitung und Warteschlange)
            GridProductionOrders.DataContext = GlobalVariables.dtProdOrder.DefaultView;

            DataTable ProdAufträge = new DataTable();
            ProdAufträge.Clear();
            if (!ProdAufträge.Columns.Contains("article"))
            {
                ProdAufträge.Columns.Add("article", typeof(int));
                ProdAufträge.Columns.Add("quantity", typeof(int));
            }

            
            foreach (DataRowView Row in GlobalVariables.dtProdOrder.DefaultView)
            {
                ProdAufträge.Rows.Add(int.Parse(Row[0].ToString()), int.Parse(Row[1].ToString()));
            }
            Productionlist.DataContext = ProdAufträge.DefaultView;

            #endregion DataTable
            GlobalVariables.ProduktionsAufträgeAktuellePeriode.Clear();
            foreach (DataRow Produktionsauftrag in GlobalVariables.dtProdOrder.Rows)
            {
                int Index;
                if(Produktionsauftrag[0] != null && Produktionsauftrag[1] != null)
                { 
                    foreach (TeilPrototyp teil in GlobalVariables.AlleTeile)
                    {
                        if(teil.TeilID == int.Parse(Produktionsauftrag[0].ToString()))
                            GlobalVariables.ProduktionsAufträgeAktuellePeriode.Add(new OrderPrototyp(int.Parse(Produktionsauftrag[0].ToString()),int.Parse(Produktionsauftrag[1].ToString()), teil));//List x,y aus allen Teilen richtiges auswählenTeilPrototyp);
                    }
                }
            }

            //foreach (OrderPrototyp O in GlobalVariables.ProduktionsAufträgeAktuellePeriode)
            //{
            //    Console.WriteLine("Artikel: " + O.Artikel+ " Menge: " + O.Menge + " ID: " + O.TeilPrototyp.TeilID.ToString());
            //}



            //Warteschlange und Bearbeitung hinzufügen
            //Warteschlange
            if (GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables.Contains("waitinglist") && GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables["workplace"].Columns.Contains("period")
                && GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables["workplace"].Columns.Contains("order"))
            {
                DataTable WaitSnake = GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables["waitinglist"].Clone();
                WaitSnake.Columns[1].DataType = typeof(int);
                WaitSnake.Columns[0].DataType = typeof(int);
                foreach (DataRow datarow in GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables["waitinglist"].Rows)
                {
                    if (datarow[6] != DBNull.Value)
                    {
                        WaitSnake.ImportRow(datarow);
                    }
                }

                DataView wsReal = new DataView(WaitSnake);
                wsReal.Sort = "period DESC, order DESC";

                foreach (DataRowView dr in wsReal)
                {
                    int i = ((int) dr.Row["order"]);
                    int i2 = ((int) dr.Row["period"]);
                    //Console.WriteLine("Periode: " + i2.ToString() + " Order: " + i.ToString() + " Item: " +
                    //                  (string) dr.Row["item"]);
                }

                foreach (DataRowView dr in wsReal)
                {
                    foreach (TeilPrototyp teil in GlobalVariables.AlleTeile)
                    {
                        if (teil.TeilID == int.Parse((string) dr["item"]))
                        {
                            //alt GlobalVariables.ProduktionsAufträgeAktuellePeriode.Insert(0, (new OrderPrototyp(int.Parse((string) dr["item"]), int.Parse((string) dr["amount"]), teil)));
                            GlobalVariables.ProduktionsAufträgeAktuellePeriode.Insert(0,
                                new OrderPrototyp(int.Parse((string) dr["item"]), 0, teil));
                            foreach (TeilPrototyp TP in GlobalVariables.AlleTeile)
                            {
                                if (TP.TeilID == int.Parse((string) dr["item"]))
                                {
                                    foreach (ArbeitsstationPrototyp AP in TP.KetteStationen)
                                    {
                                        if (AP.Arbeitsplatz.ID == (int) dr["workplace_id"])
                                        {
                                            AP.Warteschlange = AP.Warteschlange + int.Parse((string) dr["amount"]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables.Contains("workplace") && GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables["workplace"].Columns.Contains("period") 
                && GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables["workplace"].Columns.Contains("order"))
            {
                //in Arbeit
                //DataTable WaitSnake = new DataTable();
                DataTable inArbeit = GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables["workplace"].Clone();
                inArbeit.Columns[1].DataType = typeof(int);
                foreach (DataRow datarow in GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables["workplace"].Rows)
                {
                    try
                    {
                        if (datarow[10] != DBNull.Value)
                        {
                            inArbeit.ImportRow(datarow);
                        }
                    }
                    catch (Exception)
                    {
                            inArbeit.ImportRow(datarow); 
                    }
                    
                }

                DataView ws = new DataView(inArbeit);
                ws.Sort = "period ASC, order ASC";

                foreach (DataRowView dr in ws)
                {
                    foreach (TeilPrototyp teil in GlobalVariables.AlleTeile)
                    {
                        if (teil.TeilID == int.Parse((string) dr["item"]))
                        {
                            foreach (TeilPrototyp TP in GlobalVariables.AlleTeile)
                            {
                                if (TP.TeilID == int.Parse((string) dr["item"]))
                                {
                                    foreach (ArbeitsstationPrototyp AP in TP.KetteStationen)
                                    {
                                        if (AP.Arbeitsplatz.ID == (int) dr["id"])
                                        {
                                            AP.Warteschlange = AP.Warteschlange + int.Parse((string) dr["amount"]);
                                            AP.Arbeitsplatz.Blockierzeit = int.Parse((string) dr["timeneed"]);
                                            AP.Produziert = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Test5,5");

            //foreach (DataRowView dr in ws)
            //{
            //    Console.WriteLine("In Bearbeitung: Periode: " + (string)dr["period"] + " Order: " + (string)dr.Row["order"]);
            //}

           


            Console.WriteLine("Test6");

            foreach (OrderPrototyp O in GlobalVariables.ProduktionsAufträgeAktuellePeriode)
            {
                Console.WriteLine("Artikel: " + O.Artikel + " Menge: " + O.Menge + " ID: " + O.TeilPrototyp.TeilID.ToString());
            }
            

            Console.WriteLine("Test7");
            //Iterieren Warteschlange weitergeben, Teile am Schluss etc. als Eigene Klasse


            ////Umwandlung Tabelle Lager zu Dictionary
            GlobalVariables.Lagerstand.Clear();
            foreach (DataRow Row in GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables[2].Rows)
            {
                GlobalVariables.Lagerstand.Add(int.Parse((string)Row[0]),int.Parse((string)Row[1]));
            }

            Dictionary<int, int> LagerZuBeginn = ObjectCopier.Clone(GlobalVariables.Lagerstand);



            //SimulationPrototyp2 test2 = new SimulationPrototyp2();

            //test2.SimulationPrototypDurchführung(GlobalVariables.ProduktionsAufträgeAktuellePeriode, GlobalVariables.Lagerstand, 1);

            Kapa.DataContext = GlobalVariables.KPErg.DefaultView;

            //foreach (var O in GlobalVariables.ProduktionsAufträgeAktuellePeriode)
            //{
            //    foreach (var VARIABLE in O.TeilPrototyp.KetteStationen)
            //    {
            //        Console.WriteLine("Arbeitsplatz: " + VARIABLE.Arbeitsplatz.ID.ToString() + " Station: " + VARIABLE.ID.ToString() + " Grund: " + VARIABLE.BegründungStop);
            //    }

            //}

            foreach (KeyValuePair<int, int> VARIABLE in GlobalVariables.Lagerstand)
            {
                Console.WriteLine("Lager Vorher Artikel: " + VARIABLE.Key + " Menge: " + VARIABLE.Value);
            }

            //TODO!!! = Kappa befüllen
            DataTable Kapaza = new DataTable();
            Kapaza.Clear();
            

            
            if (!Kapaza.Columns.Contains("station"))
            {
                Kapaza.Columns.Add("station", typeof(int));
                Kapaza.Columns.Add("shift", typeof(int));
                Kapaza.Columns.Add("overtime", typeof(int));
            }



            Workingtimelist.DataContext = Kapaza;
            Workingtimelist.ItemsSource = Kapaza.DefaultView;

            for (int i=1; i < 16; i++)
            {
                if(i != 5)
                    Kapaza.Rows.Add(i, 1, 0);

            }

            




            foreach (KeyValuePair<int, int> VARIABLE in GlobalVariables.Lagerstand)
            {
                Console.WriteLine("Lager Danach Artikel: " + VARIABLE.Key + " Menge: " + VARIABLE.Value);
            }


            Console.WriteLine("Test9");

            //foreach ( KeyValuePair<int, Arbeitsplatzprototyp> A in GlobalVariables.OriginalAlleArbeitsplätze)
            //{
            //    Console.WriteLine("Arbeitsplatz " + A.Value.ID.ToString() + " Arbeitszeit: " + A.Value.ArbeitszeitProTagInMinuten.ToString());
            //}


            //Direktverkäufe vorbereiten
            DataTable DirektverkäufeAnfang = new DataTable();
            if (!DirektverkäufeAnfang.Columns.Contains("article"))
            {
                DirektverkäufeAnfang.Columns.Add("quantity", typeof(int));
                DirektverkäufeAnfang.Columns.Add("article", typeof(int));
                DirektverkäufeAnfang.Columns.Add("penalty", typeof(double));
                DirektverkäufeAnfang.Columns.Add("price", typeof(double));
            }
            DirektverkäufeAnfang.Clear();

            DirektverkäufeAnfang.Rows.Add(0, 1, 0, 0);
            DirektverkäufeAnfang.Rows.Add(0, 2, 0, 0);
            DirektverkäufeAnfang.Rows.Add(0, 3, 0, 0);

            Selldirect.DataContext = DirektverkäufeAnfang;
            Selldirect.ItemsSource = DirektverkäufeAnfang.DefaultView;


            //Bestellungsplannung
            DataTable Bestellungsplannung = new DataTable();
            if (!Bestellungsplannung.Columns.Contains("Teil"))
            {
                Bestellungsplannung.Columns.Add("Teil");
                Bestellungsplannung.Columns.Add("Lagerbestand");
                Bestellungsplannung.Columns.Add("Brutto Periode n");
                Bestellungsplannung.Columns.Add("Brutto Periode n+1");
                Bestellungsplannung.Columns.Add("Brutto Periode n+2");
                Bestellungsplannung.Columns.Add("Brutto Periode n+3");
                Bestellungsplannung.Columns.Add("Bestellung Periode n");
                Bestellungsplannung.Columns.Add("Bestellung Periode n+1");
                Bestellungsplannung.Columns.Add("Bestellung Periode n+2");
                Bestellungsplannung.Columns.Add("Bestellung Periode n+3");
            }
            Bestellungsplannung.Clear();


            DataRow result2 = GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables["results"].Rows[0];
            int AktuellePeriode = int.Parse(result2["period"].ToString());

            DataTable AlteBestellungen = new DataTable();
            if (!AlteBestellungen.Columns.Contains("item"))
            {
                AlteBestellungen.Columns.Add("Vergangenheit", typeof(int));
                AlteBestellungen.Columns.Add("item", typeof(int));
                AlteBestellungen.Columns.Add("Menge", typeof(int));
                AlteBestellungen.Columns.Add("Modus", typeof(int));
            }
            AlteBestellungen.Clear();
            string B1;
            string B2;
            string B3;
            string B4;
            int B11;
            int B22;
            int B33;
            int B44;
            foreach (DataRow DR in GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables["order"].Rows)
            {

                if (GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables["order"].Columns.Contains("futureinwardstockmovement_Id"))
                {
                    if (DR["futureinwardstockmovement_Id"]!=DBNull.Value)
                    {
                        B1 = DR["orderperiod"].ToString();
                        B11 = int.Parse(B1)-AktuellePeriode;
                        B2 = DR["article"].ToString();
                        B22 = int.Parse(B2);
                        B3 = DR["amount"].ToString();
                        B33 = int.Parse(B3);
                        B4 = DR["mode"].ToString();
                        B44 = int.Parse(B4);
                        AlteBestellungen.Rows.Add(B11, B22, B33, B44);
                    }
                        

                }
            }
            foreach (DataRow DR in AlteBestellungen.Rows)
            {
                Console.WriteLine("Vergangenheit: " + DR[0].ToString() + " Artikel: " + DR[1].ToString() + " Menge: " + DR[2].ToString());
            }

            DataRow[] results3 = new DataRow[4];


            DataTable Bestellungsliste = new DataTable();
            if (!Bestellungsliste.Columns.Contains("article"))
            {
                Bestellungsliste.Columns.Add("article", typeof(int));
                Bestellungsliste.Columns.Add("quantity", typeof(int));
                Bestellungsliste.Columns.Add("modus", typeof(int));
            }
            Bestellungsliste.Clear();

            //Teil21
            double T21LZ = 1.8;
            double T21AB = 0.4;
            double T21E = 1.8/2;
            int BruttoT21P1 = GlobalVariables.SaleChildBikeN.GetValueOrDefault() * 1 + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text));
            int BruttoT21P2 = GlobalVariables.SaleChildBikeN1.GetValueOrDefault() * 1;
            int BruttoT21P3 = GlobalVariables.SaleChildBikeN2.GetValueOrDefault() * 1;
            int BruttoT21P4 = GlobalVariables.SaleChildBikeN3.GetValueOrDefault() * 1;
            int P1Zuwachs = 0;
            int P2Zuwachs = 0;
            int P3Zuwachs = 0;
            int P4Zuwachs = 0;


                results = AlteBestellungen.Select("item = '21'");
                foreach (var row in results)
                {
                    if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T21LZ + T21AB);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                    else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T21E);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                }


            int Periode1 = LagerZuBeginn[21] - BruttoT21P1;
            int Periode2 = LagerZuBeginn[21] - BruttoT21P1 - BruttoT21P2 + P2Zuwachs;
            int Periode3 = LagerZuBeginn[21] - BruttoT21P1 - BruttoT21P2 - BruttoT21P3 + P3Zuwachs;
            int Periode4 = LagerZuBeginn[21] - BruttoT21P1 - BruttoT21P2 - BruttoT21P3 - BruttoT21P2 + P4Zuwachs;

            int Modus = 5;
            int Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1*(-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                if (T21LZ + T21AB > 2)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge =  Periode3 *(-1);
            }
            if (Periode4 < 0 && Bestellmenge < Periode4 * (-1))
            {
                Bestellmenge = Periode4 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(21, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(21, LagerZuBeginn[21], BruttoT21P1, BruttoT21P2, BruttoT21P3, BruttoT21P4, Periode1, Periode2, Periode3, Periode4);



            //Teil22
            double T22LZ = 1.7;
            double T22AB = 0.4;
            double T22E = 1.7 / 2;
            int BruttoT22P1 = GlobalVariables.SaleFemaleBikeN.GetValueOrDefault() * 1 + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text));
            int BruttoT22P2 = GlobalVariables.SaleFemaleBikeN1.GetValueOrDefault() * 1;
            int BruttoT22P3 = GlobalVariables.SaleFemaleBikeN2.GetValueOrDefault() * 1;
            int BruttoT22P4 = GlobalVariables.SaleFemaleBikeN3.GetValueOrDefault() * 1;
            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            foreach (DataRow DR in AlteBestellungen.Rows)
            {
                results = AlteBestellungen.Select("item = '22'");
                foreach (var row in results)
                {
                    if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T22LZ + T22AB);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                    else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T22E);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                }
            }

            Periode1 = LagerZuBeginn[22] - BruttoT22P1;
            Periode2 = LagerZuBeginn[22] - BruttoT22P1 - BruttoT22P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[22] - BruttoT22P1 - BruttoT22P2 - BruttoT22P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[22] - BruttoT22P1 - BruttoT22P2 - BruttoT22P3 - BruttoT22P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                if (T22LZ + T22AB > 2)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = Periode3 * (-1);
            }
            if (Periode4 < 0 && Bestellmenge < Periode4 * (-1))
            {
                Bestellmenge = Periode4 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(22, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(22, LagerZuBeginn[22], BruttoT22P1, BruttoT22P2, BruttoT22P3, BruttoT22P4, Periode1, Periode2, Periode3, Periode4);


            //Teil23
            double T23LZ = 1.2;
            double T23AB = 0.2;
            double T23E = 1.2 / 2;
            int BruttoT23P1 = GlobalVariables.SaleMaleBikeN.GetValueOrDefault() * 1 + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text));
            int BruttoT23P2 = GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 1;
            int BruttoT23P3 = GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 1;
            int BruttoT23P4 = GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 1;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            results = AlteBestellungen.Select("item = '23'");
            foreach (var row in results)
            {
                if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T23LZ + T23AB);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
                else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T23E);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
            }


            Periode1 = LagerZuBeginn[23] - BruttoT23P1;
            Periode2 = LagerZuBeginn[23] - BruttoT23P1 - BruttoT23P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[23] - BruttoT23P1 - BruttoT23P2 - BruttoT23P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[23] - BruttoT23P1 - BruttoT23P2 - BruttoT23P3 - BruttoT23P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                if (T23LZ + T23AB > 2)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = Periode3 * (-1);
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(23, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(23, LagerZuBeginn[23], BruttoT23P1, BruttoT23P2, BruttoT23P3, BruttoT23P4, Periode1, Periode2, Periode3, Periode4);


            //Teil24
            double T24LZ = 3.2;
            double T24AB = 0.3;
            double T24E = 3.2 / 2;
            int BruttoT24P1 = ((GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 7) 
                + ((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 7) 
                + ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 7);
            int BruttoT24P2 = GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 7 + GlobalVariables.SaleChildBikeN1.Value * 7 + GlobalVariables.SaleFemaleBikeN1.Value * 7;
            int BruttoT24P3 = GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 7 + GlobalVariables.SaleChildBikeN2.Value * 7 + GlobalVariables.SaleFemaleBikeN2.Value * 7;
            int BruttoT24P4 = GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 7 + GlobalVariables.SaleChildBikeN3.Value * 7 + GlobalVariables.SaleFemaleBikeN3.Value * 7;
            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;
            int P5Zuwachs = 0;


                results = AlteBestellungen.Select("item = '24'");
                foreach (var row in results)
                {
                    if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T24LZ + T24AB);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                    else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T24E);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                }

            Periode1 = LagerZuBeginn[24] - BruttoT24P1;
            Periode2 = LagerZuBeginn[24] - BruttoT24P1 - BruttoT24P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[24] - BruttoT24P1 - BruttoT24P2 - BruttoT24P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[24] - BruttoT24P1 - BruttoT24P2 - BruttoT24P3 - BruttoT24P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge =  + Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                Bestellmenge =  + Periode3 * (-1);
                Modus = 4;
            }
            if (Periode4 < 0 && Bestellmenge < Periode4 * (-1))
            {
                if (T24LZ + T24AB > 3)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge =  + Periode4 * (-1);
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(24, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(24, LagerZuBeginn[24], BruttoT24P1, BruttoT24P2, BruttoT24P3, BruttoT24P4, Periode1, Periode2, Periode3, Periode4);



            //Teil25
            double T25LZ = 0.9;
            double T25AB = 0.2;
            double T25E = 0.9 / 2;
            int BruttoT25P1 = ((GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 4)
                + ((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 4)
                + ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 4);
            int BruttoT25P2 = GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 4 + GlobalVariables.SaleChildBikeN1.Value * 4 + GlobalVariables.SaleFemaleBikeN1.Value * 4;
            int BruttoT25P3 = GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 4 + GlobalVariables.SaleChildBikeN2.Value * 4 + GlobalVariables.SaleFemaleBikeN2.Value * 4;
            int BruttoT25P4 = GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 4 + GlobalVariables.SaleChildBikeN3.Value * 4 + GlobalVariables.SaleFemaleBikeN3.Value * 4;


            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;


                results = AlteBestellungen.Select("item = '25'");
                foreach (var row in results)
                {
                    if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T25LZ + T25AB);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                    else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T25E);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                }
            

            Periode1 = LagerZuBeginn[25] - BruttoT25P1;
            Periode2 = LagerZuBeginn[25] - BruttoT25P1 - BruttoT25P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[25] - BruttoT25P1 - BruttoT25P2 - BruttoT25P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[25] - BruttoT25P1 - BruttoT25P2 - BruttoT25P3 - BruttoT25P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge =  + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                if (T25LZ + T25AB > 1)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge =  + Periode2 * (-1);
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                Bestellmenge =  + Periode3 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(25, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(25, LagerZuBeginn[25], BruttoT25P1, BruttoT25P2, BruttoT25P3, BruttoT25P4, Periode1, Periode2, Periode3, Periode4);


            //Teil27
            double T27LZ = 0.9;
            double T27AB = 0.2;
            double T27E = 0.9 / 2;
            int BruttoT27P1 = ((GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 2)
                + ((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 2)
                + ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 2); ;
            int BruttoT27P2 = GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 2 + GlobalVariables.SaleChildBikeN1.Value * 2 + GlobalVariables.SaleFemaleBikeN1.Value * 2;
            int BruttoT27P3 = GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 2 + GlobalVariables.SaleChildBikeN2.Value * 2 + GlobalVariables.SaleFemaleBikeN2.Value * 2;
            int BruttoT27P4 = GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 2 + GlobalVariables.SaleChildBikeN3.Value * 2 + GlobalVariables.SaleFemaleBikeN3.Value * 2;

            Bestellungsplannung.Rows.Add(27, LagerZuBeginn[27], BruttoT27P1, BruttoT27P2, BruttoT27P3, BruttoT27P4,
                LagerZuBeginn[27] - BruttoT27P1, LagerZuBeginn[27] - BruttoT27P1 - BruttoT27P2, LagerZuBeginn[27] - BruttoT27P1 - BruttoT27P2 - BruttoT27P3,
                LagerZuBeginn[27] - BruttoT27P1 - BruttoT27P2 - BruttoT27P3 - BruttoT27P2);

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;


            results = AlteBestellungen.Select("item = '27'");
            foreach (var row in results)
            {
                if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T27LZ + T27AB);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
                else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T27E);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
            }


            Periode1 = LagerZuBeginn[27] - BruttoT27P1;
            Periode2 = LagerZuBeginn[27] - BruttoT27P1 - BruttoT27P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[27] - BruttoT27P1 - BruttoT27P2 - BruttoT27P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[27] - BruttoT27P1 - BruttoT27P2 - BruttoT27P3 - BruttoT27P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = +Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                if (T27LZ + T27AB > 1)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = +Periode2 * (-1);
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                Bestellmenge = +Periode3 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(27, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(27, LagerZuBeginn[27], BruttoT27P1, BruttoT27P2, BruttoT27P3, BruttoT27P4, Periode1, Periode2, Periode3, Periode4);


            //Teil28
            double T28LZ = 1.7;
            double T28AB = 0.4;
            double T28E = 1.7 / 2;
            int BruttoT28P1 = ((GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 6)
                + ((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 4)
                + ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 5);
            int BruttoT28P2 = GlobalVariables.SaleChildBikeN1.Value * 4 + GlobalVariables.SaleFemaleBikeN1.Value * 5 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 6;
            int BruttoT28P3 = GlobalVariables.SaleChildBikeN2.Value * 4 + GlobalVariables.SaleFemaleBikeN2.Value * 5 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 6;
            int BruttoT28P4 = GlobalVariables.SaleChildBikeN3.Value * 4 + GlobalVariables.SaleFemaleBikeN3.Value * 5 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 6;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            foreach (DataRow DR in AlteBestellungen.Rows)
            {
                results = AlteBestellungen.Select("item = '28'");
                foreach (var row in results)
                {
                    if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T28LZ + T28AB);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                    else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T28E);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                }
            }

            Periode1 = LagerZuBeginn[28] - BruttoT28P1;
            Periode2 = LagerZuBeginn[28] - BruttoT28P1 - BruttoT28P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[28] - BruttoT28P1 - BruttoT28P2 - BruttoT28P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[28] - BruttoT28P1 - BruttoT28P2 - BruttoT28P3 - BruttoT28P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                if (T28LZ + T28AB > 2)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = Periode3 * (-1);
            }
            if (Periode4 < 0 && Bestellmenge < Periode4 * (-1))
            {
                Bestellmenge = Periode4 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(28, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(28, LagerZuBeginn[28], BruttoT28P1, BruttoT28P2, BruttoT28P3, BruttoT28P4, Periode1, Periode2, Periode3, Periode4);

            //Teil32
            double T32LZ = 2.1;
            double T32AB = 0.5;
            double T32E = 2.1 / 2;
            int BruttoT32P1 = ((GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 3)
                + ((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 3)
                + ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 3);
            int BruttoT32P2 = GlobalVariables.SaleChildBikeN1.Value * 3 + GlobalVariables.SaleFemaleBikeN1.Value * 3 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 3;
            int BruttoT32P3 = GlobalVariables.SaleChildBikeN2.Value * 3 + GlobalVariables.SaleFemaleBikeN2.Value * 3 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 3;
            int BruttoT32P4 = GlobalVariables.SaleChildBikeN3.Value * 3 + GlobalVariables.SaleFemaleBikeN3.Value * 3 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 3;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            foreach (DataRow DR in AlteBestellungen.Rows)
            {
                results = AlteBestellungen.Select("item = '32'");
                foreach (var row in results)
                {
                    if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T32LZ + T32AB);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                    else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T32E);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                }
            }

            Periode1 = LagerZuBeginn[32] - BruttoT32P1;
            Periode2 = LagerZuBeginn[32] - BruttoT32P1 - BruttoT32P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[32] - BruttoT32P1 - BruttoT32P2 - BruttoT32P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[32] - BruttoT32P1 - BruttoT32P2 - BruttoT32P3 - BruttoT32P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge =  + Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                if (T32LZ + T32AB > 2)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = Periode3 * (-1);
            }
            if (Periode4 < 0 && Bestellmenge < Periode4 * (-1))
            {
                Bestellmenge = Periode4 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(32, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(32, LagerZuBeginn[32], BruttoT32P1, BruttoT32P2, BruttoT32P3, BruttoT32P4, Periode1, Periode2, Periode3, Periode4);


            //Teil33
            double T33LZ = 1.9;
            double T33AB = 0.5;
            double T33E = 1.9 / 2;
            int BruttoT33P1 = ((GlobalVariables.SaleMaleBikeN.GetValueOrDefault() +
                                +(int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text)))*2);
            int BruttoT33P2 = GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 2;
            int BruttoT33P3 = GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 2;
            int BruttoT33P4 = GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 2;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            foreach (DataRow DR in AlteBestellungen.Rows)
            {
                results = AlteBestellungen.Select("item = '33'");
                foreach (var row in results)
                {
                    if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T33LZ + T33AB);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                    else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T33E);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                }
            }

            Periode1 = LagerZuBeginn[33] - BruttoT33P1;
            Periode2 = LagerZuBeginn[33] - BruttoT33P1 - BruttoT33P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[33] - BruttoT33P1 - BruttoT33P2 - BruttoT33P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[33] - BruttoT33P1 - BruttoT33P2 - BruttoT33P3 - BruttoT33P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                if (T33LZ + T33AB > 2)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = Periode3 * (-1);
            }
            if (Periode4 < 0 && Bestellmenge < Periode4 * (-1))
            {
                Bestellmenge = Periode4 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(33, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(33, LagerZuBeginn[33], BruttoT33P1, BruttoT33P2, BruttoT33P3, BruttoT33P4, Periode1, Periode2, Periode3, Periode4);

            //Teil34
            double T34LZ = 1.6;
            double T34AB = 0.3;
            double T34E = 1.6 / 2;
            int BruttoT34P1 = ((GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 0)
                + ((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 72)
                + ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 0);
            int BruttoT34P2 = GlobalVariables.SaleChildBikeN1.Value * 72 + GlobalVariables.SaleFemaleBikeN1.Value * 0 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 0;
            int BruttoT34P3 = GlobalVariables.SaleChildBikeN2.Value * 72 + GlobalVariables.SaleFemaleBikeN2.Value * 0 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 0;
            int BruttoT34P4 = GlobalVariables.SaleChildBikeN3.Value * 72 + GlobalVariables.SaleFemaleBikeN3.Value * 0 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 0;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            foreach (DataRow DR in AlteBestellungen.Rows)
            {
                results = AlteBestellungen.Select("item = '34'");
                foreach (var row in results)
                {
                    if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T34LZ + T34AB);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                    else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T34E);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                }
            }

            Periode1 = LagerZuBeginn[34] - BruttoT34P1;
            Periode2 = LagerZuBeginn[34] - BruttoT34P1 - BruttoT34P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[34] - BruttoT34P1 - BruttoT34P2 - BruttoT34P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[34] - BruttoT34P1 - BruttoT34P2 - BruttoT34P3 - BruttoT34P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                if (T34LZ + T34AB > 2)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = Periode3 * (-1);
            }


            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(34, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(34, LagerZuBeginn[34], BruttoT34P1, BruttoT34P2, BruttoT34P3, BruttoT34P4, Periode1, Periode2, Periode3, Periode4);

            //Teil35
            double T35LZ = 2.2;
            double T35AB = 0.4;
            double T35E = 2.2 / 2;
            int BruttoT35P1 = (((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 4) +
                ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 5) +
                (GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 6);
            int BruttoT35P2 = GlobalVariables.SaleChildBikeN1.Value * 4 + GlobalVariables.SaleFemaleBikeN1.Value * 5 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 6;
            int BruttoT35P3 = GlobalVariables.SaleChildBikeN2.Value * 4 + GlobalVariables.SaleFemaleBikeN2.Value * 5 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 6;
            int BruttoT35P4 = GlobalVariables.SaleChildBikeN3.Value * 4 + GlobalVariables.SaleFemaleBikeN3.Value * 5 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 6;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            foreach (DataRow DR in AlteBestellungen.Rows)
            {
                results = AlteBestellungen.Select("item = '35'");
                foreach (var row in results)
                {
                    if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T35LZ + T35AB);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                    else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T35E);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                }
            }

            Periode1 = LagerZuBeginn[35] - BruttoT35P1;
            Periode2 = LagerZuBeginn[35] - BruttoT35P1 - BruttoT35P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[35] - BruttoT35P1 - BruttoT35P2 - BruttoT35P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[35] - BruttoT35P1 - BruttoT35P2 - BruttoT35P3 - BruttoT35P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = +Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                if (T35LZ + T35AB > 2)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = Periode3 * (-1);
            }
            if (Periode4 < 0 && Bestellmenge < Periode4 * (-1))
            {
                Bestellmenge = Periode4 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(35, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(35, LagerZuBeginn[35], BruttoT35P1, BruttoT35P2, BruttoT35P3, BruttoT35P4, Periode1, Periode2, Periode3, Periode4);

            //Teil36
            double T36LZ = 1.2;
            double T36AB = 0.1;
            double T36E = 1.2 / 2;
            int BruttoT36P1 = (((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 1) +
                ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 1) +
                (GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 1);
            int BruttoT36P2 = GlobalVariables.SaleChildBikeN1.Value * 1 + GlobalVariables.SaleFemaleBikeN1.Value * 1 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 1;
            int BruttoT36P3 = GlobalVariables.SaleChildBikeN2.Value * 1 + GlobalVariables.SaleFemaleBikeN2.Value * 1 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 1;
            int BruttoT36P4 = GlobalVariables.SaleChildBikeN3.Value * 1 + GlobalVariables.SaleFemaleBikeN3.Value * 1 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 1;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            results = AlteBestellungen.Select("item = '36'");
            foreach (var row in results)
            {
                if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T36LZ + T36AB);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
                else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T36E);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
            }


            Periode1 = LagerZuBeginn[36] - BruttoT36P1;
            Periode2 = LagerZuBeginn[36] - BruttoT36P1 - BruttoT36P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[36] - BruttoT36P1 - BruttoT36P2 - BruttoT36P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[36] - BruttoT36P1 - BruttoT36P2 - BruttoT36P3 - BruttoT36P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                if (T36LZ + T36AB > 2)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = Periode3 * (-1);
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(36, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(36, LagerZuBeginn[36], BruttoT36P1, BruttoT36P2, BruttoT36P3, BruttoT36P4, Periode1, Periode2, Periode3, Periode4);

            //Teil37
            double T37LZ = 1.5;
            double T37AB = 0.3;
            double T37E = 1.5 / 2;
            int BruttoT37P1 = (((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 1) +
                ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 1) +
                (GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 1);
            int BruttoT37P2 = GlobalVariables.SaleChildBikeN1.Value * 1 + GlobalVariables.SaleFemaleBikeN1.Value * 1 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 1;
            int BruttoT37P3 = GlobalVariables.SaleChildBikeN2.Value * 1 + GlobalVariables.SaleFemaleBikeN2.Value * 1 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 1;
            int BruttoT37P4 = GlobalVariables.SaleChildBikeN3.Value * 1 + GlobalVariables.SaleFemaleBikeN3.Value * 1 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 1;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            results = AlteBestellungen.Select("item = '37'");
            foreach (var row in results)
            {
                if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T37LZ + T37AB);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
                else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T37E);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
            }


            Periode1 = LagerZuBeginn[37] - BruttoT37P1;
            Periode2 = LagerZuBeginn[37] - BruttoT37P1 - BruttoT37P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[37] - BruttoT37P1 - BruttoT37P2 - BruttoT37P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[37] - BruttoT37P1 - BruttoT37P2 - BruttoT37P3 - BruttoT37P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                if (T37LZ + T37AB > 2)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = Periode3 * (-1);
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(37, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(37, LagerZuBeginn[37], BruttoT37P1, BruttoT37P2, BruttoT37P3, BruttoT37P4, Periode1, Periode2, Periode3, Periode4);

            //Teil38
            double T38LZ = 1.7;
            double T38AB = 0.4;
            double T38E = 1.7 / 2;
            int BruttoT38P1 = (((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 1) +
                ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 1) +
                (GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 1);
            int BruttoT38P2 = GlobalVariables.SaleChildBikeN1.Value * 1 + GlobalVariables.SaleFemaleBikeN1.Value * 1 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 1;
            int BruttoT38P3 = GlobalVariables.SaleChildBikeN2.Value * 1 + GlobalVariables.SaleFemaleBikeN2.Value * 1 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 1;
            int BruttoT38P4 = GlobalVariables.SaleChildBikeN3.Value * 1 + GlobalVariables.SaleFemaleBikeN3.Value * 1 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 1;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            foreach (DataRow DR in AlteBestellungen.Rows)
            {
                results = AlteBestellungen.Select("item = '38'");
                foreach (var row in results)
                {
                    if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T38LZ + T38AB);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                    else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T38E);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                }
            }

            Periode1 = LagerZuBeginn[38] - BruttoT38P1;
            Periode2 = LagerZuBeginn[38] - BruttoT38P1 - BruttoT38P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[38] - BruttoT38P1 - BruttoT38P2 - BruttoT38P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[38] - BruttoT38P1 - BruttoT38P2 - BruttoT38P3 - BruttoT38P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                if (T38LZ + T38AB > 2)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = Periode3 * (-1);
            }
            if (Periode4 < 0 && Bestellmenge < Periode4 * (-1))
            {
                Bestellmenge = Periode4 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(38, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(38, LagerZuBeginn[38], BruttoT38P1, BruttoT38P2, BruttoT38P3, BruttoT38P4, Periode1, Periode2, Periode3, Periode4);

            //Teil39
            double T39LZ = 1.5;
            double T39AB = 0.3;
            double T39E = 1.5 / 2;
            int BruttoT39P1 = (((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 2) +
                ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 2) +
                (GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 2);
            int BruttoT39P2 = GlobalVariables.SaleChildBikeN1.Value * 2 + GlobalVariables.SaleFemaleBikeN1.Value * 2 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 2;
            int BruttoT39P3 = GlobalVariables.SaleChildBikeN2.Value * 2 + GlobalVariables.SaleFemaleBikeN2.Value * 2 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 2;
            int BruttoT39P4 = GlobalVariables.SaleChildBikeN3.Value * 2 + GlobalVariables.SaleFemaleBikeN3.Value * 2 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 2;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            results = AlteBestellungen.Select("item = '39'");
            foreach (var row in results)
            {
                if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T39LZ + T39AB);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
                else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T39E);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
            }


            Periode1 = LagerZuBeginn[39] - BruttoT39P1;
            Periode2 = LagerZuBeginn[39] - BruttoT39P1 - BruttoT39P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[39] - BruttoT39P1 - BruttoT39P2 - BruttoT39P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[39] - BruttoT39P1 - BruttoT39P2 - BruttoT39P3 - BruttoT39P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                if (T39LZ + T39AB > 2)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = Periode3 * (-1);
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(39, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(39, LagerZuBeginn[39], BruttoT39P1, BruttoT39P2, BruttoT39P3, BruttoT39P4, Periode1, Periode2, Periode3, Periode4);

            //Teil40
            double T40LZ = 1.7;
            double T40AB = 0.2;
            double T40E = 1.7 / 2;
            int BruttoT40P1 = (((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 1) +
                ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 1) +
                (GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 1);
            int BruttoT40P2 = GlobalVariables.SaleChildBikeN1.Value * 1 + GlobalVariables.SaleFemaleBikeN1.Value * 1 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 1;
            int BruttoT40P3 = GlobalVariables.SaleChildBikeN2.Value * 1 + GlobalVariables.SaleFemaleBikeN2.Value * 1 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 1;
            int BruttoT40P4 = GlobalVariables.SaleChildBikeN3.Value * 1 + GlobalVariables.SaleFemaleBikeN3.Value * 1 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 1;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            results = AlteBestellungen.Select("item = '40'");
            foreach (var row in results)
            {
                if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T40LZ + T40AB);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
                else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T40E);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
            }


            Periode1 = LagerZuBeginn[40] - BruttoT40P1;
            Periode2 = LagerZuBeginn[40] - BruttoT40P1 - BruttoT40P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[40] - BruttoT40P1 - BruttoT40P2 - BruttoT40P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[40] - BruttoT40P1 - BruttoT40P2 - BruttoT40P3 - BruttoT40P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                if (T40LZ + T40AB > 2)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = Periode3 * (-1);
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(40, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(40, LagerZuBeginn[40], BruttoT40P1, BruttoT40P2, BruttoT40P3, BruttoT40P4, Periode1, Periode2, Periode3, Periode4);

            //Teil41
            double T41LZ = 0.9;
            double T41AB = 0.2;
            double T41E = 0.9 / 2;

            int BruttoT41P1 = (((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 1) +
                ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 1) +
                (GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 1);
            int BruttoT41P2 = GlobalVariables.SaleChildBikeN1.Value * 1 + GlobalVariables.SaleFemaleBikeN1.Value * 1 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 1;
            int BruttoT41P3 = GlobalVariables.SaleChildBikeN2.Value * 1 + GlobalVariables.SaleFemaleBikeN2.Value * 1 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 1;
            int BruttoT41P4 = GlobalVariables.SaleChildBikeN3.Value * 1 + GlobalVariables.SaleFemaleBikeN3.Value * 1 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 1;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;


            results = AlteBestellungen.Select("item = '41'");
            foreach (var row in results)
            {
                if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T41LZ + T41AB);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
                else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T41E);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
            }


            Periode1 = LagerZuBeginn[41] - BruttoT41P1;
            Periode2 = LagerZuBeginn[41] - BruttoT41P1 - BruttoT41P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[41] - BruttoT41P1 - BruttoT41P2 - BruttoT41P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[41] - BruttoT41P1 - BruttoT41P2 - BruttoT41P3 - BruttoT41P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = +Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                if (T41LZ + T41AB > 1)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = +Periode2 * (-1);
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                Bestellmenge = +Periode3 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(41, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(41, LagerZuBeginn[41], BruttoT41P1, BruttoT41P2, BruttoT41P3, BruttoT41P4, Periode1, Periode2, Periode3, Periode4);

            //Teil42
            double T42LZ = 1.2;
            double T42AB = 0.3;
            double T42E = 1.2 / 2;

            int BruttoT42P1 = (((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 2) +
                ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 2) +
                (GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 2);
            int BruttoT42P2 = GlobalVariables.SaleChildBikeN1.Value * 2 + GlobalVariables.SaleFemaleBikeN1.Value * 2 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 2;
            int BruttoT42P3 = GlobalVariables.SaleChildBikeN2.Value * 2 + GlobalVariables.SaleFemaleBikeN2.Value * 2 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 2;
            int BruttoT42P4 = GlobalVariables.SaleChildBikeN3.Value * 2 + GlobalVariables.SaleFemaleBikeN3.Value * 2 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 2;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;


            results = AlteBestellungen.Select("item = '42'");
            foreach (var row in results)
            {
                if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T42LZ + T42AB);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
                else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T42E);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
            }


            Periode1 = LagerZuBeginn[42] - BruttoT42P1;
            Periode2 = LagerZuBeginn[42] - BruttoT42P1 - BruttoT42P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[42] - BruttoT42P1 - BruttoT42P2 - BruttoT42P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[42] - BruttoT42P1 - BruttoT42P2 - BruttoT42P3 - BruttoT42P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = +Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                if (T42LZ + T42AB > 1)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = +Periode2 * (-1);
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                Bestellmenge = +Periode3 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(42, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(42, LagerZuBeginn[42], BruttoT42P1, BruttoT42P2, BruttoT42P3, BruttoT42P4, Periode1, Periode2, Periode3, Periode4);

            //Teil43
            double T43LZ = 2.0;
            double T43AB = 0.5;
            double T43E = 2.0 / 2;

            int BruttoT43P1 = (((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 1) +
                ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 1) +
                (GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 1);
            int BruttoT43P2 = GlobalVariables.SaleChildBikeN1.Value * 1 + GlobalVariables.SaleFemaleBikeN1.Value * 1 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 1;
            int BruttoT43P3 = GlobalVariables.SaleChildBikeN2.Value * 1 + GlobalVariables.SaleFemaleBikeN2.Value * 1 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 1;
            int BruttoT43P4 = GlobalVariables.SaleChildBikeN3.Value * 1 + GlobalVariables.SaleFemaleBikeN3.Value * 1 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 1;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            foreach (DataRow DR in AlteBestellungen.Rows)
            {
                results = AlteBestellungen.Select("item = '43'");
                foreach (var row in results)
                {
                    if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T43LZ + T43AB);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                    else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T43E);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                }
            }

            Periode1 = LagerZuBeginn[43] - BruttoT43P1;
            Periode2 = LagerZuBeginn[43] - BruttoT43P1 - BruttoT43P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[43] - BruttoT43P1 - BruttoT43P2 - BruttoT43P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[43] - BruttoT43P1 - BruttoT43P2 - BruttoT43P3 - BruttoT43P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = +Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                if (T43LZ + T43AB > 2)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = Periode3 * (-1);
            }
            if (Periode4 < 0 && Bestellmenge < Periode4 * (-1))
            {
                Bestellmenge = Periode4 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(43, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(43, LagerZuBeginn[43], BruttoT43P1, BruttoT43P2, BruttoT43P3, BruttoT43P4, Periode1, Periode2, Periode3, Periode4);


            //Teil44
            double T44LZ = 1.0;
            double T44AB = 0.2;
            double T44E = 1.0 / 2;
            int BruttoT44P1 = (((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 3) +
                ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 3) +
                (GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 3);
            int BruttoT44P2 = GlobalVariables.SaleChildBikeN1.Value * 3 + GlobalVariables.SaleFemaleBikeN1.Value * 3 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 3;
            int BruttoT44P3 = GlobalVariables.SaleChildBikeN2.Value * 3 + GlobalVariables.SaleFemaleBikeN2.Value * 3 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 3;
            int BruttoT44P4 = GlobalVariables.SaleChildBikeN3.Value * 3 + GlobalVariables.SaleFemaleBikeN3.Value * 3 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 3;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;


            results = AlteBestellungen.Select("item = '44'");
            foreach (var row in results)
            {
                if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T44LZ + T44AB);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
                else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T44E);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
            }


            Periode1 = LagerZuBeginn[44] - BruttoT44P1;
            Periode2 = LagerZuBeginn[44] - BruttoT44P1 - BruttoT44P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[44] - BruttoT44P1 - BruttoT44P2 - BruttoT44P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[44] - BruttoT44P1 - BruttoT44P2 - BruttoT44P3 - BruttoT44P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = +Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                if (T44LZ + T44AB > 1)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = +Periode2 * (-1);
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                Bestellmenge = +Periode3 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(44, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(44, LagerZuBeginn[44], BruttoT44P1, BruttoT44P2, BruttoT44P3, BruttoT44P4, Periode1, Periode2, Periode3, Periode4);

            //Teil45
            double T45LZ = 1.7;
            double T45AB = 0.3;
            double T45E = 1.7 / 2;
            int BruttoT45P1 = (((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 1) +
                ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 1) +
                (GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 1);
            int BruttoT45P2 = GlobalVariables.SaleChildBikeN1.Value * 1 + GlobalVariables.SaleFemaleBikeN1.Value * 1 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 1;
            int BruttoT45P3 = GlobalVariables.SaleChildBikeN2.Value * 1 + GlobalVariables.SaleFemaleBikeN2.Value * 1 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 1;
            int BruttoT45P4 = GlobalVariables.SaleChildBikeN3.Value * 1 + GlobalVariables.SaleFemaleBikeN3.Value * 1 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 1;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            foreach (DataRow DR in AlteBestellungen.Rows)
            {
                results = AlteBestellungen.Select("item = '45'");
                foreach (var row in results)
                {
                    if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T45LZ + T45AB);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                    else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T45E);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                }
            }

            Periode1 = LagerZuBeginn[45] - BruttoT45P1;
            Periode2 = LagerZuBeginn[45] - BruttoT45P1 - BruttoT45P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[45] - BruttoT45P1 - BruttoT45P2 - BruttoT45P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[45] - BruttoT45P1 - BruttoT45P2 - BruttoT45P3 - BruttoT45P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                if (T45LZ + T45AB > 2)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = Periode3 * (-1);
            }


            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(45, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(45, LagerZuBeginn[45], BruttoT45P1, BruttoT45P2, BruttoT45P3, BruttoT45P4, Periode1, Periode2, Periode3, Periode4);

            //Teil46
            double T46LZ = 0.9;
            double T46AB = 0.3;
            double T46E = 0.9 / 2;
            int BruttoT46P1 = (((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 1) +
                ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 1) +
                (GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 1);
            int BruttoT46P2 = GlobalVariables.SaleChildBikeN1.Value * 1 + GlobalVariables.SaleFemaleBikeN1.Value * 1 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 1;
            int BruttoT46P3 = GlobalVariables.SaleChildBikeN2.Value * 1 + GlobalVariables.SaleFemaleBikeN2.Value * 1 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 1;
            int BruttoT46P4 = GlobalVariables.SaleChildBikeN3.Value * 1 + GlobalVariables.SaleFemaleBikeN3.Value * 1 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 1;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;


            results = AlteBestellungen.Select("item = '46'");
            foreach (var row in results)
            {
                if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T46LZ + T46AB);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
                else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T46E);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
            }


            Periode1 = LagerZuBeginn[46] - BruttoT46P1;
            Periode2 = LagerZuBeginn[46] - BruttoT46P1 - BruttoT46P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[46] - BruttoT46P1 - BruttoT46P2 - BruttoT46P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[46] - BruttoT46P1 - BruttoT46P2 - BruttoT46P3 - BruttoT46P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = +Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                if (T46LZ + T46AB > 1)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = +Periode2 * (-1);
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                Bestellmenge = +Periode3 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(46, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(46, LagerZuBeginn[46], BruttoT46P1, BruttoT46P2, BruttoT46P3, BruttoT46P4, Periode1, Periode2, Periode3, Periode4);

            //Teil47
            double T47LZ = 1.1;
            double T47AB = 0.1;
            double T47E = 1.1 / 2;
            int BruttoT47P1 = (((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 1) +
                ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 1) +
                (GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 1);
            int BruttoT47P2 = GlobalVariables.SaleChildBikeN1.Value * 1 + GlobalVariables.SaleFemaleBikeN1.Value * 1 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 1;
            int BruttoT47P3 = GlobalVariables.SaleChildBikeN2.Value * 1 + GlobalVariables.SaleFemaleBikeN2.Value * 1 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 1;
            int BruttoT47P4 = GlobalVariables.SaleChildBikeN3.Value * 1 + GlobalVariables.SaleFemaleBikeN3.Value * 1 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 1;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;


            results = AlteBestellungen.Select("item = '47'");
            foreach (var row in results)
            {
                if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T47LZ + T47AB);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
                else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T47E);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
            }


            Periode1 = LagerZuBeginn[47] - BruttoT47P1;
            Periode2 = LagerZuBeginn[47] - BruttoT47P1 - BruttoT47P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[47] - BruttoT47P1 - BruttoT47P2 - BruttoT47P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[47] - BruttoT47P1 - BruttoT47P2 - BruttoT47P3 - BruttoT47P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = +Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                if (T47LZ + T47AB > 1)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = +Periode2 * (-1);
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                Bestellmenge = +Periode3 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(47, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(47, LagerZuBeginn[47], BruttoT47P1, BruttoT47P2, BruttoT47P3, BruttoT47P4, Periode1, Periode2, Periode3, Periode4);

            //Teil48
            double T48LZ = 1.0;
            double T48AB = 0.2;
            double T48E = 1.0 / 2;
            int BruttoT48P1 = (((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 2) +
                ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 2) +
                (GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 2);
            int BruttoT48P2 = GlobalVariables.SaleChildBikeN1.Value * 2 + GlobalVariables.SaleFemaleBikeN1.Value * 2 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 2;
            int BruttoT48P3 = GlobalVariables.SaleChildBikeN2.Value * 2 + GlobalVariables.SaleFemaleBikeN2.Value * 2 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 2;
            int BruttoT48P4 = GlobalVariables.SaleChildBikeN3.Value * 2 + GlobalVariables.SaleFemaleBikeN3.Value * 2 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 2;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;


            results = AlteBestellungen.Select("item = '48'");
            foreach (var row in results)
            {
                if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T48LZ + T48AB);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
                else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T48E);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
            }


            Periode1 = LagerZuBeginn[48] - BruttoT48P1;
            Periode2 = LagerZuBeginn[48] - BruttoT48P1 - BruttoT48P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[48] - BruttoT48P1 - BruttoT48P2 - BruttoT48P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[48] - BruttoT48P1 - BruttoT48P2 - BruttoT48P3 - BruttoT48P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = +Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                if (T48LZ + T48AB > 1)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = +Periode2 * (-1);
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                Bestellmenge = +Periode3 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(48, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(48, LagerZuBeginn[48], BruttoT48P1, BruttoT48P2, BruttoT48P3, BruttoT48P4, Periode1, Periode2, Periode3, Periode4);

            //Teil52
            double T52LZ = 2.0;
            double T52AB = 0.4;
            double T52E = 1.6 / 2;
            int BruttoT52P1 = (((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 2) +
                ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 0) +
                (GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 0);
            int BruttoT52P2 = GlobalVariables.SaleChildBikeN1.Value * 2 + GlobalVariables.SaleFemaleBikeN1.Value * 0 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 0;
            int BruttoT52P3 = GlobalVariables.SaleChildBikeN2.Value * 2 + GlobalVariables.SaleFemaleBikeN2.Value * 0 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 0;
            int BruttoT52P4 = GlobalVariables.SaleChildBikeN3.Value * 2 + GlobalVariables.SaleFemaleBikeN3.Value * 0 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 0;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            foreach (DataRow DR in AlteBestellungen.Rows)
            {
                results = AlteBestellungen.Select("item = '52'");
                foreach (var row in results)
                {
                    if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T52LZ + T52AB);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                    else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                    {
                        double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T52E);
                        if (ZeitCheck <= 1)
                        {
                            P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                        }
                        if (ZeitCheck <= 2)
                        {
                            P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                        if (ZeitCheck <= 3)
                        {
                            P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                        }
                    }
                }
            }

            Periode1 = LagerZuBeginn[52] - BruttoT52P1;
            Periode2 = LagerZuBeginn[52] - BruttoT52P1 - BruttoT52P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[52] - BruttoT52P1 - BruttoT52P2 - BruttoT52P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[52] - BruttoT52P1 - BruttoT52P2 - BruttoT52P3 - BruttoT52P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                if (T52LZ + T52AB > 2)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = Periode3 * (-1);
            }
            if (Periode4 < 0 && Bestellmenge < Periode4 * (-1))
            {
                Bestellmenge = Periode4 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(52, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(52, LagerZuBeginn[52], BruttoT52P1, BruttoT52P2, BruttoT52P3, BruttoT52P4, Periode1, Periode2, Periode3, Periode4);

            //Teil53
            double T53LZ = 1.6;
            double T53AB = 0.2;
            double T53E = 1.6 / 2;
            int BruttoT53P1 = (((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 72) +
                ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 0) +
                (GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 0);
            int BruttoT53P2 = GlobalVariables.SaleChildBikeN1.Value * 72 + GlobalVariables.SaleFemaleBikeN1.Value * 0 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 0;
            int BruttoT53P3 = GlobalVariables.SaleChildBikeN2.Value * 72 + GlobalVariables.SaleFemaleBikeN2.Value * 0 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 0;
            int BruttoT53P4 = GlobalVariables.SaleChildBikeN3.Value * 72 + GlobalVariables.SaleFemaleBikeN3.Value * 0 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 0;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            results = AlteBestellungen.Select("item = '53'");
            foreach (var row in results)
            {
                if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T53LZ + T53AB);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
                else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T53E);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
            }


            Periode1 = LagerZuBeginn[53] - BruttoT53P1;
            Periode2 = LagerZuBeginn[53] - BruttoT53P1 - BruttoT53P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[53] - BruttoT53P1 - BruttoT53P2 - BruttoT53P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[53] - BruttoT53P1 - BruttoT53P2 - BruttoT53P3 - BruttoT53P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                if (T53LZ + T53AB > 2)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = Periode3 * (-1);
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(53, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(53, LagerZuBeginn[53], BruttoT53P1, BruttoT53P2, BruttoT53P3, BruttoT53P4, Periode1, Periode2, Periode3, Periode4);

            //Teil57
            double T57LZ = 1.7;
            double T57AB = 0.3;
            double T57E = 1.7 / 2;
            int BruttoT57P1 = (((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 0) +
                ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 2) +
                (GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 0);
            int BruttoT57P2 = GlobalVariables.SaleChildBikeN1.Value * 0 + GlobalVariables.SaleFemaleBikeN1.Value * 2 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 0;
            int BruttoT57P3 = GlobalVariables.SaleChildBikeN2.Value * 0 + GlobalVariables.SaleFemaleBikeN2.Value * 2 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 0;
            int BruttoT57P4 = GlobalVariables.SaleChildBikeN3.Value * 0 + GlobalVariables.SaleFemaleBikeN3.Value * 2 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 0;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            results = AlteBestellungen.Select("item = '57'");
            foreach (var row in results)
            {
                if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T57LZ + T57AB);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
                else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T57E);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
            }


            Periode1 = LagerZuBeginn[57] - BruttoT57P1;
            Periode2 = LagerZuBeginn[57] - BruttoT57P1 - BruttoT57P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[57] - BruttoT57P1 - BruttoT57P2 - BruttoT57P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[57] - BruttoT57P1 - BruttoT57P2 - BruttoT57P3 - BruttoT57P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                if (T57LZ + T57AB > 2)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = Periode3 * (-1);
            }
            if (Periode4 < 0 && Bestellmenge < Periode4 * (-1))
            {
                Bestellmenge = Periode4 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(57, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(57, LagerZuBeginn[57], BruttoT57P1, BruttoT57P2, BruttoT57P3, BruttoT57P4, Periode1, Periode2, Periode3, Periode4);

            //Teil58
            double T58LZ = 1.6;
            double T58AB = 0.5;
            double T58E = 1.6 / 2;
            int BruttoT58P1 = (((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 0) +
                ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 72) +
                (GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 0);
            int BruttoT58P2 = GlobalVariables.SaleChildBikeN1.Value * 0 + GlobalVariables.SaleFemaleBikeN1.Value * 72 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 0;
            int BruttoT58P3 = GlobalVariables.SaleChildBikeN2.Value * 0 + GlobalVariables.SaleFemaleBikeN2.Value * 72 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 0;
            int BruttoT58P4 = GlobalVariables.SaleChildBikeN3.Value * 0 + GlobalVariables.SaleFemaleBikeN3.Value * 72 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 0;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            results = AlteBestellungen.Select("item = '58'");
            foreach (var row in results)
            {
                if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T58LZ + T58AB);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
                else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T58E);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
            }


            Periode1 = LagerZuBeginn[58] - BruttoT58P1;
            Periode2 = LagerZuBeginn[58] - BruttoT58P1 - BruttoT58P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[58] - BruttoT58P1 - BruttoT58P2 - BruttoT58P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[58] - BruttoT58P1 - BruttoT58P2 - BruttoT58P3 - BruttoT58P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                Bestellmenge = Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                if (T58LZ + T58AB > 2)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = Periode3 * (-1);
            }
            if (Periode4 < 0 && Bestellmenge < Periode4 * (-1))
            {
                Bestellmenge = Periode4 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(58, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(58, LagerZuBeginn[58], BruttoT58P1, BruttoT58P2, BruttoT58P3, BruttoT58P4, Periode1, Periode2, Periode3, Periode4);

            //Teil59
            double T59LZ = 0.7;
            double T59AB = 0.2;
            double T59E = 0.7 / 2;
            int BruttoT59P1 = (((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 2) +
                ((GlobalVariables.SaleFemaleBikeN.Value + (int.Parse(FemaleBikeSafetyP2.Text) - int.Parse(FemaleBikeStockP2.Text))) * 2) +
                (GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 2);
            int BruttoT59P2 = GlobalVariables.SaleChildBikeN1.Value * 2 + GlobalVariables.SaleFemaleBikeN1.Value * 2 + GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 2;
            int BruttoT59P3 = GlobalVariables.SaleChildBikeN2.Value * 2 + GlobalVariables.SaleFemaleBikeN2.Value * 2 + GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 2;
            int BruttoT59P4 = GlobalVariables.SaleChildBikeN3.Value * 2 + GlobalVariables.SaleFemaleBikeN3.Value * 2 + GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 2;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            results = AlteBestellungen.Select("item = '59'");
            foreach (var row in results)
            {
                if (int.Parse(row["Modus"].ToString()) == 5) //5 normal
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T59LZ + T59AB);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
                else if (int.Parse(row["Modus"].ToString()) == 4) //4 Eil
                {
                    double ZeitCheck = (int.Parse(row["Vergangenheit"].ToString()) + T59E);
                    if (ZeitCheck <= 1)
                    {
                        P2Zuwachs = (P2Zuwachs + int.Parse(row["Menge"].ToString()));
                    }
                    if (ZeitCheck <= 2)
                    {
                        P3Zuwachs = P3Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                    if (ZeitCheck <= 3)
                    {
                        P4Zuwachs = P4Zuwachs + int.Parse(row["Menge"].ToString());
                    }
                }
            }


            Periode1 = LagerZuBeginn[59] - BruttoT59P1;
            Periode2 = LagerZuBeginn[59] - BruttoT59P1 - BruttoT59P2 + P2Zuwachs;
            Periode3 = LagerZuBeginn[59] - BruttoT59P1 - BruttoT59P2 - BruttoT59P3 + P3Zuwachs;
            Periode4 = LagerZuBeginn[59] - BruttoT59P1 - BruttoT59P2 - BruttoT59P3 - BruttoT59P2 + P4Zuwachs;

            Modus = 5;
            Bestellmenge = 0;
            if (Periode1 < 0)
            {
                Bestellmenge = Bestellmenge + Periode1 * (-1);
                Modus = 4;
            }
            if (Periode2 < 0 && Bestellmenge < Periode2 * (-1))
            {
                if (T59LZ + T59AB > 1)
                    Modus = 4;
                else
                {
                    if (Modus != 4)
                        Modus = 5;
                }
                Bestellmenge = Periode2 * (-1);
            }
            if (Periode3 < 0)
            {
                Bestellmenge =  + Periode3 * (-1);
                if (Modus != 4)
                    Modus = 5;
            }

            if (Bestellmenge > 0)
            {
                Bestellungsliste.Rows.Add(59, Bestellmenge, Modus);
            }

            Bestellungsplannung.Rows.Add(59, LagerZuBeginn[59], BruttoT59P1, BruttoT59P2, BruttoT59P3, BruttoT59P4, Periode1, Periode2, Periode3, Periode4);



            ////Lager befüllen
            //foreach (DataRow r1Row in GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables[2].Rows)
            //{
            //    foreach (DataRow r2Row in Bestellungsplannung.Rows)
            //    {
            //        if (r2Row["Teil"].ToString() == r1Row["id"].ToString())
            //        {
            //            r2Row["Lagerbestand"] = int.Parse(r1Row["amount"].ToString());
            //        }
            //    }
            //}


            Bestellung.DataContext = Bestellungsplannung.DefaultView;

            BestellungslisteZu.DataContext = Bestellungsliste;
            BestellungslisteZu.ItemsSource = Bestellungsliste.DefaultView;

            Orderlist.DataContext = BestellungslisteZu.DataContext;
            Orderlist.ItemsSource = BestellungslisteZu.ItemsSource;

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
            if (old < GlobalVariables.dtProdOrder.Rows.Count-1)
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


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            List<XMLsellwish> VerkäufeImport = new List<XMLsellwish>();
            List<XMLselldirect> DirektVerkäufe = new List<XMLselldirect>();
            List<XMLorderlist> Bestellungen = new List<XMLorderlist>();
            List<XMLproductionlist> Aufträge = new List<XMLproductionlist>();
            List<XMLworkingtimelist> Schichten = new List<XMLworkingtimelist>();

            foreach (System.Data.DataRowView dr in Sellwish.ItemsSource)
                {
                if(dr[0] != DBNull.Value && dr[1] != DBNull.Value)
                    VerkäufeImport.Add(new XMLsellwish((int)dr[0],(int)dr[1]));
                }

            foreach (System.Data.DataRowView dr in Orderlist.ItemsSource)
            {
                if (dr[0] != DBNull.Value && dr[1] != DBNull.Value && dr[2] != DBNull.Value)
                    Bestellungen.Add(new XMLorderlist((int)dr[0], (int)dr[1], (int)dr[2]));
            }

            foreach (System.Data.DataRowView dr in Selldirect.ItemsSource)
            {
                if (dr[0] != DBNull.Value && dr[1] != DBNull.Value && dr[2] != DBNull.Value && dr[3] != DBNull.Value)
                    DirektVerkäufe.Add(new XMLselldirect((int)dr["quantity"], (int)dr["article"], (double)dr["price"], (double)dr["penalty"]));
            }

            foreach (System.Data.DataRowView dr in Productionlist.ItemsSource)
            {
                if (dr[0] != DBNull.Value && dr[1] != DBNull.Value)
                    Aufträge.Add(new XMLproductionlist((int)dr[0], (int)dr[1]));
            }

            foreach (System.Data.DataRowView dr in Workingtimelist.ItemsSource)
            {
                if (dr[0] != DBNull.Value && dr[1] != DBNull.Value && dr[2] != DBNull.Value)
                    Schichten.Add(new XMLworkingtimelist((int)dr[0], (int)dr[1], (int)dr[2]));
            }





            XMLExport Ende = new XMLExport();
            Ende.XMLExportReal(VerkäufeImport, DirektVerkäufe, Bestellungen, Aufträge, Schichten);

        }

        private void Workingtimelist_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void TextBox_PreviewTextInput1to3(object sender, TextCompositionEventArgs e)
        {
                e.Handled = new Regex("[^1-3]{1}").IsMatch(e.Text);
        }

        private void TextBox_PreviewTextInput1to240(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^1-3]{1}").IsMatch(e.Text);
        }

        private void Handbuch(object sender, RoutedEventArgs e)
        {
            string Pfad = System.AppDomain.CurrentDomain.BaseDirectory;

            System.Diagnostics.Process.Start(Pfad + @"Handbuch.pdf");
        }
    }
}
