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

namespace DPR_DataMigrationEngine.Controllers.WellManagement
{
     [CustomAuthorize]
    public class WellController : Controller     
    {
        public WellController()
		{
			 ViewBag.LoadStatus = "0";
		}
        private const int ItemsPerPage = 50;
        private const int PageNumber = 1;

         [CustomAuthorize(Roles = "Admin")]
        public ActionResult Wells()
        {
             try
            {
               var wellTypeList = GetWellTypes();

                if (!wellTypeList.Any())
                {
                    return RedirectToAction("WellTypes", "WellType");
                }

                var fieldList = GetFields();

                if (!fieldList.Any())
                {
                    return RedirectToAction("Fields", "Field");
                }

                var wellClassList = GetWellClasses();
                if (!wellClassList.Any())
                {
                    return RedirectToAction("WellClasses", "WellClass");
                }

                int dataCount;
                var wellList = GetWells(ItemsPerPage, PageNumber, out dataCount);

                if (!wellList.Any())
                {
                    ViewBag.ErrorCode = 0;
                    ViewBag.Title = "Well SetUp";
                    return View(new WellViewModel
                    {
                        WellTypes = wellTypeList,
                        Fields = fieldList,
                        Blocks = GetBlocks(),
                        Wells = wellList,
                        WellClasses = wellClassList
                    });

                }

                ViewBag.Title = "Manage Wells";
                Session["_wellPage"] = 1;
                return View(new WellViewModel { WellTypes = wellTypeList, Fields = fieldList, Blocks = GetBlocks(), Wells = wellList, WellClasses = wellClassList });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = "Well list could not be retrieved. Please try again or contact the Admin.";
                ViewBag.Title = "Well SetUp";
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return View(new WellViewModel { WellTypes = GetWellTypes(), Fields = GetFields(), Blocks = GetBlocks(), Wells = new List<Well>(), WellClasses = GetWellClasses() });
            }
        }

         public ActionResult GetMoreWells()  
         {
             int currentPage;
             
             var o = Session["_wellPage"];
             if (o != null)
             {
                 var page = (int)o;
                 if (page < 1)
                 {
                     currentPage = 1;
                 }

                 else
                 {
                     currentPage = page + 1;
                 }
             }
             else
             {
                 currentPage = 1;
             }

             var wellList = GetMoreWellObjects(50, currentPage);

             if (!wellList.Any())
             {
                 return Json(new List<Company>(), JsonRequestBehavior.AllowGet);
             }
             Session["_wellPage"] = currentPage;
             return Json(wellList, JsonRequestBehavior.AllowGet);
         }

         private List<WellReportObject> GetMoreWellObjects(int itemsPerPage, int pageNumber)
         {
             try
             {
                 var wellList = new WellServices().GetWellObjects(itemsPerPage, pageNumber) ?? new List<WellReportObject>();

                 if (!wellList.Any())
                 {
                     return new List<WellReportObject>();
                 }

                 return wellList;
             }
             catch (Exception ex)
             {
                 ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                 return new List<WellReportObject>();
             }
         }

         private List<WellReportObject> GetWellObjects(int itemsPerPage, int pageNumber, long companyId)
         {
             try
             {
                 var wellList = new WellServices().GetWellObjects(itemsPerPage, pageNumber, companyId) ?? new List<WellReportObject>();

                 if (!wellList.Any())
                 {
                     return new List<WellReportObject>();
                 }
                 
                 return wellList;
             }
             catch (Exception ex)
             {
                 ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                 return new List<WellReportObject>();
             }
         }

         private List<WellType> GetWellTypes()
        {
            var wellTypeList = new WellTypeServices().GetAllOrderedWellTypes() ?? new List<WellType>();

            if (!wellTypeList.Any())
            {
                return new List<WellType>();
            }

            return wellTypeList;
        }

        private ActionResult GetSelect2Wells()
        {
            try
            {
                var fxs = new WellServices().GetWells() ?? new List<Well>();
                if (!fxs.Any())
                {
                    return Json(new List<Well>(), JsonRequestBehavior.AllowGet);
                }

                return Json(fxs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
               
                ErrorLogger.LogEror(ex.StackTrace,ex.Source,ex.Message);
                return Json(new List<Well>(), JsonRequestBehavior.AllowGet);
            }
        }

        private List<Well> GetWells(int itemsPerPage, int pageNumber, out int dataCount)
        {
            try
            {
                var fxs = new WellServices().GetAllOrderedWells(itemsPerPage, pageNumber, out dataCount) ?? new List<Well>();
                if (!fxs.Any())
                {
                    return new List<Well>();
                }
                dataCount = dataCount - 1;
                ViewBag.DataCount = dataCount.ToString(CultureInfo.InvariantCulture);

                var totalPages = dataCount / itemsPerPage;

                // Counting the last page
                if (dataCount % itemsPerPage != 0)
                {
                    totalPages++;
                }
                fxs.Remove(fxs.Find(m => m.WellId == (int)OtherNotAvailable.Not_Available));
                //ViewBag.ItemCount = (pageNumber * itemsPerPage) - 150;

                ViewBag.TotalPages = totalPages;
                ViewBag.Page = pageNumber;
                return fxs;
            }
            catch (Exception ex)
            {
                dataCount = 0;
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Well>();
            }
        }
        private List<WellClass> GetWellClasses()
        {
            var wellTypeList = new WellClasServices().GetAllOrderedWellClasses2() ?? new List<WellClass>();

            if (!wellTypeList.Any())
            {
                return new List<WellClass>();
            }

            return wellTypeList;
        }

        private List<Company> GetCompanies()
        {
            var companyList = new CompanyServices().GetCompaniesWithFields() ?? new List<Company>();

            if (!companyList.Any())
            {
                return new List<Company>();
            }

            return companyList;
        }
        private List<Field> GetFields()
        {
            var fielList = new FieldServices().GetFields() ?? new List<Field>();

            if (!fielList.Any())
            {
                return new List<Field>();
            }

            return fielList;
        }

        public ActionResult GetFieldsByBlock(int blockId) 
        {  
            try
            {
                if (blockId < 1)
                {
                    return Json(new List<FieldReportObject>(), JsonRequestBehavior.AllowGet);
                }

                var blocklist = new FieldServices().GetFieldsByBlockId(blockId);

                if (!blocklist.Any())
                {
                    return Json(new List<FieldReportObject>(), JsonRequestBehavior.AllowGet);
                }

                return Json(blocklist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<FieldReportObject>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetBlocksByCompany(long companyId) 
        {
            try
            {
                if (companyId < 1)
                {
                    return Json(new List<BlockObject>(), JsonRequestBehavior.AllowGet);
                }

                var blocklist = new BlockServices().GetCompanyBlocksWithFields(companyId);

                if (!blocklist.Any())
                {
                    return Json(new List<BlockObject>(), JsonRequestBehavior.AllowGet);
                }

                return Json(blocklist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<BlockObject>(), JsonRequestBehavior.AllowGet);
            }
        }

         [CustomAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddWell(Well well)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    well.Error = "Please supply all required fields and try again";
                    well.ErrorCode = -1;
                    return Json(well, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(well);

                if (wx.Code < 1)
                {
                    well.Error = wx.Error;
                    well.ErrorCode = -1;
                    return Json(well, JsonRequestBehavior.AllowGet);
                }
                
                var k = new WellServices().AddWellCheckDuplicate(well);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        well.Error = "Well  already exists";
                        well.ErrorCode = -3;
                        return Json(well, JsonRequestBehavior.AllowGet);
                    }

                    if (k == -4)
                    {
                        well.Error = "Well has already been classified";
                        well.ErrorCode = -3;
                        return Json(well, JsonRequestBehavior.AllowGet);
                    }

                    well.Error = "Process Failed! Please contact the Admin or try again later";
                    well.ErrorCode = 0;
                    return Json(well, JsonRequestBehavior.AllowGet);
                }

                if (well.SpudDate != null)
                {
                    well.Date = ((DateTime)well.SpudDate).ToString("yyyy/MM/dd");
                }

                well.Error = "Record was added successfully";
                well.ErrorCode = 1;
                well.WellId = k;
                return Json(well, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                well.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                well.ErrorCode = 0;
                return Json(well, JsonRequestBehavior.AllowGet);
            }
        }

         [CustomAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddWell2(Well well)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    well.Error = "Please supply all required wells and try again";
                    well.ErrorCode = -1;
                    return Json(well, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(well);

                if (wx.Code < 1)
                {
                    well.Error = wx.Error;
                    well.ErrorCode = -1;
                    return Json(well, JsonRequestBehavior.AllowGet);
                }

                Session["_well"] = well;
                if (well.SpudDate != null)
                {
                    well.Date = ((DateTime)well.SpudDate).ToString("yyyy/MM/dd");
                }

                //well.Error = "Record was added successfully";
                well.ErrorCode = 1;
                return Json(well, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                well.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                well.ErrorCode = 0;
                return Json(well, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditWell(Well well)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_well"] == null)
                {
                    well.Error = "Session has expired";
                    well.ErrorCode = 0;
                    return Json(well, JsonRequestBehavior.AllowGet);
                }

                var oldWell = Session["_well"] as Well;

                if (oldWell == null || oldWell.WellId < 1)
                {
                    well.Error = "Session has expired";
                    well.ErrorCode = -1;
                    return Json(well, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    well.Error = "Please supply all required wells and try again";
                    well.ErrorCode = -1;
                    return Json(well, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(well);

                if (wx.Code < 1)
                {
                    well.Error = wx.Error;
                    well.ErrorCode = -1;
                    return Json(well, JsonRequestBehavior.AllowGet);
                }
                
                oldWell.SpudDate = well.SpudDate;
                oldWell.Name = well.Name;
                oldWell.TechnicalAllowable = well.TechnicalAllowable;
                oldWell.WellTypeId = well.WellTypeId;
                oldWell.TotalDept = well.TotalDept;
                oldWell.Remarks = well.Remarks;
                if (well.SpudDate != null)
                {
                    well.Date = ((DateTime)well.SpudDate).ToString("yyyy/MM/dd");
                }

                var k = new WellServices().UpdateWellCheckDuplicate(oldWell);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        well.Error = "Well  already exists";
                        well.ErrorCode = 0;
                        return Json(well, JsonRequestBehavior.AllowGet);
                    }

                    well.Error = "Process Failed! Please contact the Admin or try again later";
                    well.ErrorCode = 0;
                    return Json(well, JsonRequestBehavior.AllowGet);
                }
                
                    well.Error = "Well  Information was successfully updated";
                    well.ErrorCode = 1;
                    return Json(well, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                well.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                well.ErrorCode = 0;
                return Json(well, JsonRequestBehavior.AllowGet);
            }
        }

         [CustomAuthorize(Roles = "Admin")]
        [HttpPost]
        [ActionName("DeleteWell")]
        public ActionResult DeleteWell(int id)
        {
            var well = new Well();

            try
            {
                if (id < 1)
                {
                    well.Error = "Invalid Selection";
                    well.ErrorCode = 0;
                    return Json(well, JsonRequestBehavior.AllowGet);
                }
                if (new WellServices().DeleteWellCheckReferences(id))
                {
                    well.Error = "Well  Information was successfully deleted.";
                    well.ErrorCode = 1;
                    return Json(well, JsonRequestBehavior.AllowGet);
                }

                well.Error = "Process Failed! Please try again later";
                well.ErrorCode = 0;
                return Json(well, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                well.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                well.ErrorCode = 0;
                return Json(well, JsonRequestBehavior.AllowGet);
            }
        }

         [CustomAuthorize(Roles = "Admin, Super_Admin")]
        public ActionResult EditWell(int id)
        {
            var well = new Well();
            try
            {
                if (id < 1)
                {
                    well.Error = "Invalid Selection!";
                    well.ErrorCode = -1;
                    return Json(well, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new WellServices().GetWell(id);

                if (myViewObj == null || myViewObj.WellId < 1)
                {
                    well.Error = "Well  Information could not be retrieved.";
                    well.ErrorCode = -1;
                    return Json(well, JsonRequestBehavior.AllowGet);
                }
                Session["_well"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.WellId;
                if (well.SpudDate != null)
                {
                    well.Date = ((DateTime)well.SpudDate).ToString("yyyy/MM/dd");
                }

                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                well.Error = "An unknown error was Well  Information could not be retrieved.";
                well.ErrorCode = -1;
                return Json(well, JsonRequestBehavior.AllowGet);
            }
        }

        private static GenericValidator ValidateControl(Well model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.Name.Trim()))
                {
                    gVal.Error = "Please provide Well  Name.";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.FieldId < 1)
                {
                    gVal.Error = "Please select a valid Field";
                    gVal.Code = 0;
                    return gVal;
                }
                
                if (model.WellTypeId < 1)
                {
                    gVal.Error = "Please select a valid Well Type";
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

        private List<Block> GetBlocks()
        {
            return new BlockServices().GetBlocks() ?? new List<Block>();
        }
        private List<Terrain> GetTerrains()
        {
            return new TerrainServices().GetAllOrderedTerrains() ?? new List<Terrain>();
        }
        private List<Company> GetCompanyies()
        {
            return new CompanyServices().GetAllOrderedCompanies() ?? new List<Company>();
        }

        public ActionResult WellUpload()
        {
           return View();
        }
        
         [CustomAuthorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult WellUpload(HttpPostedFileBase file)
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

                    var mList = new List<Well>();
                    var msg = string.Empty;
                    if (!new WellUploadManager().Import(Server.MapPath(path), "well", ref mList, ref msg))
                    {
                        ViewBag.ErrorCode = -1;
                        ViewBag.ErrorMessage = msg;
                        return View();
                    }

                    if (!mList.Any())
                    {
                        
                        ViewBag.ErrorCode = -1;
                        ViewBag.ErrorMessage = msg;
                        return View();
                    }
                    
                    var errorList = new List<Well>();
                    var succesList = new List<Well>();
                    foreach (var wellInfo in mList)
                    {

                        var wellId = new WellServices().AddWellCheckDuplicate2(wellInfo);

                        if (wellId < 1)
                        {
                            errorList.Add(wellInfo);
                        }
                        else
                        {
                            succesList.Add(wellInfo);
 
                            if (wellId > 0)
                            {
                                if (!string.IsNullOrEmpty(wellInfo.WellClassName))
                                {
                                    new WellClasServices().CreateWellClassAddClassification(wellInfo.WellClassName, wellId);
                                }
                            }
                        }
                    }
                    if (errorList.Any() && succesList.Any())
                    {
                        var ts = succesList.Count + " records were successfully uploaded." +
                            "\n" + errorList.Count + " record(s) could not be uploaded due to duplicates/unknown errors encountered.";
                        ViewBag.ErrorCode = -1;
                        
                        if (msg.Length > 0)
                        {
                            ts += "<br/>" + msg;
                        }
                        ViewBag.ErrorMessage = ts;
                        return View(); 
                    }

                    if (errorList.Any() && !succesList.Any())
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

                    if (!errorList.Any() && succesList.Any())
                    {
                        ViewBag.ErrorCode = 5;
                        //ViewBag.ErrorMessage = mList.Count + " records were successfully uploaded.";

                        var tsx = succesList.Count + " records were successfully uploaded.";
                        
                        if (msg.Length > 0)
                        {
                            tsx += "<br/>" + msg;
                        }
                        ViewBag.ErrorMessage = tsx;
                        return View();
                    }
                    

                    //return View("Wells", new WellViewModel { WellTypes = GetWellTypes(), Companies = GetCompanies(), Blocks = GetBlocks(), Wells = GetWells(ItemsPerPage, PageNumber, out dataCount), WellClasses = GetWellClasses() });
                }
                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = "The selected file is invalid";
                return View(); 

            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = "The selected file is invalid";
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
                  var tty = GetCompanies();
                  if (!tty.Any())
                  {
                      var txy = new WellViewModel
                      {
                          Companies = new List<Company>(),
                          WellReportObjects = new List<WellReportObject>()
                      };
                      return View(txy);  
                  }
                  return View(new WellViewModel
                  {
                      Companies = tty,
                      WellReportObjects = new List<WellReportObject>()
                  });
            }
            catch (Exception ex)
            {
                return View(new WellViewModel
                {
                    Companies = new List<Company>(),
                    WellReportObjects = new List<WellReportObject>()
                });
            }
        }

        public ActionResult GetMoreStaticWellReports()
        {
            int currentPage;

            var o = Session["_wellRepoPage"];
            if (Session["_companyId"] == null)
            {
                return Json(new List<Company>(), JsonRequestBehavior.AllowGet);
            }

            var companyId = (long)Session["_companyId"];
            if (companyId < 1)
            {
                return Json(new List<Company>(), JsonRequestBehavior.AllowGet);
            }
            if (o != null)
            {
                var page = (int)o;
                if (page < 1)
                {
                    currentPage = 1;
                }

                else
                {
                    currentPage = page + 1;
                }
            }
            else
            {
                currentPage = 1;
            }

            var workoverReportList = GetWellObjects(50, currentPage, companyId);

            if (!workoverReportList.Any())
            {
                return Json(new List<WellReportObject>(), JsonRequestBehavior.AllowGet);
            }
            Session["_wellRepoPage"] = currentPage;
            return Json(workoverReportList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWellReportsByCompany(long companyId)
        {
            const int currentPage = 1;

            var wellReportList = GetWellObjects(50, currentPage, companyId);

            if (!wellReportList.Any())
            {
                return View("Report", new WellViewModel{WellReportObjects = new List<WellReportObject>(), Companies = GetCompanies()});
            }

            Session["_wellRepoPage"] = currentPage;
            Session["_companyId"] = companyId;
            return View("Report", new WellViewModel { WellReportObjects = wellReportList, Companies = GetCompanies() });
        }

      }
}