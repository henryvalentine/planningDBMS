using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class EquipmentUsageHistoryServices
	{
        public List<EquipmentUsageHistory> GetAllOrderedEquipmentUsageHistories()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.EquipmentUsageHistories.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<EquipmentUsageHistory>();
                    }
                    return myObjList.OrderBy(m => m.EquipmentUsageHistoryId).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<EquipmentUsageHistory>();
            }
        }

        public int AddEquipmentUsageHistory(EquipmentUsageHistory equipmentUsageHistory)
        {
            try
            {
                if (equipmentUsageHistory == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var processedItem = db.EquipmentUsageHistories.Add(equipmentUsageHistory);
                    db.SaveChanges();
                    return processedItem.EquipmentUsageHistoryId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateEquipmentUsageHistory(EquipmentUsageHistory equipmentUsageHistory)
        {
            try
            {
                if (equipmentUsageHistory == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    db.EquipmentUsageHistories.Attach(equipmentUsageHistory);
                    db.Entry(equipmentUsageHistory).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public bool DeleteEquipmentUsageHistoryCheckReferences(int equipmentUsageHistoryId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.EquipmentUsageHistories.Where(s => s.EquipmentUsageHistoryId == equipmentUsageHistoryId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.EquipmentUsageHistories.Remove(myObj[0]);
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
