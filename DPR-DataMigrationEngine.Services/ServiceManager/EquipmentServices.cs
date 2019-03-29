using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class EquipmentServices
	{
        public List<Equipment> GetAllOrderedEquipments()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Equipments.Include("EquipmentType").ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Equipment>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Equipment>();
            }
        }

        public List<Equipment> GetEquipments()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Equipments.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Equipment>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Equipment>();
            }
        }

        public int AddEquipmentCheckDuplicate(Equipment equipment)
        {
            try
            {
                if (equipment == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Equipments.Any())
                    {
                        if (db.Equipments.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == equipment.Name.ToLower().Replace(" ", string.Empty)) > 0)
                        {
                            return -3;
                        }
                    }


                    var processedEquipment = db.Equipments.Add(equipment);
                    db.SaveChanges();
                    return processedEquipment.EquipmentId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateEquipmentCheckDuplicate(Equipment equipment)
        {
            try
            {
                if (equipment == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Equipments.Any())
                    {
                        if (db.Equipments.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == equipment.Name.ToLower().Replace(" ", string.Empty) && m.EquipmentId != equipment.EquipmentId) > 0)
                        {
                            return -3;
                        }
                    }

                    db.Equipments.Attach(equipment);
                    db.Entry(equipment).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public Equipment GetEquipment(int equipmentId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Equipments.Where(s => s.EquipmentId == equipmentId).ToList();
                    if (!myObj.Any())
                    {
                        return new Equipment();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Equipment();
            }
        }

        public List<Equipment> GetEquipmentsByEquipmentType(int equipmentTypeId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Equipments.Where(s => s.EquipmentTypeId == equipmentTypeId).Include("EquipmentType").ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Equipment>();
                    }

                    return myObjList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Equipment>();
            }
        }

        public bool DeleteEquipmentCheckReferences(int equipmentId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.WellWorkovers.Count(m => m.EquipmentId == equipmentId) > 0)
                    {
                        return false;
                    }

                    if (db.EquipmentUsageHistories.Count(m => m.EquipmentId == equipmentId) > 0)
                    {
                        return false;
                    }

                    var myObj = db.Equipments.Where(s => s.EquipmentId == equipmentId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.Equipments.Remove(myObj[0]);
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


        public int GetEquipmentId(string equipmentName)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {

                    var myObj = db.Equipments.Where(s => s.Name.ToLower().Replace(" ", string.Empty).Trim() == equipmentName.ToLower().Replace(" ", string.Empty).Trim()).ToList();


                    if (!myObj.Any())
                    {
                        var equipment = new Equipment { Name = equipmentName, EquipmentTypeId = 1, LicenseStatus = false };
                        var processedBlocks = db.Equipments.Add(equipment);
                        db.SaveChanges();
                        return processedBlocks.EquipmentId;
                    }

                    return myObj[0].EquipmentId;
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
