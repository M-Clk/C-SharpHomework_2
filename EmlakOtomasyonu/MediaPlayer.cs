using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmlakOtomasyonu
{
    public partial class MediaPlayer : Form
    {
        string picPath;
        public MediaPlayer(string picPath)
        {
            this.picPath = picPath;
            InitializeComponent();
        }
        int selectedImageIndex=0;
        private void MediaPlayer_Load(object sender, EventArgs e)
        {
            DirectoryInfo directory = new DirectoryInfo(picPath);
            FileInfo[] Archives = directory.GetFiles();
            foreach (FileInfo fileinfo in Archives)
            {
                imageList1.Images.Add(Image.FromFile(fileinfo.FullName));
            }
            if(imageList1.Images.Count>0)
            pictureBox1.Image = imageList1.Images[selectedImageIndex];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (imageList1.Images.Count - 1 > selectedImageIndex)
                pictureBox1.Image = imageList1.Images[++selectedImageIndex];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (selectedImageIndex != 0)
                pictureBox1.Image = imageList1.Images[--selectedImageIndex];
        }
    }
}
