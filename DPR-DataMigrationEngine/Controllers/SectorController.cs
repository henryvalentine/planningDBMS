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
    public class SectorController : Controller
    {
        public SectorController()
		{
			 ViewBag.LoadStatus = "0";
		}
        public ViewResult Sectors()
        {

            var sectorList = new SectorServices().GetAllOrderedSectors() ?? new List<Sector>();

            if (!sectorList.Any())
            {
                ViewBag.Title = "Sector Set Up";
                return View(sectorList);
            }
            sectorList.Remove(sectorList.Find(m => m.SectorId == (int)OtherNotAvailable.Not_Available));
            ViewBag.Title = "Manage Sectors";
            return View(sectorList);
        }
        
        [HttpPost]
        public ActionResult AddSector(Sector sector)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    sector.Error = "Please supply all required fields and try again";
                    sector.ErrorCode = -1;
                    return Json(sector, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(sector);

                if (wx.Code < 1)
                {
                    sector.Error = wx.Error;
                    sector.ErrorCode = -1;
                    return Json(sector, JsonRequestBehavior.AllowGet);
                }

                sector.Name = sector.Name;
                var k = new SectorServices().AddSectorCheckDuplicate(sector);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        sector.Error = "Sector already exists";
                        sector.ErrorCode = -3;
                        return Json(sector, JsonRequestBehavior.AllowGet);
                    }

                    sector.Error = "Process Failed! Please contact the Admin or try again later";
                    sector.ErrorCode = 0;
                    return Json(sector, JsonRequestBehavior.AllowGet);
                }

                sector.Error = "Record was added successfully";
                sector.ErrorCode = 1;
                sector.SectorId = k;
                return Json(sector, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                sector.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                sector.ErrorCode = 0;
                return Json(sector, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditSector(Sector sector)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_sector"] == null)
                {
                    sector.Error = "Session has expired";
                    sector.ErrorCode = 0;
                    return Json(sector, JsonRequestBehavior.AllowGet);
                }

                var oldSector = Session["_sector"] as Sector;

                if (oldSector == null || oldSector.SectorId < 1)
                {
                    sector.Error = "Session has expired";
                    sector.ErrorCode = 0;
                    return Json(sector, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    sector.Error = "Please supply all required fields and try again";
                    sector.ErrorCode = -1;
                    return Json(sector, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(sector);

                if (wx.Code < 1)
                {
                    sector.Error = wx.Error;
                    sector.ErrorCode = -1;
                    return Json(sector, JsonRequestBehavior.AllowGet);
                }

                oldSector.Name = sector.Name;

                var k = new SectorServices().UpdateSectorCheckDuplicate(oldSector);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        sector.Error = "Sector already exists";
                        sector.ErrorCode = 0;
                        return Json(sector, JsonRequestBehavior.AllowGet);
                    }

                    sector.Error = "Process Failed! Please contact the Admin or try again later";
                    sector.ErrorCode = 0;
                    return Json(sector, JsonRequestBehavior.AllowGet);
                }
                
                    sector.Error = "Sector Information was successfully updated";
                    sector.ErrorCode = 1;
                    return Json(sector, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                sector.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                sector.ErrorCode = 0;
                return Json(sector, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("DeleteSector")]
        public ActionResult DeleteSector(int id)
        {
            var sector = new Sector();

            try
            {
                if (id < 1)
                {
                    sector.Error = "Invalid Selection";
                    sector.ErrorCode = 0;
                    return Json(sector, JsonRequestBehavior.AllowGet);
                }
                if (new SectorServices().DeleteSectorCheckReferences(id))
                {
                    sector.Error = "Sector Information was successfully deleted.";
                    sector.ErrorCode = 1;
                    return Json(sector, JsonRequestBehavior.AllowGet);
                }

                sector.Error = "Process Failed! Please try again later";
                sector.ErrorCode = 0;
                return Json(sector, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                sector.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                sector.ErrorCode = 0;
                return Json(sector, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditSector(int id)
        {
            var sector = new Sector();
            try
            {
                if (id < 1)
                {
                    sector.Error = "Invalid Selection!";
                    sector.ErrorCode = -1;
                    return Json(sector, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new SectorServices().GetSector(id);

                if (myViewObj == null || myViewObj.SectorId < 1)
                {
                    sector.Error = "Sector Information could not be retrieved.";
                    sector.ErrorCode = -1;
                    return Json(sector, JsonRequestBehavior.AllowGet);
                }
                Session["_sector"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.SectorId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                sector.Error = "An unknown error was Sector Information could not be retrieved.";
                sector.ErrorCode = -1;
                return Json(sector, JsonRequestBehavior.AllowGet);
            }
        }

        private static GenericValidator ValidateControl(Sector model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.Name.Trim()))
                {
                    gVal.Error = "Please enter Sector Name.";
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