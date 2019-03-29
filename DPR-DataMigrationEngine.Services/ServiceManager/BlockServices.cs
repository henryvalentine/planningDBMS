using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{

	public class BlockServices
	{
        public List<Block> GetAllOrderedBlocks()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Blocks
                        .Include("Company")
                        .Include("BlockType")
                        .Include("LeaseType")
                        .ToList();
                    if (!myObjList.Any())
                    {     
                        return new List<Block>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Block>();
            }
        }
        public List<Block> GetBlocksWithFields()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Blocks.Where(m => m.Fields.Any()).ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Block>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Block>();
            }
        }
        public List<Block> GetBlocks()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Blocks.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Block>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Block>();
            }
        }
        public int AddBlockCheckDuplicate(Block block)
        {
            try
            {
                if (block == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Blocks.Any())
                    {
                        if (db.Blocks.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == block.Name.ToLower().Replace(" ", string.Empty)) > 0)
                        {
                            return -3;
                        }
                    }

                  var processedBlock =  db.Blocks.Add(block);
                   db.SaveChanges();
                    return processedBlock.BlockId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateBlockCheckDuplicate(Block block)
        {
            try
            {
                if (block == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Blocks.Any())
                    {
                        if (db.Blocks.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == block.Name.ToLower().Replace(" ", string.Empty) && m.BlockId != block.BlockId) > 0)
                        {
                            return -3;
                        }
                    }
                    db.Blocks.Attach(block);
                    db.Entry(block).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public bool DeleteBlockCheckReferences(int blockId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Fields.Count(m => m.BlockId == blockId) > 0)
                    {
                        return false;
                    }
                    
                    var myObj = db.Blocks.Where(s => s.BlockId == blockId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.Blocks.Remove(myObj[0]);
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
        public Block GetBlock(int blockId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Blocks.Where(s => s.BlockId == blockId).ToList();
                    if (!myObj.Any())
                    {
                        return new Block();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Block();
            }
        }
        public int GetBlockId(string blockName)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {

                    var myObj = db.Blocks.Where(s => s.Name.ToLower().Replace(" ", string.Empty).Trim() == blockName.ToLower().Replace(" ", string.Empty).Trim()).ToList();
                    
                    
                    //if (!myObj.Any())
                    //{
                    //    var block = new Block { Name = blockName, BlockTypeId = blockId, TerrainId = 1};
                    //    var processedBlocks = db.Blocks.Add(block);
                    //    db.SaveChanges();
                    //    return processedBlocks.BlockId;
                    //}

                    if (!myObj.Any())
                    {
                        return 0;
                    }

                    return myObj[0].BlockId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 1;
            }
        }
        public List<BlockObject> GetBlocksByCompanyId(long companyId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = (from b in db.Blocks.Where(c => c.CompanyId == companyId)
                                     select new BlockObject
                                    {
                                        BlockId = b.BlockId,
                                        Name = b.Name
                                    }).ToList();
                    if (!myObjList.Any())
                    {
                        return new List<BlockObject>();
                    }

                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<BlockObject>();
            }
        }

        public List<BlockObject> GetCompanyBlocksWithFields(long companyId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = (from b in db.Blocks.Where(m => m.CompanyId == companyId && m.Fields.Any())
                                    select new BlockObject
                                    {
                                        BlockId = b.BlockId,
                                        Name = b.Name
                                    }).ToList();

                    if (!myObjList.Any())
                    {
                        return new List<BlockObject>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<BlockObject>();
            }
        }
	}
	
}
