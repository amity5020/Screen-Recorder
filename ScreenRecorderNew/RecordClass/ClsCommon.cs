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
         public static string SqlConn = "Server=115.124.113.64;Database=videopitch;user id=sa;password=occ@181016";
       // public static string SqlConn = "Server=tcp:videopitchdb1.database.windows.net,1433;Initial Catalog = testingvideopitch; Persist Security Info=False;User ID = videopitchdb1admin; Password=video@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout = 30;";
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
