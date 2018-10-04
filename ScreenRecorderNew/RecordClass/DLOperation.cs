using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading;

namespace ScreenRecorderNew
{
    public class EXEConfig
    {
        public int RecordNo { get; set; }
        public int TimeInterval { get; set; }
        public int TimeToExit { get; set; }

    }
    public class LastActiveResult
    {
        public bool IsLogged { get; set; }
        public int Seconds { get; set; }
    }
    public class DLOperation
    {
        
        public DLOperation()
        {
            
        }
        public bool exeConfig()
        {
           
            try
            {
                HttpClient client = new HttpClient();
                string method = "GetEXEConfig";

                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(ClsCommon.APIBASEURL + method).Result;
                if (response.IsSuccessStatusCode)
                {
                    string responsestr = response.Content.ReadAsStringAsync().Result;
                    EXEConfig eXEConfig = JsonConvert.DeserializeObject<EXEConfig>(responsestr);
                    ClsCommon.TimeToexit = eXEConfig.TimeToExit;
                    ClsCommon.Interval = eXEConfig.TimeInterval;
                }else
                {
                    ClsCommon.WriteLog(response.Content.ReadAsStringAsync().Result + " Mthod :- getConfig");
                }
                return true;
            }
            catch(Exception ex)
            {
                ClsCommon.WriteLog(ex.Message + " Mthod :- getConfigexception");
                return false;
            }
        }
        public LastActiveResult checkLastActive()
        {
            LastActiveResult lastActiveResult = new LastActiveResult() { IsLogged = false,Seconds=0 };
            try
            {
                HttpClient client = new HttpClient();
                string method = "CheckLastActive?GUID="+ClsCommon.UserId;

                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(ClsCommon.APIBASEURL + method).Result;
                if (response.IsSuccessStatusCode)
                {
                    string responsestr = response.Content.ReadAsStringAsync().Result;
                    lastActiveResult = JsonConvert.DeserializeObject<LastActiveResult>(responsestr);
                  
                }else
                {
                    ClsCommon.WriteLog(response.Content.ReadAsStringAsync().Result);
                }
                return lastActiveResult;
            }
            catch(Exception ex)
            {
                ClsCommon.WriteLog(ex.Message+" Mthod :- CheckLastActive");
             //   ClsCommon.WriteLog(ex.Message+" Mthod :- CheckLastActive");
                return lastActiveResult;
            }
        }
        public bool SaveEntry(string UserId, string URL, int LogType)
        {
            try
            {
                HttpClient client = new HttpClient();
                string method = "";
                if (URL.Trim() == "")
                {
                    method = "/SaveLogEntry?UserId=" + UserId + "&LogType=" + LogType;
                }
                else
                {
                    method = "/SaveLogEntry?UserId=" + UserId + "&URL=" + URL + "&LogType=" + LogType;
                }
                // client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(ClsCommon.APIBASEURL + method).Result;
                //var dataObjects = response.Content.Result;
                return true;
            }catch(Exception ex)
            {
                ClsCommon.WriteLog(ex.Message + " Mthod=SaveEntry");
                return false;
            }
        }
       
    }
}
