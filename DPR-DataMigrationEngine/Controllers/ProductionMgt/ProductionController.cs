using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPR_DataMigrationEngine.BulkUploadManagerManager;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;
using DPR_DataMigrationEngine.ViewModels;

namespace DPR_DataMigrationEngine.Controllers.ProductionMgt
{
    [CustomAuthorize]
    public class ProductionController : Controller   
    {
        public ProductionController()
		{
			 ViewBag.LoadStatus = "0";
		}
        private const int ItemsPerPage = 50;
        private const int PageNumber = 1;

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        public ActionResult Productions()
        {
            try
            {
                var productList = GetProducts();

                if (!productList.Any())
                {
                    ViewBag.Edit = 0;
                    ViewBag.Title = "Product Set Up";
                    return RedirectToAction("Products", "Product");
                }

                var fieldList = GetFields();

                if (!fieldList.Any())
                {
                    return RedirectToAction("Fields", "Field");
                }

                var yearList = GetYears();
                var monthList = GetMonths();
                int dataCount;
                var productionList = GetProductions(ItemsPerPage, PageNumber, out dataCount);
                if (!productionList.Any())
                {
                    ViewBag.ErrorCode = 0;
                    ViewBag.Title = "Production Set Up";
                    return View(new ProductionViewModel
                    {
                        Products = GetProducts(),
                        Fields = GetFields(),
                        YearList = yearList,
                        MonthList = monthList,
                        Productions = GetProductions(ItemsPerPage, PageNumber, out dataCount)
                    });

                }

                ViewBag.ErrorCode = 0;
                ViewBag.Title = "Manage Productions";
                Session["_prodPage"] = 1;
                return View(new ProductionViewModel { Products = GetProducts(), Fields = GetFields(), YearList = yearList, MonthList = monthList, Productions = GetProductions( ItemsPerPage, PageNumber, out dataCount) });
            }
            catch (Exception ex)
            {
               ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = "Production List is could not be retrieved. Please try again later.";
                ViewBag.Title = "Production Set Up";
                var yearList = GetYears();
                var monthList = GetMonths();
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return View(new ProductionViewModel { Products = GetProducts(), Fields = GetFields(), YearList = yearList, MonthList = monthList, Productions = new List<Production>()});

            }
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        public ActionResult DataTemplates()
        {
            return View();
        }

        private List<BlockType> GetBlockTypes()
        {
            return new BlockTypeTypeServices().GetAllOrderedBlockTypes() ?? new List<BlockType>();
        }
       private List<Terrain> GetTerrains()
        {
            return new TerrainServices().GetAllOrderedTerrains() ?? new List<Terrain>();
        }
        private List<Company> GetCompanies()
        {
            return new CompanyServices().GetAllOrderedCompanies() ?? new List<Company>();
        }
        private List<Product> GetProducts()
        {
            try
            {
                var productList = new ProductServices().GetAllOrderedProducts() ?? new List<Product>();

                if (!productList.Any())
                {
                    return new List<Product>();
                }

                return productList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Product>();
            }
        }
        private List<Production> GetProductions(int itemsPerPage, int pageNumber, out int dataCount)
        {
            try
            {
                var productionList = new ProductionServices().GetAllOrderedProductions(itemsPerPage, pageNumber, out dataCount);
                ViewBag.PrDataCount = dataCount.ToString(CultureInfo.InvariantCulture);

                var totalPages = dataCount / itemsPerPage;

                // Counting the last page
                if (dataCount % itemsPerPage != 0)
                {
                    totalPages++;
                }

                ViewBag.PrTotalPages = totalPages;
                ViewBag.PrPage = pageNumber;
                if (!productionList.Any())
                {
                    return new List<Production>();
                }
                productionList.ForEach(m =>
                {
                    m.FieldName = m.Field.Name;
                    m.ProductName = m.Product.Name;
                    m.Quantity = Convert.ToDecimal(m.Quantity).ToString("#,##0");
                    m.MonthName = Enum.GetName(typeof(MonthList), m.Month);
                });
                return productionList;
            }
            catch (Exception ex)
            {
                dataCount = 0;
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Production>();
            }
        }
        private List<Field> GetFields()
        {
            try
            {
                var fieldList = new FieldServices().GetFields() ?? new List<Field>();

                if (!fieldList.Any())
                {
                    return new List<Field>();
                }

                return fieldList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Field>();
            }
        }
        public ActionResult GetMoreWellCompletions()
        {
            int currentPage;

            var o = Session["_prodPage"];
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

            var wellList = GetWellComletionObjects(50, currentPage);

            if (!wellList.Any())
            {
                return Json(new List<ProductionObject>(), JsonRequestBehavior.AllowGet);
            }
            Session["_prodPage"] = currentPage;
            return Json(wellList, JsonRequestBehavior.AllowGet);
        }
        private List<ProductionObject> GetWellComletionObjects(int itemsPerPage, int pageNumber)
        {
            try
            {
                var wellCompletions = new ProductionServices().GetMoreProductionObjects(itemsPerPage, pageNumber);

                if (!wellCompletions.Any())
                {

                    return new List<ProductionObject>();
                }

                return wellCompletions;
            }
            catch (Exception ex)
            {
                return new List<ProductionObject>();
            }
        }
        public ActionResult GetProductionsByField(int fieldId)
        {
            try
            {
                if (fieldId < 1)
                {
                    return Json(new List<Field>(), JsonRequestBehavior.AllowGet);
                }

                var productionList = new ProductionServices().GetAllOrderedProductionsByFieldId(fieldId);

                if (!productionList.Any())
                {
                    return Json(new List<Production>(), JsonRequestBehavior.AllowGet);
                }

                return Json(productionList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<Production>(), JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        [HttpPost]
        public ActionResult AddProduction(Production production)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    production.Error = "Please supply all required fields and try again";
                    production.ErrorCode = -1;
                    return Json(production, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(production);
                if (wx.Code < 1)
                {
                    production.Error = wx.Error;
                    production.ErrorCode = -1;
                    return Json(production, JsonRequestBehavior.AllowGet);
                }
                var ty = production.Quantity.Replace(",", "").Replace(" ", "");
                production.Quantity = ty;
                production.Year = production.ProductionDate.Year;
                production.Month = production.ProductionDate.Month;
                production.MonthName = Enum.GetName(typeof(MonthList), production.Month);
                var k = new ProductionServices().AddProduction(production);
                if (k < 1)
                {
                   production.Error = "Process Failed! Please contact the Admin or try again later";
                    production.ErrorCode = 0;
                    return Json(production, JsonRequestBehavior.AllowGet);
                }
                
                production.Error = "Record was added successfully";
                production.ErrorCode = 1;
                production.ProductionId = k;
                return Json(production, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                production.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                production.ErrorCode = 0;
                return Json(production, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
       [HttpPost]
        public ActionResult EditProduction(Production production)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_production"] == null)
                {
                    production.Error = "Session has expired";
                    production.ErrorCode = 0;
                    return Json(production, JsonRequestBehavior.AllowGet);
                }

                var oldProduction = Session["_production"] as Production;

                if (oldProduction == null || oldProduction.ProductionId < 1)
                {
                    production.Error = "Session has expired";
                    production.ErrorCode = 0;
                    return Json(production, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    production.Error = "Please supply all required Productions and try again";
                    production.ErrorCode = -1;
                    return Json(production, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(production);

                if (wx.Code < 1)
                {
                    production.Error = wx.Error;
                    production.ErrorCode = -1;
                    return Json(production, JsonRequestBehavior.AllowGet);
                }
                oldProduction.Remark = production.Remark;
                oldProduction.FieldId = production.FieldId;
                oldProduction.ProductId = production.ProductId;
                oldProduction.Quantity = production.Quantity.Replace(",", "").Replace(" ", ""); ;
                oldProduction.APIGravity = production.APIGravity;
                oldProduction.Year = production.ProductionDate.Year;
                oldProduction.Month = production.ProductionDate.Month;
                production.ProductionDateString = production.ProductionDate.ToString("yyyy/MM/dd");
                production.Year = production.ProductionDate.Year;
                production.Month = production.ProductionDate.Month;
                production.MonthName = Enum.GetName(typeof(MonthList), production.Month);
                var k = new ProductionServices().UpdateProductionCheckDuplicate(oldProduction);
                if (k < 1)
                {
                    production.Error = "Process Failed! Please contact the Admin or try again later";
                    production.ErrorCode = 0;
                    return Json(production, JsonRequestBehavior.AllowGet);
                }
                
                    production.Error = "Production  Information was successfully updated";
                    production.ErrorCode = 1;
                    return Json(production, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                production.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                production.ErrorCode = 0;
                return Json(production, JsonRequestBehavior.AllowGet);
            }
        }
        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        [HttpPost]
        [ActionName("DeleteProduction")]
        public ActionResult DeleteProduction(int id)
        {
            var production = new Production();

            try
            {
                if (id < 1)
                {
                    production.Error = "Invalid Selection";
                    production.ErrorCode = 0;
                    return Json(production, JsonRequestBehavior.AllowGet);
                }
                if (new ProductionServices().DeleteProductionCheckReferences(id))
                {
                    production.Error = "Production  Information was successfully deleted.";
                    production.ErrorCode = 1;
                    return Json(production, JsonRequestBehavior.AllowGet);
                }

                production.Error = "Process Failed! Please try again later";
                production.ErrorCode = 0;
                return Json(production, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                production.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                production.ErrorCode = 0;
                return Json(production, JsonRequestBehavior.AllowGet);
            }
        }

       [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        public ActionResult EditProduction(int id)
        {
            var production = new Production();
            try
            {
                if (id < 1)
                {
                    production.Error = "Invalid Selection!";
                    production.ErrorCode = -1;
                    return Json(production, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new ProductionServices().GetProduction(id);

                if (myViewObj == null || myViewObj.ProductionId < 1)
                {
                    production.Error = "Production  Information could not be retrieved.";
                    production.ErrorCode = -1;
                    return Json(production, JsonRequestBehavior.AllowGet);
                }
                Session["_production"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.ProductionId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                production.Error = "An unknown error was Production  Information could not be retrieved.";
                production.ErrorCode = -1;
                return Json(production, JsonRequestBehavior.AllowGet);
            }
        }
        private static GenericValidator ValidateControl(Production model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (model.ProductionDate.Year == 0001)
                {
                    gVal.Error = "Please provide a valid Production Year.";
                    gVal.Code = 0;
                    return gVal;

                }
                if (model.ProductionDate > DateTime.Now)
                {
                    gVal.Error = "Please provide a valid Production Year.";
                    gVal.Code = 0;
                    return gVal;

                }
                if (!string.IsNullOrEmpty(model.Quantity))
                {
                    var td = model.Quantity.Replace(",", "").Replace(" ", "").Replace(".", "");
                    long yt;
                    var fer = long.TryParse(td, out yt);
                    if (!fer)
                    {
                        gVal.Error = "Invalid Product Quantity";
                        gVal.Code = 0;
                        return gVal;
                    }
                    if (yt < 1)
                    {
                        gVal.Error = "Invalid Product Quantity";
                        gVal.Code = 0;
                        return gVal;
                    }
                }
                else
                {
                    gVal.Error = "Please provide the Product Quantity";
                    gVal.Code = 0;
                    return gVal;
                    
                }

                if (model.ProductId < 1)
                {
                    gVal.Error = "Please select a valid Product";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.FieldId < 1)
                {
                    gVal.Error = "Please select a valid Field";
                    gVal.Code = 0;
                    return gVal;
                }
                
                gVal.Code = 1;
                return gVal;
            }
            catch (Exception)
            {
                gVal.Error = "Process validation failed. Please supply all required entries and try again.";
                gVal.Code = 0;
                return gVal;
            }
        }
        public ActionResult ProductionUpload()
        {
            return View();
        }
        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        [HttpPost]
        public ActionResult ProductionUpload(HttpPostedFileBase file)
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
                    var mList = new List<Production>();
                    var msg = string.Empty;
                    if (!new ProductionUploadManager().Import(Server.MapPath(path), "production", ref mList, ref msg))
                    {
                        ViewBag.ErrorCode = -1;
                        ViewBag.ErrorMessage = "Bulk upload Failed! An unknown error was encountered.";
                        return View();
                    }

                    if (!mList.Any())
                    {
                        var error = msg.Length > 0 ? msg : "Bulk upload Failed! unknown error occurred";
                        ViewBag.ErrorCode = -2;
                        ViewBag.ErrorMessage = error;
                        return View();
                    }

                    var errorList = new List<Production>();
                    var succesList = new List<Production>();
                    foreach (var production in mList)
                    {
                        //production.FieldId = productionInfo.FieldId;
                        //production.ProductId = productionInfo.ProductId;

                        var processedItem = new ProductionServices().AddProduction(production);

                        if (processedItem < 1)
                        {
                            errorList.Add(production);
                        }
                        else
                        {
                            succesList.Add(production);
                        }
                    }

                    if (errorList.Any() && succesList.Any())
                    {
                        var ts = succesList.Count + " records were successfully uploaded." +
                            "<br/>" + errorList.Count + " records could not be uploaded due to duplicates/unknown errors encountered.";
                        ViewBag.ErrorCode = -1;

                        if (msg.Length > 0)
                        {
                            ts += "<br/>" + msg;
                        }
                        ViewBag.ErrorMessage = ts;
                        return View();
                    }

                    if (errorList.Any() && !succesList.Any())
                    {
                        var ts = errorList.Count + " records could not be uploaded due to duplicates/unknown errors encountered.";
                        ViewBag.ErrorCode = -1;

                        if (msg.Length > 0)
                        {
                            ts += "<br/>" + msg;
                        }
                        ViewBag.ErrorMessage = ts;
                        return View();
                    }

                    if (!errorList.Any() && succesList.Any() && msg.Length > 0)
                    {
                        ViewBag.ErrorCode = -1;
                        //ViewBag.ErrorMessage = mList.Count + " records were successfully uploaded.";

                        var tsx = succesList.Count + " records were successfully uploaded.";
                        tsx += "<br/>" + msg;
                        ViewBag.ErrorMessage = tsx;
                        return View();
                    }

                    if (!errorList.Any() && succesList.Any() && (string.IsNullOrEmpty(msg) || msg.Length < 1))
                    {
                        ViewBag.ErrorCode = 5;
                        var tsx = succesList.Count + " records were successfully uploaded.";
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
                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = "Error: File is empty or the content could not be accessed";
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return View();
            }
        }
        public bool DownloadContentFromFolder(string path)
        {
            try
            {
                Response.Clear();
                var filename = Path.GetFileName(path);
                HttpContext.Response.Buffer = true;
                HttpContext.Response.Charset = "";
                HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = GetMimeType(filename);
                HttpContext.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + filename + "\"");
                Response.WriteFile(Server.MapPath(path));
                Response.End();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }
        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            var extension = Path.GetExtension(fileName);
            if (extension != null)
            {
                var ext = extension.ToLower();
                var regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
                if (regKey != null && regKey.GetValue("Content Type") != null)
                    mimeType = regKey.GetValue("Content Type").ToString();
            }
            return mimeType;
        }
        private List<DocObject> GetMonths()
        {
            return new EnumToObjList().GetMonthList().OrderByDescending(m => m.DocId).ToList();
        }
        private List<DocObject> GetYears()
        {
            var jxs = new ProductionServices().GetProductionYears();
            if (!jxs.Any())
            {
                return new List<DocObject>();
            }
            var tdv = new List<DocObject>();
            jxs.ForEach(m =>
            {
                if (!tdv.Exists(t => t.DocId == m))
                {
                    tdv.Add(new DocObject{ DocName = m.ToString(CultureInfo.InvariantCulture), DocId = m});
                }
            });
            return tdv.OrderByDescending(m => m.DocName).ToList();
        }
        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        public ActionResult GetProductionsByMonth(GenericSearch search)
        {
            try
            {
                if (search.Year < 1)
                {
                    ViewBag.ErrorCode = -1;
                    ViewBag.ErrorMessage = "Please provide a valid Report Year";
                    return View("Productions", new ProductionViewModel
                    {
                        Products = GetProducts(),
                        Fields = GetFields(),
                        YearList = GetYears(),
                        MonthList = GetMonths(),
                        Productions = new List<Production>()
                    });
                }

                var productions = GetProducttionsBySearchDate(search);
                ViewBag.Title = "Manage Productions";
                ViewBag.ErrorCode = 5;
                ViewBag.ErrorMessage = "";

                return View("Productions", new ProductionViewModel
                {
                    Products = GetProducts(),
                    Fields = GetFields(),
                    YearList = GetYears(),
                    MonthList = GetMonths(),
                    Productions = productions
                });
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = "An error was encountered, the seach operation could not be completed.";
                return View("Productions", new ProductionViewModel
                {
                    Products = GetProducts(),
                    Fields = GetFields(),
                    YearList = GetYears(),
                    MonthList = GetMonths(),
                    Productions = new List<Production>()
                });
            }
        }
        public List<Production> GetProducttionsBySearchDate(GenericSearch search)
        {
            try
            {
                var productions = new ProductionServices().GetAllOrderedProductionsByMonth(search.Month, search.Year);

                if (!productions.Any())
                {
                    return new List<Production>();
                }

                productions.ForEach(m =>
                {
                    m.FieldName = m.Field.Name;
                    m.ProductName = m.Product.Name;
                    m.MonthName = Enum.GetName(typeof(MonthList), m.Month);
                    m.Quantity = Convert.ToDecimal(m.Quantity).ToString("#,##0");
                });
                return productions;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Production>();
            }
        }
        public ActionResult GetProductionsByQueryDateDate(GenericSearch search)
        {
            try
            {
                if (search.Year < 1)
                {
                    ViewBag.ErrorCode = -1;
                    ViewBag.ErrorMessage = "Please provide a valid Report Year";
                    return View("Report", new ProductionViewModel
                    {
                        Products = new List<Product>(),
                        Fields = new List<Field>(),
                        YearList = GetYears(),
                        MonthList = GetMonths(),
                        ProductionObjects = new List<ProductionObject>()
                    });
                }

                if (search.Month > 0 )
                {
                    ViewBag.SearchPeriod = Enum.GetName(typeof(MonthList), search.Month) + "/" + search.Year;
                }

                else
                {
                    ViewBag.SearchPeriod = search.Year;
                }

                var txd = new ProductionServices().GetProductionStaticReportByPeriod(PageNumber, ItemsPerPage, search.Month, search.Year);
               
                
                if (!txd.Any())
                {
                    ViewBag.ErrorMessage = "No record found";
                    ViewBag.ErrorCode = -1;
                   return View("Report", new ProductionViewModel
                    {
                        Products = new List<Product>(),
                        Fields = new List<Field>(),
                        YearList = GetYears(),
                        MonthList = GetMonths(),
                        ProductionObjects = new List<ProductionObject>()
                    });
                 } 

               var txx = new ProductionViewModel
                {
                    Products = new List<Product>(),
                    Fields = new List<Field>(),
                    YearList = GetYears(),
                    MonthList = GetMonths(),
                    ProductionObjects = txd
                };

                Session["_prodRepoPage"] = 1;
                Session["_prodRepoGenSearch"] = search;
                return View("Report", txx);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                ViewBag.ErrorMessage = "No record found";
                ViewBag.ErrorCode = -1;
               return View("Report", new ProductionViewModel
                {
                    Products = new List<Product>(),
                    Fields = new List<Field>(),
                    YearList = GetYears(),
                    MonthList = GetMonths(),
                    Productions = new List<Production>()
                });
            }
        }
        public ActionResult Report()
        {
            try
            {
                return View(new ProductionViewModel
                {
                    Products = new List<Product>(),
                    Fields = new List<Field>(),
                    YearList = GetYears(),
                    MonthList = GetMonths(),
                    ProductionObjects = new List<ProductionObject>()
                });
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return View(new ProductionViewModel
                {
                    Products = new List<Product>(),
                    Fields = new List<Field>(),
                    YearList = GetYears(),
                    MonthList = GetMonths(),
                    Productions = new List<Production>()
                });
            }
        }

        public ActionResult GetMoreStaticProductionReports()
        {
            int currentPage;
           
            if(Session["_prodRepoGenSearch"] == null)
            {
                return Json(new List<ProductionObject>(), JsonRequestBehavior.AllowGet);
            }

            var search = Session["_prodRepoGenSearch"] as GenericSearch;

            if (search == null || search.Year < 1)
            {
                return Json(new List<ProductionObject>(), JsonRequestBehavior.AllowGet);
            }

            var o = Session["_prodRepoPage"];

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

            var productionReportList = GetProductionObjects(50, currentPage, search);
            
            if (!productionReportList.Any())
            {
                return Json(new List<ProductionObject>(), JsonRequestBehavior.AllowGet);
            }
             Session["_prodRepoPage"] = currentPage;
            return Json(productionReportList, JsonRequestBehavior.AllowGet);
        }
        private List<ProductionObject> GetProductionObjects(int pageNumber, int itemsPerPage, GenericSearch search)
        {
            try
            {
                var productionObjects = new ProductionServices().GetProductionStaticReportByPeriod(itemsPerPage, pageNumber, search.Month, search.Year);

                if (!productionObjects.Any())
                {

                    return new List<ProductionObject>();
                }

                return productionObjects;
            }
            catch (Exception ex)
            {
                return new List<ProductionObject>();
            }
        }
    }
}