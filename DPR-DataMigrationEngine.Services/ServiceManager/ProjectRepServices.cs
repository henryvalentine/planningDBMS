using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class ProjectRepServices
	{
        public List<ProjectRep> GetAllOrderedProjectReps()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.ProjectReps.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<ProjectRep>();
                    }
                    return myObjList.OrderBy(m => m.ProjectRepId).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<ProjectRep>();
            }
        }
        public int AddProjectRepCheckDuplicate(ProjectRep field)
        {
            try
            {
                if (field == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    db.ProjectReps.Add(field);
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateProjectRepCheckDuplicate(ProjectRep field)
        {
            try
            {
                if (field == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    db.ProjectReps.Attach(field);
                    db.Entry(field).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public bool DeleteProjectRepCheckReferences(int fieldId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.ProjectReps.Where(s => s.ProjectRepId == fieldId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.ProjectReps.Remove(myObj[0]);
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
