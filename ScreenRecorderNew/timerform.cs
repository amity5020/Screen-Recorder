using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace ScreenRecorderNew
{
    public partial class timerform : Form
    {
        public timerform()
        {
            InitializeComponent();
        }

        private void timerform_Load(object sender, EventArgs e)
        {
           // GetParent();
            this.TransparencyKey = Color.Turquoise;
            this.BackColor = Color.Turquoise;
            if (Program.eRequestFor == RequestFor.ScreenRecording)
            {
                lblTimer.Text = "3";
                timer1.Start();
            }else
            if (RecordVideo.IsRecordLoad)
            {
                RecordVideo recordVideo = new RecordVideo();
                recordVideo.Show();
                timer2.Start();
               
            }else
            {
                timer1.Start();
                lblTimer.Text = "3";
            }
        }

        int count = 3;
        Form1 form1 ;
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (count == 1)
                {
                    if (Program.eRequestFor == RequestFor.ScreenRecording)
                    {
                        form1 = new Form1();
                        Thread thread = new Thread(Getresolution);
                        thread.Start();
                    }
                    lblTimer.Text = "Start";
                }
                else if (count == 0)
                {
                    timer1.Stop(); this.Hide();
                    if (Program.eRequestFor == RequestFor.ScreenRecording)
                    {
                        form1.Show();
                    }
                    else
                    {
                        this.Hide();
                    }
                }
                else
                {
                    lblTimer.Text = (count - 1).ToString();
                }
                count--;
            }catch(Exception ex)
            {
                ClsCommon.WriteLog(ex.Message + " Method:- Timer Tick.");
            }
        }
        
     void Getresolution()
        {
            try
            {
                Process.Start(Application.ExecutablePath.Replace("ScreenRecorder.exe","") +"\\resolution.exe").WaitForExit();
                base.Invoke(new MethodInvoker(() =>
                {
                    form1.StartCamera();
                }));
                ClsCommon.WriteLog(Application.ExecutablePath + " Method:- GetResolution.");
            }
            catch(Exception ex)
            {
                ClsCommon.WriteLog(ex.Message + " Method:- GetResolution.");
            }
        }
        private void timerform_Paint(object sender, PaintEventArgs e)
        {
            //var hb = new HatchBrush(HatchStyle.Percent90, this.TransparencyKey);
            //e.Graphics.FillRectangle(hb, this.DisplayRectangle);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            timer2.Dispose();
            this.Hide();
        }
    }
}
