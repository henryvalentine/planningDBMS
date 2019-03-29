using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPR_DataMigrationEngine.BulkUploadManagerManager;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;
using DPR_DataMigrationEngine.ViewModels;

namespace DPR_DataMigrationEngine.Controllers
{
    [CustomAuthorize]
    public class IncidentController : Controller
    {
        public IncidentController()
		{
			 ViewBag.LoadStatus = "0";
		}
        private const int ItemsPerPage = 40;
        private const int PageNumber = 1;

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        public ActionResult Incidents(int? page, int? pageSize)
        {
            var incidentHistoryTypeList = GetIncidentTypes();

            if (!incidentHistoryTypeList.Any())
            {
                ViewBag.Edit = 0;
                ViewBag.Title = "Incident Type SetUp";
                return View("~/Views/IncidentType/IncidentTypes.cshtml", new List<IncidentType>());
            }

            var companyList = GetCompanies();

            if (!companyList.Any())
            {
                ViewBag.Edit = 0;
                ViewBag.Title = "Company Set Up";
                return View("~/Views/Company/Companies.cshtml", new List<Company>());
            }
            int dataCount;
            var incidentHistoryList = GetIncidentHistories(pageSize ?? ItemsPerPage, page ?? PageNumber, out dataCount);
            if (!incidentHistoryList.Any())
            {
                ViewBag.Edit = 1;
                ViewBag.Title = "Set up Health & Safety Environment Incidents";
                return View(new IncidentViewModel{IncidentTypes = incidentHistoryTypeList, Companies = companyList, Incidents = new List<IncidentHistory>()});
            }
            ViewBag.Title = "Manage Health & Safety Environment Incidents";
            return View(new IncidentViewModel { IncidentTypes = incidentHistoryTypeList, Companies = companyList, Incidents = incidentHistoryList });
        }
        private List<IncidentHistory> GetIncidentHistories(int itemsPerPage, int pageNumber, out int dataCount)
        {
            try
            {
                var incidentHistoryList = new IncidentHistoryServices().GetAllOrderedIncidentHistories(itemsPerPage, pageNumber, out dataCount);

                ViewBag.PrDataCount = dataCount.ToString(CultureInfo.InvariantCulture);

                var totalPages = dataCount / itemsPerPage;

                // Counting the last page
                if (dataCount % itemsPerPage != 0)
                {
                    totalPages++;
                }

                ViewBag.PrTotalPages = totalPages;
                ViewBag.PrPage = pageNumber;
                if (!incidentHistoryList.Any())
                {
                    return new List<IncidentHistory>();  
                }
               
                return incidentHistoryList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                dataCount = 0;
                return new List<IncidentHistory>();
            }
        }
        private List<IncidentHistory> GetIncidentHistories()
        {
            try
            {
                var incidentHistoryList = new IncidentHistoryServices().GetAllOrderedIncidentHistories();
                if (!incidentHistoryList.Any())
                {
                    return new List<IncidentHistory>();
                }
               
                return incidentHistoryList;
            }
            catch (Exception ex)
            {
                return new List<IncidentHistory>();
            }
        }
        private List<IncidentType> GetIncidentTypes()
        {
            var incidentHistoryTypeList = new IncidentTypeServices().GetAllOrderedIncidentTypes() ?? new List<IncidentType>();

            if (!incidentHistoryTypeList.Any())
            {
                return new List<IncidentType>();
            }

            return incidentHistoryTypeList;
        }
        private List<Company> GetCompanies()
        {
            var companyList = new CompanyServices().GetAllOrderedCompanies() ?? new List<Company>();

            if (!companyList.Any())
            {
                return new List<Company>();
            }

            return companyList;
        }
        public ActionResult GetIncdentsByCompany(int companyId)
        {
            try
            {
                if (companyId < 1)
                {
                    return Json(new List<IncidentHistory>(), JsonRequestBehavior.AllowGet);
                }

                var incidentHistoryList = new IncidentHistoryServices().GetIncidentHistoriesByCompany(companyId);

                if (!incidentHistoryList.Any())
                {
                    return Json(new List<IncidentHistory>(), JsonRequestBehavior.AllowGet);
                }

                return Json(incidentHistoryList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<IncidentHistory>(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetIncdentsByType(int incidentTypeId)
        {
            try
            {
                if (incidentTypeId < 1)
                {
                    return Json(new List<IncidentHistory>(), JsonRequestBehavior.AllowGet);
                }

                var incidentHistoryList = new IncidentHistoryServices().GetIncidentHistoriesByType(incidentTypeId);

                if (!incidentHistoryList.Any())
                {
                    return Json(new List<IncidentHistory>(), JsonRequestBehavior.AllowGet);
                }

                return Json(incidentHistoryList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<IncidentHistory>(), JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        [HttpPost]
        public ActionResult AddIncidentHistory(IncidentHistory incidentHistory)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    incidentHistory.Error = "Please supply all required fields and try again";
                    incidentHistory.ErrorCode = -1;
                    return Json(incidentHistory, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(incidentHistory);

                if (wx.Code < 1)
                {
                    incidentHistory.Error = wx.Error;
                    incidentHistory.ErrorCode = -1;
                    return Json(incidentHistory, JsonRequestBehavior.AllowGet);
                }

                var k = new IncidentHistoryServices().AddIncidentHistory(incidentHistory);
                if (k < 1)
                {
                    incidentHistory.Error = "Process Failed! Please contact the Admin or try again later";
                    incidentHistory.ErrorCode = 0;
                    return Json(incidentHistory, JsonRequestBehavior.AllowGet);
                }
                
                incidentHistory.Error = "Record was added successfully";
                incidentHistory.ErrorCode = 1;
                incidentHistory.IncidentHistoryId = k;
                return Json(incidentHistory, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                incidentHistory.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                incidentHistory.ErrorCode = 0;
                return Json(incidentHistory, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        [HttpPost]
        public ActionResult EditIncidentHistory(IncidentHistory incidentHistory)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_incidentHistory"] == null)
                {
                    incidentHistory.Error = "Session has expired";
                    incidentHistory.ErrorCode = 0;
                    return Json(incidentHistory, JsonRequestBehavior.AllowGet);
                }

                var oldIncidentHistory = Session["_incidentHistory"] as IncidentHistory;

                if (oldIncidentHistory == null || oldIncidentHistory.IncidentHistoryId < 1)
                {
                    incidentHistory.Error = "Session has expired";
                    incidentHistory.ErrorCode = 0;
                    return Json(incidentHistory, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    incidentHistory.Error = "Please supply all required fields and try again";
                    incidentHistory.ErrorCode = -1;
                    return Json(incidentHistory, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(incidentHistory);

                if (wx.Code < 1)
                {
                    incidentHistory.Error = wx.Error;
                    incidentHistory.ErrorCode = -1;
                    return Json(incidentHistory, JsonRequestBehavior.AllowGet);
                }

                incidentHistory.Date = incidentHistory.IncidentDate.ToString("dd/MM/yyyy");
                oldIncidentHistory.IncidentTypeId = incidentHistory.IncidentTypeId;
                oldIncidentHistory.Title = incidentHistory.Title;
                oldIncidentHistory.CompanyId = incidentHistory.CompanyId;
                oldIncidentHistory.Location = incidentHistory.Location;
                incidentHistory.Description = incidentHistory.Description;
                oldIncidentHistory.IncidentDate = incidentHistory.IncidentDate;
                oldIncidentHistory.ReportedBy = incidentHistory.ReportedBy;

                var k = new IncidentHistoryServices().UpdateIncidentHistory(oldIncidentHistory);
                if (k < 1)
                {
                    incidentHistory.Error = "Process Failed! Please contact the Admin or try again later";
                    incidentHistory.ErrorCode = 0;
                    return Json(incidentHistory, JsonRequestBehavior.AllowGet);
                }
                
                    incidentHistory.Error = "Incident  Information was successfully updated";
                    incidentHistory.ErrorCode = 1;
                    return Json(incidentHistory, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                incidentHistory.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                incidentHistory.ErrorCode = 0;
                return Json(incidentHistory, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        [HttpPost]
        [ActionName("DeleteWell")]
        public ActionResult DeleteIncident(int id)
        {
            var incidentHistory = new IncidentHistory();

            try
            {
                if (id < 1)
                {
                    incidentHistory.Error = "Invalid Selection";
                    incidentHistory.ErrorCode = 0;
                    return Json(incidentHistory, JsonRequestBehavior.AllowGet);
                }
                if (new IncidentHistoryServices().DeleteIncidentHistoryCheckReferences(id))
                {
                    incidentHistory.Error = "Incident  Information was successfully deleted.";
                    incidentHistory.ErrorCode = 1;
                    return Json(incidentHistory, JsonRequestBehavior.AllowGet);
                }

                incidentHistory.Error = "Process Failed! Please try again later";
                incidentHistory.ErrorCode = 0;
                return Json(incidentHistory, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                incidentHistory.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                incidentHistory.ErrorCode = 0;
                return Json(incidentHistory, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        public ActionResult EditIncidentHistory(int id)
        {
            var incidentHistory = new IncidentHistory();
            try
            {
                if (id < 1)
                {
                    incidentHistory.Error = "Invalid Selection!";
                    incidentHistory.ErrorCode = -1;
                    return Json(incidentHistory, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new IncidentHistoryServices().GetIncidentHistory(id);

                if (myViewObj == null || myViewObj.IncidentHistoryId < 1)
                {
                    incidentHistory.Error = "Incident  Information could not be retrieved.";
                    incidentHistory.ErrorCode = -1;
                    return Json(incidentHistory, JsonRequestBehavior.AllowGet);
                }
                Session["_incidentHistory"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.IncidentHistoryId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                incidentHistory.Error = "An unknown error was encountered. Incident  Information could not be retrieved.";
                incidentHistory.ErrorCode = -1;
                return Json(incidentHistory, JsonRequestBehavior.AllowGet);
            }
        }
        private static GenericValidator ValidateControl(IncidentHistory model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.Title.Trim()))
                {
                    gVal.Error = "Please provide Incident Title.";
                    gVal.Code = 0;
                    return gVal;
                }

                if (string.IsNullOrEmpty(model.Location))
                {
                    gVal.Error = "Please provide Incident Location";
                    gVal.Code = 0;
                    return gVal;
                }

                if (string.IsNullOrEmpty(model.Description))
                {
                    gVal.Error = "Please provide Incident Description";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.IncidentDate.Year == 0001)
                {
                    gVal.Error = "Please provide a valid Incident Date";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.CompanyId < 1)
                {
                    gVal.Error = "Please select a valid Company";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.IncidentTypeId < 1)
                {
                    gVal.Error = "Please select a valid Incident Type";
                    gVal.Code = 0;
                    return gVal;
                }
                gVal.Code = 1;
                return gVal;
            }
            catch (Exception ex)
            {
                gVal.Error = "Process validation failed. Please supply all required entries and try again.";
                gVal.Code = 0;
                return gVal;
            }
        }
        private static GenericValidator ValidateControl2(IncidentHistory model)
        {
            var gVal = new GenericValidator();

            try
            {

                if (model.CompanyId < 1)
                {
                    gVal.Error = "Please select a valid company";
                    gVal.Code = 0;
                    return gVal;
                }
                
                if (model.IncidentTypeId < 1)
                {
                    gVal.Error = "Please select a valid Incident Type";
                    gVal.Code = 0;
                    return gVal;
                }
                gVal.Code = 1;
                return gVal;
            }
            catch (Exception ex)
            {
                gVal.Error = "Process validation failed. Please supply all required entries and try again.";
                gVal.Code = 0;
                return gVal;
            }
        }

        public ActionResult IncidentUpload()
        {
            return View();
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        [HttpPost]
        public ActionResult IncidentUpload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    
                    const string folderPath = "~/BulkUploadTemplate";

                    var fileName = file.FileName;
                    var path = folderPath + "/" + fileName;
                    if (System.IO.File.Exists(Server.MapPath(path)))
                    {
                        System.IO.File.Delete(Server.MapPath(path));
                    }
                    file.SaveAs(Server.MapPath(folderPath + "/" + fileName));

                    var mList = new List<IncidentHistory>();
                    var msg = string.Empty;
                    if (!new IncidentUploadManager().Import(Server.MapPath(path), "incidents", ref mList, ref msg))
                    {
                        return View();
                    }

                    if (!mList.Any())
                    {
                        ViewBag.ErrorMessage = msg;
                        ViewBag.ErrorCode = -1;
                        //call Index Action here
                        return View();
                    }

                    var errorList = new List<IncidentHistory>();
                    var successList = new List<IncidentHistory>();
                    foreach (var incidentHistoryInfo in mList)
                    {
                        var processedItem = new IncidentHistoryServices().AddIncidentHistory(incidentHistoryInfo);

                        if (processedItem < 1)
                        {
                            errorList.Add(incidentHistoryInfo);
                        }
                        else
                        {
                            successList.Add(incidentHistoryInfo);
                        }
                    }

                    if (!successList.Any())
                    {
                        var error = msg.Length > 0 ? msg : "Bulk upload Failed! unknown error occurred";
                        ViewBag.ErrorCode = -2;
                        ViewBag.ErrorMessage = error;
                        return View();
                    }
                    
                    if (errorList.Any() && successList.Any())
                    {
                        var ts = successList.Count + " records were successfully uploaded." +
                            "<br/>" + errorList.Count + " records could not be uploaded due to duplicates/unknown errors encountered.";
                        ViewBag.ErrorCode = -1;

                        if (msg.Length > 0)
                        {
                            ts += "<br/>" + msg;
                        }
                        ViewBag.ErrorMessage = ts;
                        return View();
                    }

                    if (errorList.Any() && !successList.Any())
                    {
                        var ts = errorList.Count + " records could not be uploaded due to duplicates/unknown errors encountered.";
                        ViewBag.ErrorCode = -1;

                        if (msg.Length > 0)
                        {
                            ts += "<br/>" + msg;
                        }
                        ViewBag.ErrorMessage = ts;
                        return View();
                    }

                    if (!errorList.Any() && successList.Any() && msg.Length > 0)
                    {
                        ViewBag.ErrorCode = -1;
                        //ViewBag.ErrorMessage = mList.Count + " records were successfully uploaded.";

                        var tsx = successList.Count + " records were successfully uploaded.";
                        tsx += "<br/>" + msg;
                        ViewBag.ErrorMessage = tsx;
                        return View();
                    }

                    if (!errorList.Any() && successList.Any() && (string.IsNullOrEmpty(msg) || msg.Length < 1))
                    {
                        ViewBag.ErrorCode = 5;
                        var tsx = successList.Count + " records were successfully uploaded.";
                        ViewBag.ErrorMessage = tsx;
                        return View();
                    }
                }
                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = "The selected file is invalid";
                return View();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source,ex.Message);
                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = "An unknown error was encountered. Data upload faild not be completed.";
                return View();
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
                Response.WriteFile(Server.MapPath(path));
                Response.End();
                return true;
            }
            catch (Exception ex)
            {
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

        public ActionResult Report()
        {
            try
            {
                return View(GetIncidentHistories());
            }
            catch (Exception ex)
            {
               return View(new List<IncidentHistory>());
            }
        }
    }
}