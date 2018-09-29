using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using NAudio.Wave;
using ScreenRecorderNew.Properties;

namespace ScreenRecorderNew
{
    public partial class Form1 : Form
    {
        bool isrecording, istopped = false;
        MainWindowViewModel MainWindowViewModel;
        int time = 0;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            float hfact=1, wfact=1;
            if (File.Exists(Program.Localpath + "\\resolution.txt"))
            {
                var str = File.ReadAllText(Program.Localpath + "\\resolution.txt").Split('_');
                Program.width = int.Parse(str[0]);
                Program.height = int.Parse(str[1]);
                wfact = float.Parse(str[2]);
                hfact = float.Parse(str[3]);
            }
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(int.Parse(((float)workingArea.Right * wfact - (float)Size.Width * wfact).ToString().Split('.')[0])/ 2,
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
                VideoPreview videoPreview = new VideoPreview();
                videoPreview.Show();
                closedByCode = true;
                this.Close();
            }
            else
            {
                Thread thread = new Thread(StartRecord);
                thread.Start();
            }
        }
        void StartRecord()
        {
            base.Invoke(new MethodInvoker(() =>
            {
                isrecording = true;
                btnStartStop.BackgroundImage = Resources.StopRecording1;
                this.Text = "Recording...";
                if (Program.IsRecordAgain)
                {
                    this.Text = "Recording Again...";
                }
                DeviceCount = WaveIn.DeviceCount;
                bool isaudio = DeviceCount > 0 ? true : false;
                MainWindowViewModel.StartRecording(isaudio, 0, 2);
                timer1.Interval = 1000;
                timer1.Start();
            }));
        }
        public void StartCamera()
        {
            MainWindowViewModel = new MainWindowViewModel
            {
                IsDesktopSource = true,
                IsIpCameraSource = false,
                IsWebcamSource = false
            };
            MainWindowViewModel.StartCamera();
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
                DLOperation dLOperation = new DLOperation();
                dLOperation.SaveEntry(ClsCommon.UserId, "",3);
                Environment.Exit(1); 
            }
        }
        private void btnStartStop_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Stop Recording", btnStartStop);
        }
        void showplay()
        {
            StartForm startForm = new StartForm();
            startForm.ShowDialog();
        }
    }
}
