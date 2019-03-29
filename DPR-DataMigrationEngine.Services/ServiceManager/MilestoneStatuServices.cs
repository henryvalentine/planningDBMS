using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class MilestoneStatusServices
	{
        public List<MilestoneStatu> GetAllOrderedMilestoneStatuses()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.MilestoneStatus.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<MilestoneStatu>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<MilestoneStatu>();
            }
        }
        public int AddMilestoneStatusCheckDuplicate(MilestoneStatu milestoneStatus)
        {
            try
            {
                if (milestoneStatus == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.MilestoneStatus.Any())
                    {
                        if (db.MilestoneStatus.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == milestoneStatus.Name.ToLower().Replace(" ", string.Empty)) > 0)
                        {
                            return -3;
                        }
                    }

                    var processedItem = db.MilestoneStatus.Add(milestoneStatus);
                    db.SaveChanges();
                    return processedItem.MilestoneStatusId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateMilestoneStatusCheckDuplicate(MilestoneStatu milestoneStatus)
        {
            try
            {
                if (milestoneStatus == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.MilestoneStatus.Any())
                    {
                        if (db.MilestoneStatus.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == milestoneStatus.Name.ToLower().Replace(" ", string.Empty) && m.MilestoneStatusId != milestoneStatus.MilestoneStatusId) > 0)
                        {
                            return -3;
                        }
                    }

                    db.MilestoneStatus.Attach(milestoneStatus);
                    db.Entry(milestoneStatus).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public MilestoneStatu GetMilestoneStatus(int milestoneStatusId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.MilestoneStatus.Where(s => s.MilestoneStatusId == milestoneStatusId).ToList();
                    if (!myObj.Any())
                    {
                        return new MilestoneStatu();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new MilestoneStatu();
            }
        }
        public bool DeleteMilestoneStatuCheckReferences(int milestoneStatusId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.ProjectMileStones.Count(m => m.MileStoneStatusId == milestoneStatusId) > 0)
                    {
                        return false;
                    }

                    var myObj = db.MilestoneStatus.Where(s => s.MilestoneStatusId == milestoneStatusId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.MilestoneStatus.Remove(myObj[0]);
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
	}
    
}
