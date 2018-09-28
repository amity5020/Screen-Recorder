using System;

namespace ScreenRecorderNew
{
    partial class RecordVideo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecordVideo));
            this.lblRecordTime = new System.Windows.Forms.Label();
            this.timerRecordTime = new System.Windows.Forms.Timer(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cmbWebCamera = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbMicrophone = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblRecordTime
            // 
            this.lblRecordTime.AutoSize = true;
            this.lblRecordTime.Location = new System.Drawing.Point(12, 368);
            this.lblRecordTime.Name = "lblRecordTime";
            this.lblRecordTime.Size = new System.Drawing.Size(0, 20);
            this.lblRecordTime.TabIndex = 1;
            // 
            // timerRecordTime
            // 
            this.timerRecordTime.Interval = 1000;
            this.timerRecordTime.Tick += new System.EventHandler(this.timerRecordTime_Tick);
            // 
            // timer1
            // 
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // cmbWebCamera
            // 
            this.cmbWebCamera.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWebCamera.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbWebCamera.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbWebCamera.FormattingEnabled = true;
            this.cmbWebCamera.Location = new System.Drawing.Point(68, 5);
            this.cmbWebCamera.Name = "cmbWebCamera";
            this.cmbWebCamera.Size = new System.Drawing.Size(203, 24);
            this.cmbWebCamera.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-1, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "Webcam";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(2, 34);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(535, 293);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.BackgroundImage = global::ScreenRecorderNew.Properties.Resources.Start;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(221, 324);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 62);
            this.button1.TabIndex = 20;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(272, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Microphone";
            // 
            // cmbMicrophone
            // 
            this.cmbMicrophone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMicrophone.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbMicrophone.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbMicrophone.FormattingEnabled = true;
            this.cmbMicrophone.Location = new System.Drawing.Point(360, 5);
            this.cmbMicrophone.Name = "cmbMicrophone";
            this.cmbMicrophone.Size = new System.Drawing.Size(178, 24);
            this.cmbMicrophone.TabIndex = 6;
            // 
            // RecordVideo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(540, 390);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbMicrophone);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbWebCamera);
            this.Controls.Add(this.lblRecordTime);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "RecordVideo";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Record Video";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RecordVideo_FormClosing);
            this.Load += new System.EventHandler(this.RecordVideo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

      

        #endregion

        private System.Windows.Forms.Label lblRecordTime;
        private System.Windows.Forms.Timer timerRecordTime;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ComboBox cmbWebCamera;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbMicrophone;
    }
}