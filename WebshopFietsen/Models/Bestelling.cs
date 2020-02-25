using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebshopFietsen.Models
{
    public class Bestelling
    {
        //Om twee kenmerken van de bestelling te binden aan de view Bevestiging
        public int OrderId { get; set; }
        public double Bedrag { get; set; }
    }
}
