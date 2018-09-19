using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenRecorderNew
{
    static class Program
    {
        public static RequestFor eRequestFor = RequestFor.VideoRecording;
        public static string Localpath = "";
        public static CloudFile cloudFile;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ScreenRecorder";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!CheckForProtocolMessage())
           //  if (false)
            {
                MessageBox.Show("Please launch the Screen Recorder only from the Trezle coaching application.");
                Environment.Exit(1);
            }
            else
            {
                Localpath = path;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                if (eRequestFor == RequestFor.ScreenRecording)
                {
                    Application.Run(new timerform());
                }
                else
                {
                    RecordVideo.IsRecordLoad = true;
                    Application.Run(new timerform());
                }
            }
        }
        private static bool CheckForProtocolMessage()
        {
            string[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length > 1)
            {
                // Format = "Owf:OpenForm?id=111"
                string[] args = arguments[1].Split(':');
                if (args[0].Trim().ToUpper() == "TREZLERECORDER" && args.Length > 1)
                { // Means this is a URL protocol
                    string[] actionDetail = args[1].Split('?');
                    if (actionDetail.Length > 1)
                    {
                        switch (actionDetail[0].Trim().ToUpper())
                        {
                            case "OPENFORM":
                                string[] details = actionDetail[1].Split('=');
                                if (details.Length > 1)
                                {
                                    if (details[0] == "IdRecord")
                                    {
                                        eRequestFor = RequestFor.VideoRecording;
                                    }
                                    else
                                    {
                                        eRequestFor = RequestFor.ScreenRecording;
                                    }
                                    string id = details[1].Trim();
                                    ClsCommon.UserId = id;
                                    return true;
                                }
                                break;
                        }
                    }
                }
            }
            return false;
        }
      
    }
    public enum RequestFor
    {
        none=0,
        ScreenRecording = 1,
        VideoRecording = 2
    }
}
