using System;
using System.Windows.Forms;

namespace EmlakOtomasyonu
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }
        KullaniciManager kullaniciManager;
        private void Login_Load(object sender, EventArgs e)
        {
            kullaniciManager = KullaniciManager.Instance;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GirisYap();
        }

        private void txtSifre_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
             GirisYap();
        }
        void GirisYap()
        {
            if (kullaniciManager.KullaniciKontrol(txtKullaniciAdi.Text, txtSifre.Text))
            {
                this.Hide();
                AnaForm form = new AnaForm();
                form.ShowDialog();
            }
            else
            {
                txtSifre.Clear();
                MessageBox.Show("Kullanici adi veya sifre yanlis. Tekrar Deneyin.", "Hatali Giris", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtKullaniciAdi_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled =!char.IsLetterOrDigit(e.KeyChar) && !char.IsPunctuation(e.KeyChar) && !char.IsControl(e.KeyChar) ;
        }
    }
}