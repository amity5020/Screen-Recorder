using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ScreenRecorderNew
{
   public class DLOperation
    {
        
        public DLOperation()
        {
            
        }
        public bool SaveEntry(string UserId, string URL, int LogType)
        {
            HttpClient client = new HttpClient();
            string method = "";
            if(URL.Trim()=="")
            {
                method= "/SaveLogEntry?UserId=" + UserId +"&LogType=" + LogType;
            }else
            {
                method = "/SaveLogEntry?UserId=" + UserId + "&URL=" + URL + "&LogType=" + LogType;
            }
           // client.BaseAddress = new Uri(URL);
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(ClsCommon.APIBASEURL+method ).Result;
            //var dataObjects = response.Content.Result;
            return true;
        }
        //    public bool SaveEntry(string UserId,string URL,int LogType)
        //{
        //    try
        //    {
        //        con = new SqlConnection(ClsCommon.SqlConn);
        //        cmd = new SqlCommand("Insert into RecordEntry(UserId,URL,EntryDate,LogType)values(@UserId,@URL,getdate(),@LogType)", con);
        //        cmd.Parameters.AddWithValue("@UserId",UserId);
        //        cmd.Parameters.AddWithValue("@URL", URL);
        //        cmd.Parameters.AddWithValue("@LogType", LogType);
        //        con.Open();
        //        int i=cmd.ExecuteNonQuery();
        //        if(i==1)
        //        {
        //            return true;
        //        }else
        //        {
        //            return false;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        return false;
        //    }
        //    finally
        //    {
        //        if (con.State == ConnectionState.Open)
        //        {
        //            con.Close();
        //            con.Dispose();
        //        }
        //    }
        //}

        //public bool SaveExeString(string GUID)
        //{
        //    try
        //    {
        //        con = new SqlConnection(ClsCommon.SqlConn);
        //        cmd = new SqlCommand("Insert into ExeRecords(EXEGuid,EntryDateTime)values(@GUID,getdate())", con);
        //        cmd.Parameters.AddWithValue("@GUID", GUID);
        //        con.Open();
        //        int i = cmd.ExecuteNonQuery();
        //        if (i == 1)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //    finally
        //    {
        //        if (con.State == ConnectionState.Open)
        //        {
        //            con.Close();
        //            con.Dispose();
        //        }
        //    }
        //}
    }
}
