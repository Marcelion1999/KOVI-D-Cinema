using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOVI_D_Cinema
{
    class Dátum
    {
        private int év;
        private int hónap;
        private int nap;
        private int óra;
        private string s_date;

        public Dátum(string sor)
        {
            string[] adat = sor.Split('-');
            this.Év = Convert.ToInt32(adat[0]);
            this.Hónap = Convert.ToInt32(adat[1]);
            this.Nap = Convert.ToInt32(adat[2]);
            this.Óra = Convert.ToInt32(adat[3]);
            this.s_date = String.Format("{0}-{1}-{2} : {3}", this.Év, this.Hónap, this.Nap, this.Óra);
        }

        public int Év { get => év; set => év = value; }
        public int Hónap { get => hónap; set => hónap = value; }
        public int Nap { get => nap; set => nap = value; }
        public int Óra { get => óra; set => óra = value; }
        public string S_date { get => s_date; set => s_date = value; }
    }
}
