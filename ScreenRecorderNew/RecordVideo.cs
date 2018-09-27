using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using NAudio.Wave;
using ScreenRecorderNew.Properties;

namespace ScreenRecorderNew
{
    public partial class RecordVideo : Form
    {
        public bool _recorder=false;
        MainWindowViewModel MainWindowView;
        List<FilterInfo> filterInfos;
        List<WaveInCapabilities> waveInCapabilities;
        public RecordVideo()
        {
            InitializeComponent();
           
        }
      
        void showplay()
        {
            StartForm startForm = new StartForm();
            startForm.ShowDialog();
        }
        void GetDevices()
        {
            MainWindowView = new MainWindowViewModel();
            filterInfos = MainWindowView.GetVideoDevices();
            if(filterInfos==null||filterInfos.Count==0)
            {
                MessageBox.Show("Webcam not available");
                Environment.Exit(1);
            }else
            {
                foreach(var item in filterInfos)
                {
                    cmbWebCamera.Items.Add(item.Name);
                }
                cmbWebCamera.SelectedIndex = 0;
            }

        }
        void GetAudioDevices()
        {
            waveInCapabilities = new List<WaveInCapabilities>();
            int waveInDevices = WaveIn.DeviceCount;

            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {   
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                waveInCapabilities.Add(deviceInfo);
                cmbMicrophone.Items.Add(deviceInfo.ProductName);
               // Console.WriteLine("Device {0}: {1}, {2} channels", waveInDevice, deviceInfo.ProductName, deviceInfo.Channels);
            }if (waveInDevices > 0)
            {
                cmbMicrophone.SelectedIndex = 0;
            }
        }
        //   string CameraUrl;
        bool ClosedByCode = false;
        private void RecordVideo_Load(object sender, EventArgs e)
        {
           // GetParent();
            GetDevices();
            GetAudioDevices();
            startCamera();
            cmbWebCamera.SelectedIndexChanged += cmbWebCamera_SelectedIndexChanged;
           
        }
        private  void parent_exit(object sender, EventArgs e)
        {
            
        }
        private  void GetParent()
        {
            var myId = Process.GetCurrentProcess().Id;
            var query = string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", myId);
            var search = new ManagementObjectSearcher("root\\CIMV2", query);
            var results = search.Get().GetEnumerator();
            results.MoveNext();
            var queryObj = results.Current;
            var parentId = (uint)queryObj["ParentProcessId"];
            var parent = Process.GetProcessById((int)parentId);
            
            MessageBox.Show("I was started by " + parent.ProcessName);

            parent.Exited += parent_exit;
           // parent.WaitForExit();
            //  Console.ReadLine();
        }
        private void RecordVideo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_recorder)
            {
                DialogResult dr = MessageBox.Show("Video is recording. \n Are you sure close Application.", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {

                    timerRecordTime.Enabled = false;
                    timerRecordTime.Stop();
                    try
                    {
                        _recorder = false;
                        this.Text = "Recording Stopped.";
                        MainWindowView.StopRecording();
                      //  MainWindowView.StopCamera();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n\n" + ex.ToString());
                    }
                }
                else
                {
                    e.Cancel = true;
                    return;
                }

            }

            
            if (!ClosedByCode)
            {
                DLOperation dLOperation = new DLOperation();
                dLOperation.SaveEntry(ClsCommon.UserId, "",3);
                MainWindowView.StopCamera();
                Application.Exit();
            }
        }
        public static bool IsRecordLoad = false;
        private void button1_Click(object sender, EventArgs e)
        {   if(_recorder)
            {
               
                timerRecordTime.Stop();
                _recorder = false;
                this.Text = "Recording Stopped.";
                MainWindowView.StopRecording();
                MainWindowView.StopCamera();
                // Thread thread = new Thread(showplay);
                //thread.Start();
                VideoPreview videoPreview = new VideoPreview();
                videoPreview.Show();
                ClosedByCode = true;
                this.Close();
            }
            else
            {
                cmbMicrophone.Enabled = cmbWebCamera.Enabled = false;
                _recorder = true;
                //Thread tr= new Thread(startCamera);
                //tr.Start();
                IsRecordLoad = false;
                timerform timerform = new timerform();
                timerform.ShowDialog();
                button1.BackgroundImage = Resources.StopRecording2;
                this.Text = "Recording...";
                bool isaudio = waveInCapabilities.Count > 0 ? true : false;
                MainWindowView.StartRecording(isaudio,cmbMicrophone.SelectedIndex,2);
                timerRecordTime.Start();
                timerRecordTime.Interval=1000;
            }
            //timerRecordTime.Stop();
            //timer1.Start();
        }
        void startCamera()
        {
            bool isaudio = waveInCapabilities.Count > 0 ? true : false;
            MainWindowView = new MainWindowViewModel();
            MainWindowView.IsIpCameraSource = false;
            MainWindowView.IsDesktopSource = false;
            MainWindowView.IsWebcamSource = true;
            pictureBox1.Invoke(new MethodInvoker (()=> { MainWindowView.StartCamera(filterInfos[cmbWebCamera.SelectedIndex], this.pictureBox1); }));
           
        }
       int RecordButton = 0;
        private void timerRecordTime_Tick(object sender, EventArgs e)
        {
            RecordButton++;
            TimeSpan times = TimeSpan.FromSeconds(RecordButton);
            string str = times.ToString(@"hh\:mm\:ss");
            lblRecordTime.Text = str;
        }
        int btnShowCount = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_recorder)
            {
                btnShowCount++;
                if (btnShowCount > 4)
                {
                    timer1.Stop();
                    button1.Hide();
                }
            }
            // mergeAudioVideoFile();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void cmbWebCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainWindowView.StopCamera();
            startCamera();
        }
    }
}
