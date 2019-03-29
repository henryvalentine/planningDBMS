using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;

namespace DPR_DataMigrationEngine.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class IncidentTypeController : Controller
    {
        public IncidentTypeController()
		{
			 ViewBag.LoadStatus = "0";
		}
        public ViewResult IncidentTypes()
        {

            var productList = new IncidentTypeServices().GetAllOrderedIncidentTypes() ?? new List<IncidentType>();

            if (!productList.Any())
            {
                ViewBag.Title = "Incident Type SetUp";
                return View(productList);
            }
            
            ViewBag.Title = "Manage Incident Types";
            return View(productList);
        }
        
        [HttpPost]
        public ActionResult AddIncidentType(IncidentType incidentType)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    incidentType.Error = "Please supply all required fields and try again";
                    incidentType.ErrorCode = -1;
                    return Json(incidentType, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(incidentType);

                if (wx.Code < 1)
                {
                    incidentType.Error = wx.Error;
                    incidentType.ErrorCode = -1;
                    return Json(incidentType, JsonRequestBehavior.AllowGet);
                }

                incidentType.Name = incidentType.Name;
                var k = new IncidentTypeServices().AddIncidentTypeCheckDuplicate(incidentType);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        incidentType.Error = "Incident Type already exists";
                        incidentType.ErrorCode = -3;
                        return Json(incidentType, JsonRequestBehavior.AllowGet);
                    }

                    incidentType.Error = "Process Failed! Please contact the Admin or try again later";
                    incidentType.ErrorCode = 0;
                    return Json(incidentType, JsonRequestBehavior.AllowGet);
                }

                incidentType.Error = "Record was added successfully";
                incidentType.ErrorCode = 1;
                incidentType.IncidentTypeId = k;
                return Json(incidentType, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                incidentType.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                incidentType.ErrorCode = 0;
                return Json(incidentType, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditIncidentType(IncidentType incidentType)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_product"] == null)
                {
                    incidentType.Error = "Session has expired";
                    incidentType.ErrorCode = 0;
                    return Json(incidentType, JsonRequestBehavior.AllowGet);
                }

                var oldIncidentType = Session["_product"] as IncidentType;

                if (oldIncidentType == null || oldIncidentType.IncidentTypeId < 1)
                {
                    incidentType.Error = "Session has expired";
                    incidentType.ErrorCode = 0;
                    return Json(incidentType, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    incidentType.Error = "Please supply all required fields and try again";
                    incidentType.ErrorCode = -1;
                    return Json(incidentType, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(incidentType);

                if (wx.Code < 1)
                {
                    incidentType.Error = wx.Error;
                    incidentType.ErrorCode = -1;
                    return Json(incidentType, JsonRequestBehavior.AllowGet);
                }

                oldIncidentType.Name = incidentType.Name;
                oldIncidentType.Description = incidentType.Description;

                var k = new IncidentTypeServices().UpdateIncidentTypeCheckDuplicate(oldIncidentType);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        incidentType.Error = "Incident Type already exists";
                        incidentType.ErrorCode = 0;
                        return Json(incidentType, JsonRequestBehavior.AllowGet);
                    }

                    incidentType.Error = "Process Failed! Please contact the Admin or try again later";
                    incidentType.ErrorCode = 0;
                    return Json(incidentType, JsonRequestBehavior.AllowGet);
                }
                
                    incidentType.Error = "Incident Type Information was successfully updated";
                    incidentType.ErrorCode = 1;
                    return Json(incidentType, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                incidentType.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                incidentType.ErrorCode = 0;
                return Json(incidentType, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("DeleteIncidentType")]
        public ActionResult DeleteIncidentType(int id)
        {
            var incidentType = new IncidentType();

            try
            {
                if (id < 1)
                {
                    incidentType.Error = "Invalid Selection";
                    incidentType.ErrorCode = 0;
                    return Json(incidentType, JsonRequestBehavior.AllowGet);
                }
                if (new IncidentTypeServices().DeleteIncidentTypeCheckReferences(id))
                {
                    incidentType.Error = "Incident Type Information was successfully deleted.";
                    incidentType.ErrorCode = 1;
                    return Json(incidentType, JsonRequestBehavior.AllowGet);
                }

                incidentType.Error = "Process Failed! Please try again later";
                incidentType.ErrorCode = 0;
                return Json(incidentType, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                incidentType.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                incidentType.ErrorCode = 0;
                return Json(incidentType, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditIncidentType(int id)
        {
            var incidentType = new IncidentType();
            try
            {
                if (id < 1)
                {
                    incidentType.Error = "Invalid Selection!";
                    incidentType.ErrorCode = -1;
                    return Json(incidentType, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new IncidentTypeServices().GetIncidentType(id);

                if (myViewObj == null || myViewObj.IncidentTypeId < 1)
                {
                    incidentType.Error = "Incident Type Information could not be retrieved.";
                    incidentType.ErrorCode = -1;
                    return Json(incidentType, JsonRequestBehavior.AllowGet);
                }
                Session["_product"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.IncidentTypeId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                incidentType.Error = "An unknown error was IncidentType Information could not be retrieved.";
                incidentType.ErrorCode = -1;
                return Json(incidentType, JsonRequestBehavior.AllowGet);
            }
        }

        private static GenericValidator ValidateControl(IncidentType model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.Name.Trim()))
                {
                    gVal.Error = "Please enter Incident Type Name.";
                    gVal.Code = 0;
                    return gVal;
                }
                if (string.IsNullOrEmpty(model.Description.Trim()))
                {
                    gVal.Error = "Please provide Incident Description.";
                    gVal.Code = 0;
                    return gVal;
                }
               
                gVal.Code = 1;
                return gVal;
            }
            catch (Exception )
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