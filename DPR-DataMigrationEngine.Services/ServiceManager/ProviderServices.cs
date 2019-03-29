using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
    public class ProviderServices
	{
        public List<Provider> GetAllOrderedProvider(int itemsPerPage, int pageNumber)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                        var myObjList = 
                            db.Providers.OrderBy(m => m.Name)
                                .Skip((pageNumber - 1) * itemsPerPage)
                                .Take(itemsPerPage)
                                .ToList();
                        if (!myObjList.Any())
                        {
                            return new List<Provider>();
                        }
                        return myObjList;
                    }
                }
            
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Provider>();
            }
        }
        public List<Provider> GetProviders()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Providers.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Provider>();
                    }
                    return myObjList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);

                return new List<Provider>();
            }
        }
        public long AddProviderCheckDuplicate(Provider provider)
        {
            try
            {
                if (provider == null)
                {
                    return -2;
                }

                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Providers.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == provider.Name.ToLower().Replace(" ", string.Empty)) > 0)
                    {
                        return -3;
                    }
                    var processedCompany = db.Providers.Add(provider);
                    db.SaveChanges();
                    return processedCompany.ProviderId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateProviderCheckDuplicate(Provider provider)
        {
            try
            {
                if (provider == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Providers.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == provider.Name.ToLower().Replace(" ", string.Empty) && m.ProviderId != provider.ProviderId ) > 0)
                    {
                        return -3;
                    }
                   db.Providers.Attach(provider);
                    db.Entry(provider).State = EntityState.Modified;
                    db.SaveChanges();
                    return 5;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public Provider GetProvider(long providerKey, string providerScrete)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Providers.Where(s => s.ProviderKey == providerKey && s.ProviderSecrete == providerScrete).ToList();
                    if (!myObj.Any())
                    {
                        return new Provider();
                    }
                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Provider();
            }
        }

        public Provider GetProviderInfo(long providerKey)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Providers.Where(s => s.ProviderKey == providerKey).ToList();
                    if (!myObj.Any())
                    {
                        return new Provider();
                    }
                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Provider();
            }
        }

        public Provider GetProvider(long providerId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Providers.Where(s => s.ProviderId == providerId).ToList();
                    if (!myObj.Any())
                    {
                        return new Provider();
                    }
                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Provider();
            }
        }
        public bool DeleteProvider(int providerId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Providers.Where(s => s.ProviderId == providerId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.Providers.Remove(myObj[0]);
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
