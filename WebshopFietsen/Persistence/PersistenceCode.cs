using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebshopFietsen.Models;
using MySql.Data.MySqlClient;

namespace WebshopFietsen.Persistence
{
    public class PersistenceCode
    {
        string connStr = "server=localhost; user id=root; password=Test123; database=dbwebshop";

        public List<Product> loadProducten()
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select artnr, naam, foto, verkoopprijs, voorraad from tblproducten";
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            List<Product> lijst = new List<Product>();
            while (dtr.Read())
            {
                Product p = new Product();
                p.ArtNr = Convert.ToInt32(dtr["artnr"]);
                p.Naam = Convert.ToString(dtr["naam"]);
                p.Foto = Convert.ToString(dtr["foto"]);
                p.Verkoopprijs = Convert.ToDouble(dtr["verkoopprijs"]);
                p.Voorraad = Convert.ToInt32(dtr["voorraad"]);
                lijst.Add(p);
            }
            conn.Close();
            return lijst;
        }

        public Product loadProduct(int artnr)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select artnr, naam, foto, verkoopprijs, voorraad from tblproducten where artnr=" + artnr;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            Product p = new Product();
            while (dtr.Read())
            {
                p.ArtNr = Convert.ToInt32(dtr["artnr"]);
                p.Naam = Convert.ToString(dtr["naam"]);
                p.Foto = Convert.ToString(dtr["foto"]);
                p.Verkoopprijs = Convert.ToDouble(dtr["verkoopprijs"]);
                p.Voorraad = Convert.ToInt32(dtr["voorraad"]);
            }
            conn.Close();
            return p;
        }

        //Een item toevoegen/aanpassen in het winkelmandje
        public void updateWinkelmand(WinkelmandItem wmitem)
        {
            //Controle of het item al in het mandje zit
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select * from tblwinkelmandje where klantnr=" + wmitem.KlantNr + " and artnr=" + wmitem.ArtNr;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            if (dtr.HasRows)
            {
                //Het aantal toevoegen aan het reeds bestaande aantal
                MySqlConnection conn1 = new MySqlConnection(connStr);
                conn1.Open();
                string qry1 = "update tblwinkelmandje set aantal=aantal + " + wmitem.Aantal +
                    " where klantnr=" + wmitem.KlantNr + " and artnr=" + wmitem.ArtNr;
                MySqlCommand cmd1 = new MySqlCommand(qry1, conn1);
                cmd1.ExecuteNonQuery();
                conn1.Close();
            }
            else
            {
                //Een nieuw record toevoegen aan het winkelmandje
                MySqlConnection conn2 = new MySqlConnection(connStr);
                conn2.Open();
                string qry2 = "insert into tblwinkelmandje (klantnr, artnr, aantal) values " +
                    "(" + wmitem.KlantNr + "," + wmitem.ArtNr + "," + wmitem.Aantal + ")";
                MySqlCommand cmd2 = new MySqlCommand(qry2, conn2);
                cmd2.ExecuteNonQuery();
                conn2.Close();
            }
            conn.Close();

            //Voorraad afboeken in tblproducten
            MySqlConnection conn3 = new MySqlConnection(connStr);
            conn3.Open();
            string qry3 = "update tblproducten set voorraad = voorraad - " + wmitem.Aantal +
                        " where artnr=" + wmitem.ArtNr;
            MySqlCommand cmd3 = new MySqlCommand(qry3, conn3);
            cmd3.ExecuteNonQuery();
            conn3.Close();
        }

        public Klant loadKlant(int klantnr)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select * from tblklanten where klantnr=" + klantnr;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            Klant k = new Klant();
            while (dtr.Read())
            {
                k.KlantNr = Convert.ToInt32(dtr["klantnr"]);
                k.Naam = Convert.ToString(dtr["Naam"]);
                k.Voornaam = Convert.ToString(dtr["Voornaam"]);
                k.Adres = Convert.ToString(dtr["Adres"]);
                k.PC = Convert.ToString(dtr["PC"]);
                k.Gemeente = Convert.ToString(dtr["Gemeente"]);
                k.Email = Convert.ToString(dtr["email"]);
            }
            conn.Close();
            return k;
        }

        public List<WinkelmandItem> loadWinkelmandItems(int klantnr)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select tblproducten.artnr, naam, foto, verkoopprijs, aantal, aantal*verkoopprijs as subtotaal " +
                "from tblproducten inner join tblwinkelmandje on tblproducten.artnr=tblwinkelmandje.artnr " +
                "where klantnr=" + klantnr;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            List<WinkelmandItem> lijst = new List<WinkelmandItem>();
            while (dtr.Read())
            {
                WinkelmandItem wmitem = new WinkelmandItem();
                wmitem.KlantNr = klantnr;
                wmitem.ArtNr = Convert.ToInt32(dtr["artnr"]);
                wmitem.Naam = Convert.ToString(dtr["naam"]);
                wmitem.Aantal = Convert.ToInt32(dtr["aantal"]);
                wmitem.Foto = Convert.ToString(dtr["foto"]);
                wmitem.Verkoopprijs = Convert.ToDouble(dtr["verkoopprijs"]);
                wmitem.Totaal = Convert.ToDouble(dtr["subtotaal"]);
                lijst.Add(wmitem);
            }
            conn.Close();
            return lijst;
        }

        public Totalen loadTotalen(int klantnr)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select sum(aantal*verkoopprijs) as excl, sum(aantal*verkoopprijs)*0.21 as btw, " +
                "sum(aantal*verkoopprijs)*1.21 as incl " +
                "from tblproducten inner join tblwinkelmandje on tblproducten.artnr=tblwinkelmandje.artnr " +
                "where klantnr=" + klantnr;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            Totalen totalen = new Totalen();
            while (dtr.Read())
            {
                double uit;
                if (double.TryParse(dtr["excl"].ToString(), out uit))
                {
                    totalen.ExclBTW = Convert.ToDouble(dtr["excl"]); ;
                    totalen.BTW = Convert.ToDouble(dtr["btw"]);
                    totalen.InclBTW = Convert.ToDouble(dtr["incl"]);
                }
                else
                {
                    totalen.ExclBTW = 0;
                    totalen.BTW = 0;
                    totalen.InclBTW = 0;
                }
            }
            conn.Close();
            return totalen;
        }

        public void deleteWinkelmanditem(WinkelmandItem winkelmandItem)
        {
            //We verwijderen het product voor de betreffende klant uit het winkelmandje.
            MySqlConnection conn1 = new MySqlConnection(connStr);
            conn1.Open();
            string qry1 = "delete from tblwinkelmandje where artnr=" + winkelmandItem.ArtNr + " and klantnr=" + winkelmandItem.KlantNr;
            MySqlCommand cmd1 = new MySqlCommand(qry1, conn1);
            cmd1.ExecuteNonQuery();
            conn1.Close();

            //We schrijven het aantal terug bij bij de voorraad van het betreffende product.
            MySqlConnection conn2 = new MySqlConnection(connStr);
            conn2.Open();
            string qry2 = "update tblproducten set voorraad=voorraad + " + winkelmandItem.Aantal + " where artnr=" + winkelmandItem.ArtNr;
            MySqlCommand cmd2 = new MySqlCommand(qry2, conn2);
            cmd2.ExecuteNonQuery();
            conn2.Close();
        }

        public Bestelling insertBestelling(int klantnr)
        {
            //Een nieuwe bestelling aanmaken
            MySqlConnection conn1 = new MySqlConnection(connStr);
            conn1.Open();
            string qry1 = "insert into tblbestelling (orderdatum, klantnr) values ('" + DateTime.Today.ToString("yyyy-MM-dd") +
                "', " + klantnr + ")";
            MySqlCommand cmd1 = new MySqlCommand(qry1, conn1);
            cmd1.ExecuteNonQuery();
            conn1.Close();

            //Het zonet gegenereerde ordernummer ophalen om toe te voegen aan de orderlijnen
            MySqlConnection conn2 = new MySqlConnection(connStr);
            conn2.Open();
            string qry2 = "select max(ordernr) as laatste from tblbestelling where klantnr=" + klantnr;
            MySqlCommand cmd2 = new MySqlCommand(qry2, conn2);
            MySqlDataReader dtr2 = cmd2.ExecuteReader();
            int ordernr = 0;
            while (dtr2.Read())
            {
                ordernr = Convert.ToInt32(dtr2["laatste"]);
            }
            conn2.Close();

            //Alle winkelmanditems ophalen
            MySqlConnection conn3 = new MySqlConnection(connStr);
            conn3.Open();
            string qry3 = "select * from tblwinkelmandje where klantnr=" + klantnr;
            MySqlCommand cmd3 = new MySqlCommand(qry3, conn3);
            MySqlDataReader dtr3 = cmd3.ExecuteReader();
            List<WinkelmandItem> lijst = new List<WinkelmandItem>();
            while (dtr3.Read())
            {
                WinkelmandItem wmi = new WinkelmandItem();
                wmi.ArtNr = Convert.ToInt32(dtr3["artnr"]);
                wmi.Aantal = Convert.ToInt32(dtr3["aantal"]);
                lijst.Add(wmi);
            }
            conn3.Close();

            //Van elk winkelmanditem in de lijst een orderlijn maken
            MySqlConnection conn4 = new MySqlConnection(connStr);
            conn4.Open();
            foreach (var wmi in lijst)
            {
                string qry4 = "insert into tblbestellingdetails (ordernr, aantal, artnr) values (" +
                               ordernr + "," + wmi.Aantal + "," + wmi.ArtNr + ")";
                MySqlCommand cmd4 = new MySqlCommand(qry4, conn4);
                cmd4.ExecuteNonQuery();
            }
            conn4.Close();

            //een object van het model Bestelling aanmaken om terug te sturen
            Bestelling bestelling = new Bestelling();
            bestelling.OrderId = ordernr;
            bestelling.Bedrag = loadTotalen(klantnr).InclBTW;


            //Het winkelmandje leegmaken
            MySqlConnection conn5 = new MySqlConnection(connStr);
            conn5.Open();
            string qry5 = "delete from tblwinkelmandje where klantnr=" + klantnr;
            MySqlCommand cmd5 = new MySqlCommand(qry5, conn5);
            cmd5.ExecuteNonQuery();
            conn5.Close();

            return bestelling;
        }

        public int checkLoginCredentials(LoginCredentials lc)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string qry = "select klantnr from tblklanten where gebrnaam='" + lc.Gebruikersnaam + "' and binary wachtwoord='" + lc.Wachtwoord + "'";
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            int usrID = -1;
            while (dtr.Read())
            {
                usrID = Convert.ToInt32(dtr["klantnr"]);
            }
            conn.Close();
            return usrID;
        }
    }
}
