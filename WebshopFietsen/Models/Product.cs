using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebshopFietsen.Models
{
    public class Product
    {
        public int ArtNr { get; set; }
        public string Naam { get; set; }
        public string Foto { get; set; }
        public double Aankoopprijs { get; set; }
        public double Verkoopprijs { get; set; }
        public int Voorraad { get; set; }
    }
}
