using System;
using System.Collections.Generic;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class WellCompletionIntervalServices
	{
        public List<WellCompletionInterval> GetAllOrderedWellCompletionIntervals()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.WellCompletionIntervals.Include("WellCompletion").ToList();
                    if (!myObjList.Any())
                    {
                        return new List<WellCompletionInterval>();
                    }
                    return myObjList.OrderBy(m => m.DateCompleted).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellCompletionInterval>();
            }
        }
        public int AddWellCompletionIntervalCheckDuplicate(WellCompletionInterval wellCompletionInterval)
        {
            try
            {
                if (wellCompletionInterval == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    db.WellCompletionIntervals.Add(wellCompletionInterval);
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int AddWellCompletionInterval2(WellCompletionInterval wellCompletionInterval)
        {
            try
            {
                if (wellCompletionInterval == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    db.WellCompletionIntervals.Add(wellCompletionInterval);
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                DeleteWellCompletionIntervalCheckReferences(wellCompletionInterval.WellCompletionId);
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateWellCompletionIntervalCheckDuplicate(WellCompletionInterval wellCompletionInterval)
        {
            try
            {
                if (wellCompletionInterval == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var entityToUpdateList = db.WellCompletionIntervals.Where(m => m.WellCompletionIntervalId == wellCompletionInterval.WellCompletionIntervalId).ToList();
                    if (entityToUpdateList.Any())
                    {
                        var entityToUpdate = entityToUpdateList[0];
                        entityToUpdate.LowerInterval = wellCompletionInterval.LowerInterval;
                        entityToUpdate.UpperInterval = wellCompletionInterval.UpperInterval;
                        entityToUpdate.DateCompleted = wellCompletionInterval.DateCompleted;
                        entityToUpdate.LastUpdatedTime = DateTime.Now.ToString("hh:mm:ss t ");
                        entityToUpdate.WellCompletionId = wellCompletionInterval.WellCompletionId;
                        //db.WellCompletionIntervals.Attach(wellCompletionInterval);
                        //db.Entry(wellCompletionInterval).State = EntityState.Modified;
                       return db.SaveChanges();
                    }

                    return -2;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public bool DeleteWellCompletionIntervalCheckReferences(int wellCompletionIntervalId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.WellCompletionIntervals.Where(s => s.WellCompletionIntervalId == wellCompletionIntervalId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.WellCompletionIntervals.Remove(myObj[0]);
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

        public WellCompletionInterval GetWellCompletionInterval(int wellCompletionIntervalId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.WellCompletionIntervals.Where(s => s.WellCompletionIntervalId == wellCompletionIntervalId).ToList();
                    if (!myObj.Any())
                    {
                        return new WellCompletionInterval();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new WellCompletionInterval();
            }
        }
	}
    
}
