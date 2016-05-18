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

            //Stock
            //P123
            DataRow[] results = data.Tables[2].Select("id = '16'");
            int E16 = Int32.Parse(results[0].ItemArray[1].ToString());
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
            int E17 = Int32.Parse(results[0].ItemArray[1].ToString());
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
            int E26 = Int32.Parse(results[0].ItemArray[1].ToString());
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


            //P3


            //DataRow[] results = data.Tables[2].Select("id = '1'");
            //string i2 = results[0].ItemArray[1].ToString();
            //DataRow[] results2 = data.Tables[2].Select("id = '21'");
            //string i3 = results2[0].ItemArray[1].ToString();

            //string i = data.Tables[2].Rows[1].ItemArray[1].ToString();
            //MessageBox.Show(i2);
            //MessageBox.Show(i3);

        }

        private void ProgrammplannungRepeat()
        {
            
        }

        #endregion Data
    }
}
