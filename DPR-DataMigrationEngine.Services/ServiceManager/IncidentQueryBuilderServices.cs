using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{

    public class IncidentQueryBuilderServices
	{
        public List<IncidentQuery> GetAllOrderedIncidentQueries()
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    var myObjList = db.IncidentQueries.ToList();
                    if (!myObjList.Any())
                    {     
                        return new List<IncidentQuery>();
                    }
                    return myObjList.OrderBy(m => m.IncidentQueryName).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<IncidentQuery>();
            }
        }
        public List<IncidentQuery> GetIncidentQueries()
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    var myObjList = db.IncidentQueries.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<IncidentQuery>();
                    }
                    return myObjList.OrderBy(m => m.IncidentQueryName).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<IncidentQuery>();
            }
        }
        public long AddIncidentQueryCheckDuplicate(IncidentQuery incidentQuery)
        {
            try
            {
                if (incidentQuery == null)
                { return -2; }
                using (var db = new QueryBuilderEntities())
                {
                    if (db.IncidentQueries.Any())
                    {
                        if (db.IncidentQueries.Count(m => m.IncidentQueryName.ToLower().Trim().Replace(" ", "") == incidentQuery.IncidentQueryName.ToLower().Trim().Replace(" ", "")) > 0)
                        {
                            return -4;
                        }
                    }                                     

                  var processedIncidentQuery =  db.IncidentQueries.Add(incidentQuery);
                  db.SaveChanges();
                  return processedIncidentQuery.IncidentQueryId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public bool IsIncidentQueryExisting(IncidentQuery incidentQuery)
        {
            try
            {
                if (incidentQuery == null)
                { return false; }
                using (var db = new QueryBuilderEntities())
                {
                    if (db.IncidentQueries.Any())
                    {
                        if (db.IncidentQueries.Count(m => m.IncidentQueryName.ToLower().Trim().Replace(" ", "") == incidentQuery.IncidentQueryName.ToLower().Trim().Replace(" ", "")) > 0)
                        {
                            return true;
                        }

                        return false;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }
      public bool DeleteIncidentQueryCheckReferences(int incidentQueryId)
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    if (db.IncidentQueries.Count(m => m.IncidentQueryId == incidentQueryId) > 0)
                    {
                        return false;
                    }
                    
                    var myObj = db.IncidentQueries.Where(s => s.IncidentQueryId == incidentQueryId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.IncidentQueries.Remove(myObj[0]);
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
      public IncidentQuery GetIncidentQuery(int incidentQueryId)
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    var myObj = db.IncidentQueries.Where(s => s.IncidentQueryId == incidentQueryId).ToList();
                    if (!myObj.Any())
                    {
                        return new IncidentQuery();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new IncidentQuery();
            }
        }
       
	}
	
}
