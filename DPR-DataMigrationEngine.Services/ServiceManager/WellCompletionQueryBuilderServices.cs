using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{

    public class WellCompletionQueryBuilderServices
	{
        public List<WellCompletionQuery> GetAllOrderedWellCompletionQueries()
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    var myObjList = db.WellCompletionQueries.ToList();
                    if (!myObjList.Any())
                    {     
                        return new List<WellCompletionQuery>();
                    }
                    return myObjList.OrderBy(m => m.WellCompletionQueryName).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellCompletionQuery>();
            }
        }

        public List<WellCompletionQuery> GetWellCompletionQueries()
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    var myObjList = db.WellCompletionQueries.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<WellCompletionQuery>();
                    }
                    return myObjList.OrderBy(m => m.WellCompletionQueryName).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellCompletionQuery>();
            }
        }
        public long AddWellCompletionQueryCheckDuplicate(WellCompletionQuery wellCompletionQuery)
        {
            try
            {
                if (wellCompletionQuery == null)
                { return -2; }
                using (var db = new QueryBuilderEntities())
                {
                    if (db.WellCompletionQueries.Any())
                    {
                        if (db.WellCompletionQueries.Count(m => m.WellId == wellCompletionQuery.WellId
                            && m.CompanyId == wellCompletionQuery.CompanyId
                           && (m.StartDate != null && m.StartDate.Value.Year == wellCompletionQuery.StartDate.Value.Year && wellCompletionQuery.StartDate != null)
                            && (m.EndDate != null && m.EndDate.Value.Year == wellCompletionQuery.EndDate.Value.Year && wellCompletionQuery.EndDate != null)
                            && m.EquipmentId == wellCompletionQuery.EquipmentId
                            && m.TerrainId == wellCompletionQuery.TerrainId
                            && m.ZoneId == wellCompletionQuery.ZoneId
                            && m.CompletionTypeId == wellCompletionQuery.CompletionTypeId
                            && m.WellClassId == wellCompletionQuery.WellClassId
                            && m.WellTypeId == wellCompletionQuery.WellTypeId
                            ) > 0)
                        {
                            return -3;
                        }

                        if (db.WellCompletionQueries.Count(m => m.WellCompletionQueryName.ToLower().Trim().Replace(" ", "") == wellCompletionQuery.WellCompletionQueryName.ToLower().Trim().Replace(" ", "")
                            ) > 0)
                        {
                            return -4;
                        }
                    }                                     

                  var processedWellCompletionQuery =  db.WellCompletionQueries.Add(wellCompletionQuery);
                  db.SaveChanges();
                  return processedWellCompletionQuery.WellCompletionQueryId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public bool IsWellCompletionQueryExisting(WellCompletionQuery wellCompletionQuery)
        {
            try
            {
                if (wellCompletionQuery == null)
                { return false; }
                using (var db = new QueryBuilderEntities())
                {
                     if (db.WellCompletionQueries.Count(m => 
                            m.WellId == wellCompletionQuery.WellId 
                            && m.CompanyId == wellCompletionQuery.CompanyId
                            && (m.StartDate != null && m.StartDate.Value.Year == wellCompletionQuery.StartDate.Value.Year && wellCompletionQuery.StartDate != null)
                            && (m.EndDate != null && m.EndDate.Value.Year == wellCompletionQuery.EndDate.Value.Year && wellCompletionQuery.EndDate != null)
                            && m.EquipmentId == wellCompletionQuery.EquipmentId
                            && m.TerrainId == wellCompletionQuery.TerrainId
                            && m.ZoneId == wellCompletionQuery.ZoneId 
                            && m.CompletionTypeId == wellCompletionQuery.CompletionTypeId
                            && m.WellClassId == wellCompletionQuery.WellClassId
                            && m.WellTypeId == wellCompletionQuery.WellTypeId
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
      public bool DeleteWellCompletionQueryCheckReferences(int wellCompletionQueryId)
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    if (db.WellCompletionQueries.Count(m => m.WellCompletionQueryId == wellCompletionQueryId) > 0)
                    {
                        return false;
                    }
                    
                    var myObj = db.WellCompletionQueries.Where(s => s.WellCompletionQueryId == wellCompletionQueryId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.WellCompletionQueries.Remove(myObj[0]);
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
      public WellCompletionQuery GetWellCompletionQuery(int wellCompletionQueryId)
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    var myObj = db.WellCompletionQueries.Where(s => s.WellCompletionQueryId == wellCompletionQueryId).ToList();
                    if (!myObj.Any())
                    {
                        return new WellCompletionQuery();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new WellCompletionQuery();
            }
        }
       
	}
	
}
