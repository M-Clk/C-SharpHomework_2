using System;

namespace HomeClassProject
{
    public class KiralikEv : Ev
    {
        private decimal depozito;
        private decimal kira;
        private const int defaultCost = 200;

        internal KiralikEv(int emlakNumarasi, decimal kira, decimal depozito) : base(emlakNumarasi)
        {
            Depozito = depozito;
            Kira = kira;
        }

        internal KiralikEv(int emlakNumarasi, decimal kira) : base(emlakNumarasi)
        {
            Kira = kira;
        }

        internal KiralikEv(int emlakNumarasi, int odaSayisi, int katNumarasi, int alani, string semti, decimal kira) : base(emlakNumarasi, odaSayisi, katNumarasi, alani, semti)
        {
            Kira = kira;
        }

        internal KiralikEv()
        {
        }

        public decimal Depozito
        {
            get => depozito;
            set
            {
                Logger.Logla(EmlakNumarasi + " numarali evin depozitosu girildi.", value);
                depozito = (value < 0) ? 0 : value;
            }
        }

        public decimal Kira
        {
            get => kira;
            set
            {
                Logger.Logla(EmlakNumarasi + " numarali evin kirasi girildi.", value);
                kira = (value < 0) ? 0 : value;
            }
        }

        public int FiyatHesapla()
        {
            int katsayi;
            Dosya katsayiDosyasi = Dosya.DosyaGetir("room_cons.txt");
            if (int.TryParse(katsayiDosyasi.Oku(), out katsayi))
                return OdaSayisi * katsayi;
            else
                return defaultCost * OdaSayisi;
        }

        public override string EvBilgileri()
        {
            string format = "Emlak Numarası={0}|Oda Sayısı={1}|Kat Numarası={2}|Alan={3}|Yapım Tarihi={4:dd.MM.yyyy}|Semt={5}|Tür={6}|Aktif={7}|Kira={8}|Depozito={9}";
            string sonuc = string.Format(format, EmlakNumarasi, OdaSayisi, KatNumarasi, Alani, YapimTarihi, Semti, Enum.GetName(typeof(Tur), Turu), Aktif, Kira, Depozito);
            return sonuc;
        }
    }
}