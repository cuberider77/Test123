using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebshopFietsen.Models;
using WebshopFietsen.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace WebshopFietsen.Controllers
{
    public class HomeController : Controller
    {
        PersistenceCode persistenceCode = new PersistenceCode();

        [Authorize]
        public IActionResult Index(ProductRepository productRepository)
        {
            //Even een vast klantid, achteraf aanpassen met login-systeem:
            HttpContext.Session.SetString("klant", User.Identity.Name);

            productRepository.Producten = persistenceCode.loadProducten();
            return View(productRepository);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Index()
        {
            return RedirectToAction("Winkelmand");
        }

        [Authorize]
        public IActionResult Toevoegen(int artnr)
        {
            VMToevoegen vmToevoegen = new VMToevoegen();
            vmToevoegen.product = persistenceCode.loadProduct(artnr);
            //Na het tonen van onderstaande view verdwijnt de instance van vmToevoegen
            //en dus ook zijn property Product en zijn we het artnr kwijt waarover het gaat.
            //De enige property die na de post bekend zal zijn is aantal (via asp-for). 
            //We hebben echter het artnr ook nog nodig bij de post, dus tijdelijk in session zetten.
            HttpContext.Session.SetString("artnr", Convert.ToString(artnr));
            return View(vmToevoegen);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Toevoegen(VMToevoegen vmToevoegen)
        {
            //Eerst laatste nieuwe info over product ophalen
            int artnr = Convert.ToInt32(HttpContext.Session.GetString("artnr"));
            vmToevoegen.product = persistenceCode.loadProduct(artnr);

            if (ModelState.IsValid)
            {
                if (vmToevoegen.aantal > 0)
                {
                    if (vmToevoegen.aantal <= vmToevoegen.product.Voorraad)
                    {
                        WinkelmandItem wmitem = new WinkelmandItem();
                        wmitem.ArtNr = artnr;
                        wmitem.KlantNr = Convert.ToInt32(HttpContext.Session.GetString("klant"));
                        wmitem.Aantal = Convert.ToInt32(vmToevoegen.aantal);
                        persistenceCode.updateWinkelmand(wmitem);
                        return RedirectToAction("Winkelmand");
                    }
                    else
                    {
                        ViewBag.fout = "Onvoldoende voorraad!";
                        return View(vmToevoegen);
                    }
                }
                else
                {
                    ViewBag.fout = "Geen positief aantal!";
                    return View(vmToevoegen);
                }
            }
            else
            {
                return View(vmToevoegen);
            }
        }

        [Authorize]
        public IActionResult Winkelmand(VMWinkelmand vmWinkelmand)
        {
            int klantnr = Convert.ToInt32(HttpContext.Session.GetString("klant"));
            
            Klant klant = persistenceCode.loadKlant(klantnr);
            WinkelmandItemRepository wmirepo = new WinkelmandItemRepository();
            wmirepo.WinkelmandItems = persistenceCode.loadWinkelmandItems(klantnr);
            Totalen totalen = persistenceCode.loadTotalen(klantnr);

            vmWinkelmand.klant = klant;
            vmWinkelmand.totalen = totalen;
            vmWinkelmand.winkelmandItemRepository = wmirepo;

            return View(vmWinkelmand);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Winkelmand()
        {
            int klantnr = Convert.ToInt32(HttpContext.Session.GetString("klant"));
            Bestelling bestelling = persistenceCode.insertBestelling(klantnr);
            return RedirectToAction("Bevestiging", bestelling);
        }

        [Authorize]
        public IActionResult Verwijderen(int artnr, int aantal)
        {
            int klantnr = Convert.ToInt32(HttpContext.Session.GetString("klant"));
            WinkelmandItem wmi = new WinkelmandItem();
            wmi.KlantNr = klantnr;
            wmi.ArtNr = artnr;
            wmi.Aantal = aantal;
            persistenceCode.deleteWinkelmanditem(wmi);
            return RedirectToAction("Winkelmand");
         }

        [Authorize]
        public IActionResult Bevestiging(Bestelling bestelling)
        {
            Mailer mailer = new Mailer();
            int klantnr = Convert.ToInt32(HttpContext.Session.GetString("klant"));
            mailer.Ontvanger = persistenceCode.loadKlant(klantnr).Email;
            mailer.Onderwerp = "Bevestiging bestelling nr. " + bestelling.OrderId;
            mailer.Bericht = "We hebben uw bestelling met nr. " + bestelling.OrderId + " goed ontvangen." + Environment.NewLine +
                    "Gelieve het bedrag van € " + Math.Round(bestelling.Bedrag, 2) + " over te schrijven op rekeningnummer BE12 3456 7890 1212." + Environment.NewLine +
                    "Bedankt voor uw vertrouwen!" + Environment.NewLine +
                    "L. Nassen";
            mailer.Verstuur();

            return View(bestelling);
        }
    }
}
