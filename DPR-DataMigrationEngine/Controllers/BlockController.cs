using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DPR_DataMigrationEngine.BulkUploadManagerManager;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.Services.ServiceManager;
using DPR_DataMigrationEngine.ViewModels;
using Block = DPR_DataMigrationEngine.EF.Models.Block;

namespace DPR_DataMigrationEngine.Controllers
{
   [CustomAuthorize(Roles = "Admin,Super_Admin")]
    public class BlockController : Controller
    {
        public BlockController()
		{
			 ViewBag.LoadStatus = "0";
		}

       public ActionResult Blocks()
        {
            try
            {
                var leaseType = GetLeaseTypes();

                if (!leaseType.Any())
                {
                    return RedirectToAction("LeaseTypes", "LeaseType");
                }
              

                var companies = GetCompanies();

                if (!companies.Any())
                {
                    return RedirectToAction("Companies", "Company");
                }

                var blockTypes = GetBlockTypes();

                var blockList = GetBlocks();

                if (!blockList.Any())
                {
                    ViewBag.Edit = 1;
                    ViewBag.Title = "Block SetUp";
                    return View(new BlockViewModel
                    {
                        BlockTypes = blockTypes,
                        Blocks = new List<Block>(),
                        LeaseTypes = leaseType,
                        Companies = companies
                    });
                }
              
                ViewBag.Title = "Manage blocks";
                return View(new BlockViewModel { BlockTypes = blockTypes, Blocks = blockList, LeaseTypes = leaseType, Companies = GetCompanies()});
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = "Block list could not be retrieved. Please try again later.";
                ViewBag.Title = "Block SetUp";
                return View(new BlockViewModel { BlockTypes = GetBlockTypes(), Blocks = new List<Block>(), LeaseTypes = GetLeaseTypes(), Companies = GetCompanies() });
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

        private List<LeaseType> GetLeaseTypes()
        {
            return new LeaseTypeServices().GetAllOrderedLeaseTypes() ?? new List<LeaseType>();
        }

        private List<Field> GetFields()
        {
            return new FieldServices().GetFields() ?? new List<Field>();
        }

        private List<Company> GetCompanies()
        {
            return new CompanyServices().GetAllOrderedCompanies() ?? new List<Company>();
        }

        [HttpPost]
        public ActionResult AddBlock(Block block)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    block.Error = "Please supply all required fields and try again";
                    block.ErrorCode = -1;
                    return Json(block, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(block);

                if (wx.Code < 1)
                {
                    block.Error = wx.Error;
                    block.ErrorCode = -1;
                    return Json(block, JsonRequestBehavior.AllowGet);
                }
               
                var k = new BlockServices().AddBlockCheckDuplicate(block);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        block.Error = "Block  already exists";
                        block.ErrorCode = -3;
                        return Json(block, JsonRequestBehavior.AllowGet);
                    }

                    block.Error = "Process Failed! Please contact the Admin or try again later";
                    block.ErrorCode = 0;
                    return Json(block, JsonRequestBehavior.AllowGet);
                }

                block.Error = "Record was added successfully";
                block.ErrorCode = 1;
                block.BlockId = k;
                return Json(block, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                //ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                block.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                block.ErrorCode = 0;
                return Json(block, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditBlock(Block block)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_block"] == null)
                {
                    block.Error = "Session has expired";
                    block.ErrorCode = 0;
                    return Json(block, JsonRequestBehavior.AllowGet);
                }

                var oldBlock = Session["_block"] as Block;

                if (oldBlock == null || oldBlock.BlockId < 1)
                {
                    block.Error = "Session has expired";
                    block.ErrorCode = 0;
                    return Json(block, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    block.Error = "Please supply all required fields and try again";
                    block.ErrorCode = -1;
                    return Json(block, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(block);

                if (wx.Code < 1)
                {
                    block.Error = wx.Error;
                    block.ErrorCode = -1;
                    return Json(block, JsonRequestBehavior.AllowGet);
                }

                oldBlock.Name = block.Name;
                oldBlock.CompanyId = block.CompanyId;
                oldBlock.LeaseTypeId = block.LeaseTypeId;
                oldBlock.YearOfAward = block.YearOfAward;
                oldBlock.Area = block.Area;
                oldBlock.BlockTypeId = block.BlockTypeId;
                
                var k = new BlockServices().UpdateBlockCheckDuplicate(oldBlock);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        block.Error = "Block  already exists";
                        block.ErrorCode = 0;
                        return Json(block, JsonRequestBehavior.AllowGet);
                    }

                    block.Error = "Process Failed! Please contact the Admin or try again later";
                    block.ErrorCode = 0;
                    return Json(block, JsonRequestBehavior.AllowGet);
                }
                
                    block.Error = "Block  Information was successfully updated";
                    block.ErrorCode = 1;
                    return Json(block, JsonRequestBehavior.AllowGet);
            }
            catch (Exception )
            {
                //ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                block.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                block.ErrorCode = 0;
                return Json(block, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("DeleteBlock")]
        public ActionResult DeleteBlock(int id)
        {
            var block = new Block();

            try
            {
                if (id < 1)
                {
                    block.Error = "Invalid Selection";
                    block.ErrorCode = 0;
                    return Json(block, JsonRequestBehavior.AllowGet);
                }
                if (new BlockServices().DeleteBlockCheckReferences(id))
                {
                    block.Error = "Block  Information was successfully deleted.";
                    block.ErrorCode = 1;
                    return Json(block, JsonRequestBehavior.AllowGet);
                }

                block.Error = "Process Failed! Please try again later";
                block.ErrorCode = 0;
                return Json(block, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                //ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                block.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                block.ErrorCode = 0;
                return Json(block, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditBlock(int id)
        {
            var block = new Block();
            try
            {
                if (id < 1)
                {
                    block.Error = "Invalid Selection!";
                    block.ErrorCode = -1;
                    return Json(block, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new BlockServices().GetBlock(id);

                if (myViewObj == null || myViewObj.BlockId < 1)
                {
                    block.Error = "Block  Information could not be retrieved.";
                    block.ErrorCode = -1;
                    return Json(block, JsonRequestBehavior.AllowGet);
                }
                Session["_block"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.BlockId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                //ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                block.Error = "An unknown error was Block  Information could not be retrieved.";
                block.ErrorCode = -1;
                return Json(block, JsonRequestBehavior.AllowGet);
            }
        }
        private static GenericValidator ValidateControl(Block model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.Name.Trim()))
                {
                    gVal.Error = "Please enter Block  Name.";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.CompanyId < 1)
                {
                    gVal.Error = "Please select a valid Company.";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.BlockTypeId < 1)
                {
                    gVal.Error = "Please select a valid Block Type.";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.LeaseTypeId < 1)
                {
                    gVal.Error = "Please select a valid Lease Type.";
                    gVal.Code = 0;
                    return gVal;
                }
               
                gVal.Code = 1;
                return gVal;
            }
            catch (Exception)
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

        public ActionResult BlockUpload()
        {
           return View();
           
        }

        [HttpPost]
        public ActionResult BlockUpload(HttpPostedFileBase file)
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
                    var mList = new List<Block>();
                    var msg = string.Empty;
                    if (!new BlockUploadManager().Import(Server.MapPath(path), "Blocks", ref mList, ref msg))
                    {
                        ViewBag.ErrorCode = -1;
                        ViewBag.ErrorMessage = msg;
                        return View();
                    }

                    if (!mList.Any())
                    {
                        const string error = "Bulk upload Failed! unknown error occurred";
                        ViewBag.ErrorCode = -1;
                        ViewBag.ErrorMessage = error;
                        return View();
                    }

                    var errorList = new List<Block>();
                    var successList = new List<Block>();
                    mList.ForEach(m =>
                    {
                        var processedItem = new BlockServices().AddBlockCheckDuplicate(m);
                        if (processedItem < 1)
                        {
                            errorList.Add(m);
                        }
                        else
                        {
                            successList.Add(m);
                        }
                    });
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
                        return View(new WellViewModel { Blocks = new List<Block>(), Fields = GetFields() });
                    }

                    if (errorList.Any() && !successList.Any())
                    {
                        var ts = errorList.Count + " records could not be uploaded due to duplicates/unknown errors encountered after processing the template.";
                        ViewBag.ErrorCode = -1;

                        if (msg.Length > 0)
                        {
                            ts += "<br/>" + msg;
                        }
                        ViewBag.ErrorMessage = ts;
                        return View(new WellViewModel { Blocks = new List<Block>(), Fields = GetFields() });
                    }

                    if (!errorList.Any() && successList.Any() && msg.Length > 0)
                    {
                        ViewBag.ErrorCode = -1;
                        //ViewBag.ErrorMessage = mList.Count + " records were successfully uploaded.";

                        var tsx = successList.Count + " records were successfully uploaded.";

                         tsx += "<br/>" + msg;
                        
                        ViewBag.ErrorMessage = tsx;
                        return View();
                    }

                    else
                    {
                        ViewBag.ErrorCode = 5;

                        var tsx = successList.Count + " records were successfully uploaded.";

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

       public ActionResult Report()
       {
           try
           {
               var blockList = GetBlocks();

               if (!blockList.Any())
               {
                   return View(new List<Block>());
               }
               
               return View(blockList);
           }
           catch (Exception)
           {
               return View(new List<Block>());
           }           
       }
    }
}