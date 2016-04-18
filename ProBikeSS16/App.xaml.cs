using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ProBikeSS16
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Member variables
        public static App Instance;
        public static String Directory;
        public event EventHandler LanguageChangedEvent;
        #endregion

        #region Constructor
        public App()
        {
            // Initialize static variables
            Instance = this;
            Directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Load the Localization Resource Dictionary based on OS language
            SetLanguageResourceDictionary(GetLocXAMLFilePath(CultureInfo.CurrentCulture.Name));
        }

        #endregion

        #region Functions
        /// <summary>
        /// Dynamically load a Localization ResourceDictionary from a file
        /// </summary>
        public void SwitchLanguage(string inFiveCharLang)
        {
            if (CultureInfo.CurrentCulture.Name.Equals(inFiveCharLang))
                return;

            var ci = new CultureInfo(inFiveCharLang);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            SetLanguageResourceDictionary(GetLocXAMLFilePath(inFiveCharLang));
            if (null != LanguageChangedEvent)
            {
                LanguageChangedEvent(this, new EventArgs());
            }
        }

        /// <summary>
        /// Returns the path to the ResourceDictionary file based on the language character string.
        /// </summary>
        /// <param name="inFiveCharLang"></param>
        /// <returns></returns>
        private string GetLocXAMLFilePath(string inFiveCharLang)
        {
            string locXamlFile = "LocalizationDictionary." + inFiveCharLang + ".xaml";
            return Path.Combine(Directory, inFiveCharLang, locXamlFile);
        }

        /// <summary>
        /// Sets or replaces the ResourceDictionary by dynamically loading
        /// a Localization ResourceDictionary from the file path passed in.
        /// </summary>
        /// <param name="inFile"></param>
        private void SetLanguageResourceDictionary(String inFile)
        {
            if (File.Exists(inFile))
            {
                // Read in ResourceDictionary File
                var languageDictionary = new ResourceDictionary();
                languageDictionary.Source = new Uri(inFile);

                // Remove any previous Localization dictionaries loaded
                int langDictId = -1;
                for (int i = 0; i < Resources.MergedDictionaries.Count; i++)
                {
                    var md = Resources.MergedDictionaries[i];
                    // Make sure your Localization ResourceDictionarys have the ResourceDictionaryName
                    // key and that it is set to a value starting with "Loc-".
                    if (md.Contains("ResourceDictionaryName"))
                    {
                        if (md["ResourceDictionaryName"].ToString().StartsWith("Loc-"))
                        {
                            langDictId = i;
                            break;
                        }
                    }
                }
                if (langDictId == -1)
                {
                    // Add in newly loaded Resource Dictionary
                    Resources.MergedDictionaries.Add(languageDictionary);
                }
                else
                {
                    // Replace the current langage dictionary with the new one
                    Resources.MergedDictionaries[langDictId] = languageDictionary;
                }
            }
        }
        #endregion
    }
}
