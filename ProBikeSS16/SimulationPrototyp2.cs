using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    class SimulationPrototyp2
    {
        public void SimulationPrototypDurchführung(List<OrderPrototyp> AufträgePeriode, Dictionary<int, int> Lagerstand, int Zählstufe)
        {
            List<int> ArbeitsstationenHoch = new List<int>();


            //Liste der Arbeitsplätze generieren
            Dictionary<int, Arbeitsplatzprototyp> Arbeitsplätze = new Dictionary<int, Arbeitsplatzprototyp>();
            foreach (var Order in AufträgePeriode)
            {
                foreach (ArbeitsstationPrototyp Sta in Order.TeilPrototyp.KetteStationen)
                {
                    bool containsItem = Arbeitsplätze.Any(item => item.Value.ID == Sta.Arbeitsplatz.ID);
                    if (!containsItem)
                    {
                        Arbeitsplätze.Add(Sta.Arbeitsplatz.ID, Sta.Arbeitsplatz);
                    }
                }
            }

            //Console.WriteLine("SIM1: Generierte Arbeitsplätze Anzahl Arbeitsplätze: " + Arbeitsplätze.Count);



            //Eingabedaten speichern
            GlobalVariables.OriginalProduktionsAufträgeAktuellePeriode = ObjectCopier.Clone(AufträgePeriode);


            Dictionary<int, Arbeitsplatzprototyp> ABs = new Dictionary<int, Arbeitsplatzprototyp>();
            foreach (var Order in GlobalVariables.OriginalProduktionsAufträgeAktuellePeriode)
            {
                foreach (ArbeitsstationPrototyp Sta in Order.TeilPrototyp.KetteStationen)
                {
                    bool containsItem = ABs.Any(item => item.Key == Sta.Arbeitsplatz.ID);
                    if (!containsItem)
                    {
                        ABs.Add(Sta.Arbeitsplatz.ID, Sta.Arbeitsplatz);
                    }
                }
            }
            //GlobalVariables.OriginalAlleArbeitsplätze = ObjectCopier.Clone(Arbeitsplätze);
            GlobalVariables.OriginalAlleArbeitsplätze = ABs;
            GlobalVariables.OriginalLagerstand = ObjectCopier.Clone(Lagerstand);
            Dictionary<int, int> LagerZuBeginn = ObjectCopier.Clone(Lagerstand);

            //Console.WriteLine("SIM2 Kopierte Anzahl Arbeitsplätze: " + Arbeitsplätze.Count);


            //Alle Werktage durchgehen
            for (int w = 1; w < 6; w++)
            {
                //jede Arbeitsminute
                for (int i = 0; i < 1440; i = i + Zählstufe)
                {
                    //Auftragsreiehfolge beachten
                    foreach (OrderPrototyp Auftrag in AufträgePeriode)
                    {
                        // Weitergabe an erste Station
                        if (Auftrag.Menge > 0)
                        {
                            Auftrag.TeilPrototyp.KetteStationen[0].Warteschlange = Auftrag.TeilPrototyp.KetteStationen[0].Warteschlange + Auftrag.Menge;
                            Auftrag.Menge = 0;
                        }
                        //Console.WriteLine(AufträgePeriode[5].TeilPrototyp.KetteStationen[0].Warteschlange);
                        //Alle Stationen checken
                        foreach (ArbeitsstationPrototyp Station in Auftrag.TeilPrototyp.KetteStationen)
                        {

                            //Console.WriteLine("Arbeitszeit" + Station.Arbeitsplatz.ArbeitszeitProTagInMinuten.ToString() + " größer als " + i.ToString());
                            //Arbeiter noch eingeteilt?
                            if (Station.Arbeitsplatz.ArbeitszeitProTagInMinuten >= i)
                            {
                                //Warteschlange vorhanden?
                                //Console.WriteLine(Station.Warteschlange);
                                if (Station.Warteschlange > 0)
                                {
                                    //Console.WriteLine("Ist " + (Station.Arbeitsplatz.Blockierzeit).ToString() + " größer als 0(Soll nicht)");
                                    //Arbeiter arbeitet bereits woanders?
                                    if (Station.Arbeitsplatz.Blockierzeit == 0)
                                    {
                                        //Genügend Teile in Lager?
                                        bool TeileVorhanden = true;
                                        foreach (KeyValuePair<int, int> Teile in Station.TeileProStation)
                                        {
                                            if (Station.TeileProStation.Count > 0)
                                            {
                                                //Console.WriteLine("Ist " + (Teile.Value*10).ToString() + " größer als " + Lagerstand[Teile.Key].ToString());
                                                if ((Teile.Value * 10) <= Lagerstand[Teile.Key])
                                                {
                                                    TeileVorhanden = true;
                                                }
                                                else
                                                {
                                                    TeileVorhanden = false;
                                                    break;
                                                }
                                            }
                                        }
                                        if (TeileVorhanden)
                                        {
                                            //Teile aus Lager entnehmen
                                            foreach (KeyValuePair<int, int> Teile in Station.TeileProStation)
                                            {
                                                Lagerstand[Teile.Key] = Lagerstand[Teile.Key] - (Teile.Value * 10);
                                            }
                                            //Rüstung checken(Rüsten, Hochzählen, Blockierzeit)
                                            if (Auftrag.TeilPrototyp.TeilID != Station.Arbeitsplatz.RüstID)
                                            {
                                                Station.Arbeitsplatz.RüstID = Auftrag.TeilPrototyp.TeilID;
                                                Station.Arbeitsplatz.Rüstzeit = Station.Arbeitsplatz.Rüstzeit +
                                                                                Station.Rüstzeit;
                                                Station.Arbeitsplatz.Rüstungen = Station.Arbeitsplatz.Rüstungen + 1;
                                                Station.Arbeitsplatz.Blockierzeit = Station.Arbeitsplatz.Blockierzeit +
                                                                                    Station.Rüstzeit;
                                            }
                                            Station.Produziert = true;
                                            Station.Warteschlange = Station.Warteschlange - 10;
                                            Station.Arbeitsplatz.Arbeitszeit = Station.Arbeitsplatz.Arbeitszeit +
                                                                               Station.Prodzeit*10;
                                            Station.Arbeitsplatz.Blockierzeit = Station.Arbeitsplatz.Blockierzeit +
                                                                                Station.Prodzeit*10;
                                            Station.BegründungStop = "Produzieren";
                                        }
                                        else
                                        {
                                            Station.BegründungStop = "Keine Teile";
                                        }
                                    }
                                    else
                                    {
                                        Station.BegründungStop = "Blockiert durch anderen Arbeitsplatz";
                                    }
                                }
                                else
                                {
                                    Station.BegründungStop = "Keine WarteSchlange";
                                }
                            }
                            else
                            {
                                Station.BegründungStop = "Keine Arbeitszeit";
                            }
                        }
                    }

                    //Check ob Leerzeit


                    //LagerveränderungUndWarteschlangenWeitergabe(AufträgePeriode, Arbeitsplätze, Lagerstand); //+Produktion aus

                    foreach (OrderPrototyp OP in AufträgePeriode)
                    {
                        foreach (ArbeitsstationPrototyp ASP in OP.TeilPrototyp.KetteStationen)
                        {
                            if (ASP.Produziert)
                            {
                                if (ASP.Arbeitsplatz.Blockierzeit == 1)
                                {
                                    int index = OP.TeilPrototyp.KetteStationen.FindIndex(a => a.ID == ASP.ID);
                                    //Console.WriteLine("Kette Position:" + index.ToString());
                                    if (index < OP.TeilPrototyp.KetteStationen.Count - 1)
                                    {
                                        OP.TeilPrototyp.KetteStationen[index + 1].Warteschlange =
                                            OP.TeilPrototyp.KetteStationen[index + 1].Warteschlange + 10;
                                    }
                                    else
                                    {
                                        Lagerstand[ASP.Teil] = Lagerstand[ASP.Teil] + 10;
                                    }
                                    ASP.Produziert = false;
                                }
                            }
                        }
                    }





                    foreach (KeyValuePair<int, Arbeitsplatzprototyp> ABP in Arbeitsplätze)
                    {
                        if (ABP.Value.Blockierzeit > 0 || ABP.Value.ArbeitszeitProTagInMinuten >= i)
                            ABP.Value.Blockierzeit = ABP.Value.Blockierzeit - 1;
                    }




                }
                //Console.WriteLine(Wochentag);
                //foreach (var Aufträge in AufträgePeriode)
                //{
                //    foreach (var Station in Aufträge.TeilPrototyp.KetteStationen)
                //    {
                //        if(Station.ID == "Teil7Station1Arbeitsplatz10" || Station.ID == "Teil7Station2Arbeitsplatz11")
                //        Console.WriteLine(Station.ID + " Warteschlange: " + Station.Warteschlange.ToString());
                //    }
                //}
                
                //foreach (var order in AufträgePeriode)
                //{
                //    Console.WriteLine("Artikel: " + order.Artikel + " Menge: " + order.Menge);
                //    foreach (var VARIABLE in order.TeilPrototyp.KetteStationen)
                //    {
                //        Console.WriteLine("Station: " + VARIABLE.ID + " Warteschlange: " + VARIABLE.Warteschlange + " Rüstungen: " + VARIABLE.Arbeitsplatz.Rüstungen);
                //    }
                //}
            }
            //Console.WriteLine("Vor GoodCheck");


            






            bool Ergebnis = false;
            foreach (OrderPrototyp Order in AufträgePeriode)
            {
                foreach (ArbeitsstationPrototyp Station in Order.TeilPrototyp.KetteStationen)
                {
                    if (Station.BegründungStop != "Keine Arbeitszeit")
                    {
                        if (Station.Produziert)
                        {
                           // Console.WriteLine("Produziert " + Station.ID);
                        }
                    }
                    else
                    {
                        //if (Station.Warteschlange>0)
                        //{
                        bool TeileVorhanden = true;
                        foreach (KeyValuePair<int, int> Teile in Station.TeileProStation)
                        {
                            if (Station.TeileProStation.Count > 0)
                            {
                                //Console.WriteLine("Ist " + (Teile.Value*10).ToString() + " größer als " + Lagerstand[Teile.Key].ToString());
                                if ((Teile.Value * 10) <= Lagerstand[Teile.Key])
                                {
                                    TeileVorhanden = true;
                                }
                                else
                                {
                                    TeileVorhanden = false;
                                    break;
                                }
                            }
                        }
                        if (TeileVorhanden)
                        {

                            foreach (OrderPrototyp OA in GlobalVariables.OriginalProduktionsAufträgeAktuellePeriode)
                            {
                                foreach (ArbeitsstationPrototyp OAK in OA.TeilPrototyp.KetteStationen)
                                {
                                    if (Station.ID == OAK.ID && Station.Warteschlange > 0 && Station.Produziert == false)
                                    {
                                        //OAK.Arbeitsplatz.ArbeitszeitProTagInMinuten =
                                        //    OAK.Arbeitsplatz.ArbeitszeitProTagInMinuten + Zählstufe;
                                        //Arbeitsplätze[OAK.Arbeitsplatz.ID].ArbeitszeitProTagInMinuten =
                                        //    Arbeitsplätze[OAK.Arbeitsplatz.ID].ArbeitszeitProTagInMinuten +
                                        //    Zählstufe;
                                        //Station.Arbeitsplatz.ArbeitszeitProTagInMinuten =
                                        //    Station.Arbeitsplatz.ArbeitszeitProTagInMinuten + Zählstufe;
                                        Ergebnis = true;


                                        bool containsItem =
                                            ArbeitsstationenHoch.Any(item => item == Station.Arbeitsplatz.ID);
                                        if (!containsItem)
                                        {
                                            ArbeitsstationenHoch.Add(Station.Arbeitsplatz.ID);
                                        }
                                    }
                                }
                            }
                        }
                        //}

                       // Console.WriteLine(Station.ID + " Warteschlange: " + Station.Warteschlange.ToString() + " Teile = " + TeileVorhanden.ToString());
                    }
                }
            }


            if (Ergebnis) //Wenn schlecht dann Arbeitsplätze original Arbeitszeit erhöhen
            {


                //Auftrag wieder zurücksetzten ohne Struktur zu stören
                foreach (OrderPrototyp Order in AufträgePeriode)
                {
                    foreach (var OrderKopie in GlobalVariables.OriginalProduktionsAufträgeAktuellePeriode)
                    {
                        if (OrderKopie.TeilPrototyp.TeilID == Order.TeilPrototyp.TeilID)
                        {
                            Order.Menge = OrderKopie.Menge;
                            foreach (ArbeitsstationPrototyp Station in Order.TeilPrototyp.KetteStationen)
                            {
                                foreach (ArbeitsstationPrototyp StationKopie in OrderKopie.TeilPrototyp.KetteStationen)
                                {
                                    if (Station.ID == StationKopie.ID)
                                    {
                                        //Station.Arbeitsplatz.ArbeitszeitProTagInMinuten = StationKopie.Arbeitsplatz.ArbeitszeitProTagInMinuten;
                                        Station.Arbeitsplatz.Blockierzeit = StationKopie.Arbeitsplatz.Blockierzeit;
                                        Station.Arbeitsplatz.Arbeitszeit = StationKopie.Arbeitsplatz.Arbeitszeit;
                                        Station.Arbeitsplatz.RüstID = StationKopie.Arbeitsplatz.RüstID;
                                        Station.Arbeitsplatz.Rüstungen = StationKopie.Arbeitsplatz.Rüstungen;
                                        Station.Arbeitsplatz.Rüstzeit = StationKopie.Arbeitsplatz.Rüstzeit;

                                        Station.Warteschlange = StationKopie.Warteschlange;
                                        Station.Produziert = StationKopie.Produziert;
                                        Station.BegründungStop = StationKopie.BegründungStop;
                                    }
                                }
                            }
                            
                        }
                    }
                }

                //Arbeitsplätze Arbeitszeit erhöhen;
                foreach (int StationZuErhöhen in ArbeitsstationenHoch)
                {
                    bool NichtErhöht = true;
                    foreach (OrderPrototyp Auftrag in AufträgePeriode)
                    {
                        foreach (ArbeitsstationPrototyp Station in Auftrag.TeilPrototyp.KetteStationen)
                        {
                            if (NichtErhöht)
                            {
                                if (StationZuErhöhen == Station.Arbeitsplatz.ID && Station.Arbeitsplatz.ArbeitszeitProTagInMinuten <=1440)
                                {
                                    if ((Station.Arbeitsplatz.ArbeitszeitProTagInMinuten>=480 && Station.Arbeitsplatz.ArbeitszeitProTagInMinuten<720)
                                        || (Station.Arbeitsplatz.ArbeitszeitProTagInMinuten >= 960 && Station.Arbeitsplatz.ArbeitszeitProTagInMinuten < 1200))
                                    {
                                        Station.Arbeitsplatz.ArbeitszeitProTagInMinuten =
                                        Station.Arbeitsplatz.ArbeitszeitProTagInMinuten + Zählstufe;
                                    }
                                    else if ((Station.Arbeitsplatz.ArbeitszeitProTagInMinuten == 720)
                                        || (Station.Arbeitsplatz.ArbeitszeitProTagInMinuten == 1200))
                                    {
                                        Station.Arbeitsplatz.ArbeitszeitProTagInMinuten =
                                        Station.Arbeitsplatz.ArbeitszeitProTagInMinuten + 240;
                                    }
                                    NichtErhöht = false;

                                }
                            }
                        }
                    }
                }

                SimulationPrototypDurchführung(AufträgePeriode, LagerZuBeginn, 1);
            }

            //foreach (var A in AufträgePeriode)
            //{
            //    foreach (var VARIABLE in A.TeilPrototyp.KetteStationen)
            //    {
            //        if (VARIABLE.ID == "Teil1Station1Arbeitsplatz4" || VARIABLE.ID == "Teil2Station1Arbeitsplatz4" ||
            //            VARIABLE.ID == "Teil3Station1Arbeitsplatz4")
            //        {
            //            Console.WriteLine("Station: " + VARIABLE.ID + " Rüstzeit: " + VARIABLE.Rüstzeit + " Prodzeit: " + VARIABLE.Prodzeit);
            //        }
            //    }
            //}
            GlobalVariables.KPErg.Clear();
            if (!GlobalVariables.KPErg.Columns.Contains("ID"))
            {
                GlobalVariables.KPErg.Columns.Add("ID");
                GlobalVariables.KPErg.Columns.Add("Rüstungen");
                GlobalVariables.KPErg.Columns.Add("ArbeitszeitInMinutenTag");
                GlobalVariables.KPErg.Columns["ArbeitszeitInMinutenTag"].DataType = typeof(double);
                GlobalVariables.KPErg.Columns.Add("Rüstzeit");
                GlobalVariables.KPErg.Columns.Add("Prodzeit");
            }


            foreach (var VARIABLE in Arbeitsplätze)
            {
                //Console.WriteLine("ID: " + VARIABLE.Value.ID + " Rüstungen: " + VARIABLE.Value.Rüstungen + " Arbeitszeit: " + VARIABLE.Value.ArbeitszeitProTagInMinuten
                //    + " Rüstzeit: " + VARIABLE.Value.Rüstzeit + " Prodzeit: " + VARIABLE.Value.Arbeitszeit + " Leerzeit: " + VARIABLE.Value.Leerzeit);
                double Test = (((double.Parse(VARIABLE.Value.Rüstzeit.ToString()) + double.Parse(VARIABLE.Value.Arbeitszeit.ToString()))*1.1) / 5);
                //Console.WriteLine(Test);
                GlobalVariables.KPErg.Rows.Add(VARIABLE.Value.ID, VARIABLE.Value.Rüstungen,  double.Parse(Test.ToString()), VARIABLE.Value.Rüstzeit, VARIABLE.Value.Arbeitszeit);
            }




            ////Console.WriteLine("SimuVorbei");
            //GlobalVariables.Lagerstand = Lagerstand;
            //foreach (KeyValuePair<int, int> keyValuePair in Lagerstand)
            //{
            //    Console.WriteLine("LAGER: " + keyValuePair.Key + ":" + keyValuePair.Value);
            //}
        }
    }
}
