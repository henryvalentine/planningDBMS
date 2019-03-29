using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class FieldServices
	{
        public List<Field> GetAllOrderedFields(int itemsPerPage, int pageNumber)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Fields.OrderBy(m => m.Name).Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).Include("Block").Include("Terrain").Include("Company").Include("Zone").ToList();    
                    if (!myObjList.Any())
                    {
                        return new List<Field>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList(); 
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Field>();
            }
        }
        public List<FieldReportObject> GetMoreFields(int itemsPerPage, int pageNumber)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Fields.OrderBy(m => m.Name).Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage)
                        .Include("Block")
                        .Include("Terrain")
                        .Include("Company")
                        .Include("Zone")
                        .ToList();
                    if (!myObjList.Any())
                    {
                        return new List<FieldReportObject>();
                    }

                  
                    var newList = new List<FieldReportObject>();
                    myObjList.ForEach(m => newList.Add(new FieldReportObject
                    {
                        FieldId = m.FieldId,
                        Name = m.Name,
                        BlockName = m.Block.Name,
                        TerrainName = m.Terrain.Name,
                        CompanyName = m.Company.Name,
                        TechnicalAllowableStr = (float?)m.TechnicalAllowable > 0 ? m.TechnicalAllowable.ToString() : " ",
                        ZoneName = m.Zone.Name
                    }));
                    return newList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<FieldReportObject>();
            }
        }
        public List<FieldReportObject> GetFieldsByTerrain(int blockId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Fields.Where(m => m.TerrainId == blockId).Include("Block").Include("Terrain").Include("Company").Include("Zone").ToList();    
                    if (!myObjList.Any())
                    {
                        return new List<FieldReportObject>();  
                    }
                    var newList = new List<FieldReportObject>();
                    myObjList.ForEach(m => newList.Add(new FieldReportObject
                    {
                        Name = m.Name,
                        BlockName = m.Block.Name,
                        TerrainName = m.Terrain.Name,
                        CompanyName = m.Company.Name,
                        TechnicalAllowable = (float?) m.TechnicalAllowable,
                        ZoneName = m.Zone.Name
                    }));
                    return newList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<FieldReportObject>();
            }
        }
        public List<FieldReportObject> GetFieldsByCompanyId(long companyId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Fields.Where(m => m.CompanyId == companyId).ToList();
                    if (!myObjList.Any())
                    {
                        return new List<FieldReportObject>();
                    }
                    var newList = new List<FieldReportObject>();
                    myObjList.ForEach(m => newList.Add(new FieldReportObject
                    {
                        Name = m.Name,
                        FieldId = m.FieldId
                    }));
                    return newList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<FieldReportObject>();
            }
        }
        public List<FieldReportObject> GetFieldsByBlockId(int blockId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Fields.Where(m => m.BlockId == blockId).ToList();
                    if (!myObjList.Any())
                    {
                        return new List<FieldReportObject>();
                    }
                    var newList = new List<FieldReportObject>();
                    myObjList.ForEach(m => newList.Add(new FieldReportObject
                    {
                        Name = m.Name,
                        FieldId = m.FieldId
                    }));
                    return newList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<FieldReportObject>();
            }
        }
        public List<Field> GetFields()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Fields.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Field>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Field>();
            }
        }
        public List<Field> GetFieldsWithWells()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Fields.Where(m => m.Wells.Any()).ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Field>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Field>();
            }
        }
        public int AddFieldCheckDuplicate(Field field)
        {
            try
            {
                if (field == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Fields.Any())
                    {
                        if (db.Fields.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == field.Name.ToLower().Replace(" ", string.Empty) && m.CompanyId == field.CompanyId) > 0)
                        {
                            return -3;
                        }
                    }

                    var processedItem = db.Fields.Add(field);
                    db.SaveChanges();
                    return processedItem.FieldId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateFieldCheckDuplicate(Field field)
        {
            try
            {
                if (field == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Fields.Any())
                    {
                        if (db.Fields.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == field.Name.ToLower().Replace(" ", string.Empty) && m.FieldId != field.FieldId) > 0)
                        {
                            return -3;
                        }
                    }

                    db.Fields.Attach(field);
                    db.Entry(field).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public bool DeleteFieldCheckReferences(int fieldId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Wells.Count(m => m.FieldId == fieldId) > 0)
                    {
                        return false;
                    }

                    var myObj = db.Fields.Where(s => s.FieldId == fieldId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.Fields.Remove(myObj[0]);
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
        public Field GetField(int fieldId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Fields.Where(s => s.FieldId == fieldId).ToList();
                    if (!myObj.Any())
                    {
                        return new Field();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Field();
            }
        }
        public Field GetFieldTerrain(int terrainId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Fields.Where(s => s.TerrainId == terrainId).ToList();
                    if (!myObj.Any())
                    {
                        return new Field();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Field();
            }
        }
        public Field GetFieldIdByFieldName(string fieldName)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Fields.Where(s => s.Name.Trim().ToLower().Replace(" ", "") == fieldName.Trim().ToLower().Replace(" ", "")).ToList();
                    if (!myObj.Any())
                    {
                        return new Field();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Field();
            }
        }
        public Zone GetZoneByField(int fieldId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Fields.Where(s => s.FieldId == fieldId).Include("Zone").ToList();
                    if (!myObj.Any())
                    {
                        return new Zone();
                    }
                    return myObj[0].Zone;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Zone();
            }
        }
        public Terrain GetTerrainByField(int terrainId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Fields.Where(s => s.TerrainId == terrainId).Include("Terrain").ToList();
                    if (!myObj.Any())
                    {
                        return new Terrain();
                    }

                    return myObj[0].Terrain;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Terrain();
            }
        }
        public List<Field> GetAllOrderedFieldsByCompanyId(int companyId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Fields.Where(m => m.CompanyId == companyId).ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Field>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Field>();
            }
        }
        public int GetFieldIdByName(string fieldName)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Fields.Where(s => s.Name.ToLower().Trim().Replace(" ", string.Empty) == fieldName.ToLower().Trim().Replace(" ", string.Empty)).ToList();
                    if (!myObj.Any())
                    {
                        var field = new Field { Name = fieldName, ZoneId = 1, TerrainId = 1, CompanyId = 1 };
                        var processedField = db.Fields.Add(field);
                        db.SaveChanges();
                        return processedField.FieldId;
                    }
                    var ttd = myObj[0];
                    return ttd.FieldId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public Field GetFielAndCompanyIds(string fieldName)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Fields.Where(s => s.Name.ToLower().Trim().Replace(" ", string.Empty) == fieldName.ToLower().Trim().Replace(" ", string.Empty)).ToList();
                    if (!myObj.Any())
                    {
                        var field = new Field { Name = fieldName, ZoneId = 1, TerrainId = 1, CompanyId = 1 };
                        var processedField = db.Fields.Add(field);
                        db.SaveChanges();
                        return processedField;
                    }
                    var ttd = myObj[0];
                    return ttd;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Field { Name = fieldName, ZoneId = 1, TerrainId = 1, CompanyId = 1 }; 
            }
        }
        public bool CheckField(string fieldName)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Fields.Where(s => s.Name.ToLower().Trim().Replace(" ", string.Empty) == fieldName.ToLower().Trim().Replace(" ", string.Empty)).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }
        public List<FieldReportObject> GetOrderedFieldReportObjects(FieldQuery queryBuilder)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {

                    if (queryBuilder != null)
                    {
                        var query = (

                            from f in db.Fields.OrderBy(m => m.Name)
                            join c in db.Companies on f.CompanyId equals c.CompanyId
                            where ((queryBuilder.CompanyId > 0 && queryBuilder.CompanyId == c.CompanyId) || (queryBuilder.CompanyId < 1 && f.CompanyId == c.CompanyId))
                            join t in db.Terrains on f.TerrainId equals t.TerrainId
                            where ((queryBuilder.TerrainId > 0 && queryBuilder.TerrainId == t.TerrainId) || (queryBuilder.TerrainId < 1 && f.TerrainId == t.TerrainId))
                            join b in db.Blocks on f.BlockId equals b.BlockId
                            where ((queryBuilder.BlockId > 0 && queryBuilder.BlockId == b.BlockId) || (queryBuilder.BlockId < 1 && f.BlockId == b.BlockId))
                            join z in db.Zones on f.ZoneId equals z.ZoneId
                             where ((queryBuilder.ZoneId > 0 && z.ZoneId == queryBuilder.ZoneId) || (z.ZoneId == f.ZoneId && queryBuilder.ZoneId < 1))
                                 
                            select new FieldReportObject
                            {
                                Name = t.Name,
                                FieldName = f.Name,
                                BlockName = b.Name,
                                CompanyName = c.Name,
                                ZoneName = z.Name,
                                TerrainName = t.Name,
                            }).ToList();

                        return !query.Any() ? new List<FieldReportObject>() : query;
                    }
                    return new List<FieldReportObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<FieldReportObject>();
            }
        }
	}
	
}
