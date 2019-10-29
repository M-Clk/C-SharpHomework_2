﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HomeClassProject;
using System.Linq;
using System.Drawing;

namespace EmlakOtomasyonu
{
    public partial class AnaForm : Form
    {
        public AnaForm()
        {
            InitializeComponent();
        }
        EvManager evManager = EvManager.Instance;
        Ev seciliEv;
        private void AnaForm_Load(object sender, EventArgs e)
        {
            TurYukle();
            cbKategori.SelectedIndex = 0;
            cbIli.Items.AddRange(SemtManager.Instance.IlleriGetir());
            cbIli.SelectedIndex = 0;
            
        }

        private void AnaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            evManager.EvleriKaydet();
            Application.ExitThread();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if(evManager.EmlakNumarasiKontrol((int)numEmlakNo.Value))
            {
                MessageBox.Show("Sistemde zaten bu emlak numarasına ait bir ev var.", "Emlak Numarası Çakışması", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cbKategori.SelectedIndex == 0)
                SatilikEvKaydet();
            else KiralikEvKaydet();
            Kaydedildi();
            
        }
        void Kaydedildi()
        {
            KayitEkraniSifirla();
            cbKategori_SelectedIndexChanged(cbKategori, new KeyEventArgs(Keys.Enter));
        }
        void KayitEkraniSifirla()
        {          
            foreach (var item in tblKayit.Controls)
            {
                if (item is NumericUpDown)
                    ((NumericUpDown)item).Value = ((NumericUpDown)item).Minimum;
                if (item is ComboBox)
                if(((ComboBox)item).Items.Count>0)
                    ((ComboBox)item).SelectedIndex = 0;
                

            }
            tblDuzenle.Visible = false;
            btnKaydet.Visible = true;
            dtYapimTarihi.Value = DateTime.Now;
        }

        void SatilikEvKaydet()
        {
            SatilikEv yeniSatilik = (SatilikEv)evManager.EvOlustur((int)numEmlakNo.Value, numFiyatOrKira.Value);
            yeniSatilik = (SatilikEv)evManager.OrtakEvBilgileriGir((int)numOdaSayisi.Value, (int)numKatNumarasi.Value,numAlani.Value, dtYapimTarihi.Value, cbSemti.SelectedItem.ToString()+", "+ cbIli.SelectedItem.ToString(), (Tur)Enum.Parse(typeof(Tur), cbTuru.SelectedItem.ToString()), cbAktif.Checked, yeniSatilik);
        }
        void KiralikEvKaydet()
        {
            KiralikEv yeniKiralik = (KiralikEv)evManager.EvOlustur((int)numEmlakNo.Value, numFiyatOrKira.Value, numDepozito.Value);
            yeniKiralik = (KiralikEv)evManager.OrtakEvBilgileriGir((int)numOdaSayisi.Value, (int)numKatNumarasi.Value, numAlani.Value, dtYapimTarihi.Value, cbSemti.SelectedItem.ToString() + ", " + cbIli.SelectedItem.ToString(), (Tur)Enum.Parse(typeof(Tur), cbTuru.SelectedItem.ToString()), cbAktif.Checked, yeniKiralik);
        }

        void TurYukle()
        {
            cbTuru.Items.AddRange(Enum.GetNames(typeof(Tur)));
            cbTuru.SelectedIndex = 0;
        }

        private void cbKategori_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool kiralikMi = ((ComboBox)sender).SelectedIndex == 1;
            numDepozito.Visible = kiralikMi;
            lblDepozito.Visible = kiralikMi;
            lblKiraOrFiyat.Text = kiralikMi ? "Kira (₺)" : "Fiyat (₺)";
            if (kiralikMi)
                EvleriGoster(evManager.KiralikEvleriGetir());
            else EvleriGoster(evManager.SatilikEvleriGetir());
            KayitEkraniSifirla();
        }

        private void cbIli_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbSemti.Items.Clear();
            cbSemti.Items.AddRange(SemtManager.Instance.SemtGetir(((ComboBox)sender).SelectedItem.ToString()));
            cbSemti.SelectedIndex = 0;
        }
        void EvleriGoster(List<Ev> evler)
        {
            listView1.Clear();
           
            foreach (string item in evler[0].EvBilgileri().Split('|'))//Kolon isimleri
                listView1.Columns.Add(item.Split('=')[0]);
            foreach (var item in evler)
            {
                string[] datas = item.EvBilgileri().Split('|').Select(x => x.Split('=')[1]).ToArray();
                var listViewItem = new ListViewItem(datas);
                listView1.Items.Add(listViewItem);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            seciliEv = evManager.EvGetir(int.Parse(listView1.SelectedItems[0].Text));
            numEmlakNo.Value = seciliEv.EmlakNumarasi;
            numOdaSayisi.Value = seciliEv.OdaSayisi;
            numKatNumarasi.Value = seciliEv.KatNumarasi;
            numAlani.Value = seciliEv.Alani;
            dtYapimTarihi.Value = seciliEv.YapimTarihi;
            cbTuru.SelectedItem = Enum.GetName(typeof(Tur), seciliEv.Turu);
            string[] ilVeSemt = seciliEv.Semti.Split(new string[]{", "},StringSplitOptions.None);
            cbIli.SelectedItem = ilVeSemt[1];
            cbSemti.SelectedItem = ilVeSemt[0];
            cbAktif.Checked = seciliEv.Aktif;
            numFiyatOrKira.Value = (seciliEv is KiralikEv) ? ((KiralikEv)seciliEv).Kira : ((SatilikEv)seciliEv).Fiyat;
            if(seciliEv is KiralikEv)
            numDepozito.Value = ((KiralikEv)seciliEv).Depozito;
            btnArsivle.Text = seciliEv.Aktif ? "Arsivle" : "Etkinlestir";
            btnArsivle.BackColor = seciliEv.Aktif ? Color.LightSalmon : Color.GreenYellow;
            tblDuzenle.Visible = true;
            btnKaydet.Visible = false;
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (!((int)numEmlakNo.Value).Equals(seciliEv.EmlakNumarasi) && evManager.EmlakNumarasiKontrol((int)numEmlakNo.Value))
            {
                MessageBox.Show("Sistemde zaten bu emlak numarasına ait bir ev var.", "Emlak Numarası Çakışması", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            seciliEv.EmlakNumarasi = (int)numEmlakNo.Value ;
            seciliEv.OdaSayisi =  (int)numOdaSayisi.Value;
            seciliEv.KatNumarasi= (int)numKatNumarasi.Value ;
            seciliEv.Alani = numAlani.Value;
            seciliEv.YapimTarihi = dtYapimTarihi.Value;
            seciliEv.Turu = (Tur)Enum.Parse(typeof(Tur), cbTuru.SelectedItem.ToString());
            seciliEv.Semti=cbSemti.SelectedItem.ToString()+", "+cbIli.SelectedItem.ToString();
            seciliEv.Aktif = cbAktif.Checked;
            if (seciliEv is SatilikEv)
                ((SatilikEv)seciliEv).Fiyat = numFiyatOrKira.Value;
            else
            {
                ((KiralikEv)seciliEv).Kira = numFiyatOrKira.Value;
                ((KiralikEv)seciliEv).Depozito = numDepozito.Value;
            }
                
            evManager.EvGuncelle(seciliEv);
            Kaydedildi();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            evManager.EvSil(seciliEv);
            Kaydedildi();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            seciliEv.Aktif = !seciliEv.Aktif;
            Kaydedildi();
        }
    }
}