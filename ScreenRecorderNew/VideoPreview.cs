using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            axWindowsMediaPlayer1.URL = Program.Localpath + "\\output.mp4";
            progressBar1.Hide();
            lblProgress.Hide();
        }
        bool isclosedbycode = false;
        private void btnRecordAgain_Click(object sender, EventArgs e)
        {
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
                DialogResult dialog = MessageBox.Show("Are you sure close application and discard recording?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialog == DialogResult.Yes)
                {
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
                        dLOperation.SaveEntry(ClsCommon.UserId, Program.cloudFile.BlockBlob.SnapshotQualifiedStorageUri.PrimaryUri.ToString());

                    }
                    int percent = (CurrentChunk * 100 / (int)totalChunk) ;
                    progressBar1.Invoke(new MethodInvoker(() => { progressBar1.Value = percent; progressBar1.Update(); }));
                    lblProgress.Invoke(new MethodInvoker(() => { lblProgress.Text = percent.ToString() + " %"; lblProgress.Update(); }));
                }
            }
            this.Invoke(new MethodInvoker(() => { MessageBox.Show(this, "Uploaded Successfully."); }));
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
    }
}
