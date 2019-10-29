namespace EmlakOtomasyonu
{
    internal class Kullanici
    {
        private string kullaniciAdi;
        private string sifre;

        public Kullanici(string kullaniciAdi, string sifre)
        {
            KullaniciAdi = kullaniciAdi;
            Sifre = sifre;
        }
        public string KullaniciAdi {
            get => kullaniciAdi;
            set
            {
                value = value.Replace("|", "");
                kullaniciAdi = value.Replace("=", "");
            }
        }
        public string Sifre {
            get => sifre;
            set {
                value = value.Replace("|", "");
                sifre = value.Replace("=", "");
            }
        }
    }
}