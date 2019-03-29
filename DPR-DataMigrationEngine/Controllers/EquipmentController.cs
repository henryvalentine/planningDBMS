using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DPR_DataMigrationEngine.BulkUploadManagerManager;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;

namespace DPR_DataMigrationEngine.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class EquipmentController : Controller
    {
        public EquipmentController()
		{
			 ViewBag.LoadStatus = "0";
		}
        public ActionResult Equipments()
        {
            var equipmentTypeList = new EquipmentTypeServices().GetAllOrderedEquipmentTypes() ?? new List<EquipmentType>();

            if (!equipmentTypeList.Any())
            {

                return RedirectToAction("EquipmentTypes", "EquipmentType");
            }

            var equipmentList = new EquipmentServices().GetAllOrderedEquipments() ?? new List<Equipment>();

            if (!equipmentList.Any())
            {
                ViewBag.Edit = 1;
                ViewBag.Title = "Equipment Type SetUp";
                return View(Tuple.Create(equipmentTypeList, new List<Equipment>()));
            }
            equipmentList.ForEach(m =>
            {
                m.EquipmentLicenseStatus = m.LicenseStatus ? "Licensed" : "Unlicensed";
            });
           
            var txx = Tuple.Create(equipmentTypeList, equipmentList);
            ViewBag.Title = "Manage Equipments";
            return View(txx);
        }

        [HttpPost]
        public ActionResult AddEquipment(Equipment equipment)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    equipment.Error = "Please supply all required fields and try again";
                    equipment.ErrorCode = -1;
                    return Json(equipment, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(equipment);

                if (wx.Code < 1)
                {
                    equipment.Error = wx.Error;
                    equipment.ErrorCode = -1;
                    return Json(equipment, JsonRequestBehavior.AllowGet);
                }
              
                equipment.EquipmentType = new EquipmentType
                {
                    EquipmentTypeId = equipment.EquipmentTypeId,
                    Name = equipment.EquipmentType.Name
                };
                equipment.EquipmentLicenseStatus = equipment.LicenseStatus ? "Licensed" : "Unlicensed";
                var k = new EquipmentServices().AddEquipmentCheckDuplicate(equipment);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        equipment.Error = "Equipment  already exists";
                        equipment.ErrorCode = -3;
                        return Json(equipment, JsonRequestBehavior.AllowGet);
                    }

                    equipment.Error = "Process Failed! Please contact the Admin or try again later";
                    equipment.ErrorCode = 0;
                    return Json(equipment, JsonRequestBehavior.AllowGet);
                }

                equipment.Error = "Record was added successfully";
                equipment.ErrorCode = 1;
                equipment.EquipmentId = k;
                return Json(equipment, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                equipment.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                equipment.ErrorCode = 0;
                return Json(equipment, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditEquipment(Equipment equipment)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_equipment"] == null)
                {
                    equipment.Error = "Session has expired";
                    equipment.ErrorCode = 0;
                    return Json(equipment, JsonRequestBehavior.AllowGet);
                }

                var oldEquipment = Session["_equipment"] as Equipment;

                if (oldEquipment == null || oldEquipment.EquipmentId < 1)
                {
                    equipment.Error = "Session has expired";
                    equipment.ErrorCode = 0;
                    return Json(equipment, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    equipment.Error = "Please supply all required fields and try again";
                    equipment.ErrorCode = -1;
                    return Json(equipment, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(equipment);

                if (wx.Code < 1)
                {
                    equipment.Error = wx.Error;
                    equipment.ErrorCode = -1;
                    return Json(equipment, JsonRequestBehavior.AllowGet);
                }

                oldEquipment.Name = equipment.Name;
                oldEquipment.EquipmentTypeId = equipment.EquipmentTypeId;
                oldEquipment.LicenseStatus = equipment.LicenseStatus;
                oldEquipment.EquipmentType = new EquipmentType
                {
                    EquipmentTypeId = equipment.EquipmentTypeId,
                    Name = equipment.EquipmentType.Name
                };
                equipment.EquipmentLicenseStatus = equipment.LicenseStatus ? "Licensed" : "Unlicensed";
                var k = new EquipmentServices().UpdateEquipmentCheckDuplicate(oldEquipment);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        equipment.Error = "Equipment  already exists";
                        equipment.ErrorCode = 0;
                        return Json(equipment, JsonRequestBehavior.AllowGet);
                    }

                    equipment.Error = "Process Failed! Please contact the Admin or try again later";
                    equipment.ErrorCode = 0;
                    return Json(equipment, JsonRequestBehavior.AllowGet);
                }
                
                    equipment.Error = "Equipment  Information was successfully updated";
                    equipment.ErrorCode = 1;
                    return Json(equipment, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                equipment.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                equipment.ErrorCode = 0;
                return Json(equipment, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("DeleteEquipment")]
        public ActionResult DeleteEquipment(int id)
        {
            var equipment = new Equipment();

            try
            {
                if (id < 1)
                {
                    equipment.Error = "Invalid Selection";
                    equipment.ErrorCode = 0;
                    return Json(equipment, JsonRequestBehavior.AllowGet);
                }
                if (new EquipmentServices().DeleteEquipmentCheckReferences(id))
                {
                    equipment.Error = "Equipment  Information was successfully deleted.";
                    equipment.ErrorCode = 1;
                    return Json(equipment, JsonRequestBehavior.AllowGet);
                }

                equipment.Error = "Process Failed! Please try again later";
                equipment.ErrorCode = 0;
                return Json(equipment, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                equipment.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                equipment.ErrorCode = 0;
                return Json(equipment, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditEquipment(int id)
        {
            var equipment = new Equipment();
            try
            {
                if (id < 1)
                {
                    equipment.Error = "Invalid Selection!";
                    equipment.ErrorCode = -1;
                    return Json(equipment, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new EquipmentServices().GetEquipment(id);

                if (myViewObj == null || myViewObj.EquipmentId < 1)
                {
                    equipment.Error = "Equipment  Information could not be retrieved.";
                    equipment.ErrorCode = -1;
                    return Json(equipment, JsonRequestBehavior.AllowGet);
                }
                Session["_equipment"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.EquipmentId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                equipment.Error = "An unknown error was Equipment  Information could not be retrieved.";
                equipment.ErrorCode = -1;
                return Json(equipment, JsonRequestBehavior.AllowGet);
            }
        }

        private static GenericValidator ValidateControl(Equipment model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.Name.Trim()))
                {
                    gVal.Error = "Please enter Equipment  Name.";
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

        public ActionResult SaveToFolder2(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    const string folderPath = "~/BulkUploadTemplate";

                    var fileName = file.FileName;
                    var path = folderPath + "/" + fileName;
                    if (System.IO.File.Exists(Server.MapPath(path)))
                    {
                        System.IO.File.Delete(Server.MapPath(path));
                    }

                    file.SaveAs(Server.MapPath(folderPath + "/" + fileName));
                    var mList = new List<Equipment>();
                    var msg = string.Empty;
                    if (!new EquipmentUploadManager().Import(Server.MapPath(path), "equipment", ref mList, ref msg))
                    {
                        ViewBag.ErrorCode = -1;
                        ViewBag.ErrorMessage = "Bulk upload Failed! An unknown error was encountered.";
                        return View("~/Views/Home/MassUploads.cshtml");
                    }

                    if (!mList.Any())
                    {
                        const string error = "None missing in action";
                        ViewBag.ErrorCode = -1;
                        ViewBag.ErrorMessage = error;
                        return View("~/Views/Home/MassUploads.cshtml");
                    }
                    ViewBag.ErrorCode = 1;
                    ViewBag.ErrorMessage = mList.Count + " Equipments successfully uploaded";
                    return View("~/Views/Home/MassUploads.cshtml");
                }
                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = "Error: File is empty or the content could not be accessed";
                return View("~/Views/Home/MassUploads.cshtml");
            }
            catch (Exception ex)
            {
                const string error = "Bulk upload Failed! unknown error occurred";
                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = error;
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return View("~/Views/Home/MassUploads.cshtml");
            }
        }

        public ActionResult Reports()
        {
           
            var equipmentList = new EquipmentServices().GetAllOrderedEquipments() ?? new List<Equipment>();

            if (!equipmentList.Any())
            {
                ViewBag.Edit = 1;
                ViewBag.Title = "Equipment List";
                return View(new List<Equipment>());
            }
            equipmentList.ForEach(m =>
            {
                m.EquipmentLicenseStatus = m.LicenseStatus ? "Licensed" : "Unlicensed";
            });

            ViewBag.Title = "Equipment List";
            return View(equipmentList);
        }
    }
}