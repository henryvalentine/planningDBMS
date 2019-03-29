using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class PersonServices
	{
        public List<Person> GetAllOrderedPersons()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.People.Include("Company").ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Person>();
                    }
                    return myObjList.OrderBy(m => m.LastName).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Person>();
            }
        }
        public long AddPersonCheckDuplicate(Person person)
        {
            try
            {
                if (person == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.People.Any())
                    {
                        if (db.People.Count(m => m.LastName.ToLower().Trim().Replace(" ", string.Empty) == person.LastName.ToLower().Trim().Replace(" ", string.Empty) && m.FirstName.ToLower().Trim().Replace(" ", string.Empty) == person.FirstName.ToLower().Trim().Replace(" ", string.Empty) && m.CompanyId == person.CompanyId) > 0)
                        {
                            return -3;
                        }
                    }

                   var processedPerson = db.People.Add(person);
                   db.SaveChanges();
                    return processedPerson.PersonId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdatePersonCheckDuplicate(Person person)
        {
            try
            {
                if (person == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.People.Any())
                    {
                        if (db.People.Count(m => m.LastName.ToLower().Trim().Replace(" ", string.Empty) == person.LastName.ToLower().Trim().Replace(" ", string.Empty) && m.FirstName.ToLower().Trim().Replace(" ", string.Empty) == person.FirstName.ToLower().Trim().Replace(" ", string.Empty) && m.PersonId != person.PersonId && m.CompanyId == person.CompanyId) > 0)
                        {
                            return -3;
                        }
                    }

                    var personToUpdate = db.People.Find(person.PersonId);
                    if (personToUpdate == null)
                    {
                        return -2;
                    }
                    personToUpdate.FirstName = person.FirstName;
                    personToUpdate.LastName = person.LastName;
                    personToUpdate.Email = person.Email;
                    personToUpdate.Designation = person.Designation;
                    personToUpdate.PhoneNumber = person.PhoneNumber;
                    personToUpdate.CompanyId = person.CompanyId;
                    db.SaveChanges();
                    return 2;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public bool DeletePersonCheckReferences(int personId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.ProjectReps.Count(m => m.PersonId == personId) > 0)
                    {
                        return false;
                    }

                    var myObj = db.People.Where(s => s.PersonId == personId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.People.Remove(myObj[0]);
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
        public Person GetPerson(int personId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.People.Where(s => s.PersonId == personId).ToList();
                    if (!myObj.Any())
                    {
                        return new Person();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Person();
            }
        }

        public Person GetPersonByCompanyId(long companyId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.People.Where(s => s.CompanyId == companyId).ToList();
                    if (!myObj.Any())
                    {
                        return new Person();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Person();
            }
        }

        public List<Person> GetAllOrderedPersonsByCompanyId(int companyId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.People.Where(m => m.CompanyId == companyId).Include("Company").ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Person>();
                    }
                    return myObjList.OrderBy(m => m.LastName).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Person>();
            }
        }
	}
	
}
