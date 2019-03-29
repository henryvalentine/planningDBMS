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
    public class LeaseTypeController : Controller
    {
        public LeaseTypeController()
		{
			 ViewBag.LoadStatus = "0";
		}
        public ActionResult LeaseTypes()
        {

            var productList = new LeaseTypeServices().GetAllOrderedLeaseTypes() ?? new List<LeaseType>();

            if (!productList.Any())
            {
                ViewBag.Title = "Lease Type SetUp";
                return View(productList);
            }
            
            ViewBag.Title = "Manage Incident Types";
            return View(productList);
        }
        
        [HttpPost]
        public ActionResult AddLeaseType(LeaseType leaseType)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    leaseType.Error = "Please supply all required fields and try again";
                    leaseType.ErrorCode = -1;
                    return Json(leaseType, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(leaseType);

                if (wx.Code < 1)
                {
                    leaseType.Error = wx.Error;
                    leaseType.ErrorCode = -1;
                    return Json(leaseType, JsonRequestBehavior.AllowGet);
                }

                leaseType.Name = leaseType.Name;
                var k = new LeaseTypeServices().AddLeaseTypeCheckDuplicate(leaseType);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        leaseType.Error = "Lease Type already exists";
                        leaseType.ErrorCode = -3;
                        return Json(leaseType, JsonRequestBehavior.AllowGet);
                    }

                    leaseType.Error = "Process Failed! Please contact the Admin or try again later";
                    leaseType.ErrorCode = 0;
                    return Json(leaseType, JsonRequestBehavior.AllowGet);
                }

                leaseType.Error = "Record was added successfully";
                leaseType.ErrorCode = 1;
                leaseType.LeaseTypeId = k;
                return Json(leaseType, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                leaseType.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                leaseType.ErrorCode = 0;
                return Json(leaseType, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditLeaseType(LeaseType incidentType)
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

                var oldLeaseType = Session["_product"] as LeaseType;

                if (oldLeaseType == null || oldLeaseType.LeaseTypeId < 1)
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

                oldLeaseType.Name = incidentType.Name.Trim();
                oldLeaseType.Description = incidentType.Description.Trim();

                var k = new LeaseTypeServices().UpdateLeaseTypeCheckDuplicate(oldLeaseType);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        incidentType.Error = "Lease Type already exists";
                        incidentType.ErrorCode = 0;
                        return Json(incidentType, JsonRequestBehavior.AllowGet);
                    }

                    incidentType.Error = "Process Failed! Please contact the Admin or try again later";
                    incidentType.ErrorCode = 0;
                    return Json(incidentType, JsonRequestBehavior.AllowGet);
                }
                
                    incidentType.Error = "Lease Type Information was successfully updated";
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
        [ActionName("DeleteLeaseType")]
        public ActionResult DeleteLeaseType(int id)
        {
            var incidentType = new LeaseType();

            try
            {
                if (id < 1)
                {
                    incidentType.Error = "Invalid Selection";
                    incidentType.ErrorCode = 0;
                    return Json(incidentType, JsonRequestBehavior.AllowGet);
                }
                if (new LeaseTypeServices().DeleteLeaseTypeCheckReferences(id))
                {
                    incidentType.Error = "Lease Type Information was successfully deleted.";
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

        public ActionResult EditLeaseType(int id)
        {
            var incidentType = new LeaseType();
            try
            {
                if (id < 1)
                {
                    incidentType.Error = "Invalid Selection!";
                    incidentType.ErrorCode = -1;
                    return Json(incidentType, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new LeaseTypeServices().GetLeaseType(id);

                if (myViewObj == null || myViewObj.LeaseTypeId < 1)
                {
                    incidentType.Error = "Lease Type Information could not be retrieved.";
                    incidentType.ErrorCode = -1;
                    return Json(incidentType, JsonRequestBehavior.AllowGet);
                }
                Session["_product"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.LeaseTypeId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                incidentType.Error = "An unknown error was LeaseType Information could not be retrieved.";
                incidentType.ErrorCode = -1;
                return Json(incidentType, JsonRequestBehavior.AllowGet);
            }
        }

        private static GenericValidator ValidateControl(LeaseType model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.Name.Trim()))
                {
                    gVal.Error = "Please enter Lease Type Name.";
                    gVal.Code = 0;
                    return gVal;
                }
                //if (string.IsNullOrEmpty(model.Description.Trim()))
                //{
                //    gVal.Error = "Please provide Incident Description.";
                //    gVal.Code = 0;
                //    return gVal;
                //}
               
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