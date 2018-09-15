using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
namespace ScreenRecorderNew
{
   public class DLOperation
    {
        SqlConnection con;
        SqlCommand cmd;
        public DLOperation()
        {
            
        }
        public bool SaveEntry(string UserId,string URL)
        {
            try
            {
                con = new SqlConnection(ClsCommon.SqlConn);
                cmd = new SqlCommand("Insert into RecordEntry(UserId,URL,EntryDate)values(@UserId,@URL,getdate())", con);
                cmd.Parameters.AddWithValue("@UserId",UserId);
                cmd.Parameters.AddWithValue("@URL", URL);
                con.Open();
                int i=cmd.ExecuteNonQuery();
                if(i==1)
                {
                    return true;
                }else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }
    }
}
