using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;
using DPR_DataMigrationEngine.ViewModels;

namespace DPR_DataMigrationEngine.Controllers.WellManagement
{
     [CustomAuthorize(Roles = "Admin")]
    public class WellClassificationController : Controller
    {
        public WellClassificationController()
		{
			 ViewBag.LoadStatus = "0";
		}
        public ActionResult WellClassifications()
        {
            var wellClassList = new WellClasServices().GetAllOrderedWellClasses() ?? new List<WellClass>();

            if (!wellClassList.Any())
            {
                ViewBag.Edit = 0;
                ViewBag.Title = "Well Class SetUp";
                return RedirectToAction("WellClasses", "WellClass");
            }

            var wellList = GetWells();

            if (!wellList.Any())
            {
                return RedirectToAction("Wells", "Well");

            }

            var wellClassificationList = new WellClassificationServices().GetAllOrderedWellClassifications() ?? new List<WellClassification>();

            if (!wellClassificationList.Any())
            {
                ViewBag.Edit = 0;
                ViewBag.Title = "Well Classifications";
                var tp = Tuple.Create(wellClassList, wellList, wellClassificationList);
                return View(tp);
            }

            wellClassificationList.ForEach(m =>
            {
                m.WellClassName = m.WellClass.Name;
                m.WellName = m.Well.Name;
            });

            var txx = Tuple.Create(wellClassList, wellList, wellClassificationList);
            ViewBag.Title = "Manage Well Classifications";
            return View(txx);
        }

        private List<WellObject> GetWells()
        {
            var ttg = new WellServices().GetWells(200, 1);
            if (!ttg.Any())
            {
                return new List<WellObject>();
            }
            Session["_normClassWellpageNumber"] = 1;
            return ttg;

        }

        public ActionResult GetMoreWells()
        {
            int pageNumber = 1;

            var o = Session["_normClassWellpageNumber"];
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
            Session["_normClassWellpageNumber"] = pageNumber;
            return Json(dfg, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult AddWellClassification(WellClassification wellClassification)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    wellClassification.Error = "Please supply all required entries and try again";
                    wellClassification.ErrorCode = -1;
                    return Json(wellClassification, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(wellClassification);

                if (wx.Code < 1)
                {
                    wellClassification.Error = wx.Error;
                    wellClassification.ErrorCode = -1;
                    return Json(wellClassification, JsonRequestBehavior.AllowGet);
                }
              
                var k = new WellClassificationServices().AddWellClassificationCheckDuplicate(wellClassification);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        wellClassification.Error = "This Well has already been Classified";
                        wellClassification.ErrorCode = -3;
                        return Json(wellClassification, JsonRequestBehavior.AllowGet);
                    }

                    wellClassification.Error = "Process Failed! Please contact the Admin or try again later";
                    wellClassification.ErrorCode = 0;
                    return Json(wellClassification, JsonRequestBehavior.AllowGet);
                }

                wellClassification.Error = "Well information was successfully Classified";
                wellClassification.ErrorCode = 1;
                wellClassification.WellClassificationId = k;
                return Json(wellClassification, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellClassification.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                wellClassification.ErrorCode = 0;
                return Json(wellClassification, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditWellClassification(WellClassification wellClassification)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_wellClassification"] == null)
                {
                    wellClassification.Error = "Session has expired";
                    wellClassification.ErrorCode = 0;
                    return Json(wellClassification, JsonRequestBehavior.AllowGet);
                }

                var oldWellClassification = Session["_wellClassification"] as WellClassification;

                if (oldWellClassification == null || oldWellClassification.WellClassificationId < 1)
                {
                    wellClassification.Error = "Session has expired";
                    wellClassification.ErrorCode = 0;
                    return Json(wellClassification, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    wellClassification.Error = "Please supply all required wellClassifications and try again";
                    wellClassification.ErrorCode = -1;
                    return Json(wellClassification, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(wellClassification);

                if (wx.Code < 1)
                {
                    wellClassification.Error = wx.Error;
                    wellClassification.ErrorCode = -1;
                    return Json(wellClassification, JsonRequestBehavior.AllowGet);
                }

                oldWellClassification.WellId = wellClassification.WellId;
                oldWellClassification.WellClassId = wellClassification.WellClassId;

                var k = new WellClassificationServices().UpdateWellClassification(oldWellClassification);
                if (k < 1)
                {
                    wellClassification.Error = "Process Failed! Please contact the Admin or try again later";
                    wellClassification.ErrorCode = 0;
                    return Json(wellClassification, JsonRequestBehavior.AllowGet);
                }
                
                    wellClassification.Error = "Well Classification  Information was successfully updated";
                    wellClassification.ErrorCode = 1;
                    return Json(wellClassification, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellClassification.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                wellClassification.ErrorCode = 0;
                return Json(wellClassification, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("DeleteWellClassification")]
        public ActionResult DeleteWellClassification(int id)
        {
            var wellClassification = new WellClassification();

            try
            {
                if (id < 1)
                {
                    wellClassification.Error = "Invalid Selection";
                    wellClassification.ErrorCode = 0;
                    return Json(wellClassification, JsonRequestBehavior.AllowGet);
                }
                if (new WellClassificationServices().DeleteWellClassificationCheckReferences(id))
                {
                    wellClassification.Error = "Well Classification  Information was successfully deleted.";
                    wellClassification.ErrorCode = 1;
                    return Json(wellClassification, JsonRequestBehavior.AllowGet);
                }

                wellClassification.Error = "Process Failed! Please try again later";
                wellClassification.ErrorCode = 0;
                return Json(wellClassification, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellClassification.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                wellClassification.ErrorCode = 0;
                return Json(wellClassification, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditWellClassification(int id)
        {
            var wellClassification = new WellClassification();
            try
            {
                if (id < 1)
                {
                    wellClassification.Error = "Invalid Selection!";
                    wellClassification.ErrorCode = -1;
                    return Json(wellClassification, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new WellClassificationServices().GetWellClassification(id);

                if (myViewObj == null || myViewObj.WellClassificationId < 1)
                {
                    wellClassification.Error = "Well Classification  Information could not be retrieved.";
                    wellClassification.ErrorCode = -1;
                    return Json(wellClassification, JsonRequestBehavior.AllowGet);
                }
                Session["_wellClassification"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.WellClassificationId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                wellClassification.Error = "An unknown error was Well Classification  Information could not be retrieved.";
                wellClassification.ErrorCode = -1;
                return Json(wellClassification, JsonRequestBehavior.AllowGet);
            }
        }

        private static GenericValidator ValidateControl(WellClassification model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (model.WellClassId < 1)
                {
                    gVal.Error = "Please select a valid Well Class";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.WellId < 1)
                {
                    gVal.Error = "Please select a valid Well information";
                    gVal.Code = 0;
                    return gVal;
                }

                gVal.Code = 1;
                return gVal;
            }
            catch (Exception )
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
    }
}