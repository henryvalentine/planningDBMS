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
    [CustomAuthorize(Roles = "Super_Admin")]
    public class MilestoneStatusController : Controller
    {
        public MilestoneStatusController()
		{
			 ViewBag.LoadStatus = "0";
		}
        public ViewResult MilestoneStatus()
        {

            var milestoneStatusList = new MilestoneStatusServices().GetAllOrderedMilestoneStatuses() ?? new List<MilestoneStatu>();

            if (!milestoneStatusList.Any())
            {
                ViewBag.Title = "Milestone Status SetUp";
                return View(milestoneStatusList);
            }
            milestoneStatusList.Remove(milestoneStatusList.Find(m => m.MilestoneStatusId == (int)OtherNotAvailable.Not_Available));
            ViewBag.Title = "Manage Milestone Statuses";
            return View(milestoneStatusList);
        }
        
        [HttpPost]
        public ActionResult AddMilestoneStatu(MilestoneStatu milestoneStatus)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    milestoneStatus.Error = "Please supply all required fields and try again";
                    milestoneStatus.ErrorCode = -1;
                    return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(milestoneStatus);

                if (wx.Code < 1)
                {
                    milestoneStatus.Error = wx.Error;
                    milestoneStatus.ErrorCode = -1;
                    return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
                }

                milestoneStatus.Name = milestoneStatus.Name;
                var k = new MilestoneStatusServices().AddMilestoneStatusCheckDuplicate(milestoneStatus);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        milestoneStatus.Error = "Milestone Status already exists";
                        milestoneStatus.ErrorCode = -3;
                        return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
                    }

                    milestoneStatus.Error = "Process Failed! Please contact the Admin or try again later";
                    milestoneStatus.ErrorCode = 0;
                    return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
                }

                milestoneStatus.Error = "Record was successfully added";
                milestoneStatus.ErrorCode = 1;
                milestoneStatus.MilestoneStatusId = k;
                return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                milestoneStatus.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                milestoneStatus.ErrorCode = 0;
                return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditMilestoneStatu(MilestoneStatu milestoneStatus)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_milestoneStatus"] == null)
                {
                    milestoneStatus.Error = "Session has expired";
                    milestoneStatus.ErrorCode = 0;
                    return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
                }

                var oldMilestoneStatu = Session["_milestoneStatus"] as MilestoneStatu;

                if (oldMilestoneStatu == null || oldMilestoneStatu.MilestoneStatusId < 1)
                {
                    milestoneStatus.Error = "Session has expired";
                    milestoneStatus.ErrorCode = 0;
                    return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    milestoneStatus.Error = "Please supply all required fields and try again";
                    milestoneStatus.ErrorCode = -1;
                    return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(milestoneStatus);

                if (wx.Code < 1)
                {
                    milestoneStatus.Error = wx.Error;
                    milestoneStatus.ErrorCode = -1;
                    return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
                }

                oldMilestoneStatu.Name = milestoneStatus.Name;

                var k = new MilestoneStatusServices().UpdateMilestoneStatusCheckDuplicate(oldMilestoneStatu);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        milestoneStatus.Error = "Milestone Status already exists";
                        milestoneStatus.ErrorCode = 0;
                        return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
                    }

                    milestoneStatus.Error = "Process Failed! Please contact the Admin or try again later";
                    milestoneStatus.ErrorCode = 0;
                    return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
                }
                
                    milestoneStatus.Error = "Milestone Status Information was successfully updated";
                    milestoneStatus.ErrorCode = 1;
                    return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                milestoneStatus.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                milestoneStatus.ErrorCode = 0;
                return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("DeleteMilestoneStatu")]
        public ActionResult DeleteMilestoneStatu(int id)
        {
            var milestoneStatus = new MilestoneStatu();

            try
            {
                if (id < 1)
                {
                    milestoneStatus.Error = "Invalid Selection";
                    milestoneStatus.ErrorCode = 0;
                    return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
                }
                if (new MilestoneStatusServices().DeleteMilestoneStatuCheckReferences(id))
                {
                    milestoneStatus.Error = "Milestone Status Information was successfully deleted.";
                    milestoneStatus.ErrorCode = 1;
                    return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
                }

                milestoneStatus.Error = "Process Failed! Please try again later";
                milestoneStatus.ErrorCode = 0;
                return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                milestoneStatus.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                milestoneStatus.ErrorCode = 0;
                return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditMilestoneStatu(int id)
        {
            var milestoneStatus = new MilestoneStatu();
            try
            {
                if (id < 1)
                {
                    milestoneStatus.Error = "Invalid Selection!";
                    milestoneStatus.ErrorCode = -1;
                    return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new MilestoneStatusServices().GetMilestoneStatus(id);

                if (myViewObj == null || myViewObj.MilestoneStatusId < 1)
                {
                    milestoneStatus.Error = "Milestone Status Information could not be retrieved.";
                    milestoneStatus.ErrorCode = -1;
                    return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
                }
                Session["_milestoneStatus"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.MilestoneStatusId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                milestoneStatus.Error = "An unknown error was Milestone Status Information could not be retrieved.";
                milestoneStatus.ErrorCode = -1;
                return Json(milestoneStatus, JsonRequestBehavior.AllowGet);
            }
        }

        private static GenericValidator ValidateControl(MilestoneStatu model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.Name.Trim()))
                {
                    gVal.Error = "Please enter Milestone Status Name.";
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