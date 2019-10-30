using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HomeClassProject;
using System.Linq;
using System.Drawing;
using System.IO;

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
        string[] seciliResimler;
        private void AnaForm_Load(object sender, EventArgs e)
        {
            TurYukle();
            cbKategori.SelectedIndex = 0;
            cbIli.Items.AddRange(SemtManager.Instance.IlleriGetir());
            cbIli.SelectedIndex = 0;
            cmbIldekiler.Items.AddRange(SemtManager.Instance.IlleriGetir());
            cmbIldekiler.SelectedIndex = 0;
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
                ResimKaydet(SatilikEvKaydet());
            else ResimKaydet(KiralikEvKaydet());
            Kaydedildi();
            
        }
        void ResimKaydet(Ev yeniEv)
        { if (seciliResimler.Length == 0) return;
            string picPath = Application.StartupPath + "\\Resimler\\" + yeniEv.EmlakNumarasi;
            if (!Directory.Exists(picPath))
            Directory.CreateDirectory(picPath);
            int i = 0;
            foreach (var item in seciliResimler)
                File.Copy(item, picPath + "\\" +yeniEv.EmlakNumarasi+"_"+(i++));
            
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
            lblResimSec.Visible = true;
            seciliResimler=new string[0];
            dtYapimTarihi.Value = DateTime.Now;
        }

        Ev SatilikEvKaydet()
        {
            SatilikEv yeniSatilik = (SatilikEv)evManager.EvOlustur((int)numEmlakNo.Value, numFiyatOrKira.Value);
            yeniSatilik = (SatilikEv)evManager.OrtakEvBilgileriGir((int)numOdaSayisi.Value, (int)numKatNumarasi.Value,numAlani.Value, dtYapimTarihi.Value, cbSemti.SelectedItem.ToString()+", "+ cbIli.SelectedItem.ToString(), (Tur)Enum.Parse(typeof(Tur), cbTuru.SelectedItem.ToString()), true, yeniSatilik);
            return yeniSatilik;
        }
        Ev KiralikEvKaydet()
        {
            KiralikEv yeniKiralik = (KiralikEv)evManager.EvOlustur((int)numEmlakNo.Value, numFiyatOrKira.Value, numDepozito.Value);
            yeniKiralik = (KiralikEv)evManager.OrtakEvBilgileriGir((int)numOdaSayisi.Value, (int)numKatNumarasi.Value, numAlani.Value, dtYapimTarihi.Value, cbSemti.SelectedItem.ToString() + ", " + cbIli.SelectedItem.ToString(), (Tur)Enum.Parse(typeof(Tur), cbTuru.SelectedItem.ToString()), true, yeniKiralik);
            return yeniKiralik;
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
            cbFiyatOrKiraAz.Text = kiralikMi ? "Kirası En Az Su Kadar" : "Fiyatı En Az Su Kadar";
            cbFiyatOrKiraFazla.Text = kiralikMi ? "Kirası En Fazla Su Kadar" : "Fiyatı En Fazla Su Kadar";
            EvleriFiltrele();
            KayitEkraniSifirla();
        }
        void EvleriFiltrele()
        {
            var filtreliList = evManager.EvFiltrele(
                cbTarihAz.Checked ? dtTarihAz.Value : default(DateTime),
                cbTarihFazla.Checked ? dtTarihFazla.Value : default(DateTime),
                cbAlanAz.Checked ? numAlanAz.Value : -1,
                cbOdaSayisiAz.Checked ? (int)numOdaSayisiAz.Value : -1,
                cbIldekiler.Checked ? cmbIldekiler.SelectedItem.ToString() : null,
                cbFiyatOrKiraAz.Checked ? numFiyatOrKiraAz.Value : -1,
                cbFiyatOrKiraFazla.Checked ? numFiyatOrKiraFazla.Value : -1,
                cbAktif.Checked,
                cbKategori.SelectedIndex == 1
                );
            EvleriGoster(filtreliList);
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
            if (evler.Count == 0) return;
            foreach (string item in evler[0].EvBilgileri().Split('|'))//Kolon isimleri
                listView1.Columns.Add(item.Split('=')[0],100);
            listView1.Columns.RemoveAt(7);
            foreach (var item in evler)
            {
                string[] data = item.EvBilgileri().Split('|').Select(x => x.Split('=')[1]).ToArray();
                var listViewItem = new ListViewItem(data);
                listViewItem.SubItems.RemoveAt(7);
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
            numFiyatOrKira.Value = (seciliEv is KiralikEv) ? ((KiralikEv)seciliEv).Kira : ((SatilikEv)seciliEv).Fiyat;
            if(seciliEv is KiralikEv)
            numDepozito.Value = ((KiralikEv)seciliEv).Depozito;
            btnArsivle.Text = seciliEv.Aktif ? "Arsivle" : "Etkinlestir";
            btnArsivle.BackColor = seciliEv.Aktif ? Color.LightSalmon : Color.GreenYellow;
            tblDuzenle.Visible = true;
            btnKaydet.Visible = false;
            lblResimler.Text = "";
            lblResimSec.Visible = false;
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            string picPath = Application.StartupPath + "\\Resimler\\" + seciliEv.EmlakNumarasi;
            if (Directory.Exists(picPath))
            {
                MediaPlayer media = new MediaPlayer(picPath);
                media.ShowDialog();
            }
            else MessageBox.Show("Bu ile ilgili resim bulunamadi.", "Oops...", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void label10_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            seciliResimler = openFileDialog1.FileNames;
            lblResimler.Text = openFileDialog1.FileNames.Length + " resim secildi.";
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Kaydedildi();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            EvleriFiltrele();
        }

        private void cbTarihFazla_CheckedChanged(object sender, EventArgs e)
        {
            dtTarihFazla.Visible = ((CheckBox)sender).Checked;
        }

        private void cbTarihAz_CheckedChanged(object sender, EventArgs e)
        {
            dtTarihAz.Visible = ((CheckBox)sender).Checked;
        }

        private void cbAlanAz_CheckedChanged(object sender, EventArgs e)
        {
            numAlanAz.Visible = ((CheckBox)sender).Checked;
        }

        private void cbOdaSayisiAz_CheckedChanged(object sender, EventArgs e)
        {
            numOdaSayisiAz.Visible = ((CheckBox)sender).Checked;
        }

        private void cbIldekiler_CheckedChanged(object sender, EventArgs e)
        {
            cmbIldekiler.Visible = ((CheckBox)sender).Checked;
        }

        private void cbFiyatOrKiraAz_CheckedChanged(object sender, EventArgs e)
        {
            numFiyatOrKiraAz.Visible = ((CheckBox)sender).Checked;
        }

        private void cbFiyatOrKiraFazla_CheckedChanged(object sender, EventArgs e)
        {
            numFiyatOrKiraFazla.Visible = ((CheckBox)sender).Checked;
        }
    }
}