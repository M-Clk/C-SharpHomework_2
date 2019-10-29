using System;

namespace HomeClassProject
{
    public abstract class Ev
    {
        private decimal alani;
        private int odaSayisi;
        private int katNumarasi;
        private string semti;
        private DateTime yapimTarihi;
        private Tur turu;
        private bool aktif;
        private int emlakNumarasi;

        public Ev(int emlakNumarasi, int odaSayisi, int katNumarasi, decimal alani, string semti)
        {
            EmlakNumarasi = emlakNumarasi;
            OdaSayisi = odaSayisi;
            KatNumarasi = katNumarasi;
            Alani = alani;
            Semti = semti;
        }

        public Ev(int emlakNumarasi)
        {
            EmlakNumarasi = emlakNumarasi;
        }

        public Ev()
        {
        }

        public int KatNumarasi
        {
            get => katNumarasi;
            set
            {
                Logger.Logla(emlakNumarasi + " numarali evin kat numarasi girildi.", value);
                katNumarasi = (value < 0) ? 0 : value;
            }
        }

        public int OdaSayisi
        {
            get => odaSayisi;
            set
            {
                Logger.Logla(emlakNumarasi + " numarali evin oda sayisi girildi.", value);
                odaSayisi = (value < 1) ? 1 : value;
            }
        }

        public DateTime YapimTarihi
        {
            get => yapimTarihi;
            set
            {
                Logger.Logla(emlakNumarasi + " numarali evin yapim tarihi girildi. Girilen:" + value.ToString("dd.MM.yyyy"));
                if (value > DateTime.Now)
                    yapimTarihi = DateTime.Now;
                else yapimTarihi = value;
            }
        }

        public decimal Alani
        {
            get => alani;
            set
            {
                Logger.Logla(emlakNumarasi + " numarali evin alani girildi.", value);
                alani = (value < 10) ? 10 : value;
            }
        }

        public bool Aktif
        {
            get => aktif;
            set => aktif = value;
        }

        public int EmlakNumarasi
        {
            get => emlakNumarasi;
            set
            {
                Logger.Logla(emlakNumarasi + " numarali bir ev girildi.");
                emlakNumarasi = (value < 0) ? 0 : value;
            }
        }

        public string Semti
        {
            get => semti;
            set
            {
                value = value.Replace("|", "");
                semti = value.Replace("&", "");
            }
        }

        public Tur Turu
        {
            get => turu;
            set => turu = value;
        }

        public int EvinYasi
        {
            get => DateTime.Now.Year - yapimTarihi.Year;
        }

        public abstract string EvBilgileri();
    }
}