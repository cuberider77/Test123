using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace WebshopFietsen.Models
{
    public class Mailer
    {
        //Declaratie van properties
        public string Ontvanger { get; set; }
        public string Onderwerp { get; set; }
        public string Bericht { get; set; }

        public void Verstuur()
        {
            //SMTP-mail via GMail en Outlook blokkeert steeds omwille van spam-beveiliging.
            //Vanaf nu SendGrid (zie bijgevoegd Word-document voor een handleiding).

            SendGridClient client = new SendGridClient(Environment.GetEnvironmentVariable("boojah"));
            EmailAddress from = new EmailAddress("webshopfietsen@abc.xy", "Online Fietsen");
            EmailAddress to = new EmailAddress(Ontvanger, "klant");
            string onderwerp = Onderwerp;
            string bericht = Bericht;
            client.SendEmailAsync(MailHelper.CreateSingleEmail(from, to, onderwerp, bericht, ""));
        }
    }
}


