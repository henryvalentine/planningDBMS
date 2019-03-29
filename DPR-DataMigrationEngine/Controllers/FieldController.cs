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
using DPR_DataMigrationEngine.ViewModels;
using Block = DPR_DataMigrationEngine.EF.Models.Block;

namespace DPR_DataMigrationEngine.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class FieldController : Controller
    {
        private const int ItemsPerPage = 50;  
        private const int PageNumber = 1;
        public ActionResult Fields()
        {
            try
            {
                var terrainList = GetTerrains();

                if (!terrainList.Any())
                {
                    return RedirectToAction("Terrains", "Terrain");
                }

                var zoneList = GetZones();

                if (!zoneList.Any())
                {
                    return RedirectToAction("Zones", "Zone");
                }

                var companyList = GetCompanyies();

                if (!companyList.Any())
                {
                    return RedirectToAction("Companies", "Company");
                }

                var blockList = GetBlocks();

                if (!blockList.Any())
                {
                    return RedirectToAction("Blocks", "Block");
                }


                var fieldList = GetFields(ItemsPerPage, PageNumber);

                if (!fieldList.Any())
                {
                    ViewBag.Edit = 1;
                    ViewBag.Title = "Field SetUp";
                    return View(new FieldViewModel { Terrains = terrainList, Zones = zoneList, Companies = companyList, Fields = new List<Field>(), Blocks = blockList });
                }

                ViewBag.Title = "Manage Fields";
                Session["_fieldPage"] = 1;
                return View(new FieldViewModel { Terrains = terrainList, Zones = zoneList, Companies = companyList, Fields = fieldList, Blocks = blockList });
            }
            catch (Exception)
            {
                ViewBag.Edit = 1;
                ViewBag.Title = "Field SetUp";
                ViewBag.Title = "Field SetUp";
                return View(new FieldViewModel { Terrains = GetTerrains(), Zones = GetZones(), Companies = GetCompanyies(), Fields = new List<Field>(), Blocks = GetBlocks() });
            }
        }

        private List<BlockType> GetBlockTypes()
        {
            return new BlockTypeTypeServices().GetAllOrderedBlockTypes() ?? new List<BlockType>();
        }
        private List<Block> GetBlocks()
        {
            return new BlockServices().GetAllOrderedBlocks() ?? new List<Block>();
        }
        private List<Terrain> GetTerrains()
        {
            return new TerrainServices().GetAllOrderedTerrains() ?? new List<Terrain>();
        }
        private List<Company> GetCompanyies()
        {
            return new CompanyServices().GetAllOrderedCompanies() ?? new List<Company>();
        }
        private List<Field> GetFields(int itemsPerPage, int pageNumber)
        {
            var fields = new FieldServices().GetAllOrderedFields(itemsPerPage, pageNumber);
            if (!fields.Any())
            {
                return new List<Field>();
            }
            fields.Remove(fields.Find(m => m.FieldId == (int) OtherNotAvailable.Not_Available));
            return fields;
        }
        private List<Zone> GetZones()
        {
            return new ZoneServices().GetAllOrderedZones() ?? new List<Zone>();
        }
        public ActionResult GetMoreFieldObjects()
        {
            int currentPage;

            var o = Session["_fieldPage"];
            if (o != null)
            {
                var page = (int)o;
                if (page < 1)
                {
                    currentPage = 1;
                }

                else
                {
                    currentPage = page + 1;
                }
            }
            else
            {
                currentPage = 1;
            }

            var wellList = GetMoreFields(50, currentPage);

            if (!wellList.Any())
            {
                return Json(new List<ProductionObject>(), JsonRequestBehavior.AllowGet);
            }
            Session["_fieldPage"] = currentPage;
            return Json(wellList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFieldsByCompany(long companyId) 
        {
            var fieldList = new FieldServices().GetFieldsByCompanyId(companyId);

            if (!fieldList.Any())
            {
                return Json(new List<Field>(), JsonRequestBehavior.AllowGet);
            }
            
            return Json(fieldList, JsonRequestBehavior.AllowGet);
        } 
        private List<FieldReportObject> GetMoreFields(int itemsPerPage, int pageNumber)
        {
            try
            {
                var fields = new FieldServices().GetMoreFields(itemsPerPage, pageNumber);

                if (!fields.Any())
                {

                    return new List<FieldReportObject>();
                }
                fields.Remove(fields.Find(m => m.FieldId == (int)OtherNotAvailable.Not_Available));
                return fields;
            }
            catch (Exception ex)
            {
                return new List<FieldReportObject>();
            }
        }

        [HttpPost]
        public ActionResult AddField(Field field)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    field.Error = "Please supply all required fields and try again";
                    field.ErrorCode = -1;
                    return Json(field, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(field);

                if (wx.Code < 1)
                {
                    field.Error = wx.Error;
                    field.ErrorCode = -1;
                    return Json(field, JsonRequestBehavior.AllowGet);
                }
              
                var k = new FieldServices().AddFieldCheckDuplicate(field);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        field.Error = "Field  already exists";
                        field.ErrorCode = -3;
                        return Json(field, JsonRequestBehavior.AllowGet);
                    }

                    field.Error = "Process Failed! Please contact the Admin or try again later";
                    field.ErrorCode = 0;
                    return Json(field, JsonRequestBehavior.AllowGet);
                }

                field.Error = "Record was added successfully";
                field.ErrorCode = 1;
                field.FieldId = k;
                return Json(field, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                field.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                field.ErrorCode = 0;
                return Json(field, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditField(Field field)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_field"] == null)
                {
                    field.Error = "Session has expired";
                    field.ErrorCode = 0;
                    return Json(field, JsonRequestBehavior.AllowGet);
                }

                var oldField = Session["_field"] as Field;

                if (oldField == null || oldField.FieldId < 1)
                {
                    field.Error = "Session has expired";
                    field.ErrorCode = 0;
                    return Json(field, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    field.Error = "Please supply all required fields and try again";
                    field.ErrorCode = -1;
                    return Json(field, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(field);

                if (wx.Code < 1)
                {
                    field.Error = wx.Error;
                    field.ErrorCode = -1;
                    return Json(field, JsonRequestBehavior.AllowGet);
                }

                oldField.Name = field.Name;
                oldField.TechnicalAllowable = field.TechnicalAllowable;
                oldField.CompanyId = field.CompanyId;
                oldField.BlockId = field.BlockId;
                oldField.TerrainId = field.TerrainId;
                oldField.ZoneId = field.ZoneId;

                var k = new FieldServices().UpdateFieldCheckDuplicate(oldField);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        field.Error = "Field  already exists";
                        field.ErrorCode = 0;
                        return Json(field, JsonRequestBehavior.AllowGet);
                    }

                    field.Error = "Process Failed! Please contact the Admin or try again later";
                    field.ErrorCode = 0;
                    return Json(field, JsonRequestBehavior.AllowGet);
                }
                
                    field.Error = "Field  Information was successfully updated";
                    field.ErrorCode = 1;
                    return Json(field, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                field.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                field.ErrorCode = 0;
                return Json(field, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("DeleteField")]
        public ActionResult DeleteField(int id)
        {
            var field = new Field();

            try
            {
                if (id < 1)
                {
                    field.Error = "Invalid Selection";
                    field.ErrorCode = 0;
                    return Json(field, JsonRequestBehavior.AllowGet);
                }
                if (new FieldServices().DeleteFieldCheckReferences(id))
                {
                    field.Error = "Field  Information was successfully deleted.";
                    field.ErrorCode = 1;
                    return Json(field, JsonRequestBehavior.AllowGet);
                }

                field.Error = "Process Failed! Please try again later";
                field.ErrorCode = 0;
                return Json(field, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                field.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                field.ErrorCode = 0;
                return Json(field, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditField(int id)
        {
            var field = new Field();
            try
            {
                if (id < 1)
                {
                    field.Error = "Invalid Selection!";
                    field.ErrorCode = -1;
                    return Json(field, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new FieldServices().GetField(id);

                if (myViewObj == null || myViewObj.FieldId < 1)
                {
                    field.Error = "Field  Information could not be retrieved.";
                    field.ErrorCode = -1;
                    return Json(field, JsonRequestBehavior.AllowGet);
                }
                Session["_field"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.FieldId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                field.Error = "An unknown error was Field  Information could not be retrieved.";
                field.ErrorCode = -1;
                return Json(field, JsonRequestBehavior.AllowGet);
            }
        }
        private static GenericValidator ValidateControl(Field model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.Name.Trim()))
                {
                    gVal.Error = "Please enter Field  Name.";
                    gVal.Code = 0;
                    return gVal;
                }

                //if (model.TechnicalAllowable < 1)
                //{
                //    gVal.Error = "The Allowed Technical Value is invalid";
                //    gVal.Code = 0;
                //    return gVal;
                //}

                if (model.CompanyId < 1)
                {
                    gVal.Error = "Please select a valid company";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.ZoneId < 1)
                {
                    gVal.Error = "Please select a valid Zone";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.TerrainId < 1)
                {
                    gVal.Error = "Please select a valid Terrain";
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
        public ActionResult FieldUpload()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                return View();
            }
        }
        public ActionResult FieldUpload2()
        {
            ViewBag.ErrorCode = 0;
           return View();
            
        }
        [HttpPost]
        public ActionResult FieldUpload(HttpPostedFileBase file)
        {
            try
            {
                var successList = new List<Field>();
                var errorList = new List<Field>();
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
                    var mList = new List<Field>();
                    var msg = string.Empty;
                    if (!new FieldUploadManager().Import(Server.MapPath(path), "Fields", ref mList, ref msg))
                    {
                        ViewBag.ErrorCode = -1;
                        ViewBag.ErrorMessage = msg;
                        return View();
                    }

                    if (!mList.Any())
                    {
                       ViewBag.ErrorCode = -1;
                        ViewBag.ErrorMessage = msg;
                        return View();
                    }


                    foreach (var fieldInfo in mList)
                    {

                        var fieldId = new FieldServices().AddFieldCheckDuplicate(fieldInfo);

                        if (fieldId < 1)
                        {
                            errorList.Add(fieldInfo);
                        }
                        else
                        {
                            successList.Add(fieldInfo);
                            
                        }
                    }
                    if (errorList.Any() && successList.Any())
                    {
                        var ts = successList.Count + " record(s) were successfully uploaded." +
                            "\n" + errorList.Count + " record(s) could not be uploaded due to duplicates/unknown errors encountered.";
                        ViewBag.ErrorCode = -1;

                        if (msg.Length > 0)
                        {
                            ts += "<br/>" + msg;
                        }
                        ViewBag.ErrorMessage = ts;
                        return View();
                    }

                    if (errorList.Any() && !successList.Any())
                    {
                        var ts = errorList.Count + " record(s) could not be uploaded due to duplicates/unknown errors encountered.";
                        ViewBag.ErrorCode = -1;

                        if (msg.Length > 0)
                        {
                            ts += "<br/>" + msg;
                        }
                        ViewBag.ErrorMessage = ts;
                        return View();
                    }

                    if (!errorList.Any() && successList.Any())
                    {
                        ViewBag.ErrorCode = 5;
                        //ViewBag.ErrorMessage = mList.Count + " records were successfully uploaded.";

                        var tsx = successList.Count + " records were successfully uploaded.";

                        if (msg.Length > 0)
                        {
                            tsx += "<br/>" + msg;
                        }
                        ViewBag.ErrorMessage = tsx;
                        return View();
                    }
                }
                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = "Error: File is empty or the content could not be accessed";
                return View();
            }
            catch (Exception ex)
            {
                const string error = "Bulk upload Failed! unknown error occurred";
                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = error;
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return View();
            }
        }
        [HttpPost]
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
                    var mList = new List<Field>();
                    var msg = string.Empty;
                    if (!new FieldUploadManager2().Import(Server.MapPath(path), "Fields", ref mList, ref msg))
                    {
                        ViewBag.ErrorCode = -1;
                        ViewBag.ErrorMessage = msg;
                        return View("FieldUpload2");
                    }

                    if (!mList.Any())
                    {
                        const string error = "An error was encountered. Bulk upload failed.";
                        ViewBag.ErrorCode = -1;
                        ViewBag.ErrorMessage = error;
                        return View("FieldUpload2");
                    }
                    ViewBag.ErrorCode = 1;

                    var str = "";
                    var errorlist = new List<string>();
                    mList.ForEach(m =>
                    {
                        if (m.FieldId < 1)
                        {
                            errorlist.Add( m.Name );
                        }
                    });

                    if (errorlist.Any())
                    {
                        ViewBag.ErrorMessage = errorlist.Count + " Entries failed to upload";
                        ViewBag.ErrorCode = -1;
                        return View("FieldUpload2");
                    }

                    ViewBag.ErrorMessage = mList.Count + " Entries successfully uploaded.";
                    ViewBag.ErrorCode = 5;
                    return View("FieldUpload2");
                }
                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = "Error: File is empty or the content could not be accessed";
                return View("FieldUpload2");
            }
            catch (Exception ex)
            {
                const string error = "Bulk upload Failed! unknown error occurred";
                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = error;
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return View("FieldUpload2");
            }
        }
    }
}