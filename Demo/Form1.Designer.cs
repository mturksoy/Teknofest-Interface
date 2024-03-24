namespace Demo
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.glControl1 = new OpenTK.GLControl();
            this.labelx = new System.Windows.Forms.Label();
            this.labelxx = new System.Windows.Forms.Label();
            this.labely = new System.Windows.Forms.Label();
            this.labelyy = new System.Windows.Forms.Label();
            this.labelz = new System.Windows.Forms.Label();
            this.labelzz = new System.Windows.Forms.Label();
            this.baslatButton = new System.Windows.Forms.Button();
            this.durdurButton = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.TimerXYZ = new System.Windows.Forms.Timer(this.components);
            this.readingPort = new System.IO.Ports.SerialPort(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.serialPortBox = new System.Windows.Forms.ComboBox();
            this.boundRateBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // glControl1
            // 
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Location = new System.Drawing.Point(12, 12);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(340, 303);
            this.glControl1.TabIndex = 0;
            this.glControl1.VSync = false;
            this.glControl1.Load += new System.EventHandler(this.glControl1_Load);
            this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
            // 
            // labelx
            // 
            this.labelx.AutoSize = true;
            this.labelx.Location = new System.Drawing.Point(407, 161);
            this.labelx.Name = "labelx";
            this.labelx.Size = new System.Drawing.Size(14, 13);
            this.labelx.TabIndex = 4;
            this.labelx.Text = "X";
            // 
            // labelxx
            // 
            this.labelxx.AutoSize = true;
            this.labelxx.Location = new System.Drawing.Point(398, 211);
            this.labelxx.Name = "labelxx";
            this.labelxx.Size = new System.Drawing.Size(35, 13);
            this.labelxx.TabIndex = 5;
            this.labelxx.Text = "label4";
            // 
            // labely
            // 
            this.labely.AutoSize = true;
            this.labely.Location = new System.Drawing.Point(491, 161);
            this.labely.Name = "labely";
            this.labely.Size = new System.Drawing.Size(14, 13);
            this.labely.TabIndex = 6;
            this.labely.Text = "Y";
            // 
            // labelyy
            // 
            this.labelyy.AutoSize = true;
            this.labelyy.Location = new System.Drawing.Point(481, 211);
            this.labelyy.Name = "labelyy";
            this.labelyy.Size = new System.Drawing.Size(35, 13);
            this.labelyy.TabIndex = 7;
            this.labelyy.Text = "label6";
            // 
            // labelz
            // 
            this.labelz.AutoSize = true;
            this.labelz.Location = new System.Drawing.Point(580, 161);
            this.labelz.Name = "labelz";
            this.labelz.Size = new System.Drawing.Size(14, 13);
            this.labelz.TabIndex = 8;
            this.labelz.Text = "Z";
            this.labelz.Click += new System.EventHandler(this.label7_Click);
            // 
            // labelzz
            // 
            this.labelzz.AutoSize = true;
            this.labelzz.Location = new System.Drawing.Point(568, 211);
            this.labelzz.Name = "labelzz";
            this.labelzz.Size = new System.Drawing.Size(35, 13);
            this.labelzz.TabIndex = 9;
            this.labelzz.Text = "label8";
            // 
            // baslatButton
            // 
            this.baslatButton.Location = new System.Drawing.Point(410, 101);
            this.baslatButton.Name = "baslatButton";
            this.baslatButton.Size = new System.Drawing.Size(75, 23);
            this.baslatButton.TabIndex = 11;
            this.baslatButton.Text = "BASLAT";
            this.baslatButton.UseVisualStyleBackColor = true;
            this.baslatButton.Click += new System.EventHandler(this.baslatButton_Click);
            // 
            // durdurButton
            // 
            this.durdurButton.Location = new System.Drawing.Point(519, 101);
            this.durdurButton.Name = "durdurButton";
            this.durdurButton.Size = new System.Drawing.Size(75, 23);
            this.durdurButton.TabIndex = 12;
            this.durdurButton.Text = "DURDUR";
            this.durdurButton.UseVisualStyleBackColor = true;
            this.durdurButton.Click += new System.EventHandler(this.durdurButton_Click);
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // TimerXYZ
            // 
            this.TimerXYZ.Tick += new System.EventHandler(this.TimerXYZ_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(370, 251);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "X";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(481, 256);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "label3";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(370, 280);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 15;
            this.button2.Text = "Y";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(370, 309);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 16;
            this.button3.Text = "Z";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(481, 290);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "label4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(481, 319);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "label5";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(378, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Serial Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(378, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Bound Rate";
            // 
            // serialPortBox
            // 
            this.serialPortBox.FormattingEnabled = true;
            this.serialPortBox.Location = new System.Drawing.Point(451, 12);
            this.serialPortBox.Name = "serialPortBox";
            this.serialPortBox.Size = new System.Drawing.Size(121, 21);
            this.serialPortBox.TabIndex = 21;
            // 
            // boundRateBox
            // 
            this.boundRateBox.Location = new System.Drawing.Point(451, 54);
            this.boundRateBox.Name = "boundRateBox";
            this.boundRateBox.Size = new System.Drawing.Size(121, 20);
            this.boundRateBox.TabIndex = 22;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1192, 666);
            this.Controls.Add(this.boundRateBox);
            this.Controls.Add(this.serialPortBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.durdurButton);
            this.Controls.Add(this.baslatButton);
            this.Controls.Add(this.labelzz);
            this.Controls.Add(this.labelz);
            this.Controls.Add(this.labelyy);
            this.Controls.Add(this.labely);
            this.Controls.Add(this.labelxx);
            this.Controls.Add(this.labelx);
            this.Controls.Add(this.glControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenTK.GLControl glControl1;
        private System.Windows.Forms.Label labelx;
        private System.Windows.Forms.Label labelxx;
        private System.Windows.Forms.Label labely;
        private System.Windows.Forms.Label labelyy;
        private System.Windows.Forms.Label labelz;
        private System.Windows.Forms.Label labelzz;
        private System.Windows.Forms.Button baslatButton;
        private System.Windows.Forms.Button durdurButton;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Timer TimerXYZ;
        private System.IO.Ports.SerialPort readingPort;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox serialPortBox;
        private System.Windows.Forms.TextBox boundRateBox;
    }
}

