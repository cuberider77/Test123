using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebshopFietsen.Models
{
    public class VMWinkelmand
    {
        public Klant klant { get; set; }
        public WinkelmandItemRepository winkelmandItemRepository { get; set; }
        public Totalen totalen { get; set; }
    }
}
