using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO.Ports;

namespace Demo
{
    public partial class SATX : Form
    {
        float x = 0, y = 0, z = 0;
        bool cx = false, cy = false, cz = false;
        Timer timer1;

        public SATX()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] portlar = SerialPort.GetPortNames();
            foreach (string portAdi in portlar)
            {
                serialPortBox.Items.Add(portAdi);
            }
            GL.ClearColor(Color.Navy);
            TimerXYZ.Interval = 1;
            TimerXYZ.Tick += new EventHandler(TimerXYZ_Tick);

            timer1 = new Timer();
            timer1.Interval = 1000; // 1 saniyede bir tetiklenecek
            timer1.Tick += new EventHandler(timer1_Tick);

            readingPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            // Add the gray panels (this part is unchanged)
            // Gri arka plan rengi olan panel
            Panel panelGriArkaPlan1 = new Panel();
            panelGriArkaPlan1.BackColor = Color.Gray;
            panelGriArkaPlan1.Location = new Point(397, 256);
            panelGriArkaPlan1.Size = new Size(612 - 397, 360 - 256);
            this.Controls.Add(panelGriArkaPlan1);

            // Gri arka plan rengi olan panel
            Panel panelGriArkaPlan2 = new Panel();
            panelGriArkaPlan2.BackColor = Color.Gray;
            panelGriArkaPlan2.Location = new Point(24, 13);
            panelGriArkaPlan2.Size = new Size(960 - 24, 50 - 13);
            this.Controls.Add(panelGriArkaPlan2);

            // Gri arka plan rengi olan panel
            Panel panelGriArkaPlan3 = new Panel();
            panelGriArkaPlan3.BackColor = Color.Gray;
            panelGriArkaPlan3.Location = new Point(973, 140);
            panelGriArkaPlan3.Size = new Size(1220 - 973, 695 - 100);
            this.Controls.Add(panelGriArkaPlan3);
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string sonuc = readingPort.ReadLine();
                string[] paket = sonuc.Split('*');

                // Değişkenleri ayrıştır
                if (paket.Length >= 5)
                {
                    if (float.TryParse(paket[0], out float modelInisHizi) &&
                        float.TryParse(paket[1], out float gorevInisHizi) &&
                        float.TryParse(paket[2], out float tasiyiciGps) &&
                        float.TryParse(paket[3], out float gorevGps) &&
                        bool.TryParse(paket[4], out bool ayrilmaDurumu))
                    {
                        // Kontrolleri gerçekleştir
                        CheckModelInisHizi(modelInisHizi);
                        CheckGorevInisHizi(gorevInisHizi);
                        CheckTasiyiciGps(tasiyiciGps);
                        CheckGorevGps(gorevGps);
                        CheckAyrilmaDurumu(ayrilmaDurumu);

                        // Label güncellemeleri (UI güncellemesi ana thread'de yapılmalı)
                        this.Invoke((MethodInvoker)delegate {
                            labelxx.Text = paket[0];
                            labelyy.Text = paket[1];
                            labelzz.Text = paket[2];
                        });

                        x = modelInisHizi;
                        y = gorevInisHizi;
                        z = tasiyiciGps;
                    }
                    else
                    {
                        // Veri dönüştürme hatası
                        ShowWarning("Seri porttan gelen verilerde format hatası.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri okuma hatası: {ex.Message}");
            }
        }

        private void baslatButton_Click(object sender, EventArgs e)
        {
            try
            {
                readingPort.BaudRate = Convert.ToInt32(boundRateBox.Text);
                readingPort.PortName = serialPortBox.Text;
                if (!readingPort.IsOpen)
                {
                    timer1.Start();
                    readingPort.Open();
                    durdurButton.Enabled = true;
                    baglanButton.Enabled = false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("BAĞLANTI KURULAMADI");
                durdurButton.Enabled = true;
            }
        }

        private void durdurButton_Click(object sender, EventArgs e)
        {
            readingPort.Close();
            timer1.Stop();
            baglanButton.Enabled = true;
            durdurButton.Enabled = false;
            MessageBox.Show("BAĞLANTI KESİLDİ");
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            float step = 1.0f;
            float topla = step;
            float radius = 5.0f;
            float dikey1 = radius, dikey2 = -radius;
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(1.04f, 4 / 3, 1, 10000);
            Matrix4 lookat = Matrix4.LookAt(25, 0, 0, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookat);
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            GL.Rotate(x, 1.0, 0.0, 0.0);
            GL.Rotate(z, 0.0, 1.0, 0.0);
            GL.Rotate(y, 0.0, 0.0, 1.0);

            silindir(step, topla, radius, 3, -5);
            koni(0.01f, 0.01f, radius, 3.0f, 3, 5);
            koni(0.01f, 0.01f, radius, 2.0f, -5.0f, -10.0f);

            GL.Begin(BeginMode.Lines);
            GL.Color3(Color.FromArgb(250, 0, 0));
            GL.Vertex3(-30.0, 0.0, 0.0);
            GL.Vertex3(30.0, 0.0, 0.0);
            GL.Color3(Color.FromArgb(0, 0, 0));
            GL.Vertex3(0.0, 30.0, 0.0);
            GL.Vertex3(0.0, -30.0, 0.0);
            GL.Color3(Color.FromArgb(0, 0, 250));
            GL.Vertex3(0.0, 0.0, 30.0);
            GL.Vertex3(0.0, 0.0, -30.0);
            GL.End();
            glControl1.SwapBuffers();
        }

        private void CheckModelInisHizi(float hiz)
        {
            if (hiz < 12 || hiz > 14)
            {
                this.Invoke((MethodInvoker)delegate {
                    ModelInisHiziKontrol.Text = "1";
                    ModelInisHiziKontrolRenk.BackColor = Color.Red;
                });
                ShowWarning("Model uydu iniş hızı 12-14 m/s dışında!");
            }
        }

        private void CheckGorevInisHizi(float hiz)
        {
            if (hiz < 6 || hiz > 8)
            {
                this.Invoke((MethodInvoker)delegate {
                    GorevInisHiziKontrol.Text = "1";
                    GorevInisHiziKontrolRenk.BackColor = Color.Red;
                });
                ShowWarning("Görev yükü iniş hızı 6-8 m/s dışında!");
            }
        }

        private void CheckTasiyiciGps(float gps)
        {
            if (gps == 0) // 0 değeri veri alınamaması olarak varsayıldı, gerekirse düzenleyin
            {
                this.Invoke((MethodInvoker)delegate {
                    TasiyiciGpsKontrol.Text = "1";
                    TasiyiciGpsKontrolRenk.BackColor = Color.Red;
                });
                ShowWarning("Taşıyıcı konum verisi alınamıyor!");
            }
        }

        private void CheckGorevGps(float gps)
        {
            if (gps == 0) // 0 değeri veri alınamaması olarak varsayıldı, gerekirse düzenleyin
            {
                this.Invoke((MethodInvoker)delegate {
                    GorevGpsKontrol.Text = "1";
                    GorevGpsKontrolRenk.BackColor = Color.Red;
                });
                ShowWarning("Görev yükü konum verisi alınamıyor!");
            }
        }

        private void CheckAyrilmaDurumu(bool ayrilma)
        {
            if (!ayrilma)
            {
                this.Invoke((MethodInvoker)delegate {
                    AyrilmaKontrol.Text = "1";
                    AyrilmaKontrolRenk.BackColor = Color.Red;
                });
                ShowWarning("Ayrılma gerçekleşmedi!");
            }
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cx = !cx;
            TimerXYZ.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cy = !cy;
            TimerXYZ.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cz = !cz;
            TimerXYZ.Start();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
        }

        private void silindir(float step, float topla, float radius, float dikey1, float dikey2)
        {
            float eski_step = 0.1f;
            GL.Begin(BeginMode.Quads);

            while (step <= 360)
            {
                color_template(step);
                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 2) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 2) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);

                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            GL.End();
            GL.Begin(BeginMode.Lines);
            step = eski_step;
            topla = step;

            while (step <= 180)
            {
                color_template(step);
                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);

                GL.Vertex3(ciz1_x, dikey1, ciz1_y);
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);
                step += topla;
            }
            step = eski_step;
            topla = step;

            while (step <= 180)
            {
                color_template(step);
                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey2, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);

                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            GL.End();
        }

        private void koni(float step, float topla, float radius1, float radius2, float dikey1, float dikey2)
        {
            float eski_step = 0.1f;
            GL.Begin(BeginMode.Lines);

            while (step <= 360)
            {
                if (step < 45)
                    GL.Color3(1.0, 1.0, 1.0);
                else if (step < 90)
                    GL.Color3(1.0, 0.0, 0.0);
                else if (step < 135)
                    GL.Color3(1.0, 1.0, 1.0);
                else if (step < 180)
                    GL.Color3(1.0, 0.0, 0.0);
                else if (step < 225)
                    GL.Color3(1.0, 1.0, 1.0);
                else if (step < 270)
                    GL.Color3(1.0, 0.0, 0.0);
                else if (step < 315)
                    GL.Color3(1.0, 1.0, 1.0);
                else if (step < 360)
                    GL.Color3(1.0, 0.0, 0.0);

                float ciz1_x = (float)(radius1 * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius1 * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius2 * Math.Cos(step * Math.PI / 180F));
                float ciz2_y = (float)(radius2 * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            GL.End();
            GL.Begin(BeginMode.Lines);
            step = eski_step;
            topla = step;

            while (step <= 180)
            {
                color_template(step);
                float ciz1_x = (float)(radius2 * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius2 * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey2, ciz1_y);

                float ciz2_x = (float)(radius2 * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius2 * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);

                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            step = eski_step;
            topla = step;
            GL.End();
        }

        private void color_template(float step)
        {
            if (step < 45)
                GL.Color3(Color.FromArgb(255, 0, 0));
            else if (step < 90)
                GL.Color3(Color.FromArgb(255, 255, 255));
            else if (step < 135)
                GL.Color3(Color.FromArgb(255, 0, 0));
            else if (step < 180)
                GL.Color3(Color.FromArgb(255, 255, 255));
            else if (step < 225)
                GL.Color3(Color.FromArgb(255, 0, 0));
            else if (step < 270)
                GL.Color3(Color.FromArgb(255, 255, 255));
            else if (step < 315)
                GL.Color3(Color.FromArgb(255, 0, 0));
            else if (step < 360)
                GL.Color3(Color.FromArgb(255, 255, 255));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                string[] paket;
                string sonuc = readingPort.ReadLine();
                paket = sonuc.Split('*');
                labelxx.Text = paket[0];
                labelyy.Text = paket[1];
                labelzz.Text = paket[2];
                x = Convert.ToInt32(paket[0]);
                y = Convert.ToInt32(paket[1]);
                z = Convert.ToInt32(paket[2]);
                glControl1.Invalidate();
                readingPort.DiscardInBuffer();
            }
            catch
            {
                // Hata oluşursa, ek bir işlem yapmadan geç
            }
        }

        private void TimerXYZ_Tick(object sender, EventArgs e)
        {
            // TimerXYZ'nin tıklama olayını tanımlayın
        }

        private void label7_Click(object sender, EventArgs e)
        {
            // label7 tıklama işlemi boş
        }
    }
}
