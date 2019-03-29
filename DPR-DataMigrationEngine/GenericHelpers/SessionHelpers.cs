using System;
using System.Web;

namespace DPR_DataMigrationEngine.GenericHelpers
{
    public class GenericHelpers
    {
        public void SetCurrentSession(string sName, dynamic obj)
        {
            HttpContext.Current.Session["_" + sName] = obj;
        }

        public static bool GetCurrentSession(string sName)
        {
            return HttpContext.Current.Session["_" + sName] == null;
        }

        public static T GetDataFromCurrentSession<T>(string sName)
        {
            return (T)HttpContext.Current.Session["_" + sName];
        }
        
        public static void ResetCurrentSession(string sName)
        {
            HttpContext.Current.Session["_" + sName] = null;
        }
        
    }
}