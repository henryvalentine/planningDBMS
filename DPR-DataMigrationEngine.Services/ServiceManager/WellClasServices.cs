using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class WellClasServices
	{
        public List<WellClass> GetAllOrderedWellClasses()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.WellClasses.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<WellClass>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellClass>();
            }
        }

        public List<WellClass> GetAllOrderedWellClasses2()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.WellClasses.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<WellClass>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellClass>();
            }
        }
        public int AddWellClassCheckDuplicate(WellClass wellClass)
        {
            try
            {
                if (wellClass == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.WellClasses.Any())
                    {
                        if (db.WellClasses.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == wellClass.Name.ToLower().Replace(" ", string.Empty)) > 0)
                        {
                            return -3;
                        }
                    }

                    db.WellClasses.Add(wellClass);
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateWellClassCheckDuplicate(WellClass wellClass)
        {
            try
            {
                if (wellClass == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.WellClasses.Any())
                    {
                        if (db.WellClasses.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == wellClass.Name.ToLower().Replace(" ", string.Empty) && m.WellClassId != wellClass.WellClassId) > 0)
                        {
                            return -3;
                        }
                    }

                    db.WellClasses.Attach(wellClass);
                    db.Entry(wellClass).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public bool DeleteWellClassCheckReferences(int wellClassId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.WellClasses.Count(m => m.WellClassId == wellClassId) > 0)
                    {
                        return false;
                    }

                    var myObj = db.WellClasses.Where(s => s.WellClassId == wellClassId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.WellClasses.Remove(myObj[0]);
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

        public WellClass GetWellClass(int wellClassId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.WellClasses.Where(s => s.WellClassId == wellClassId).ToList();
                    if (!myObj.Any())
                    {
                        return new WellClass();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new WellClass();
            }
        }

        public int CreateWellClassAddClassification(string wellClassName, int wellId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.WellClasses.Where(s => s.Name.ToLower().Replace(" ", string.Empty).Trim() == wellClassName.ToLower().Replace(" ", string.Empty).Trim()).ToList();
                    if (!myObj.Any())
                    {
                        var wellClass = new WellClass { Name = wellClassName.Trim() };
                        var processedWellClass = db.WellClasses.Add(wellClass);
                        db.SaveChanges();
                        if (processedWellClass.WellClassId > 0)
                        {
                            var newWellClassification = new WellClassification
                            {
                                WellClassId = processedWellClass.WellClassId,
                                WellId = wellId
                            };

                            var processedWellClassification = new WellClassificationServices().AddWellClassificationCheckDuplicate(newWellClassification);
                            return processedWellClassification;
                        }
                    }

                     var edt = new WellClassification
                     {
                         WellClassId = myObj[0].WellClassId,
                         WellId = wellId
                     };

                     var dsx = new WellClassificationServices().AddWellClassificationCheckDuplicate(edt);
                     return dsx;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

    }
}
