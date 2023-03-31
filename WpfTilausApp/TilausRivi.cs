using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfTilausApp
{
    public class TilausRivi
    {
        public int TilausNumero { get; set; }
        public int TilausRiviNumero { get; set; }
        public int TuoteNumero { get; set; }
        public string TuoteNimi { get; set; }
        public int Maara { get; set; }
        public decimal AHinta { get; set; }    
        public decimal Summa { get; set; }

        //public string SSumma { get; set; }
        //public string SHinta { get; set; }


        public decimal RiviSumma()
        {
            Summa = AHinta * Maara;
            return Summa;
        }

        //public string SRiviSumma()
        //{
        //    SSumma = SHinta * Maara;
        //    return SSumma;
        //}

    }
}
