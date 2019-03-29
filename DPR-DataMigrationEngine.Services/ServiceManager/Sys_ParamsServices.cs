using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{

// ReSharper disable once InconsistentNaming
    public class Sys_ParamsServices
	{
        public Sys_Param GetSysParams()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Sys_Param.ToList();
                    if (!myObjList.Any())
                    {     
                        return new Sys_Param();
                    }
                    return myObjList[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Sys_Param();
            }
        }

        public int AddSys_ParamsCheckPrevious(Sys_Param sysParam)
        {
            try
            {
                if (sysParam == null)
                {
                    return -2;
                }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Sys_Param.Any())
                    {
                       return -3;
                    }

                   var processedParams = db.Sys_Param.Add(sysParam);
                   db.SaveChanges();
                   return processedParams.Sys_ParamId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateSys_Params(Sys_Param sys_Param)
        {
            try
            {
                if (sys_Param == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    db.Sys_Param.Attach(sys_Param);
                    db.Entry(sys_Param).State = EntityState.Modified;
                    return db.SaveChanges();
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
