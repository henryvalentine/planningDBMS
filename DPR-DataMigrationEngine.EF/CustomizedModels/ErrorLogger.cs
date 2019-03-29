using System;
using System.IO;
using System.Text;
using System.Web;

namespace DPR_DataMigrationEngine.EF.CustomizedModels
{
    public static class ErrorLogger
    {
        public static string GetLogFilePath()
        {
            try
            {
                return HttpContext.Current.Server.MapPath("~/App_Data/" + "ErrorLog2.txt");
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static void LogEror(string stackTrace, string errorSource, string errorMessage)
        {
            var logPath =  GetLogFilePath();
            if (!string.IsNullOrEmpty(logPath))
            {
                var sw = File.AppendText(logPath);
                try
                {
                    var messageBuilder = new StringBuilder();
                    messageBuilder.Append("Exception Date :: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt") + "\n");
                    messageBuilder.Append("Exception Stack Trace :: " + stackTrace + "\n");
                    messageBuilder.Append("Exception Source :: " + errorSource + "\n");
                    messageBuilder.Append("Exception Message :: " + errorMessage + "\n\n");
                    sw.WriteLine(messageBuilder);
                    sw.Close();
                }
                catch (Exception ex)
                {
                    sw.WriteLine(ex.Message);
                }
            }
           
        }

    }
}