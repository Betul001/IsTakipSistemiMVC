using IsTakipSistemiMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using IsTakipSistemiMVC.Filters;

namespace IsTakipSistemiMVC.Controllers
{
    public class SistemYoneticisiController : Controller
    {
        IsTakipDBEntities entity = new IsTakipDBEntities();
        // GET: SistemYoneticisi

        [AuthFilter(3)]
        public ActionResult Index()
        {
            var birimler = (from b in entity.Birimler where b.aktiflik == true select b).ToList();

            string labelBirim = "[";

            foreach(var birim in birimler)
            {
                labelBirim += "'" + birim.birimAd + "',";
            }

            labelBirim += "]";

            ViewBag.labelBirim = labelBirim;

            List<int> islerToplam = new List<int>();

            foreach (var birim in birimler)
            {
                int toplam = 0;

                var personeller = (from p in entity.Personeller where p.personelBirimId == birim.birimId && p.aktiflik == true 
                                   select p).ToList();

                foreach (var personel in personeller)
                {
                    var isler = (from i in entity.Isler2 where i.isPersonelId == personel.personelId select i).ToList();

                    toplam += islerToplam.Count();

                }

                islerToplam.Add(toplam);

            }

            string dataIs = "[";

            foreach (var i in islerToplam)
            {
                dataIs += "'" + i + "',";
            }

            dataIs += "]";

            ViewBag.dataIs = dataIs;

            return View();
            
        }


        public ActionResult Birim()
        {

            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 3)
            {
                var birimler = (from b in entity.Birimler where b.aktiflik == true select b).ToList();

                return View(birimler);
            }

            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

        public ActionResult Olustur()
        {

            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 3)
            {
                

                return View();
            }

            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

        [HttpPost,ActFilter("Yeni Birim Eklendi")]

        public ActionResult Olustur(string birimAd)
        {
            Birimler yeniBirim = new Birimler();
            string yeniAd = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(birimAd);
            yeniBirim.birimAd = yeniAd;
            yeniBirim.aktiflik= true;

            entity.Birimler.Add(yeniBirim);
            entity.SaveChanges();

            TempData["bilgi"] = yeniBirim.birimAd;



            return RedirectToAction("Birim");
        }

        public ActionResult Guncelle(int id)
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 3)
            {

                var birim = (from b in entity.Birimler where b.birimId == id select b).FirstOrDefault();

                return View(birim);
            }

            else
            {
                return RedirectToAction("Index", "Login");
            }
        }


        [HttpPost,ActFilter("Birim Güncellendi")]

        public ActionResult Guncelle(FormCollection fc)
        {
            
            int birimId = Convert.ToInt32(fc["birimId"]);
            string yeniAd = fc["birimAd"];

            var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault();

            birim.birimAd = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(yeniAd);

            entity.SaveChanges();

            TempData["bilgi"] = birim.birimAd;

            return RedirectToAction("Birim");
        }

        [ActFilter("Birim Silindi")]
        public ActionResult Sil(int id) 
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 3)
            {

                var birim = (from b in entity.Birimler where b.birimId == id select b).FirstOrDefault();

                birim.aktiflik = false;
                entity.SaveChanges();

                TempData["bilgi"] = birim.birimAd;

                return RedirectToAction("Birim");
            }

            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

        //Bu kısıtlama filtresi yetkiturid'si 3, yani sadece sistem yöneticisinin bu sayfayı görüntülemesini sağlayan kısım.
        
        [AuthFilter(3)]
        public ActionResult Loglar()
        {

            var loglar = (from l in entity.Loglar orderby l.tarih descending select l).ToList();



            return View(loglar);
        }

    }
}