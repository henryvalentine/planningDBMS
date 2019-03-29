using System;
using System.Collections.Generic;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{

    public class FieldQueryBuilderServices
	{
        public List<FieldQuery> GetAllOrderedFieldQueries()
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    var myObjList = db.FieldQueries.ToList();
                    if (!myObjList.Any())
                    {     
                        return new List<FieldQuery>();
                    }
                    return myObjList.OrderBy(m => m.FieldQueryName).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<FieldQuery>();
            }
        }
        public List<FieldQuery> GetFieldQueries()
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    var myObjList = db.FieldQueries.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<FieldQuery>();
                    }
                    return myObjList.OrderBy(m => m.FieldQueryName).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<FieldQuery>();
            }
        }
        public long AddFieldQueryCheckDuplicate(FieldQuery fieldQuery)
        {
            try
            {
                if (fieldQuery == null)
                { return -2; }
                using (var db = new QueryBuilderEntities())
                {
                    if (db.FieldQueries.Any())
                    {
                        if (db.FieldQueries.Count(m => m.ZoneId == fieldQuery.ZoneId 
                            && m.CompanyId == fieldQuery.CompanyId
                            && m.TerrainId == fieldQuery.TerrainId
                            && m.ZoneId == fieldQuery.ZoneId
                            ) > 0)
                        {
                            return -3;
                        }

                        if (db.FieldQueries.Count(m => m.FieldQueryName.ToLower().Trim().Replace(" ", "") == fieldQuery.FieldQueryName.ToLower().Trim().Replace(" ", "")
                           ) > 0)
                        {
                            return -4;
                        }
                    }                                     

                  var processedFieldQuery =  db.FieldQueries.Add(fieldQuery);
                  db.SaveChanges();
                  return processedFieldQuery.FieldQueryId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public bool IsFieldQueryExisting(FieldQuery fieldQuery)
        {
            try
            {
                if (fieldQuery == null)
                { return false; }
                using (var db = new QueryBuilderEntities())
                {
                    if (db.FieldQueries.Any())
                    {
                        if (db.FieldQueries.Count(m => m.ZoneId == fieldQuery.ZoneId
                           && m.CompanyId == fieldQuery.CompanyId
                           && m.TerrainId == fieldQuery.TerrainId
                           && m.ZoneId == fieldQuery.ZoneId
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
      public bool DeleteFieldQueryCheckReferences(int fieldQueryId)
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    if (db.FieldQueries.Count(m => m.FieldQueryId == fieldQueryId) > 0)
                    {
                        return false;
                    }
                    
                    var myObj = db.FieldQueries.Where(s => s.FieldQueryId == fieldQueryId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.FieldQueries.Remove(myObj[0]);
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
      public FieldQuery GetFieldQuery(int fieldQueryId)
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    var myObj = db.FieldQueries.Where(s => s.FieldQueryId == fieldQueryId).ToList();
                    if (!myObj.Any())
                    {
                        return new FieldQuery();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new FieldQuery();
            }
        }
       
	}
	
}
