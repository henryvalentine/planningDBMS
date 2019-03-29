using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class WellWorkOverReasonServices
	{
        public List<WellWorkOverReason> GetAllOrderedWellWorkOverReasons()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.WellWorkOverReasons.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<WellWorkOverReason>();
                    }
                    return myObjList.OrderBy(m => m.Title).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellWorkOverReason>();
            }
        }
        public int AddWellWorkOverReason(WellWorkOverReason wellWorkOverReason)
        {
            try
            {
                if (wellWorkOverReason == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.WellWorkOverReasons.Where(s => s.Title.Trim().ToLower().Replace(" ", "") == wellWorkOverReason.Title.Trim().ToLower().Replace(" ", "")).ToList();
                    if (!myObj.Any())
                    {
                      return -3;
                    }
                    var processedItem = db.WellWorkOverReasons.Add(wellWorkOverReason);
                    db.SaveChanges();
                    return processedItem.WellWorkOverReasonId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateWellWorkOverReason(WellWorkOverReason wellWorkOverReason)
        {
            try
            {
                if (wellWorkOverReason == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    db.WellWorkOverReasons.Attach(wellWorkOverReason);
                    db.Entry(wellWorkOverReason).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public bool DeleteWellWorkOverReasonCheckReferences(int wellWorkOverReasonId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.WellWorkovers.Count(m => m.WellWorkOverReasonId == wellWorkOverReasonId) > 0)
                    {
                        return false;
                    }

                    var myObj = db.WellWorkOverReasons.Where(s => s.WellWorkOverReasonId == wellWorkOverReasonId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.WellWorkOverReasons.Remove(myObj[0]);
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
        public WellWorkOverReason GetWellWorkOverReason(int wellWorkOverReasonId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.WellWorkOverReasons.Where(s => s.WellWorkOverReasonId == wellWorkOverReasonId).ToList();
                    if (!myObj.Any())
                    {
                        return new WellWorkOverReason();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new WellWorkOverReason();
            }
        }
        public int GetWellWorkOverReasonIdByName(string wellWorkOverReason)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.WellWorkOverReasons.Where(s => s.Title.Trim().ToLower().Replace(" ", "") == wellWorkOverReason.Trim().ToLower().Replace(" ", "")).ToList();
                    
                    if (!myObj.Any())
                    {
                        var reason = new WellWorkOverReason
                        {
                            Title = wellWorkOverReason.Trim()
                        };
                        var processedItem = db.WellWorkOverReasons.Add(reason);
                        db.SaveChanges();
                        return processedItem.WellWorkOverReasonId;
                    }

                    return myObj[0].WellWorkOverReasonId;
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
