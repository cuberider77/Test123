using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebshopFietsen.Models
{
    public class WinkelmandItem
    {
        public int KlantNr { get; set; }
        public int ArtNr { get; set; }
        public string Naam { get; set; }
        public int Aantal { get; set; }
        public string Foto { get; set; }
        public double Verkoopprijs { get; set; }
        public double Totaal { get; set; }
    }
}
