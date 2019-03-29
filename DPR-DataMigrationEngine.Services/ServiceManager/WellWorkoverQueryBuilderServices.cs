using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{

    public class WellWorkoverQueryBuilderServices
	{
        public List<WellWorkoverQuery> GetAllOrderedWellWorkoverQueries()
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    var myObjList = db.WellWorkoverQueries.ToList();
                    if (!myObjList.Any())
                    {     
                        return new List<WellWorkoverQuery>();
                    }
                    return myObjList.OrderBy(m => m.WellWorkoverQueryName).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellWorkoverQuery>();
            }
        }

        public List<WellWorkoverQuery> GetWellWorkoverQueries()
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    var myObjList = db.WellWorkoverQueries.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<WellWorkoverQuery>();
                    }
                    return myObjList.OrderBy(m => m.WellWorkoverQueryName).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellWorkoverQuery>();
            }
        }
        public long AddWellWorkoverQueryCheckDuplicate(WellWorkoverQuery wellWorkoverQuery)
        {
            try
            {
                if (wellWorkoverQuery == null)
                { return -2; }
                using (var db = new QueryBuilderEntities())
                {
                    if (db.WellWorkoverQueries.Any())
                    {
                        if (db.WellWorkoverQueries.Count(m => m.WellId == wellWorkoverQuery.WellId 
                            && m.CompanyId == wellWorkoverQuery.CompanyId
                            && m.WorkoverReasonId == wellWorkoverQuery.WorkoverReasonId
                            && (m.StartDate != null && m.StartDate.Value.Year == wellWorkoverQuery.StartDate.Value.Year && wellWorkoverQuery.StartDate != null)
                            && (m.EndDate != null && m.EndDate.Value.Year == wellWorkoverQuery.EndDate.Value.Year && wellWorkoverQuery.EndDate != null)
                            && m.EquipmentId == wellWorkoverQuery.EquipmentId
                            && m.TerrainId == wellWorkoverQuery.TerrainId
                            && m.ZoneId == wellWorkoverQuery.ZoneId
                            ) > 0)
                        {
                            return -3;
                        }

                        if (db.WellWorkoverQueries.Count(m => m.WellWorkoverQueryName.ToLower().Trim().Replace(" ", "") == wellWorkoverQuery.WellWorkoverQueryName.ToLower().Trim().Replace(" ", "")
                            ) > 0)
                        {
                            return -4;
                        }
                    }                                     

                  var processedWellWorkoverQuery =  db.WellWorkoverQueries.Add(wellWorkoverQuery);
                  db.SaveChanges();
                  return processedWellWorkoverQuery.WellWorkoverQueryId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public bool IsWellWorkoverQueryExisting(WellWorkoverQuery wellWorkoverQuery)
        {
            try
            {
                if (wellWorkoverQuery == null)
                { return false; }
                using (var db = new QueryBuilderEntities())
                {
                     if (db.WellWorkoverQueries.Count(m => m.WellId == wellWorkoverQuery.WellId 
                            && m.CompanyId == wellWorkoverQuery.CompanyId
                            && m.WorkoverReasonId == wellWorkoverQuery.WorkoverReasonId
                             && (m.StartDate != null && m.StartDate.Value.Year == wellWorkoverQuery.StartDate.Value.Year && wellWorkoverQuery.StartDate != null)
                            && (m.EndDate != null && m.EndDate.Value.Year == wellWorkoverQuery.EndDate.Value.Year && wellWorkoverQuery.EndDate != null)
                            && m.EquipmentId == wellWorkoverQuery.EquipmentId
                            && m.TerrainId == wellWorkoverQuery.TerrainId
                            && m.ZoneId == wellWorkoverQuery.ZoneId
                            ) > 0)
                        {
                            return true;
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
      public bool DeleteWellWorkoverQueryCheckReferences(int wellWorkoverQueryId)
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    if (db.WellWorkoverQueries.Count(m => m.WellWorkoverQueryId == wellWorkoverQueryId) > 0)
                    {
                        return false;
                    }
                    
                    var myObj = db.WellWorkoverQueries.Where(s => s.WellWorkoverQueryId == wellWorkoverQueryId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.WellWorkoverQueries.Remove(myObj[0]);
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
      public WellWorkoverQuery GetWellWorkoverQuery(int wellWorkoverQueryId)
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    var myObj = db.WellWorkoverQueries.Where(s => s.WellWorkoverQueryId == wellWorkoverQueryId).ToList();
                    if (!myObj.Any())
                    {
                        return new WellWorkoverQuery();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new WellWorkoverQuery();
            }
        }
       
	}
	
}
