using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DPR_DataMigrationEngine.BulkUploadManagerManager;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;
using DPR_DataMigrationEngine.ViewModels;

namespace DPR_DataMigrationEngine.Controllers.WellManagement
{
    [CustomAuthorize]
    public class WellCompletionController : Controller    
    {
        public WellCompletionController() 
		{
			 ViewBag.LoadStatus = "0";
		}
        private const int ItemsPerPage = 50;
        private const int PageNumber = 1;

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        public ActionResult WellCompletions()
        {
            try
            {
                var wellCompletionTypeList = GetWellCompletionTypes();

                if (!wellCompletionTypeList.Any())
                {
                    return RedirectToAction("WellCompletionTypes", "WellCompletionType");
                }

                var equipmentList = GetEquipments();

                if (!equipmentList.Any())
                {
                  return RedirectToAction("Equipments", "Equipment");
                    
                }

                var wells = GetWells();
                if (!wells.Any())
                {
                    return RedirectToAction("Wells", "Well");
                }

                int dataCount;
                var wellCompletions = GetWellComletions(ItemsPerPage, PageNumber, out dataCount);

                if (!wellCompletions.Any())
                {
                    ViewBag.Edit = 1;
                    ViewBag.Title = "Well Completion SetUp";
                    return View(new WellCompletionViewModel
                    {
                        WellCompletionTypes = wellCompletionTypeList,
                        WellObjects = wells,
                        Equipments = equipmentList,
                        WellCompletions = new List<WellCompletion>(),
                        MonthList = GetMonths(),
                        YearList = GetYears()
                    });
                }


                var txx = new WellCompletionViewModel
                {
                    WellCompletionTypes = wellCompletionTypeList,
                    WellObjects = wells,
                    Equipments = equipmentList,
                    WellCompletions = wellCompletions,
                    MonthList = GetMonths(),
                    YearList = GetYears()
                };
                Session["_wellCompPage"] = 1;
                ViewBag.Title = "Manage Well Completions";
                return View(txx);
            }
            catch (Exception ex)
            {

                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage =
                    "An unknown error was encountered. Well Copletion list could not be retrieved.";
                return View(new WellCompletionViewModel
                {
                    WellCompletionTypes = GetWellCompletionTypes(),
                    WellObjects = GetWells(), 
                    Equipments = GetEquipments(), WellCompletions = new List<WellCompletion>(),
                                                          MonthList = GetMonths(),
                                                          YearList = GetYears()
                });
            }
        }
        private List<WellCompletionType> GetWellCompletionTypes()
        {
            try
            {
               return new WellCompletionTypeServices().GetAllOrderedWellCompletionTypes() ?? new List<WellCompletionType>();
            }
            catch (Exception ex)
            {
                return new List<WellCompletionType>();
            }
        }
        private List<Equipment> GetEquipments()
        {
            try
            {
                return new EquipmentServices().GetAllOrderedEquipments() ?? new List<Equipment>();
            }
            catch (Exception ex)
            {
                return new List<Equipment>();
            }
        }
        private List<WellObject> GetWells()
        {
            var ttg = new WellServices().GetWells(200, 1);
            if (!ttg.Any())
            {
                return new List<WellObject>();
            }
            Session["_normWellpageNumber"] = 1;
            return ttg;

        }
        public ActionResult GetMoreWells()
        {
            int pageNumber = 1;

            var o = Session["_normWellpageNumber"];
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
            Session["_normWellpageNumber"] = pageNumber;
            return Json(dfg, JsonRequestBehavior.AllowGet);
        }
        private List<WellCompletion> GetWellComletions(int itemsPerPage, int pageNumber, out int dataCount)
        {
            try
            {
                var wellCompletions = new WellCompletionServices().GetAllOrderedWellCompletions(itemsPerPage, pageNumber, out dataCount);
                
                if (!wellCompletions.Any())
                {
                    dataCount = 0;
                    ViewBag.TotalPages = 0;
                    ViewBag.Page = 1;
                    return new List<WellCompletion>();
                }
                ViewBag.PrDataCount = dataCount.ToString(CultureInfo.InvariantCulture);

                var totalPages = dataCount / itemsPerPage;

                if (dataCount % itemsPerPage != 0)
                {

                    totalPages++;
                }

                ViewBag.PrTotalPages = totalPages;
                ViewBag.PrPage = pageNumber;
                wellCompletions.ForEach(m =>
                {
                    m.WellName = m.Well.Name;
                    m.WellCompletionTypeName = m.WellCompletionType.Type;
                    m.EquipmentName = m.Equipment.Name;
                });

                return wellCompletions;
            }
            catch (Exception ex)
            {
                dataCount = 0;
                ViewBag.TotalPages = 0;
                ViewBag.Page = 1;
                return new List<WellCompletion>();
            }
        }
        public ActionResult GetMoreWellCompletions()
        {
            int currentPage;

            var o = Session["_wellCompPage"];
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

            var wellList = GetWellComletionObjects(50, currentPage);

            if (!wellList.Any())
            {
                return Json(new List<Company>(), JsonRequestBehavior.AllowGet);
            }
            Session["_wellCompPage"] = currentPage;
            return Json(wellList, JsonRequestBehavior.AllowGet);
        }
        private List<WellCompletionObject> GetWellComletionObjects(int itemsPerPage, int pageNumber)
        {
            try
            {
                var wellCompletions = new WellCompletionServices().GetWellObjects(itemsPerPage, pageNumber);

                if (!wellCompletions.Any())
                {

                    return new List<WellCompletionObject>();
                }
               
                return wellCompletions;
            }
            catch (Exception ex)
            {
                return new List<WellCompletionObject>();
            }
        }
        private List<WellCompletion> GetWellComletionReportByQueryPeriods(GenericSearch search)
        {
            try
            {
                var wellCompletions = new WellCompletionServices().GetAllOrderedWellCompletionReportByPeriod(search.Month, search.Year);

                if (!wellCompletions.Any())
                {
                    return new List<WellCompletion>();
                }
                
                return wellCompletions;
            }
            catch (Exception ex)
            {
                //dataCount = 0;
                //ViewBag.TotalPages = 0;
                //ViewBag.Page = 1;
                return new List<WellCompletion>();
            }
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        [HttpPost]
        public ActionResult AddWellCompletion(WellCompletion wellCompletion)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    wellCompletion.Error = "Please supply all required well Completions and try again";
                    wellCompletion.ErrorCode = -1;
                    return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(wellCompletion);

                if (wx.Code < 1)
                {
                    wellCompletion.Error = wx.Error;
                    wellCompletion.ErrorCode = -1;
                    return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                }

               if (wellCompletion.U1 <= 0 )
                {
                    wellCompletion.Error = "Please provide a valid Upper Interval";
                    wellCompletion.ErrorCode = -1;
                    return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                }
                
                if (wellCompletion.WellCompletionTypeId > 1 )
                {
                   if (wellCompletion.L2 <= 0 )
                    {
                        wellCompletion.Error = "Please provide a valid Lower Intervalv 2";
                        wellCompletion.ErrorCode = -1;
                        return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                    }

                    if (wellCompletion.U2 <= 0 )
                    {
                        wellCompletion.Error = "Please provide a valid Upper Interval 2";
                        wellCompletion.ErrorCode = -1;
                        return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                    }

                    if (wellCompletion.U2 < wellCompletion.L2 || wellCompletion.U2.Equals(wellCompletion.L2))
                    {
                        wellCompletion.Error = "Please provide a valid Upper Interval 2";
                        wellCompletion.ErrorCode = -1;
                        return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                    }
                
                }
                
                var k = new WellCompletionServices().AddWellCompletionCheckDuplicate(wellCompletion);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        wellCompletion.Error = "Well Completion  already exists";
                        wellCompletion.ErrorCode = -3;
                        return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                    }

                    wellCompletion.Error = "Process Failed! Please contact the Admin or try again later";
                    wellCompletion.ErrorCode = 0;
                    return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                }

                wellCompletion.Error = "Record was added successfully";
                wellCompletion.ErrorCode = 1;
                wellCompletion.WellCompletionId = k;
                wellCompletion.DatecomPletedString=wellCompletion.DateCompleted.ToString();
                return Json(wellCompletion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellCompletion.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                wellCompletion.ErrorCode = 0;
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);

                return Json(wellCompletion, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        [HttpPost]
        public ActionResult EditWellCompletion(WellCompletion wellCompletion)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_wellCompletion"] == null || Session["_wellCompObj"] == null)
                {
                    wellCompletion.Error = "Session has expired";
                    wellCompletion.ErrorCode = 0;
                    return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                }

               var oldWellCompletion = Session["_wellCompletion"] as WellCompletion;
               var oldWellCompObj = Session["_wellCompObj"] as WellCompletionObject;

                if (oldWellCompletion == null || oldWellCompletion.WellCompletionId < 1 || oldWellCompObj == null || oldWellCompObj.WellCompletionId < 1)
                {
                    wellCompletion.Error = "Session has expired";
                    wellCompletion.ErrorCode = 0;
                    return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    wellCompletion.Error = "Please supply all required entries and try again";
                    wellCompletion.ErrorCode = -1;
                    return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(wellCompletion);

                if (wx.Code < 1)
                {
                    wellCompletion.Error = wx.Error;
                    wellCompletion.ErrorCode = -1;
                    return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                }

                oldWellCompletion.EquipmentId = wellCompletion.EquipmentId;
                oldWellCompletion.WellId = wellCompletion.WellId;
                oldWellCompletion.WellCompletionTypeId = wellCompletion.WellCompletionTypeId;
                wellCompletion.IsInitial = wellCompletion.IsInitial;

                if (wellCompletion.WellCompletionTypeId > 1)
                {
                    if (wellCompletion.L2 <= 0)
                    {
                        wellCompletion.Error = "Please provide a valid Lower Intervalv 2";
                        wellCompletion.ErrorCode = -1;
                        return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                    }

                    if (wellCompletion.U2 <= 0)
                    {
                        wellCompletion.Error = "Please provide a valid Upper Interval 2";
                        wellCompletion.ErrorCode = -1;
                        return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                    }

                    if (wellCompletion.U2 < wellCompletion.L2 || wellCompletion.U2.Equals(wellCompletion.L2))
                    {
                        wellCompletion.Error = "Please provide a valid Second Upper Interval 2";
                        wellCompletion.ErrorCode = -1;
                        return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                    }

                   
                    var newCompletionInterval2 = new WellCompletionInterval
                    {
                        LowerInterval = wellCompletion.L2,
                        UpperInterval = wellCompletion.U2,
                        DateCompleted = wellCompletion.DateCompleted.ToString(),
                        LastUpdatedTime = DateTime.Now.ToString("g"),
                        WellCompletionId = oldWellCompletion.WellCompletionId
                    };

                    if (oldWellCompObj.IntervalId2 > 0)
                    {
                        newCompletionInterval2.WellCompletionIntervalId = oldWellCompObj.IntervalId2;
                        var r = new WellCompletionIntervalServices().UpdateWellCompletionIntervalCheckDuplicate(newCompletionInterval2);
                        if (r < 1)
                        {
                            wellCompletion.Error = "Well Completion Interval could not be updated";
                            wellCompletion.ErrorCode = 0;
                            return Json(wellCompletion, JsonRequestBehavior.AllowGet);

                        }

                    }

                    if (oldWellCompObj.IntervalId2 < 1)
                    {
                        var r = new WellCompletionIntervalServices().AddWellCompletionIntervalCheckDuplicate(newCompletionInterval2);
                        if (r < 1)
                        {
                            wellCompletion.Error = "Well Completion Interval could not be updated";
                            wellCompletion.ErrorCode = 0;
                            return Json(wellCompletion, JsonRequestBehavior.AllowGet);

                        }

                    }

                    var newCompletionInterval1 = new WellCompletionInterval
                    {
                        LowerInterval = wellCompletion.L1,
                        UpperInterval = wellCompletion.U1,
                        DateCompleted = wellCompletion.DateCompleted.ToString(),
                        LastUpdatedTime = DateTime.Now.ToString("g"),
                        WellCompletionId = oldWellCompletion.WellCompletionId
                    };

                    if (oldWellCompObj.IntervalId1 > 0)
                    {
                        newCompletionInterval1.WellCompletionIntervalId = oldWellCompObj.IntervalId1;
                        var r = new WellCompletionIntervalServices().UpdateWellCompletionIntervalCheckDuplicate(newCompletionInterval1);
                        if (r < 1)
                        {
                            wellCompletion.Error = "Well Completion Interval could not be updated";
                            wellCompletion.ErrorCode = 0;
                            return Json(wellCompletion, JsonRequestBehavior.AllowGet);

                        }

                    }

                    if (oldWellCompObj.IntervalId1 < 1)
                    {
                        var r = new WellCompletionIntervalServices().AddWellCompletionIntervalCheckDuplicate(newCompletionInterval1);
                        if (r < 1)
                        {
                            wellCompletion.Error = "Well Completion Interval could not be updated";
                            wellCompletion.ErrorCode = 0;
                            return Json(wellCompletion, JsonRequestBehavior.AllowGet);

                        }

                    }

                }

                if (wellCompletion.WellCompletionTypeId < 2)
                {
                   
                    if (wellCompletion.U1 <= 0)
                    {
                        wellCompletion.Error = "Please provide a valid Upper Interval";
                        wellCompletion.ErrorCode = -1;
                        return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                    }

                    if (wellCompletion.U1 < wellCompletion.L1 || wellCompletion.U1.Equals(wellCompletion.L1))
                    {
                        wellCompletion.Error = "Please provide a valid Upper Interval";
                        wellCompletion.ErrorCode = -1;
                        return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                    }

                    if (oldWellCompObj.IntervalId2 > 0)
                    {
                        var r = new WellCompletionIntervalServices().DeleteWellCompletionIntervalCheckReferences(oldWellCompObj.IntervalId2);
                        if (!r)
                        {
                            wellCompletion.Error = "The process could not be completed. The requested item update will cause data loss.";
                            wellCompletion.ErrorCode = 0;
                            return Json(wellCompletion, JsonRequestBehavior.AllowGet);

                        }

                    }
                    
                     var newCompletionInterval = new WellCompletionInterval
                     {
                         LowerInterval = wellCompletion.L1,
                         UpperInterval = wellCompletion.U1,
                         DateCompleted = wellCompletion.DateCompleted.ToString(),
                         LastUpdatedTime = DateTime.Now.ToString("g"),
                         WellCompletionId = oldWellCompletion.WellCompletionId
                     };

                     if (oldWellCompObj.IntervalId1 > 0)
                     {
                         newCompletionInterval.WellCompletionIntervalId = oldWellCompObj.IntervalId1;
                         var r = new WellCompletionIntervalServices().UpdateWellCompletionIntervalCheckDuplicate(newCompletionInterval);
                         if (r < 1)
                         {

                             wellCompletion.Error = "Well Completion Interval could not be updated";
                             wellCompletion.ErrorCode = 0;
                             return Json(wellCompletion, JsonRequestBehavior.AllowGet);

                         }

                     }

                     if (oldWellCompObj.IntervalId1 < 1)
                     {
                         var r = new WellCompletionIntervalServices().AddWellCompletionIntervalCheckDuplicate(newCompletionInterval);
                         if (r < 1)
                         {

                             wellCompletion.Error = "Well Completion Interval could not be updated";
                             wellCompletion.ErrorCode = 0;
                             return Json(wellCompletion, JsonRequestBehavior.AllowGet);

                         }

                     }

                }

                var k = new WellCompletionServices().UpdateWellCompletionCheckDuplicate(oldWellCompletion);
                if (k < 1)
                {
                    wellCompletion.Error = "Process Failed! Please contact the Admin or try again later";
                    wellCompletion.ErrorCode = 0;
                    return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                }
                
                wellCompletion.Error = "Well Completion  Information was successfully updated";
                wellCompletion.ErrorCode = 1;
                return Json(wellCompletion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellCompletion.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                wellCompletion.ErrorCode = 0;
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return Json(wellCompletion, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        [HttpPost]
        [ActionName("DeleteWellCompletion")]
        public ActionResult DeleteWellCompletion(int id)
        {
            var wellCompletion = new WellCompletion();

            try
            {
                if (id < 1)
                {
                    wellCompletion.Error = "Invalid Selection";
                    wellCompletion.ErrorCode = 0;
                    return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                }
                if (new WellCompletionServices().DeleteWellCompletionCheckReferences(id))
                {
                    wellCompletion.Error = "Well Completion  Information was successfully deleted.";
                    wellCompletion.ErrorCode = 1;
                    return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                }

                wellCompletion.Error = "Process Failed! Please try again later";
                wellCompletion.ErrorCode = 0;
                return Json(wellCompletion, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellCompletion.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                wellCompletion.ErrorCode = 0;
                return Json(wellCompletion, JsonRequestBehavior.AllowGet);
            }
        }
         private List<DocObject> GetMonths()
        {
            return new EnumToObjList().GetMonthList().OrderByDescending(m => m.DocId).ToList();
        }
        private List<DocObject> GetYears()
        {
            var jxs = new WellCompletionServices().GetWellCompletionYears();
            if (!jxs.Any())
            {
                return new List<DocObject>();
            }
            var tdv = new List<DocObject>();
            jxs.ForEach(m =>
            {
                var ts = int.Parse(m);

                if (!tdv.Exists(t => t.DocId == ts))
                {
                    tdv.Add(new DocObject { DocName = m, DocId = ts });
                }
            });
            return tdv.OrderByDescending(m => m.DocName).ToList();
        }
       [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        public ActionResult EditWellCompletion(int id)
        {
            var wellCompletion = new WellCompletion();
            try
            {
                if (id < 1)
                {
                    wellCompletion.Error = "Invalid Selection!";
                    wellCompletion.ErrorCode = -1;
                    return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new WellCompletionServices().GetWellCompletion(id);

                if (myViewObj == null || myViewObj.WellCompletionId < 1)
                {
                    wellCompletion.Error = "Well Completion  Information could not be retrieved.";
                    wellCompletion.ErrorCode = -1;
                    return Json(wellCompletion, JsonRequestBehavior.AllowGet);
                }
                
                var welllcompObj = new WellCompletionObject
                                        {
                                            DatecomPletedString = myViewObj.DatecomPletedString,
                                            WellCompletionId =myViewObj.WellCompletionId,
                                            WellId =  myViewObj.WellId,
                                            EquipmentId =myViewObj.EquipmentId,
                                            WellCompletionTypeId =myViewObj.WellCompletionTypeId,
                                            IsInitial = myViewObj.IsInitial
                                        };

                if (myViewObj.WellCompletionIntervals.Any())
                {
                    var intervals = myViewObj.WellCompletionIntervals.ToList();

                    if (myViewObj.WellCompletionTypeId > 1)
                    {
                        var upperInt = intervals.Max(m => m.UpperInterval);
                        var lowerInt = intervals.Min(m => m.UpperInterval);

                        if (lowerInt > 0)
                        {
                            var interval1 = intervals.Find(m => m.UpperInterval.Equals(lowerInt));
                            if (interval1 != null && interval1.WellCompletionIntervalId > 0)
                            {
                                welllcompObj.L1 = interval1.LowerInterval;
                                welllcompObj.U1 = interval1.UpperInterval;
                                welllcompObj.IntervalId1 = interval1.WellCompletionIntervalId;   
                            }
                        }
                        
                        if (upperInt > 0)
                        {
                            var interval2 = intervals.Find(m => m.UpperInterval.Equals(upperInt));
                            if (interval2 != null)
                            {
                                welllcompObj.L2 = interval2.LowerInterval;
                                welllcompObj.U2 = interval2.UpperInterval;
                                welllcompObj.IntervalId2 = interval2.WellCompletionIntervalId;
                                welllcompObj.DateCompleted = DateTime.Parse(interval2.DateCompleted);
                            }
                           
                        }
                       
                    }

                    if (myViewObj.WellCompletionTypeId < 2)
                    {
                        welllcompObj.L1 = intervals[0].LowerInterval;
                        welllcompObj.U1 = intervals[0].UpperInterval;
                        welllcompObj.IntervalId1 = intervals[0].WellCompletionIntervalId;
                        welllcompObj.DateCompleted = DateTime.Parse(intervals[0].DateCompleted);

                    }
                }

                Session["_wellCompletion"] = myViewObj;
                Session["_wellCompObj"] = welllcompObj;
                welllcompObj.ErrorCode = 5;
                return Json(welllcompObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellCompletion.Error = "An unknown error was encountered. Well Completion  Information could not be retrieved.";
                wellCompletion.ErrorCode = -1;
                return Json(wellCompletion, JsonRequestBehavior.AllowGet);
            }
        }
        private static GenericValidator ValidateControl(WellCompletion model)
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

                if (model.WellCompletionTypeId < 1)
                {
                    gVal.Error = "Please select a valid Well Completion Type";
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
        public int GetLoggedOnUserId()
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser != null)
            {
                return Convert.ToInt32(membershipUser.ProviderUserKey);
            }
            return 0;
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        public ActionResult GetWellCompletionsByMonth(GenericSearch search)
        {
            try
            {
                if (search.Month < 1)
                {
                    ViewBag.ErrorCode = -1;
                    ViewBag.ErrorMessage = "Please provide a valid Report Month";
                    return View("Report", new WellCompletionViewModel
                    {
                        WellCompletionTypes = new List<WellCompletionType>(),
                        Wells = new List<Well>(),
                        Equipments = new List<Equipment>(),
                        WellCompletions = new List<WellCompletion>(),
                        MonthList = GetMonths(),
                        YearList = GetYears()
                    });

                }
                if (search.Year < 1)
                {
                    ViewBag.ErrorCode = -1;
                    ViewBag.ErrorMessage = "Please provide a valid Report Year";
                    return View("Report", new WellCompletionViewModel
                    {
                        WellCompletionTypes = new List<WellCompletionType>(),
                        Wells = new List<Well>(),
                        Equipments = new List<Equipment>(),
                        WellCompletions = new List<WellCompletion>(),
                        MonthList = GetMonths(),
                        YearList = GetYears()
                    });
                }

                var wellCompletions = GetCompletionsBySearchDate(search);
                ViewBag.Title = "Manage Well Workovers";
                if (!wellCompletions.Any())
                {
                    ViewBag.ErrorCode = -1;
                    ViewBag.ErrorMessage = "No record found";

                    return View("WellCompletions", new WellCompletionViewModel
                    {
                        WellCompletionTypes = GetWellCompletionTypes(),
                        WellObjects = GetWells(),
                        Equipments = GetEquipments(),
                        WellCompletions = new List<WellCompletion>(),
                        MonthList = GetMonths(),
                        YearList = GetYears()
                    });
                }


                ViewBag.ErrorCode = 5;
                ViewBag.ErrorMessage = "";

                return View("WellCompletions", new WellCompletionViewModel
                {
                    WellCompletionTypes = GetWellCompletionTypes(),
                    WellObjects = GetWells(),
                    Equipments = GetEquipments(), WellCompletions = wellCompletions,
                    MonthList = GetMonths(),
                    YearList = GetYears()
                });
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                ViewBag.ErrorCode = -1;
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                ViewBag.ErrorMessage = "An error was encountered, the seach operation could not be completed.";
                return View("~/Views/WellCompletion/WellCompletions.cshtml", new WellCompletionViewModel { WellCompletionTypes = GetWellCompletionTypes(),
                                                                                                           WellObjects = GetWells(),
                                                                                                           Equipments = GetEquipments(),
                                                                                                           WellCompletions = new List<WellCompletion>(),
                                                                                                           MonthList = GetMonths(),
                                                                                                           YearList = GetYears()
                });
            }
        }
        public List<WellCompletion> GetCompletionsBySearchDate(GenericSearch search)
        {
            try
            {
                var wellCompletions = new WellCompletionServices().GetAllOrderedCompletionsByMonth(search.Month, search.Year);

                if (!wellCompletions.Any())
                {
                    return new List<WellCompletion>();
                }

                wellCompletions.ForEach(m =>
                {
                    m.EquipmentName = m.Equipment.Name;
                    m.WellName = m.Well.Name;
                    m.WellCompletionTypeName = m.WellCompletionType.Type;
                    m.EquipmentName = m.Equipment.Name;
                });
                return wellCompletions;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellCompletion>();
            }
        }
        public ActionResult GetWellCompletionsByQueryDateDate(GenericSearch search)
        {
            try
            {
               if (search.Year < 1)
                {
                    ViewBag.ErrorCode = -1;
                    ViewBag.ErrorMessage = "Please provide a valid Report Year";
                    return View("Report", new WellCompletionViewModel
                    {
                        WellCompletionTypes = new List<WellCompletionType>(),
                        Wells = new List<Well>(),
                        Equipments = new List<Equipment>(),
                        WellCompletions = new List<WellCompletion>(),
                        WellCompletionReportObjects = new List<WellCompletionReportObject>(),
                        MonthList = GetMonths(),
                        YearList = GetYears()
                    });
                }

               var txd = GetWellCompletionStaticReports(ItemsPerPage, PageNumber, search);
                if (!txd.Any())
                {
                    ViewBag.ErrorMessage = "No record found";
                    ViewBag.ErrorCode = -1;

                    if (search.Month < 1)
                    {
                        ViewBag.SearchPeriod = search.Year;
                    }
                    if (search.Month > 0)
                    {
                        ViewBag.SearchPeriod = Enum.GetName(typeof(MonthList), search.Month) + "/" + search.Year;
                    }
                    
                    return View("Report", new WellCompletionViewModel
                    {
                        WellCompletionTypes = new List<WellCompletionType>(),
                        Wells = new List<Well>(),
                        Equipments = new List<Equipment>(),
                        WellCompletions = new List<WellCompletion>(),
                        WellCompletionReportObjects = new List<WellCompletionReportObject>(),
                        MonthList = GetMonths(),
                        YearList = GetYears()
                    });
                }
                   ViewBag.SearchPeriod = Enum.GetName(typeof(MonthList), search.Month) + "/" + search.Year;
                   var txx = new WellCompletionViewModel
                    {
                        WellCompletionTypes = new List<WellCompletionType>(),
                        Wells = new List<Well>(),
                        Equipments = new List<Equipment>(),
                        WellCompletions = new List<WellCompletion>(),
                        WellCompletionReportObjects = txd,
                        MonthList = GetMonths(),
                        YearList = GetYears()
                    };
                   Session["_wellCompRepoGenSearch"] = search;
                   Session["_wellCompRepoPage"] = 1;
                   return View("Report", txx);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "No record found";
                ViewBag.ErrorCode = -1;
                ViewBag.SearchPeriod = Enum.GetName(typeof(MonthList), search.Month) + "/" + search.Year;
                return View("Report", new WellCompletionViewModel
                {
                    WellCompletionTypes = new List<WellCompletionType>(),
                    Wells = new List<Well>(),
                    Equipments = new List<Equipment>(),
                    WellCompletions = new List<WellCompletion>(),
                    WellCompletionReportObjects = new List<WellCompletionReportObject>(),
                    MonthList = GetMonths(),
                    YearList = GetYears()
                });
            }
        }
        public ActionResult Report()
        {
            return View(new WellCompletionViewModel
                {
                    WellCompletionTypes = new List<WellCompletionType>(),
                    Wells = new List<Well>(),
                    Equipments = new List<Equipment>(),
                    WellCompletions = new List<WellCompletion>(),
                    WellCompletionReportObjects = new List<WellCompletionReportObject>(),
                    MonthList = GetMonths(),
                    YearList = GetYears()
                });
                
            
        }
        private List<WellCompletionReportObject> GetWellCompletionStaticReports(int itemsPerPage, int pageNumber, GenericSearch search)
        {
            try
            {
                var wellCompletions = new WellCompletionServices().GetWellStaticCompletionReport(itemsPerPage, pageNumber, search.Month, search.Year);

                if (!wellCompletions.Any())
                {
                    return new List<WellCompletionReportObject>();
                }

             
                return wellCompletions;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellCompletionReportObject>();
            }
        }

        public ActionResult GetMoreStaticWellCompletionReports()
        {
            int currentPage;

            if (Session["_wellCompRepoGenSearch"] == null)
            {
                return Json(new List<ProductionObject>(), JsonRequestBehavior.AllowGet);
            }

            var search = Session["_wellCompRepoGenSearch"] as GenericSearch;

            if (search == null || search.Year < 1)
            {
                return Json(new List<ProductionObject>(), JsonRequestBehavior.AllowGet);
            }


            var o = Session["_wellCompRepoPage"];
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

            var wellCompletionReportList = GetWellCompletionStaticReports(50, currentPage, search);

            if (!wellCompletionReportList.Any())
            {
                return Json(new List<ProductionObject>(), JsonRequestBehavior.AllowGet);
            }
            Session["_wellCompRepoPage"] = currentPage;
            Session["_wellCompRepoGenSearch"] = search;
            return Json(wellCompletionReportList, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult WellCompletionUpload()
        {
            return View(GetWellCompletionTypes());
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        [HttpPost]
        public ActionResult WellCompletionUpload(int id, HttpPostedFileBase file)
        {
            try
            {
                if (id < 1)
                {
                    ViewBag.ErrorCode = -1;
                    ViewBag.ErrorMessage = "Please select Well Completion Type.";
                    return View(GetWellCompletionTypes());
                }

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
                    var mList = new List<WellCompletion>();
                    var msg = string.Empty;

                    if (id == 1)
                    {
                        if (!new WellCompletionUploadManager().Import(Server.MapPath(path), "wellcompletion", ref mList, ref msg, id))
                        {
                            ViewBag.ErrorCode = -1;
                            ViewBag.ErrorMessage = "Bulk upload Failed! An unknown error was encountered.";
                            return View(GetWellCompletionTypes());
                        }
                    }

                    if (id == 2)
                    {
                        if (!new WellCompletionUploadManager().Import(Server.MapPath(path), "wellcompletion", ref mList, ref msg, id))
                        {
                            ViewBag.ErrorCode = -1;
                            ViewBag.ErrorMessage = "Bulk upload Failed! An unknown error was encountered.";
                            return View(GetWellCompletionTypes());
                        }
                    }


                    if (mList.Any() && msg.Length > 0)
                    {
                        ViewBag.ErrorCode = -1;
                        ViewBag.ErrorMessage = mList.Count + " records were successfully uploaded.<br/>" + msg;
                        return View(GetWellCompletionTypes());
                    }
                    
                    ViewBag.ErrorCode = 5;
                    var tsx = mList.Count + " records were successfully uploaded.";
                    ViewBag.ErrorMessage = tsx;
                    return View(GetWellCompletionTypes());
                }
                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = "Error: content of file could not be accessed. File is empty or corrupted.";
                return View(GetWellCompletionTypes());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = "Error: content of file could not be accessed. File is empty or corrupted.";
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return View(GetWellCompletionTypes());
            }
        }
       
    }
}