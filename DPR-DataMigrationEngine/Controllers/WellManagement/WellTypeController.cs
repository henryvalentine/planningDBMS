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
    public class WellTypeController : Controller
    {
        public WellTypeController()
		{
			 ViewBag.LoadStatus = "0";
		}
        public ViewResult WellTypes()
        {

            var wellTypeList = new WellTypeServices().GetAllOrderedWellTypes() ?? new List<WellType>();

            if (!wellTypeList.Any())
            {
                ViewBag.Title = "Well Type SetUp";
                return View(wellTypeList);
            }
            
            ViewBag.Title = "Manage Well Types";
            return View(wellTypeList);
        }
        
        [HttpPost]
        public ActionResult AddWellType(WellType wellType)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    wellType.Error = "Please supply all required fields and try again";
                    wellType.ErrorCode = -1;
                    return Json(wellType, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(wellType);

                if (wx.Code < 1)
                {
                    wellType.Error = wx.Error;
                    wellType.ErrorCode = -1;
                    return Json(wellType, JsonRequestBehavior.AllowGet);
                }

                wellType.Title = wellType.Title;
                var k = new WellTypeServices().AddWellTypeCheckDuplicate(wellType);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        wellType.Error = "Well Type already exists";
                        wellType.ErrorCode = 0;
                        return Json(wellType, JsonRequestBehavior.AllowGet);
                    }

                    wellType.Error = "Process Failed! Please contact the Admin or try again later";
                    wellType.ErrorCode = 0;
                    return Json(wellType, JsonRequestBehavior.AllowGet);
                }

                wellType.Error = "Record was added successfully";
                wellType.ErrorCode = 1;
                wellType.WellTypeId = k;
                return Json(wellType, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellType.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                wellType.ErrorCode = 0;
                return Json(wellType, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditWellType(WellType wellType)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_wellType"] == null)
                {
                    wellType.Error = "Session has expired";
                    wellType.ErrorCode = 0;
                    return Json(wellType, JsonRequestBehavior.AllowGet);
                }

                var oldWellType = Session["_wellType"] as WellType;

                if (oldWellType == null || oldWellType.WellTypeId < 1)
                {
                    wellType.Error = "Session has expired";
                    wellType.ErrorCode = 0;
                    return Json(wellType, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    wellType.Error = "Please supply all required fields and try again";
                    wellType.ErrorCode = -1;
                    return Json(wellType, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(wellType);

                if (wx.Code < 1)
                {
                    wellType.Error = wx.Error;
                    wellType.ErrorCode = -1;
                    return Json(wellType, JsonRequestBehavior.AllowGet);
                }

                oldWellType.Title = wellType.Title;

                var k = new WellTypeServices().UpdateWellTypeCheckDuplicate(oldWellType);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        wellType.Error = "Well Type already exists";
                        wellType.ErrorCode = 0;
                        return Json(wellType, JsonRequestBehavior.AllowGet);
                    }

                    wellType.Error = "Process Failed! Please contact the Admin or try again later";
                    wellType.ErrorCode = 0;
                    return Json(wellType, JsonRequestBehavior.AllowGet);
                }
                
                    wellType.Error = "Well Type Information was successfully updated";
                    wellType.ErrorCode = 1;
                    return Json(wellType, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellType.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                wellType.ErrorCode = 0;
                return Json(wellType, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("DeleteWellType")]
        public ActionResult DeleteWellType(int id)
        {
            var wellType = new WellType();

            try
            {
                if (id < 1)
                {
                    wellType.Error = "Invalid Selection";
                    wellType.ErrorCode = 0;
                    return Json(wellType, JsonRequestBehavior.AllowGet);
                }
                if (new WellTypeServices().DeleteWellTypeCheckReferences(id))
                {
                    wellType.Error = "Well Type Information was successfully deleted.";
                    wellType.ErrorCode = 1;
                    return Json(wellType, JsonRequestBehavior.AllowGet);
                }

                wellType.Error = "Process Failed! Please try again later";
                wellType.ErrorCode = 0;
                return Json(wellType, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellType.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                wellType.ErrorCode = 0;
                return Json(wellType, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditWellType(int id)
        {
            var wellType = new WellType();
            try
            {
                if (id < 1)
                {
                    wellType.Error = "Invalid Selection!";
                    wellType.ErrorCode = -1;
                    return Json(wellType, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new WellTypeServices().GetWellType(id);

                if (myViewObj == null || myViewObj.WellTypeId < 1)
                {
                    wellType.Error = "Well Type Information could not be retrieved.";
                    wellType.ErrorCode = -1;
                    return Json(wellType, JsonRequestBehavior.AllowGet);
                }
                Session["_wellType"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.WellTypeId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellType.Error = "An unknown error was Well Type Information could not be retrieved.";
                wellType.ErrorCode = -1;
                return Json(wellType, JsonRequestBehavior.AllowGet);
            }
        }

        private static GenericValidator ValidateControl(WellType model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.Title.Trim()))
                {
                    gVal.Error = "Please enter Well Type Name.";
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