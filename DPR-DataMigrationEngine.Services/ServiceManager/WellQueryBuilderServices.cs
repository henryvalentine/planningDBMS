using System;
using System.Collections.Generic;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{

    public class WellQueryBuilderServices
	{
        public List<WellQuery> GetAllOrderedWellQueries()
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    var myObjList = db.WellQueries.ToList();
                    if (!myObjList.Any())
                    {     
                        return new List<WellQuery>();
                    }
                    return myObjList.OrderBy(m => m.WellQueryName).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellQuery>();
            }
        }

        public List<WellQuery> GetWellQueries()
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    var myObjList = db.WellQueries.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<WellQuery>();
                    }
                    return myObjList.OrderBy(m => m.WellQueryName).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellQuery>();
            }
        }
        public long AddWellQueryCheckDuplicate(WellQuery wellQuery)
        {
            try
            {
                if (wellQuery == null)
                { return -2; }
                using (var db = new QueryBuilderEntities())
                {
                    if (db.WellQueries.Any())
                    {
                        if (db.WellQueries.Count(m => m.WellTypeId == wellQuery.WellTypeId && m.CompanyId == wellQuery.CompanyId 
                            && m.FieldId == wellQuery.FieldId
                            && (m.StartDate != null && m.StartDate.Value.Year == wellQuery.StartDate.Value.Year && wellQuery.StartDate != null)
                            && (m.EndDate != null && m.EndDate.Value.Year == wellQuery.EndDate.Value.Year && wellQuery.EndDate != null)
                            && m.WellClassId == wellQuery.WellClassId
                            && m.TerrainId == wellQuery.TerrainId
                             && m.BlockId == wellQuery.TerrainId
                            && m.ZoneId == wellQuery.ZoneId
                            ) > 0)
                        {
                            return -3;
                        }

                        if (db.WellQueries.Count(m => m.WellQueryName.ToLower().Trim().Replace(" ", "") == wellQuery.WellQueryName.ToLower().Trim().Replace(" ", "")
                            ) > 0)
                        {
                            return -4;
                        }
                    }                                     

                  var processedWellQuery =  db.WellQueries.Add(wellQuery);
                   db.SaveChanges();
                    return processedWellQuery.WellQueryId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public bool IsWellQueryExisting(WellQuery wellQuery)
        {
            try
            {
                if (wellQuery == null)
                { return false; }
                using (var db = new QueryBuilderEntities())
                {
                    if (db.WellQueries.Any())
                    {
                        if (db.WellQueries.Count(m => m.WellTypeId == wellQuery.WellTypeId && m.CompanyId == wellQuery.CompanyId
                            && m.FieldId == wellQuery.FieldId
                            && (m.StartDate != null && m.StartDate.Value.Year == wellQuery.StartDate.Value.Year && wellQuery.StartDate != null)
                            && (m.EndDate != null && m.EndDate.Value.Year == wellQuery.EndDate.Value.Year && wellQuery.EndDate != null)
                            && m.WellClassId == wellQuery.WellClassId
                            && m.TerrainId == wellQuery.TerrainId
                            && m.ZoneId == wellQuery.ZoneId
                            ) > 0)
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
      public bool DeleteWellQueryCheckReferences(int wellQueryId)
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    if (db.WellQueries.Count(m => m.WellQueryId == wellQueryId) > 0)
                    {
                        return false;
                    }
                    
                    var myObj = db.WellQueries.Where(s => s.WellQueryId == wellQueryId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.WellQueries.Remove(myObj[0]);
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
      public WellQuery GetWellQuery(int wellQueryId)
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    var myObj = db.WellQueries.Where(s => s.WellQueryId == wellQueryId).ToList();
                    if (!myObj.Any())
                    {
                        return new WellQuery();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new WellQuery();
            }
        }
       
	}
	
}
