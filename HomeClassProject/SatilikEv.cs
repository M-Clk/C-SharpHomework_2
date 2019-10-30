using System;

namespace HomeClassProject
{
    public class SatilikEv : Ev
    {
        private decimal fiyat;

        internal SatilikEv(int emlakNumarasi, decimal fiyat) : base(emlakNumarasi)
        {
            Fiyat = fiyat;
        }

        internal SatilikEv(int emlakNumarasi, int odaSayisi, int katNumarasi, int alani, string semti, decimal fiyati) : base(emlakNumarasi, odaSayisi, katNumarasi, alani, semti)
        {
            Fiyat = fiyati;
        }

        internal SatilikEv()
        { }

        public decimal Fiyat
        {
            get => fiyat;
            set
            {
                Logger.Logla(EmlakNumarasi + " numarali evin fiyati girildi.", value);
                fiyat = (value < 0) ? 0 : value;
            }
        }

        public override string EvBilgileri()
        {
            string format = "Emlak Numarası={0}|Oda Sayısı={1}|Kat Numarası={2}|Alan={3}|Yapım Tarihi={4:dd.MM.yyyy}|Semt={5}|Tür={6}|Aktif={7}|Fiyat={8}";
            string sonuc = string.Format(format, EmlakNumarasi, OdaSayisi, KatNumarasi, Alani, YapimTarihi, Semti, Enum.GetName(typeof(Tur), Turu), Aktif, Fiyat);
            return sonuc;
        }
    }
}