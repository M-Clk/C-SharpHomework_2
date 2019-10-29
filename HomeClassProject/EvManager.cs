using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeClassProject
{

    public class EvManager
    {
        private static readonly EvManager instance = new EvManager();
        public static List<Ev> evList;

        private EvManager() //Static siniflarda kurucu metot yazilamadigindan singleton kullandik
        {
            evList = new List<Ev>(1000);//Kapasitesi 1000 olarak istenildi
            DosyadanKiralikEvYukle();
            DosyadanSatilikEvYukle();
        }

        public static EvManager Instance { get => instance; }//Singleton

        private void DosyadanKiralikEvYukle()
        {
            Dosya dosya = Dosya.DosyaGetir("kiralik.txt");
            string dosyaSonucu = dosya.Oku();
            string[] evler = dosyaSonucu.Split('&');
            if (evler[0].Length == 0) return;
            foreach (string evBilgisi in evler)
            {
                string[] ozellikler = evBilgisi.Split('|');

                int emlakNumarasi = int.Parse(ozellikler[0].Substring(ozellikler[0].LastIndexOf('=') + 1));
                decimal kira = decimal.Parse(ozellikler[8].Substring(ozellikler[8].LastIndexOf('=') + 1));
                decimal depozito = decimal.Parse(ozellikler[9].Substring(ozellikler[9].LastIndexOf('=') + 1));

                KiralikEv kiralikEv = (KiralikEv)EvOlustur(emlakNumarasi, kira, depozito);
                kiralikEv = (KiralikEv)OrtakEvBilgileriGir(ozellikler, kiralikEv);
            }
        }

        private void DosyadanSatilikEvYukle()
        {
            Dosya dosya = Dosya.DosyaGetir("satilik.txt");
            string dosyaSonucu = dosya.Oku();
            string[] evler = dosyaSonucu.Split('&');
            if (evler[0].Length == 0) return;
            foreach (string evBilgisi in evler)
            {
                string[] ozellikler = evBilgisi.Split('|');

                int emlakNumarasi = int.Parse(ozellikler[0].Substring(ozellikler[0].LastIndexOf('=') + 1));
                decimal fiyat = decimal.Parse(ozellikler[8].Substring(ozellikler[8].LastIndexOf('=') + 1));

                SatilikEv satilikEv = (SatilikEv)EvOlustur(emlakNumarasi, fiyat);
                satilikEv = (SatilikEv)OrtakEvBilgileriGir(ozellikler, satilikEv);
            }
        }

        public Ev OrtakEvBilgileriGir(string[] siraliOzellikler, Ev ev)
        {
            ev.OdaSayisi = int.Parse(siraliOzellikler[1].Substring(siraliOzellikler[1].LastIndexOf('=') + 1));
            ev.KatNumarasi = int.Parse(siraliOzellikler[2].Substring(siraliOzellikler[2].LastIndexOf('=') + 1));
            ev.Alani = decimal.Parse(siraliOzellikler[3].Substring(siraliOzellikler[3].LastIndexOf('=') + 1));
            ev.YapimTarihi = DateTime.Parse(siraliOzellikler[4].Substring(siraliOzellikler[4].LastIndexOf('=') + 1));
            ev.Semti = siraliOzellikler[5].Substring(siraliOzellikler[5].LastIndexOf('=') + 1);
            ev.Turu = (Tur)Enum.Parse(typeof(Tur), siraliOzellikler[6].Substring(siraliOzellikler[6].LastIndexOf('=') + 1));
            ev.Aktif = bool.Parse(siraliOzellikler[7].Substring(siraliOzellikler[7].LastIndexOf('=') + 1));
            return ev;
        }
        public Ev OrtakEvBilgileriGir(int odaSayisi, int katNumarasi, decimal alani, DateTime yapimTarihi, string semti, Tur turu, bool aktif , Ev ev)
        {
            ev.OdaSayisi = odaSayisi;
            ev.KatNumarasi = katNumarasi;
            ev.Alani = alani;
            ev.YapimTarihi = yapimTarihi;
            ev.Semti = semti;
            ev.Turu = turu;
            ev.Aktif = aktif;
            return ev;
        }

        public Ev EvOlustur(int emlakNumarasi, decimal fiyat)
        {
            if (EmlakNumarasiKontrol(emlakNumarasi))
                return null;
            else
            {
                SatilikEv newEv = new SatilikEv(emlakNumarasi, fiyat);
                evList.Add(newEv);
                return newEv;
            }
        }

        public Ev EvOlustur(int emlakNumarasi, decimal kira, decimal depozito)
        {
            if (EmlakNumarasiKontrol(emlakNumarasi))
                return null;
            else
            {
                KiralikEv newEv = new KiralikEv(emlakNumarasi, kira, depozito);
                evList.Add(newEv);
                return newEv;
            }
        }

        public bool EmlakNumarasiKontrol(int emlakNumarasi)
        {
            var sonuc = evList.Where(tempEv => tempEv.EmlakNumarasi == emlakNumarasi);
            if (sonuc.Count() > 0) return true;
            else return false;
        }
        public Ev EvGetir(int emlakNumarasi)
        {
           List<Ev> sonuc = evList.Where(tempEv => tempEv.EmlakNumarasi == emlakNumarasi).ToList();
            return sonuc[0];
        }
        public void EvleriKaydet()
        {
            List<Ev> evs = SatilikEvleriGetir();
            Dosya dosya = Dosya.DosyaGetir("satilik.txt");

            dosya.Guncelle(string.Join("&", evs.Select(ev => ev.EvBilgileri()).ToArray()));

            evs = KiralikEvleriGetir();
            dosya = Dosya.DosyaGetir("kiralik.txt");

            dosya.Guncelle(string.Join("&", evs.Select(ev => ev.EvBilgileri()).ToArray()));
        }

        public List<Ev> EvListele()
        {
            return evList;
        }
        public List<Ev> KiralikEvleriGetir()
        {
            return evList.Where(tempEv => tempEv is KiralikEv).ToList();
        }
        public List<Ev> SatilikEvleriGetir()
        {
            return evList.Where(tempEv => tempEv is SatilikEv).ToList();
        }
        public List<Ev> EvListele(bool aktiflik)
        {
            return evList.Where(tempEv => tempEv.Aktif == aktiflik).ToList();
        }

        public List<Ev> EvListele(Tur turu)
        {
            return evList.Where(tempEv => tempEv.Turu == turu).ToList();
        }

        public List<Ev> EvListele(int odaSayisi)
        {
            return evList.Where(tempEv => tempEv.OdaSayisi == odaSayisi).ToList();
        }

        public void EvGuncelle(Ev ev)
        {
            evList.Where(tempEv => tempEv.Equals(ev)).ToList()[0] = ev;
        }

        public void EvSil(Ev ev)
        {
            evList.Remove(ev);
        }
        public void EvArsivle(Ev ev)
        {
            ev.Aktif = false;
        }

    }
}