using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
    public partial class StartForm : Form
    {
        public StartForm()
        {
            InitializeComponent();
        }
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
        private void StartForm_Load(object sender, EventArgs e)
        {
            if (CheckForInternetConnection())
            {
                Thread thread = new Thread(UploadVideo);
                thread.Start();
            }else
            {
                MessageBox.Show("Internet connection not detected try again.");
                Environment.Exit(1);
            }
        }
       
        ReturnData uploadChunk(byte[] chunk)
        {
            UploadToAzure uploadToAzure = new UploadToAzure();
            var res = uploadToAzure.UploadChunk(CurrentChunk, chunk);
            return res;
        }
        int CurrentChunk = 1;
        const int chunkSize = (1024 * 1024)*4;
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
                        timer1.Enabled = false;
                        DLOperation dLOperation = new DLOperation();
                        dLOperation.SaveEntry(ClsCommon.UserId, Program.cloudFile.BlockBlob.SnapshotQualifiedStorageUri.PrimaryUri.ToString());

                    }
                }
            }
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

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            
            //RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            //rk.SetValue("ScreenRecorder", GetApplicationDirectory_New()+ "\\ScreenRecordSharpApi.exe");
          
            this.Hide();
            timer1.Stop();
            timer1.Enabled = false;
        }
    }
   

}
