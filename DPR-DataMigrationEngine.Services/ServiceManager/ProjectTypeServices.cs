using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
    public class ProjectTypeServices
	{
        public List<ProjectType> GetAllOrderedProjectTypes()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.ProjectTypes.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<ProjectType>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<ProjectType>();
            }
        }
        public int AddProjectTypeCheckDuplicate(ProjectType projectType)
        {
            try
            {
                if (projectType == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.ProjectTypes.Any())
                    {
                        if (db.ProjectTypes.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == projectType.Name.ToLower().Replace(" ", string.Empty)) > 0)
                        {
                            return -3;
                        }
                    }

                    var processedItem = db.ProjectTypes.Add(projectType);
                    db.SaveChanges();
                    return processedItem.ProjectTypeId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateProjectTypeCheckDuplicate(ProjectType projectType)
        {
            try
            {
                if (projectType == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.ProjectTypes.Any())
                    {
                        if (db.ProjectTypes.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == projectType.Name.ToLower().Replace(" ", string.Empty) && m.ProjectTypeId != projectType.ProjectTypeId) > 0)
                        {
                            return -3;
                        }
                    }

                    db.ProjectTypes.Attach(projectType);
                    db.Entry(projectType).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public ProjectType GetProjectType(int projectTypeId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.ProjectTypes.Where(s => s.ProjectTypeId == projectTypeId).ToList();
                    if (!myObj.Any())
                    {
                        return new ProjectType();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new ProjectType();
            }
        }

        public int GetProjectTypeId(string projectTypeName)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.ProjectTypes.Where(s => s.Name.ToLower().Trim().Replace(" ", string.Empty) == projectTypeName.ToLower().Trim().Replace(" ", string.Empty)).ToList();
                    if (!myObj.Any())
                    {

                        return 0;
                    }
                    var ttd = myObj[0];
                    return ttd.ProjectTypeId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public bool DeleteProjectTypeCheckReferences(int projectTypeId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Projects.Count(m => m.ProjectTypeId == projectTypeId) > 0)
                    {
                        return false;
                    }

                    var myObj = db.ProjectTypes.Where(s => s.ProjectTypeId == projectTypeId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.ProjectTypes.Remove(myObj[0]);
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
	}


	
}
