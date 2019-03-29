using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;

namespace DPR_DataMigrationEngine.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class ProjectTypeController : Controller
    {
        public ProjectTypeController()
		{
			 ViewBag.LoadStatus = "0";
		}
        public ViewResult ProjectTypes()
        {

            var projectTypeList = new ProjectTypeServices().GetAllOrderedProjectTypes() ?? new List<ProjectType>();

            if (!projectTypeList.Any())
            {
                ViewBag.Title = "Project Type SetUp";
                return View(projectTypeList);
            }
            projectTypeList.Remove(projectTypeList.Find(m => m.ProjectTypeId == (int)OtherNotAvailable.Not_Available));
            ViewBag.Title = "Manage Project Types";
            return View(projectTypeList);
        }
        
        [HttpPost]
        public ActionResult AddProjectType(ProjectType projectType)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    projectType.Error = "Please supply all required fields and try again";
                    projectType.ErrorCode = -1;
                    return Json(projectType, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(projectType);

                if (wx.Code < 1)
                {
                    projectType.Error = wx.Error;
                    projectType.ErrorCode = -1;
                    return Json(projectType, JsonRequestBehavior.AllowGet);
                }

                projectType.Name = projectType.Name;
                var k = new ProjectTypeServices().AddProjectTypeCheckDuplicate(projectType);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        projectType.Error = "Project Type already exists";
                        projectType.ErrorCode = -3;
                        return Json(projectType, JsonRequestBehavior.AllowGet);
                    }

                    projectType.Error = "Process Failed! Please contact the Admin or try again later";
                    projectType.ErrorCode = 0;
                    return Json(projectType, JsonRequestBehavior.AllowGet);
                }

                projectType.Error = "Record was added successfully";
                projectType.ErrorCode = 1;
                projectType.ProjectTypeId = k;
                return Json(projectType, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                projectType.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                projectType.ErrorCode = 0;
                return Json(projectType, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditProjectType(ProjectType projectType)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_projectType"] == null)
                {
                    projectType.Error = "Session has expired";
                    projectType.ErrorCode = 0;
                    return Json(projectType, JsonRequestBehavior.AllowGet);
                }

                var oldProjectType = Session["_projectType"] as ProjectType;

                if (oldProjectType == null || oldProjectType.ProjectTypeId < 1)
                {
                    projectType.Error = "Session has expired";
                    projectType.ErrorCode = 0;
                    return Json(projectType, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    projectType.Error = "Please supply all required fields and try again";
                    projectType.ErrorCode = -1;
                    return Json(projectType, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(projectType);

                if (wx.Code < 1)
                {
                    projectType.Error = wx.Error;
                    projectType.ErrorCode = -1;
                    return Json(projectType, JsonRequestBehavior.AllowGet);
                }

                oldProjectType.Name = projectType.Name;
                oldProjectType.Description = projectType.Description;

                var k = new ProjectTypeServices().UpdateProjectTypeCheckDuplicate(oldProjectType);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        projectType.Error = "Project Type already exists";
                        projectType.ErrorCode = 0;
                        return Json(projectType, JsonRequestBehavior.AllowGet);
                    }

                    projectType.Error = "Process Failed! Please contact the Admin or try again later";
                    projectType.ErrorCode = 0;
                    return Json(projectType, JsonRequestBehavior.AllowGet);
                }
                
                    projectType.Error = "Project Type Information was successfully updated";
                    projectType.ErrorCode = 1;
                    return Json(projectType, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                projectType.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                projectType.ErrorCode = 0;
                return Json(projectType, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("DeleteProjectType")]
        public ActionResult DeleteProjectType(int id)
        {
            var projectType = new ProjectType();

            try
            {
                if (id < 1)
                {
                    projectType.Error = "Invalid Selection";
                    projectType.ErrorCode = 0;
                    return Json(projectType, JsonRequestBehavior.AllowGet);
                }
                if (new ProjectTypeServices().DeleteProjectTypeCheckReferences(id))
                {
                    projectType.Error = "Project Type Information was successfully deleted.";
                    projectType.ErrorCode = 1;
                    return Json(projectType, JsonRequestBehavior.AllowGet);
                }

                projectType.Error = "Process Failed! Please try again later";
                projectType.ErrorCode = 0;
                return Json(projectType, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                projectType.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                projectType.ErrorCode = 0;
                return Json(projectType, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditProjectType(int id)
        {
            var projectType = new ProjectType();
            try
            {
                if (id < 1)
                {
                    projectType.Error = "Invalid Selection!";
                    projectType.ErrorCode = -1;
                    return Json(projectType, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new ProjectTypeServices().GetProjectType(id);

                if (myViewObj == null || myViewObj.ProjectTypeId < 1)
                {
                    projectType.Error = "Project Type Information could not be retrieved.";
                    projectType.ErrorCode = -1;
                    return Json(projectType, JsonRequestBehavior.AllowGet);
                }
                Session["_projectType"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.ProjectTypeId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                projectType.Error = "An unknown error was ProjectType Information could not be retrieved.";
                projectType.ErrorCode = -1;
                return Json(projectType, JsonRequestBehavior.AllowGet);
            }
        }

        private static GenericValidator ValidateControl(ProjectType model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.Name.Trim()))
                {
                    gVal.Error = "Please enter Project Type Name.";
                    gVal.Code = 0;
                    return gVal;
                }
                //if (string.IsNullOrEmpty(model.Description.Trim()))
                //{
                //    gVal.Error = "Please provide Incident Description.";
                //    gVal.Code = 0;
                //    return gVal;
                //}
               
                gVal.Code = 1;
                return gVal;
            }
            catch (Exception )
            {
                gVal.Error = "Process validation failed. Please supply all required fields and try again.";
                gVal.Code = 0;
                return gVal;
            }
        }

        public int GetLoggedOnUserId()
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser != null)
            {
                return Convert.ToInt32(membershipUser.ProviderUserKey);
            }
            return 0;
        }
    }
}