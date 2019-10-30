using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeClassProject
{

    public class EvManager
    {
        private static readonly EvManager instance = new EvManager();
        private static List<Ev> evList;

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

        public List<Ev> EvFiltrele(DateTime dateEnAz, DateTime dateEnFazla, decimal AlanEnAz, int odaSayisiEnAz, string ilAdi, decimal fiyatOrKiraEnAz, decimal fiyatOrKiraEnFazla, bool aktif, bool kiralikMi)
        {
            var list = kiralikMi ? KiralikEvleriGetir() : SatilikEvleriGetir(); 
              var  filtreliList = list.Where(tempEv =>
                FiltreleTarihiEnAzSuOlan(tempEv, dateEnAz))
                .Where(tempEv => FiltreleTarihiEnFazlaSuOlan(tempEv, dateEnFazla))
                .Where(tempEv => FiltreleAlaniEnAzSuOlan(tempEv, AlanEnAz))
                .Where(tempEv => FiltreleOdaSayisiEnAzSuOlan(tempEv, odaSayisiEnAz))
                .Where(tempEv => FiltreleIliSuOlan(tempEv, ilAdi))
                .Where(tempEv => (tempEv is KiralikEv) ? FiltreleKiraEnAzSuOlan(tempEv, fiyatOrKiraEnAz) : FiltreleFiyatEnAzSuOlan(tempEv, fiyatOrKiraEnAz))
                .Where(tempEv => (tempEv is KiralikEv) ? FiltreleKiraEnFazlaSuOlan(tempEv, fiyatOrKiraEnFazla) : FiltreleFiyatEnFazlaSuOlan(tempEv, fiyatOrKiraEnFazla))
                .Where(tempEv=> tempEv.Aktif==aktif)
                .ToList();
            return filtreliList;
        }

        private List<Ev> KiralikEvleriGetir()
        {
            return evList.Where(tempEv => tempEv is KiralikEv).ToList();
        }
        private List<Ev> SatilikEvleriGetir()
        {
            return evList.Where(tempEv => tempEv is SatilikEv).ToList();
        }

        private bool FiltreleTarihiEnAzSuOlan(Ev ev, DateTime date)
        {
            if (date == default(DateTime))
                return true;
            else 
                return ev.YapimTarihi >= date;
        }
        private bool FiltreleTarihiEnFazlaSuOlan(Ev ev, DateTime date)
        {
            if (date == default(DateTime))
                return true;
            else
                return ev.YapimTarihi <= date;
        }
        private bool FiltreleAlaniEnAzSuOlan(Ev ev, decimal alan)
        {
            if (alan == -1)
                return true;
            else
                return ev.Alani >= alan;
        }
        private bool FiltreleOdaSayisiEnAzSuOlan(Ev ev, int odaSayisi)
        {
            if (odaSayisi == -1)
                return true;
            else
                return ev.OdaSayisi >= odaSayisi;
        }
        private bool FiltreleIliSuOlan(Ev ev, string ilAdi)
        {
            if (ilAdi == null)
                return true;
            else
            {
                string il=  ev.Semti.Split(new string[] { ", " }, StringSplitOptions.None)[1];
                return il.Equals(ilAdi);
            }
        }
        private bool FiltreleFiyatEnAzSuOlan(Ev ev, decimal fiyat)
        {
            if (fiyat == -1)
                return true;
            else
                return ((SatilikEv)ev).Fiyat >= fiyat;
        }
        private bool FiltreleFiyatEnFazlaSuOlan(Ev ev, decimal fiyat)
        {
            if (fiyat == -1)
                return true;
            else
                return ((SatilikEv)ev).Fiyat <= fiyat;
        }
        private bool FiltreleKiraEnAzSuOlan(Ev ev, decimal kira)
        {
            if (kira == -1)
                return true;
            else
                return ((KiralikEv)ev).Kira >= kira;
        }
        private bool FiltreleKiraEnFazlaSuOlan(Ev ev, decimal kira)
        {
            if (kira == -1)
                return true;
            else
                return ((KiralikEv)ev).Kira <= kira;
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