using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;
using DPR_DataMigrationEngine.ViewModels;

namespace DPR_DataMigrationEngine.Controllers.Reports.DynamicReports
{
    [CustomAuthorize]
    public class WellWorkoverReportController : Controller
    {
        public WellWorkoverReportController()
		{
			 ViewBag.LoadStatus = "0";
		}

        private const int ItemsPerPage = 50;
        private const int PageNumber = 1;

        public ActionResult DynamicReports()
        {
            return View(new WellWorkoverViewModel
            {
                Companies = GetCompanies(),
                Terrains = GetTerrains(),
                WellObjects = GetWells(),
                WellClasses = GetWellClasses(),
                WellTypes = GetWellTypes(),
                Zones = GetZones(),
                WellWorkOverReasons = GetWellWorkoverReasons(),
                WellWorkoverQueries = GetWellWorkoverQueries(),
                Equipments = GetEquipments()
            });
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

        private List<WellObject> GetWells() 
        {
            var ttg = new WellServices().GetWells(200, PageNumber);
            if (!ttg.Any())
            {
                return new List<WellObject>();
            }
            Session["_workOverWellpageNumber"] = 1;
            return ttg;

        }

        public ActionResult GetMoreWells()
        {
            int pageNumber = 1;

            var o = Session["_workOverWellpageNumber"];
            if (o != null)
            {
                var tfd = (int)o;
                if (tfd < 1)
                {
                    pageNumber = PageNumber;
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
            Session["_workOverWellpageNumber"] = pageNumber;
            return Json( dfg, JsonRequestBehavior.AllowGet);
        }
        private List<WellWorkoverQuery> GetWellWorkoverQueries()
        {
            var previousQueries = new WellWorkoverQueryBuilderServices().GetAllOrderedWellWorkoverQueries() ?? new List<WellWorkoverQuery>();

            if (!previousQueries.Any())
            {
                return new List<WellWorkoverQuery>();
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
            var jxs = new WellWorkoverServices().GetWellWorkoverYears();
            if (!jxs.Any())
            {
                return new List<DocObject>();
            }
            var tdv = new List<DocObject>();
            jxs.ForEach(m =>
            {
                var ts = m;

                if (!tdv.Exists(t => t.DocId == ts))
                {
                    tdv.Add(new DocObject { DocName = m.ToString(CultureInfo.InvariantCulture), DocId = ts });
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
        public ActionResult GetWellWorkovers(WellWorkoverQuery queryBuilder)
        {
            if (queryBuilder == null)
            {
                return Json(new List<WellWorkoverReportObject>(), JsonRequestBehavior.AllowGet);
            }

            if (queryBuilder.EquipmentId < 1 && queryBuilder.WellTypeId < 1 && queryBuilder.CompanyId < 1 && ((queryBuilder.StartDate != null && queryBuilder.StartDate.Value.Year == 0001) || (queryBuilder.StartDate == null)) && ((queryBuilder.EndDate != null && queryBuilder.EndDate.Value.Year == 0001) || (queryBuilder.EndDate == null)) && queryBuilder.WorkoverReasonId < 1 && queryBuilder.WellClassId < 1 && queryBuilder.TerrainId < 1 && queryBuilder.ZoneId < 1)
            {
                return Json(new List<WellWorkoverReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var wellWorkoverList = new WellWorkoverServices().GetOrderedWellWorkoverReportObjects(queryBuilder) ?? new List<WellWorkoverReportObject>();

            if (!wellWorkoverList.Any())
            {
                return Json(new List<WellWorkoverReportObject>(), JsonRequestBehavior.AllowGet);
            }
            Session["_successfulWellWorkoverQuery"] = queryBuilder;
            var jsonResult = Json(wellWorkoverList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }
        private List<WellWorkOverReason> GetWellWorkoverReasons()
        {
            return new WellWorkOverReasonServices().GetAllOrderedWellWorkOverReasons() ?? new List<WellWorkOverReason>();
        }
        public ActionResult GetWellWorkovers2(int queryId)
        {
            if (queryId < 1)
            {
                return Json(new List<WellWorkoverReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var query = new WellWorkoverQueryBuilderServices().GetWellWorkoverQuery(queryId);

            if (query == null || query.WellWorkoverQueryId < 1)
            {

                return Json(new List<WellWorkoverReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var wellWorkoverList = new WellWorkoverServices().GetOrderedWellWorkoverReportObjects(query) ?? new List<WellWorkoverReportObject>();

            if (!wellWorkoverList.Any())
            {
                return Json(new List<WellWorkoverReportObject>(), JsonRequestBehavior.AllowGet);
            }

            var jsonResult = Json(wellWorkoverList, JsonRequestBehavior.AllowGet);
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

                if (Session["_successfulWellWorkoverQuery"] == null)
                {
                    genVal.Error = "Session has expired";
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                var queryBuilder = Session["_successfulWellWorkoverQuery"] as WellWorkoverQuery;

                if (queryBuilder == null)
                {
                    genVal.Error = "Session has expired";
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                queryBuilder.WellWorkoverQueryName = queryName.Trim();

                var k = new WellWorkoverQueryBuilderServices().AddWellWorkoverQueryCheckDuplicate(queryBuilder);
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
        public ActionResult IsWellWorkoverQueryExisting()
        {
            var genVal = new GenericValidator();

            try
            {
                if (Session["_successfulWellWorkoverQuery"] == null)
                {
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                var queryBuilder = Session["_successfulWellWorkoverQuery"] as WellWorkoverQuery;

                if (queryBuilder == null)
                {
                    genVal.Code = -1;
                    return Json(genVal, JsonRequestBehavior.AllowGet);
                }

                var k = new WellWorkoverQueryBuilderServices().IsWellWorkoverQueryExisting(queryBuilder);
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

        //public bool ExportToExcel()
        //{
        //    var o = Session["_currentProductionQuery"];
        //    if (o != null)
        //    {
        //        List<EmployeeMaster> emList = dc.EmployeeMasters.ToList();
        //        StringBuilder sb = new StringBuilder();

        //        if (emList.Count > 0)
        //        {
        //            string fileName = Path.Combine(Server.MapPath("~/ImportDocument"), DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xlsx");
        //            string conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0 Xml;HDR=Yes'";
        //            using (OleDbConnection con = new OleDbConnection(conString))
        //            {
        //                string strCreateTab = "Create table EmployeeData (" +
        //                    " [Employee ID] varchar(50), " +
        //                    " [Company Name] varchar(200), " +
        //                    " [Contact Name] varchar(200), " +
        //                    " [Contact Title] varchar(200), " +
        //                    " [Employee Address] varchar(200), " +
        //                    " [Postal Code] varchar(50))";
        //                if (con.State == ConnectionState.Closed)
        //                {
        //                    con.Open();
        //                }

        //                var cmd = new OleDbCommand(strCreateTab, con);
        //                cmd.ExecuteNonQuery();

        //                string strInsert = "Insert into EmployeeData([Employee ID],[Company Name]," +
        //                    " [Contact Name], [Contact Title], [Employee Address], [Postal Code]" +
        //                    ") values(?,?,?,?,?,?)";
        //                var cmdIns = new OleDbCommand(strInsert, con);
        //                cmdIns.Parameters.Add("?", OleDbType.VarChar, 50);
        //                cmdIns.Parameters.Add("?", OleDbType.VarChar, 200);
        //                cmdIns.Parameters.Add("?", OleDbType.VarChar, 200);
        //                cmdIns.Parameters.Add("?", OleDbType.VarChar, 200);
        //                cmdIns.Parameters.Add("?", OleDbType.VarChar, 200);
        //                cmdIns.Parameters.Add("?", OleDbType.VarChar, 50);

        //                foreach (var i in emList)
        //                {
        //                    cmdIns.Parameters[0].Value = i.EmployeeID;
        //                    cmdIns.Parameters[1].Value = i.CompanyName;
        //                    cmdIns.Parameters[2].Value = i.ContactName;
        //                    cmdIns.Parameters[3].Value = i.ContactTitle;
        //                    cmdIns.Parameters[4].Value = i.EmployeeAddress;
        //                    cmdIns.Parameters[5].Value = i.PostalCode;

        //                    cmdIns.ExecuteNonQuery();
        //                }
        //            }

        //            // Create Downloadable file
        //            byte[] content = File.ReadAllBytes(fileName);
        //            HttpContext context = HttpContext.Current;

        //            context.Response.BinaryWrite(content);
        //            context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //            context.Response.AppendHeader("Content-Disposition", "attachment; filename=EmployeeData.xlsx");
        //            context.Response.End();
        //        }

        //        return true;
        //    }
        //    return false;
        //}
        
    }
}