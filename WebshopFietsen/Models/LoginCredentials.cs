using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebshopFietsen.Models
{
    public class LoginCredentials
    {
        [Required(ErrorMessage ="Vul uw gebruikersnaam in!")]
        public string Gebruikersnaam { get; set; }

        [Required(ErrorMessage ="Vul uw wachtwoord in!")]
        public string Wachtwoord { get; set; }

        public int ID { get; set; }
    }
}
