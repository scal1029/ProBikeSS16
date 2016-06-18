using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml;
using System.Xml.Linq;
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
                DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(3));

                animation.Completed += (s, t) =>
                {
                    Okay1.BeginAnimation(Image.OpacityProperty, null);
                    Okay1.Visibility = Visibility.Hidden;
                };

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

                    animation.Completed += (s, t) =>
                    {
                        Okay2.BeginAnimation(Image.OpacityProperty, null);
                        Okay2.Visibility = Visibility.Hidden;
                    };

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
                Tab2.IsEnabled = true;
                Tab3.IsEnabled = true;
                Tab4.IsEnabled = true;
                Tab5.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Mistakes");
            }
            Okay3.Visibility = Visibility.Visible;
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(3));

            animation.Completed += (s, t) =>
            {
                Okay3.BeginAnimation(Image.OpacityProperty, null);
                Okay3.Visibility = Visibility.Hidden;
            };

            Okay3.BeginAnimation(Image.OpacityProperty, animation);

            refreshKapaPlanInputs();
            refreshAmountKapaPlan();
            calculateCapNeed();
            calculateSetUpTime();
            calculateOldSetUpTime();
            calcWholeCap();
            calcWorkLoad();
            calculateShiftAndOverDo();
        }

        #region Refreshing Functions
        public void refreshKapaPlanInputs()
        {
            E4A10.Text = GlobalVariables.E4Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_10.getApproxProdTimed11(GlobalVariables.E4Produktionsauftrag).ToString();
            E5A10.Text = GlobalVariables.E5Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_10.getApproxProdTimed11(GlobalVariables.E5Produktionsauftrag).ToString();
            E6A10.Text = GlobalVariables.E6Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_10.getApproxProdTimed11(GlobalVariables.E6Produktionsauftrag).ToString();
            E7A10.Text = GlobalVariables.E7Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_10.getApproxProdTimed11(GlobalVariables.E7Produktionsauftrag).ToString();
            E8A10.Text = GlobalVariables.E8Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_10.getApproxProdTimed11(GlobalVariables.E8Produktionsauftrag).ToString();
            E9A10.Text = GlobalVariables.E9Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_10.getApproxProdTimed11(GlobalVariables.E9Produktionsauftrag).ToString();

            E4A11.Text = GlobalVariables.E4Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_11.getApproxProdTimeE4(GlobalVariables.E4Produktionsauftrag).ToString();
            E5A11.Text = GlobalVariables.E5Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_11.getApproxProdTimeE5(GlobalVariables.E5Produktionsauftrag).ToString();
            E6A11.Text = GlobalVariables.E6Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_11.getApproxProdTimeE6(GlobalVariables.E6Produktionsauftrag).ToString();
            E7A11.Text = GlobalVariables.E7Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_11.getApproxProdTimeE7(GlobalVariables.E7Produktionsauftrag).ToString();
            E8A11.Text = GlobalVariables.E8Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_11.getApproxProdTimeE8(GlobalVariables.E8Produktionsauftrag).ToString();
            E9A11.Text = GlobalVariables.E9Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_11.getApproxProdTimeE9(GlobalVariables.E9Produktionsauftrag).ToString();

            E10A7.Text = GlobalVariables.E10Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_7.getApproxProdTimeOd9(GlobalVariables.E10Produktionsauftrag).ToString();
            E11A7.Text = GlobalVariables.E11Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_7.getApproxProdTimeOd9(GlobalVariables.E11Produktionsauftrag).ToString();
            E12A7.Text = GlobalVariables.E12Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_7.getApproxProdTimeOd9(GlobalVariables.E12Produktionsauftrag).ToString();
            E13A7.Text = GlobalVariables.E13Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_7.getApproxProdTimeOd9(GlobalVariables.E13Produktionsauftrag).ToString();
            E14A7.Text = GlobalVariables.E14Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_7.getApproxProdTimeOd9(GlobalVariables.E14Produktionsauftrag).ToString();
            E15A7.Text = GlobalVariables.E15Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_7.getApproxProdTimeOd9(GlobalVariables.E15Produktionsauftrag).ToString();

            E10A8.Text = GlobalVariables.E10Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_8.getApproxProdTimeOd7_1_p1(GlobalVariables.E10Produktionsauftrag).ToString();
            E11A8.Text = GlobalVariables.E11Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_8.getApproxProdTimeOd7_1_p2(GlobalVariables.E11Produktionsauftrag).ToString();
            E12A8.Text = GlobalVariables.E12Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_8.getApproxProdTimeOd7_1_p3(GlobalVariables.E12Produktionsauftrag).ToString();
            E13A8.Text = GlobalVariables.E13Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_8.getApproxProdTimeOd7_3_p1(GlobalVariables.E13Produktionsauftrag).ToString();
            E14A8.Text = GlobalVariables.E14Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_8.getApproxProdTimeOd7_3_p2(GlobalVariables.E14Produktionsauftrag).ToString();
            E15A8.Text = GlobalVariables.E15Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_8.getApproxProdTimeOd7_3_p3(GlobalVariables.E15Produktionsauftrag).ToString();

            E10A9.Text = GlobalVariables.E10Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_9.getApproxProdTime3(GlobalVariables.E10Produktionsauftrag).ToString();
            E11A9.Text = GlobalVariables.E11Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_9.getApproxProdTime3(GlobalVariables.E11Produktionsauftrag).ToString();
            E12A9.Text = GlobalVariables.E12Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_9.getApproxProdTime3(GlobalVariables.E12Produktionsauftrag).ToString();
            E13A9.Text = GlobalVariables.E13Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_9.getApproxProdTime3(GlobalVariables.E13Produktionsauftrag).ToString();
            E14A9.Text = GlobalVariables.E14Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_9.getApproxProdTime3(GlobalVariables.E14Produktionsauftrag).ToString();
            E15A9.Text = GlobalVariables.E15Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_9.getApproxProdTime3(GlobalVariables.E15Produktionsauftrag).ToString();

            E10A12.Text = GlobalVariables.E10Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_12.getApproxProdTimeDirectTo8(GlobalVariables.E10Produktionsauftrag).ToString();
            E11A12.Text = GlobalVariables.E11Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_12.getApproxProdTimeDirectTo8(GlobalVariables.E11Produktionsauftrag).ToString();
            E12A12.Text = GlobalVariables.E12Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_12.getApproxProdTimeDirectTo8(GlobalVariables.E12Produktionsauftrag).ToString();
            E13A12.Text = GlobalVariables.E13Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_12.getApproxProdTimeDirectTo8(GlobalVariables.E13Produktionsauftrag).ToString();
            E14A12.Text = GlobalVariables.E14Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_12.getApproxProdTimeDirectTo8(GlobalVariables.E14Produktionsauftrag).ToString();
            E15A12.Text = GlobalVariables.E15Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_12.getApproxProdTimeDirectTo8(GlobalVariables.E15Produktionsauftrag).ToString();

            E10A13.Text = GlobalVariables.E10Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_13.getApproxProdTimeDirectTo12(GlobalVariables.E10Produktionsauftrag).ToString();
            E11A13.Text = GlobalVariables.E11Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_13.getApproxProdTimeDirectTo12(GlobalVariables.E11Produktionsauftrag).ToString();
            E12A13.Text = GlobalVariables.E12Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_13.getApproxProdTimeDirectTo12(GlobalVariables.E12Produktionsauftrag).ToString();
            E13A13.Text = GlobalVariables.E13Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_13.getApproxProdTimeDirectTo12(GlobalVariables.E13Produktionsauftrag).ToString();
            E14A13.Text = GlobalVariables.E14Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_13.getApproxProdTimeDirectTo12(GlobalVariables.E14Produktionsauftrag).ToString();
            E15A13.Text = GlobalVariables.E15Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_13.getApproxProdTimeDirectTo12(GlobalVariables.E15Produktionsauftrag).ToString();

            E16A6.Text = GlobalVariables.E16Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_6.getApproxProdTimeOd14_p1(GlobalVariables.E16Produktionsauftrag).ToString();
            E16A14.Text = GlobalVariables.E16Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_14.getApproxProdTimeE16(GlobalVariables.E16Produktionsauftrag).ToString();

            E17A15.Text = GlobalVariables.E17Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_15.getApproxProdTimeE17(GlobalVariables.E17Produktionsauftrag).ToString();

            E18A6.Text = GlobalVariables.E18Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_6.getApproxProdTimeOd8_p1(GlobalVariables.E18Produktionsauftrag).ToString();
            E19A6.Text = GlobalVariables.E19Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_6.getApproxProdTimeOd8_p2(GlobalVariables.E19Produktionsauftrag).ToString();
            E20A6.Text = GlobalVariables.E20Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_6.getApproxProdTimeOd8_p3(GlobalVariables.E20Produktionsauftrag).ToString();

            E18A7.Text = GlobalVariables.E18Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_7.getApproxProdTimeOd9(GlobalVariables.E18Produktionsauftrag).ToString();
            E19A7.Text = GlobalVariables.E19Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_7.getApproxProdTimeOd9(GlobalVariables.E19Produktionsauftrag).ToString();
            E20A7.Text = GlobalVariables.E20Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_7.getApproxProdTimeOd9(GlobalVariables.E20Produktionsauftrag).ToString();

            E18A8.Text = GlobalVariables.E18Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_8.getApproxProdTimeOd7_2_p1(GlobalVariables.E18Produktionsauftrag).ToString();
            E19A8.Text = GlobalVariables.E19Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_8.getApproxProdTimeOd7_2_p2(GlobalVariables.E19Produktionsauftrag).ToString();
            E20A8.Text = GlobalVariables.E20Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_8.getApproxProdTimeOd7_2_p3(GlobalVariables.E20Produktionsauftrag).ToString();

            E18A9.Text = GlobalVariables.E18Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_9.getApproxProdTime2(GlobalVariables.E18Produktionsauftrag).ToString();
            E19A9.Text = GlobalVariables.E19Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_9.getApproxProdTime2(GlobalVariables.E19Produktionsauftrag).ToString();
            E20A9.Text = GlobalVariables.E20Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_9.getApproxProdTime2(GlobalVariables.E20Produktionsauftrag).ToString();

            E26A7.Text = GlobalVariables.E26Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_7.getApproxProdTimeOd9(GlobalVariables.E26Produktionsauftrag).ToString();
            E26A15.Text = GlobalVariables.E26Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_15.getApproxProdTimeE26(GlobalVariables.E26Produktionsauftrag).ToString();

            E49A1.Text = GlobalVariables.E49Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_1.getApproxProdTimeE49(GlobalVariables.E49Produktionsauftrag).ToString();
            E54A1.Text = GlobalVariables.E54Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_1.getApproxProdTimeE54(GlobalVariables.E54Produktionsauftrag).ToString();
            E29A1.Text = GlobalVariables.E29Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_1.getApproxProdTimeE29(GlobalVariables.E29Produktionsauftrag).ToString();

            E50A2.Text = GlobalVariables.E50Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_2.getApproxProdTimeE50(GlobalVariables.E50Produktionsauftrag).ToString();
            E55A2.Text = GlobalVariables.E55Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_2.getApproxProdTimeE55(GlobalVariables.E55Produktionsauftrag).ToString();
            E30A2.Text = GlobalVariables.E30Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_2.getApproxProdTimeE30(GlobalVariables.E30Produktionsauftrag).ToString();

            E51A3.Text = GlobalVariables.E51Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_3.getApproxProdTimeE51(GlobalVariables.E51Produktionsauftrag).ToString();
            E56A3.Text = GlobalVariables.E56Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_3.getApproxProdTimeE56(GlobalVariables.E56Produktionsauftrag).ToString();
            E31A3.Text = GlobalVariables.E31Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_3.getApproxProdTimeE31(GlobalVariables.E31Produktionsauftrag).ToString();

            P1A4.Text = GlobalVariables.P1Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_4.getApproxProdTimeP1(GlobalVariables.P1Produktionsauftrag).ToString();
            P2A4.Text = GlobalVariables.P2Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_4.getApproxProdTimeP2(GlobalVariables.P2Produktionsauftrag).ToString();
            P3A4.Text = GlobalVariables.P3Produktionsauftrag < 0 ? "0" : GlobalVariables.factory.Wp_4.getApproxProdTimeP3(GlobalVariables.P3Produktionsauftrag).ToString();
        }

        public void refreshAmountKapaPlan()
        {
            AmE4.Text = GlobalVariables.E4Produktionsauftrag < 0 ? "0" : GlobalVariables.E4Produktionsauftrag.ToString();
            AmE5.Text = GlobalVariables.E5Produktionsauftrag < 0 ? "0" : GlobalVariables.E5Produktionsauftrag.ToString();
            AmE6.Text = GlobalVariables.E6Produktionsauftrag < 0 ? "0" : GlobalVariables.E6Produktionsauftrag.ToString();
            AmE7.Text = GlobalVariables.E7Produktionsauftrag < 0 ? "0" : GlobalVariables.E7Produktionsauftrag.ToString();
            AmE8.Text = GlobalVariables.E8Produktionsauftrag < 0 ? "0" : GlobalVariables.E8Produktionsauftrag.ToString();
            AmE9.Text = GlobalVariables.E9Produktionsauftrag < 0 ? "0" : GlobalVariables.E9Produktionsauftrag.ToString();
            AmE10.Text = GlobalVariables.E10Produktionsauftrag < 0 ? "0" : GlobalVariables.E10Produktionsauftrag.ToString();
            AmE11.Text = GlobalVariables.E11Produktionsauftrag < 0 ? "0" : GlobalVariables.E11Produktionsauftrag.ToString();
            AmE12.Text = GlobalVariables.E12Produktionsauftrag < 0 ? "0" : GlobalVariables.E12Produktionsauftrag.ToString();
            AmE13.Text = GlobalVariables.E13Produktionsauftrag < 0 ? "0" : GlobalVariables.E13Produktionsauftrag.ToString();
            AmE14.Text = GlobalVariables.E14Produktionsauftrag < 0 ? "0" : GlobalVariables.E14Produktionsauftrag.ToString();
            AmE15.Text = GlobalVariables.E15Produktionsauftrag < 0 ? "0" : GlobalVariables.E15Produktionsauftrag.ToString();
            AmE16.Text = GlobalVariables.E16Produktionsauftrag < 0 ? "0" : GlobalVariables.E16Produktionsauftrag.ToString();
            AmE17.Text = GlobalVariables.E17Produktionsauftrag < 0 ? "0" : GlobalVariables.E17Produktionsauftrag.ToString();
            AmE18.Text = GlobalVariables.E18Produktionsauftrag < 0 ? "0" : GlobalVariables.E18Produktionsauftrag.ToString();
            AmE19.Text = GlobalVariables.E19Produktionsauftrag < 0 ? "0" : GlobalVariables.E19Produktionsauftrag.ToString();
            AmE20.Text = GlobalVariables.E20Produktionsauftrag < 0 ? "0" : GlobalVariables.E20Produktionsauftrag.ToString();

            AmE26.Text = GlobalVariables.E26Produktionsauftrag < 0 ? "0" : GlobalVariables.E26Produktionsauftrag.ToString();

            AmE49.Text = GlobalVariables.E49Produktionsauftrag < 0 ? "0" : GlobalVariables.E49Produktionsauftrag.ToString();
            AmE54.Text = GlobalVariables.E54Produktionsauftrag < 0 ? "0" : GlobalVariables.E54Produktionsauftrag.ToString();
            AmE29.Text = GlobalVariables.E29Produktionsauftrag < 0 ? "0" : GlobalVariables.E29Produktionsauftrag.ToString();

            AmE50.Text = GlobalVariables.E50Produktionsauftrag < 0 ? "0" : GlobalVariables.E50Produktionsauftrag.ToString();
            AmE55.Text = GlobalVariables.E55Produktionsauftrag < 0 ? "0" : GlobalVariables.E55Produktionsauftrag.ToString();
            AmE30.Text = GlobalVariables.E30Produktionsauftrag < 0 ? "0" : GlobalVariables.E30Produktionsauftrag.ToString();

            AmE51.Text = GlobalVariables.E51Produktionsauftrag < 0 ? "0" : GlobalVariables.E51Produktionsauftrag.ToString();
            AmE56.Text = GlobalVariables.E56Produktionsauftrag < 0 ? "0" : GlobalVariables.E56Produktionsauftrag.ToString();
            AmE31.Text = GlobalVariables.E31Produktionsauftrag < 0 ? "0" : GlobalVariables.E31Produktionsauftrag.ToString();

            AmP1.Text = GlobalVariables.P1Produktionsauftrag < 0 ? "0" : GlobalVariables.P1Produktionsauftrag.ToString();
            AmP2.Text = GlobalVariables.P2Produktionsauftrag < 0 ? "0" : GlobalVariables.P2Produktionsauftrag.ToString();
            AmP3.Text = GlobalVariables.P3Produktionsauftrag < 0 ? "0" : GlobalVariables.P3Produktionsauftrag.ToString();
        }

        public void calculateCapNeed()
        {
            calculateCapNeedA1();
            calculateCapNeedA2();
            calculateCapNeedA3();
            calculateCapNeedA4();
            calculateCapNeedA6();
            calculateCapNeedA7();
            calculateCapNeedA8();
            calculateCapNeedA9();
            calculateCapNeedA10();
            calculateCapNeedA11();
            calculateCapNeedA12();
            calculateCapNeedA13();
            calculateCapNeedA14();
            calculateCapNeedA15();
        }

        public void calculateSetUpTime()
        {
            setUpTimeA1.Text = (Math.Round(GlobalVariables.A1SetUpsLastPeriod * 1.15 / 10, MidpointRounding.AwayFromZero) * 10).ToString();
            setUpTimeA2.Text = (Math.Round(GlobalVariables.A2SetUpsLastPeriod * 1.15 / 10, MidpointRounding.AwayFromZero) * 10).ToString();
            setUpTimeA3.Text = (Math.Round(GlobalVariables.A3SetUpsLastPeriod * 1.15 / 10, MidpointRounding.AwayFromZero) * 10).ToString();
            setUpTimeA4.Text = (Math.Round(GlobalVariables.A4SetUpsLastPeriod * 1.15 / 10, MidpointRounding.AwayFromZero) * 10).ToString();
            setUpTimeA6.Text = (Math.Round(GlobalVariables.A6SetUpsLastPeriod * 1.15 / 10, MidpointRounding.AwayFromZero) * 10).ToString();
            setUpTimeA7.Text = (Math.Round(GlobalVariables.A7SetUpsLastPeriod * 1.15 / 10, MidpointRounding.AwayFromZero) * 10).ToString();
            setUpTimeA8.Text = (Math.Round(GlobalVariables.A8SetUpsLastPeriod * 1.15 / 10, MidpointRounding.AwayFromZero) * 10).ToString();
            setUpTimeA9.Text = (Math.Round(GlobalVariables.A9SetUpsLastPeriod * 1.15 / 10, MidpointRounding.AwayFromZero) * 10).ToString();
            setUpTimeA10.Text = (Math.Round(GlobalVariables.A10SetUpsLastPeriod * 1.15 / 10, MidpointRounding.AwayFromZero) * 10).ToString();
            setUpTimeA11.Text = (Math.Round(GlobalVariables.A11SetUpsLastPeriod * 1.15 / 10, MidpointRounding.AwayFromZero) * 10).ToString();
            setUpTimeA12.Text = (Math.Round(GlobalVariables.A12SetUpsLastPeriod * 1.15 / 10, MidpointRounding.AwayFromZero) * 10).ToString();
            setUpTimeA13.Text = (Math.Round(GlobalVariables.A13SetUpsLastPeriod * 1.15 / 10, MidpointRounding.AwayFromZero) * 10).ToString();
            setUpTimeA14.Text = (Math.Round(GlobalVariables.A14SetUpsLastPeriod * 1.15 / 10, MidpointRounding.AwayFromZero) * 10).ToString();
            setUpTimeA15.Text = (Math.Round(GlobalVariables.A15SetUpsLastPeriod * 1.15 / 10, MidpointRounding.AwayFromZero) * 10).ToString();
        }

        public void calculateOldSetUpTime()
        {
            if (Int32.Parse(kapOldA1.Text) > 0)
                setUpOldTimeA1.Text = (Math.Round((double)Int32.Parse(kapOldA1.Text) / 100) * 20).ToString();
            if (Int32.Parse(kapOldA2.Text) > 0)
                setUpOldTimeA2.Text = (Math.Round((double)Int32.Parse(kapOldA2.Text) / 100) * 30).ToString();
            if (Int32.Parse(kapOldA3.Text) > 0)
                setUpOldTimeA3.Text = (Math.Round((double)Int32.Parse(kapOldA3.Text) / 100) * 20).ToString();
            if (Int32.Parse(kapOldA4.Text) > 0)
                setUpOldTimeA4.Text = (Math.Round((double)Int32.Parse(kapOldA4.Text) / 100) * 30).ToString();
            if (Int32.Parse(kapOldA6.Text) > 0)
                setUpOldTimeA6.Text = (Math.Round((double)Int32.Parse(kapOldA6.Text) / 100) * 15).ToString();
            if (Int32.Parse(kapOldA7.Text) > 0)
                setUpOldTimeA7.Text = (Math.Round((double)Int32.Parse(kapOldA7.Text) / 100) * 20).ToString();
            if (Int32.Parse(kapOldA8.Text) > 0)
                setUpOldTimeA8.Text = (Math.Round((double)Int32.Parse(kapOldA8.Text) / 100) * 20).ToString();
            if (Int32.Parse(kapOldA9.Text) > 0)
                setUpOldTimeA9.Text = (Math.Round((double)Int32.Parse(kapOldA9.Text) / 100) * 15).ToString();
            if (Int32.Parse(kapOldA10.Text) > 0)
                setUpOldTimeA10.Text = (Math.Round((double)Int32.Parse(kapOldA10.Text) / 100) * 20).ToString();
            if (Int32.Parse(kapOldA11.Text) > 0)
                setUpOldTimeA11.Text = (Math.Round((double)Int32.Parse(kapOldA11.Text) / 100) * 15).ToString();
            if (Int32.Parse(kapOldA14.Text) > 0)
                setUpOldTimeA15.Text = (Math.Round((double)Int32.Parse(kapOldA15.Text) / 100) * 15).ToString();
        }

        public void calculateShiftAndOverDo()
        {
            SuOTimeA1.Text = calcShift(Int32.Parse(wholeKapA1.Text));
            SuOTimeA2.Text = calcShift(Int32.Parse(wholeKapA2.Text));
            SuOTimeA3.Text = calcShift(Int32.Parse(wholeKapA3.Text));
            SuOTimeA4.Text = calcShift(Int32.Parse(wholeKapA4.Text));
            SuOTimeA6.Text = calcShift(Int32.Parse(wholeKapA6.Text));
            SuOTimeA7.Text = calcShift(Int32.Parse(wholeKapA7.Text));
            SuOTimeA8.Text = calcShift(Int32.Parse(wholeKapA8.Text));
            SuOTimeA9.Text = calcShift(Int32.Parse(wholeKapA9.Text));
            SuOTimeA10.Text = calcShift(Int32.Parse(wholeKapA10.Text));
            SuOTimeA11.Text = calcShift(Int32.Parse(wholeKapA11.Text));
            SuOTimeA12.Text = calcShift(Int32.Parse(wholeKapA12.Text));
            SuOTimeA13.Text = calcShift(Int32.Parse(wholeKapA13.Text));
            SuOTimeA14.Text = calcShift(Int32.Parse(wholeKapA14.Text));
            SuOTimeA15.Text = calcShift(Int32.Parse(wholeKapA15.Text));

            OTimeA1.Text = calcOverDo(Int32.Parse(wholeKapA1.Text));
            OTimeA2.Text = calcOverDo(Int32.Parse(wholeKapA2.Text));
            OTimeA3.Text = calcOverDo(Int32.Parse(wholeKapA3.Text));
            OTimeA4.Text = calcOverDo(Int32.Parse(wholeKapA4.Text));
            OTimeA6.Text = calcOverDo(Int32.Parse(wholeKapA6.Text));
            OTimeA7.Text = calcOverDo(Int32.Parse(wholeKapA7.Text));
            OTimeA8.Text = calcOverDo(Int32.Parse(wholeKapA8.Text));
            OTimeA9.Text = calcOverDo(Int32.Parse(wholeKapA9.Text));
            OTimeA10.Text = calcOverDo(Int32.Parse(wholeKapA10.Text));
            OTimeA11.Text = calcOverDo(Int32.Parse(wholeKapA11.Text));
            OTimeA12.Text = calcOverDo(Int32.Parse(wholeKapA12.Text));
            OTimeA13.Text = calcOverDo(Int32.Parse(wholeKapA13.Text));
            OTimeA14.Text = calcOverDo(Int32.Parse(wholeKapA14.Text));
            OTimeA15.Text = calcOverDo(Int32.Parse(wholeKapA15.Text));


            GlobalVariables.Kapaza.Rows.Clear();
            double overtime = Math.Ceiling((double)Int32.Parse(OTimeA1.Text) / 50) * 10;
            GlobalVariables.Kapaza.Rows.Add(1, Int32.Parse(SuOTimeA1.Text), overtime > 240 ? 240 : overtime);
            overtime = Math.Ceiling((double)Int32.Parse(OTimeA2.Text) / 50) * 10;
            GlobalVariables.Kapaza.Rows.Add(2, Int32.Parse(SuOTimeA2.Text), overtime > 240 ? 240 : overtime);
            overtime = Math.Ceiling((double)Int32.Parse(OTimeA3.Text) / 50) * 10;
            GlobalVariables.Kapaza.Rows.Add(3, Int32.Parse(SuOTimeA3.Text), overtime > 240 ? 240 : overtime);
            overtime = Math.Ceiling((double)Int32.Parse(OTimeA4.Text) / 50) * 10;
            GlobalVariables.Kapaza.Rows.Add(4, Int32.Parse(SuOTimeA4.Text), overtime > 240 ? 240 : overtime);
            overtime = Math.Ceiling((double)Int32.Parse(OTimeA6.Text) / 50) * 10;
            GlobalVariables.Kapaza.Rows.Add(6, Int32.Parse(SuOTimeA6.Text), overtime > 240 ? 240 : overtime);
            overtime = Math.Ceiling((double)Int32.Parse(OTimeA7.Text) / 50) * 10;
            GlobalVariables.Kapaza.Rows.Add(7, Int32.Parse(SuOTimeA7.Text), overtime > 240 ? 240 : overtime);
            overtime = Math.Ceiling((double)Int32.Parse(OTimeA8.Text) / 50) * 10;
            GlobalVariables.Kapaza.Rows.Add(8, Int32.Parse(SuOTimeA8.Text), overtime > 240 ? 240 : overtime);
            overtime = Math.Ceiling((double)Int32.Parse(OTimeA9.Text) / 50) * 10;
            GlobalVariables.Kapaza.Rows.Add(9, Int32.Parse(SuOTimeA9.Text), overtime > 240 ? 240 : overtime);
            overtime = Math.Ceiling((double)Int32.Parse(OTimeA10.Text) / 50) * 10;
            GlobalVariables.Kapaza.Rows.Add(10, Int32.Parse(SuOTimeA10.Text), overtime > 240 ? 240 : overtime);
            overtime = Math.Ceiling((double)Int32.Parse(OTimeA11.Text) / 50) * 10;
            GlobalVariables.Kapaza.Rows.Add(11, Int32.Parse(SuOTimeA11.Text), overtime > 240 ? 240 : overtime);
            overtime = Math.Ceiling((double)Int32.Parse(OTimeA12.Text) / 50) * 10;
            GlobalVariables.Kapaza.Rows.Add(12, Int32.Parse(SuOTimeA12.Text), overtime > 240 ? 240 : overtime);
            overtime = Math.Ceiling((double)Int32.Parse(OTimeA13.Text) / 50) * 10;
            GlobalVariables.Kapaza.Rows.Add(13, Int32.Parse(SuOTimeA13.Text), overtime > 240 ? 240 : overtime);
            overtime = Math.Ceiling((double)Int32.Parse(OTimeA14.Text) / 50) * 10;
            GlobalVariables.Kapaza.Rows.Add(14, Int32.Parse(SuOTimeA14.Text), overtime > 240 ? 240 : overtime);
            overtime = Math.Ceiling((double)Int32.Parse(OTimeA15.Text) / 50) * 10;
            GlobalVariables.Kapaza.Rows.Add(15, Int32.Parse(SuOTimeA15.Text), overtime > 240 ? 240 : overtime);
        }

        public void calcWholeCap()
        {
            GlobalVariables.blockingErrorKapaPlan = false;
            int sum = Int32.Parse(kapA1.Text) + Int32.Parse(setUpTimeA1.Text) + Int32.Parse(kapOldA1.Text) + Int32.Parse(setUpOldTimeA1.Text);
            if (sum > 7200)
            {
                MessageBox.Show("Workplace 1 has an overload in worktime (" + Math.Abs(7200 - sum) + " minutes), please reduce the specific production");
                GlobalVariables.blockingErrorKapaPlan = true;
                wholeKapA1.Text = (7200 - sum).ToString();
            } else if (sum < 1200)
            {
                MessageBox.Show("WARNING | Workplace 1 has a low workload (" + sum + " (" + (int)(((double)sum /2400)*100) + "%)) --> Recommendation to improve it!");
            }
            else
                wholeKapA1.Text = sum.ToString();

            sum = Int32.Parse(kapA2.Text) + Int32.Parse(setUpTimeA2.Text) + Int32.Parse(kapOldA2.Text) + Int32.Parse(setUpOldTimeA2.Text);
            if (sum > 7200)
            {
                MessageBox.Show("Workplace 2 has an overload in worktime (" + Math.Abs(7200 - sum) + " minutes), please reduce the specific production");
                GlobalVariables.blockingErrorKapaPlan = true;
                wholeKapA2.Text = (7200 - sum).ToString();
            }
            else if (sum < 1200)
            {
                MessageBox.Show("WARNING | Workplace 2 has a low workload (" + sum + " (" + (int)(((double)sum /2400)*100) + "%)) --> Recommendation to improve it!");
            }
            else
                wholeKapA2.Text = sum.ToString();

            sum = Int32.Parse(kapA3.Text) + Int32.Parse(setUpTimeA3.Text) + Int32.Parse(kapOldA3.Text) + Int32.Parse(setUpOldTimeA3.Text);
            if (sum > 7200)
            {
                MessageBox.Show("Workplace 3 has an overload in worktime (" + Math.Abs(7200 - sum) + " minutes), please reduce the specific production");
                GlobalVariables.blockingErrorKapaPlan = true;
                wholeKapA3.Text = (7200 - sum).ToString();
            }
            else if (sum < 1200)
            {
                MessageBox.Show("WARNING | Workplace 3 has a low workload (" + sum + " (" + (int)(((double)sum /2400)*100) + "%)) --> Recommendation to improve it!");
            }
            else
                wholeKapA3.Text = sum.ToString();

            sum = Int32.Parse(kapA4.Text) + Int32.Parse(setUpTimeA4.Text) + Int32.Parse(kapOldA4.Text) + Int32.Parse(setUpOldTimeA4.Text);
            if (sum > 7200)
            {
                MessageBox.Show("Workplace 4 has an overload in worktime (" + Math.Abs(7200 - sum) + " minutes), please reduce the specific production");
                GlobalVariables.blockingErrorKapaPlan = true;
                wholeKapA4.Text = (7200 - sum).ToString();
            }
            else if (sum < 1200)
            {
                MessageBox.Show("WARNING | Workplace 4 has a low workload (" + sum + " (" + (int)(((double)sum /2400)*100) + "%)) --> Recommendation to improve it!");
            }
            else
                wholeKapA4.Text = sum.ToString();

            sum = Int32.Parse(kapA6.Text) + Int32.Parse(setUpTimeA6.Text) + Int32.Parse(kapOldA6.Text) + Int32.Parse(setUpOldTimeA6.Text);
            if (sum > 7200)
            {
                MessageBox.Show("Workplace 6 has an overload in worktime (" + Math.Abs(7200 - sum) + " minutes), please reduce the specific production");
                GlobalVariables.blockingErrorKapaPlan = true;
                wholeKapA6.Text = (7200 - sum).ToString();
            }
            else if (sum < 1200)
            {
                MessageBox.Show("WARNING | Workplace 6 has a low workload (" + sum + " (" + (int)(((double)sum /2400)*100) + "%)) --> Recommendation to improve it!");
            }
            else
                wholeKapA6.Text = sum.ToString();

            sum = Int32.Parse(kapA7.Text) + Int32.Parse(setUpTimeA7.Text) + Int32.Parse(kapOldA7.Text) + Int32.Parse(setUpOldTimeA7.Text);
            if (sum > 7200)
            {
                MessageBox.Show("Workplace 7 has an overload in worktime (" + Math.Abs(7200 - sum) + " minutes), please reduce the specific production");
                GlobalVariables.blockingErrorKapaPlan = true;
                wholeKapA7.Text = (7200 - sum).ToString();
            }
            else if (sum < 1200)
            {
                MessageBox.Show("WARNING | Workplace 7 has a low workload (" + sum + " (" + (int)(((double)sum /2400)*100) + "%)) --> Recommendation to improve it!");
            }
            else
                wholeKapA7.Text = sum.ToString();

            sum = Int32.Parse(kapA8.Text) + Int32.Parse(setUpTimeA8.Text) + Int32.Parse(kapOldA8.Text) + Int32.Parse(setUpOldTimeA8.Text);
            if (sum > 7200)
            {
                MessageBox.Show("Workplace 8 has an overload in worktime (" + Math.Abs(7200 - sum) + " minutes), please reduce the specific production");
                GlobalVariables.blockingErrorKapaPlan = true;
                wholeKapA8.Text = (7200 - sum).ToString();
            }
            else if (sum < 1200)
            {
                MessageBox.Show("WARNING | Workplace 8 has a low workload (" + sum + " (" + (int)(((double)sum /2400)*100) + "%)) --> Recommendation to improve it!");
            }
            else
                wholeKapA8.Text = sum.ToString();

            sum = Int32.Parse(kapA9.Text) + Int32.Parse(setUpTimeA9.Text) + Int32.Parse(kapOldA9.Text) + Int32.Parse(setUpOldTimeA9.Text);
            if (sum > 7200)
            {
                MessageBox.Show("Workplace 9 has an overload in worktime (" + Math.Abs(7200 - sum) + " minutes), please reduce the specific production");
                GlobalVariables.blockingErrorKapaPlan = true;
                wholeKapA9.Text = (7200 - sum).ToString();
            }
            else if (sum < 1200)
            {
                MessageBox.Show("WARNING | Workplace 9 has a low workload (" + sum + " (" + (int)(((double)sum /2400)*100) + "%)) --> Recommendation to improve it!");
            }
            else
                wholeKapA9.Text = sum.ToString();

            sum = Int32.Parse(kapA10.Text) + Int32.Parse(setUpTimeA10.Text) + Int32.Parse(kapOldA10.Text) + Int32.Parse(setUpOldTimeA10.Text);
            if (sum > 7200)
            {
                MessageBox.Show("Workplace 10 has an overload in worktime (" + Math.Abs(7200 - sum) + " minutes), please reduce the specific production");
                GlobalVariables.blockingErrorKapaPlan = true;
                wholeKapA10.Text = (7200 - sum).ToString();
            }
            else if (sum < 1200)
            {
                MessageBox.Show("WARNING | Workplace 10 has a low workload (" + sum + " (" + (int)(((double)sum /2400)*100) + "%)) --> Recommendation to improve it!");
            }
            else
                wholeKapA10.Text = sum.ToString();

            sum = Int32.Parse(kapA11.Text) + Int32.Parse(setUpTimeA11.Text) + Int32.Parse(kapOldA11.Text) + Int32.Parse(setUpOldTimeA11.Text);
            if (sum > 7200)
            {
                MessageBox.Show("Workplace 11 has an overload in worktime (" + Math.Abs(7200 - sum) + " minutes), please reduce the specific production");
                GlobalVariables.blockingErrorKapaPlan = true;
                wholeKapA11.Text = (7200 - sum).ToString();
            }
            else if (sum < 1200)
            {
                MessageBox.Show("WARNING | Workplace 11 has a low workload (" + sum + " (" + (int)(((double)sum /2400)*100) + "%)) --> Recommendation to improve it!");
            }
            else
                wholeKapA11.Text = sum.ToString();

            sum = Int32.Parse(kapA12.Text) + Int32.Parse(setUpTimeA12.Text) + Int32.Parse(kapOldA12.Text) + Int32.Parse(setUpOldTimeA12.Text);
            if (sum > 7200)
            {
                MessageBox.Show("Workplace 12 has an overload in worktime (" + Math.Abs(7200 - sum) + " minutes), please reduce the specific production");
                GlobalVariables.blockingErrorKapaPlan = true;
                wholeKapA12.Text = (7200 - sum).ToString();
            }
            else if (sum < 1200)
            {
                MessageBox.Show("WARNING | Workplace 12 has a low workload (" + sum + " (" + (int)(((double)sum /2400)*100) + "%)) --> Recommendation to improve it!");
            }
            else
                wholeKapA12.Text = sum.ToString();

            sum = Int32.Parse(kapA13.Text) + Int32.Parse(setUpTimeA13.Text) + Int32.Parse(kapOldA13.Text) + Int32.Parse(setUpOldTimeA13.Text);
            if (sum > 7200)
            {
                MessageBox.Show("Workplace 13 has an overload in worktime (" + Math.Abs(7200 - sum) + " minutes), please reduce the specific production");
                GlobalVariables.blockingErrorKapaPlan = true;
                wholeKapA13.Text = (7200 - sum).ToString();
            }
            else if (sum < 1200)
            {
                MessageBox.Show("WARNING | Workplace 13 has a low workload (" + sum + " (" + (int)(((double)sum /2400)*100) + "%)) --> Recommendation to improve it!");
            }
            else
                wholeKapA13.Text = sum.ToString();

            sum = Int32.Parse(kapA14.Text) + Int32.Parse(setUpTimeA14.Text) + Int32.Parse(kapOldA14.Text) + Int32.Parse(setUpOldTimeA14.Text);
            if (sum > 7200)
            {
                MessageBox.Show("Workplace 14 has an overload in worktime (" + Math.Abs(7200 - sum) + " minutes), please reduce the specific production");
                GlobalVariables.blockingErrorKapaPlan = true;
                wholeKapA14.Text = (7200 - sum).ToString();
            }
            else if (sum < 1200)
            {
                MessageBox.Show("WARNING | Workplace 14 has a low workload (" + sum + " (" + (int)(((double)sum /2400)*100) + "%)) --> Recommendation to improve it!");
            }
            else
                wholeKapA14.Text = sum.ToString();

            sum = Int32.Parse(kapA15.Text) + Int32.Parse(setUpTimeA15.Text) + Int32.Parse(kapOldA15.Text) + Int32.Parse(setUpOldTimeA15.Text);
            if (sum > 7200)
            {
                MessageBox.Show("Workplace 15 has an overload in worktime (" + Math.Abs(7200- sum) + " minutes), please reduce the specific production");
                GlobalVariables.blockingErrorKapaPlan = true;
                wholeKapA15.Text = (7200 - sum).ToString();
            }
            else if (sum < 1200)
            {
                MessageBox.Show("WARNING | Workplace 15 has a low workload (" + sum + " (" + (int)(((double)sum /2400)*100) + "%)) --> Recommendation to improve it!");
            }
            else
                wholeKapA15.Text = sum.ToString();
        }

        public void calcWorkLoad()
        {
            LabelWP1.ToolTip = specWorkLoad(1, Int32.Parse(wholeKapA1.Text));
            LabelWP2.ToolTip = specWorkLoad(2, Int32.Parse(wholeKapA2.Text));
            LabelWP3.ToolTip = specWorkLoad(3, Int32.Parse(wholeKapA3.Text));
            LabelWP4.ToolTip = specWorkLoad(4, Int32.Parse(wholeKapA4.Text));
            LabelWP6.ToolTip = specWorkLoad(6, Int32.Parse(wholeKapA6.Text));
            LabelWP7.ToolTip = specWorkLoad(7, Int32.Parse(wholeKapA7.Text));
            LabelWP8.ToolTip = specWorkLoad(8, Int32.Parse(wholeKapA8.Text));
            LabelWP9.ToolTip = specWorkLoad(9, Int32.Parse(wholeKapA9.Text));
            LabelWP10.ToolTip = specWorkLoad(10, Int32.Parse(wholeKapA10.Text));
            LabelWP11.ToolTip = specWorkLoad(11, Int32.Parse(wholeKapA11.Text));
            LabelWP12.ToolTip = specWorkLoad(12, Int32.Parse(wholeKapA12.Text));
            LabelWP13.ToolTip = specWorkLoad(13, Int32.Parse(wholeKapA13.Text));
            LabelWP14.ToolTip = specWorkLoad(14, Int32.Parse(wholeKapA14.Text));
            LabelWP15.ToolTip = specWorkLoad(15, Int32.Parse(wholeKapA15.Text));
        }

        public string specWorkLoad(int wp, int wholeCap)
        {
            if (wholeCap > 7200)
                return "";
            else if (wholeCap <= 7200 && wholeCap > 6000)
                return "Workload " + (int)(Math.Round(((double)wholeCap / 7200) * 100)) + "%";
            else if(wholeCap <= 6000 && wholeCap > 4800)
                return "Workload " + (int)(Math.Round(((double)wholeCap / 6000) * 100)) + "%";
            else if (wholeCap <= 4800 && wholeCap > 3600)
                return "Workload " + (int)(Math.Round(((double)wholeCap / 4800) * 100)) + "%";
            else if (wholeCap <= 3600 && wholeCap > 2400)
                return "Workload " + (int)(Math.Round(((double)wholeCap / 3600) * 100)) + "%";
            else if (wholeCap <= 2400 && wholeCap >= 0)
                return "Workload " + (int)(Math.Round(((double)wholeCap / 2400) * 100)) + "%";

            return "";
        }

        public string calcShift(int sum)
        {
            int shift = 0;
            if (sum > 7200 || sum < 0)
            {
                shift = -1;
            }
            else if (sum > 6000)
            {
                shift = 3;
            }
            else if (sum > 3600 && sum <= 6000)
            {
                shift = 2;
            }
            else
                shift = 1;

            return shift.ToString();
        }

        public string calcOverDo(int sum)
        {
            int timeToOverDo = 0;
            if (sum > 7200 || sum < 0)
            {
                timeToOverDo = -1;
            }
            if (sum > 4800 && sum <= 6000)
            {
                timeToOverDo = sum - 4800;
            }
            else if(sum > 2400 && sum <= 3600)
            {
                timeToOverDo = sum - 2400;
            }

            return timeToOverDo.ToString();
        }

        public void calculateCapNeedA1()
        {
            kapA1.Text = (Int32.Parse(E49A1.Text) + Int32.Parse(E54A1.Text) + Int32.Parse(E29A1.Text)).ToString();
        }

        public void calculateCapNeedA2()
        {
            kapA2.Text = (Int32.Parse(E50A2.Text) + Int32.Parse(E55A2.Text) + Int32.Parse(E30A2.Text)).ToString();
        }

        public void calculateCapNeedA3()
        {
            kapA3.Text = (Int32.Parse(E51A3.Text) + Int32.Parse(E56A3.Text) + Int32.Parse(E31A3.Text)).ToString();
        }

        public void calculateCapNeedA4()
        {
            kapA4.Text = (Int32.Parse(P1A4.Text) + Int32.Parse(P2A4.Text) + Int32.Parse(P3A4.Text)).ToString();
        }

        public void calculateCapNeedA6()
        {
            kapA6.Text = (Int32.Parse(E16A6.Text) 
                + Int32.Parse(E18A6.Text) 
                + Int32.Parse(E19A6.Text)
                + Int32.Parse(E20A6.Text)).ToString();
        }

        public void calculateCapNeedA7()
        {
            kapA7.Text = (Int32.Parse(E10A7.Text)
                + Int32.Parse(E11A7.Text)
                + Int32.Parse(E12A7.Text)
                + Int32.Parse(E13A7.Text)
                + Int32.Parse(E14A7.Text)
                + Int32.Parse(E15A7.Text)
                + Int32.Parse(E18A7.Text)
                + Int32.Parse(E19A7.Text)
                + Int32.Parse(E20A7.Text)
                + Int32.Parse(E26A7.Text)).ToString();
        }

        public void calculateCapNeedA8()
        {
            kapA8.Text = (Int32.Parse(E10A8.Text)
                + Int32.Parse(E11A8.Text)
                + Int32.Parse(E12A8.Text)
                + Int32.Parse(E13A8.Text)
                + Int32.Parse(E14A8.Text)
                + Int32.Parse(E15A8.Text)
                + Int32.Parse(E18A8.Text)
                + Int32.Parse(E19A8.Text)
                + Int32.Parse(E20A8.Text)).ToString();
        }

        public void calculateCapNeedA9()
        {
            kapA9.Text = (Int32.Parse(E10A9.Text)
                + Int32.Parse(E11A9.Text)
                + Int32.Parse(E12A9.Text)
                + Int32.Parse(E13A9.Text)
                + Int32.Parse(E14A9.Text)
                + Int32.Parse(E15A9.Text)
                + Int32.Parse(E18A9.Text)
                + Int32.Parse(E19A9.Text)
                + Int32.Parse(E20A9.Text)).ToString();
        }

        public void calculateCapNeedA10()
        {
            kapA10.Text = (Int32.Parse(E4A10.Text)
                + Int32.Parse(E5A10.Text)
                + Int32.Parse(E6A10.Text)
                + Int32.Parse(E7A10.Text)
                + Int32.Parse(E8A10.Text)
                + Int32.Parse(E9A10.Text)).ToString();
        }

        public void calculateCapNeedA11()
        {
            kapA11.Text = (Int32.Parse(E4A11.Text)
                + Int32.Parse(E5A11.Text)
                + Int32.Parse(E6A11.Text)
                + Int32.Parse(E7A11.Text)
                + Int32.Parse(E8A11.Text)
                + Int32.Parse(E9A11.Text)).ToString();
        }

        public void calculateCapNeedA12()
        {
            kapA12.Text = (Int32.Parse(E10A12.Text)
                + Int32.Parse(E11A12.Text)
                + Int32.Parse(E12A12.Text)
                + Int32.Parse(E13A12.Text)
                + Int32.Parse(E14A12.Text)
                + Int32.Parse(E15A12.Text)).ToString();
        }

        public void calculateCapNeedA13()
        {
            kapA13.Text = (Int32.Parse(E10A13.Text)
                + Int32.Parse(E11A13.Text)
                + Int32.Parse(E12A13.Text)
                + Int32.Parse(E13A13.Text)
                + Int32.Parse(E14A13.Text)
                + Int32.Parse(E15A13.Text)).ToString();
        }

        public void calculateCapNeedA14()
        {
            kapA14.Text = (Int32.Parse(E16A14.Text)).ToString();
        }

        public void calculateCapNeedA15()
        {
            kapA15.Text = (Int32.Parse(E17A15.Text)
                + Int32.Parse(E26A15.Text)).ToString();
        }


        #endregion

        #region Data

        private void Programmplannung(DataSet data)
        {


            DataRow result_0 = GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables["results"].Rows[0];
            GlobalVariables.curPeriod = int.Parse(result_0["period"].ToString())+1;
            GlobalVariables.groupNumber = int.Parse(result_0["period"].ToString());

            capCurPeriod.Content = GlobalVariables.curPeriod.ToString();
            GlobalVariables.capN1Period = GlobalVariables.curPeriod + 1;
            GlobalVariables.capN2Period = GlobalVariables.curPeriod + 2;
            GlobalVariables.capN3Period = GlobalVariables.curPeriod + 3;

            PeriodeN.Content = GlobalVariables.curPeriod.ToString();
            PeriodeN1.Content = GlobalVariables.capN1Period.ToString();
            PeriodeN2.Content = GlobalVariables.capN2Period.ToString();
            PeriodeN3.Content = GlobalVariables.capN3Period.ToString();

            Bestellung.Columns[2].Header = GlobalVariables.curPeriod;
            Bestellung.Columns[3].Header = GlobalVariables.capN1Period;
            Bestellung.Columns[4].Header = GlobalVariables.capN2Period;
            Bestellung.Columns[5].Header = GlobalVariables.capN3Period;
            Bestellung.Columns[6].Header = GlobalVariables.curPeriod;
            Bestellung.Columns[7].Header = GlobalVariables.capN1Period;
            Bestellung.Columns[8].Header = GlobalVariables.capN2Period;
            Bestellung.Columns[9].Header = GlobalVariables.capN3Period;
            //capCurPeriod.Content = (string)(App.Current.Resources["PeriodeNLabel"]);
            //Application.Current.Resources["PeriodeNLabel"] = "PETER";
            //groupNumber.Content = "Gruppe " + groupNumber;

            //GeplanterVerkauf
            ChildBikeOrderP1.Text = GlobalVariables.SaleChildBikeN.ToString();
            FemaleBikeOrderP2.Text = GlobalVariables.SaleFemaleBikeN.ToString();
            MaleBikeOrderP3.Text = GlobalVariables.SaleMaleBikeN.ToString();

            DataTable Prognose = new DataTable();
            Prognose.Clear();
            if (!Prognose.Columns.Contains("article"))
            {
                Prognose.Columns.Add("article", typeof(int));
                Prognose.Columns.Add("quantity", typeof(int));
            }

            Prognose.Rows.Add(1, GlobalVariables.SaleChildBikeN);
            Prognose.Rows.Add(2, GlobalVariables.SaleFemaleBikeN);
            Prognose.Rows.Add(3, GlobalVariables.SaleMaleBikeN);

            Sellwish.DataContext = Prognose;
            Sellwish.ItemsSource = Prognose.DefaultView;

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
            while (E16 > 0)
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
                if (results != null && results.Length > 0)
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
                FemaleBikeInProductionP2.Text = ((results.Length) * 10).ToString();
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
            if (!GlobalVariables.dtProdOrder.Columns.Contains("Item"))
                GlobalVariables.dtProdOrder.Columns.Add("Item");
            if (!GlobalVariables.dtProdOrder.Columns.Contains("Amount"))
                GlobalVariables.dtProdOrder.Columns.Add("Amount");

            //Enter all Rows
            DataRow P1P = GlobalVariables.dtProdOrder.NewRow();
            P1P["Item"] = "1";
            if (int.Parse(ChildBikePlannedProductionP1.Text) > 0)
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

            List<DataRow> P1P2P3 = new List<DataRow> { P1P, P2P, P3P };
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
            GlobalVariables.AlleArbeitsplätze.Add(1, A1);
            GlobalVariables.AlleArbeitsplätze.Add(2, A2);
            GlobalVariables.AlleArbeitsplätze.Add(3, A3);
            GlobalVariables.AlleArbeitsplätze.Add(4, A4);
            GlobalVariables.AlleArbeitsplätze.Add(6, A6);
            GlobalVariables.AlleArbeitsplätze.Add(7, A7);
            GlobalVariables.AlleArbeitsplätze.Add(8, A8);
            GlobalVariables.AlleArbeitsplätze.Add(9, A9);
            GlobalVariables.AlleArbeitsplätze.Add(10, A10);
            GlobalVariables.AlleArbeitsplätze.Add(11, A11);
            GlobalVariables.AlleArbeitsplätze.Add(12, A12);
            GlobalVariables.AlleArbeitsplätze.Add(13, A13);
            GlobalVariables.AlleArbeitsplätze.Add(14, A14);
            GlobalVariables.AlleArbeitsplätze.Add(15, A15);
            Console.WriteLine("Test1");
            //2. Arbeitsstationen initialisieren und zu Kette machen
            //Arbeitsstationen P123
            //E26
            Dictionary<int, int> TeileE26_1 = new Dictionary<int, int>();
            TeileE26_1.Add(44, 2);
            TeileE26_1.Add(48, 2);
            ArbeitsstationPrototyp E261 = new ArbeitsstationPrototyp("Teil26Station1Arbeitsplatz7", A7, 30, 2, TeileE26_1, "Init", 26);
            Dictionary<int, int> TeileE26_2 = new Dictionary<int, int>();
            TeileE26_2.Add(47, 1);
            ArbeitsstationPrototyp E262 = new ArbeitsstationPrototyp("Teil26Station2Arbeitsplatz15", A15, 15, 3, TeileE26_2, "Init", 26);
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
            ArbeitsstationPrototyp E131 = new ArbeitsstationPrototyp("Teil13Station1Arbeitsplatz13", A13, 0, 2, TeileE13_1, "Init", 13);
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
            Console.WriteLine("Test2");
            //P123
            List<ArbeitsstationPrototyp> KetteE26 = new List<ArbeitsstationPrototyp> { E261, E262 };
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

            Console.WriteLine("Test3");
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
            Console.WriteLine("Test4");



            //Produktionsauftragsliste erstellen(ohne in Bearbeitung und Warteschlange)


            GlobalVariables.Aussortierung.Clear();
            if (!GlobalVariables.Aussortierung.Columns.Contains("Item"))
                GlobalVariables.Aussortierung.Columns.Add("Item");
            if (!GlobalVariables.Aussortierung.Columns.Contains("Amount"))
                GlobalVariables.Aussortierung.Columns.Add("Amount");

            foreach (DataRow Row in GlobalVariables.dtProdOrder.Rows)
            {
                if(Row[0] != DBNull.Value && int.Parse((string)Row[1]) != 0)
                {
                    GlobalVariables.Aussortierung.Rows.Add(Row[0], Row[1]);
                }
            }


            GridProductionOrders.DataContext = GlobalVariables.Aussortierung;

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
            //alt: Productionlist.DataContext = ProdAufträge.DefaultView;
            Productionlist.DataContext = GridProductionOrders.DataContext;

            #endregion DataTable
            GlobalVariables.ProduktionsAufträgeAktuellePeriode.Clear();
            foreach (DataRow Produktionsauftrag in GlobalVariables.dtProdOrder.Rows)
            {
                int Index;
                if (Produktionsauftrag[0] != null && Produktionsauftrag[1] != null)
                {
                    foreach (TeilPrototyp teil in GlobalVariables.AlleTeile)
                    {
                        if (teil.TeilID == int.Parse(Produktionsauftrag[0].ToString()))
                            GlobalVariables.ProduktionsAufträgeAktuellePeriode.Add(new OrderPrototyp(int.Parse(Produktionsauftrag[0].ToString()), int.Parse(Produktionsauftrag[1].ToString()), teil));//List x,y aus allen Teilen richtiges auswählenTeilPrototyp);
                    }
                }
            }

            foreach (OrderPrototyp O in GlobalVariables.ProduktionsAufträgeAktuellePeriode)
            {
                Console.WriteLine("Artikel: " + O.Artikel + " Menge: " + O.Menge + " ID: " + O.TeilPrototyp.TeilID.ToString());
            }

            Console.WriteLine("Test5");

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
                    int i = ((int)dr.Row["order"]);
                    int i2 = ((int)dr.Row["period"]);
                    Console.WriteLine("Periode: " + i2.ToString() + " Order: " + i.ToString() + " Item: " +
                                      (string)dr.Row["item"]);
                }

                foreach (DataRowView dr in wsReal)
                {
                    foreach (TeilPrototyp teil in GlobalVariables.AlleTeile)
                    {
                        if (teil.TeilID == int.Parse((string)dr["item"]))
                        {
                            //alt GlobalVariables.ProduktionsAufträgeAktuellePeriode.Insert(0, (new OrderPrototyp(int.Parse((string) dr["item"]), int.Parse((string) dr["amount"]), teil)));
                            GlobalVariables.ProduktionsAufträgeAktuellePeriode.Insert(0,
                                new OrderPrototyp(int.Parse((string)dr["item"]), 0, teil));
                            foreach (TeilPrototyp TP in GlobalVariables.AlleTeile)
                            {
                                if (TP.TeilID == int.Parse((string)dr["item"]))
                                {
                                    foreach (ArbeitsstationPrototyp AP in TP.KetteStationen)
                                    {
                                        if (AP.Arbeitsplatz.ID == (int)dr["workplace_id"])
                                        {
                                            AP.Warteschlange = AP.Warteschlange + int.Parse((string)dr["amount"]);
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
                        if (teil.TeilID == int.Parse((string)dr["item"]))
                        {
                            foreach (TeilPrototyp TP in GlobalVariables.AlleTeile)
                            {
                                if (TP.TeilID == int.Parse((string)dr["item"]))
                                {
                                    foreach (ArbeitsstationPrototyp AP in TP.KetteStationen)
                                    {
                                        if (AP.Arbeitsplatz.ID == (int)dr["id"])
                                        {
                                            AP.Warteschlange = AP.Warteschlange + int.Parse((string)dr["amount"]);
                                            AP.Arbeitsplatz.Blockierzeit = int.Parse((string)dr["timeneed"]);
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
                GlobalVariables.Lagerstand.Add(int.Parse((string)Row[0]), int.Parse((string)Row[1]));
            }

            Dictionary<int, int> LagerZuBeginn = ObjectCopier.Clone(GlobalVariables.Lagerstand);



            //SimulationPrototyp2 test2 = new SimulationPrototyp2();

            //test2.SimulationPrototypDurchführung(GlobalVariables.ProduktionsAufträgeAktuellePeriode, GlobalVariables.Lagerstand, 1);

            Kapa.DataContext = GlobalVariables.KPErg;

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
            GlobalVariables.Kapaza = new DataTable();
            GlobalVariables.Kapaza.Clear();



            if (!GlobalVariables.Kapaza.Columns.Contains("station"))
            {
                GlobalVariables.Kapaza.Columns.Add("station", typeof(int));
                GlobalVariables.Kapaza.Columns.Add("shift", typeof(int));
                GlobalVariables.Kapaza.Columns.Add("overtime", typeof(int));
            }



            Workingtimelist.DataContext = GlobalVariables.Kapaza;
            Workingtimelist.ItemsSource = GlobalVariables.Kapaza.DefaultView;
            Workingtimelist1.DataContext = GlobalVariables.Kapaza;
            Workingtimelist1.ItemsSource = GlobalVariables.Kapaza.DefaultView;



            foreach (KeyValuePair<int, int> VARIABLE in GlobalVariables.Lagerstand)
            {
                Console.WriteLine("Lager Danach Artikel: " + VARIABLE.Key + " Menge: " + VARIABLE.Value);
            }


            Console.WriteLine("Test9");

            //foreach ( KeyValuePair<int, Arbeitsplatzprototyp> A in GlobalVariables.OriginalAlleArbeitsplätze)
            //{
            //    Console.WriteLine("Arbeitsplatz " + A.Value.ID.ToString() + " Arbeitszeit: " + A.Value.ArbeitszeitProTagInMinuten.ToString());
            //}

            #region direktverkäufe
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
            #endregion direktverkäufe

            #region bestellungsplannung
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
            if (GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables.Contains("order"))
            {
                foreach (DataRow DR in GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables["order"].Rows)
                {

                    if (GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables["order"].Columns.Contains("futureinwardstockmovement_Id"))
                    {
                        if (DR["futureinwardstockmovement_Id"] != DBNull.Value)
                        {
                            B1 = DR["orderperiod"].ToString();
                            B11 = int.Parse(B1) - AktuellePeriode;
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
            }

            foreach (DataRow DR in AlteBestellungen.Rows)
            {
                Console.WriteLine("Vergangenheit: " + DR[0].ToString() + " Artikel: " + DR[1].ToString() + " Menge: " + DR[2].ToString());
            }




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
            double T21E = 1.8 / 2;
            int BruttoT21P1 = GlobalVariables.SaleChildBikeN.GetValueOrDefault() * 1 + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text));
            int BruttoT21P2 = GlobalVariables.SaleChildBikeN1.GetValueOrDefault() * 1;
            int BruttoT21P3 = GlobalVariables.SaleChildBikeN2.GetValueOrDefault() * 1;
            int BruttoT21P4 = GlobalVariables.SaleChildBikeN3.GetValueOrDefault() * 1;
            int P1Zuwachs = 0;
            int P2Zuwachs = 0;
            int P3Zuwachs = 0;
            int P4Zuwachs = 0;

            if (BruttoT21P1 < 0)
                BruttoT21P1 = 0;
            if (BruttoT21P2 < 0)
                BruttoT21P2 = 0;
            if (BruttoT21P3 < 0)
                BruttoT21P3 = 0;
            if (BruttoT21P4 < 0)
                BruttoT21P4 = 0;


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
                if (T21LZ + T21AB > 2)
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

            if (BruttoT22P1 < 0)
                BruttoT22P1 = 0;
            if (BruttoT22P2 < 0)
                BruttoT22P2 = 0;
            if (BruttoT22P3 < 0)
                BruttoT22P3 = 0;
            if (BruttoT22P4 < 0)
                BruttoT22P4 = 0;

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

            if (BruttoT23P1 < 0)
                BruttoT23P1 = 0;
            if (BruttoT23P2 < 0)
                BruttoT23P2 = 0;
            if (BruttoT23P3 < 0)
                BruttoT23P3 = 0;
            if (BruttoT23P4 < 0)
                BruttoT23P4 = 0;

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

            if (BruttoT24P1 < 0)
                BruttoT24P1 = 0;
            if (BruttoT24P2 < 0)
                BruttoT24P2 = 0;
            if (BruttoT24P3 < 0)
                BruttoT24P3 = 0;
            if (BruttoT24P4 < 0)
                BruttoT24P4 = 0;

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
                Bestellmenge = +Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                Bestellmenge = +Periode3 * (-1);
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
                Bestellmenge = +Periode4 * (-1);
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

            if (BruttoT25P1 < 0)
                BruttoT25P1 = 0;
            if (BruttoT25P2 < 0)
                BruttoT25P2 = 0;
            if (BruttoT25P3 < 0)
                BruttoT25P3 = 0;
            if (BruttoT25P4 < 0)
                BruttoT25P4 = 0;

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
                Bestellmenge = +Periode1 * (-1);
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

            if (BruttoT27P1 < 0)
                BruttoT27P1 = 0;
            if (BruttoT27P2 < 0)
                BruttoT27P2 = 0;
            if (BruttoT27P3 < 0)
                BruttoT27P3 = 0;
            if (BruttoT27P4 < 0)
                BruttoT27P4 = 0;

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
            if (BruttoT28P1 < 0)
                BruttoT28P1 = 0;
            if (BruttoT28P2 < 0)
                BruttoT28P2 = 0;
            if (BruttoT28P3 < 0)
                BruttoT28P3 = 0;
            if (BruttoT28P4 < 0)
                BruttoT28P4 = 0;

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

            if (BruttoT32P1 < 0)
                BruttoT32P1 = 0;
            if (BruttoT32P2 < 0)
                BruttoT32P2 = 0;
            if (BruttoT32P3 < 0)
                BruttoT32P3 = 0;
            if (BruttoT32P4 < 0)
                BruttoT32P4 = 0;

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
                                +(int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 2);
            int BruttoT33P2 = GlobalVariables.SaleMaleBikeN1.GetValueOrDefault() * 2;
            int BruttoT33P3 = GlobalVariables.SaleMaleBikeN2.GetValueOrDefault() * 2;
            int BruttoT33P4 = GlobalVariables.SaleMaleBikeN3.GetValueOrDefault() * 2;

            P1Zuwachs = 0;
            P2Zuwachs = 0;
            P3Zuwachs = 0;
            P4Zuwachs = 0;

            if (BruttoT21P1 < 0)
                BruttoT21P1 = 0;
            if (BruttoT21P2 < 0)
                BruttoT21P2 = 0;
            if (BruttoT21P3 < 0)
                BruttoT21P3 = 0;
            if (BruttoT21P4 < 0)
                BruttoT21P4 = 0;

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
            int BruttoT34P1 = ((GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 72)
                + ((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 0)
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
                Bestellmenge = +Periode3 * (-1);
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
            GlobalVariables.Bestellungsspeicher = Bestellungsliste;

            Bestellung.DataContext = Bestellungsplannung.DefaultView;

            BestellungslisteZu.DataContext = GlobalVariables.Bestellungsspeicher;
            BestellungslisteZu.ItemsSource = GlobalVariables.Bestellungsspeicher.DefaultView;

            //LagerZuProdCheck
            DataTable VerbrauchInPeriode = new DataTable();
            if (!VerbrauchInPeriode.Columns.Contains("Teil"))
            {
                VerbrauchInPeriode.Columns.Add("Teil");
                VerbrauchInPeriode.Columns.Add("Verbrauch");
                VerbrauchInPeriode.Columns.Add("Lager");
                VerbrauchInPeriode.Columns.Add("Differenz");
            }
            VerbrauchInPeriode.Clear();



            int P1Produktionsauftrag = 0;
            if (GlobalVariables.P1Produktionsauftrag > 0)
                P1Produktionsauftrag = GlobalVariables.P1Produktionsauftrag;
            int P2Produktionsauftrag = 0;
            if (GlobalVariables.P2Produktionsauftrag > 0)
                P2Produktionsauftrag = GlobalVariables.P2Produktionsauftrag;
            int P3Produktionsauftrag = 0;
            if (GlobalVariables.P3Produktionsauftrag > 0)
                P3Produktionsauftrag = GlobalVariables.P3Produktionsauftrag;
            int E4Produktionsauftrag = 0;
            if (GlobalVariables.E4Produktionsauftrag > 0)
                E4Produktionsauftrag = GlobalVariables.E4Produktionsauftrag;
            int E5Produktionsauftrag = 0;
            if (GlobalVariables.E5Produktionsauftrag > 0)
                E5Produktionsauftrag = GlobalVariables.E5Produktionsauftrag;
            int E6Produktionsauftrag = 0;
            if (GlobalVariables.E6Produktionsauftrag > 0)
                E6Produktionsauftrag = GlobalVariables.E6Produktionsauftrag;
            int E7Produktionsauftrag = 0;
            if (GlobalVariables.E7Produktionsauftrag > 0)
                E7Produktionsauftrag = GlobalVariables.E7Produktionsauftrag;
            int E8Produktionsauftrag = 0;
            if (GlobalVariables.E8Produktionsauftrag > 0)
                E8Produktionsauftrag = GlobalVariables.E8Produktionsauftrag;
            int E9Produktionsauftrag = 0;
            if (GlobalVariables.E9Produktionsauftrag > 0)
                E9Produktionsauftrag = GlobalVariables.E9Produktionsauftrag;
            int E10Produktionsauftrag = 0;
            if (GlobalVariables.E10Produktionsauftrag > 0)
                E10Produktionsauftrag = GlobalVariables.E10Produktionsauftrag;
            int E11Produktionsauftrag = 0;
            if (GlobalVariables.E11Produktionsauftrag > 0)
                E11Produktionsauftrag = GlobalVariables.E11Produktionsauftrag;
            int E12Produktionsauftrag = 0;
            if (GlobalVariables.E12Produktionsauftrag > 0)
                E12Produktionsauftrag = GlobalVariables.E12Produktionsauftrag;
            int E13Produktionsauftrag = 0;
            if (GlobalVariables.E13Produktionsauftrag > 0)
                E13Produktionsauftrag = GlobalVariables.E13Produktionsauftrag;
            int E14Produktionsauftrag = 0;
            if (GlobalVariables.E14Produktionsauftrag > 0)
                E14Produktionsauftrag = GlobalVariables.E14Produktionsauftrag;
            int E15Produktionsauftrag = 0;
            if (GlobalVariables.E15Produktionsauftrag > 0)
                E15Produktionsauftrag = GlobalVariables.E15Produktionsauftrag;
            int E16Produktionsauftrag = 0;
            if (GlobalVariables.E16Produktionsauftrag > 0)
                E16Produktionsauftrag = GlobalVariables.E16Produktionsauftrag;
            int E17Produktionsauftrag = 0;
            if (GlobalVariables.E17Produktionsauftrag > 0)
                E17Produktionsauftrag = GlobalVariables.E17Produktionsauftrag;
            int E18Produktionsauftrag = 0;
            if (GlobalVariables.E18Produktionsauftrag > 0)
                E18Produktionsauftrag = GlobalVariables.E18Produktionsauftrag;
            int E19Produktionsauftrag = 0;
            if (GlobalVariables.E19Produktionsauftrag > 0)
                E19Produktionsauftrag = GlobalVariables.E19Produktionsauftrag;
            int E20Produktionsauftrag = 0;
            if (GlobalVariables.E20Produktionsauftrag > 0)
                E20Produktionsauftrag = GlobalVariables.E20Produktionsauftrag;
            int E26Produktionsauftrag = 0;
            if (GlobalVariables.E26Produktionsauftrag > 0)
                E26Produktionsauftrag = GlobalVariables.E26Produktionsauftrag;
            int E49Produktionsauftrag = 0;
            if (GlobalVariables.E49Produktionsauftrag > 0)
                E49Produktionsauftrag = GlobalVariables.E49Produktionsauftrag;
            int E54Produktionsauftrag = 0;
            if (GlobalVariables.E54Produktionsauftrag > 0)
                E54Produktionsauftrag = GlobalVariables.E54Produktionsauftrag;
            int E29Produktionsauftrag = 0;
            if (GlobalVariables.E29Produktionsauftrag > 0)
                E29Produktionsauftrag = GlobalVariables.E29Produktionsauftrag;
            int E50Produktionsauftrag = 0;
            if (GlobalVariables.E50Produktionsauftrag > 0)
                E50Produktionsauftrag = GlobalVariables.E50Produktionsauftrag;
            int E55Produktionsauftrag = 0;
            if (GlobalVariables.E55Produktionsauftrag > 0)
                E55Produktionsauftrag = GlobalVariables.E55Produktionsauftrag;
            int E30Produktionsauftrag = 0;
            if (GlobalVariables.E30Produktionsauftrag > 0)
                E30Produktionsauftrag = GlobalVariables.E30Produktionsauftrag;
            int E51Produktionsauftrag = 0;
            if (GlobalVariables.E51Produktionsauftrag > 0)
                E51Produktionsauftrag = GlobalVariables.E51Produktionsauftrag;
            int E56Produktionsauftrag = 0;
            if (GlobalVariables.E56Produktionsauftrag > 0)
                E56Produktionsauftrag = GlobalVariables.E56Produktionsauftrag;
            int E31Produktionsauftrag = 0;
            if (GlobalVariables.E31Produktionsauftrag > 0)
                E31Produktionsauftrag = GlobalVariables.E31Produktionsauftrag;



            int t4 = 0;
            int t5 = 0;
            int t6 = 0;
            int t7 = 0;
            int t8 = 0;
            int t9 = 0;
            int t10 = 0;
            int t11 = 0;
            int t12 = 0;
            int t13 = 0;
            int t14 = 0;
            int t15 = 0;
            int t16 = 0;
            int t17 = 0;
            int t18 = 0;
            int t19 = 0;
            int t20 = 0;
            int t21 = P1Produktionsauftrag;
            int t22 = P2Produktionsauftrag;
            int t23 = P3Produktionsauftrag;
            int t24 = E16Produktionsauftrag + E49Produktionsauftrag* 2 + E54Produktionsauftrag * 2 + E29Produktionsauftrag * 2 + E50Produktionsauftrag * 2
                 + E55Produktionsauftrag * 2 + E30Produktionsauftrag * 2 + E51Produktionsauftrag + E56Produktionsauftrag + E31Produktionsauftrag
                 + P1Produktionsauftrag + P2Produktionsauftrag + P3Produktionsauftrag;
            int t25 = E49Produktionsauftrag* 2 + E54Produktionsauftrag * 2 + E29Produktionsauftrag * 2 + E50Produktionsauftrag * 2
                + E55Produktionsauftrag * 2 + E30Produktionsauftrag * 2; ;
            int t26 = 0;
            int t27 = +E51Produktionsauftrag + E56Produktionsauftrag + E31Produktionsauftrag
                + P1Produktionsauftrag + P2Produktionsauftrag + P3Produktionsauftrag;
            int t28 = E16Produktionsauftrag + E18Produktionsauftrag*3 + E19Produktionsauftrag*4 + E20Produktionsauftrag*5;
            int t29 = 0;
            int t30 = 0;
            int t31 = 0;
            int t32 = E10Produktionsauftrag + E11Produktionsauftrag + E12Produktionsauftrag + E13Produktionsauftrag + E14Produktionsauftrag + E15Produktionsauftrag
                + E18Produktionsauftrag + E19Produktionsauftrag + E20Produktionsauftrag;
            int t33 = E6Produktionsauftrag + E9Produktionsauftrag;
            int t34 = E6Produktionsauftrag* 36 + E9Produktionsauftrag*36;
            int t35 = E4Produktionsauftrag*2 + E5Produktionsauftrag*2 + E6Produktionsauftrag* 2 
                + E7Produktionsauftrag* 2 + E8Produktionsauftrag*2 + E9Produktionsauftrag*2;
            int t36 = E4Produktionsauftrag + E5Produktionsauftrag + E6Produktionsauftrag;
            int t37 = E7Produktionsauftrag + E8Produktionsauftrag + E9Produktionsauftrag;
            int t38 = E7Produktionsauftrag + E8Produktionsauftrag + E9Produktionsauftrag;
            int t39 = E10Produktionsauftrag + E11Produktionsauftrag + E12Produktionsauftrag + E13Produktionsauftrag + +E14Produktionsauftrag + E15Produktionsauftrag;
            int t40 = E16Produktionsauftrag;
            int t41 = E16Produktionsauftrag;
            int t42 = E16Produktionsauftrag*2;
            int t43 = E17Produktionsauftrag;
            int t44 = E17Produktionsauftrag + E26Produktionsauftrag*2;
            int t45 = E17Produktionsauftrag;
            int t46 = E17Produktionsauftrag;
            int t47 = E26Produktionsauftrag*1;
            int t48 = E26Produktionsauftrag*2;
            int t49 = 0;
            int t50 = 0;
            int t51 = 0;
            int t52 = E4Produktionsauftrag + E7Produktionsauftrag;
            int t53 = E4Produktionsauftrag* 36 + E7Produktionsauftrag*36;
            int t54 = 0;
            int t55 = 0;
            int t56 = 0;
            int t57 = E5Produktionsauftrag + E8Produktionsauftrag;
            int t58 = E5Produktionsauftrag* 36 + E8Produktionsauftrag*36;
            int t59 = E18Produktionsauftrag*2 + E19Produktionsauftrag*2 + E20Produktionsauftrag*2;


            Console.WriteLine("TestTESTSET: " + t33.ToString());


            

            //foreach(DataRow row in GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables[2].Rows)
            //    {
            //        if((int)row["id"] == 21 || (int)row["id"] == 22 || (int)row["id"] == 23 || (int)row["id"] == 24 || (int)row["id"] == 25 || (int)row["id"] == 27 ||
            //        (int)row["id"] == 28 || (int)row["id"] == 32 || (int)row["id"] == 33 || (int)row["id"] == 34 || (int)row["id"] == 35 || (int)row["id"] == 36 ||
            //        (int)row["id"] == 37 || (int)row["id"] == 38 || (int)row["id"] == 39 || (int)row["id"] == 40 || (int)row["id"] == 41 || (int)row["id"] == 42 ||
            //        (int)row["id"] == 43 || (int)row["id"] == 44 || (int)row["id"] == 45 || (int)row["id"] == 46 || (int)row["id"] == 47 || (int)row["id"] == 48 ||
            //        (int)row["id"] == 52 || (int)row["id"] == 53 || (int)row["id"] == 57 || (int)row["id"] == 58 || (int)row["id"] == 59 )
            //        {
            //            VerbrauchInPeriode.Rows.Add();
            //        }
            //    }


            #endregion bestellungsplannung

            Orderlist.DataContext = BestellungslisteZu.DataContext;
            Orderlist.ItemsSource = BestellungslisteZu.ItemsSource;
            
            foreach (DataRow row in data.Tables["workplace"].Rows)
            {
                int id = Convert.ToInt32((string)row["id"]);
                int setUps = -1;
                if (!(row["setupevents"] is DBNull))
                    setUps = Convert.ToInt32((string)row["setupevents"]);

                if(id == 1 && setUps >= 0)
                {
                    GlobalVariables.A1SetUpsLastPeriod = (setUps * 20);
                }
                if (id == 2 && setUps >= 0)
                {
                    GlobalVariables.A2SetUpsLastPeriod = (setUps * 30);
                }
                if (id == 3 && setUps >= 0)
                {
                    GlobalVariables.A3SetUpsLastPeriod = (setUps * 20);
                }
                if (id == 4 && setUps >= 0)
                {
                    GlobalVariables.A4SetUpsLastPeriod = (setUps * 30);
                }
                if (id == 6 && setUps >= 0)
                {
                    GlobalVariables.A6SetUpsLastPeriod = (setUps * 15);
                }
                if (id == 7 && setUps >= 0)
                {
                    GlobalVariables.A7SetUpsLastPeriod = (setUps * 25);
                }
                if (id == 8 && setUps >= 0)
                {
                    GlobalVariables.A8SetUpsLastPeriod = (setUps * 20);
                }
                if (id == 9 && setUps >= 0)
                {
                    GlobalVariables.A9SetUpsLastPeriod = (setUps * 15);
                }
                if (id == 10 && setUps >= 0)
                {
                    GlobalVariables.A10SetUpsLastPeriod = (setUps * 20);
                }
                if (id == 11 && setUps >= 0)
                {
                    GlobalVariables.A11SetUpsLastPeriod = (setUps * 15);
                }
                if (id == 12 && setUps >= 0)
                {
                    GlobalVariables.A12SetUpsLastPeriod = (setUps * 0);
                }
                if (id == 13 && setUps >= 0)
                {
                    GlobalVariables.A13SetUpsLastPeriod = (setUps * 0);
                }
                if (id == 14 && setUps >= 0)
                {
                    GlobalVariables.A14SetUpsLastPeriod = (setUps * 0);
                }
                if (id == 15 && setUps >= 0)
                {
                    GlobalVariables.A15SetUpsLastPeriod = (setUps * 15);
                }
            }

            foreach (DataRow row in data.Tables["workplace"].Rows)
            {
                int id = Convert.ToInt32((string)row["id"]);
                int timeneed = -1;
                if (!(row["timeneed"] is DBNull))
                    timeneed = Convert.ToInt32((string)row["timeneed"]);

                if (id == 1 && timeneed >= 0)
                {
                    kapOldA1.Text = (Int32.Parse(kapOldA1.Text) + timeneed).ToString();
                }
                if (id == 2 && timeneed >= 0)
                {
                    kapOldA2.Text = (Int32.Parse(kapOldA2.Text) + timeneed).ToString();
                }
                if (id == 3 && timeneed >= 0)
                {
                    kapOldA3.Text = (Int32.Parse(kapOldA3.Text) + timeneed).ToString();
                }
                if (id == 4 && timeneed >= 0)
                {
                    kapOldA4.Text = (Int32.Parse(kapOldA4.Text) + timeneed).ToString();
                }
                if (id == 6 && timeneed >= 0)
                {
                    kapOldA6.Text = (Int32.Parse(kapOldA6.Text) + timeneed).ToString();
                }
                if (id == 7 && timeneed >= 0)
                {
                    kapOldA7.Text = (Int32.Parse(kapOldA7.Text) + timeneed).ToString();
                }
                if (id == 8 && timeneed >= 0)
                {
                    kapOldA8.Text = (Int32.Parse(kapOldA8.Text) + timeneed).ToString();
                }
                if (id == 9 && timeneed >= 0)
                {
                    kapOldA9.Text = (Int32.Parse(kapOldA9.Text) + timeneed).ToString();
                }
                if (id == 10 && timeneed >= 0)
                {
                    kapOldA10.Text = (Int32.Parse(kapOldA10.Text) + timeneed).ToString();
                }
                if (id == 11 && timeneed >= 0)
                {
                    kapOldA11.Text = (Int32.Parse(kapOldA11.Text) + timeneed).ToString();
                }
                if (id == 12 && timeneed >= 0)
                {
                    kapOldA12.Text = (Int32.Parse(kapOldA12.Text) + timeneed).ToString();
                }
                if (id == 13 && timeneed >= 0)
                {
                    kapOldA13.Text = (Int32.Parse(kapOldA13.Text) + timeneed).ToString();
                }
                if (id == 14 && timeneed >= 0)
                {
                    kapOldA14.Text = (Int32.Parse(kapOldA14.Text) + timeneed).ToString();
                }
                if (id == 15 && timeneed >= 0)
                {
                    kapOldA15.Text = (Int32.Parse(kapOldA15.Text) + timeneed).ToString();
                }
            }
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
            #endregion RepeatProgrammplannungKalkulation

            #region DataTable

            GlobalVariables.dtProdOrder.Clear();
            if (!GlobalVariables.dtProdOrder.Columns.Contains("Item"))
                GlobalVariables.dtProdOrder.Columns.Add("Item");
            if (!GlobalVariables.dtProdOrder.Columns.Contains("Amount"))
                GlobalVariables.dtProdOrder.Columns.Add("Amount");

            //Enter all Rows
            DataRow P1P = GlobalVariables.dtProdOrder.NewRow();
            P1P["Item"] = "1";
            if (int.Parse(ChildBikePlannedProductionP1.Text) > 0)
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


            #endregion wut

            #region DataTable

            
            if (!GlobalVariables.dtProdOrder.Columns.Contains("Item"))
                GlobalVariables.dtProdOrder.Columns.Add("Item");
            if (!GlobalVariables.dtProdOrder.Columns.Contains("Amount"))
                GlobalVariables.dtProdOrder.Columns.Add("Amount");


            GlobalVariables.Aussortierung.Clear();
            if (!GlobalVariables.Aussortierung.Columns.Contains("Item"))
                GlobalVariables.Aussortierung.Columns.Add("Item");
            if (!GlobalVariables.Aussortierung.Columns.Contains("Amount"))
                GlobalVariables.Aussortierung.Columns.Add("Amount");


            




            List<DataRow> P1P2P3 = new List<DataRow> { P1P, P2P, P3P };
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
            #endregion Ordering
            #endregion Ordering2


            foreach (DataRow Row in GlobalVariables.dtProdOrder.Rows)
            {
                if (Row[0] != DBNull.Value && int.Parse((string)Row[1]) != 0)
                {
                    GlobalVariables.Aussortierung.Rows.Add(Row[0], Row[1]);
                }
            }


            GridProductionOrders.DataContext = GlobalVariables.Aussortierung.DefaultView;

            ////Umwandlung Tabelle Lager zu Dictionary
            GlobalVariables.Lagerstand.Clear();
            foreach (DataRow Row in GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables[2].Rows)
            {
                GlobalVariables.Lagerstand.Add(int.Parse((string)Row[0]), int.Parse((string)Row[1]));
            }

            Dictionary<int, int> LagerZuBeginn = ObjectCopier.Clone(GlobalVariables.Lagerstand);

            #region bestellungsplannung
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
            if (GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables.Contains("order"))
            {
                foreach (DataRow DR in GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables["order"].Rows)
                {

                    if (GlobalVariables.InputDataSetWithoutOldBatchCalc.Tables["order"].Columns.Contains("futureinwardstockmovement_Id"))
                    {
                        if (DR["futureinwardstockmovement_Id"] != DBNull.Value)
                        {
                            B1 = DR["orderperiod"].ToString();
                            B11 = int.Parse(B1) - AktuellePeriode;
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
            }

            foreach (DataRow DR in AlteBestellungen.Rows)
            {
                Console.WriteLine("Vergangenheit: " + DR[0].ToString() + " Artikel: " + DR[1].ToString() + " Menge: " + DR[2].ToString());
            }




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
            double T21E = 1.8 / 2;
            int BruttoT21P1 = GlobalVariables.SaleChildBikeN.GetValueOrDefault() * 1 + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text));
            int BruttoT21P2 = GlobalVariables.SaleChildBikeN1.GetValueOrDefault() * 1;
            int BruttoT21P3 = GlobalVariables.SaleChildBikeN2.GetValueOrDefault() * 1;
            int BruttoT21P4 = GlobalVariables.SaleChildBikeN3.GetValueOrDefault() * 1;
            int P1Zuwachs = 0;
            int P2Zuwachs = 0;
            int P3Zuwachs = 0;
            int P4Zuwachs = 0;


            DataRow[] results = AlteBestellungen.Select("item = '21'");
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
                if (T21LZ + T21AB > 2)
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
                Bestellmenge = +Periode2 * (-1);
                Modus = 4;
            }
            if (Periode3 < 0 && Bestellmenge < Periode3 * (-1))
            {
                Bestellmenge = +Periode3 * (-1);
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
                Bestellmenge = +Periode4 * (-1);
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
                Bestellmenge = +Periode1 * (-1);
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
                                +(int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 2);
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
            int BruttoT34P1 = ((GlobalVariables.SaleMaleBikeN.GetValueOrDefault() + (int.Parse(MaleBikeSafetyP3.Text) - int.Parse(MaleBikeStockP3.Text))) * 72)
                + ((GlobalVariables.SaleChildBikeN.Value + (int.Parse(ChildBikeSafetyP1.Text) - int.Parse(ChildBikeStockP1.Text))) * 0)
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
                Bestellmenge = +Periode3 * (-1);
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


            GlobalVariables.Bestellungsspeicher = Bestellungsliste;

            Bestellung.DataContext = Bestellungsplannung.DefaultView;

            BestellungslisteZu.DataContext = GlobalVariables.Bestellungsspeicher;
            BestellungslisteZu.ItemsSource = GlobalVariables.Bestellungsspeicher.DefaultView;

            #endregion bestellungsplannung


            Orderlist.DataContext = BestellungslisteZu.DataContext;
            Orderlist.ItemsSource = BestellungslisteZu.ItemsSource;

            refreshKapaPlanInputs();
            refreshAmountKapaPlan();
            calculateCapNeed();
            calculateSetUpTime();
            calcWholeCap();
            calcWorkLoad();
            calculateShiftAndOverDo();
        }

        #endregion Data

        private void DeleteOrder_OnClick(object sender, RoutedEventArgs e)
        {
            var currentRowIndex = GridProductionOrders.SelectedIndex;
            if(!(currentRowIndex < 0))
            {
                DataRowView row = (DataRowView)GridProductionOrders.SelectedItems[0];
                row.Delete();
            }
                
        }


        private void AddOrder_OnClick(object sender, RoutedEventArgs e)
        {
            if (AddItemBox.Text.Length > 0 && AddAmountBox.Text.Length > 0)
            {
                string ItemID;
                string amount;
                ItemID = AddItemBox.Text;
                amount = AddAmountBox.Text;
                DataRow AddRow = GlobalVariables.Aussortierung.NewRow();
                AddRow["Item"] = ItemID;
                AddRow["Amount"] = amount;
                GlobalVariables.Aussortierung.Rows.Add(AddRow);
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
            DataRow row = GlobalVariables.Aussortierung.Rows[old];
            DataRow row2 = GlobalVariables.Aussortierung.NewRow();
            row2.ItemArray = row.ItemArray;

            if (old - 1 >= 0)
            {
                GlobalVariables.Aussortierung.Rows.Remove(GlobalVariables.Aussortierung.Rows[old]);
                GlobalVariables.Aussortierung.Rows.InsertAt(row2, old - 1);
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
            if (old < GlobalVariables.Aussortierung.Rows.Count - 1)
            {
                DataRow row = GlobalVariables.Aussortierung.Rows[old];
                DataRow row2 = GlobalVariables.Aussortierung.NewRow();
                row2.ItemArray = row.ItemArray;

                if (old + 1 <= GlobalVariables.Aussortierung.Rows.Count)
                {
                    GlobalVariables.Aussortierung.Rows.Remove(GlobalVariables.Aussortierung.Rows[old]);
                    GlobalVariables.Aussortierung.Rows.InsertAt(row2, old + 1);
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
            if(GlobalVariables.blockingErrorKapaPlan)
            {
                MessageBox.Show("Overtime for at least one Workstation exist! (Please correct the production in the Capa-Plan-Section, to Export the XML properly)");
                return;
            }

            List<XMLsellwish> VerkäufeImport = new List<XMLsellwish>();
            List<XMLselldirect> DirektVerkäufe = new List<XMLselldirect>();
            List<XMLorderlist> Bestellungen = new List<XMLorderlist>();
            List<XMLproductionlist> Aufträge = new List<XMLproductionlist>();
            List<XMLworkingtimelist> Schichten = new List<XMLworkingtimelist>();


            //DataGridView 
            Sellwish.Items.Refresh();
            foreach (DataRowView dr in Sellwish.ItemsSource)
            {
                if (dr[0] != DBNull.Value && dr[1] != DBNull.Value)
                    VerkäufeImport.Add(new XMLsellwish((int)dr[0], (int)dr[1]));
            }

            foreach (DataRowView dr in Orderlist.ItemsSource)
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
                    Aufträge.Add(new XMLproductionlist(int.Parse((string)dr[0]), (int.Parse((string)dr[1]))));
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

        private void Evaluierung(object sender, RoutedEventArgs e)
        {

            foreach (System.Data.DataRowView dr in GridOldStock.ItemsSource)
            {
                var row = GridOldStock.ItemContainerGenerator.ContainerFromItem(dr) as DataGridRow;
                if ((double.Parse((string)dr["pct"])) >= 80)
                {
                    row.Background = Brushes.Green;
                }
                if ((double.Parse((string)dr["pct"])) < 80)
                {
                    row.Background = Brushes.Yellow;
                }
                if ((double.Parse((string)dr["pct"])) < 60)
                {
                    row.Background = Brushes.Orange;
                }
                if ((double.Parse((string)dr["pct"])) < 40)
                {
                    row.Background = Brushes.Red;
                }
            }
        }

        private void Evaluierung2(object sender, RoutedEventArgs e)
        {
            foreach (System.Data.DataRowView dr in Bestellung.ItemsSource)
            {
                var row = Bestellung.ItemContainerGenerator.ContainerFromItem(dr) as DataGridRow;
                if ((int.Parse((string)dr["Bestellung Periode n+3"])) > 0)
                {
                    row.Background = Brushes.Green;
                }
                if ((int.Parse((string)dr["Bestellung Periode n+3"])) <= 0)
                {
                    row.Background = Brushes.Yellow;
                }
                if ((int.Parse((string)dr["Bestellung Periode n+2"])) <= 0)
                {
                    row.Background = Brushes.Orange;
                }
                if ((int.Parse((string)dr["Bestellung Periode n+1"])) <= 0)
                {
                    row.Background = Brushes.OrangeRed;
                }
                if ((int.Parse((string)dr["Bestellung Periode n"])) <= 0)
                {
                    row.Background = Brushes.Red;
                }
            }
        }

        private void DeleteOrderBestellung_OnClick(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)BestellungslisteZu.SelectedItems[0];
            row.Delete();
        }


        private void AddOrderBestellung_OnClick(object sender, RoutedEventArgs e)
        {
            if (OrderArticle.Text.Length > 0 && OrderAmount.Text.Length > 0 && BestellungModus.SelectedIndex > -1)
            {
                int ItemID;
                int amount;
                int modus;
                ItemID = int.Parse(OrderArticle.Text);
                amount = int.Parse(OrderAmount.Text);
                if(BestellungModus.SelectedIndex == 0)
                {
                    modus = 4;
                }
                else
                {
                    modus = 5;
                }
                DataRow AddRow = GlobalVariables.Bestellungsspeicher.NewRow();
                AddRow["article"] = ItemID;
                AddRow["quantity"] = amount;
                AddRow["modus"] = modus;
                

                GlobalVariables.Bestellungsspeicher.Rows.Add(AddRow);
            }
            else
            {
                MessageBox.Show("Fehler");
            }

        }
    }
}
