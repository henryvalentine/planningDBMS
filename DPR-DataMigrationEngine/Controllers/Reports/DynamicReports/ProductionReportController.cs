using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;
using DPR_DataMigrationEngine.ViewModels;

namespace DPR_DataMigrationEngine.Controllers.Reports.DynamicReports
{
    [CustomAuthorize]
    public class ProductionReportController : Controller
    {
        public ProductionReportController()
		{
			 ViewBag.LoadStatus = "0";
		}
        public ActionResult DynamicReports()
        {
            return View(new ProductionViewModel
            {
                Products = GetProducts(), 
                Fields = GetFields(), 
                Companies = GetCompanies(),
                BlockTypes = GetBlockTypes(),
                ProductionQueries = GetProductionQueries()
            });
        }
        private List<ProductionQuery> GetProductionQueries()
        {
            var previousQueries = new ProductionQueryBuilderServices().GetAllOrderedProductionQueries() ?? new List<ProductionQuery>();

            if (!previousQueries.Any())
            {
                return new List<ProductionQuery>();
            }
            return previousQueries;
        }
        private List<Zone> GetZones()
            {
                var zones = new ZoneServices().GetAllOrderedZones() ?? new List<Zone>();

                if (!zones.Any())
                {
                    return new List<Zone>();
                }
                return zones;
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
                    tdv.Add(new DocObject { DocName = m.ToString(CultureInfo.InvariantCulture), DocId = m });
                }
            });
            return tdv.OrderByDescending(m => m.DocName).ToList();
        }
        private List<DocObject> GetMonths()
        {
            return new EnumToObjList().GetMonthList().OrderByDescending(m => m.DocId).ToList();
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
            return new CompanyServices().GetCompaniesWithBlocks() ?? new List<Company>();
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
        public ActionResult GetProductions(ProductionQuery queryBuilder)
        {
            if (queryBuilder == null)
            {
                return Json(new List<ProductionObject>(), JsonRequestBehavior.AllowGet);
            }


            if (queryBuilder.BlockId < 1 && queryBuilder.CompanyId < 1 && ((queryBuilder.StartDate != null && queryBuilder.StartDate.Value.Year == 0001) || (queryBuilder.StartDate == null)) && ((queryBuilder.EndDate != null && queryBuilder.EndDate.Value.Year == 0001) || (queryBuilder.EndDate == null)) && queryBuilder.FieldId < 1 && queryBuilder.ProductId < 1 && queryBuilder.TerrainId < 1 && queryBuilder.ZoneId < 1)
            {
                return Json(new List<ProductionObject>(), JsonRequestBehavior.AllowGet);
            }

            var productionList = new ProductionServices().GetOrderedProductionReportObjects(queryBuilder) ?? new List<ProductionObject>();

            if (!productionList.Any())
            {
                return Json(new List<ProductionObject>(), JsonRequestBehavior.AllowGet);
            }
            Session["_successfulProductionQuery"] = queryBuilder;
            var jsonResult = Json(productionList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }
        public ActionResult GetProductions2(int queryId)
        {
            if (queryId < 1)
            {
                return Json(new List<ProductionObject>(), JsonRequestBehavior.AllowGet);
            }


            var query = new ProductionQueryBuilderServices().GetProductionQuery(queryId);

            if (query == null || query.ProductionQueryId < 1)
            {

                return Json(new List<ProductionObject>(), JsonRequestBehavior.AllowGet);
            }

            var productionList = new ProductionServices().GetOrderedProductionReportObjects(query) ?? new List<ProductionObject>();

            if (!productionList.Any())
            {
                return Json(new List<ProductionObject>(), JsonRequestBehavior.AllowGet);
            }

            var jsonResult = Json(productionList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }
        [HttpPost]
        public ActionResult SaveSuccessfulQuery(string queryName)
        {
            var genVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(queryName))
                {
                    genVal.Error = "Please supply all required fields and try again";
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                if (Session["_successfulProductionQuery"] == null)
                {
                    genVal.Error = "Session has expired";
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                var queryBuilder = Session["_successfulProductionQuery"] as ProductionQuery;

                if (queryBuilder == null)
                {
                    genVal.Error = "Session has expired";
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                queryBuilder.ProductionQueryName = queryName.Trim();

                var k = new ProductionQueryBuilderServices().AddProductionQueryCheckDuplicate(queryBuilder);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        genVal.Error = "Query already exists";
                        genVal.Code = -3;
                        return Json(genVal, JsonRequestBehavior.AllowGet);
                    }

                    if (k == -4)
                    {
                        genVal.Error = "Query Name already exists. Please provide a different one";
                        genVal.Code = -3;
                        return Json(genVal, JsonRequestBehavior.AllowGet);
                    }

                    genVal.Error = "Process Failed! Please contact the Admin or try again later";
                    genVal.Code = 0;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                genVal.Error = "Query was successfully saved";
                genVal.Code = (int) k;
                return Json(genVal, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                //ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                genVal.Error = "An unknown error was encountered. Query could not be saved. Please try again.";
                genVal.Code = 0;
                return Json(genVal, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult IsProductionQueryExisting()
        {
            var genVal = new GenericValidator();

            try
            {
                if (Session["_successfulProductionQuery"] == null)
                {
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                var queryBuilder = Session["_successfulProductionQuery"] as ProductionQuery;

                if (queryBuilder == null)
                {
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                var k = new ProductionQueryBuilderServices().IsProductionQueryExisting(queryBuilder);
                if (k)
                {
                    genVal.Code = 5;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                if (Roles.IsUserInRole("Admin"))
                {
                    genVal.Code = -5;
                }
                else
                {
                    genVal.Code = -2;
                }
                
                return Json(genVal, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                genVal.Code = 0;
                return Json(genVal, JsonRequestBehavior.AllowGet);
            }
        }

        //public void ExportClientsListToExcel()
        //{
        //    var grid = new System.Web.UI.WebControls.GridView();

        //    grid.DataSource = /*from d in dbContext.diners
        //                      where d.user_diners.All(m => m.user_id == userID) && d.active == true */
        //                      from d in ClientsList
        //                      select new
        //                      {
        //                          FirstName = d.FirstName,
        //                          LastName = d.LastName,
        //                          DOB = d.Dob,
        //                          Email = d.Email

        //                      };

        //    grid.DataBind();

        //    Response.ClearContent();
        //    Response.AddHeader("content-disposition", "attachment; filename=Exported_Diners.xls");
        //    Response.ContentType = "application/excel";
        //    var sw = new StringWriter();
        //    var htw = new HtmlTextWriter(sw);

        //    grid.RenderControl(htw);

        //    Response.Write(sw.ToString());

        //    Response.End();

        //}
    }
}