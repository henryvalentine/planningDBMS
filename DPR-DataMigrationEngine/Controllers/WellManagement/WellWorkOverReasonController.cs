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
    [CustomAuthorize(Roles = "Admin")]
    public class WellWorkOverReasonController : Controller
    {
        public WellWorkOverReasonController()
		{
			 ViewBag.LoadStatus = "0";
		}
        public ViewResult WellWorkOverReasons()
        {

            var wellWorkOverReasonList = new WellWorkOverReasonServices().GetAllOrderedWellWorkOverReasons() ?? new List<WellWorkOverReason>();

            if (!wellWorkOverReasonList.Any())
            {
                ViewBag.Title = "Well Workover Reason SetUp";
                return View(wellWorkOverReasonList);
            }
            wellWorkOverReasonList.Remove(wellWorkOverReasonList.Find(m => m.WellWorkOverReasonId == (int)OtherNotAvailable.Not_Available));
            ViewBag.Title = "Manage Well Workover Reasons";
            return View(wellWorkOverReasonList);
        }
        
        [HttpPost]
        public ActionResult AddWellWorkOverReason(WellWorkOverReason wellWorkOverReason)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    wellWorkOverReason.Error = "Please supply all required fields and try again";
                    wellWorkOverReason.ErrorCode = -1;
                    return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(wellWorkOverReason);

                if (wx.Code < 1)
                {
                    wellWorkOverReason.Error = wx.Error;
                    wellWorkOverReason.ErrorCode = -1;
                    return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
                }

                var k = new WellWorkOverReasonServices().AddWellWorkOverReason(wellWorkOverReason);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        wellWorkOverReason.Error = "Well Workover Reason already exists";
                        wellWorkOverReason.ErrorCode = -3;
                        return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
                    }

                    wellWorkOverReason.Error = "Process Failed! Please contact the Admin or try again later";
                    wellWorkOverReason.ErrorCode = 0;
                    return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
                }

                wellWorkOverReason.Error = "Record was added successfully";
                wellWorkOverReason.ErrorCode = 1;
                wellWorkOverReason.WellWorkOverReasonId = k;
                return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellWorkOverReason.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                wellWorkOverReason.ErrorCode = 0;
                return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditWellWorkOverReason(WellWorkOverReason wellWorkOverReason)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_wellWorkOverReason"] == null)
                {
                    wellWorkOverReason.Error = "Session has expired";
                    wellWorkOverReason.ErrorCode = 0;
                    return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
                }

                var oldWellWorkOverReason = Session["_wellWorkOverReason"] as WellWorkOverReason;

                if (oldWellWorkOverReason == null || oldWellWorkOverReason.WellWorkOverReasonId < 1)
                {
                    wellWorkOverReason.Error = "Session has expired";
                    wellWorkOverReason.ErrorCode = 0;
                    return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    wellWorkOverReason.Error = "Please supply all required fields and try again";
                    wellWorkOverReason.ErrorCode = -1;
                    return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(wellWorkOverReason);

                if (wx.Code < 1)
                {
                    wellWorkOverReason.Error = wx.Error;
                    wellWorkOverReason.ErrorCode = -1;
                    return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
                }

                oldWellWorkOverReason.Title = wellWorkOverReason.Title;
                oldWellWorkOverReason.Description = wellWorkOverReason.Description;

                var k = new WellWorkOverReasonServices().UpdateWellWorkOverReason(oldWellWorkOverReason);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        wellWorkOverReason.Error = "Well Workover Reason already exists";
                        wellWorkOverReason.ErrorCode = 0;
                        return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
                    }

                    wellWorkOverReason.Error = "Process Failed! Please contact the Admin or try again later";
                    wellWorkOverReason.ErrorCode = 0;
                    return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
                }
                
                    wellWorkOverReason.Error = "Well Workover Reason Information was successfully updated";
                    wellWorkOverReason.ErrorCode = 1;
                    return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellWorkOverReason.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                wellWorkOverReason.ErrorCode = 0;
                return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("DeleteWellWorkOverReason")]
        public ActionResult DeleteWellWorkOverReason(int id)
        {
            var wellWorkOverReason = new WellWorkOverReason();

            try
            {
                if (id < 1)
                {
                    wellWorkOverReason.Error = "Invalid Selection";
                    wellWorkOverReason.ErrorCode = 0;
                    return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
                }
                if (new WellWorkOverReasonServices().DeleteWellWorkOverReasonCheckReferences(id))
                {
                    wellWorkOverReason.Error = "Well Workover Reason Information was successfully deleted.";
                    wellWorkOverReason.ErrorCode = 1;
                    return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
                }

                wellWorkOverReason.Error = "Process Failed! Please try again later";
                wellWorkOverReason.ErrorCode = 0;
                return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellWorkOverReason.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                wellWorkOverReason.ErrorCode = 0;
                return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditWellWorkOverReason(int id)
        {
            var wellWorkOverReason = new WellWorkOverReason();
            try
            {
                if (id < 1)
                {
                    wellWorkOverReason.Error = "Invalid Selection!";
                    wellWorkOverReason.ErrorCode = -1;
                    return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new WellWorkOverReasonServices().GetWellWorkOverReason(id);

                if (myViewObj == null || myViewObj.WellWorkOverReasonId < 1)
                {
                    wellWorkOverReason.Error = "Well Workover Reason Information could not be retrieved.";
                    wellWorkOverReason.ErrorCode = -1;
                    return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
                }
                Session["_wellWorkOverReason"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.WellWorkOverReasonId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellWorkOverReason.Error = "An unknown error was Well Workover Reason Information could not be retrieved.";
                wellWorkOverReason.ErrorCode = -1;
                return Json(wellWorkOverReason, JsonRequestBehavior.AllowGet);
            }
        }

        private static GenericValidator ValidateControl(WellWorkOverReason model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.Title.Trim()))
                {
                    gVal.Error = "Please provide Well Workover Reason.";
                    gVal.Code = 0;
                    return gVal;
                }

                //if (string.IsNullOrEmpty(model.Description.Trim()))
                //{
                //    gVal.Error = "Please provide Description.";
                //    gVal.Code = 0;
                //    return gVal;
                //}
               
                gVal.Code = 1;
                return gVal;
            }
            catch (Exception ex)
            {
                gVal.Error = "Process validation failed. Please supply all required fields and try again.";
                gVal.Code = 0;
                return gVal;
            }
        }
        
    }
}