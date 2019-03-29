using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class CompanyServices
	{
        public List<Company> GetAllOrderedCompanies(int itemsPerPage, int pageNumber, out int dataCount)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                        dataCount = db.Companies.Count();
                        var myObjList =
                            db.Companies.OrderBy(m => m.Name)
                                .Skip((pageNumber - 1)*itemsPerPage)
                                .Take(itemsPerPage)
                                .Include("People")
                                .ToList();
                        if (!myObjList.Any())
                        {
                            return new List<Company>();
                        }
                        myObjList.ForEach(x =>
                        {
                            if (x.People.Any())
                            {
                                var dtg = x.People.ToList()[0];
                                x.PersonName = dtg.FirstName + " " + dtg.LastName;
                            }
                            else
                            {
                                x.PersonName = "N/A";
                            }
                        });
                        return myObjList.OrderBy(m => m.Name).ToList();
                    
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                dataCount = 0;
                return new List<Company>();
            }
        }

        public List<Company> GetAllOrderedCompanies()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                   
                        var myObjList =
                            db.Companies
                                .ToList();
                        if (!myObjList.Any())
                        {
                            return new List<Company>();
                        }
                        return myObjList.OrderBy(m => m.Name).ToList();
                    
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
               
                return new List<Company>();
            }
        }

        public List<Company> GetCompaniesWithFields()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {

                    var myObjList = (from f in db.Fields
                        where (f.Wells.Any())
                        join c in db.Companies on f.CompanyId equals c.CompanyId
                        select c).ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Company>();
                    }
                    var newList = new List<Company>();
                    myObjList.ForEach(m =>
                    {
                        if (!newList.Exists(x => x.CompanyId == m.CompanyId))
                        {
                            newList.Add(m);
                        }
                    });
                    return newList.OrderBy(m => m.Name).ToList();

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);

                return new List<Company>();
            }
        }

        public List<Company> GetCompaniesWithBlocks()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {

                    var myObjList = (from c in db.Companies
                                     join b in db.Blocks on c.CompanyId equals b.CompanyId
                                     join f in db.Fields on b.BlockId equals  f.BlockId
                                     select c).ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Company>();
                    }
                    var newList = new List<Company>();
                    myObjList.ForEach(m =>
                    {
                        if (!newList.Exists(x => x.CompanyId == m.CompanyId))
                        {
                            newList.Add(m);
                        }
                    });
                    return newList.OrderBy(m => m.Name).ToList();

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);

                return new List<Company>();
            }
        }

        public long AddCompanyCheckDuplicate(Company company)
        {
            try
            {
                if (company == null)
                {
                    return -2;
                }
                
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Companies.Any())
                    {
                        if (db.Companies.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == company.Name.ToLower().Replace(" ", string.Empty)) > 0)
                        {
                            return -3;
                        }
                    }

                  var processedCompany =  db.Companies.Add(company);
                  db.SaveChanges();
                  //if (company.People != null && company.People.Any())
                  //  {
                  //      var person = company.People.ElementAt(0);
                  //      person.CompanyId = processedCompany.CompanyId;
                  //      var personId = new PersonServices().AddPersonCheckDuplicate(person);
                  //      if (personId < 1)
                  //      {
                  //          DeleteCompanyCheckReferences(processedCompany.CompanyId);
                  //          return 0;
                  //      }
                  //  }
                  return processedCompany.CompanyId ;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateCompanyCheckDuplicate(Company company, Person person)
        {
            try
            {
                if (company == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Companies.Any())
                    {
                        if (db.Companies.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == company.Name.ToLower().Replace(" ", string.Empty) && m.CompanyId != company.CompanyId) > 0)
                        {
                            return -3;
                        }

                    }
                    
                    db.Companies.Attach(company);
                    db.Entry(company).State = EntityState.Modified;
                    db.SaveChanges();
                    var status = 0;

                    if (person != null)
                    {
                        if (person.PersonId < 1)
                        {
                            person.CompanyId = company.CompanyId;
                            var processStatus = new PersonServices().AddPersonCheckDuplicate(person);
                            if (processStatus < 1)
                            {
                                return status;
                            }
                            status = 1;
                        }
                        else
                        {
                            var processStatus = new PersonServices().UpdatePersonCheckDuplicate(person);
                            if (processStatus < 1)
                            {
                                return status;
                            }
                            status = 1;
                        }
                    }
                        
                    return status;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public CompanyObject GetCompany(int companyId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Companies.Where(s => s.CompanyId == companyId).Include("People").ToList();
                    if (!myObj.Any())
                    {
                        return new CompanyObject();
                    }
                    var ttd = myObj[0];
                    var personObject = new PersonObject();
                    if (ttd.People != null && ttd.People.Any())
                    {
                        var person = ttd.People.ToList()[0];
                        personObject = new PersonObject
                        {
                            PersonId = person.PersonId,
                            LastName = person.LastName,
                            FirstName = person.FirstName,
                            CompanyId = person.CompanyId,
                            Designation = person.Designation,
                            PhoneNumber = person.PhoneNumber,
                            Email = person.Email,

                        };
                    }

                    
                    var compObject = new CompanyObject
                    {
                        CompanyId = ttd.CompanyId,
                        Name = ttd.Name,
                        Address = ttd.Address,
                        RCNumber=ttd.RCNumber,
                        PersonObj = personObject
                     
                    };
                    return compObject;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new CompanyObject();
            }
        }

        public List<CompanyObject> GetCompanyObjects(int itemsPerPage, int pageNumber)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Companies.OrderBy(m => m.Name)
                                .Skip((pageNumber - 1)*itemsPerPage)
                                .Take(itemsPerPage)
                                .Include("People").ToList();
                    if (!myObjList.Any())
                    {
                        return new List<CompanyObject>();
                    }

                    var ttf = new List<CompanyObject>();

                    myObjList.ForEach(m =>
                    {
                        var ttd = m;
                        var str = "";
                        if (ttd.People.Any())
                        {
                            var dtg = ttd.People.ToList()[0];
                           str = dtg.FirstName + " " + dtg.LastName;
                        }
                        else
                        {
                          str = "N/A";
                        }

                        var compObject = new CompanyObject
                        {
                            CompanyId = ttd.CompanyId,
                            Name = ttd.Name,
                            Address = string.IsNullOrEmpty(ttd.Address) ? " " : ttd.Address,
                            RCNumber = string.IsNullOrEmpty(ttd.RCNumber) ? " " : ttd.RCNumber,
                            CanonialName = string.IsNullOrEmpty(ttd.CanonialName) ? " " : ttd.CanonialName,
                            Contact = string.IsNullOrEmpty(ttd.CanonialName) ? " " : ttd.CanonialName,
                            PersonName = str
                        };
                        ttf.Add(compObject);
                    });

                    if (!ttf.Any())
                    {
                        return new List<CompanyObject>();
                    }
                    return ttf;
                }
                
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<CompanyObject>();
            }
        }

        public long GetCompanyId(string companyName, string cannonialName)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Companies.Where(s => s.Name.ToLower().Trim().Replace(" ", string.Empty) == companyName.ToLower().Trim().Replace(" ", string.Empty)).ToList();
                    if (!myObj.Any())
                    {
                        var company = new Company { Name = companyName, CanonialName = cannonialName };
                        var processedCompany = db.Companies.Add(company);
                        db.SaveChanges();
                        return processedCompany.CompanyId;
                    }
                    var ttd = myObj[0];
                    return ttd.CompanyId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 1;
            }
        }

        public long GetCompanyId(string companyName)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Companies.Where(s => s.Name.ToLower().Trim().Replace(" ", string.Empty) == companyName.ToLower().Trim().Replace(" ", string.Empty)).ToList();
                    if (!myObj.Any())
                    {
                       
                        return 0;
                    }
                    var ttd = myObj[0];
                    return ttd.CompanyId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 1;
            }
        }
        public bool DeleteCompanyCheckReferences(long companyId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Fields.Count(m => m.CompanyId == companyId) > 0)
                    {
                        return false;
                    }

                    if (db.People.Count(m => m.CompanyId == companyId) > 0)
                    {
                        return false;
                    }

                    var myObj = db.Companies.Where(s => s.CompanyId == companyId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.Companies.Remove(myObj[0]);
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
