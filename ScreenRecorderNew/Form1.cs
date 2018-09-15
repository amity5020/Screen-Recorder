using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using ScreenRecorderNew.Properties;

namespace ScreenRecorderNew
{
    public partial class Form1 : Form
    {
        bool isrecording, istopped = false;
        //public static string Localpath;
        MainWindowViewModel MainWindowViewModel;
        int time = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point((workingArea.Right - Size.Width) / 2,
                                      50);
            btnStartStop_Click(sender, e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            time++;
            TimeSpan times = TimeSpan.FromSeconds(time);
            string str = times.ToString(@"hh\:mm\:ss");
            lblTime.Text = str;
        }
        void ShowPreview()
        {
            VideoPreview videoPreview = new VideoPreview();
            videoPreview.ShowDialog();
        }
        int DeviceCount = 0;
        bool closedByCode = false;
        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (isrecording)
            {
                isrecording = false;
                istopped = true;
                this.Text = "Recording Stopped.";
                timer1.Stop();
                time = 0;
                timer1.Dispose();
                MainWindowViewModel.StopRecording();
                MainWindowViewModel.Dispose();
                MainWindowViewModel = null;
                // this.Dispose();
                VideoPreview videoPreview = new VideoPreview();
                videoPreview.Show();
                closedByCode = true;
                this.Close();
               
                //Process.Start(Program.Localpath+"//index.html");
                //Thread thread = new Thread(showplay);
                //thread.Start();
                //this.Hide();
            }
            else
            {
                isrecording = true;
               // timerform timerform = new timerform();
               // timerform.ShowDialog();
                btnStartStop.BackgroundImage = Resources.StopRecording1;
                this.Text = "Recording...";
                DeviceCount = WaveIn.DeviceCount;
                bool isaudio = DeviceCount > 0 ? true : false;
                MainWindowViewModel = new MainWindowViewModel();
                MainWindowViewModel.IsDesktopSource = true;
                MainWindowViewModel.IsIpCameraSource = false;
                MainWindowViewModel.IsWebcamSource = false;
                MainWindowViewModel.StartCamera();
                MainWindowViewModel.StartRecording(isaudio,0,2);
                timer1.Interval = 1000;
                timer1.Start();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isrecording)
            {
                DialogResult dr = MessageBox.Show("Screen is recording. \nAre you sure close Application.", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    isrecording = false;
                    istopped = true;
                    this.Text = "Recording Stopped.";
                    timer1.Stop();
                    time = 0;
                    MainWindowViewModel.StopRecording();
                   
                }
                else
                {
                    e.Cancel=true;
                    return;
                }
            }
            if (!closedByCode)
            {
                Environment.Exit(1);
            }
        }

        void showplay()
        {
            StartForm startForm = new StartForm();
            startForm.ShowDialog();
        }
    }
}
