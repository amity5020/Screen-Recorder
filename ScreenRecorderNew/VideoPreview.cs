using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace ScreenRecorderNew
{
    public partial class VideoPreview : Form
    {
        public VideoPreview()
        {
            InitializeComponent();
        }
        private void VideoPreview_Load(object sender, EventArgs e)
        {
            Thread thread = new Thread(StartPlay);
            thread.Start();
        }
        void StartPlay()
        {
            base.Invoke(new MethodInvoker(()=>{
            axWindowsMediaPlayer1.URL = Program.Localpath + "\\output.mp4";
            axWindowsMediaPlayer1.settings.volume = 100;
            progressBar1.Hide();
            lblProgress.Hide();
            }));
        }
        bool isclosedbycode = false;
        private void btnRecordAgain_Click(object sender, EventArgs e)
        {
            Program.IsRecordAgain = true;
            RecordVideo.IsRecordLoad = true;
            timerform form1 = new timerform();
            form1.Show();
            isclosedbycode = true;
            this.Close();
        }
        private void VideoPreview_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isclosedbycode)
            {
                DialogResult dialog = MessageBox.Show("Your recording will not be saved. Continue?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialog == DialogResult.Yes)
                {
                    if (CheckForInternetConnection())
                    {
                        DLOperation dLOperations = new DLOperation();
                        dLOperations.SaveEntry(ClsCommon.UserId, "",3);
                    }
                    Environment.Exit(1);
                }else
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!File.Exists(Program.Localpath+"\\output.mp4"))
            {
                MessageBox.Show("File Not Available.");
                isclosedbycode = true;
                timerform timerform = new timerform();
                timerform.Show();
                this.Close();
                return;
            }
            if (CheckForInternetConnection())
            {
                btnUpload.Enabled = false;
                btnRecordAgain.Enabled = false;
                progressBar1.Show();
                lblProgress.Show();
                Thread thread = new Thread(UploadVideo);
                thread.Start();
            }
            else
            {
                MessageBox.Show("Internet connection not detected try again.");
                Environment.Exit(1);
            }
        }

        #region Upload Video
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        ReturnData uploadChunk(byte[] chunk)
        {
            UploadToAzure uploadToAzure = new UploadToAzure();
            var res = uploadToAzure.UploadChunk(CurrentChunk, chunk);
            return res;
        }
        int CurrentChunk = 1;
        const int chunkSize = (1024 * 1024) * 4;
        void UploadVideo()
        {

            UploadToAzure uploadToAzure = new UploadToAzure();
            using (var file = File.OpenRead(Program.Localpath + "//output.mp4"))
            {

                int bytesRead;
                var buffer = new byte[file.Length];
                while ((bytesRead = file.Read(buffer, 0, buffer.Length)) > 0)
                {

                }

                byte[][] vs = BufferSplit(buffer, chunkSize);
                long filesize = file.Length;
                long totalChunk = vs.Length;
                string FileName = Guid.NewGuid().ToString();
                uploadToAzure.SetMetadata((int)totalChunk, FileName, filesize, "");
                for (int i = 0; i < totalChunk; i++)
                {
                    CurrentChunk = i + 1;
                    var res = uploadChunk(vs[i]);
                    Thread.Sleep(10);
                    if (res.isLastBlock)
                    {
                        // timer1.Enabled = false;
                        DLOperation dLOperation = new DLOperation();
                        dLOperation.SaveEntry(ClsCommon.UserId, Program.cloudFile.BlockBlob.SnapshotQualifiedStorageUri.PrimaryUri.ToString(),2);

                    }
                    int percent = (CurrentChunk * 100 / (int)totalChunk) ;
                    progressBar1.Invoke(new MethodInvoker(() => { progressBar1.Value = percent; progressBar1.Update(); }));
                    lblProgress.Invoke(new MethodInvoker(() => { lblProgress.Text = percent.ToString() + " %"; lblProgress.Update(); }));
                }
            }
            this.Invoke(new MethodInvoker(() => { MessageBox.Show(this, "Content Captured. Press OK to continue submission."); }));
            Environment.Exit(1);
        }
        public static byte[][] BufferSplit(byte[] buffer, int blockSize)
        {
            byte[][] blocks = new byte[(buffer.Length + blockSize - 1) / blockSize][];

            for (int i = 0, j = 0; i < blocks.Length; i++, j += blockSize)
            {
                blocks[i] = new byte[Math.Min(blockSize, buffer.Length - i * blockSize)];
                Array.Copy(buffer, j, blocks[i], 0, blocks[i].Length);
            }
            return blocks;
        }
        public static string GetApplicationDirectory_New()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
        #endregion
        #region round Button
        private void roundButton_Paint(object sender,
       System.Windows.Forms.PaintEventArgs e)
        {

            //  base.OnPaint(e);
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, btnRecordAgain.Width, btnRecordAgain.Height);
            GraphicsPath GraphPath = GetRoundPath(Rect, 35);

            btnRecordAgain.Region = new Region(GraphPath);
            using (Pen pen = new Pen(Color.White, 1.75f))
            {
                pen.Alignment = PenAlignment.Inset;
                e.Graphics.DrawPath(pen, GraphPath);
            }

        }


        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();

            GraphPath.AddArc(Rect.X, Rect.Y, radius, radius, 180, 90);
            GraphPath.AddLine(Rect.X + r2, Rect.Y, Rect.Width - r2, Rect.Y);
            GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y, radius, radius, 270, 90);
            GraphPath.AddLine(Rect.Width, Rect.Y + r2, Rect.Width, Rect.Height - r2);
            GraphPath.AddArc(Rect.X + Rect.Width - radius,
                             Rect.Y + Rect.Height - radius, radius, radius, 0, 90);
            GraphPath.AddLine(Rect.Width - r2, Rect.Height, Rect.X + r2, Rect.Height);
            GraphPath.AddArc(Rect.X, Rect.Y + Rect.Height - radius, radius, radius, 90, 90);
            GraphPath.AddLine(Rect.X, Rect.Height - r2, Rect.X, Rect.Y + r2);

            GraphPath.CloseFigure();
            return GraphPath;
        }
        #endregion

        private void btnUpload_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, btnUpload.Width, btnUpload.Height);
            GraphicsPath GraphPath = GetRoundPath(Rect, 35);

            btnUpload.Region = new Region(GraphPath);
            using (Pen pen = new Pen(Color.White, 1.75f))
            {
                pen.Alignment = PenAlignment.Inset;
                e.Graphics.DrawPath(pen, GraphPath);
            }
        }
    }
}
