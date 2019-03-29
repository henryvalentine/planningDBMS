using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{

    public class BlockTypeTypeServices
	{
        public List<BlockType> GetAllOrderedBlockTypes()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.BlockTypes.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<BlockType>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<BlockType>();
            }
        }

        public int AddBlockTypeCheckDuplicate(BlockType blockType)
        {
            try
            {
                if (blockType == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.BlockTypes.Any())
                    {
                        if (db.BlockTypes.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == blockType.Name.ToLower().Replace(" ", string.Empty)) > 0)
                        {
                            return -3;
                        }
                    }

                  var processedBlockType =  db.BlockTypes.Add(blockType);
                   db.SaveChanges();
                    return processedBlockType.BlockTypeId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateBlockTypeCheckDuplicate(BlockType blockType)
        {
            try
            {
                if (blockType == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.BlockTypes.Any())
                    {
                        if (db.BlockTypes.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == blockType.Name.ToLower().Replace(" ", string.Empty) && m.BlockTypeId != blockType.BlockTypeId) > 0)
                        {
                            return -3;
                        }
                    }
                    db.BlockTypes.Attach(blockType);
                    db.Entry(blockType).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public bool DeleteBlockTypeCheckReferences(int blockTypeId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Blocks.Count(m => m.BlockTypeId == blockTypeId) > 0)
                    {
                        return false;
                    }
                    
                    var myObj = db.BlockTypes.Where(s => s.BlockTypeId == blockTypeId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.BlockTypes.Remove(myObj[0]);
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

        public BlockType GetBlockType(int blockTypeId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.BlockTypes.Where(s => s.BlockTypeId == blockTypeId).ToList();
                    if (!myObj.Any())
                    {
                        return new BlockType();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new BlockType();
            }
        }
	}
	
}
