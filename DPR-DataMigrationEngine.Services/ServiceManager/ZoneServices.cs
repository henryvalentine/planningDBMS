using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class ZoneServices
	{
        public List<Zone> GetAllOrderedZones()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Zones.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Zone>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Zone>();
            }
        }
        public int AddZoneCheckDuplicate(Zone zone)
        {
            try
            {
                if (zone == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Zones.Any())
                    {
                        if (db.Zones.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == zone.Name.ToLower().Replace(" ", string.Empty)) > 0)
                        {
                            return -3;
                        }
                    }

                    var processedItem = db.Zones.Add(zone);
                    db.SaveChanges();
                    return processedItem.ZoneId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateZoneCheckDuplicate(Zone zone)
        {
            try
            {
                if (zone == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Zones.Any())
                    {
                        if (db.Zones.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == zone.Name.ToLower().Replace(" ", string.Empty) && m.ZoneId != zone.ZoneId) > 0)
                        {
                            return -3;
                        }
                    }

                    db.Zones.Attach(zone);
                    db.Entry(zone).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);

                return 0;
            }
        }

        public Zone GetZone(int zoneId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Zones.Where(s => s.ZoneId == zoneId).ToList();
                    if (!myObj.Any())
                    {
                        return new Zone();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Zone();
            }
        }


        public int GetZoneIdByName(string zoneName)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Zones.Where(s => s.Name.Trim().ToLower().Replace(" ", "") == zoneName.Trim().ToLower().Replace(" ", "")).ToList();
                    if (!myObj.Any())
                    {
                        var newT = new Zone
                        {
                            Name = zoneName
                        };

                        var processedItem = db.Zones.Add(newT);
                        db.SaveChanges();
                        return processedItem.ZoneId;
                    }

                    return myObj[0].ZoneId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        
        public bool DeleteZoneCheckReferences(int zoneId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Fields.Count(m => m.ZoneId == zoneId) > 0)
                    {
                        return false;
                    }

                    if (db.Fields.Count(m => m.ZoneId == zoneId) > 0)
                    {
                        return false;
                    }

                    var myObj = db.Zones.Where(s => s.ZoneId == zoneId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.Zones.Remove(myObj[0]);
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
