using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeClassProject
{
    public static class Logger
    {
        public static void Logla(string message, decimal girilenSayisalDeger) //herhangi bir sayisal deger girisi loglama
        {
            Dosya logDosyasi = Dosya.DosyaGetir("log.txt");
            string durum = girilenSayisalDeger >= 0 ? "+" : "-";
            logDosyasi.EkleSatir("(" + durum + ") " + "[" + DateTime.Now.ToString() + "]" + message + " Girilen:"+girilenSayisalDeger);
        }
        public static void Logla(string message)//herhangi bir bolgi mesaji loglama
        {
            Dosya logDosyasi = Dosya.DosyaGetir("log.txt");
            logDosyasi.EkleSatir("(i) " + "[" + DateTime.Now.ToString() + "]" + message);
        }
    }
}
