using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebshopFietsen.Models
{
    public class VMToevoegen
    {
        //De controle van het ingevoerde aantal is vrij complex (positief, kleiner of gelijk aan voorraad).
        //Hier kijken we alleen of het een geheel getal is en of het ingevuld is. De rest volgt elders.
        [Required(ErrorMessage ="Aantal is een verplicht veld!")]
        public int? aantal { get; set; }
        
        public Product product { get; set; }
    }
}
