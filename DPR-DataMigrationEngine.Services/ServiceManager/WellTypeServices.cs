using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class WellTypeServices
	{
        public List<WellType> GetAllOrderedWellTypes()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.WellTypes.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<WellType>();
                    }
                    return myObjList.OrderBy(m => m.Title).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellType>();
            }
        }
        public int AddWellTypeCheckDuplicate(WellType wellType)
        {
            try
            {
                if (wellType == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.WellTypes.Any())
                    {
                        if (db.WellTypes.Count(m => m.Title.ToLower().Replace(" ", string.Empty) == wellType.Title.ToLower().Replace(" ", string.Empty)) > 0)
                        {
                            return -3;
                        }
                    }

                    db.WellTypes.Add(wellType);
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateWellTypeCheckDuplicate(WellType wellType)
        {
            try
            {
                if (wellType == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.WellTypes.Any())
                    {
                        if (db.WellTypes.Count(m => m.Title.ToLower().Replace(" ", string.Empty) == wellType.Title.ToLower().Replace(" ", string.Empty) && m.WellTypeId != wellType.WellTypeId) > 0)
                        {
                            return -3;
                        }
                    }

                    db.WellTypes.Attach(wellType);
                    db.Entry(wellType).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public bool DeleteWellTypeCheckReferences(int wellTypeId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Wells.Count(m => m.WellTypeId == wellTypeId) > 0)
                    {
                        return false;
                    }

                    var myObj = db.WellTypes.Where(s => s.WellTypeId == wellTypeId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.WellTypes.Remove(myObj[0]);
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

        public WellType GetWellType(int wellTypeId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.WellTypes.Where(s => s.WellTypeId == wellTypeId).ToList();
                    if (!myObj.Any())
                    {
                        return new WellType();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new WellType();
            }
        }

        public int GetWellTypeId(string wellTypeName)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.WellTypes.Where(s => s.Title.ToLower().Trim().Replace(" ", string.Empty) == wellTypeName.ToLower().Trim().Replace(" ", string.Empty)).ToList();
                    if (!myObj.Any())
                    {
                        var wellType = new WellType { Title = wellTypeName.Trim()};
                        var processedWellType = db.WellTypes.Add(wellType);
                        db.SaveChanges();
                        return processedWellType.WellTypeId;
                    }
                    var ttd = myObj[0];
                    return ttd.WellTypeId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0; ;
            }
        }
	}
	
}
