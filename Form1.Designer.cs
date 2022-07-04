namespace main
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxPATH = new System.Windows.Forms.TextBox();
            this.textBoxxslt = new System.Windows.Forms.TextBox();
            this.button20 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxBR = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxyc = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxFTPUSER = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxFTPPSW = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBoxSTATUS = new System.Windows.Forms.GroupBox();
            this.progressBarstatus = new System.Windows.Forms.ProgressBar();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBoxSTATUS.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 3000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 139);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 17);
            this.label1.TabIndex = 95;
            this.label1.Text = "本地文件夹路径";
            // 
            // textBoxPATH
            // 
            this.textBoxPATH.Location = new System.Drawing.Point(110, 133);
            this.textBoxPATH.Name = "textBoxPATH";
            this.textBoxPATH.Size = new System.Drawing.Size(235, 23);
            this.textBoxPATH.TabIndex = 96;
            this.textBoxPATH.Text = "D:\\ProductFile\\NEW";
            // 
            // textBoxxslt
            // 
            this.textBoxxslt.Location = new System.Drawing.Point(110, 191);
            this.textBoxxslt.Multiline = true;
            this.textBoxxslt.Name = "textBoxxslt";
            this.textBoxxslt.Size = new System.Drawing.Size(235, 64);
            this.textBoxxslt.TabIndex = 106;
            this.textBoxxslt.Text = "C:\\CAMWorksData\\CAMWorks2022x64\\Lang\\English\\Setup_Sheet_Templates\\Mill\\mill oper" +
    "ations.xslt";
            // 
            // button20
            // 
            this.button20.Location = new System.Drawing.Point(13, 261);
            this.button20.Name = "button20";
            this.button20.Size = new System.Drawing.Size(332, 37);
            this.button20.TabIndex = 77;
            this.button20.Text = "定时计算";
            this.button20.UseVisualStyleBackColor = true;
            this.button20.Click += new System.EventHandler(this.button20_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 213);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 17);
            this.label5.TabIndex = 107;
            this.label5.Text = "工艺单模板路径";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 162);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 17);
            this.label2.TabIndex = 108;
            this.label2.Text = "避让文件夹路径";
            // 
            // textBoxBR
            // 
            this.textBoxBR.Location = new System.Drawing.Point(110, 162);
            this.textBoxBR.Name = "textBoxBR";
            this.textBoxBR.Size = new System.Drawing.Size(235, 23);
            this.textBoxBR.TabIndex = 109;
            this.textBoxBR.Text = "D:\\ProductFile\\BIRANGNEW";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 17);
            this.label3.TabIndex = 110;
            this.label3.Text = "远程避让文件夹";
            // 
            // textBoxyc
            // 
            this.textBoxyc.Location = new System.Drawing.Point(110, 16);
            this.textBoxyc.Name = "textBoxyc";
            this.textBoxyc.Size = new System.Drawing.Size(235, 23);
            this.textBoxyc.TabIndex = 111;
            this.textBoxyc.Text = "FTP://192.168.194.202:8848/";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 17);
            this.label4.TabIndex = 112;
            this.label4.Text = "FTP用户名";
            // 
            // textBoxFTPUSER
            // 
            this.textBoxFTPUSER.Location = new System.Drawing.Point(81, 74);
            this.textBoxFTPUSER.Name = "textBoxFTPUSER";
            this.textBoxFTPUSER.Size = new System.Drawing.Size(94, 23);
            this.textBoxFTPUSER.TabIndex = 113;
            this.textBoxFTPUSER.Text = "admin";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(193, 77);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 17);
            this.label6.TabIndex = 114;
            this.label6.Text = "FTP密码";
            // 
            // textBoxFTPPSW
            // 
            this.textBoxFTPPSW.Location = new System.Drawing.Point(251, 74);
            this.textBoxFTPPSW.Name = "textBoxFTPPSW";
            this.textBoxFTPPSW.Size = new System.Drawing.Size(94, 23);
            this.textBoxFTPPSW.TabIndex = 115;
            this.textBoxFTPPSW.Text = "123";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(11, 304);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(163, 44);
            this.button1.TabIndex = 116;
            this.button1.Text = "停止";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Controls.Add(this.textBox3);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.textBoxFTPPSW);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.textBoxFTPUSER);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBoxyc);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textBoxBR);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.button20);
            this.groupBox2.Controls.Add(this.textBoxxslt);
            this.groupBox2.Controls.Add(this.textBoxPATH);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(512, 358);
            this.groupBox2.TabIndex = 92;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "路径配置";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(525, 19);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(95, 44);
            this.button3.TabIndex = 121;
            this.button3.Text = "开发测试按钮";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_2);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(110, 45);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(235, 23);
            this.textBox3.TabIndex = 120;
            this.textBox3.Text = "FTP://192.168.194.202:8848/";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 17);
            this.label7.TabIndex = 119;
            this.label7.Text = "远程检查文件夹";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(360, 16);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(138, 332);
            this.textBox2.TabIndex = 118;
            this.textBox2.Text = resources.GetString("textBox2.Text");
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(182, 304);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(163, 44);
            this.button2.TabIndex = 117;
            this.button2.Text = "出工程图";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBoxSTATUS);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 358);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(512, 476);
            this.panel1.TabIndex = 93;
            // 
            // groupBoxSTATUS
            // 
            this.groupBoxSTATUS.Controls.Add(this.progressBarstatus);
            this.groupBoxSTATUS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxSTATUS.Location = new System.Drawing.Point(0, 434);
            this.groupBoxSTATUS.Name = "groupBoxSTATUS";
            this.groupBoxSTATUS.Size = new System.Drawing.Size(512, 42);
            this.groupBoxSTATUS.TabIndex = 127;
            this.groupBoxSTATUS.TabStop = false;
            this.groupBoxSTATUS.Text = "当前状态:";
            // 
            // progressBarstatus
            // 
            this.progressBarstatus.AccessibleDescription = "";
            this.progressBarstatus.AccessibleName = "";
            this.progressBarstatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBarstatus.Location = new System.Drawing.Point(3, 19);
            this.progressBarstatus.Name = "progressBarstatus";
            this.progressBarstatus.Size = new System.Drawing.Size(506, 20);
            this.progressBarstatus.TabIndex = 123;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(512, 434);
            this.textBox1.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 834);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox2);
            this.Name = "Form1";
            this.Text = "自动计算服务端4.1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBoxSTATUS.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private BindingSource bindingSource1;
        private System.Windows.Forms.Timer timer1;
        private Label label1;
        private TextBox textBoxPATH;
        private TextBox textBoxxslt;
        private Button button20;
        private Label label5;
        private Label label2;
        private TextBox textBoxBR;
        private Label label3;
        private TextBox textBoxyc;
        private Label label4;
        private TextBox textBoxFTPUSER;
        private Label label6;
        private TextBox textBoxFTPPSW;
        private Button button1;
        private GroupBox groupBox2;
        private Panel panel1;
        private Button button2;
        private TextBox textBox2;
        private TextBox textBox3;
        private Label label7;
        private Button button3;
        private GroupBox groupBoxSTATUS;
        private ProgressBar progressBarstatus;
        private TextBox textBox1;
    }
}