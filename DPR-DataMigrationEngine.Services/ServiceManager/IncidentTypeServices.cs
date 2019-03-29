using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class IncidentTypeServices
	{
        public List<IncidentType> GetAllOrderedIncidentTypes()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.IncidentTypes.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<IncidentType>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<IncidentType>();
            }
        }
        public int AddIncidentTypeCheckDuplicate(IncidentType incidentType)
        {
            try
            {
                if (incidentType == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.IncidentTypes.Any())
                    {
                        if (db.IncidentTypes.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == incidentType.Name.ToLower().Replace(" ", string.Empty)) > 0)
                        {
                            return -3;
                        }
                    }

                    var processedItem = db.IncidentTypes.Add(incidentType);
                    db.SaveChanges();
                    return processedItem.IncidentTypeId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateIncidentTypeCheckDuplicate(IncidentType incidentType)
        {
            try
            {
                if (incidentType == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.IncidentTypes.Any())
                    {
                        if (db.IncidentTypes.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == incidentType.Name.ToLower().Replace(" ", string.Empty) && m.IncidentTypeId != incidentType.IncidentTypeId) > 0)
                        {
                            return -3;
                        }
                    }

                    db.IncidentTypes.Attach(incidentType);
                    db.Entry(incidentType).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public IncidentType GetIncidentType(int incidentTypeId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.IncidentTypes.Where(s => s.IncidentTypeId == incidentTypeId).ToList();
                    if (!myObj.Any())
                    {
                        return new IncidentType();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new IncidentType();
            }
        }
        public bool DeleteIncidentTypeCheckReferences(int incidentTypeId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.IncidentHistories.Count(m => m.IncidentTypeId == incidentTypeId) > 0)
                    {
                        return false;
                    }

                    var myObj = db.IncidentTypes.Where(s => s.IncidentTypeId == incidentTypeId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.IncidentTypes.Remove(myObj[0]);
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

        public int GetIncidentTypeId(string incidentName)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {

                    var myObj = db.IncidentTypes.Where(s => s.Name.ToLower().Replace(" ", string.Empty).Trim() == incidentName.ToLower().Replace(" ", string.Empty).Trim()).ToList();
                    

                    if (!myObj.Any())
                    {
                        return 0;
                    }

                    return myObj[0].IncidentTypeId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 1;
            }
        }
	}


	
}
