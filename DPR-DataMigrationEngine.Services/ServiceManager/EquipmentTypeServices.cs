using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class EquipmentTypeServices
	{
        public List<EquipmentType> GetAllOrderedEquipmentTypes()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.EquipmentTypes.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<EquipmentType>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<EquipmentType>();
            }
        }

        public int AddEquipmentTypeCheckDuplicate(EquipmentType equipmentType)
        {
            try
            {
                if (equipmentType == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.EquipmentTypes.Any())
                    {
                        if (db.EquipmentTypes.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == equipmentType.Name.ToLower().Replace(" ", string.Empty)) > 0)
                        {
                            return -3;
                        }
                    }
                    var processedItem = db.EquipmentTypes.Add(equipmentType);
                    db.SaveChanges();
                    return processedItem.EquipmentTypeId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateEquipmentTypeCheckDuplicate(EquipmentType equipmentType)
        {
            try
            {
                if (equipmentType == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.EquipmentTypes.Any())
                    {
                        if (db.EquipmentTypes.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == equipmentType.Name.ToLower().Replace(" ", string.Empty) && m.EquipmentTypeId != equipmentType.EquipmentTypeId) > 0)
                        {
                            return -3;
                        }
                    }

                    db.EquipmentTypes.Attach(equipmentType);
                    db.Entry(equipmentType).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

	    public EquipmentType GetEquipmentType(int equipmentTypeId)
	    {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.EquipmentTypes.Where(s => s.EquipmentTypeId == equipmentTypeId).ToList();
                    if (!myObj.Any())
                    {
                        return new EquipmentType();
                    }

                    return myObj[0]; 
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new EquipmentType();
            }
	    }

        public bool DeleteEquipmentTypeCheckReferences(int equipmentTypeId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Equipments.Count(m => m.EquipmentTypeId == equipmentTypeId) > 0)
                    {
                        return false;
                    }
                 
                    var myObj = db.EquipmentTypes.Where(s => s.EquipmentTypeId == equipmentTypeId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.EquipmentTypes.Remove(myObj[0]);
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
