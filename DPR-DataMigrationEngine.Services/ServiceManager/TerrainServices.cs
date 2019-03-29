using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class TerrainServices
	{
        public List<Terrain> GetAllOrderedTerrains()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Terrains.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Terrain>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Terrain>();
            }
        }

        public List<Terrain> GetATerrains()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Terrains.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Terrain>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Terrain>();
            }
        }
        public int AddTerrainCheckDuplicate(Terrain terrain)
        {
            try
            {
                if (terrain == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Terrains.Any())
                    {
                        if (db.Terrains.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == terrain.Name.ToLower().Replace(" ", string.Empty)) > 0)
                        {
                            return -3;
                        }
                    }

                    var processedItem = db.Terrains.Add(terrain);
                    db.SaveChanges();
                    return processedItem.TerrainId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateTerrainCheckDuplicate(Terrain terrain)
        {
            try
            {
                if (terrain == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Terrains.Any())
                    {
                        if (db.Terrains.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == terrain.Name.ToLower().Replace(" ", string.Empty) && m.TerrainId != terrain.TerrainId) > 0)
                        {
                            return -3;
                        }
                    }

                    db.Terrains.Attach(terrain);
                    db.Entry(terrain).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        
        public bool DeleteTerrainCheckReferences(int terrainId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Projects.Count(m => m.TerrainId == terrainId) > 0)
                    {
                        return false;
                    }

                    if (db.Fields.Count(m => m.TerrainId == terrainId) > 0)
                    {
                        return false;
                    }

                    var myObj = db.Terrains.Where(s => s.TerrainId == terrainId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.Terrains.Remove(myObj[0]);
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
        public Terrain GetTerrain(int terrainId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Terrains.Where(s => s.TerrainId == terrainId).ToList();
                    if (!myObj.Any())
                    {
                        return new Terrain();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Terrain();
            }
        }

        public int GetTerrainIdByName(string terrainName)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Terrains.Where(s => s.Name.Trim().ToLower().Replace(" ", "") == terrainName.Trim().ToLower().Replace(" ", "")).ToList();
                    if (!myObj.Any())
                    {
                        var newT = new Terrain
                        {
                            Name = terrainName
                        };

                        var processedItem = db.Terrains.Add(newT);
                        db.SaveChanges();
                        return processedItem.TerrainId;
                    }

                    return myObj[0].TerrainId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int GetTerrainId(string terrainName)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Terrains.Where(s => s.Name.ToLower().Replace(" ", string.Empty).Trim() == terrainName.ToLower().Replace(" ", string.Empty).Trim()).ToList();
                    if (!myObj.Any())
                    {
                        if (db.Terrains.Any())
                        {
                            if (db.Terrains.Count(s => s.Name.ToLower().Replace(" ", string.Empty).Trim() == terrainName.ToLower().Replace(" ", string.Empty).Trim()) > 0)
                            {
                                return -3;
                            }
                        }
                        var terrain = new Terrain { Name = terrainName.Trim()};
                        var processedTerrain = db.Terrains.Add(terrain);
                        db.SaveChanges();
                        return processedTerrain.TerrainId;
                    }

                    return myObj[0].TerrainId;
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
