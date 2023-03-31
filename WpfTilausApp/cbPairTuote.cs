using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfTilausApp
{
    public class cbPairTuote
    {
        public string tuoteNimi { get; set; }
        public int tuoteNro { get; set; }
        public decimal aHinta { get; set; }

      

        public cbPairTuote(string TuoteNimi, int TuoteNro, decimal AHinta)
        {
            tuoteNimi = TuoteNimi;
            tuoteNro = TuoteNro;
            aHinta = AHinta;
        }
    }
}
