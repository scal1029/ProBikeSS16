using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    [Serializable]
    public class Arbeitsplatzprototyp
    {
        //public int ArbeitsplatzID;
        //public int bereitsGearbeitet;
        //public int RüstZeit;
        //public int ProZeit;
        //public int ArtikelID;
        //public int Platzierung;
        //public int maxPlatz;
        //public Dictionary<int, int> TeileInStuffe;
        //public int RüstArtikelID;
        //public int EigeneWarteSchlange;
        //public int BlockZeit;
        //public Arbeitsplatzprototyp Next;

        //public Arbeitsplatzprototyp(int AID, int RZ, int PZ, int Pl, int MaxPL,
        //    Dictionary<int, int> Teile, Arbeitsplatzprototyp _next)
        //{
        //    ArbeitsplatzID = AID;
        //    bereitsGearbeitet = 0;
        //    RüstZeit = RZ;
        //    ProZeit = PZ;
        //    Platzierung = Pl;
        //    maxPlatz = MaxPL;
        //    TeileInStuffe = Teile;
        //    Next = _next;
        //}

        //public void ArbeitDone()
        //{
        //    EigeneWarteSchlange = EigeneWarteSchlange - 10;
        //}

        //public void ArbeitGet()
        //{
        //    EigeneWarteSchlange = EigeneWarteSchlange + 10;
        //}

        //public void Rüst(int rüst)
        //{
        //    RüstArtikelID = rüst;
        //    BlockZeit = BlockZeit + RüstZeit;
        //}

        //public void Produz()
        //{
        //    BlockZeit = BlockZeit + RüstZeit*10;
        //}
        public int ID;
        public int Rüstungen;
        public int Rüstzeit;
        public int Leerzeit;
        public int Arbeitszeit;
        public int ArbeitszeitProTagInMinuten;
        public int Blockierzeit;
        public int RüstID;

        public Arbeitsplatzprototyp(int _ID)
        {
            ID = _ID;
            Rüstungen = 0;
            Rüstzeit = 0;
            Leerzeit = 0;
            Arbeitszeit = 0;
            ArbeitszeitProTagInMinuten = 480;
            Blockierzeit = 0;
            RüstID = 0;
        }
    }
}
