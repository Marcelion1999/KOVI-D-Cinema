using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOVI_D_Cinema
{
    class Szék
    {
        private int vetítés_ID;
        private int sor;
        private int oszlop;
        private char típus;
        public Szék(string sor)
        {
            string[] adat = sor.Split(';');
            this.vetítés_ID = Convert.ToInt32(adat[0]);
            this.sor = Convert.ToInt32(adat[1]);
            this.oszlop = Convert.ToInt32(adat[2]);
        }

        public int Vetítés_ID { get => vetítés_ID; set => vetítés_ID = value; }
        public int Sor { get => sor; set => sor = value; }
        public int Oszlop { get => oszlop; set => oszlop = value; }
        public char Típus { get => típus; set => típus = value; }
    }
}
