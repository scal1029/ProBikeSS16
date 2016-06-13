using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    class KapazitätPrototyp
    {
        //public void Kapazitätsplannung(List<Order> ListeGeplannteProduktion, Dictionary<int,int> Lagerstand, List<Arbeitsplatzprototyp> Plätze)
        //{   
        //    //1. Schicht
        //    for (int i = 0; i < 2400; i++)
        //    {
        //        //Alle Produktionsaufträge durchlaufen
        //        foreach (Order order in ListeGeplannteProduktion)
        //        {
        //            //Alle fertigen Zwischenteile weitergeben
        //            foreach (Arbeitsplatzprototyp arbeitsplatz in order.PassendeArbeitsplätze)
        //            {
        //                if (arbeitsplatz.bereitsGearbeitet == 1 && arbeitsplatz.BlockZeit == 0)
        //                {
        //                    arbeitsplatz.Next.EigeneWarteSchlange = arbeitsplatz.Next.EigeneWarteSchlange + 10;
        //                    arbeitsplatz.bereitsGearbeitet = 0;
        //                }
        //            }

        //            //Alle dazugehörigen Arbeitsplätze durchlaufen
        //            foreach (Arbeitsplatzprototyp arbeitsplatz in order.PassendeArbeitsplätze)
        //            {
        //                //Check ob noch Aufträge vorhanden sind
        //                if (arbeitsplatz.Platzierung == 1)
        //                {
        //                    if (order.Menge == 0)
        //                    {
        //                        Console.WriteLine("Auftrag: " + order.Auftragnummer + "  Artikel: " + order.ArtikelID + " Arbeitsplatz: " + arbeitsplatz.ArbeitsplatzID + " ist ohne Warteschlange(1. Schicht)");
        //                        continue;
        //                    }
        //                    else
        //                    {
        //                        order.Menge = order.Menge - 10;
        //                        arbeitsplatz.EigeneWarteSchlange = arbeitsplatz.EigeneWarteSchlange + 10;
        //                    }
        //                }
        //                else
        //                {
        //                    if(arbeitsplatz.EigeneWarteSchlange == 0)
        //                    {
        //                        Console.WriteLine("Auftrag: " + order.Auftragnummer + "  Artikel: " + order.ArtikelID + " Arbeitsplatz: " + arbeitsplatz.ArbeitsplatzID + " ist ohne Warteschlange(1. Schicht)");
        //                        continue;
        //                    }
        //                }
                        
        //                //Check ob Arbeitsplatz belegt ist
        //                if (arbeitsplatz.BlockZeit > 0)
        //                {
        //                    Console.WriteLine("Auftrag: "+ order.Auftragnummer + "  Artikel: " + order.ArtikelID + " Arbeitsplatz: " + arbeitsplatz.ArbeitsplatzID + " ist belegt(1. Schicht)");
        //                }
        //                else
        //                {
        //                    int AlleTeileDa = 1;
        //                    foreach (KeyValuePair<int,int> teilmenge in arbeitsplatz.TeileInStuffe)
        //                    {
        //                        //Check ob alle Teile vorhanden sind
        //                        if (AlleTeileDa > 0)
        //                        {
        //                            int LagerstandAktuell = Lagerstand[teilmenge.Key];
        //                            if (LagerstandAktuell > teilmenge.Value*10)
        //                            {
                                        
        //                            }
        //                            else
        //                            {
        //                                AlleTeileDa = 0;
        //                            }
        //                        } 
        //                    }
        //                    //Check Ergebnis des TeileChecks
        //                    if (AlleTeileDa == 0)
        //                    {
        //                        Console.WriteLine("Auftrag: " + order.Auftragnummer + "  Artikel: " + order.ArtikelID + " Arbeitsplatz: " + arbeitsplatz.ArbeitsplatzID + " Material fehlt(1. Schicht)");
        //                    }
        //                    else
        //                    {
        //                        //Lager um Teile reduzieren
        //                        foreach (KeyValuePair<int, int> teilmenge in arbeitsplatz.TeileInStuffe)
        //                        {
        //                            Lagerstand[teilmenge.Key] = Lagerstand[teilmenge.Key] - teilmenge.Value*10;
        //                        }
        //                        //Check ob Gerüstet ist
        //                        if (arbeitsplatz.RüstArtikelID == order.ArtikelID)
        //                        {
        //                            Console.WriteLine("Auftrag: " + order.Auftragnummer + "  Artikel: " + order.ArtikelID + " Arbeitsplatz: " + arbeitsplatz.ArbeitsplatzID + " Gut Gerüstet(1. Schicht)");
        //                        }
        //                        else
        //                        {
        //                            arbeitsplatz.Rüst(order.ArtikelID);
        //                            Console.WriteLine("Auftrag: " + order.Auftragnummer + "  Artikel: " + order.ArtikelID + " Arbeitsplatz: " + arbeitsplatz.ArbeitsplatzID + " Rüstet(1. Schicht)"); 
        //                        }
        //                        //Belegen von Arbeitszeit
        //                        arbeitsplatz.Produz();
        //                        arbeitsplatz.bereitsGearbeitet = 1;
        //                        arbeitsplatz.EigeneWarteSchlange = arbeitsplatz.EigeneWarteSchlange - 10;
        //                    }
        //                }
        //            }
        //        }
        //        foreach (Arbeitsplatzprototyp platze in Plätze)
        //        {
        //            if (platze.BlockZeit > 0)
        //            {
        //                platze.BlockZeit = platze.BlockZeit - 1;
        //            }
        //        }
        //    }
        //}

        public void KappaPlannung(List<OrderPrototyp> Aufträge)
        {
            
        }
    }
}
