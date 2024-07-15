using System.Windows.Forms;

namespace Demo
{
    partial class HataFormu
    {
        private System.ComponentModel.IContainer components = null;
        private ListBox hataListesi;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.hataListesi = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // hataListesi
            // 
            this.hataListesi.FormattingEnabled = true;
            this.hataListesi.Location = new System.Drawing.Point(12, 12);
            this.hataListesi.Name = "hataListesi";
            this.hataListesi.Size = new System.Drawing.Size(360, 238);
            this.hataListesi.TabIndex = 0;
            // 
            // HataFormu
            // 
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.hataListesi);
            this.Name = "HataFormu";
            this.Text = "Hata Formu";
            this.ResumeLayout(false);

        }
    }
}
