using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebshopFietsen.Models
{
    public class Klant
    {
        public int KlantNr { get; set; }
        public string Naam { get; set; }
        public string Voornaam { get; set; }
        public string Adres { get; set; }
        public string PC { get; set; }
        public string Gemeente { get; set; }
        public string Email { get; set; }
    }
}
