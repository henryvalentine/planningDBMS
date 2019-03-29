using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
    public class SectorServices
	{
        public List<Sector> GetAllOrderedSectors()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Sectors.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Sector>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Sector>();
            }
        }
        public int AddSectorCheckDuplicate(Sector sector)
        {
            try
            {
                if (sector == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Sectors.Any())
                    {
                        if (db.Sectors.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == sector.Name.ToLower().Replace(" ", string.Empty)) > 0)
                        {
                            return -3;
                        }
                    }

                    var processedItem = db.Sectors.Add(sector);
                    db.SaveChanges();
                    return processedItem.SectorId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateSectorCheckDuplicate(Sector sector)
        {
            try
            {
                if (sector == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Sectors.Any())
                    {
                        if (db.Sectors.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == sector.Name.ToLower().Replace(" ", string.Empty) && m.SectorId != sector.SectorId) > 0)
                        {
                            return -3;
                        }
                    }

                    db.Sectors.Attach(sector);
                    db.Entry(sector).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public Sector GetSector(int sectorId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Sectors.Where(s => s.SectorId == sectorId).ToList();
                    if (!myObj.Any())
                    {
                        return new Sector();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Sector();
            }
        }

        public int GetSectorId(string sectorName)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Sectors.Where(s => s.Name.ToLower().Trim().Replace(" ", string.Empty) == sectorName.ToLower().Trim().Replace(" ", string.Empty)).ToList();
                    if (!myObj.Any())
                    {
                        var sct = new Sector
                        {
                            Name = sectorName.Trim()
                        };
                        var processedItem = db.Sectors.Add(sct);
                        db.SaveChanges();
                        return processedItem.SectorId;
                    }

                    return myObj[0].SectorId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 1;
            }
        }
        public bool DeleteSectorCheckReferences(int sectorId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Projects.Count(m => m.SectorId == sectorId) > 0)
                    {
                        return false;
                    }

                    var myObj = db.Sectors.Where(s => s.SectorId == sectorId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.Sectors.Remove(myObj[0]);
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
