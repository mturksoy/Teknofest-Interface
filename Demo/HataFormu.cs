using System;
using System.Windows.Forms;

namespace Demo
{
    public partial class HataFormu : Form
    {
        public HataFormu()
        {
            InitializeComponent();
        }

        public void AddHata(string mesaj)
        {
            this.Invoke((MethodInvoker)delegate
            {
                hataListesi.Items.Add(mesaj);
                this.Show(); // Hata formunu göster
            });
        }
    }
}
