using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{

    public class ProductionQueryBuilderServices
	{
        public List<ProductionQuery> GetAllOrderedProductionQueries()
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    var myObjList = db.ProductionQueries.ToList();
                    if (!myObjList.Any())
                    {     
                        return new List<ProductionQuery>();
                    }
                    return myObjList.OrderBy(m => m.ProductionQueryName).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<ProductionQuery>();
            }
        }

        public List<ProductionQuery> GetProductionQueries()
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    var myObjList = db.ProductionQueries.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<ProductionQuery>();
                    }
                    return myObjList.OrderBy(m => m.ProductionQueryName).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<ProductionQuery>();
            }
        }
        public long AddProductionQueryCheckDuplicate(ProductionQuery productionQuery)
        {
            try
            {
                if (productionQuery == null)
                { return -2; }
                using (var db = new QueryBuilderEntities())
                {
                    if (db.ProductionQueries.Any())
                    {
                        if (db.ProductionQueries.Count(m => (m.ProductId == productionQuery.ProductId 
                            && m.CompanyId == productionQuery.CompanyId 
                            && m.FieldId == productionQuery.FieldId
                            && (m.StartDate != null && m.StartDate.Value.Year == productionQuery.StartDate.Value.Year && productionQuery.StartDate != null)
                            && (m.EndDate != null && m.EndDate.Value.Year == productionQuery.EndDate.Value.Year && productionQuery.EndDate != null)
                            && m.BlockId == productionQuery.BlockId)
                            ) > 0)
                        {
                            return -3;
                        }

                        if (db.ProductionQueries.Count(m => m.ProductionQueryName.ToLower().Trim().Replace(" ", "") == productionQuery.ProductionQueryName.ToLower().Trim().Replace(" ", "")
                           ) > 0)
                        {
                            return -4;
                        }
                    }                                     

                  var processedProductionQuery =  db.ProductionQueries.Add(productionQuery);
                  db.SaveChanges();
                  return processedProductionQuery.ProductionQueryId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public bool IsProductionQueryExisting(ProductionQuery productionQuery)
        {
            try
            {
                if (productionQuery == null)
                { return false; }
                using (var db = new QueryBuilderEntities())
                {
                    if (db.ProductionQueries.Any())
                    {
                        if (db.ProductionQueries.Count(m => m.ProductId == productionQuery.ProductId
                            && m.CompanyId == productionQuery.CompanyId
                            && m.FieldId == productionQuery.FieldId
                            && (m.StartDate != null && m.StartDate.Value.Year == productionQuery.StartDate.Value.Year && productionQuery.StartDate != null)
                            && (m.EndDate != null && m.EndDate.Value.Year == productionQuery.EndDate.Value.Year && productionQuery.EndDate != null)
                            && m.BlockId == productionQuery.BlockId
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
      public bool DeleteProductionQueryCheckReferences(int productionQueryId)
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    if (db.ProductionQueries.Count(m => m.ProductionQueryId == productionQueryId) > 0)
                    {
                        return false;
                    }
                    
                    var myObj = db.ProductionQueries.Where(s => s.ProductionQueryId == productionQueryId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.ProductionQueries.Remove(myObj[0]);
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
      public ProductionQuery GetProductionQuery(int productionQueryId)
        {
            try
            {
                using (var db = new QueryBuilderEntities())
                {
                    var myObj = db.ProductionQueries.Where(s => s.ProductionQueryId == productionQueryId).ToList();
                    if (!myObj.Any())
                    {
                        return new ProductionQuery();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new ProductionQuery();
            }
        }
       
	}
	
}
