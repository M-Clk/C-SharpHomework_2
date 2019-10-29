using HomeClassProject;
using System.Collections.Generic;
using System.Linq;

namespace EmlakOtomasyonu
{
    internal class KullaniciManager
    {
        private static KullaniciManager instance = new KullaniciManager();
        private static List<Kullanici> kullaniciList;


        private KullaniciManager()
        {
            kullaniciList = new List<Kullanici>();
            KullaniciYukle();
        }

        public static KullaniciManager Instance { get => instance; }

        void KullaniciYukle()
        {
            Dosya dosya = Dosya.DosyaGetir("users.txt");
            string tumKullaniciString = dosya.Oku();
            string[] kullanicilar = tumKullaniciString.Split('|');
            if (kullanicilar[0].Length == 0) return;
            foreach (string item in kullanicilar)
            {
                Kullanici tempKullanici = new Kullanici(item.Split('=')[0], item.Split('=')[1]);
                kullaniciList.Add(tempKullanici);
            }
        }
        public void KullaniciSil(Kullanici kullanici)
        {
            kullaniciList.Remove(kullanici);
        }

        public void KullaniciGuncelle(Kullanici kullanici)
        {
            kullaniciList.Where(tempKullanici => tempKullanici.Equals(kullanici)).ToList()[0] = kullanici;
        }
        public bool KullaniciKontrol(string kullaniciAdi ,string sifre)
        {
           return kullaniciList.Where(tempKullanici => tempKullanici.KullaniciAdi.Equals(kullaniciAdi) && tempKullanici.Sifre.Equals(sifre)).Count() == 1;
        }
       
    }
}