using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO.Ports;
using Microsoft.Web.WebView2.WinForms;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Demo
{
    public partial class SATX : Form
    {
        private float x = 0, y = 0, z = 0;
        private double gpsEnlem = 39.92500;  // Başlangıç enlemi
        private double gpsBoylam = 32.83694;  // Başlangıç boylamı
        private double elapsedTime = 0;  // Zamanı takip eden değişken
        private const double timeStep = 1;  // Zaman adımı (her veri geldiğinde artar)
        private string hataKoduDegeri = "00000";
        private StreamWriter telemetryWriter;
        Timer timer1;

        public SATX()
        {
            InitializeComponent();
            this.Load += async (s, e) => await InitializeWebView();
            OpenTelemetryFile();
        }

        private async Task InitializeWebView()
        {
            // WebView2'nin başlatılmasını bekleyin
            await webView21.EnsureCoreWebView2Async();

            // İlk harita yüklemesini yapın
            LoadMap();
        }

        private void LoadMap()
        {
            string enlemStr = gpsEnlem.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string boylamStr = gpsBoylam.ToString(System.Globalization.CultureInfo.InvariantCulture);

            string htmlContent = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <title>Google Maps with Marker</title>
            <script src='https://maps.googleapis.com/maps/api/js?key=MY_API'></script>
            <script>
                var map;
                var marker;

                function initialize() {{
                    var mapOptions = {{
                        zoom: 15,
                        center: new google.maps.LatLng({enlemStr}, {boylamStr}),
                        mapTypeId: google.maps.MapTypeId.ROADMAP
                    }};
                    map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
                    marker = new google.maps.Marker({{
                        map: map,
                        position: new google.maps.LatLng({enlemStr}, {boylamStr}),
                        title: 'My location'
                    }});
                }}

                window.onload = initialize;
            </script>
            <style>
                html, body, #map-canvas {{
                    height: 100%;
                    margin: 0;
                    padding: 0;
                }}
            </style>
        </head>
        <body>
            <div id='map-canvas'></div>
        </body>
        </html>";

            // WebView2'de HTML içeriğini yükleme
            webView21.NavigateToString(htmlContent);
        }

        private void UpdateMap()
        {
            string enlemStr = gpsEnlem.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string boylamStr = gpsBoylam.ToString(System.Globalization.CultureInfo.InvariantCulture);

            string script = $@"
        if (window.marker) {{
            // Marker varsa, konumunu güncelle ve haritanın merkezini marker'a ayarla
            var newLatLng = new google.maps.LatLng({enlemStr}, {boylamStr});
            marker.setPosition(newLatLng);
            map.setCenter(newLatLng);
        }} else {{
            // İlk yüklemede haritayı ve marker'ı oluştur
            var mapOptions = {{
                zoom: 15,
                center: new google.maps.LatLng({enlemStr}, {boylamStr}),
                mapTypeId: google.maps.MapTypeId.ROADMAP
            }};
            map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
            marker = new google.maps.Marker({{
                map: map,
                position: new google.maps.LatLng({enlemStr}, {boylamStr}),
                title: 'My location'
            }});
        }}";

            // Script'i WebView2'de çalıştırma
            webView21.CoreWebView2.ExecuteScriptAsync(script);
        }

        private void OpenTelemetryFile()
        {
            try
            {
                // Telemetri verilerini kaydetmek için dosya yolunu belirliyoruz
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "telemetry_data.csv");

                // Dosya yoksa başlıkları yazacak şekilde dosya oluşturuyoruz
                bool fileExists = File.Exists(path);
                telemetryWriter = new StreamWriter(path, true); // Dosya yoksa oluşturur, varsa üzerine yazar.

                if (!fileExists)
                {
                    // Dosya yoksa başlık satırlarını yazıyoruz
                    telemetryWriter.WriteLine("PAKET NUMARASI,UYDU STATÜSÜ,HATA KODU,GÖNDERME SAATİ,BASINÇ1,BASINÇ2,YÜKSEKLİK1,YÜKSEKLİK2,İRTİFA FARKI,İNİŞ HIZI,SICAKLIK,PİL GERİLİMİ,GPS1 LATITUDE,GPS1 LONGITUDE,GPS1 ALTITUDE,PITCH,ROLL,YAW,RHRH,IoT DATA,TAKIM NO");
                    telemetryWriter.WriteLine(",,,(HH:MM:SS),Pa,Pa,(m),(m),(m),(m/s),(°C),(V),(°),(°),(m),(°),(°),(°),,(IoT),");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dosya açma hatası: {ex.Message}");
            }
        }

        private void SaveTelemetryData(JObject telemetryData, string hataDegeri)
        {
            try
            {
                string csvLine = $"{telemetryData["paketNumarasi"]},{telemetryData["uyduStatusu"]},{hataDegeri},{telemetryData["gondermeSaati"]},{telemetryData["basinc1"]},{telemetryData["basinc2"]},{telemetryData["yukseklik1"]},{telemetryData["yukseklik2"]},{telemetryData["irtifaFarki"]},{telemetryData["inisHizi"]},{telemetryData["sicaklik"]},{telemetryData["pilGerilimi"]},{telemetryData["gps1Enlem"]},{telemetryData["gps1Boylam"]},{telemetryData["gps1Yukseklik"]},{telemetryData["pitch"]},{telemetryData["roll"]},{telemetryData["yaw"]},{telemetryData["rhrh"]},{telemetryData["iotData"]},{telemetryData["takimNo"]}";
                telemetryWriter.WriteLine(csvLine);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri kaydetme hatası: {ex.Message}");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // boundRateBox'a varsayılan değer olarak 9600 atama
            boundRateBox.Text = "9600";

            string[] portlar = SerialPort.GetPortNames();
            foreach (string portAdi in portlar)
            {
                serialPortBox.Items.Add(portAdi);
            }
            GL.ClearColor(Color.Navy);
            TimerXYZ.Interval = 1;
            TimerXYZ.Tick += new EventHandler(TimerXYZ_Tick);

            timer1 = new Timer
            {
                Interval = 1000 // 1 saniyede bir tetiklenecek
            };

            timer1.Tick += new EventHandler(Timer1_Tick);

            // Gri arka plan rengi olan panel (Bağlan Kısmı)
            Panel panelGriArkaPlan1 = new Panel
            {
                BackColor = Color.Gray,
                Location = new Point(487, 115),
                Size = new Size(235, 150)
            };
            this.Controls.Add(panelGriArkaPlan1);

            // Gri arka plan rengi olan panel (Uydu Statüsü Kısmı)
            Panel panelGriArkaPlan2 = new Panel
            {
                BackColor = Color.Gray,
                Location = new Point(497, 315),
                Size = new Size(215, 115)
            };
            this.Controls.Add(panelGriArkaPlan2);

            // Gri arka plan rengi olan panel (Üst Kısım)
            Panel panelGriArkaPlan3 = new Panel
            {
                BackColor = Color.Gray,
                Location = new Point(24, 13),
                Size = new Size(1156, 37)
            };
            this.Controls.Add(panelGriArkaPlan3);

            // Gri arka plan rengi olan panel (veri paneli kısmı)
            Panel panelGriArkaPlan4 = new Panel
            {
                BackColor = Color.Gray,
                Location = new Point(1225, 61),
                Size = new Size(250, 875)
            };
            this.Controls.Add(panelGriArkaPlan4);
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string jsonData = readingPort.ReadLine();
                JObject telemetryData = JObject.Parse(jsonData);

                ulong paketNumarasi = telemetryData["paketNumarasi"].ToObject<ulong>();
                int uyduStatusu = telemetryData["uyduStatusu"].ToObject<int>();
                string gondermeSaati = telemetryData["gondermeSaati"].ToString();
                float basinc1 = telemetryData["basinc1"].ToObject<float>();
                float basinc2 = telemetryData["basinc2"].ToObject<float>();
                float yukseklik1 = telemetryData["yukseklik1"].ToObject<float>();
                float yukseklik2 = telemetryData["yukseklik2"].ToObject<float>();
                float irtifaFarki = telemetryData["irtifaFarki"].ToObject<float>();
                float inisHizi = telemetryData["inisHizi"].ToObject<float>();
                float sicaklik = telemetryData["sicaklik"].ToObject<float>();
                float pilGerilimi = telemetryData["pilGerilimi"].ToObject<float>();
                double gps1Enlem = telemetryData["gps1Enlem"].ToObject<double>();
                double gps1Boylam = telemetryData["gps1Boylam"].ToObject<double>();
                float gps1Yukseklik = telemetryData["gps1Yukseklik"].ToObject<float>();
                float pitch = telemetryData["pitch"].ToObject<float>();
                float roll = telemetryData["roll"].ToObject<float>();
                float yaw = telemetryData["yaw"].ToObject<float>();
                int rhrh = telemetryData["rhrh"].ToObject<int>();
                int iotData = telemetryData["iotData"].ToObject<int>();

                elapsedTime += timeStep;

                this.gpsEnlem = gps1Enlem;
                this.gpsBoylam = gps1Boylam;

                UpdateMap();

                this.Invoke((MethodInvoker)delegate {
                    UpdateChart(gyBasincChart, elapsedTime, basinc1);
                    UpdateChart(tBasincChart, elapsedTime, basinc2);
                    UpdateChart(gyYukseklikChart, elapsedTime, yukseklik1);
                    UpdateChart(tYukseklikChart, elapsedTime, yukseklik2);
                    UpdateChart(irtifaFarkiChart, elapsedTime, irtifaFarki);
                    UpdateChart(inisHiziChart, elapsedTime, inisHizi);
                    UpdateChart(sicaklikChart, elapsedTime, sicaklik);
                    UpdateChart(pilGerilimiChart, elapsedTime, pilGerilimi);

                    UpdateChartTimeRange(gyBasincChart);
                    UpdateChartTimeRange(tBasincChart);
                    UpdateChartTimeRange(gyYukseklikChart);
                    UpdateChartTimeRange(tYukseklikChart);
                    UpdateChartTimeRange(irtifaFarkiChart);
                    UpdateChartTimeRange(inisHiziChart);
                    UpdateChartTimeRange(sicaklikChart);
                    UpdateChartTimeRange(pilGerilimiChart);
                });

                this.Invoke((MethodInvoker)delegate {
                    paketNumarasiText.Text = paketNumarasi.ToString();
                    uyduStatusuText.Text = uyduStatusu.ToString();
                    gondermeSaatiText.Text = gondermeSaati;
                    gyBasincText.Text = basinc1.ToString();
                    tasiyiciBasincText.Text = basinc2.ToString();
                    gyYukseklikText.Text = yukseklik1.ToString();
                    tasiyiciYukseklikText.Text = yukseklik2.ToString();
                    irtifaFarkiText.Text = irtifaFarki.ToString();
                    inisHiziText.Text = inisHizi.ToString();
                    sicaklikText.Text = sicaklik.ToString();
                    pilGerilimiText.Text = pilGerilimi.ToString();
                    latitudeText.Text = gps1Enlem.ToString();
                    longitudeText.Text = gps1Boylam.ToString();
                    altitudeText.Text = gps1Yukseklik.ToString();
                    pitchText.Text = pitch.ToString();
                    rollText.Text = roll.ToString();
                    yawText.Text = yaw.ToString();
                    rhrhText.Text = rhrh.ToString();
                    iotText.Text = iotData.ToString();
                });

                if ((inisHizi < 12 || inisHizi > 14) && uyduStatusu == 2)
                {
                    CheckModelInisHizi();
                }
                else
                {
                    ResetModelInisHizi();
                }

                if ((inisHizi < 6 || inisHizi > 8) && uyduStatusu == 4)
                {
                    CheckGorevInisHizi();
                }
                else
                {
                    ResetGorevInisHizi();
                }

                if (IsDataMissing(basinc2))
                {
                    CheckTasiyiciBasinc();
                }
                else
                {
                    ResetTasiyiciBasinc();
                }

                if (IsDataMissing(gps1Enlem, gps1Boylam, gps1Yukseklik))
                {
                    CheckGorevGps();
                }
                else
                {
                    ResetGorevGps();
                }

                hataKoduDegeri = HataKoduHesapla();
                HataKoduDegerText.Text = hataKoduDegeri;

                SaveTelemetryData(telemetryData, hataKoduDegeri);

                this.x = pitch;
                this.y = roll;
                this.z = yaw;
                uyduGlControl.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri okuma hatası: {ex.Message}");
            }
        }

        private void UpdateChart(Chart chart, double time, double value)
        {
            // Grafik serisine yeni veri noktası ekle
            chart.Series[0].Points.AddXY(time, value);
        }

        private void UpdateChartTimeRange(Chart chart)
        {
            // Zaman aralığını ortada tut
            double minX = Math.Max(0, elapsedTime - 5);
            double maxX = minX + 10;

            chart.ChartAreas[0].AxisX.Minimum = minX;
            chart.ChartAreas[0].AxisX.Maximum = maxX;
        }

        private string HataKoduHesapla()
        {
            int deger = 0;

            if (ModelInisHiziKontrol.Text == "1") deger += 10000;
            if (GorevInisHiziKontrol.Text == "1") deger += 1000;
            if (TasiyiciBasincKontrol.Text == "1") deger += 100;
            if (GorevGpsKontrol.Text == "1") deger += 10;
            if (AyrilmaKontrol.Text == "1") deger += 1;

            return deger.ToString().PadLeft(5, '0');
        }

        // Arduinodan gelen verilerin eksikliğini kontrol eden fonksiyon
        private bool IsDataMissing(params object[] data)
        {
            foreach (var item in data)
            {
                if (item == null || item.Equals(0) || item.Equals(float.NaN) || item.Equals(double.NaN))
                {
                    return true;
                }
            }
            return false;
        }

        private void BaglanButton_Click(object sender, EventArgs e)
        {
            try
            {
                readingPort = new SerialPort(serialPortBox.Text, Convert.ToInt32(boundRateBox.Text));
                readingPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

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

        private void DurdurButton_Click(object sender, EventArgs e)
        {
            readingPort.Close();
            timer1.Stop();
            baglanButton.Enabled = true;
            durdurButton.Enabled = false;
            MessageBox.Show("BAĞLANTI KESİLDİ");
        }

        // Fonksiyonlar
        private void CheckModelInisHizi()
        {
            this.Invoke((MethodInvoker)delegate {
                ModelInisHiziKontrol.Text = "1";
                ModelInisHiziKontrolRenk.BackColor = Color.Red;
            });
            AddHata("Model uydu iniş hızı 12-14 m/s dışında!");
        }

        private void ResetModelInisHizi()
        {
            this.Invoke((MethodInvoker)delegate {
                ModelInisHiziKontrol.Text = "0";
                ModelInisHiziKontrolRenk.BackColor = Color.LawnGreen;
            });
        }

        private void CheckGorevInisHizi()
        {
            this.Invoke((MethodInvoker)delegate {
                GorevInisHiziKontrol.Text = "1";
                GorevInisHiziKontrolRenk.BackColor = Color.Red;
            });
            AddHata("Görev yükü iniş hızı 6-8 m/s dışında!");
        }

        private void ResetGorevInisHizi()
        {
            this.Invoke((MethodInvoker)delegate {
                GorevInisHiziKontrol.Text = "0";
                GorevInisHiziKontrolRenk.BackColor = Color.LawnGreen;
            });
        }

        private void CheckTasiyiciBasinc()
        {
            this.Invoke((MethodInvoker)delegate {
                TasiyiciBasincKontrol.Text = "1";
                TasiyiciBasincKontrolRenk.BackColor = Color.Red;
            });
            AddHata("Taşıyıcı basınç verisi alınamıyor!");
        }

        private void ResetTasiyiciBasinc()
        {
            this.Invoke((MethodInvoker)delegate {
                TasiyiciBasincKontrol.Text = "0";
                TasiyiciBasincKontrolRenk.BackColor = Color.LawnGreen;
            });
        }

        private void CheckGorevGps()
        {
            this.Invoke((MethodInvoker)delegate {
                GorevGpsKontrol.Text = "1";
                GorevGpsKontrolRenk.BackColor = Color.Red;
            });
            AddHata("Görev yükü konum verisi alınamıyor!");
        }

        private void ResetGorevGps()
        {
            this.Invoke((MethodInvoker)delegate {
                GorevGpsKontrol.Text = "0";
                GorevGpsKontrolRenk.BackColor = Color.LawnGreen;
            });
        }

        private void CheckAyrilmaDurumu()
        {
            this.Invoke((MethodInvoker)delegate {
                AyrilmaKontrol.Text = "1";
                AyrilmaKontrolRenk.BackColor = Color.Red;
            });
            AddHata("Ayrılma gerçekleşmedi!");
        }

        private void ResetAyrilmaDurumu()
        {
            this.Invoke((MethodInvoker)delegate {
                AyrilmaKontrol.Text = "0";
                AyrilmaKontrolRenk.BackColor = Color.LawnGreen;
            });
        }

        private void AddHata(string mesaj)
        {
            if (InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate {
                    hataListBox.Items.Add(mesaj);
                });
            }
            else
            {
                hataListBox.Items.Add(mesaj);
            }
        }

        private void GlControl1_Paint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(1.04f, 1, 1, 10000);
            Matrix4 lookat = Matrix4.LookAt(25, 0, 0, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookat);
            GL.Viewport(0, 0, uyduGlControl.Width, uyduGlControl.Height);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            GL.Rotate(x, 1.0, 0.0, 0.0);
            GL.Rotate(y, 0.0, 1.0, 0.0);
            GL.Rotate(z, 0.0, 0.0, 1.0);

            Silindir(1.0f, 1.0f, 5.0f, 3, -5);
            Koni(0.01f, 0.01f, 5.0f, 3.0f, 3, 5);
            Koni(0.01f, 0.01f, 5.0f, 2.0f, -5.0f, -10.0f);

            GL.Begin(PrimitiveType.Lines);
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
            uyduGlControl.SwapBuffers();
        }

        private void GlControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
        }

        private void Silindir(float step, float topla, float radius, float dikey1, float dikey2)
        {
            float eski_step = 0.1f;
            GL.Begin(PrimitiveType.Quads);

            while (step <= 360)
            {
                Color_template(step);
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
            GL.Begin(PrimitiveType.Lines);
            step = eski_step;
            topla = step;

            while (step <= 180)
            {
                Color_template(step);
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
                Color_template(step);
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

        private void Koni(float step, float topla, float radius1, float radius2, float dikey1, float dikey2)
        {
            float eski_step = 0.1f;
            GL.Begin(PrimitiveType.Lines);

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
            GL.Begin(PrimitiveType.Lines);
            step = eski_step;
            topla = step;

            while (step <= 180)
            {
                Color_template(step);
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
            GL.End();
        }

        private void Color_template(float step)
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

        private void SATX_FormClosing(object sender, FormClosingEventArgs e)
        {
            telemetryWriter?.Close(); // Dosya kapanışını unutmuyoruz
            telemetryWriter?.Dispose(); // sonradan eklendi
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                string jsonData = readingPort.ReadLine();
                JObject telemetryData = JObject.Parse(jsonData);

                x = telemetryData["pitch"].ToObject<float>();
                y = telemetryData["roll"].ToObject<float>();
                z = telemetryData["yaw"].ToObject<float>();

                uyduGlControl.Invalidate();

                readingPort.DiscardInBuffer();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veri işleme hatası: {ex.Message}");
            }
        }

        private void TimerXYZ_Tick(object sender, EventArgs e)
        {
            // TimerXYZ'nin tıklama olayı boş
        }
    }
}
