using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class IncidentHistoryServices
	{
        public List<IncidentHistory> GetAllOrderedIncidentHistories(int itemsPerPage, int pageNumber, out int dataCount)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.IncidentHistories.Any())
                    {
                        dataCount = db.IncidentHistories.Count();
                        var myObjList = db.IncidentHistories.OrderBy(m => m.IncidentHistoryId).Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).Include("Company").Include("IncidentType").ToList();

                        if (!myObjList.Any())
                        {
                            return new List<IncidentHistory>();
                        }
                        myObjList.ForEach(m =>
                        {
                            m.IncidentTypeName = m.IncidentType.Name;
                            m.CompanyName = m.Company.Name;
                            m.Date = m.IncidentDate.ToString("dd/MM/yyyy");
                        });
                        return myObjList.OrderBy(m => m.IncidentHistoryId).ToList();
                    }

                    dataCount = 0;
                    return new List<IncidentHistory>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                dataCount = 0;
                return new List<IncidentHistory>();
            }
        }

        public List<IncidentHistory> GetAllOrderedIncidentHistories()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.IncidentHistories.Include("Company").Include("IncidentType").ToList();

                    if (!myObjList.Any())
                    {
                        return new List<IncidentHistory>();
                    }
                    myObjList.ForEach(m =>
                    {
                        m.IncidentTypeName = m.IncidentType.Name;
                        m.CompanyName = m.Company.Name;
                        m.Date = m.IncidentDate.ToString("dd/MM/yyyy");
                    });
                    return myObjList.OrderBy(m => m.IncidentHistoryId).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<IncidentHistory>();
            }
        }
        public int AddIncidentHistory(IncidentHistory incidentHistory)
        {
            try
            {
                if (incidentHistory == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var processedItem = db.IncidentHistories.Add(incidentHistory);
                    db.SaveChanges();
                    return processedItem.IncidentHistoryId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateIncidentHistory(IncidentHistory incidentHistory)
        {
            try
            {
                if (incidentHistory == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    db.IncidentHistories.Attach(incidentHistory);
                    db.Entry(incidentHistory).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public bool DeleteIncidentHistoryCheckReferences(int incidentHistoryId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.IncidentHistories.Where(s => s.IncidentHistoryId == incidentHistoryId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.IncidentHistories.Remove(myObj[0]);
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

        public IncidentHistory GetIncidentHistory(int incidentHistoryId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.IncidentHistories.Where(s => s.IncidentHistoryId == incidentHistoryId).ToList();
                    if (!myObj.Any())
                    {
                        return new IncidentHistory();
                    }
                   
                    myObj[0].Date = myObj[0].IncidentDate.ToString("yyyy/MM/dd");
                  
                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new IncidentHistory();
            }
        }

        public List<IncidentHistory> GetIncidentHistoriesByCompany(int companyId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.IncidentHistories.Where(s => s.CompanyId == companyId).ToList();
                    if (!myObjList.Any())
                    {
                        return new List<IncidentHistory>();
                    }
                    myObjList.ForEach(m =>
                    {
                        m.IncidentTypeName = m.IncidentType.Name;
                        m.CompanyName = m.Company.Name;
                        m.Date = m.IncidentDate.ToString("dd/MM/yyyy");
                    });
                    return myObjList.OrderBy(m => m.IncidentDate).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<IncidentHistory>();
            }
        }

        public List<IncidentHistory> GetIncidentHistoriesByType(int incidentTypeId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.IncidentHistories.Where(s => s.IncidentTypeId == incidentTypeId).ToList();
                    if (!myObjList.Any())
                    {
                        return new List<IncidentHistory>();
                    }
                    myObjList.ForEach(m =>
                    {
                        m.IncidentTypeName = m.IncidentType.Name;
                        m.CompanyName = m.Company.Name;
                        m.Date = m.IncidentDate.ToString("dd/MM/yyyy");
                    });
                    return myObjList.OrderBy(m => m.IncidentDate).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<IncidentHistory>();
            }
        }

        public List<IncidentReportObject> GetOrderedIncidentReportObjects(IncidentQuery queryBuilder)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {

                    if (queryBuilder != null)
                    {

                        if (queryBuilder.StartDate != null && queryBuilder.StartDate.Value.Year > 0001 && queryBuilder.EndDate != null && queryBuilder.EndDate.Value.Year > 0001)
                        {
                           var query = (

                             from
                              t in db.IncidentHistories
                             where
                                 (t.IncidentDate >= queryBuilder.StartDate) && (t.IncidentDate <= queryBuilder.EndDate)
                                 
                             join
                                h in db.Companies on t.CompanyId equals h.CompanyId
                             where
                                 ((queryBuilder.CompanyId > 0 && h.CompanyId == queryBuilder.CompanyId)
                                 ||
                                     (queryBuilder.CompanyId < 1 && h.CompanyId == t.CompanyId))
                             
                             join
                                 d in db.IncidentTypes on t.IncidentTypeId equals d.IncidentTypeId
                             where
                             queryBuilder.IncidentTypeId > 0 && d.IncidentTypeId == queryBuilder.IncidentTypeId
                             ||
                             (queryBuilder.IncidentTypeId < 1 && d.IncidentTypeId == t.IncidentTypeId)
                             
                             select new IncidentReportObject
                             {
                                 IncidentName = t.Title,
                                 IncidentTypeName = d.Name,
                                 IncidentDate = t.IncidentDate,
                                 CompanyName = h.Name,
                                 ReportedBy = t.ReportedBy,
                                 Location = t.Location,
                                 Description = t.Description
                             }).ToList();

                        if (!query.Any())
                        {
                            return new List<IncidentReportObject>();
                        }
                            query.ForEach(m =>
                            {
                                m.IncidentDateStr = m.IncidentDate.ToString("dd/MM/yyyy");
                            });
                        return query.OrderBy(v => v.IncidentName).ToList();

                    }

                        if (queryBuilder.StartDate != null && queryBuilder.StartDate.Value.Year > 0001 && ((queryBuilder.EndDate != null && queryBuilder.EndDate.Value.Year == 0001) || (queryBuilder.EndDate == null)))
                        {
                       
                        var query = (

                         from
                              t in db.IncidentHistories
                         where
                             t.IncidentDate == queryBuilder.StartDate
                         join
                            h in db.Companies on t.CompanyId equals h.CompanyId
                         where
                             ((queryBuilder.CompanyId > 0 && h.CompanyId == queryBuilder.CompanyId)
                             ||
                                 (queryBuilder.CompanyId < 1 && h.CompanyId == t.CompanyId))

                         join
                             d in db.IncidentTypes on t.IncidentTypeId equals d.IncidentTypeId
                         where
                         queryBuilder.IncidentTypeId > 0 && d.IncidentTypeId == queryBuilder.IncidentTypeId
                         ||
                         (queryBuilder.IncidentTypeId < 1 && d.IncidentTypeId == t.IncidentTypeId)

                         select new IncidentReportObject
                         {
                             IncidentName = t.Title,
                             IncidentTypeName = d.Name,
                             IncidentDate = t.IncidentDate,
                             CompanyName = h.Name,
                             ReportedBy = t.ReportedBy,
                             Location = t.Location,
                             Description = t.Description
                         }).ToList();

                        if (!query.Any())
                        {
                            return new List<IncidentReportObject>();
                        }

                        query.ForEach(m =>
                        {
                            m.IncidentDateStr = m.IncidentDate.ToString("dd/MM/yyyy");
                        });
                        return query.OrderBy(v => v.IncidentName).ToList();  
                    }

                        if (((queryBuilder.StartDate != null && queryBuilder.StartDate.Value.Year == 0001) || (queryBuilder.StartDate == null)) && ((queryBuilder.EndDate != null && queryBuilder.EndDate.Value.Year == 0001) || queryBuilder.EndDate == null))
                    {
                        var query = (

                         from
                              t in db.IncidentHistories
                         join
                            h in db.Companies on t.CompanyId equals h.CompanyId
                         where
                             ((queryBuilder.CompanyId > 0 && h.CompanyId == queryBuilder.CompanyId)
                             ||
                                 (queryBuilder.CompanyId < 1 && h.CompanyId == t.CompanyId))

                         join
                             d in db.IncidentTypes on t.IncidentTypeId equals d.IncidentTypeId
                         where
                         queryBuilder.IncidentTypeId > 0 && d.IncidentTypeId == queryBuilder.IncidentTypeId
                         ||
                         (queryBuilder.IncidentTypeId < 1 && d.IncidentTypeId == t.IncidentTypeId)

                         select new IncidentReportObject
                         {
                             IncidentName = t.Title,
                             IncidentTypeName = d.Name,
                             IncidentDate = t.IncidentDate,
                             CompanyName = h.Name,
                             ReportedBy = t.ReportedBy,
                             Location = t.Location,
                             Description = t.Description
                         }).ToList();

                        if (!query.Any())
                        {
                            return new List<IncidentReportObject>();
                        }
                        query.ForEach(m =>
                        {
                            m.IncidentDateStr = m.IncidentDate.ToString("dd/MM/yyyy");
                        });
                        return query.OrderBy(v => v.IncidentName).ToList();
                    }
                    
                    }
                    return new List<IncidentReportObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<IncidentReportObject>();
            }
        }

          public List<string> GetIncidentYears()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = (from x in db.IncidentHistories
                                     select x.IncidentDate).ToList();
                    if (!myObjList.Any())
                    {
                        return new List<string>();
                    }
                    var newList = new List<string>();
                    myObjList.ForEach(m =>
                    {
                        var tty = m.Year.ToString(CultureInfo.InvariantCulture);
                        if (!string.IsNullOrEmpty(tty))
                        {
                            if (!newList.Exists(x => x == tty))
                            {
                                newList.Add(tty);
                            }
                        }
                    });
                    return newList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<string>();
            }
        }

	}
	
}
