using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfTilausApp
{
    public class TilausOtsikko
    {
        private DateTime tilausPvm; //kenttä
        private DateTime toimitusPvm;
        public int TilausNumero { get; set; } //ominaisuus
        public int AsiakasNumero { get; set; }
        public string ToimitusOsoite { get; set; }
        public string Postinumero { get; set; }

        public int PostiID { get; set; }
        public DateTime TilausPvm //tarkastus tehty ominaisuuden asetuksessa
        {
            get
            {
                return tilausPvm;
            }
            set
            {
                if (value <= DateTime.Today)
                {
                    tilausPvm = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Tilauspvm ei voi olla tulevaisuudessa!");
                }
            }
        }

        public DateTime ToimitusPvm
        {
            get
            {
                return toimitusPvm;
            }
            set
            {
                if (value > DateTime.Today)
                {
                    toimitusPvm = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("ToimitusPvm ei voi olla menneisyydessä!");
                }
            }
        }

        internal string LaskeToimitusAika()
        {
            TimeSpan erotus;
            erotus = toimitusPvm.Subtract(tilausPvm); //voidaan viitata ominaisuuteen tai kenttään, huom! kenttä pienellä
            return erotus.Days.ToString();
        }
    }
}
