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
    public class WellClassController : Controller
    {
        public WellClassController()
		{
			 ViewBag.LoadStatus = "0";
		}
        public ViewResult WellClasses()
        {
            var wellClassList = new WellClasServices().GetAllOrderedWellClasses() ?? new List<WellClass>();

            if (!wellClassList.Any())
            {
                ViewBag.Edit = 1;
                ViewBag.Title = "SetUp Well Classes ";
                return View( new List<WellClass>());
            }
          
            ViewBag.Title = "Manage Well Classes";
            return View(wellClassList);
        }

        [HttpPost]
        public ActionResult AddWellClass(WellClass wellClass)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    wellClass.Error = "Please supply all required fields and try again";
                    wellClass.ErrorCode = -1;
                    return Json(wellClass, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(wellClass);

                if (wx.Code < 1)
                {
                    wellClass.Error = wx.Error;
                    wellClass.ErrorCode = -1;
                    return Json(wellClass, JsonRequestBehavior.AllowGet);
                }
              
                var k = new WellClasServices().AddWellClassCheckDuplicate(wellClass);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        wellClass.Error = "Well Class  already exists";
                        wellClass.ErrorCode = -3;
                        return Json(wellClass, JsonRequestBehavior.AllowGet);
                    }

                    wellClass.Error = "Process Failed! Please contact the Admin or try again later";
                    wellClass.ErrorCode = 0;
                    return Json(wellClass, JsonRequestBehavior.AllowGet);
                }

                wellClass.Error = "Record was added successfully";
                wellClass.ErrorCode = 1;
                wellClass.WellClassId = k;
                return Json(wellClass, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellClass.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                wellClass.ErrorCode = 0;
                return Json(wellClass, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditWellClass(WellClass wellClass)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_wellClass"] == null)
                {
                    wellClass.Error = "Session has expired";
                    wellClass.ErrorCode = 0;
                    return Json(wellClass, JsonRequestBehavior.AllowGet);
                }

                var oldWellClass = Session["_wellClass"] as WellClass;

                if (oldWellClass == null || oldWellClass.WellClassId < 1)
                {
                    wellClass.Error = "Session has expired";
                    wellClass.ErrorCode = 0;
                    return Json(wellClass, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    wellClass.Error = "Please supply all required fields and try again";
                    wellClass.ErrorCode = -1;
                    return Json(wellClass, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(wellClass);

                if (wx.Code < 1)
                {
                    wellClass.Error = wx.Error;
                    wellClass.ErrorCode = -1;
                    return Json(wellClass, JsonRequestBehavior.AllowGet);
                }

                oldWellClass.Name = wellClass.Name;
                oldWellClass.Description = wellClass.Description;
                var k = new WellClasServices().UpdateWellClassCheckDuplicate(oldWellClass);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        wellClass.Error = "Well Class  already exists";
                        wellClass.ErrorCode = 0;
                        return Json(wellClass, JsonRequestBehavior.AllowGet);
                    }

                    wellClass.Error = "Process Failed! Please contact the Admin or try again later";
                    wellClass.ErrorCode = 0;
                    return Json(wellClass, JsonRequestBehavior.AllowGet);
                }
                
                    wellClass.Error = "Well Class  Information was successfully updated";
                    wellClass.ErrorCode = 1;
                    return Json(wellClass, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellClass.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                wellClass.ErrorCode = 0;
                return Json(wellClass, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("DeleteWellClass")]
        public ActionResult DeleteWellClass(int id)
        {
            var wellClass = new WellClass();

            try
            {
                if (id < 1)
                {
                    wellClass.Error = "Invalid Selection";
                    wellClass.ErrorCode = 0;
                    return Json(wellClass, JsonRequestBehavior.AllowGet);
                }
                if (new WellClasServices().DeleteWellClassCheckReferences(id))
                {
                    wellClass.Error = "Well Class  Information was successfully deleted.";
                    wellClass.ErrorCode = 1;
                    return Json(wellClass, JsonRequestBehavior.AllowGet);
                }

                wellClass.Error = "Process Failed! Please try again later";
                wellClass.ErrorCode = 0;
                return Json(wellClass, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellClass.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                wellClass.ErrorCode = 0;
                return Json(wellClass, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditWellClass(int id)
        {
            var wellClass = new WellClass();
            try
            {
                if (id < 1)
                {
                    wellClass.Error = "Invalid Selection!";
                    wellClass.ErrorCode = -1;
                    return Json(wellClass, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new WellClasServices().GetWellClass(id);

                if (myViewObj == null || myViewObj.WellClassId < 1)
                {
                    wellClass.Error = "Well Class  Information could not be retrieved.";
                    wellClass.ErrorCode = -1;
                    return Json(wellClass, JsonRequestBehavior.AllowGet);
                }
                Session["_wellClass"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.WellClassId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellClass.Error = "An unknown error was Well Class  Information could not be retrieved.";
                wellClass.ErrorCode = -1;
                return Json(wellClass, JsonRequestBehavior.AllowGet);
            }
        }

        private static GenericValidator ValidateControl(WellClass model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.Name.Trim()))
                {
                    gVal.Error = "Please enter WellClass  Name.";
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