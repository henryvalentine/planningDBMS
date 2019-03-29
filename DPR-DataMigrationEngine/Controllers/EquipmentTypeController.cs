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
    public class EquipmentTypeController : Controller
    {
        public EquipmentTypeController()
		{
			 ViewBag.LoadStatus = "0";
		}
        public ViewResult EquipmentTypes()
        {

            var equipmentTypeList = new EquipmentTypeServices().GetAllOrderedEquipmentTypes() ?? new List<EquipmentType>();

            if (!equipmentTypeList.Any())
            {
                ViewBag.Title = "Equipment Type SetUp";
                return View(equipmentTypeList);
            }
            
            ViewBag.Title = "Manage Equipment Types";
            return View(equipmentTypeList);
        }
        
        [HttpPost]
        public ActionResult AddEquipmentType(EquipmentType equipmentType)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    equipmentType.Error = "Please supply all required fields and try again";
                    equipmentType.ErrorCode = -1;
                    return Json(equipmentType, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(equipmentType);

                if (wx.Code < 1)
                {
                    equipmentType.Error = wx.Error;
                    equipmentType.ErrorCode = -1;
                    return Json(equipmentType, JsonRequestBehavior.AllowGet);
                }

                equipmentType.Name = equipmentType.Name;
                var k = new EquipmentTypeServices().AddEquipmentTypeCheckDuplicate(equipmentType);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        equipmentType.Error = "Equipment Type already exists";
                        equipmentType.ErrorCode = 0;
                        return Json(equipmentType, JsonRequestBehavior.AllowGet);
                    }

                    equipmentType.Error = "Process Failed! Please contact the Admin or try again later";
                    equipmentType.ErrorCode = 0;
                    return Json(equipmentType, JsonRequestBehavior.AllowGet);
                }

                equipmentType.Error = "Record was added successfully";
                equipmentType.ErrorCode = 1;
                equipmentType.EquipmentTypeId = k;
                return Json(equipmentType, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                equipmentType.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                equipmentType.ErrorCode = 0;
                return Json(equipmentType, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditEquipmentType(EquipmentType equipmentType)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_equipmentType"] == null)
                {
                    equipmentType.Error = "Session has expired";
                    equipmentType.ErrorCode = 0;
                    return Json(equipmentType, JsonRequestBehavior.AllowGet);
                }

                var oldEquipmentType = Session["_equipmentType"] as EquipmentType;

                if (oldEquipmentType == null || oldEquipmentType.EquipmentTypeId < 1)
                {
                    equipmentType.Error = "Session has expired";
                    equipmentType.ErrorCode = 0;
                    return Json(equipmentType, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    equipmentType.Error = "Please supply all required fields and try again";
                    equipmentType.ErrorCode = -1;
                    return Json(equipmentType, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(equipmentType);

                if (wx.Code < 1)
                {
                    equipmentType.Error = wx.Error;
                    equipmentType.ErrorCode = -1;
                    return Json(equipmentType, JsonRequestBehavior.AllowGet);
                }

                oldEquipmentType.Name = equipmentType.Name;

                var k = new EquipmentTypeServices().UpdateEquipmentTypeCheckDuplicate(oldEquipmentType);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        equipmentType.Error = "Equipment Type already exists";
                        equipmentType.ErrorCode = 0;
                        return Json(equipmentType, JsonRequestBehavior.AllowGet);
                    }

                    equipmentType.Error = "Process Failed! Please contact the Admin or try again later";
                    equipmentType.ErrorCode = 0;
                    return Json(equipmentType, JsonRequestBehavior.AllowGet);
                }
                
                    equipmentType.Error = "Equipment Type Information was successfully updated";
                    equipmentType.ErrorCode = 1;
                    return Json(equipmentType, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                equipmentType.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                equipmentType.ErrorCode = 0;
                return Json(equipmentType, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("DeleteEquipmentType")]
        public ActionResult DeleteEquipmentType(int id)
        {
            var equipmentType = new EquipmentType();

            try
            {
                if (id < 1)
                {
                    equipmentType.Error = "Invalid Selection";
                    equipmentType.ErrorCode = 0;
                    return Json(equipmentType, JsonRequestBehavior.AllowGet);
                }
                if (new EquipmentTypeServices().DeleteEquipmentTypeCheckReferences(id))
                {
                    equipmentType.Error = "Equipment Type Information was successfully deleted.";
                    equipmentType.ErrorCode = 1;
                    return Json(equipmentType, JsonRequestBehavior.AllowGet);
                }

                equipmentType.Error = "Process Failed! Please try again later";
                equipmentType.ErrorCode = 0;
                return Json(equipmentType, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                equipmentType.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                equipmentType.ErrorCode = 0;
                return Json(equipmentType, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditEquipmentType(int id)
        {
            var equipmentType = new EquipmentType();
            try
            {
                if (id < 1)
                {
                    equipmentType.Error = "Invalid Selection!";
                    equipmentType.ErrorCode = -1;
                    return Json(equipmentType, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new EquipmentTypeServices().GetEquipmentType(id);

                if (myViewObj == null || myViewObj.EquipmentTypeId < 1)
                {
                    equipmentType.Error = "Equipment Type Information could not be retrieved.";
                    equipmentType.ErrorCode = -1;
                    return Json(equipmentType, JsonRequestBehavior.AllowGet);
                }
                Session["_equipmentType"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.EquipmentTypeId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                equipmentType.Error = "An unknown error was Equipment Type Information could not be retrieved.";
                equipmentType.ErrorCode = -1;
                return Json(equipmentType, JsonRequestBehavior.AllowGet);
            }
        }

        private static GenericValidator ValidateControl(EquipmentType model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.Name.Trim()))
                {
                    gVal.Error = "Please enter Equipment Type Name.";
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