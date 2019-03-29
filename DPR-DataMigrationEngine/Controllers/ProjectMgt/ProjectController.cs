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

namespace DPR_DataMigrationEngine.Controllers.ProjectMgt
{
    
    [CustomAuthorize]
    public class ProjectController : Controller
    {
        public ProjectController()    
		{
			 ViewBag.LoadStatus = "0";
		}

        private const int ItemsPerPage = 40;
        private const int PageNumber = 1;

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        public ActionResult Projects(int? page, int? pageSize)
        {
            try
            {
                var terrainList = GetTerrains();

                if (!terrainList.Any())
                {
                    return RedirectToAction("Terrains", "Terrain");
                }

                var companyList = GetCompanies();

                if (!companyList.Any())
                {
                    return RedirectToAction("Companies", "Company");
                }

                var sectorList = GetSectors();
                if (!sectorList.Any())
                {
                    return RedirectToAction("Sectors", "Sector");
                }

                var projectTypes = GetProjectTypes();
                if (!projectTypes.Any())
                {
                    return RedirectToAction("ProjectTypes", "ProjectType");
                }
                int dataCount;
                var projectList = GetProjects(pageSize ?? ItemsPerPage, page ?? PageNumber, out dataCount);
                if (!projectList.Any())
                {
                    ViewBag.Edit = 1;
                    ViewBag.ErrorCode = 0;
                    ViewBag.Title = "Project Set Up";
                    return View(new ProjectViewModel { Terrains = terrainList, Sectors = sectorList, ProjectTypes = projectTypes, Companies = companyList, Projects = projectList });

                }
                
                ViewBag.ErrorCode = 0;
                ViewBag.Title = "Manage Projects";
                return View(new ProjectViewModel { Terrains = terrainList, Sectors = sectorList, ProjectTypes = projectTypes, Companies = companyList, Projects = projectList });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorCode = -1;
                ViewBag.Title = "Manage Projects";
                ViewBag.ErrorMesssage = "Unknown error was encountered. Project list could not be retrieved";
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return View(new ProjectViewModel { Terrains = GetTerrains(), Sectors = GetSectors(), ProjectTypes = GetProjectTypes(), Companies = GetCompanies(), Projects = new List<Project>() });
            }
        }
        private List<Project> GetProjects(int itemsPerPage, int pageNumber, out int dataCount)
        {
            try
            {
                var projectList = new ProjectServices().GetAllOrderedProjects(itemsPerPage, pageNumber, out dataCount);

                if (!projectList.Any())
                {
                    return new List<Project>();
                }

                //projectList.Remove(projectList.Find(m => m.ProjectId == (int)OtherNotAvailable.Not_Available));
                ViewBag.DataCount = dataCount.ToString(CultureInfo.InvariantCulture);

                var totalPages = dataCount / itemsPerPage;

                // Counting the last page
                if (dataCount % itemsPerPage != 0)
                {
                    totalPages++;
                }

                ViewBag.TotalPages = totalPages;
                ViewBag.Page = pageNumber;
                projectList.ForEach(m =>
                {
                    var status = "";
                    if (m.CompletionStatus == 1)
                    {
                        status = "Completed";
                    }
                    if (m.CompletionStatus == 0)
                    {
                        status = "UnCompleted";
                    }

                    m.SectorName = m.Sector.Name;
                    m.ProjectTypeName = m.ProjectType.Name;
                    m.CompanyName = m.Company.Name;
                    m.TerrainName = m.Terrain.Name;
                    m.Completion = status;
                    if (m.DateCompleted != null)
                    {
                        m.DatecomPletedString = ((DateTime)m.DateCompleted).ToString("yyyy/MM/dd");
                    }

                });
                return projectList;
            }
            catch (Exception ex)
            {
                dataCount = 0;
               return new List<Project>();
            }
        }
        private List<Terrain> GetTerrains()
        {
            var sectorList = new TerrainServices().GetAllOrderedTerrains() ?? new List<Terrain>();

            if (!sectorList.Any())
            {
                return new List<Terrain>();
            }

            return sectorList;
        }
        private List<Company> GetCompanies()
        {
            var sectorList = new CompanyServices().GetAllOrderedCompanies() ?? new List<Company>();

            if (!sectorList.Any())
            {
                return new List<Company>();
            }

            return sectorList;
        }
        private List<Sector> GetSectors()
        {
            var sectorList = new SectorServices().GetAllOrderedSectors() ?? new List<Sector>();

            if (!sectorList.Any())
            {
               return new List<Sector>();
            }

            return sectorList;
        }
        private List<ProjectType> GetProjectTypes()
        {
            var sectorList = new ProjectTypeServices().GetAllOrderedProjectTypes() ?? new List<ProjectType>();

            if (!sectorList.Any())
            {
                return new List<ProjectType>();
            }

            return sectorList;
        }

       [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        public ActionResult GetProjectsByTerrain(int terrainId)
        {
            try
            {
                if (terrainId < 1)
                {
                    return Json(new List<Field>(), JsonRequestBehavior.AllowGet);
                }

                var projectList = new ProjectServices().GetAllOrderedProjectsByTerrain(terrainId);

                if (!projectList.Any())
                {
                    return Json(new List<Project>(), JsonRequestBehavior.AllowGet);
                }

                return Json(projectList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<Project>(), JsonRequestBehavior.AllowGet);
            }
        }

       [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        [HttpPost]
        public ActionResult AddProject(Project project)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    project.Error = "Please supply all required fields and try again";
                    project.ErrorCode = -1;
                    return Json(project, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(project);
                if (wx.Code < 1)
                {
                    project.Error = wx.Error;
                    project.ErrorCode = -1;
                    return Json(project, JsonRequestBehavior.AllowGet);
                }
                
                var k = new ProjectServices().AddProjectCheckDuplicate(project);
                if (k < 1)
                {
                   project.Error = "Process Failed! Please contact the Admin or try again later";
                    project.ErrorCode = 0;
                    return Json(project, JsonRequestBehavior.AllowGet);
                }
                
                project.Error = "Record was added successfully";
                project.ErrorCode = 1;
                project.ProjectId = k;
                project.DatecomPletedString = project.DateCompleted.ToString();
                return Json(project, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                project.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                project.ErrorCode = 0;
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return Json(project, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        [HttpPost]
        public ActionResult EditProject(Project project)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_project"] == null)
                {
                    project.Error = "Session has expired";
                    project.ErrorCode = 0;
                    return Json(project, JsonRequestBehavior.AllowGet);
                }

                var oldProject = Session["_project"] as Project;

                if (oldProject == null || oldProject.ProjectId < 1)
                {
                    project.Error = "Session has expired";
                    project.ErrorCode = 0;
                    return Json(project, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    project.Error = "Please supply all required Projects and try again";
                    project.ErrorCode = -1;
                    return Json(project, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(project);

                if (wx.Code < 1)
                {
                    project.Error = wx.Error;
                    project.ErrorCode = -1;
                    return Json(project, JsonRequestBehavior.AllowGet);
                }

                oldProject.TerrainId = project.TerrainId;
                oldProject.SectorId = project.SectorId;
                oldProject.CompanyId = project.CompanyId;
                oldProject.Name = project.Name.Trim();
                oldProject.Description = project.Description;
                oldProject.ProjectTypeId = project.ProjectTypeId;
                oldProject.Duration = project.Duration;
                oldProject.Cost = project.Cost;
                oldProject.DateCompleted = project.DateCompleted;
                oldProject.ProjectObjectives = project.ProjectObjectives;
                oldProject.CompletionStatus = project.CompletionStatus;
                project.DateCompleted = project.DateCompleted;
                if (project.DateCompleted != null)
                    project.DatecomPletedString = ((DateTime)project.DateCompleted).ToString(CultureInfo.InvariantCulture);
                var k = new ProjectServices().UpdateProjectCheckDuplicate(oldProject);
                if (k < 1)
                {
                    project.Error = "Process Failed! Please contact the Admin or try again later";
                    project.ErrorCode = 0;
                    return Json(project, JsonRequestBehavior.AllowGet);
                }
                
                    project.Error = "Project  Information was successfully updated";
                    project.ErrorCode = 1;
                    return Json(project, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                project.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                project.ErrorCode = 0;
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return Json(project, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        [HttpPost]
        [ActionName("DeleteProject")]
        public ActionResult DeleteProject(int id)
        {
            var project = new Project();

            try
            {
                if (id < 1)
                {
                    project.Error = "Invalid Selection";
                    project.ErrorCode = 0;
                    return Json(project, JsonRequestBehavior.AllowGet);
                }
                if (new ProjectServices().DeleteProjectCheckReferences(id))
                {
                    project.Error = "Project  Information was successfully deleted.";
                    project.ErrorCode = 1;
                    return Json(project, JsonRequestBehavior.AllowGet);
                }

                project.Error = "Process Failed! Please try again later";
                project.ErrorCode = 0;
                return Json(project, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                project.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                project.ErrorCode = 0;
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return Json(project, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        public ActionResult EditProject(int id)
        {
            var project = new Project();
            try
            {
                if (id < 1)
                {
                    project.Error = "Invalid Selection!";
                    project.ErrorCode = -1;
                    return Json(project, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new ProjectServices().GetProject(id);

                if (myViewObj == null || myViewObj.ProjectId < 1)
                {
                    project.Error = "Project  Information could not be retrieved.";
                    project.ErrorCode = -1;
                    return Json(project, JsonRequestBehavior.AllowGet);
                }
                if (myViewObj.DateCompleted != null)
                {
                    myViewObj.DatecomPletedString = myViewObj.DateCompleted.ToString();
                }
                Session["_project"] = myViewObj;
                myViewObj.ErrorCode = 7;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                project.Error = "An unknown error was Project  Information could not be retrieved.";
                project.ErrorCode = -1;
                return Json(project, JsonRequestBehavior.AllowGet);
            }
        }
        private static GenericValidator ValidateControl(Project model)
        {
            var gVal = new GenericValidator();

            try
            {
              
                if (string.IsNullOrEmpty(model.Name.Trim()))
                {
                    gVal.Error = "Please provide Company's Name.";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.Cost <= 0)
                {
                    gVal.Error = "Please provide Project Cost.";
                    gVal.Code = 0;
                    return gVal;
                }
                
                if (model.Duration < 1)
                {
                    gVal.Error = "Please a valid Project Duration.";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.ProjectTypeId < 1)
                {
                    gVal.Error = "Please a valid Project Type.";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.TerrainId < 1)
                {
                    gVal.Error = "Please select a valid Terrain.";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.SectorId < 1)
                {
                    gVal.Error = "Please select a valid Sector";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.CompanyId < 1)
                {
                    gVal.Error = "Please select a valid Company";
                    gVal.Code = 0;
                    return gVal;
                }
                
                gVal.Code = 1;
                return gVal;
            }
            catch (Exception ex)
            {
                gVal.Error = "Process validation failed. Please supply all required entries and try again.";
                gVal.Code = 0;
                return gVal;
            }
        }

        public ActionResult ProjectUpload()
        {
            return View();
        }

      [CustomAuthorize(Roles = "Data_Entry,Super_Admin")]
        [HttpPost]
        public ActionResult ProjectUpload(HttpPostedFileBase file)
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
                    var mList = new List<Project>();
                    var msg = string.Empty;
                    if (!new ProjectUploadManager().Import(Server.MapPath(path), "projects", ref mList, ref msg))
                    {
                        ViewBag.ErrorCode = -1;
                        ViewBag.ErrorMessage = "Bulk upload Failed! An unknown error was encountered.";
                        return View();
                    }
                    
                    if (!mList.Any() && msg.Length > 0)
                    {
                        ViewBag.ErrorCode = -1;
                        ViewBag.ErrorMessage = msg;
                        return View();
                    }

                    var errorList = new List<Project>();
                    var successList = new List<Project>();
                    foreach (var projectInfo in mList)
                    {
                        var processedItem = new ProjectServices().AddProjectCheckDuplicate(projectInfo);

                        if (processedItem < 1)
                        {
                            errorList.Add(projectInfo);
                        }
                        else
                        {
                            successList.Add(projectInfo);
                        }
                    }

                    if (errorList.Any() && successList.Any())
                    {
                        var ts = successList.Count + " records were successfully uploaded." +
                            "\n" + errorList.Count + " records could not be uploaded due to duplicates/unknown errors encountered.";
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
                        var ts = errorList.Count + " records could not be uploaded due to duplicates/unknown errors encountered.";
                        ViewBag.ErrorCode = -1;

                        if (msg.Length > 0)
                        {
                            ts += "<br/>" + msg;
                        }
                        ViewBag.ErrorMessage = ts;
                        return View();
                    }

                    if (!errorList.Any() && successList.Any() && msg.Length > 0)
                    {
                        ViewBag.ErrorCode = -1;

                        var tsx = successList.Count + " records were successfully uploaded.";

                        ViewBag.ErrorMessage = tsx + "<br/>" + msg;
                        return View();
                    }

                    ViewBag.ErrorCode = 1;
                    ViewBag.ErrorMessage = successList.Count + " records were successfully uploaded.";
                    return View();
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
        public ActionResult GetAllOrderedProjectsByTerrain(int terrainId)
        {
            try
            {
               
                var myObjList = new ProjectServices().GetAllOrderedProjectsByTerrain(terrainId);
                if (!myObjList.Any())
                {
                    return Json(new List<Project>(), JsonRequestBehavior.AllowGet);
                }

                return Json(myObjList, JsonRequestBehavior.AllowGet);
               
            }
            catch (Exception)
            {
                return Json(new List<Project>(), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetOrderedCompletedProjectsByMonth(DateTime date, int completionStatus)
        {
            try
            {
                var myObjList = new ProjectServices().GetOrderedCompletedProjectsByMonth(date, completionStatus);
                if (!myObjList.Any())
                {
                    return Json(new List<Project>(), JsonRequestBehavior.AllowGet);
                }

                return Json(myObjList, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return Json(new List<Project>(), JsonRequestBehavior.AllowGet);
            }
        }
        private List<ProjectMileStone> GetProjectMilestones(long projectId)
        {
            try
            {
                var projectMilestoneList = new ProjectMileStoneServices().GetAllOrderedProjectMileStonesByProject(projectId);

                if (!projectMilestoneList.Any())
                {
                    return new List<ProjectMileStone>();
                }

                projectMilestoneList.ForEach(m =>
                {
                    if (m.ParentId > 0)
                    {
                        var milestone = new ProjectMileStoneServices().GetProjectMileStone(m.ParentId);
                        if (milestone != null && milestone.ProjectMileStoneId > 0)
                        {
                            m.ParentName = milestone.Title;
                        }
                        else
                        {
                            m.ParentName = "N/A";
                        }
                    }

                    m.ProjectName = m.Project.Name;
                    m.DateDueString = m.DateDue.ToString("yyyy/MM/dd");
                    m.MileStoneName = m.MilestoneStatu.Name;
                });
                return projectMilestoneList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<ProjectMileStone>();
            }
        }

        public ViewResult ProjectMileStonesReport(long projectId)
        {
            try
            {
                var projectMilestoneList = ProjectMileStonesForReport(projectId);
                if (!projectMilestoneList.Any())
                {
                   return View( new List<ProjectMileStone>());

                }
                ViewBag.Title = "Project: " + projectMilestoneList[0].Project.Name;
                return View(projectMilestoneList);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return View(new List<ProjectMileStone>());
            }
        }
        private List<ProjectMileStone> ProjectMileStonesForReport(long projectId)
        {
            try
            {
                var projectMilestoneList = new ProjectMileStoneServices().GetAllOrderedProjectMileStonesByProject(projectId);

                if (!projectMilestoneList.Any())
                {
                    return new List<ProjectMileStone>();
                }

                projectMilestoneList.ForEach(m =>
                {
                    if (m.ParentId > 0)
                    {
                        var milestone = new ProjectMileStoneServices().GetProjectMileStone(m.ParentId);
                        if (milestone != null && milestone.ProjectMileStoneId > 0)
                        {
                            m.ParentName = milestone.Title;
                        }
                        else
                        {
                            m.ParentName = "N/A";
                        }
                    }

                    m.ProjectName = m.Project.Name;
                    m.DateDueString = m.DateDue.ToString("yyyy/MM/dd");
                    m.MileStoneName = m.MilestoneStatu.Name;
                });
                return projectMilestoneList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<ProjectMileStone>();
            }
        }
        public ViewResult ProjectMileStones(long projectId)
        {
            try
            {
                var milestones = GetMilestoneStatus();
                if (!milestones.Any())
                {
                    ViewBag.ErrorCode = -1;
                    ViewBag.Title = "MileStone Set Up";
                    ViewBag.ErrorMeassage = "Milestone List is empty";
                    return View("~/Views/MilestoneStatus/MilestoneStatus.cshtml", new List<MilestoneStatu>());
                }
               
                var projectMilestoneList = GetProjectMilestones(projectId);
                if (!projectMilestoneList.Any())
                {
                    var myViewObj = new ProjectServices().GetStrippedProject(projectId);

                    if (myViewObj == null || myViewObj.ProjectId < 1)
                    {
                        ViewBag.Title = "Project  Information could not be retrieved.";
                        ViewBag.ErrorCode = -1;
                        return View(new ProjectMilestoneViewModel { MilestoneStatuses = milestones, Projects = new List<Project>(), CompletionStatuses = GetCompletionStatuses(), ProjectMileStones = new List<ProjectMileStone>() });
                    }

                    ViewBag.ErrorCode = -1;
                    ViewBag.Title = "Project :  " + myViewObj.Name;
                    ViewBag.ProjectId = myViewObj.ProjectId;
                    ViewBag.ErrorMeassage = "Project Milestone List is empty";
                    return View(new ProjectMilestoneViewModel { MilestoneStatuses = milestones, Projects = new List<Project>(), CompletionStatuses = GetCompletionStatuses(), ProjectMileStones = new List<ProjectMileStone>() });

                }

                ViewBag.ErrorCode = 3;
                ViewBag.Title = "Project: " + projectMilestoneList[0].Project.Name;
                Session["_project_For_Milestone"] = projectMilestoneList[0].Project;
                ViewBag.ProjectId = projectMilestoneList[0].ProjectId;
                return View(new ProjectMilestoneViewModel { MilestoneStatuses = milestones, Projects = new List<Project>(), CompletionStatuses = GetCompletionStatuses(), ProjectMileStones = projectMilestoneList });
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return View(new ProjectMilestoneViewModel { MilestoneStatuses = GetMilestoneStatus(), Projects = new List<Project>(), CompletionStatuses = GetCompletionStatuses(), ProjectMileStones = new List<ProjectMileStone>() });
            }
        }
        private List<DocObject> GetCompletionStatuses()
        {
            try
            {
                return new EnumToObjList().GetCompletionStatuses();

            }
            catch (Exception ex)
            {

                return new List<DocObject>();
            }
        }
        public List<MilestoneStatu> GetMilestoneStatus()
        {
            return new MilestoneStatusServices().GetAllOrderedMilestoneStatuses() ?? new List<MilestoneStatu>();
        }
        public ViewResult Reports()
        {
            var tty = GetProjectObjects(ItemsPerPage, PageNumber);
            if (!tty.Any())
            {
                return View(new List<ProjectReportObject>());
            }
            Session["_projectRepoPage"] = 1;

            return View(tty);
        }

        private List<ProjectReportObject> GetProjectObjects(int pageNumber, int itemsPerPage)
        {
            try
            {
                var productionObjects = new ProjectServices().GetStaticProjectReport(itemsPerPage, pageNumber);

                if (!productionObjects.Any())
                {

                    return new List<ProjectReportObject>();
                }

                return productionObjects;
            }
            catch (Exception ex)
            {
                return new List<ProjectReportObject>();
            }
        }

        public ActionResult GetMoreStaticProJectReports()
        {
            int currentPage;

            var o = Session["_projectRepoPage"];
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

            var productionReportList = GetProjectObjects(50, currentPage);

            if (!productionReportList.Any())
            {
                return Json(new List<ProductionObject>(), JsonRequestBehavior.AllowGet);
            }
            Session["_projectRepoPage"] = currentPage;
            return Json(productionReportList, JsonRequestBehavior.AllowGet);
        }

        private List<Project> GetProjects(int companyId)
        {
            try
            {
                if (companyId < 1)
                {
                    return new List<Project>();
                }
                var projectList = new ProjectServices().GetAllOrderedProjectsByCompany(companyId);

                if (!projectList.Any())
                {
                    return new List<Project>();
                }

                
                projectList.ForEach(m =>
                {
                    var status = "";
                    if (m.CompletionStatus == 1)
                    {
                        status = "Completed";
                    }
                    if (m.CompletionStatus == 0)
                    {
                        status = "UnCompleted";
                    }

                    m.SectorName = m.Sector.Name;
                    m.ProjectTypeName = m.ProjectType.Name;
                    m.CompanyName = m.Company.Name;
                    m.TerrainName = m.Terrain.Name;
                    m.Completion = status;
                    if (m.DateCompleted != null)
                    {
                        m.DatecomPletedString = m.DateCompleted.ToString();
                    }

                });
                return projectList;
            }
            catch (Exception ex)
            {
                return new List<Project>();
            }
        }
    }
}