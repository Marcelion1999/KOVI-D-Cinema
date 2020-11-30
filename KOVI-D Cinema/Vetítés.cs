using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOVI_D_Cinema
{
    class Vetítés
    {
        private int iD;
        private int film_ID;
        private int szek_ID;
        private Dátum datum;

        public Vetítés(string sor)
        {
            string[] adat = sor.Split(';');
            this.ID = Convert.ToInt32(adat[0]);
            this.Film_ID = Convert.ToInt32(adat[1]);
            this.Szek_ID = Convert.ToInt32(adat[2]);
            this.datum = new Dátum(adat[3]);
        }
        ~Vetítés() { }

        public int ID { get => iD; set => iD = value; }
        public int Film_ID { get => film_ID; set => film_ID = value; }
        public int Szek_ID { get => szek_ID; set => szek_ID = value; }
        internal Dátum Datum { get => datum; set => datum = value; }
    }
}
