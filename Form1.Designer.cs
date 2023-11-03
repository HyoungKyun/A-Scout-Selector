using System.Drawing;
using System.Windows.Forms;


namespace A_Scout_Viewer
{    

    partial class A_Scout : MetroFramework.Forms.MetroForm //상속 클래스 변경 
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.btPreview = new MetroFramework.Controls.MetroButton();
            this.btStop = new MetroFramework.Controls.MetroButton();
            this.TileState = new MetroFramework.Controls.MetroTile();
            this.lbFrameRate = new MetroFramework.Drawing.Html.HtmlLabel();
            this.tbFrameRate = new MetroFramework.Controls.MetroTrackBar();
            this.tbISO = new MetroFramework.Controls.MetroTrackBar();
            this.lbISO = new MetroFramework.Drawing.Html.HtmlLabel();
            this.tbExposure = new MetroFramework.Controls.MetroTrackBar();
            this.lbExposure = new MetroFramework.Drawing.Html.HtmlLabel();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lbFPS = new MetroFramework.Drawing.Html.HtmlLabel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.metroCBCam3 = new MetroFramework.Controls.MetroCheckBox();
            this.metroCBCam2 = new MetroFramework.Controls.MetroCheckBox();
            this.metroCBCam1 = new MetroFramework.Controls.MetroCheckBox();
            this.lbFPS2 = new MetroFramework.Drawing.Html.HtmlLabel();
            this.lbFPS3 = new MetroFramework.Drawing.Html.HtmlLabel();
            this.btSettingSave = new MetroFramework.Controls.MetroButton();
            this.cbCam1Pos = new MetroFramework.Controls.MetroComboBox();
            this.cbCam2Pos = new MetroFramework.Controls.MetroComboBox();
            this.cbCam3Pos = new MetroFramework.Controls.MetroComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.metroRadioButton3 = new MetroFramework.Controls.MetroRadioButton();
            this.metroRadioButton2 = new MetroFramework.Controls.MetroRadioButton();
            this.metroRadioButton1 = new MetroFramework.Controls.MetroRadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btPreview
            // 
            this.btPreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btPreview.FontSize = MetroFramework.MetroButtonSize.Medium;
            this.btPreview.Location = new System.Drawing.Point(20, 443);
            this.btPreview.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btPreview.Name = "btPreview";
            this.btPreview.Size = new System.Drawing.Size(180, 45);
            this.btPreview.TabIndex = 0;
            this.btPreview.Text = "Live View";
            this.btPreview.UseSelectable = true;
            this.btPreview.Click += new System.EventHandler(this.btPreview_Click);
            // 
            // btStop
            // 
            this.btStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btStop.FontSize = MetroFramework.MetroButtonSize.Medium;
            this.btStop.Location = new System.Drawing.Point(20, 520);
            this.btStop.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btStop.Name = "btStop";
            this.btStop.Size = new System.Drawing.Size(180, 45);
            this.btStop.TabIndex = 2;
            this.btStop.Text = "Live Stop";
            this.btStop.UseSelectable = true;
            this.btStop.Click += new System.EventHandler(this.btStop_Click);
            // 
            // TileState
            // 
            this.TileState.ActiveControl = null;
            this.TileState.Location = new System.Drawing.Point(20, 82);
            this.TileState.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TileState.Name = "TileState";
            this.TileState.Size = new System.Drawing.Size(180, 120);
            this.TileState.TabIndex = 0;
            this.TileState.Text = "Camera State";
            this.TileState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.TileState.TileImage = global::A_Scout_Viewer.Properties.Resources.Golf;
            this.TileState.TileImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.TileState.UseSelectable = true;
            this.TileState.Click += new System.EventHandler(this.TileState_Click);
            // 
            // lbFrameRate
            // 
            this.lbFrameRate.AutoScroll = true;
            this.lbFrameRate.AutoScrollMinSize = new System.Drawing.Size(137, 39);
            this.lbFrameRate.AutoSize = false;
            this.lbFrameRate.BackColor = System.Drawing.SystemColors.Window;
            this.lbFrameRate.Font = new System.Drawing.Font("Arial Black", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbFrameRate.Location = new System.Drawing.Point(87, 94);
            this.lbFrameRate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lbFrameRate.Name = "lbFrameRate";
            this.lbFrameRate.Size = new System.Drawing.Size(408, 50);
            this.lbFrameRate.TabIndex = 9;
            this.lbFrameRate.Text = "Frame Rate";
            // 
            // tbFrameRate
            // 
            this.tbFrameRate.BackColor = System.Drawing.Color.Transparent;
            this.tbFrameRate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tbFrameRate.Location = new System.Drawing.Point(87, 140);
            this.tbFrameRate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbFrameRate.Name = "tbFrameRate";
            this.tbFrameRate.Size = new System.Drawing.Size(407, 38);
            this.tbFrameRate.TabIndex = 10;
            this.tbFrameRate.Text = "Frame Rate";
            this.tbFrameRate.Scroll += new System.Windows.Forms.ScrollEventHandler(this.tbFrameRate_Scroll);
            // 
            // tbISO
            // 
            this.tbISO.BackColor = System.Drawing.Color.Transparent;
            this.tbISO.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tbISO.Location = new System.Drawing.Point(87, 255);
            this.tbISO.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbISO.Name = "tbISO";
            this.tbISO.Size = new System.Drawing.Size(407, 38);
            this.tbISO.TabIndex = 12;
            this.tbISO.Text = "ISO";
            this.tbISO.Scroll += new System.Windows.Forms.ScrollEventHandler(this.tbISO_Scroll);
            // 
            // lbISO
            // 
            this.lbISO.AutoScroll = true;
            this.lbISO.AutoScrollMinSize = new System.Drawing.Size(50, 39);
            this.lbISO.AutoSize = false;
            this.lbISO.BackColor = System.Drawing.SystemColors.Window;
            this.lbISO.Font = new System.Drawing.Font("Arial Black", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbISO.Location = new System.Drawing.Point(87, 213);
            this.lbISO.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lbISO.Name = "lbISO";
            this.lbISO.Size = new System.Drawing.Size(408, 50);
            this.lbISO.TabIndex = 11;
            this.lbISO.Text = "ISO";
            // 
            // tbExposure
            // 
            this.tbExposure.BackColor = System.Drawing.Color.Transparent;
            this.tbExposure.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tbExposure.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.tbExposure.Location = new System.Drawing.Point(87, 377);
            this.tbExposure.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbExposure.Name = "tbExposure";
            this.tbExposure.Size = new System.Drawing.Size(407, 38);
            this.tbExposure.TabIndex = 14;
            this.tbExposure.Text = "ISO";
            this.tbExposure.Scroll += new System.Windows.Forms.ScrollEventHandler(this.tbExposure_Scroll);
            // 
            // lbExposure
            // 
            this.lbExposure.AutoScroll = true;
            this.lbExposure.AutoScrollMinSize = new System.Drawing.Size(173, 39);
            this.lbExposure.AutoSize = false;
            this.lbExposure.BackColor = System.Drawing.SystemColors.Window;
            this.lbExposure.Font = new System.Drawing.Font("Arial Black", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbExposure.Location = new System.Drawing.Point(87, 334);
            this.lbExposure.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lbExposure.Name = "lbExposure";
            this.lbExposure.Size = new System.Drawing.Size(408, 50);
            this.lbExposure.TabIndex = 13;
            this.lbExposure.Text = "Exposure Time";
            // 
            // metroPanel1
            // 
            this.metroPanel1.BackgroundImage = global::A_Scout_Viewer.Properties.Resources.AIVIWORKS_CI_1;
            this.metroPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(20, 961);
            this.metroPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Size = new System.Drawing.Size(180, 40);
            this.metroPanel1.TabIndex = 0;
            this.metroPanel1.TabStop = true;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(225, 85);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(600, 412);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox_DoubleClick);
            // 
            // lbFPS
            // 
            this.lbFPS.AutoScroll = true;
            this.lbFPS.AutoScrollMinSize = new System.Drawing.Size(118, 33);
            this.lbFPS.AutoSize = false;
            this.lbFPS.BackColor = System.Drawing.SystemColors.Window;
            this.lbFPS.Font = new System.Drawing.Font("Arial Black", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbFPS.Location = new System.Drawing.Point(569, 503);
            this.lbFPS.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lbFPS.Name = "lbFPS";
            this.lbFPS.Size = new System.Drawing.Size(256, 50);
            this.lbFPS.TabIndex = 14;
            this.lbFPS.Text = "Cam1 FPS : ";
            this.lbFPS.Click += new System.EventHandler(this.lbFPS_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Location = new System.Drawing.Point(863, 85);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(600, 412);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 19;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            this.pictureBox2.DoubleClick += new System.EventHandler(this.pictureBox_DoubleClick);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox3.Location = new System.Drawing.Point(225, 562);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(600, 412);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 20;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.DoubleClick += new System.EventHandler(this.pictureBox_DoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.metroCBCam3);
            this.groupBox1.Controls.Add(this.metroCBCam2);
            this.groupBox1.Controls.Add(this.metroCBCam1);
            this.groupBox1.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox1.Location = new System.Drawing.Point(20, 228);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(195, 182);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Camera List";
            // 
            // metroCBCam3
            // 
            this.metroCBCam3.AutoSize = true;
            this.metroCBCam3.FontSize = MetroFramework.MetroCheckBoxSize.Tall;
            this.metroCBCam3.Location = new System.Drawing.Point(28, 130);
            this.metroCBCam3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.metroCBCam3.Name = "metroCBCam3";
            this.metroCBCam3.Size = new System.Drawing.Size(103, 25);
            this.metroCBCam3.TabIndex = 2;
            this.metroCBCam3.Text = "Camera 3";
            this.metroCBCam3.UseSelectable = true;
            this.metroCBCam3.CheckedChanged += new System.EventHandler(this.metroCBCam3_CheckedChanged);
            // 
            // metroCBCam2
            // 
            this.metroCBCam2.AutoSize = true;
            this.metroCBCam2.FontSize = MetroFramework.MetroCheckBoxSize.Tall;
            this.metroCBCam2.Location = new System.Drawing.Point(28, 86);
            this.metroCBCam2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.metroCBCam2.Name = "metroCBCam2";
            this.metroCBCam2.Size = new System.Drawing.Size(103, 25);
            this.metroCBCam2.TabIndex = 1;
            this.metroCBCam2.Text = "Camera 2";
            this.metroCBCam2.UseSelectable = true;
            this.metroCBCam2.CheckedChanged += new System.EventHandler(this.metroCBCam2_CheckedChanged);
            // 
            // metroCBCam1
            // 
            this.metroCBCam1.AutoSize = true;
            this.metroCBCam1.FontSize = MetroFramework.MetroCheckBoxSize.Tall;
            this.metroCBCam1.Location = new System.Drawing.Point(28, 42);
            this.metroCBCam1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.metroCBCam1.Name = "metroCBCam1";
            this.metroCBCam1.Size = new System.Drawing.Size(103, 25);
            this.metroCBCam1.TabIndex = 0;
            this.metroCBCam1.Text = "Camera 1";
            this.metroCBCam1.UseSelectable = true;
            this.metroCBCam1.CheckedChanged += new System.EventHandler(this.metroCBCam1_CheckedChanged);
            // 
            // lbFPS2
            // 
            this.lbFPS2.AutoScroll = true;
            this.lbFPS2.AutoScrollMinSize = new System.Drawing.Size(118, 33);
            this.lbFPS2.AutoSize = false;
            this.lbFPS2.BackColor = System.Drawing.SystemColors.Window;
            this.lbFPS2.Font = new System.Drawing.Font("Arial Black", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbFPS2.Location = new System.Drawing.Point(1207, 503);
            this.lbFPS2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lbFPS2.Name = "lbFPS2";
            this.lbFPS2.Size = new System.Drawing.Size(256, 50);
            this.lbFPS2.TabIndex = 15;
            this.lbFPS2.Text = "Cam2 FPS : ";
            this.lbFPS2.Click += new System.EventHandler(this.lbFPS2_Click);
            // 
            // lbFPS3
            // 
            this.lbFPS3.AutoScroll = true;
            this.lbFPS3.AutoScrollMinSize = new System.Drawing.Size(118, 33);
            this.lbFPS3.AutoSize = false;
            this.lbFPS3.BackColor = System.Drawing.SystemColors.Window;
            this.lbFPS3.Font = new System.Drawing.Font("Arial Black", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbFPS3.Location = new System.Drawing.Point(569, 978);
            this.lbFPS3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lbFPS3.Name = "lbFPS3";
            this.lbFPS3.Size = new System.Drawing.Size(256, 50);
            this.lbFPS3.TabIndex = 16;
            this.lbFPS3.Text = "Cam3 FPS : ";
            // 
            // btSettingSave
            // 
            this.btSettingSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btSettingSave.FontSize = MetroFramework.MetroButtonSize.Medium;
            this.btSettingSave.Location = new System.Drawing.Point(20, 604);
            this.btSettingSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btSettingSave.Name = "btSettingSave";
            this.btSettingSave.Size = new System.Drawing.Size(180, 45);
            this.btSettingSave.TabIndex = 21;
            this.btSettingSave.Text = "Save Setting";
            this.btSettingSave.UseSelectable = true;
            this.btSettingSave.Click += new System.EventHandler(this.btSettingSave_Click);
            // 
            // cbCam1Pos
            // 
            this.cbCam1Pos.FormattingEnabled = true;
            this.cbCam1Pos.ItemHeight = 23;
            this.cbCam1Pos.Location = new System.Drawing.Point(225, 512);
            this.cbCam1Pos.Name = "cbCam1Pos";
            this.cbCam1Pos.Size = new System.Drawing.Size(307, 29);
            this.cbCam1Pos.TabIndex = 22;
            this.cbCam1Pos.UseSelectable = true;
            // 
            // cbCam2Pos
            // 
            this.cbCam2Pos.FormattingEnabled = true;
            this.cbCam2Pos.ItemHeight = 23;
            this.cbCam2Pos.Location = new System.Drawing.Point(863, 512);
            this.cbCam2Pos.Name = "cbCam2Pos";
            this.cbCam2Pos.Size = new System.Drawing.Size(307, 29);
            this.cbCam2Pos.TabIndex = 23;
            this.cbCam2Pos.UseSelectable = true;
            // 
            // cbCam3Pos
            // 
            this.cbCam3Pos.FormattingEnabled = true;
            this.cbCam3Pos.ItemHeight = 23;
            this.cbCam3Pos.Location = new System.Drawing.Point(225, 988);
            this.cbCam3Pos.Name = "cbCam3Pos";
            this.cbCam3Pos.Size = new System.Drawing.Size(307, 29);
            this.cbCam3Pos.TabIndex = 24;
            this.cbCam3Pos.UseSelectable = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.metroRadioButton3);
            this.groupBox2.Controls.Add(this.metroRadioButton2);
            this.groupBox2.Controls.Add(this.metroRadioButton1);
            this.groupBox2.Controls.Add(this.tbExposure);
            this.groupBox2.Controls.Add(this.tbISO);
            this.groupBox2.Controls.Add(this.lbExposure);
            this.groupBox2.Controls.Add(this.tbFrameRate);
            this.groupBox2.Controls.Add(this.lbISO);
            this.groupBox2.Controls.Add(this.lbFrameRate);
            this.groupBox2.Font = new System.Drawing.Font("Arial Black", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox2.Location = new System.Drawing.Point(863, 568);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(599, 449);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Camera Setting";
            // 
            // metroRadioButton3
            // 
            this.metroRadioButton3.AutoSize = true;
            this.metroRadioButton3.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.metroRadioButton3.Location = new System.Drawing.Point(375, 54);
            this.metroRadioButton3.Name = "metroRadioButton3";
            this.metroRadioButton3.Size = new System.Drawing.Size(84, 19);
            this.metroRadioButton3.TabIndex = 17;
            this.metroRadioButton3.Text = "Camera 3";
            this.metroRadioButton3.UseSelectable = true;
            // 
            // metroRadioButton2
            // 
            this.metroRadioButton2.AutoSize = true;
            this.metroRadioButton2.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.metroRadioButton2.Location = new System.Drawing.Point(230, 54);
            this.metroRadioButton2.Name = "metroRadioButton2";
            this.metroRadioButton2.Size = new System.Drawing.Size(84, 19);
            this.metroRadioButton2.TabIndex = 16;
            this.metroRadioButton2.Text = "Camera 2";
            this.metroRadioButton2.UseSelectable = true;
            // 
            // metroRadioButton1
            // 
            this.metroRadioButton1.AutoSize = true;
            this.metroRadioButton1.FontSize = MetroFramework.MetroCheckBoxSize.Medium;
            this.metroRadioButton1.Location = new System.Drawing.Point(85, 54);
            this.metroRadioButton1.Name = "metroRadioButton1";
            this.metroRadioButton1.Size = new System.Drawing.Size(84, 19);
            this.metroRadioButton1.TabIndex = 15;
            this.metroRadioButton1.Text = "Camera 1";
            this.metroRadioButton1.UseSelectable = true;
            // 
            // A_Scout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1510, 1046);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.cbCam3Pos);
            this.Controls.Add(this.cbCam2Pos);
            this.Controls.Add(this.cbCam1Pos);
            this.Controls.Add(this.btSettingSave);
            this.Controls.Add(this.lbFPS3);
            this.Controls.Add(this.lbFPS2);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lbFPS);
            this.Controls.Add(this.TileState);
            this.Controls.Add(this.metroPanel1);
            this.Controls.Add(this.btStop);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btPreview);
            this.Font = new System.Drawing.Font("Arial Black", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "A_Scout";
            this.Padding = new System.Windows.Forms.Padding(38, 100, 38, 32);
            this.Text = "A-Scout Selector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.A_Scout_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroButton btPreview;
        private System.Windows.Forms.PictureBox pictureBox1;
        private MetroFramework.Controls.MetroButton btStop;
        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroTile TileState;
        private MetroFramework.Drawing.Html.HtmlLabel lbFrameRate;
        private MetroFramework.Controls.MetroTrackBar tbFrameRate;
        private MetroFramework.Controls.MetroTrackBar tbISO;
        private MetroFramework.Drawing.Html.HtmlLabel lbISO;
        private MetroFramework.Controls.MetroTrackBar tbExposure;
        private MetroFramework.Drawing.Html.HtmlLabel lbExposure;
        private MetroFramework.Drawing.Html.HtmlLabel lbFPS;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private GroupBox groupBox1;
        private MetroFramework.Controls.MetroCheckBox metroCBCam2;
        private MetroFramework.Controls.MetroCheckBox metroCBCam1;
        private MetroFramework.Controls.MetroCheckBox metroCBCam3;
        private MetroFramework.Drawing.Html.HtmlLabel lbFPS2;
        private MetroFramework.Drawing.Html.HtmlLabel lbFPS3;
        private MetroFramework.Controls.MetroButton btSettingSave;
        private MetroFramework.Controls.MetroComboBox cbCam1Pos;
        private MetroFramework.Controls.MetroComboBox cbCam2Pos;
        private MetroFramework.Controls.MetroComboBox cbCam3Pos;
        private GroupBox groupBox2;
        private MetroFramework.Controls.MetroRadioButton metroRadioButton3;
        private MetroFramework.Controls.MetroRadioButton metroRadioButton2;
        private MetroFramework.Controls.MetroRadioButton metroRadioButton1;
    }
}

