using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;

namespace DPR_DataMigrationEngine.Controllers.WellManagement
{
     [CustomAuthorize(Roles = "Admin")]
    public class WellCompletionTypeController : Controller
    {
        public WellCompletionTypeController()
		{
			 ViewBag.LoadStatus = "0";
		}
        public ActionResult WellCompletionTypes()
        {

            var wellCompletionTypeList = new WellCompletionTypeServices().GetAllOrderedWellCompletionTypes() ?? new List<WellCompletionType>();

            if (!wellCompletionTypeList.Any())
            {
                ViewBag.Title = "Well Completion Type SetUp";
                return View(wellCompletionTypeList);
            }
            
            ViewBag.Title = "Manage Well Completion Types";
            return View(wellCompletionTypeList);
        }
        
        [HttpPost]
        public ActionResult AddWellCompletionType(WellCompletionType wellCompletionType)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    wellCompletionType.Error = "Please supply all required fields and try again";
                    wellCompletionType.ErrorCode = -1;
                    return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(wellCompletionType);

                if (wx.Code < 1)
                {
                    wellCompletionType.Error = wx.Error;
                    wellCompletionType.ErrorCode = -1;
                    return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
                }
              
                var k = new WellCompletionTypeServices().AddWellCompletionTypeCheckDuplicate(wellCompletionType);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        wellCompletionType.Error = "Well Completion Type already exists";
                        wellCompletionType.ErrorCode = 0;
                        return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
                    }

                    wellCompletionType.Error = "Process Failed! Please contact the Admin or try again later";
                    wellCompletionType.ErrorCode = 0;
                    return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
                }

                wellCompletionType.Error = "Record was added successfully";
                wellCompletionType.ErrorCode = 1;
                wellCompletionType.WellCompletionTypeId = k;
                return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellCompletionType.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                wellCompletionType.ErrorCode = 0;
                return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditWellCompletionType(WellCompletionType wellCompletionType)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_wellCompletionType"] == null)
                {
                    wellCompletionType.Error = "Session has expired";
                    wellCompletionType.ErrorCode = 0;
                    return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
                }

                var oldWellCompletionType = Session["_wellCompletionType"] as WellCompletionType;

                if (oldWellCompletionType == null || oldWellCompletionType.WellCompletionTypeId < 1)
                {
                    wellCompletionType.Error = "Session has expired";
                    wellCompletionType.ErrorCode = 0;
                    return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    wellCompletionType.Error = "Please supply all required fields and try again";
                    wellCompletionType.ErrorCode = -1;
                    return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(wellCompletionType);

                if (wx.Code < 1)
                {
                    wellCompletionType.Error = wx.Error;
                    wellCompletionType.ErrorCode = -1;
                    return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
                }

                oldWellCompletionType.Type = wellCompletionType.Type;

                var k = new WellCompletionTypeServices().UpdateWellCompletionTypeCheckDuplicate(oldWellCompletionType);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        wellCompletionType.Error = "Well Completion Type already exists";
                        wellCompletionType.ErrorCode = 0;
                        return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
                    }

                    wellCompletionType.Error = "Process Failed! Please contact the Admin or try again later";
                    wellCompletionType.ErrorCode = 0;
                    return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
                }
                
                    wellCompletionType.Error = "Well Completion Type Information was successfully updated";
                    wellCompletionType.ErrorCode = 1;
                    return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellCompletionType.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                wellCompletionType.ErrorCode = 0;
                return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("DeleteWellCompletionType")]
        public ActionResult DeleteWellCompletionType(int id)
        {
            var wellCompletionType = new WellCompletionType();

            try
            {
                if (id < 1)
                {
                    wellCompletionType.Error = "Invalid Selection";
                    wellCompletionType.ErrorCode = 0;
                    return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
                }
                if (new WellCompletionTypeServices().DeleteWellCompletionTypeCheckReferences(id))
                {
                    wellCompletionType.Error = "Well Completion Type Information was successfully deleted.";
                    wellCompletionType.ErrorCode = 1;
                    return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
                }

                wellCompletionType.Error = "Process Failed! Please try again later";
                wellCompletionType.ErrorCode = 0;
                return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellCompletionType.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                wellCompletionType.ErrorCode = 0;
                return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditWellCompletionType(int id)
        {
            var wellCompletionType = new WellCompletionType();
            try
            {
                if (id < 1)
                {
                    wellCompletionType.Error = "Invalid Selection!";
                    wellCompletionType.ErrorCode = -1;
                    return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new WellCompletionTypeServices().GetWellCompletionType(id);

                if (myViewObj == null || myViewObj.WellCompletionTypeId < 1)
                {
                    wellCompletionType.Error = "Well Completion Type Information could not be retrieved.";
                    wellCompletionType.ErrorCode = -1;
                    return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
                }
                Session["_wellCompletionType"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.WellCompletionTypeId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellCompletionType.Error = "An unknown error was Well Completion Type Information could not be retrieved.";
                wellCompletionType.ErrorCode = -1;
                return Json(wellCompletionType, JsonRequestBehavior.AllowGet);
            }
        }

        private static GenericValidator ValidateControl(WellCompletionType model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.Type.Trim()))
                {
                    gVal.Error = "Please enter Well Completion Type Name.";
                    gVal.Code = 0;
                    return gVal;
                }
               
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

        public int GetLoggedOnUserId()
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser != null)
            {
                return Convert.ToInt32(membershipUser.ProviderUserKey);
            }
            return 0;
        }
    }
}