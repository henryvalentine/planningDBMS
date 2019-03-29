using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;
using DPR_DataMigrationEngine.ViewModels;

namespace DPR_DataMigrationEngine.Controllers.Reports.DynamicReports
{
    [CustomAuthorize]
    public class WellCompletionReportController : Controller
    {
        public WellCompletionReportController()
		{
			 ViewBag.LoadStatus = "0";
		}

        public ActionResult DynamicReports()
        {
            return View(new WellCompletionViewModel
            {
                Companies = GetCompanies(),
                Terrains = GetTerrains(),
                WellObjects = GetWells(),
                WellClasses = GetWellClasses(),
                WellTypes = GetWellTypes(),
                Zones = GetZones(),
                WellCompletionQueries = GetWellCompletionQueries(),
                WellCompletionTypes = GetWellCompletionTypes(),
                Equipments = GetEquipments(),

            });
        }
        
        private List<WellObject> GetWells()
        {
            var ttg = new WellServices().GetWells(200, 1);
            if (!ttg.Any())
            {
                return new List<WellObject>();
            }
            Session["_CompWellpageNumber"] = 1;
            return ttg;

        }

        public ActionResult GetMoreWells()
        {
            int pageNumber = 1;

            var o = Session["_CompWellpageNumber"];
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
            Session["_CompWellpageNumber"] = pageNumber;
            return Json(dfg, JsonRequestBehavior.AllowGet);
        }
        private List<WellCompletionQuery> GetWellCompletionQueries()
        {
            var previousQueries = new WellCompletionQueryBuilderServices().GetAllOrderedWellCompletionQueries() ?? new List<WellCompletionQuery>();

            if (!previousQueries.Any())
            {
                return new List<WellCompletionQuery>();
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
            var jxs = new WellCompletionServices().GetWellCompletionYears();
            if (!jxs.Any())
            {
                return new List<DocObject>();
            }
            var tdv = new List<DocObject>();
            jxs.ForEach(m =>
            {
                var ts = int.Parse(m);

                if (!tdv.Exists(t => t.DocId == ts))
                {
                    tdv.Add(new DocObject { DocName = m, DocId = ts });
                }
            });
            return tdv.OrderByDescending(m => m.DocName).ToList();
        }
        private List<DocObject> GetMonths()
        {
            return new EnumToObjList().GetMonthList().OrderByDescending(m => m.DocId).ToList();
        }
        private List<Terrain> GetTerrains()
        {
            return new TerrainServices().GetAllOrderedTerrains() ?? new List<Terrain>();
        }
        private List<Company> GetCompanies()
        {
            return new CompanyServices().GetAllOrderedCompanies() ?? new List<Company>();
        }
        private List<WellType> GetWellTypes()
        {
            try
            {
                var wellTypes = new WellTypeServices().GetAllOrderedWellTypes() ?? new List<WellType>();

                if (!wellTypes.Any())
                {
                    return new List<WellType>();
                }

                return wellTypes;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellType>();
            }
        }
        private List<WellClass> GetWellClasses()
        {
            try
            {
                var wellClasses = new WellClasServices().GetAllOrderedWellClasses() ?? new List<WellClass>();

                if (!wellClasses.Any())
                {
                    return new List<WellClass>();
                }

                return wellClasses;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellClass>();
            }
        }
        private List<WellCompletionType> GetWellCompletionTypes()
        {
            try
            {
                return new WellCompletionTypeServices().GetAllOrderedWellCompletionTypes() ?? new List<WellCompletionType>();
            }
            catch (Exception ex)
            {
                return new List<WellCompletionType>();
            }
        }
        private List<Equipment> GetEquipments()
        {
            try
            {
                return new EquipmentServices().GetEquipments() ?? new List<Equipment>();
            }
            catch (Exception ex)
            {
                return new List<Equipment>();
            }
        }
        public ActionResult GetWellCompletions(WellCompletionQuery queryBuilder)
        {
            if (queryBuilder == null)
            {
                return Json(new List<WellCompletionReportObject>(), JsonRequestBehavior.AllowGet);
            }

            if (queryBuilder.EquipmentId < 1 && queryBuilder.WellTypeId < 1 && queryBuilder.CompanyId < 1 && ((queryBuilder.StartDate != null && queryBuilder.StartDate.Value.Year == 0001) || (queryBuilder.StartDate == null)) && ((queryBuilder.EndDate != null && queryBuilder.EndDate.Value.Year == 0001) || (queryBuilder.EndDate == null)) && queryBuilder.CompletionTypeId < 1 && queryBuilder.WellClassId < 1 && queryBuilder.TerrainId < 1 && queryBuilder.ZoneId < 1 && queryBuilder.WellId < 1)
            {
                return Json(new List<WellCompletionReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var wellCompletionList = new WellCompletionServices().GetWellCompletionReports(queryBuilder) ?? new List<WellCompletionReportObject>();

            if (!wellCompletionList.Any())
            {
                return Json(new List<WellCompletionReportObject>(), JsonRequestBehavior.AllowGet);
            }
            Session["_successfulWellCompletionQuery"] = queryBuilder;
            var jsonResult = Json(wellCompletionList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }
        public ActionResult GetWellCompletions2(int queryId)
        {
            if (queryId < 1)
            {
                return Json(new List<WellCompletionReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var query = new WellCompletionQueryBuilderServices().GetWellCompletionQuery(queryId);

            if (query == null || query.WellCompletionQueryId < 1)
            {

                return Json(new List<WellCompletionReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var wellCompletionList = new WellCompletionServices().GetWellCompletionReports(query) ?? new List<WellCompletionReportObject>();

            if (!wellCompletionList.Any())
            {
                return Json(new List<WellCompletionReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var jsonResult = Json(wellCompletionList, JsonRequestBehavior.AllowGet);
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

                if (Session["_successfulWellCompletionQuery"] == null)
                {
                    genVal.Error = "Session has expired";
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                var queryBuilder = Session["_successfulWellCompletionQuery"] as WellCompletionQuery;

                if (queryBuilder == null)
                {
                    genVal.Error = "Session has expired";
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                queryBuilder.WellCompletionQueryName = queryName.Trim();

                var k = new WellCompletionQueryBuilderServices().AddWellCompletionQueryCheckDuplicate(queryBuilder);
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
        public ActionResult IsWellCompletionQueryExisting()
        {
            var genVal = new GenericValidator();

            try
            {
                if (Session["_successfulWellCompletionQuery"] == null)
                {
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                var queryBuilder = Session["_successfulWellCompletionQuery"] as WellCompletionQuery;

                if (queryBuilder == null)
                {
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                var k = new WellCompletionQueryBuilderServices().IsWellCompletionQueryExisting(queryBuilder);
                if (k)
                {
                    genVal.Code = 5;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                genVal.Code = -3;
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