using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
    public class WellCompletionTypeServices
	{
        public List<WellCompletionType> GetAllOrderedWellCompletionTypes()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.WellCompletionTypes.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<WellCompletionType>();
                    }
                    return myObjList.OrderBy(m => m.Type).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellCompletionType>();
            }
        }
        public int AddWellCompletionTypeCheckDuplicate(WellCompletionType wellCompletionType)
        {
            try
            {
                if (wellCompletionType == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.WellCompletionTypes.Any())
                    {
                        if (db.WellCompletionTypes.Count(m => m.Type.ToLower().Replace(" ", string.Empty) == wellCompletionType.Type.ToLower().Replace(" ", string.Empty)) > 0)
                        {
                            return -3;
                        }
                    }

                    db.WellCompletionTypes.Add(wellCompletionType);
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateWellCompletionTypeCheckDuplicate(WellCompletionType wellCompletionType)
        {
            try
            {
                if (wellCompletionType == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.WellCompletionTypes.Any())
                    {
                        if (db.WellCompletionTypes.Count(m => m.Type.ToLower().Replace(" ", string.Empty) == wellCompletionType.Type.ToLower().Replace(" ", string.Empty) && m.WellCompletionTypeId != wellCompletionType.WellCompletionTypeId) > 0)
                        {
                            return -3;
                        }
                    }

                    db.WellCompletionTypes.Attach(wellCompletionType);
                    db.Entry(wellCompletionType).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public bool DeleteWellCompletionTypeCheckReferences(int wellCompletionTypeId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.WellCompletions.Count(m => m.WellCompletionTypeId == wellCompletionTypeId) > 0)
                    {
                        return false;
                    }

                    var myObj = db.WellCompletionTypes.Where(s => s.WellCompletionTypeId == wellCompletionTypeId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.WellCompletionTypes.Remove(myObj[0]);
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

        public WellCompletionType GetWellCompletionType(int wellCompletionTypeId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.WellCompletionTypes.Where(s => s.WellCompletionTypeId == wellCompletionTypeId).ToList();
                    if (!myObj.Any())
                    {
                        return new WellCompletionType();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new WellCompletionType();
            }
        }
	}


	
}
