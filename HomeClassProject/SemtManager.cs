using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeClassProject
{
    public class SemtManager
    {
        private static Dictionary<string, string[]> semtList = new Dictionary<string, string[]>();
        private static SemtManager instance = new SemtManager();
        
        private SemtManager()
        {
            SemtYukle();
        }

        public static SemtManager Instance { get => instance; }

        void SemtYukle()
        {
            Dosya dosya = Dosya.DosyaGetir("semtler.txt");
            string ilVeSemtler = dosya.Oku();
            string[] tekIleGoreSemtler = ilVeSemtler.Split('|');
            if (tekIleGoreSemtler[0].Length == 0) return;
            foreach (string item in tekIleGoreSemtler)
            {
                string ilAdi = item.Split('=')[0];
                string semtler = item.Split('=')[1];
                semtList.Add(ilAdi, semtler.Split(','));
            }
        }
        public string[] IlleriGetir()
        {
            return semtList.Keys.ToArray();
        }
        public string[] SemtGetir(string ilAdi)
        {
            return semtList[ilAdi];
        }


    }
}
