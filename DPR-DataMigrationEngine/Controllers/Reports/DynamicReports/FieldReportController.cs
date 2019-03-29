using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;
using DPR_DataMigrationEngine.ViewModels;

namespace DPR_DataMigrationEngine.Controllers.Reports.DynamicReports
{
    [CustomAuthorize]
    public class FieldReportController : Controller
    {
       public ActionResult DynamicReports()
        {
            return View(new FieldViewModel
            {
                Terrains = GetTerrains(),
                Zones = GetZones(),
                Companies = GetCompanyies(), 
                FieldQueries = GetFieldQueries() });
        }

       private List<Terrain> GetTerrains()
       {
           return new TerrainServices().GetAllOrderedTerrains() ?? new List<Terrain>();
       }
        private List<BlockType> GetBlockTypes()
        {
            return new BlockTypeTypeServices().GetAllOrderedBlockTypes() ?? new List<BlockType>();
        }
        private List<Block> GetBlocks()
        {
            return new BlockServices().GetAllOrderedBlocks() ?? new List<Block>();
        }
        private List<Company> GetCompanyies()
        {
            return new CompanyServices().GetCompaniesWithBlocks() ?? new List<Company>();
        }
        private List<Zone> GetZones()
        {
            return new ZoneServices().GetAllOrderedZones() ?? new List<Zone>();
        }
        public ActionResult GetFields(FieldQuery queryBuilder)
        {
            if (queryBuilder == null)
            {
                return Json(new List<FieldReportObject>(), JsonRequestBehavior.AllowGet);
            }
            
            if (queryBuilder.TerrainId < 1 && queryBuilder.CompanyId < 1 && queryBuilder.ZoneId < 1)
            {
                return Json(new List<FieldReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var fieldList = new FieldServices().GetOrderedFieldReportObjects(queryBuilder) ?? new List<FieldReportObject>();

            if (!fieldList.Any())
            {
                return Json(new List<FieldReportObject>(), JsonRequestBehavior.AllowGet);
            }
            Session["_successfulFieldQuery"] = queryBuilder;
            var jsonResult = Json(fieldList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }
        public ActionResult GetFields2(int queryId)
        {
            if (queryId < 1)
            {
                return Json(new List<FieldReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var query = new FieldQueryBuilderServices().GetFieldQuery(queryId);

            if (query == null || query.FieldQueryId < 1)
            {

                return Json(new List<FieldReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var fieldList = new FieldServices().GetOrderedFieldReportObjects(query) ?? new List<FieldReportObject>();

            if (!fieldList.Any())
            {
                return Json(new List<FieldReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var jsonResult = Json(fieldList, JsonRequestBehavior.AllowGet);
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

                if (Session["_successfulFieldQuery"] == null)
                {
                    genVal.Error = "Session has expired";
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                var queryBuilder = Session["_successfulFieldQuery"] as FieldQuery;

                if (queryBuilder == null)
                {
                    genVal.Error = "Session has expired";
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                queryBuilder.FieldQueryName = queryName.Trim();

                var k = new FieldQueryBuilderServices().AddFieldQueryCheckDuplicate(queryBuilder);
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
        public ActionResult IsFieldQueryExisting()
        {
            var genVal = new GenericValidator();

            try
            {
                if (Session["_successfulFieldQuery"] == null)
                {
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                var queryBuilder = Session["_successfulFieldQuery"] as FieldQuery;

                if (queryBuilder == null)
                {
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                var k = new FieldQueryBuilderServices().IsFieldQueryExisting(queryBuilder);
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
        private List<FieldQuery> GetFieldQueries()
        {
            var previousQueries = new FieldQueryBuilderServices().GetAllOrderedFieldQueries() ?? new List<FieldQuery>();

            if (!previousQueries.Any())
            {
                return new List<FieldQuery>();
            }
            return previousQueries;
        }
    }
}