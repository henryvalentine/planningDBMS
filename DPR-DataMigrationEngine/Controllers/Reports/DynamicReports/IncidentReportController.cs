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
    public class IncidentReportController : Controller
    {
        public IncidentReportController()
		{
			 ViewBag.LoadStatus = "0";
		}

        public ActionResult DynamicReports()
        {
            return View(new IncidentViewModel
            {
                Companies = GetCompanies(),
                IncidentTypes = GetIncidentTypes(),
                IncidentQueries = GetIncidentQueries()
            });
        }

        private List<DocObject> GetYears()
        {
            var jxs = new IncidentHistoryServices().GetIncidentYears();
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

        private List<IncidentType> GetIncidentTypes()
        {
            return new IncidentTypeServices().GetAllOrderedIncidentTypes() ?? new List<IncidentType>();
        }
        private List<Company> GetCompanies()
        {
            return new CompanyServices().GetAllOrderedCompanies() ?? new List<Company>();
        }
     
        public ActionResult GetIncidents(IncidentQuery queryBuilder)
        {
            if (queryBuilder == null)
            {
                return Json(new List<IncidentReportObject>(), JsonRequestBehavior.AllowGet);
            }


            if (queryBuilder.CompanyId < 1 && queryBuilder.CompanyId < 1 && queryBuilder.IncidentTypeId < 1 && ((queryBuilder.StartDate != null && queryBuilder.StartDate.Value.Year == 0001) || (queryBuilder.StartDate == null)) && ((queryBuilder.EndDate != null && queryBuilder.EndDate.Value.Year == 0001) || (queryBuilder.EndDate == null)))
            {
                return Json(new List<IncidentReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var incidentList = new IncidentHistoryServices().GetOrderedIncidentReportObjects(queryBuilder) ?? new List<IncidentReportObject>();

            if (!incidentList.Any())
            {
                return Json(new List<IncidentReportObject>(), JsonRequestBehavior.AllowGet);
            }
            Session["_successfulIncidentQuery"] = queryBuilder;
            var jsonResult = Json(incidentList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }
        public ActionResult GetIncidents2(int queryId)
        {
            if (queryId < 1)
            {
                return Json(new List<IncidentReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var query = new IncidentQueryBuilderServices().GetIncidentQuery(queryId);

            if (query == null || query.IncidentQueryId < 1)
            {

                return Json(new List<IncidentReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var incidentList = new IncidentHistoryServices().GetOrderedIncidentReportObjects(query) ?? new List<IncidentReportObject>();

            if (!incidentList.Any())
            {
                return Json(new List<IncidentReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var jsonResult = Json(incidentList, JsonRequestBehavior.AllowGet);
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
                    genVal.Error = "Please supply all required incidents and try again";
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                if (Session["_successfulIncidentQuery"] == null)
                {
                    genVal.Error = "Session has expired";
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                var queryBuilder = Session["_successfulIncidentQuery"] as IncidentQuery;

                if (queryBuilder == null)
                {
                    genVal.Error = "Session has expired";
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                queryBuilder.IncidentQueryName = queryName.Trim();

                var k = new IncidentQueryBuilderServices().AddIncidentQueryCheckDuplicate(queryBuilder);
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
        public ActionResult IsIncidentQueryExisting()
        {
            var genVal = new GenericValidator();

            try
            {
                if (Session["_successfulIncidentQuery"] == null)
                {
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                var queryBuilder = Session["_successfulIncidentQuery"] as IncidentQuery;

                if (queryBuilder == null)
                {
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                var k = new IncidentQueryBuilderServices().IsIncidentQueryExisting(queryBuilder);
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
        private List<IncidentQuery> GetIncidentQueries()
        {
            var previousQueries = new IncidentQueryBuilderServices().GetAllOrderedIncidentQueries() ?? new List<IncidentQuery>();

            if (!previousQueries.Any())
            {
                return new List<IncidentQuery>();
            }
            return previousQueries;
        }

    }
}