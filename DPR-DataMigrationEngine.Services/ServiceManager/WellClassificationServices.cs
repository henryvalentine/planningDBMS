using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class WellClassificationServices
	{
        public List<WellClassification> GetAllOrderedWellClassifications()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.WellClassifications.Include("Well").Include("WellClass").ToList();
                    if (!myObjList.Any())
                    {
                        return new List<WellClassification>();
                    }
                    return myObjList.OrderBy(m => m.WellClassificationId).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellClassification>();
            }
        }
        public int AddWellClassificationCheckDuplicate(WellClassification wellClassification)
        {
            try
            {
                if (wellClassification == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.WellClassifications.Any())
                    {
                        if (db.WellClassifications.Count(m => m.WellClassId == wellClassification.WellClassId && m.WellId == wellClassification.WellId) > 0)
                        {
                            return -3;
                        }
                    }

                  var txx =  db.WellClassifications.Add(wellClassification);
                  db.SaveChanges();
                    return txx.WellClassificationId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateWellClassification(WellClassification wellClassification)
        {
            try
            {
                if (wellClassification == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                  var txx =  db.WellClassifications.Attach(wellClassification);
                   db.Entry(wellClassification).State = EntityState.Modified;
                   db.SaveChanges();
                    return txx.WellClassificationId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateWellClassification2(WellClassification wellClassification)
        {
            try
            {
                if (wellClassification == null || wellClassification.WellClassificationId < 1)
                {
                    return -2;
                }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var entityToUpdate = db.WellClassifications.ToList().Find(m => m.WellClassificationId == wellClassification.WellClassificationId);
                    if (entityToUpdate == null || entityToUpdate.WellClassificationId < 1)
                    {
                        return -2;
                    }

                    entityToUpdate.WellClassId = wellClassification.WellClassId;
                    entityToUpdate.WellId = wellClassification.WellId;
                    db.Entry(entityToUpdate).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public bool DeleteWellClassificationCheckReferences(int wellClassificationId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.WellClassifications.Where(s => s.WellClassificationId == wellClassificationId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.WellClassifications.Remove(myObj[0]);
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
        public WellClassification GetWellClassification(int wellClassificationId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.WellClassifications.Where(s => s.WellClassificationId == wellClassificationId).ToList();
                    if (!myObj.Any())
                    {
                        return new WellClassification();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new WellClassification();
            }
        }

        public string GetWellClassificationByWell(int wellId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.WellClassifications.Where(s => s.WellId == wellId).Include("WellClass").ToList();
                    if (!myObj.Any())
                    {
                        return string.Empty;
                    }

                    return myObj[0].WellClass.Name;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return string.Empty;
            }
        }
	}
	
}
