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
    public class ProjectMileStoneController : Controller
    {
        public ProjectMileStoneController()
		{
			 ViewBag.LoadStatus = "0";
		}
        private const int ItemsPerPage = 40;
        private const int PageNumber = 1;
        public ViewResult ProjectMileStones(int? page, int? pageSize)
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
                var projectList = GetProjects();
                if (!projectList.Any())
                {
                    ViewBag.ErrorCode = 0;
                    ViewBag.Title = "Project Set Up";
                    ViewBag.ErrorMeassage = "Project List is empty";
                    return View(new ProjectMilestoneViewModel { MilestoneStatuses = milestones, Projects = projectList, CompletionStatuses = GetCompletionStatuses(), ProjectMileStones = new List<ProjectMileStone>() });

                }

                int dataCount;
                ViewBag.ErrorCode = 0;
                ViewBag.Title = "Manage Project Milestones";
                return View(new ProjectMilestoneViewModel { MilestoneStatuses = milestones, Projects = projectList, CompletionStatuses = GetCompletionStatuses(), ProjectMileStones = GetProjectMileStones(pageSize ?? ItemsPerPage, page ?? PageNumber, out dataCount) });
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return View(new ProjectMilestoneViewModel { MilestoneStatuses = GetMilestoneStatus(), Projects = GetProjects(), CompletionStatuses = GetCompletionStatuses(), ProjectMileStones = new List<ProjectMileStone>() });
            }
        }
        private List<ProjectMileStone> GetProjectMileStones(int itemsPerPage, int pageNumber, out int dataCount)
        {
            try
            {
                var projectMilestoneList = new ProjectMileStoneServices().GetAllOrderedProjectMileStones(itemsPerPage, pageNumber, out dataCount);
               
                if (!projectMilestoneList.Any())
                {
                    dataCount = 0;
                    ViewBag.TotalPages = 0;
                    ViewBag.Page = 1;
                    return new List<ProjectMileStone>();
                }
                ViewBag.PrDataCount = dataCount.ToString(CultureInfo.InvariantCulture);

                var totalPages = dataCount / itemsPerPage;

                if (dataCount % itemsPerPage != 0)
                {
                    totalPages++;
                }

                ViewBag.TotalPages = totalPages;
                ViewBag.Page = pageNumber;
                projectMilestoneList.Remove(projectMilestoneList.Find(m => m.ProjectMileStoneId == (int)ProjectMilestoneNotAvailable.Not_Available));
                projectMilestoneList.ForEach(m =>
                {
                    var milestone = new ProjectMileStoneServices().GetProjectMileStone(m.ProjectMileStoneId);
                    
                    if (milestone != null && milestone.ProjectMileStoneId > 0)
                    {
                        m.ParentName = milestone.Title;
                    }
                    else
                    {
                        m.ParentName = "N/A";
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
                dataCount = 0;
                ViewBag.TotalPages = 0;
                ViewBag.Page = 1;
               return new List<ProjectMileStone>();
            }
        }

        public ActionResult GetAllProjectMileStones()
        {
            try
            {
                var projectMilestoneList = new ProjectMileStoneServices().GetProjectMileStones();

                if (!projectMilestoneList.Any())
                {
                    return Json(new List<ProjectMilestoneObject>(), JsonRequestBehavior.AllowGet); 
                }
                return Json(projectMilestoneList, JsonRequestBehavior.AllowGet); 
            }
            catch (Exception )
            {
                return Json(new List<ProjectMilestoneObject>(), JsonRequestBehavior.AllowGet); 
            }
        }
        private List<DocObject> GetCompletionStatuses()
        {
            try
            {
                return new EnumToObjList().GetCompletionStatuses();
                
            }
            catch (Exception)
            {
                
                return new List<DocObject>();
            }
        }
        public List<MilestoneStatu> GetMilestoneStatus()
        {
            return new MilestoneStatusServices().GetAllOrderedMilestoneStatuses() ?? new List<MilestoneStatu>();
        }
        private List<Project> GetProjects()
        {
            try
            {
                var sectorList = new ProjectServices().GetOrderedProjects() ?? new List<Project>();

                if (!sectorList.Any())
                {
                    return new List<Project>();
                }

                return sectorList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Project>();
            }
        }
        [HttpPost]
        public ActionResult AddProjectMilestone(ProjectMileStone projectMileStone)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    projectMileStone.Error = "Please supply all required fields and try again";
                    projectMileStone.ErrorCode = -1;
                    return Json(projectMileStone, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(projectMileStone);
                if (wx.Code < 1)
                {
                    projectMileStone.Error = wx.Error;
                    projectMileStone.ErrorCode = -1;
                    return Json(projectMileStone, JsonRequestBehavior.AllowGet);
                }
                projectMileStone.LastUpdated = DateTime.Now.ToString("yyyyMM/dd");
                var k = new ProjectMileStoneServices().AddProjectMileStoneCheckDuplicate(projectMileStone);
                if (k < 1)
                {
                   projectMileStone.Error = "Process Failed! Please contact the Admin or try again later";
                    projectMileStone.ErrorCode = 0;
                    return Json(projectMileStone, JsonRequestBehavior.AllowGet);
                }
                
                projectMileStone.Error = "Record was added successfully";
                projectMileStone.ErrorCode = 1;
                projectMileStone.ProjectMileStoneId = k;
                projectMileStone.DateDueString = projectMileStone.DateDue.ToString("yyyy/MM/dd");
                return Json(projectMileStone, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                projectMileStone.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                projectMileStone.ErrorCode = 0;
                return Json(projectMileStone, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult EditProjectprojectMileStone(ProjectMileStone projectMileStone)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_projectMileStone"] == null)
                {
                    projectMileStone.Error = "Session has expired";
                    projectMileStone.ErrorCode = 0;
                    return Json(projectMileStone, JsonRequestBehavior.AllowGet);
                }

                var oldProjectMileStone = Session["_projectMileStone"] as ProjectMileStone;

                if (oldProjectMileStone == null || oldProjectMileStone.ProjectId < 1)
                {
                    projectMileStone.Error = "Session has expired";
                    projectMileStone.ErrorCode = 0;
                    return Json(projectMileStone, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    projectMileStone.Error = "Please supply all required fields Projects and try again";
                    projectMileStone.ErrorCode = -1;
                    return Json(projectMileStone, JsonRequestBehavior.AllowGet);
                }

                if (projectMileStone.ProjectId < 1)

                {
                    if( Session["_project_For_Milestone"] == null)
                    {
                        projectMileStone.Error = "Please select a valid Project";
                        projectMileStone.ErrorCode = -1;
                        return Json(projectMileStone, JsonRequestBehavior.AllowGet);
                    }

                    var project = Session["_project_For_Milestone"] as Project;

                    if (project == null || project.ProjectId < 1)
                    {
                        projectMileStone.Error = "Please select a valid Project";
                        projectMileStone.ErrorCode = -1;
                        return Json(projectMileStone, JsonRequestBehavior.AllowGet);
                    }

                    projectMileStone.ProjectId = project.ProjectId;
                    Session["_project_For_Milestone"] = null;
                }
                var wx = ValidateControl(projectMileStone);

                if (wx.Code < 1)
                {
                    projectMileStone.Error = wx.Error;
                    projectMileStone.ErrorCode = -1;
                    return Json(projectMileStone, JsonRequestBehavior.AllowGet);
                }

                oldProjectMileStone.ParentId = projectMileStone.ParentId;
                oldProjectMileStone.ProjectId = projectMileStone.ProjectId;
                oldProjectMileStone.Title = projectMileStone.Title;
                oldProjectMileStone.DateDue = projectMileStone.DateDue;
                oldProjectMileStone.LastUpdated = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss t");
                oldProjectMileStone.MileStoneStatusId = projectMileStone.MileStoneStatusId;
                projectMileStone.DateDueString = projectMileStone.DateDue.ToString("yyyy/MM/dd");
                var k = new ProjectMileStoneServices().UpdateProjectMileStoneCheckDuplicate(oldProjectMileStone);
                if (k < 1)
                {
                    projectMileStone.Error = "Process Failed! Please contact the Admin or try again later";
                    projectMileStone.ErrorCode = 0;
                    return Json(projectMileStone, JsonRequestBehavior.AllowGet);
                }
                
                    projectMileStone.Error = "Project MileStone  Information was successfully updated";
                    projectMileStone.ErrorCode = 1;
                    return Json(projectMileStone, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                projectMileStone.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                projectMileStone.ErrorCode = 0;
                return Json(projectMileStone, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [ActionName("DeleteProjectMilestone")]
        public ActionResult DeleteProject(int id)
        {
            var projectMileStone = new ProjectMileStone();

            try
            {
                if (id < 1)
                {
                    projectMileStone.Error = "Invalid Selection";
                    projectMileStone.ErrorCode = 0;
                    return Json(projectMileStone, JsonRequestBehavior.AllowGet);
                }
                if (new ProjectMileStoneServices().DeleteProjectMileStoneCheckReferences(id))
                {
                    projectMileStone.Error = "Project MileStone  Information was successfully deleted.";
                    projectMileStone.ErrorCode = 1;
                    return Json(projectMileStone, JsonRequestBehavior.AllowGet);
                }

                projectMileStone.Error = "Process Failed! Please try again later";
                projectMileStone.ErrorCode = 0;
                return Json(projectMileStone, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                projectMileStone.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                projectMileStone.ErrorCode = 0;
                return Json(projectMileStone, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult EditProjectMilestone(int id)
        {
            var projectMileStone = new ProjectMileStone();
            try
            {
                if (id < 1)
                {
                    projectMileStone.Error = "Invalid Selection!";
                    projectMileStone.ErrorCode = -1;
                    return Json(projectMileStone, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new ProjectMileStoneServices().GetProjectMileStone(id);

                if (myViewObj == null || myViewObj.ProjectId < 1)
                {
                    projectMileStone.Error = "Project MileStone  Information could not be retrieved.";
                    projectMileStone.ErrorCode = -1;
                    return Json(projectMileStone, JsonRequestBehavior.AllowGet);
                }
                if (myViewObj.DateDue != null)
                {
                    myViewObj.DateDueString = myViewObj.DateDue.ToString("yyyy/MM/dd");
                }
                Session["_projectMileStone"] = myViewObj;
                myViewObj.ErrorCode = 7;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                projectMileStone.Error = "An unknown error was ProjectMileStone  Information could not be retrieved.";
                projectMileStone.ErrorCode = -1;
                return Json(projectMileStone, JsonRequestBehavior.AllowGet);
            }
        }
        private static GenericValidator ValidateControl(ProjectMileStone model)
        {
            var gVal = new GenericValidator();

            try
            {
              
                if (string.IsNullOrEmpty(model.Title.Trim()))
                {
                    gVal.Error = "Please provide Project Milestone Title.";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.ProjectId < 1)
                {
                    gVal.Error = "Please select a valid Project.";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.MileStoneStatusId < 1)
                {
                    gVal.Error = "Please select a valid Milestone.";
                    gVal.Code = 0;
                    return gVal;
                }

                if (model.ParentId < 1)
                {
                    gVal.Error = "Please select a Parent Project Milestone Milestone.";
                    gVal.Code = 0;
                    return gVal;
                }
                

                if (string.IsNullOrEmpty(model.DateDue.ToString(CultureInfo.InvariantCulture)))
                {
                    gVal.Error = "Please provide Due Date.";
                    gVal.Code = 0;
                    return gVal;
                }
                
                gVal.Code = 1;
                return gVal;
            }
            catch (Exception )
            {
                gVal.Error = "Process validation failed. Please supply all required entries and try again.";
                gVal.Code = 0;
                return gVal;
            }
        }
        [HttpPost]
        public ActionResult SaveToFolder(ProjectMileStone projectMileStone, HttpPostedFileBase file)
        {
            int dataCount;
            try
            {
                if (projectMileStone.ProjectId < 1 || projectMileStone.MileStoneStatusId < 1)
                {
                    ViewBag.Title = "Manage Project Milestones";
                    ViewBag.ErrorCode = -1;
                    ViewBag.ErrorMessage = "Please select the required fields and try again.";
                    return View("~/Views/ProjectMileStone/ProjectMileStones.cshtml",
                        new ProjectMilestoneViewModel
                        {
                            MilestoneStatuses = GetMilestoneStatus(),
                            Projects = GetProjects(),
                            ProjectMileStones = GetProjectMileStones(ItemsPerPage, PageNumber, out dataCount)
                        });
                }

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
                    var mList = new List<ProjectMileStone>();
                    var msg = string.Empty;
                    if (
                        !new ProjectMilestoneUploadManager().Import(Server.MapPath(path), "projectmilestones", ref mList, ref msg))
                    {
                        ViewBag.ErrorCode = -1;
                        ViewBag.ErrorMessage = "Bulk upload Failed! An unknown error was encountered.";
                        return View("~/Views/ProjectMileStone/ProjectMileStones.cshtml",
                            new ProjectMilestoneViewModel
                            {
                                MilestoneStatuses = GetMilestoneStatus(),
                                Projects = GetProjects(),
                                CompletionStatuses = GetCompletionStatuses(),
                                ProjectMileStones = GetProjectMileStones(ItemsPerPage, PageNumber, out dataCount)

                            });
                    }

                    if (!mList.Any())
                    {
                        ViewBag.ErrorCode = -1;
                        ViewBag.ErrorMessage = msg.Length > 0 ? msg : "Bulk upload Failed! unknown error occurred"; 
                        return View("~/Views/ProjectMileStone/ProjectMileStones.cshtml",
                            new ProjectMilestoneViewModel
                            {
                                MilestoneStatuses = GetMilestoneStatus(),
                                Projects = GetProjects(),
                                CompletionStatuses = GetCompletionStatuses(),
                                ProjectMileStones = GetProjectMileStones(ItemsPerPage, PageNumber, out dataCount)
                            });
                    }

                    var errorList = new List<ProjectMileStone>();
                    foreach (var projectInfo in mList)
                    {
                        projectInfo.ParentId = (int)ProjectMilestoneNotAvailable.Not_Available;
                        projectInfo.ProjectId = projectMileStone.ProjectId;
                        projectInfo.LastUpdated = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss t");
                        projectInfo.MileStoneStatusId = projectMileStone.MileStoneStatusId;
                        projectMileStone.DateDueString = projectMileStone.DateDue.ToString("yyyy/MM/dd");
                        var processedItem = new ProjectMileStoneServices().AddProjectMileStoneCheckDuplicate(projectInfo);

                        if (processedItem < 1)
                        {
                            errorList.Add(projectInfo);
                        }
                    }

                    if (errorList.Any())
                    {
                        ViewBag.ErrorCode = -1;
                        ViewBag.ErrorMessage = "Bulk upload failed";
                        ViewBag.ErrorList = errorList;
                        return View("~/Views/ProjectMileStone/ProjectMileStones.cshtml",
                            new ProjectMilestoneViewModel
                            {
                                MilestoneStatuses = GetMilestoneStatus(),
                                Projects = GetProjects(),
                                CompletionStatuses = GetCompletionStatuses(),
                                ProjectMileStones = GetProjectMileStones(ItemsPerPage, PageNumber, out dataCount)
                            });
                    }

                    ViewBag.ErrorCode = 1;
                    ViewBag.ErrorMessage = "Bulk upload was successfully completed";
                    return View("~/Views/ProjectMileStone/ProjectMileStones.cshtml",
                        new ProjectMilestoneViewModel
                        {
                            MilestoneStatuses = GetMilestoneStatus(),
                            Projects = GetProjects(),
                            CompletionStatuses = GetCompletionStatuses(),
                            ProjectMileStones = GetProjectMileStones(ItemsPerPage, PageNumber, out dataCount)
                        });

                }

                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = "Bulk upload Failed! unknown error occurred";
                return View("~/Views/ProjectMileStone/ProjectMileStones.cshtml",
                    new ProjectMilestoneViewModel
                    {
                        MilestoneStatuses = GetMilestoneStatus(),
                        Projects = GetProjects(),
                        CompletionStatuses = GetCompletionStatuses(),
                        ProjectMileStones = GetProjectMileStones(ItemsPerPage, PageNumber, out dataCount)
                    });
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                ViewBag.ErrorCode = -1;
                ViewBag.ErrorMessage = "Bulk upload Failed! unknown error occurred";
                return View("~/Views/ProjectMileStone/ProjectMileStones.cshtml",
                    new ProjectMilestoneViewModel
                    {
                        MilestoneStatuses = GetMilestoneStatus(),
                        Projects = GetProjects(),
                        CompletionStatuses = GetCompletionStatuses(),
                        ProjectMileStones = GetProjectMileStones(ItemsPerPage, PageNumber, out dataCount)
                    });
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
        public ActionResult GetAllOrderedProjectMilestones(int projectId)
        {
            try
            {
               
                var myObjList = new ProjectServices().GetAllOrderedProjectsByTerrain(projectId);
                if (!myObjList.Any())
                {
                    return Json(new List<ProjectMileStone>(), JsonRequestBehavior.AllowGet);
                }

                return Json(myObjList, JsonRequestBehavior.AllowGet);
               
            }
            catch (Exception )
            {
                return Json(new List<ProjectMileStone>(), JsonRequestBehavior.AllowGet);
            }
        }
       

    }
}