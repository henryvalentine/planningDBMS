using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class ProjectMileStoneServices
	{
        public List<ProjectMileStone> GetAllOrderedProjectMileStones(int itemsPerPage, int pageNumber, out int dataCount)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.ProjectMileStones.Any())
                    {
                        dataCount = db.ProjectMileStones.Count();
                        var myObjList = db.ProjectMileStones.OrderBy(m => m.Title).Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).Include("Project").Include("MilestoneStatu").ToList();
                        if (!myObjList.Any())
                        {
                            return new List<ProjectMileStone>();
                        }
                        return myObjList;
                    }

                    dataCount = 0;
                    return new List<ProjectMileStone>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                dataCount = 0;
                return new List<ProjectMileStone>();
            }
        }

        public List<ProjectMilestoneObject> GetProjectMileStones()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.ProjectMileStones.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<ProjectMilestoneObject>();
                    }
                    var newList = new List<ProjectMilestoneObject>();
                    myObjList.ForEach(m => newList.Add(new ProjectMilestoneObject
                    {
                        ProjectMileStoneId = m.ProjectMileStoneId,
                        Title = m.Title
                    }));
                    return newList.OrderBy(m => m.Title).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<ProjectMilestoneObject>();
            }
        }
        public long AddProjectMileStoneCheckDuplicate(ProjectMileStone projectMileStone)
        {
            try
            {
                if (projectMileStone == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                  var tgr =  db.ProjectMileStones.Add(projectMileStone);
                   db.SaveChanges();
                    return tgr.ProjectMileStoneId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateProjectMileStoneCheckDuplicate(ProjectMileStone projectMileStone)
        {
            try
            {
                if (projectMileStone == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    db.ProjectMileStones.Attach(projectMileStone);
                    db.Entry(projectMileStone).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public bool DeleteProjectMileStoneCheckReferences(int projectMileStoneId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.ProjectMileStones.Where(s => s.ProjectMileStoneId == projectMileStoneId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.ProjectMileStones.Remove(myObj[0]);
                    var txx = db.SaveChanges();
                    return txx > 0;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }
        public ProjectMileStone GetProjectMileStone(long projectMilestoneId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.ProjectMileStones.Where(m => m.ProjectMileStoneId == projectMilestoneId).ToList();
                    if (!myObjList.Any())
                    {
                        return new ProjectMileStone();
                    }
                    return myObjList[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new ProjectMileStone();
            }
        }
        public List<ProjectMileStone> GetAllOrderedProjectMileStonesByProject(long projectId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.ProjectMileStones.Where(m => m.ProjectId == projectId).Include("Project").Include("MilestoneStatu").ToList();
                    if (!myObjList.Any())
                    {
                        return new List<ProjectMileStone>();
                    }
                    return myObjList.OrderBy(m => m.Title).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<ProjectMileStone>();
            }
        }
	}
	
}
