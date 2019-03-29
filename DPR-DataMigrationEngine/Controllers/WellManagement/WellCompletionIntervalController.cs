using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;

namespace DPR_DataMigrationEngine.Controllers.WellManagement
{
    [CustomAuthorize]
    public class WellCompletionIntervalController : Controller
    {
        public WellCompletionIntervalController()
		{
			 ViewBag.LoadStatus = "0";
		}
        public ViewResult WellCompletionIntervals()
        {

            var wellCompletionList = new WellCompletionServices().GetAllOrderedWellCompletions() ?? new List<WellCompletion>();

            if (!wellCompletionList.Any())
            {
                ViewBag.Edit = 1;
                ViewBag.Title = "Well Completion SetUp";
                var wellCompletionTypeList = new WellCompletionTypeServices().GetAllOrderedWellCompletionTypes() ?? new List<WellCompletionType>();

                if (!wellCompletionTypeList.Any())
                {
                    ViewBag.Edit = 0;
                    ViewBag.Title = "Well Completion Type SetUp";
                    return View("~/Views/WellCompletionType/WellCompletionTypes.cshtml", new List<WellCompletionType>());
                }
                
                
            }

            wellCompletionList.ForEach(m =>
            {
                m.WellName = m.Well.Name;
                m.WellCompletionTypeName = m.WellCompletionType.Type;
                m.EquipmentName = m.Equipment.Name;
            });

            var wellCompletionIntervalList = new WellCompletionIntervalServices().GetAllOrderedWellCompletionIntervals() ?? new List<WellCompletionInterval>();

            if (!wellCompletionIntervalList.Any())
            {
                ViewBag.Edit = 0;
                ViewBag.Title = "Well Completion Interval SetUp";
                return View(Tuple.Create(wellCompletionList, new List<WellCompletionInterval>()));
            }

            wellCompletionIntervalList.ForEach(m =>
            {
                m.DatecomPletedString = m.DateCompleted;
                m.WellName =wellCompletionList.Find(x => x.WellId == m.WellCompletion.WellId).Well.Name;
            });

            var txx = Tuple.Create(wellCompletionList, wellCompletionIntervalList);
            ViewBag.Title = "Manage Well Completion Intervals";
            return View(txx);
        }

        [HttpPost]
        public ActionResult AddWellCompletionInterval(WellCompletionInterval wellCompletionInterval)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    wellCompletionInterval.Error = "Please supply all required wellCompletionIntervals and try again";
                    wellCompletionInterval.ErrorCode = -1;
                    return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(wellCompletionInterval);

                if (wx.Code < 1)
                {
                    wellCompletionInterval.Error = wx.Error;
                    wellCompletionInterval.ErrorCode = -1;
                    return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
                }

                //TimeSpan ts = new TimeSpan(DateTime.Now.TimeOfDay);
                wellCompletionInterval.LastUpdatedTime = DateTime.Now.ToString("hh:mm:ss t z");
              
                var k = new WellCompletionIntervalServices().AddWellCompletionIntervalCheckDuplicate(wellCompletionInterval);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        wellCompletionInterval.Error = "WellCompletionInterval  already exists";
                        wellCompletionInterval.ErrorCode = -3;
                        return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
                    }

                    wellCompletionInterval.Error = "Process Failed! Please contact the Admin or try again later";
                    wellCompletionInterval.ErrorCode = 0;
                    return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
                }
                wellCompletionInterval.DatecomPletedString = wellCompletionInterval.DateCompleted;
                wellCompletionInterval.Error = "Record was added successfully";
                wellCompletionInterval.ErrorCode = 1;
                wellCompletionInterval.WellCompletionIntervalId = k;
                return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellCompletionInterval.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                wellCompletionInterval.ErrorCode = 0;
                return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditWellCompletionInterval(WellCompletionInterval wellCompletionInterval)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_wellCompletionInterval"] == null)
                {
                    wellCompletionInterval.Error = "Session has expired";
                    wellCompletionInterval.ErrorCode = 0;
                    return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
                }

                var oldWellCompletionInterval = Session["_wellCompletionInterval"] as WellCompletionInterval;

                if (oldWellCompletionInterval == null || oldWellCompletionInterval.WellCompletionIntervalId < 1)
                {
                    wellCompletionInterval.Error = "Session has expired";
                    wellCompletionInterval.ErrorCode = 0;
                    return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    wellCompletionInterval.Error = "Please supply all required entries and try again";
                    wellCompletionInterval.ErrorCode = -1;
                    return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(wellCompletionInterval);

                if (wx.Code < 1)
                {
                    wellCompletionInterval.Error = wx.Error;
                    wellCompletionInterval.ErrorCode = -1;
                    return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
                }
                
                oldWellCompletionInterval.WellCompletionId = wellCompletionInterval.WellCompletionId;
                oldWellCompletionInterval.UpperInterval = wellCompletionInterval.UpperInterval;
                oldWellCompletionInterval.LowerInterval = wellCompletionInterval.LowerInterval;
                oldWellCompletionInterval.LastUpdatedTime = DateTime.Now.ToString("hh:mm:ss t ");
                wellCompletionInterval.DateCompleted = wellCompletionInterval.DateCompleted;

                var k = new WellCompletionIntervalServices().UpdateWellCompletionIntervalCheckDuplicate(oldWellCompletionInterval);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        wellCompletionInterval.Error = "Well Completion Interval  already exists";
                        wellCompletionInterval.ErrorCode = 0;
                        return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
                    }

                    wellCompletionInterval.Error = "Process Failed! Please contact the Admin or try again later";
                    wellCompletionInterval.ErrorCode = 0;
                    return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
                }

                    wellCompletionInterval.DatecomPletedString = oldWellCompletionInterval.DateCompleted;
                    wellCompletionInterval.Error = "Well Completion Interval  Information was successfully updated";
                    wellCompletionInterval.ErrorCode = 1;
                    return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellCompletionInterval.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                wellCompletionInterval.ErrorCode = 0;
                return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [ActionName("DeleteWellCompletionInterval")]
        public ActionResult DeleteWellCompletionInterval(int id)
        {
            var wellCompletionInterval = new WellCompletionInterval();

            try
            {
                if (id < 1)
                {
                    wellCompletionInterval.Error = "Invalid Selection";
                    wellCompletionInterval.ErrorCode = 0;
                    return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
                }
                if (new WellCompletionIntervalServices().DeleteWellCompletionIntervalCheckReferences(id))
                {
                    wellCompletionInterval.Error = "WellCompletionInterval  Information was successfully deleted.";
                    wellCompletionInterval.ErrorCode = 1;
                    return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
                }

                wellCompletionInterval.Error = "Process Failed! Please try again later";
                wellCompletionInterval.ErrorCode = 0;
                return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellCompletionInterval.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                wellCompletionInterval.ErrorCode = 0;
                return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult EditWellCompletionInterval(int id)
        {
            var wellCompletionInterval = new WellCompletionInterval();
            try
            {
                if (id < 1)
                {
                    wellCompletionInterval.Error = "Invalid Selection!";
                    wellCompletionInterval.ErrorCode = -1;
                    return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new WellCompletionIntervalServices().GetWellCompletionInterval(id);

                if (myViewObj == null || myViewObj.WellCompletionIntervalId < 1)
                {
                    wellCompletionInterval.Error = "Well Completion Interval  Information could not be retrieved.";
                    wellCompletionInterval.ErrorCode = -1;
                    return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
                }
                Session["_wellCompletionInterval"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.WellCompletionIntervalId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellCompletionInterval.Error = "An unknown error was encountered. Well Completion Interval  Information could not be retrieved.";
                wellCompletionInterval.ErrorCode = -1;
                return Json(wellCompletionInterval, JsonRequestBehavior.AllowGet);
            }
        }
        private static GenericValidator ValidateControl(WellCompletionInterval model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (model.WellCompletionId < 1)
                {
                    gVal.Error = "Please select a valid Well Completion information";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.UpperInterval <= 0 )
                {
                    gVal.Error = "Please provide a valid Upper Interval";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.UpperInterval < model.LowerInterval)
                {
                    gVal.Error = "The Lower Interval should not be higher than the Upper Interval";
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
       
    }
}