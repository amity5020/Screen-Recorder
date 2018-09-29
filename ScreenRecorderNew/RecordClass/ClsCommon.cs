using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenRecorderNew
{
   public class ClsCommon
    {
        public static string APIBASEURL = "https://testing.trezlestaging.com/api/exeLogEntry/";
        
        public static string UserId="";
        public static bool WriteLog(string strMessage)
        {
            try
            {
                string strFileName = Program.Localpath+"\\log.txt";
                if (!File.Exists(strFileName))
                {
                    File.Create(Program.Localpath + "\\log.txt");
                }
                using (FileStream objFilestream = new FileStream(strFileName, FileMode.Append, FileAccess.Write))
                {
                    StreamWriter objStreamWriter = new StreamWriter((Stream)objFilestream);
                    objStreamWriter.WriteLine(DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt") + "    " + strMessage);
                    objStreamWriter.Close();
                    objFilestream.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

}
