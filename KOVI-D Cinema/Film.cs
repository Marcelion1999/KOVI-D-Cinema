using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOVI_D_Cinema
{
    class Film
    {
        private int film_ID;
        private string név;

        public Film(string sor)
        {
            string[] adat = sor.Split(';');
            this.Film_ID = Convert.ToInt32(adat[0]);
            this.Név = adat[1];
        }

        public int Film_ID { get => film_ID; set => film_ID = value; }
        public string Név { get => név; set => név = value; }
    }
}
