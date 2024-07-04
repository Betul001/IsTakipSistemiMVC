using IsTakipSistemiMVC.Filters;
using IsTakipSistemiMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsTakipSistemiMVC.Controllers
{
    public class SifreKontrolController : Controller
    {
        IsTakipDBEntities entity = new IsTakipDBEntities();

        // GET: SifreKontrol
        public ActionResult Index()
        {

            int personelId = Convert.ToInt32(Session["PersonelId"]);
            if(personelId == 0) return RedirectToAction("Index", "Login");

            var personel = (from p in entity.Personeller where p.personelId == personelId select p).FirstOrDefault();

            ViewBag.mesaj = null;
            ViewBag.yetkiTurId = null;
            ViewBag.sitil = null;

            return View(personel);
        }

        [HttpPost, ActFilter("Parola Değiştirildi")]

        public ActionResult Index(int personelId, string eskiParola, string yeniParola, string yeniParolaKontrol)
        {
            var personel = (from p in entity.Personeller where p.personelId == personelId select p).FirstOrDefault();

            if(eskiParola != personel.personelParola)
            {
                ViewBag.mesaj = "Eski Parolanızı Yanlış Girdiniz";
                ViewBag.sitil = "alert alert-danger";

                return View(personel);
            }
            if (yeniParola != yeniParolaKontrol)
            {
                ViewBag.mesaj = "Yeni Parola ve Tekrarı Eşleşmedi";
                ViewBag.sitil = "alert alert-danger";

                return View(personel);
            }
            if (yeniParola.Length<6 || yeniParola.Length>15)
            {
                ViewBag.mesaj = "Yeni Parola En Az Altı Karakter, En Çok On Beş Karakter Olmalıdır!";
                ViewBag.sitil = "alert alert-danger";

                return View(personel);
            }

            personel.personelParola = yeniParola;
            personel.yeniPersonel = false;
            entity.SaveChanges();

            TempData["bilgi"] = personel.personelKullaniciAd;

            ViewBag.mesaj = "Parolanız Başarıyla Değiştirildi";
            ViewBag.sitil = "alert alert-success";
            ViewBag.yetkiTurId = personel.personelYetkiTurId;

            return View(personel);

            //return RedirectToAction("Index");

        }
    }
}