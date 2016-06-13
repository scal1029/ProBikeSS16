using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    class SimulationPrototyp
    {

        public void SimulationPrototypDurchführung(List<OrderPrototyp> AufträgePeriode, Dictionary<int, int> Lagerstand, int Zählstufe )
        {
            //Liste der Arbeitsplätze generieren
            Dictionary<int, Arbeitsplatzprototyp> Arbeitsplätze = new Dictionary<int, Arbeitsplatzprototyp>();
            foreach (var Order in AufträgePeriode)
            {
                foreach (ArbeitsstationPrototyp Sta in Order.TeilPrototyp.KetteStationen)
                {
                    bool containsItem = Arbeitsplätze.Any(item => item.Value.ID == Sta.Arbeitsplatz.ID);
                    if (!containsItem)
                    {
                        Arbeitsplätze.Add(Sta.Arbeitsplatz.ID,Sta.Arbeitsplatz);
                    }
                }
            }

            Console.WriteLine("SIM1: Generierte Arbeitsplätze Anzahl Arbeitsplätze: " + Arbeitsplätze.Count);



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

            Console.WriteLine("SIM2 Kopierte Anzahl Arbeitsplätze: " + Arbeitsplätze.Count);


            //Alle Werktage durchgehen
            foreach (string Wochentag in GlobalVariables.Wochentage)
            {
                //jede Arbeitsminute
                for (int i = 0; i < 1440; i=i+Zählstufe)
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
                        Console.WriteLine(AufträgePeriode[5].TeilPrototyp.KetteStationen[0].Warteschlange);
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
                                        foreach (KeyValuePair<int,int> Teile in Station.TeileProStation)
                                        {
                                            if(Station.TeileProStation.Count > 0)
                                            { 
                                                //Console.WriteLine("Ist " + (Teile.Value*10).ToString() + " größer als " + Lagerstand[Teile.Key].ToString());
                                                if ((Teile.Value*10) <= Lagerstand[Teile.Key])
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
                                                Lagerstand[Teile.Key] = Lagerstand[Teile.Key] - (Teile.Value*10);
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
                                                                               Station.Prodzeit;
                                            Station.Arbeitsplatz.Blockierzeit = Station.Arbeitsplatz.Blockierzeit +
                                                                                Station.Prodzeit;
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
                    LagerveränderungUndWarteschlangenWeitergabe(AufträgePeriode, Arbeitsplätze, Lagerstand); //+Produktion aus
                    foreach (KeyValuePair<int, Arbeitsplatzprototyp> ABP in Arbeitsplätze)
                    {
                        if(ABP.Value.Blockierzeit > 0)
                            ABP.Value.Blockierzeit = ABP.Value.Blockierzeit - 1;
                    }
                }
                Console.WriteLine(Wochentag);
                //foreach (var order in AufträgePeriode)
                //{
                //    Console.WriteLine("Artikel: " + order.Artikel + " Menge: " + order.Menge);
                //    foreach (var VARIABLE in order.TeilPrototyp.KetteStationen)
                //    {
                //        Console.WriteLine("Station: " + VARIABLE.ID + " Warteschlange: " + VARIABLE.Warteschlange + " Rüstungen: " + VARIABLE.Arbeitsplatz.Rüstungen);
                //    }
                //}
            }
            Console.WriteLine("Vor GoodCheck");
            if (CheckObLösungSchlecht(AufträgePeriode, Arbeitsplätze, Lagerstand, Zählstufe)) //Wenn schlecht dann Arbeitsplätze original Arbeitszeit erhöhen
            {
                SimulationPrototypDurchführung(GlobalVariables.OriginalProduktionsAufträgeAktuellePeriode, GlobalVariables.OriginalLagerstand, 1);
            }
            Console.WriteLine("SimuVorbei");
            GlobalVariables.Lagerstand = Lagerstand;
        }

        public void LagerveränderungUndWarteschlangenWeitergabe(List<OrderPrototyp> AufträgePeriode, Dictionary<int, Arbeitsplatzprototyp> Arbeitsplätze, Dictionary<int, int> Lagerstand)
        {
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
        }

        public bool CheckObLösungSchlecht(List<OrderPrototyp> AufträgePeriode,  Dictionary<int, Arbeitsplatzprototyp> Arbeitsplätze, Dictionary<int, int> Lagerstand, int Zählstufe)
        {
            bool Ergebnis = false;
            foreach (OrderPrototyp Order in AufträgePeriode)
            {
                foreach (ArbeitsstationPrototyp Station in Order.TeilPrototyp.KetteStationen)
                {
                    if (Station.BegründungStop != "Keine Arbeitszeit")
                    {
                        
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
                                    if ((Teile.Value*10) <= Lagerstand[Teile.Key])
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
                                        if (Station.ID == OAK.ID && Station.Warteschlange > 0)
                                        {
                                            OAK.Arbeitsplatz.ArbeitszeitProTagInMinuten =
                                                OAK.Arbeitsplatz.ArbeitszeitProTagInMinuten + Zählstufe;
                                            Arbeitsplätze[OAK.Arbeitsplatz.ID].ArbeitszeitProTagInMinuten =
                                                Arbeitsplätze[OAK.Arbeitsplatz.ID].ArbeitszeitProTagInMinuten +
                                                Zählstufe;
                                            Station.Arbeitsplatz.ArbeitszeitProTagInMinuten =
                                                Station.Arbeitsplatz.ArbeitszeitProTagInMinuten + Zählstufe;
                                            Ergebnis = true;
                                        }
                                    }
                                }
                            }
                        //}
                    }
                }
            }
            //foreach (var VARIABLE in Arbeitsplätze)
            //{
            //    Console.WriteLine(VARIABLE.Value.ArbeitszeitProTagInMinuten);
            //}


            //REIHENFOLGE!!!!
            //Console.WriteLine("Interlude2");
            //for (int i = 0; i < 14; i++)
            //{
            //    GlobalVariables.OriginalAlleArbeitsplätze[i].ArbeitszeitProTagInMinuten = Arbeitsplätze[i].ArbeitszeitProTagInMinuten;
            //}

            return Ergebnis;
        }


       

    }
}
