using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;

namespace ProBikeSS16
{
    class XMLExport
    {
        public void XMLExportReal(List<XMLsellwish> Verkaufswunsch, List<XMLselldirect> Direktverkäufe, List<XMLorderlist> Bestellungen, List<XMLproductionlist> Produktionsaufträge, List<XMLworkingtimelist> Kapazität)
        {
            XDocument doc = new XDocument(new XElement("input",
                new XElement("qualitycontrol", new XAttribute("delay", 0), new XAttribute("losequantity", 0), new XAttribute("type", "no")),
                new XElement("sellwish",
                        Verkaufswunsch.Select(x => new XElement("item", new XAttribute("quantity", x.quantity), new XAttribute("article", x.article)))),
                new XElement("selldirect",
                        Direktverkäufe.Select(x => new XElement("item", new XAttribute("quantity", x.quantity), new XAttribute("article", x.article), 
                        new XAttribute("penalty", x.penalty), new XAttribute("price", x.price)))),
                new XElement("orderlist",
                        Bestellungen.Select(x => new XElement("order", new XAttribute("quantity", x.quantity), new XAttribute("article", x.article),
                        new XAttribute("modus", x.modus)))),
                new XElement("productionlist",
                        Produktionsaufträge.Select(x => new XElement("production", new XAttribute("quantity", x.quantity), new XAttribute("article", x.article)))),
                new XElement("workingtimelist",
                        Kapazität.Select(x => new XElement("workingtime", new XAttribute("overtime", x.overtime), new XAttribute("shift", x.shift),
                        new XAttribute("station", x.station))))));



            
            
            
            //doc.Save("InputFile.xml");


            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Xml (*.xml)|*.xml";
            if (saveFileDialog.ShowDialog().Value)
            {
                doc.Save(saveFileDialog.FileName);
            }
        }
    }
}
