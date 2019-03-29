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
    public class WellWorkOverController : Controller
    {
        public WellWorkOverController()  
		{
			 ViewBag.LoadStatus = "0";
		}

        private const int ItemsPerPage = 50;
        private const int PageNumber = 1;

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        public ViewResult WellWorkovers(string returnMsg = null, int returnCode = 0)
        {

            try
            {
                var wellWorkOverReasonList = GetWellWorkoverReasons();

                if (!wellWorkOverReasonList.Any())
                {
                    ViewBag.Title = "Well Workover Reason SetUp";
                    return View("~/Views/WellWorkOverReason/WellWorkOverReasons.cshtml", wellWorkOverReasonList);
                }

                var equipmentList = GetEquipmets();

                if (!equipmentList.Any())
                {
                    ViewBag.Edit = 0;
                    ViewBag.Title = "Equipment SetUp";
                    var equipmentTypeList = new EquipmentTypeServices().GetAllOrderedEquipmentTypes() ?? new List<EquipmentType>();

                    if (!equipmentTypeList.Any())
                    {
                        ViewBag.Edit = 0;
                        ViewBag.Title = "Equipment Type SetUp";
                        return View("~/Views/EquipmentType/EquipmentTypes.cshtml", new List<EquipmentType>());
                    }
                    return View("~/Views/Equipment/Equipments.cshtml", Tuple.Create(equipmentTypeList, new List<Equipment>()));
                }

                var wells = GetWells();
                if (!wells.Any())
                {
                    ViewBag.Edit = 1;
                    ViewBag.Title = "Well Set Up";
                    var wellTypeList = new WellTypeServices().GetAllOrderedWellTypes() ?? new List<WellType>();

                    if (!wellTypeList.Any())
                    {
                        ViewBag.Edit = 0;
                        ViewBag.Title = "Well Type Set Up";
                        return View("~/Views/WellType/WellTypes.cshtml", new List<WellType>());
                    }

                    var companyList = new CompanyServices().GetAllOrderedCompanies() ?? new List<Company>();
                    if (!companyList.Any())
                    {
                        ViewBag.Edit = 0;
                        ViewBag.Title = "Company SetUp";
                        return View("~/Views/Company/Companies.cshtml", new List<Company>());
                    }
                    var wellClassList = GetWellClasses();
                    if (!wellClassList.Any())
                    {
                        ViewBag.Edit = 0;
                        ViewBag.Title = "well Class Set Up";
                        return View("~/Views/WellClass/WellClasses.cshtml", wellClassList);
                    }

                    return View("~/Views/Well/Wells.cshtml", new WellViewModel { WellTypes = wellTypeList, Companies = companyList, WellClasses = wellClassList, Wells = new List<Well>() });
                }

                var yearList = GetYears();
                var monthList = GetMonths();
                int dataCount;
                var wellWorkOvers = GetWellWorkovers(ItemsPerPage, PageNumber, out dataCount);

                if (!wellWorkOvers.Any())
                {
                    ViewBag.Edit = 1;
                    ViewBag.Title = "Well Workover SetUp";

                    return View(new WellWorkoverViewModel
                    {
                        WellWorkOverReasons = wellWorkOverReasonList,
                        Equipments = equipmentList,
                        WellObjects = wells,
                        WellWorkOvers = new List<WellWorkover>()
                    });
                }

                var txx = new WellWorkoverViewModel
                {
                    WellWorkOverReasons = wellWorkOverReasonList,
                    Equipments = equipmentList,
                    WellObjects = wells,
                    WellWorkOvers = wellWorkOvers
                };

                ViewBag.Title = "Manage Well Workovers";
                Session["_workoverPage"] = 1;
                return View(txx);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                var txx = new WellWorkoverViewModel
                {
                    WellWorkOverReasons = GetWellWorkoverReasons(),
                    Equipments = GetEquipmets(),
                    WellObjects = GetWells(),
                    WellWorkOvers = new List<WellWorkover>()
                };

                ViewBag.Title = "Manage Well Workovers";
                return View(txx);
            }
        }

        public ActionResult GetMoreWellWorkOvers()
        {
            int currentPage;

            var o = Session["_workoverPage"];
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

            var wellList = GetWellWorkOverObjects(50, currentPage);

            if (!wellList.Any())
            {
                return Json(new List<WellWorkoverReportObject>(), JsonRequestBehavior.AllowGet);
            }
            Session["_workoverPage"] = currentPage;
            return Json(wellList, JsonRequestBehavior.AllowGet);
        }    
        private List<WellWorkoverReportObject> GetWellWorkOverObjects(int itemsPerPage, int pageNumber)
        {
            try
            {
                var wellCompletions = new WellWorkoverServices().GetMoreWellWorkovers(itemsPerPage, pageNumber);

                if (!wellCompletions.Any())
                {

                    return new List<WellWorkoverReportObject>();
                }

                return wellCompletions;
            }
            catch (Exception ex)
            {
                return new List<WellWorkoverReportObject>();
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
        private List<WellWorkOverReason> GetWellWorkoverReasons()
        {
            return new WellWorkOverReasonServices().GetAllOrderedWellWorkOverReasons() ?? new List<WellWorkOverReason>();
        }
        private List<WellWorkover> GetWellWorkovers(int itemsPerPage, int pageNumber, out int dataCount)
        {
            try
            {
                var wellWorkOvers = new WellWorkoverServices().GetAllOrderedWellWorkovers(itemsPerPage, pageNumber, out dataCount);

                ViewBag.PrDataCount = dataCount.ToString(CultureInfo.InvariantCulture);

                var totalPages = dataCount / itemsPerPage;

                // Counting the last page
                if (dataCount % itemsPerPage != 0)
                {
                    totalPages++;
                }

                ViewBag.PrTotalPages = totalPages;
                ViewBag.PrPage = pageNumber;
                if (!wellWorkOvers.Any())
                {
                    return new List<WellWorkover>();
                }

                wellWorkOvers.ForEach(m =>
                {
                    m.WellName = m.Well.Name;
                    m.EquipmentName = m.Equipment.Name;
                    m.Reason = m.WellWorkOverReason.Title;
                    m.MonthStr = Enum.GetName(typeof(MonthList), m.Month);
                    m.WorkoverPeriod = m.Month + "/" + m.Year;
                });

                return wellWorkOvers;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                dataCount = 0;
                return new List<WellWorkover>();
            }
        }
        private List<WellWorkover> GetWellWorkoverReports(int itemsPerPage, int pageNumber)
        {
            try
            {
                var wellWorkOvers = new WellWorkoverServices().GetWellWorkoversForReport(itemsPerPage, pageNumber);

                if (!wellWorkOvers.Any())
                {
                    return new List<WellWorkover>();
                }

              return wellWorkOvers;
            }
            catch (Exception ex)
            {
              return new List<WellWorkover>();
            }
        }
        private List<Equipment> GetEquipmets()
        {
            return new EquipmentServices().GetAllOrderedEquipments() ?? new List<Equipment>();
        }
        private List<WellObject> GetWells()
        {
            var ttg = new WellServices().GetWells(200, 1);
            if (!ttg.Any())
            {
                return new List<WellObject>();
            }
            Session["_normWorkWellpageNumber"] = 1;
            return ttg;

        }
        public ActionResult GetMoreWells()
        {
            int pageNumber = 1;

            var o = Session["_normWorkWellpageNumber"];
            if (o != null)
            {
                var tfd = (int)o;
                if (tfd < 1)
                {
                    pageNumber = 1;
                }
                else
                {
                    pageNumber += tfd;
                }
            }

            var dfg = new WellServices().GetWells(400, pageNumber);
            if (!dfg.Any())
            {
                return Json(new List<WellObject>(), JsonRequestBehavior.AllowGet);
            }
            Session["_normWorkWellpageNumber"] = pageNumber;
            return Json(dfg, JsonRequestBehavior.AllowGet);
        }
        private List<DocObject> GetMonths()
        {
            return new EnumToObjList().GetMonthList().OrderByDescending(m => m.DocId).ToList();
        }
        private List<DocObject> GetYears()
        {
            var jxs = new WellWorkoverServices().GetWellWorkoverYears();
            if (!jxs.Any())
            {
                return new List<DocObject>();
            }
            var tdv = new List<DocObject>();
            jxs.ForEach(m =>
            {
                var ts = m;

                if (!tdv.Exists(t => t.DocId == ts))
                {
                    tdv.Add(new DocObject{ DocName = m.ToString(CultureInfo.InvariantCulture), DocId = ts});
                }
            });
            return tdv.OrderByDescending(m => m.DocName).ToList();
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        [HttpPost]
        public ActionResult AddWellWorkover(WellWorkover wellWorkover)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    wellWorkover.Error = "Please supply all required well Completions and try again";
                    wellWorkover.ErrorCode = -1;
                    return Json(wellWorkover, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(wellWorkover);

                if (wx.Code < 1)
                {
                    wellWorkover.Error = wx.Error;
                    wellWorkover.ErrorCode = -1;
                    return Json(wellWorkover, JsonRequestBehavior.AllowGet);
                }

                wellWorkover.Year = wellWorkover.WorkoverDate.Year;
                wellWorkover.Month = wellWorkover.WorkoverDate.Month;
                wellWorkover.DateCompleted = wellWorkover.WorkoverDate.ToString("yyyy/mm/dd");
                var k = new WellWorkoverServices().AddWellWorkover(wellWorkover);
                if (k < 1)
                {
                    wellWorkover.Error = "Process Failed! Please contact the Admin or try again later";
                    wellWorkover.ErrorCode = 0;
                    return Json(wellWorkover, JsonRequestBehavior.AllowGet);
                }

                wellWorkover.Error = "Record was added successfully";
                wellWorkover.ErrorCode = 1;
                wellWorkover.WellWorkOverId = k;
                return Json(wellWorkover, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellWorkover.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                wellWorkover.ErrorCode = 0;
                return Json(wellWorkover, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        [HttpPost]
        public ActionResult EditWellWorkover(WellWorkover wellWorkover)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_wellWorkover"] == null)
                {
                    wellWorkover.Error = "Session has expired";
                    wellWorkover.ErrorCode = 0;
                    return Json(wellWorkover, JsonRequestBehavior.AllowGet);
                }

                var oldWellWorkover = Session["_wellWorkover"] as WellWorkover;

                if (oldWellWorkover == null || oldWellWorkover.WellWorkOverId < 1)
                {
                    wellWorkover.Error = "Session has expired";
                    wellWorkover.ErrorCode = 0;
                    return Json(wellWorkover, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    wellWorkover.Error = "Please supply all required entries and try again";
                    wellWorkover.ErrorCode = -1;
                    return Json(wellWorkover, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(wellWorkover);

                if (wx.Code < 1)
                {
                    wellWorkover.Error = wx.Error;
                    wellWorkover.ErrorCode = -1;
                    return Json(wellWorkover, JsonRequestBehavior.AllowGet);
                }

                oldWellWorkover.EquipmentId = wellWorkover.EquipmentId;
                oldWellWorkover.WellId = wellWorkover.WellId;
                oldWellWorkover.WellWorkOverReasonId = wellWorkover.WellWorkOverReasonId;
                oldWellWorkover.Year = wellWorkover.WorkoverDate.Year;
                oldWellWorkover.Month = wellWorkover.WorkoverDate.Month;
                oldWellWorkover.DateCompleted = wellWorkover.WorkoverDate.ToString("yyyy/mm/dd");
                wellWorkover.MonthStr = Enum.GetName(typeof(MonthList), oldWellWorkover.Month);
                var k = new WellWorkoverServices().UpdateWellWorkover(oldWellWorkover);
                if (k < 1)
                {
                    wellWorkover.Error = "Process Failed! Please contact the Admin or try again later";
                    wellWorkover.ErrorCode = 0;
                    return Json(wellWorkover, JsonRequestBehavior.AllowGet);
                }
                
                    wellWorkover.Error = "Well Workover Reason was successfully updated";
                    wellWorkover.ErrorCode = 1;
                    return Json(wellWorkover, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellWorkover.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                wellWorkover.ErrorCode = 0;
                return Json(wellWorkover, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        [HttpPost]
        [ActionName("DeleteWellWorkover")]
        public ActionResult DeleteWellWorkover(int id)
        {
            var wellWorkover = new WellWorkover();

            try
            {
                if (id < 1)
                {
                    wellWorkover.Error = "Invalid Selection";
                    wellWorkover.ErrorCode = 0;
                    return Json(wellWorkover, JsonRequestBehavior.AllowGet);
                }
                if (new WellWorkoverServices().DeleteWellWorkoverCheckReferences(id))
                {
                    wellWorkover.Error = "Well Workover  Information was successfully deleted.";
                    wellWorkover.ErrorCode = 1;
                    return Json(wellWorkover, JsonRequestBehavior.AllowGet);
                }

                wellWorkover.Error = "Process Failed! Please try again later";
                wellWorkover.ErrorCode = 0;
                return Json(wellWorkover, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellWorkover.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                wellWorkover.ErrorCode = 0;
                return Json(wellWorkover, JsonRequestBehavior.AllowGet);
            }
        }

       [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]  
        public ActionResult EditWellWorkover(int id)
        {
            var wellWorkover = new WellWorkover();
            try
            {
                if (id < 1)
                {
                    wellWorkover.Error = "Invalid Selection!";
                    wellWorkover.ErrorCode = -1;
                    return Json(wellWorkover, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new WellWorkoverServices().GetWellWorkover(id);

                if (myViewObj == null || myViewObj.WellWorkOverId < 1)
                {
                    wellWorkover.Error = "Well Workover information could not be retrieved.";
                    wellWorkover.ErrorCode = -1;
                    return Json(wellWorkover, JsonRequestBehavior.AllowGet);
                }
                Session["_wellWorkover"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.WellWorkOverId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellWorkover.Error = "An unknown error was encountered. Well Workover  Information could not be retrieved.";
                wellWorkover.ErrorCode = -1;
                return Json(wellWorkover, JsonRequestBehavior.AllowGet);
            }
        }
        private static GenericValidator ValidateControl(WellWorkover model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (model.WellId < 1)
                {
                    gVal.Error = "Please select a invalid Well information.";
                    gVal.Code = 0;
                    return gVal;
                }

               if (model.EquipmentId < 1)
                {
                    gVal.Error = "Please select a valid Equipment";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.WellWorkOverReasonId < 1)
                {
                    gVal.Error = "Please select a valid Well Workover Reason";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.WorkoverDate.Year == 0001)
                {
                    gVal.Error = "Please provide a valid Workover Date";
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
        public ActionResult GetWellWorkoversByMonth(GenericSearch search)
        {
            try
            {
                if (search.Month < 1)
                {
                    ViewBag.ErrorCode = -1;
                    ViewBag.ErrorMessage = "Please provide a valid Report Month";
                    return View("WellWorkovers", new WellWorkoverViewModel
                    {
                        WellWorkOverReasons = new List<WellWorkOverReason>(),
                        Equipments = new List<Equipment>(),
                        Wells = new List<Well>(),
                        WellWorkOvers = new List<WellWorkover>(),
                        YearList = GetYears(),
                        MonthList = GetMonths()
                    });
                }
                if (search.Year < 1)
                {
                    ViewBag.ErrorCode = -1;
                    ViewBag.ErrorMessage = "Please provide a valid Search Year";
                    return View("WellWorkovers", new WellWorkoverViewModel
                    {
                        WellWorkOverReasons = new List<WellWorkOverReason>(),
                        Equipments = new List<Equipment>(),
                        Wells = new List<Well>(),
                        WellWorkOvers = new List<WellWorkover>(),
                        YearList = GetYears(),
                        MonthList = GetMonths()
                    });
                }
                var wellWorkOvers = GetWorkoversBySearchDate(search.Month, search.Year);
                ViewBag.SearchPeriod = Enum.GetName(typeof(MonthList), search.Month) + "/" + search.Year;
                ViewBag.Title = "Manage Well Workovers";
                if (!wellWorkOvers.Any())
                {
                    ViewBag.ErrorCode = -1;
                    ViewBag.ErrorMessage = "No record found";

                    return View("WellWorkovers", new WellWorkoverViewModel
                    {
                        WellWorkOverReasons = GetWellWorkoverReasons(),
                        Equipments = GetEquipmets(),
                        WellObjects = GetWells(),
                        WellWorkOvers = new List<WellWorkover>(),
                        YearList = GetYears(),
                        MonthList = GetMonths()
                    });
                }


                ViewBag.ErrorCode = 5;
                ViewBag.ErrorMessage = "";

                return View("~/Views/WellWorkOver/WellWorkovers.cshtml", new WellWorkoverViewModel
                {
                    WellWorkOverReasons = GetWellWorkoverReasons(),
                    Equipments = GetEquipmets(),
                    WellObjects = GetWells(),
                    WellWorkOvers = wellWorkOvers,
                    YearList = GetYears(),
                    MonthList = GetMonths()
                });
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = "An error was encountered, the seach operation could not be completed.";
                return View("~/Views/WellWorkOver/WellWorkovers.cshtml", new WellWorkoverViewModel
                {
                    WellWorkOverReasons = GetWellWorkoverReasons(),
                    Equipments = GetEquipmets(),
                    WellObjects = GetWells(),
                    WellWorkOvers = new List<WellWorkover>(),
                    YearList = GetYears(),
                    MonthList = GetMonths()
                });
            }
        }
        public List<WellWorkover> GetWorkoversBySearchDate(int searchMonth, int searchYear)
        {
            try
            {
                var wellWorkOvers = new WellWorkoverServices().GetAllOrderedWellWorkoversByMonth(searchMonth, searchYear);

                if (!wellWorkOvers.Any())
                {
                    return new List<WellWorkover>();
                }

                wellWorkOvers.ForEach(m =>
                {
                    m.WellName = m.Well.Name;
                    m.EquipmentName = m.Equipment.Name; 
                    m.Reason = m.WellWorkOverReason.Title;
                    m.MonthStr = Enum.GetName(typeof(MonthList), m.Month);
                });
                return wellWorkOvers;
            }
            catch (Exception ex)
            {
                return new List<WellWorkover>();
            }
        }
        public ActionResult GetWorkoversByQueryDate(GenericSearch search)
        {
            try
            {
                if (search.Year < 1)
                {
                    ViewBag.ErrorCode = -1;
                    ViewBag.ErrorMessage = "Please provide a valid Report Year";
                    return View("Report", new WellWorkoverViewModel
                    {
                        WellWorkOverReasons = new List<WellWorkOverReason>(),
                        Equipments = new List<Equipment>(),
                        Wells = new List<Well>(),
                        WellWorkOvers = new List<WellWorkover>(),
                        WellWorkoverReportObjects = new List<WellWorkoverReportObject>(),
                        YearList = GetYears(),
                        MonthList = GetMonths()
                    });
                }

                if (search.Month > 0)
                {
                    ViewBag.SearchPeriod = Enum.GetName(typeof(MonthList), search.Month) + "/" + search.Year;
                }

                else
                {
                    ViewBag.SearchPeriod = search.Year;
                }

                var txd = GetWorkoverObjects(PageNumber, ItemsPerPage, search);
                if (!txd.Any())
                {

                    return View("Report", new WellWorkoverViewModel
                    {
                        WellWorkOverReasons = new List<WellWorkOverReason>(),
                        Equipments = new List<Equipment>(),
                        Wells = new List<Well>(),
                        WellWorkOvers = new List<WellWorkover>(),
                        WellWorkoverReportObjects = new List<WellWorkoverReportObject>(),
                        YearList = GetYears(),
                        MonthList = GetMonths()
                    });

                }

              var txx = new WellWorkoverViewModel
              {
                  WellWorkOverReasons = new List<WellWorkOverReason>(),
                  Equipments = new List<Equipment>(),
                  Wells = new List<Well>(),
                  WellWorkOvers = new List<WellWorkover>(),
                  WellWorkoverReportObjects = txd,
                  YearList = GetYears(),
                  MonthList = GetMonths()
              };
              Session["_workRepoPage"] = 1;
              Session["_workoverRepoGenSearch"] = search;
                return View("Report", txx);


            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                ViewBag.ErrorMessage = "No record found";
                ViewBag.ErrorCode = -1;
                return View("Report", new WellWorkoverViewModel
                {
                    WellWorkOverReasons = new List<WellWorkOverReason>(),
                    Equipments = new List<Equipment>(),
                    Wells = new List<Well>(),
                    WellWorkOvers = new List<WellWorkover>(),
                    YearList = GetYears(),
                    MonthList = GetMonths()
                });
            }
        }
        public ActionResult Report()
        {
            return View(new WellWorkoverViewModel
            {
                WellWorkOverReasons = new List<WellWorkOverReason>(),
                Equipments = new List<Equipment>(),
                Wells = new List<Well>(),
                WellWorkOvers = new List<WellWorkover>(),
                WellWorkoverReportObjects = new List<WellWorkoverReportObject>(),
                YearList = GetYears(),
                MonthList = GetMonths()
            });
        }
        public ActionResult GetMoreStaticWorkOverReports()
        {
            int currentPage;

            if (Session["_workoverRepoGenSearch"] == null)
            {
                return Json(new List<WellWorkoverReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var search = Session["_workoverRepoGenSearch"] as GenericSearch;

            if (search == null || search.Year < 1)
            {
                return Json(new List<WellWorkoverReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var o = Session["_workRepoPage"];
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

            var workoverReportList = GetWorkoverObjects(50, currentPage, search);

            if (!workoverReportList.Any())
            {
                return Json(new List<WellWorkoverReportObject>(), JsonRequestBehavior.AllowGet);
            }
            Session["_workRepoPage"] = currentPage;
            return Json(workoverReportList, JsonRequestBehavior.AllowGet);
        }
        private List<WellWorkoverReportObject> GetWorkoverObjects(int pageNumber, int itemsPerPage, GenericSearch search)
        {
            try
            {
                var wellWorkoverObjects = new WellWorkoverServices().GetMoreWellWorkoverReport(itemsPerPage, pageNumber, search.Month, search.Year);

                if (!wellWorkoverObjects.Any())
                {

                    return new List<WellWorkoverReportObject>();
                }

                return wellWorkoverObjects;
            }
            catch (Exception ex)
            {
                return new List<WellWorkoverReportObject>();
            }
        }
        public ActionResult WellWorkoverUpload()
        {
           return View();
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        [HttpPost]
        public ActionResult WellWorkoverUpload(HttpPostedFileBase file)
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
                    var mList = new List<WellWorkover>();
                    var msg = string.Empty;
                    if (!new WellWorkOverUploadManager().Import(Server.MapPath(path), "Well_Workovers", ref mList, ref msg))
                    {
                        ViewBag.ErrorCode = -1;
                        ViewBag.ErrorMessage = msg;
                        return View();
                    }

                    if (mList.Any() && msg.Length > 0)
                    {
                        ViewBag.ErrorCode = -1;
                        ViewBag.ErrorMessage = mList.Count + " records were successfully uploaded.<br/>" + msg;
                        return View();
                    }
                    
                    ViewBag.ErrorCode = 5;
                    var tsx = mList.Count + " records were successfully uploaded.";
                    ViewBag.ErrorMessage = tsx;
                    return View();
                }

                else
                {
                    ViewBag.ErrorCode = -1;
                    ViewBag.ErrorMessage = "Error: content of file could not be accessed. File is empty or corrupted.";
                    return View();
                }
               
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = "Error: content of file could not be accessed. File is empty or corrupted.";
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
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