using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeClassProject
{
    
    public class Dosya
    {
        static List<Dosya> olusmusDosyalar;
        readonly string dosyaAdi;

        public string DosyaAdi { get => dosyaAdi;}

        private Dosya(string dosyaAdi)
        {
            olusmusDosyalar.Add(this);//her yeni olusan nesneyi bu static diziye ekle
            this.dosyaAdi = dosyaAdi;
            Olustur();
        }

        public static Dosya DosyaGetir(string dosyaAdi)//sibgleton mantigi ile dizede ayni isime sahip sadece tek dosya eklenebilmesini sagliyoruz
        {
            if (olusmusDosyalar == null) olusmusDosyalar = new List<Dosya>(); //liste static old. sadece bu method ilk kez calistiginda calisir

            var sonuc = olusmusDosyalar.Where(dosya => dosya.DosyaAdi.Equals(dosyaAdi)); //linq kullanarak adi dosyaAdi olan dosyalari filtreledik
            if (sonuc.Count() > 0) return sonuc.First();
            else
                return new Dosya(dosyaAdi);
           
        }

        private void Olustur()//Bu isimde dosya yoksa olusturuyoruz ve FileStream donduren Create methodunun geri donus degerini kapatiyoruz ki okuma yazma islemi yapilabilsin
        {
            if (!File.Exists(DosyaAdi)) File.Create(DosyaAdi).Close();
        }
        public void Guncelle(string txt)
        {
            File.WriteAllText(DosyaAdi, txt);
        }
        public void Ekle(string allTxt)
        {
            File.AppendAllText(DosyaAdi, allTxt);
        }
        public void EkleSatir(string line)
        {
            File.AppendAllText(DosyaAdi, line + "\r\n");
        }
        public string Oku()
        {
            return File.ReadAllText(DosyaAdi);
        }
    }
}
