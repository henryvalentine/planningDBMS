using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;
using DPR_DataMigrationEngine.ViewModels;

namespace DPR_DataMigrationEngine.Controllers.Reports.DynamicReports
{
    [CustomAuthorize]
    public class ProjectReportController : Controller
    {
        public ProjectReportController()
		{
			 ViewBag.LoadStatus = "0";
		}
        //public ActionResult DynamicReports()
        //{
        //    return View(new ProjectViewModel
        //    {
        //        Products = GetProducts(), Fields = GetFields(),
        //        YearList = GetYears(), MonthList = GetMonths(), 
        //        Companies = GetCompanies(),
        //        Terrains = GetTerrains(),
        //        BlockTypes = GetBlockTypes(),
        //        Zones = GetZones(),
        //        ProjectQueries = GetProjectQueries()
        //    });
        //}
        //private List<ProjectQuery> GetProjectQueries()
        //{
        //    var previousQueries = new ProjectQueryBuilderServices().GetAllOrderedProjectQueries() ?? new List<ProjectQuery>();

        //    if (!previousQueries.Any())
        //    {
        //        return new List<ProjectQuery>();
        //    }
        //    return previousQueries;
        //}
        //private List<Zone> GetZones()
        //    {
        //        var zones = new ZoneServices().GetAllOrderedZones() ?? new List<Zone>();

        //        if (!zones.Any())
        //        {
        //            return new List<Zone>();
        //        }
        //        return zones;
        //    }
        //private List<DocObject> GetYears()
        //{
        //    var jxs = new ProjectServices().GetProjectYears();
        //    if (!jxs.Any())
        //    {
        //        return new List<DocObject>();
        //    }
        //    var tdv = new List<DocObject>();
        //    jxs.ForEach(m =>
        //    {
        //        var ts = int.Parse(m);

        //        if (!tdv.Exists(t => t.DocId == ts))
        //        {
        //            tdv.Add(new DocObject { DocName = m, DocId = ts });
        //        }
        //    });
        //    return tdv.OrderByDescending(m => m.DocName).ToList();
        //}
        //private List<DocObject> GetMonths()
        //{
        //    return new EnumToObjList().GetMonthList().OrderByDescending(m => m.DocId).ToList();
        //}
        //private List<BlockType> GetBlockTypes()
        //{
        //    return new BlockTypeTypeServices().GetAllOrderedBlockTypes() ?? new List<BlockType>();
        //}
        //private List<Terrain> GetTerrains()
        //{
        //    return new TerrainServices().GetAllOrderedTerrains() ?? new List<Terrain>();
        //}
        //private List<Company> GetCompanies()
        //{
        //    return new CompanyServices().GetAllOrderedCompanies() ?? new List<Company>();
        //}
        //private List<Product> GetProducts()
        //{
        //    try
        //    {
        //        var productList = new ProductServices().GetAllOrderedProducts() ?? new List<Product>();

        //        if (!productList.Any())
        //        {
        //            return new List<Product>();
        //        }

        //        return productList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
        //        return new List<Product>();
        //    }
        //}
        //private List<Field> GetFields()
        //{
        //    try
        //    {
        //        var fieldList = new FieldServices().GetFields() ?? new List<Field>();

        //        if (!fieldList.Any())
        //        {
        //            return new List<Field>();
        //        }

        //        return fieldList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
        //        return new List<Field>();
        //    }
        //}
        //public ActionResult GetProjects(ProjectQuery queryBuilder)
        //{
        //    if (queryBuilder == null)
        //    {
        //        return Json(new List<ProjectObject>(), JsonRequestBehavior.AllowGet);
        //    }


        //    if (queryBuilder.BlockTypeId < 1 && queryBuilder.CompanyId < 1 && queryBuilder.Year < 1 && queryBuilder.Month < 1 && queryBuilder.FieldId < 1 && queryBuilder.ProductId < 1 && queryBuilder.TerrainId < 1 && queryBuilder.ZoneId < 1)
        //    {
        //        return Json(new List<ProjectObject>(), JsonRequestBehavior.AllowGet);
        //    }

        //    var ProjectList = new ProjectServices().GetOrderedProjectReportObjects(queryBuilder) ?? new List<ProjectObject>();

        //    if (!ProjectList.Any())
        //    {
        //        return Json(new List<ProjectObject>(), JsonRequestBehavior.AllowGet);
        //    }
        //    Session["_successfulProjectQuery"] = queryBuilder;
        //    var jsonResult = Json(ProjectList, JsonRequestBehavior.AllowGet);
        //    jsonResult.MaxJsonLength = int.MaxValue;
        //    return jsonResult;

        //}
        //public ActionResult GetProjects2(int queryId)
        //{
        //    if (queryId < 1)
        //    {
        //        return Json(new List<ProjectObject>(), JsonRequestBehavior.AllowGet);
        //    }


        //    var query = new ProjectQueryBuilderServices().GetProjectQuery(queryId);

        //    if (query == null || query.ProjectQueryId < 1)
        //    {

        //        return Json(new List<ProjectObject>(), JsonRequestBehavior.AllowGet);
        //    }

        //    var ProjectList = new ProjectServices().GetOrderedProjectReportObjects(query) ?? new List<ProjectObject>();

        //    if (!ProjectList.Any())
        //    {
        //        return Json(new List<ProjectObject>(), JsonRequestBehavior.AllowGet);
        //    }

        //    var jsonResult = Json(ProjectList, JsonRequestBehavior.AllowGet);
        //    jsonResult.MaxJsonLength = int.MaxValue;
        //    return jsonResult;

        //}
        //[HttpPost]
        //public ActionResult SaveSuccessfulQuery(string queryName)
        //{
        //    var genVal = new GenericValidator();

        //    try
        //    {
        //        if (string.IsNullOrEmpty(queryName))
        //        {
        //            genVal.Error = "Please supply all required fields and try again";
        //            genVal.Code = -1;
        //            return Json(genVal, JsonRequestBehavior.AllowGet);
        //        }

        //        if (Session["_successfulProjectQuery"] == null)
        //        {
        //            genVal.Error = "Session has expired";
        //            genVal.Code = -1;
        //            return Json(genVal, JsonRequestBehavior.AllowGet);
        //        }

        //        var queryBuilder = Session["_successfulProjectQuery"] as ProjectQuery;

        //        if (queryBuilder == null)
        //        {
        //            genVal.Error = "Session has expired";
        //            genVal.Code = -1;
        //            return Json(genVal, JsonRequestBehavior.AllowGet);
        //        }

        //        queryBuilder.ProjectQueryName = queryName.Trim();

        //        var k = new ProjectQueryBuilderServices().AddProjectQueryCheckDuplicate(queryBuilder);
        //        if (k < 1)
        //        {
        //            if (k == -3)
        //            {
        //                genVal.Error = "Query already exists";
        //                genVal.Code = -3;
        //                return Json(genVal, JsonRequestBehavior.AllowGet);
        //            }

        //            if (k == -4)
        //            {
        //                genVal.Error = "Query Name already exists. Please provide a different one";
        //                genVal.Code = -3;
        //                return Json(genVal, JsonRequestBehavior.AllowGet);
        //            }

        //            genVal.Error = "Process Failed! Please contact the Admin or try again later";
        //            genVal.Code = 0;
        //            return Json(genVal, JsonRequestBehavior.AllowGet);
        //        }

        //        genVal.Error = "Query was successfully saved";
        //        genVal.Code = k;
        //        return Json(genVal, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception)
        //    {
        //        //ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
        //        genVal.Error = "An unknown error was encountered. Query could not be saved. Please try again.";
        //        genVal.Code = 0;
        //        return Json(genVal, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public ActionResult IsProjectQueryExisting()
        //{
        //    var genVal = new GenericValidator();

        //    try
        //    {
        //        if (Session["_successfulProjectQuery"] == null)
        //        {
        //            genVal.Code = -1;
        //            return Json(genVal, JsonRequestBehavior.AllowGet);
        //        }

        //        var queryBuilder = Session["_successfulProjectQuery"] as ProjectQuery;

        //        if (queryBuilder == null)
        //        {
        //            genVal.Code = -1;
        //            return Json(genVal, JsonRequestBehavior.AllowGet);
        //        }

        //        var k = new ProjectQueryBuilderServices().IsProjectQueryExisting(queryBuilder);
        //        if (k)
        //        {
        //            genVal.Code = 5;
        //            return Json(genVal, JsonRequestBehavior.AllowGet);
        //        }

        //        if (Roles.IsUserInRole("Admin"))
        //        {
        //            genVal.Code = -5;
        //        }
        //        else
        //        {
        //            genVal.Code = -2;
        //        }
                
        //        return Json(genVal, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception)
        //    {
        //        genVal.Code = 0;
        //        return Json(genVal, JsonRequestBehavior.AllowGet);
        //    }
        //}

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