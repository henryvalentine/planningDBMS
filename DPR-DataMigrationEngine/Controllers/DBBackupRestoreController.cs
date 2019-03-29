using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DPR_DataMigrationEngine.Controllers
{
    public class DBBackupRestoreController : Controller
    {
       [CustomAuthorize(Roles = "Admin,Super_Admin")]
        public ViewResult DBBackup()
        {
            ViewBag.Code = 0;
            var param = new Sys_ParamsServices().GetSysParams();
            if (param != null && param.Sys_ParamId > 0)
            {
                ViewBag.LoadStatus = 3;
                Session["_param"] = param;
            }
            else
            {
                ViewBag.LoadStatus = -2; 
            }

            return View(param);
        }

       
         public bool Backup(string sdc)
          {
              //var sb = new StringBuilder();
            try
            {
                var param = new Sys_ParamsServices().GetSysParams();
                if (param == null || param.Sys_ParamId < 1)
                {
                    ViewBag.StatusMessage = "An unknown error was encountered. Please provide the server access fields again.";
                    ViewBag.Code = -1;
                    return false;
                }

                var fileName = "Backup-" + DateTime.Now.ToString("yyyy-MM-dd") + ".sql";
              
                const string subPath = @"c:\App_Data\sys_params";

                if (Directory.Exists(subPath))
                {
                   var tdfv= Directory.GetFiles(subPath);
                    if (tdfv.Any())
                    {
                        foreach (var s in tdfv)
                        {
                            System.IO.File.Delete(s);
                        }
                    }
                   
                }
                else
                {
                    Directory.CreateDirectory(subPath);
                    var dInfo = new DirectoryInfo(subPath);
                    var dSecurity = dInfo.GetAccessControl();
                    dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    dInfo.SetAccessControl(dSecurity);
                }

                var backupPath = Path.Combine(subPath, fileName);

                var srv = new Server(new ServerConnection(param.ServerName, param.ServerLoginName, param.ServerLoginPassword));

                var dbs = srv.Databases[param.DBName];

                var options = new ScriptingOptions
                {
                    ScriptData = true,
                    ScriptDrops = false,
                    FileName = backupPath,
                    EnforceScriptingOptions = true,
                    ScriptSchema = true,
                    IncludeHeaders = true,
                    AppendToFile = true,
                    Indexes = true,
                    WithDependencies = true
                };

                for (var i = 0; i < dbs.Tables.Count; i++)
                {
                    if (!dbs.Tables[i].IsSystemObject)
                    {
                        dbs.Tables[i].EnumScript(options);
                    }

                }

                ViewBag.Code = 5;
                ViewBag.StatusMessage = "Action completed successfully";
                DownloadContentFromFolder(backupPath);
                return true;
            }
            catch (Exception ex)
            {
                
               ErrorLogger.LogEror(ex.StackTrace,ex.Source,ex.Message);
               ViewBag.StatusMessage = ex.Message;
               ViewBag.Code = -1;
               return false;
            }

           }

        //public List<string> GetDatabaseList()
        //{
        //    var docObjects = new List<DocObject>();

        //    // Open connection to the database
        //    string conString = "server=xeon;uid=sa;pwd=manager; database=northwind";

        //    using (SqlConnection con = new SqlConnection(conString))
        //    {
        //        con.Open();

        //        using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases", con))
        //        {
        //            using (IDataReader dr = cmd.ExecuteReader())
        //            {
        //                while (dr.Read())
        //                {
        //                    docObjects.Add(dr[0].ToString());
        //                }
        //            }
        //        }
        //    }
        //    return docObjects;

        //}
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RestoreDB(DBBackupObject dbObject, HttpPostedFileBase backupFile)
        {
            if (dbObject == null)
            {
                ViewBag.StatusMessage = "An unknown error was encountered. Please try again.";
                return View("DBBackup");
            }
            try
            {
                var srv = new Server(new ServerConnection(dbObject.ServerName, dbObject.UserName, dbObject.Password));


                Database database = srv.Databases["AdventureWorks"];
               
                var restore = new Restore
                {
                    Action = RestoreActionType.Database,
                    Database = database.Name,
                    PercentCompleteNotification = 10,
                };

                restore.Devices.AddDevice(@"E:\Data\Backup\AW.bak", DeviceType.File);
                //restore.PercentComplete += ProgressEventHandler;
                restore.SqlRestore(srv);

                ViewBag.StatusMessage = "Action completed successfully";
                return View("DBBackup");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                ViewBag.StatusMessage = ex.Message;
                return View("DBBackup");
            }
          
        }

        private static GenericValidator ValidateControl(Sys_Param model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.ServerLoginName.Trim()))
                {
                    gVal.Error = "Please Provide Server Login  Name.";
                    gVal.Code = 0;
                    return gVal;
                }

                if (string.IsNullOrEmpty(model.ServerLoginPassword.Trim()))
                {
                    gVal.Error = "Please Provide Server Login Password.";
                    gVal.Code = 0;
                    return gVal;
                }

                if (string.IsNullOrEmpty(model.DBName.Trim()))
                {
                    gVal.Error = "Please Provide Database Name.";
                    gVal.Code = 0;
                    return gVal;
                }

                if (string.IsNullOrEmpty(model.ServerName.Trim()))
                {
                    gVal.Error = "Please Provide Server Name.";
                    gVal.Code = 0;
                    return gVal;
                }

                gVal.Code = 1;
                return gVal;
            }
            catch (Exception)
            {
                gVal.Error = "Process validation failed. Please supply all required fields and try again.";
                gVal.Code = 0;
                return gVal;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddParam(Sys_Param plParam)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.StatusMessage = "Please supply all the required input and try again";
                    ViewBag.Code = -1;
                    return View("DBBackup", plParam);
                }

                var wx = ValidateControl(plParam);

                if (wx.Code < 1)
                {
                    ViewBag.StatusMessage = "Validation failed. Please supply all the required input and try again";
                    ViewBag.Code = -1;
                    return View("DBBackup", plParam);
                }

                var k = new Sys_ParamsServices().AddSys_ParamsCheckPrevious(plParam);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        ViewBag.StatusMessage = "An unknown error was encountered. Please try again.";
                        ViewBag.Code = -1;
                        return View("DBBackup", plParam);
                    }

                    ViewBag.StatusMessage = "Process failed. Please try again.";
                    ViewBag.Code = -1;
                    return View("DBBackup", plParam);
                }

                ViewBag.StatusMessage = "Database configuration was successfully added";
                ViewBag.Code = 5;
                return View("DBBackup", plParam);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                ViewBag.StatusMessage = "An unknown error was encountered. Please try again.";
                ViewBag.Code = -1;
                return View("DBBackup", plParam);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditParam(Sys_Param plParam)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_param"] == null)
                {
                    ViewBag.StatusMessage = "Session has expired";
                    ViewBag.Code = -1;
                    return View("DBBackup", plParam);
                }

                var oldParam = Session["_param"] as Sys_Param;

                if (oldParam == null || oldParam.Sys_ParamId < 1)
                {
                     ViewBag.StatusMessage =  "Session has expired";
                     ViewBag.Code = -1;
                    return View("DBBackup", plParam);
                }

                if (!ModelState.IsValid)
                {
                     ViewBag.StatusMessage = "Please supply all required fields and try again";
                     ViewBag.Code = -1;
                    return View("DBBackup", plParam);
                }

                var wx = ValidateControl(plParam);

                if (wx.Code < 1)
                {
                     ViewBag.StatusMessage  = wx.Error;
                     ViewBag.Code = -1;
                    return View("DBBackup", plParam);
                }

                oldParam.ServerName = plParam.ServerName;
                oldParam.ServerLoginPassword = plParam.ServerLoginPassword;
                oldParam.ServerLoginName = plParam.ServerLoginName;
                oldParam.DBName = plParam.DBName;

                var k = new Sys_ParamsServices().UpdateSys_Params(oldParam);
                if (k < 1)
                {
                    ViewBag.StatusMessage = "Process Failed! Please contact the Admin or try again later";
                     ViewBag.Code = -1;
                    return View("DBBackup", plParam);
                }

                 ViewBag.StatusMessage = "Database configuration was successfully updated";
                 ViewBag.Code = 5;
                 return View("DBBackup", plParam);
            }
            catch (Exception)
            {
                //ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                ViewBag.StatusMessage = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                ViewBag.Code = -1;
                return View("DBBackup", plParam);
            }
        }

        public bool DownloadContentFromFolder(string path)
        {
            try
            {
                Response.Clear();
                var filename = Path.GetFileName(path);
                HttpContext.Response.Buffer = true;
                HttpContext.Response.Charset = "";
                HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = GetMimeType(filename);
                HttpContext.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + filename + "\"");
                Response.WriteFile(path);
                Response.End();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }
        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            var extension = Path.GetExtension(fileName);
            if (extension != null)
            {
                var ext = extension.ToLower();
                var regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
                if (regKey != null && regKey.GetValue("Content Type") != null)
                    mimeType = regKey.GetValue("Content Type").ToString();
            }
            return mimeType;
        }
	}
}