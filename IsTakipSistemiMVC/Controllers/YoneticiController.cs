using IsTakipSistemiMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsTakipSistemiMVC.Controllers
{
    public class YoneticiController : Controller
    {
        IsTakipDBEntities entity = new IsTakipDBEntities();
        // GET: Yonetici
        public ActionResult Index()
        {
            // SADECE YÖNETİCİNİN GİRMESİ KOŞULUNU SAĞLAYAN KISIM

            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault();
                ViewBag.birimAd = birim.birimAd;

                var personeller = from p in entity.Personeller
                                  join i in entity.Isler2 on p.personelId equals i.isPersonelId into isler
                                  where p.personelBirimId == birimId && p.personelYetkiTurId != 1
                                  select new
                                  {
                                      personelAd = p.personelAdSoyad,
                                      isler = isler
                                  };

            List<ToplamIs> list = new List<ToplamIs>();

                foreach (var personel in personeller)
                {
                    ToplamIs toplamIs = new ToplamIs();
                    toplamIs.personelAdSoyad = personel.personelAd;

                    if(personel.isler.Count() == 0)
                    {
                        toplamIs.toplamIs = 0;
                    }
                    else
                    {
                        int toplam = 0;
                        foreach (var item in personel.isler)
                        {
                            if(item.yapilanTarih != null)
                            {
                                toplam++; 
                            }
                        }

                        toplamIs.toplamIs = toplam;
                    }

                    list.Add(toplamIs);
                }

                IEnumerable<ToplamIs> siraliListe = new List<ToplamIs>();
                siraliListe = list.OrderByDescending(i => i.toplamIs);

                return View(siraliListe);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Ata()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);

                var calisanlar = (from p in entity.Personeller where p.personelBirimId == birimId && p.personelYetkiTurId == 2 select p).ToList();


                ViewBag.personeller = calisanlar;

                var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault();

                ViewBag.birimAd = birim.birimAd;


                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

        [HttpPost]
        public ActionResult Ata(FormCollection formCollection)
        {
            string isBaslik = formCollection["isBaslik"];
            string isAciklama = formCollection["isAciklama"];
            int secilenPersonelId = Convert.ToInt32(formCollection["selectPer"]);
            

            Isler2 yeniIs = new Isler2();

            yeniIs.isBaslik = isBaslik;
            yeniIs.isAciklama = isAciklama;
            yeniIs.isPersonelId = secilenPersonelId;
            yeniIs.iletilenTarih = DateTime.Now;
            yeniIs.isDurumId = 1;
            yeniIs.isOkunma = false;

            entity.Isler2.Add(yeniIs);
            entity.SaveChanges();

            return RedirectToAction("Takip", "Yonetici");
        }

        public ActionResult Takip()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);

                var calisanlar = (from p in entity.Personeller where p.personelBirimId == birimId && 
                                  p.personelYetkiTurId == 2 && p.aktiflik == true select p).ToList();


                ViewBag.personeller = calisanlar;

                var birim = (from b in entity.Birimler where b.birimId == birimId select b).FirstOrDefault();

                ViewBag.birimAd = birim.birimAd;


                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

        [HttpPost]

        public ActionResult Takip(int selectPer)
        {
            var secilenPersonel = (from p in entity.Personeller where p.personelId == selectPer select p).FirstOrDefault();

            TempData["secilen"] = secilenPersonel;


            return RedirectToAction("Listele", "Yonetici");
        }

        public ActionResult Listele()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                Personeller secilenPersonel = (Personeller)TempData["secilen"];

                try
                {
                    var isler = (from i in entity.Isler2 where i.isPersonelId == secilenPersonel.personelId select i).ToList()
                    .OrderByDescending(i => i.iletilenTarih);

                    ViewBag.isler = isler;
                    ViewBag.personel = secilenPersonel;
                    ViewBag.isSayisi = isler.Count();

                    return View();
                }
                catch (Exception)
                {

                    return RedirectToAction("Takip", "Yonetici");
                }


                
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult AyinElemani() 
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1) 
            {
                int simdikiYil = DateTime.Now.Year;

                List<int> yillar = new List<int>();

                for (int i = simdikiYil; i >= 2024; i--)
                {
                    yillar.Add(i);
                }

                ViewBag.yillar = yillar;
                ViewBag.ayinElemani = null;

                return View();
            }

            else 
            {
                return RedirectToAction("Index", "Login");
            }
        
        }

        [HttpPost]

        public ActionResult AyinElemani(int aylar,int yillar)
        {
            int birimId = Convert.ToInt32(Session["PersonelBirimId"]);

            var personeller = entity.Personeller.Where(p => p.personelBirimId == birimId).Where
                (p => p.personelYetkiTurId != 1);

            DateTime baslangicTarih = Convert.ToDateTime("01-" + aylar + "-" +  yillar);
            DateTime bitisTarih = Convert.ToDateTime("31-" + aylar + "-" + yillar + "23:59:59");

            var isler = entity.Isler2.Where(i => i.yapilanTarih >= baslangicTarih).Where
                (i => i.yapilanTarih <= bitisTarih);

            var groupJoin = personeller.GroupJoin(isler,p => p.personelId,i => i.isPersonelId,(p, group) => new
            {
                sonucIsler = group,
                personelAd = p.personelAdSoyad
                
            });

            List<ToplamIs> list = new List<ToplamIs>();

            foreach (var personel in groupJoin)
            {
                ToplamIs toplamIs = new ToplamIs();

                toplamIs.personelAdSoyad = personel.personelAd;
                

                if (personel.sonucIsler.Count() == 0)
                {
                    toplamIs.toplamIs = 0;
                }
                else
                {
                    int toplam = 0;
                    foreach (var item in personel.sonucIsler)
                    {
                        if (item.yapilanTarih != null)
                        { 
                            toplam++; 
                        }
                    }

                    toplamIs.toplamIs = toplam;
                }

                list.Add(toplamIs);
            }

            IEnumerable<ToplamIs> siraliListe = new List<ToplamIs>();
            siraliListe = list.OrderByDescending(i => i.toplamIs);

            ViewBag.ayinElemani = siraliListe.FirstOrDefault();

            int simdikiYil = DateTime.Now.Year;

            List<int> sonucYillar = new List<int>();

            for (int i = simdikiYil; i >= 2024; i--)
            {
                sonucYillar.Add(i);
            }

            ViewBag.yillar = sonucYillar;



            return View();
        
        }

    }


}