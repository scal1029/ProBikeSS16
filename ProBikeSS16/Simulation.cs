using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    class Simulation
    {
        public bool SimuVersuch3(List<OrderPrototyp> AufträgePeriode, Dictionary<int, int> Lagerstand, ref Dictionary<int, Arbeitsplatzprototyp> ABP )
        {
            bool SimuErfolg = false;
            


            //1. Simulation durchführen
            for (int Woche = 1; Woche < 5; Woche++)
            {
                for (int Arbeitsminute = 0; Arbeitsminute <= 1440; Arbeitsminute++)
                {
                    foreach (var ProdAuftrag in AufträgePeriode)
                    {
                        foreach (var StationDesAuftrages in ProdAuftrag.TeilPrototyp.KetteStationen)
                        {
                            if (!StationDesAuftrages.Done && !StationDesAuftrages.Produziert &&
                                StationDesAuftrages.Arbeitsplatz.Blockierzeit == 0 && StationDesAuftrages.Arbeitsplatz.ArbeitszeitProTagInMinuten >= Arbeitsminute)
                            {
                                
                            }

                        }
                    }
                }
            }

            SimuErfolg = true;

            //5. angeben on Simu erfolgreich
            return SimuErfolg;
        }
    }
}
