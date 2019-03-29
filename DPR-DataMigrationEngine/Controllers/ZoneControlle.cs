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
    [CustomAuthorize]
    public class ZoneController : Controller
    {
        public ZoneController()
		{
			 ViewBag.LoadStatus = "0";
		}
        public ViewResult Zones()
        {

            var zoneList = new ZoneServices().GetAllOrderedZones() ?? new List<Zone>();

            if (!zoneList.Any())
            {
                ViewBag.Title = "Zone SetUp";
                return View(zoneList);
            }

            zoneList.Remove(zoneList.Find(m => m.ZoneId == (int)OtherNotAvailable.Not_Available));
            
            ViewBag.Title = "Manage Zones";
            return View(zoneList);
        }
        
        [HttpPost]
        public ActionResult AddZone(Zone zone)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    zone.Error = "Please supply all required fields and try again";
                    zone.ErrorCode = -1;
                    return Json(zone, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(zone);

                if (wx.Code < 1)
                {
                    zone.Error = wx.Error;
                    zone.ErrorCode = -1;
                    return Json(zone, JsonRequestBehavior.AllowGet);
                }

                zone.Name = zone.Name;
                var k = new ZoneServices().AddZoneCheckDuplicate(zone);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        zone.Error = "Zone already exists";
                        zone.ErrorCode = 0;
                        return Json(zone, JsonRequestBehavior.AllowGet);
                    }

                    zone.Error = "Process Failed! Please contact the Admin or try again later";
                    zone.ErrorCode = 0;
                    return Json(zone, JsonRequestBehavior.AllowGet);
                }

                zone.Error = "Record was added successfully";
                zone.ErrorCode = 1;
                zone.ZoneId = k;
                return Json(zone, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                zone.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                zone.ErrorCode = 0;
                return Json(zone, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditZone(Zone zone)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_zone"] == null)
                {
                    zone.Error = "Session has expired";
                    zone.ErrorCode = 0;
                    return Json(zone, JsonRequestBehavior.AllowGet);
                }

                var oldZone = Session["_zone"] as Zone;

                if (oldZone == null || oldZone.ZoneId < 1)
                {
                    zone.Error = "Session has expired";
                    zone.ErrorCode = 0;
                    return Json(zone, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    zone.Error = "Please supply all required fields and try again";
                    zone.ErrorCode = -1;
                    return Json(zone, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(zone);

                if (wx.Code < 1)
                {
                    zone.Error = wx.Error;
                    zone.ErrorCode = -1;
                    return Json(zone, JsonRequestBehavior.AllowGet);
                }

                oldZone.Name = zone.Name;

                var k = new ZoneServices().UpdateZoneCheckDuplicate(oldZone);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        zone.Error = "Zone already exists";
                        zone.ErrorCode = 0;
                        return Json(zone, JsonRequestBehavior.AllowGet);
                    }

                    zone.Error = "Process Failed! Please contact the Admin or try again later";
                    zone.ErrorCode = 0;
                    return Json(zone, JsonRequestBehavior.AllowGet);
                }
                
                    zone.Error = "Zone Information was successfully updated";
                    zone.ErrorCode = 1;
                    return Json(zone, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                zone.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                zone.ErrorCode = 0;
                return Json(zone, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("DeleteZone")]
        public ActionResult DeleteZone(int id)
        {
            var zone = new Zone();

            try
            {
                if (id < 1)
                {
                    zone.Error = "Invalid Selection";
                    zone.ErrorCode = 0;
                    return Json(zone, JsonRequestBehavior.AllowGet);
                }
                if (new ZoneServices().DeleteZoneCheckReferences(id))
                {
                    zone.Error = "Zone Information was successfully deleted.";
                    zone.ErrorCode = 1;
                    return Json(zone, JsonRequestBehavior.AllowGet);
                }

                zone.Error = "Process Failed! Please try again later";
                zone.ErrorCode = 0;
                return Json(zone, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                zone.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                zone.ErrorCode = 0;
                return Json(zone, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditZone(int id)
        {
            var zone = new Zone();
            try
            {
                if (id < 1)
                {
                    zone.Error = "Invalid Selection!";
                    zone.ErrorCode = -1;
                    return Json(zone, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new ZoneServices().GetZone(id);

                if (myViewObj == null || myViewObj.ZoneId < 1)
                {
                    zone.Error = "Zone Information could not be retrieved.";
                    zone.ErrorCode = -1;
                    return Json(zone, JsonRequestBehavior.AllowGet);
                }
                Session["_zone"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.ZoneId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                zone.Error = "An unknown error was Zone Information could not be retrieved.";
                zone.ErrorCode = -1;
                return Json(zone, JsonRequestBehavior.AllowGet);
            }
        }

        private static GenericValidator ValidateControl(Zone model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.Name.Trim()))
                {
                    gVal.Error = "Please enter Zone Name.";
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