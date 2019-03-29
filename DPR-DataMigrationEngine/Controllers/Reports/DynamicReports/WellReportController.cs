using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.GenericHelpers.GenericQueryBuilders;
using DPR_DataMigrationEngine.Services.ServiceManager;
using DPR_DataMigrationEngine.ViewModels;

namespace DPR_DataMigrationEngine.Controllers.Reports.DynamicReports
{
    [CustomAuthorize]
    public class WellReportController : Controller
    {
        public WellReportController()
		{
			 ViewBag.LoadStatus = "0";
		}
      
        public ActionResult DynamicReports()
        {
            return View(new WellViewModel
            { 
                WellTypes = GetWellTypes(), 
                Companies = GetCompanies(),
                Wells = new List<Well>(), 
                WellClasses = GetWellClasses(), 
                Terrains = GetTerrains(),
                Zones = GetZones(),
                WellQueries = GetWellQueries(),
                SpudYears = GetYears()
            });
        }

        private List<Field> GetFields()
        {
            var fielList = new FieldServices().GetFields() ?? new List<Field>();

            if (!fielList.Any())
            {
                return new List<Field>();
            }

            return fielList;
        }

        private List<WellType> GetWellTypes()
        {
            var wellTypeList = new WellTypeServices().GetAllOrderedWellTypes() ?? new List<WellType>();

            if (!wellTypeList.Any())
            {
                return new List<WellType>();
            }

            return wellTypeList;
        }
        
        private List<WellClass> GetWellClasses()
        {
            var wellTypeList = new WellClasServices().GetAllOrderedWellClasses2() ?? new List<WellClass>();

            if (!wellTypeList.Any())
            {
                return new List<WellClass>();
            }

            return wellTypeList;
        }

        private List<Company> GetCompanies()
        {
            var companies = new CompanyServices().GetCompaniesWithBlocks() ?? new List<Company>();

            if (!companies.Any())
            {
                return new List<Company>();
            }

            return companies;
        }

        private List<Terrain> GetTerrains()
        {
            var terrains = new TerrainServices().GetATerrains() ?? new List<Terrain>();

            if (!terrains.Any())
            {
                return new List<Terrain>();
            }
            return terrains;
        }

        private List<Block> GetBlocks()
        {
            return new BlockServices().GetAllOrderedBlocks() ?? new List<Block>();
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
            var jxs = new WellServices().GetSpudYears();
            if (!jxs.Any())
            {
                return new List<DocObject>();
            }
            var tdv = new List<DocObject>();
            jxs.ForEach(m =>
            {
                var gt = 0;
                var ds = int.TryParse(m, out gt);
                if (ds && gt > 0)
                {
                    tdv.Add(new DocObject
                    {

                        DocName = m,
                        DocId = int.Parse(m)
                    });
                }
                
            });
            return tdv.OrderByDescending(m => m.DocName).ToList();
        }
        private List<WellQuery> GetWellQueries()
        {
            var previousQueries = new WellQueryBuilderServices().GetAllOrderedWellQueries() ?? new List<WellQuery>();

            if (!previousQueries.Any())
            {
                return new List<WellQuery>();
            }
            return previousQueries;
        }
        public ActionResult GetWells(WellQuery queryBuilder)
        {
            if (queryBuilder == null)
            {
                return Json(new List<WellReportObject>(), JsonRequestBehavior.AllowGet);
            }


            if (queryBuilder.WellTypeId < 1 && queryBuilder.CompanyId < 1 && ((queryBuilder.StartDate != null && queryBuilder.StartDate.Value.Year == 0001) || (queryBuilder.StartDate == null)) && ((queryBuilder.EndDate != null && queryBuilder.EndDate.Value.Year == 0001) || (queryBuilder.EndDate == null)) && queryBuilder.BlockId < 1 && queryBuilder.WellClassId < 1 && queryBuilder.FieldId < 1 && queryBuilder.TerrainId < 1 && queryBuilder.ZoneId < 1)
            {
                return Json(new List<WellReportObject>(), JsonRequestBehavior.AllowGet);
            } 

            var wellList = new WellServices().GetOrderedWellReportObjects(queryBuilder) ?? new List<WellReportObject>();

            if (!wellList.Any())
            {
                return Json(new List<WellReportObject>(), JsonRequestBehavior.AllowGet);
            }
            Session["_successfulWellQuery"] = queryBuilder;
            var jsonResult = Json(wellList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
           
        }
        public ActionResult GetWells2(int queryId)
        {
            if (queryId < 1)
            {
                return Json(new List<WellReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var query = new WellQueryBuilderServices().GetWellQuery(queryId);

            if (query == null || query.WellQueryId < 1)
            {

                return Json(new List<WellReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var wellList = new WellServices().GetOrderedWellReportObjects(query) ?? new List<WellReportObject>();

            if (!wellList.Any())
            {
                return Json(new List<WellReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var jsonResult = Json(wellList, JsonRequestBehavior.AllowGet);
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

                if (Session["_successfulWellQuery"] == null)
                {
                    genVal.Error = "Session has expired";
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                var queryBuilder = Session["_successfulWellQuery"] as WellQuery;
               
                if (queryBuilder == null)
                {
                    genVal.Error = "Session has expired";
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                queryBuilder.WellQueryName = queryName.Trim();

                var k = new WellQueryBuilderServices().AddWellQueryCheckDuplicate(queryBuilder);
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
        public ActionResult IsWellQueryExisting()
        {
            var genVal = new GenericValidator();

            try
            {
                if (Session["_successfulWellQuery"] == null)
                {
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                var queryBuilder = Session["_successfulWellQuery"] as WellQuery;

                if (queryBuilder == null)
                {
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                var k = new WellQueryBuilderServices().IsWellQueryExisting(queryBuilder);
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
    }
}