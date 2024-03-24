using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO.Ports;

namespace Demo
{
    public partial class Form1 : Form
    {
        float x = 0, y = 0, z = 0;
        bool cx = false,cy = false, cz = false;
        public Form1()
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
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
        private void TimerXYZ_Tick(object sender, EventArgs e)
        {
            if (cx == true)
            {
                if (x < 360)
                    x += 5;
                else
                    x = 0;
                label3.Text = x.ToString();
            }
            if (cy == true)
            {
                if (y < 360)
                    y += 5;
                else
                    y = 0;
                label4.Text = y.ToString();
            }
            if (cz == true)
            {
                if (z < 360)
                    z += 5;
                else
                    z = 0;
                label5.Text = z.ToString();
            }
            glControl1.Invalidate();
        }



        private void baslatButton_Click(object sender, EventArgs e)
        {
            try
            {
                readingPort.BaudRate = Convert.ToInt32(boundRateBox.Text);
                readingPort.PortName = serialPortBox.Text;
                if (!readingPort.IsOpen)
                {
                    timer.Start();
                    readingPort.Open();
                    //MessageBox.Show("BAĞLANTI KURULDU");
                    durdurButton.Enabled = true;
                    baslatButton.Enabled = false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("BAĞLANTI KURULAMADI");
                durdurButton.Enabled = true;
                //durdurButton.Enabled = false;
            }
        }
        private void durdurButton_Click(object sender, EventArgs e)
        {
            readingPort.Close();
            timer.Stop();
            baslatButton.Enabled = true;
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

            GL.Rotate(x, 1.0, 0.0, 0.0);//Yönlendirme kısımları
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
            //GraphicsContext.CurrentContext.VSync = true;
            glControl1.SwapBuffers();
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

        private void silindir(float step, float topla, float radius, float dikey1, float dikey2)
        {
            float eski_step = 0.1f;
            GL.Begin(BeginMode.Quads);//Y EKSEN CIZIM DAİRENİN

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

            while (step <= 180)// UST KAPAK
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

            while (step <= 180)//ALT KAPAK
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

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (cy == false)
                cy = true;
            else
                cy = false;
            TimerXYZ.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (cz == false)
                cz = true;
            else
                cz = false;
            TimerXYZ.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cx == false)
                cx = true;
            else
                cx = false;
            TimerXYZ.Start();
        }

        

        private void koni(float step, float topla, float radius1, float radius2, float dikey1, float dikey2)
        {
            float eski_step = 0.1f;
            GL.Begin(BeginMode.Lines);//Y EKSEN CIZIM DAİRENİN

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

            while (step <= 180)// UST KAPAK
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
        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);//sonradan yazdık
        }
    }
}
