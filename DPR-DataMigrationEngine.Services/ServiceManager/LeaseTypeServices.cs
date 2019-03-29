using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class LeaseTypeServices
	{
        public List<LeaseType> GetAllOrderedLeaseTypes()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.LeaseTypes.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<LeaseType>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<LeaseType>();
            }
        }
        public int AddLeaseTypeCheckDuplicate(LeaseType leaseType)
        {
            try
            {
                if (leaseType == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.LeaseTypes.Any())
                    {
                        if (db.LeaseTypes.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == leaseType.Name.ToLower().Replace(" ", string.Empty)) > 0)
                        {
                            return -3;
                        }
                    }

                    var processedItem = db.LeaseTypes.Add(leaseType);
                    db.SaveChanges();
                    return processedItem.LeaseTypeId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateLeaseTypeCheckDuplicate(LeaseType leaseType)
        {
            try
            {
                if (leaseType == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.LeaseTypes.Any())
                    {
                        if (db.LeaseTypes.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == leaseType.Name.ToLower().Replace(" ", string.Empty) && m.LeaseTypeId != leaseType.LeaseTypeId) > 0)
                        {
                            return -3;
                        }
                    }

                    db.LeaseTypes.Attach(leaseType);
                    db.Entry(leaseType).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);

                return 0;
            }
        }

        public LeaseType GetLeaseType(int leaseTypeId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.LeaseTypes.Where(s => s.LeaseTypeId == leaseTypeId).ToList();
                    if (!myObj.Any())
                    {
                        return new LeaseType();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new LeaseType();
            }
        }

        public int GetLeaseTypeIdByName(string leaseTypeName)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.LeaseTypes.Where(s => s.Name.Trim().ToLower().Replace(" ", "") == leaseTypeName.Trim().ToLower().Replace(" ", "")).ToList();
                    if (!myObj.Any())
                    {
                        var newLeaseType = new LeaseType
                        {
                            Name = leaseTypeName

                        };

                        var processedLeaseType = db.LeaseTypes.Add(newLeaseType);
                        db.SaveChanges();
                        return processedLeaseType.LeaseTypeId;
                    }

                    return myObj[0].LeaseTypeId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        
        public bool DeleteLeaseTypeCheckReferences(int leaseTypeId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Blocks.Count(m => m.LeaseTypeId == leaseTypeId) > 0)
                    {
                        return false;
                    }

                    var myObj = db.LeaseTypes.Where(s => s.LeaseTypeId == leaseTypeId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.LeaseTypes.Remove(myObj[0]);
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
