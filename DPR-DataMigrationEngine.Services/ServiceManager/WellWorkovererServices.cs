using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class WellWorkoverServices
	{
        public List<WellWorkover> GetAllOrderedWellWorkovers(int itemsPerPage, int pageNumber, out int dataCount)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.WellWorkovers.Any())
                    {
                        dataCount = db.WellWorkovers.Count();
                        var myObjList = db.WellWorkovers.OrderBy(m => m.WellWorkOverId).Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).Include("Equipment").Include("Well").Include("WellWorkOverReason").ToList();
                        if (!myObjList.Any())
                        {
                            return new List<WellWorkover>();
                        }
                        return myObjList;
                    }

                    dataCount = 0;
                    return new List<WellWorkover>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                dataCount = 0;
                return new List<WellWorkover>();
            }
        }

        public List<WellWorkoverReportObject> GetMoreWellWorkovers(int itemsPerPage, int pageNumber)
        {
            try   
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                 
                        var myObjList = db.WellWorkovers.OrderBy(m => m.WellWorkOverId).Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).Include("Equipment").Include("Well").Include("WellWorkOverReason").ToList();
                        if (!myObjList.Any())
                        {
                            return new List<WellWorkoverReportObject>();
                        }

                    var tsdfg = new List<WellWorkoverReportObject>();
                    myObjList.ForEach(m => tsdfg.Add(new WellWorkoverReportObject
                    {
                        WellWorkOverId = m.WellWorkOverId,
                        WellName = m.Well.Name,
                        EquipmentName = m.Equipment.Name,
                        Reason = m.WellWorkOverReason.Title,
                        Year = m.Year,
                        MonthStr = Enum.GetName(typeof(Months), m.Month)
                    }));

                    return tsdfg;
                  
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellWorkoverReportObject>();
            }
        }
        public List<WellWorkover> GetWellWorkoversForReport(int itemsPerPage, int pageNumber)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList =
                        (from ww in
                            db.WellWorkovers.OrderBy(m => m.WellWorkOverId)
                                .Skip((pageNumber - 1)*itemsPerPage)
                                .Take(itemsPerPage)
                                .Include("Equipment")
                                .Include("Well")
                                .Include("WellWorkOverReason")

                            join wt in db.WellTypes on ww.Well.WellTypeId equals wt.WellTypeId
                            join f in db.Fields on ww.Well.FieldId equals f.FieldId
                            join b in db.Blocks on f.BlockId equals b.BlockId
                            join c in db.Companies on f.CompanyId equals c.CompanyId
                            join t in db.Terrains on f.TerrainId equals t.TerrainId
                            join z in db.Zones on f.ZoneId equals z.ZoneId
                            select new WellWorkover
                            {
                                    WellName = ww.Well.Name,
                                    ZoneName = z.Name,
                                    TerrainName = t.Name,
                                    WellTypeName = wt.Title,
                                    CompanyName = c.Name,
                                    EquipmentName =  ww.Equipment.Name,
                                    DatecomPletedString = ww.DateCompleted,
                                    Reason = ww.WellWorkOverReason.Title
                            }).ToList();
                    
                        if (!myObjList.Any())
                        {
                            return new List<WellWorkover>();
                        }
                        return myObjList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellWorkover>();
            }
        }
        public int AddWellWorkover(WellWorkover wellWorkover)
        {
            try
            {
                if (wellWorkover == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var processedItem = db.WellWorkovers.Add(wellWorkover);
                    db.SaveChanges();
                    return processedItem.WellWorkOverId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int AddWellWorkoverCheckDuplicates(WellWorkover wellWorkover, int wellId, int wellWorkOverReasonId, int month, long year)
        {
            try
            {
                if (wellWorkover == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.WellWorkovers.Count(m => m.WellId == wellId && m.WellWorkOverReasonId == wellWorkOverReasonId && m.EquipmentId == wellWorkover.EquipmentId && m.Month == month && m.Year == year) > 0)
                    {
                        return -3;
                    }
                    
                    var processedItem = db.WellWorkovers.Add(wellWorkover);
                    db.SaveChanges();
                    return processedItem.WellWorkOverId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateWellWorkover(WellWorkover wellWorkover)
        {
            try
            {
                if (wellWorkover == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    db.WellWorkovers.Attach(wellWorkover);
                    db.Entry(wellWorkover).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public bool DeleteWellWorkoverCheckReferences(int wellWorkoverId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                   
                    var myObj = db.WellWorkovers.Where(s => s.WellWorkOverId == wellWorkoverId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.WellWorkovers.Remove(myObj[0]);
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
        public WellWorkover GetWellWorkover(int wellWorkoverId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.WellWorkovers.Where(s => s.WellWorkOverId == wellWorkoverId).ToList();
                    if (!myObj.Any())
                    {
                        return new WellWorkover();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new WellWorkover();
            }
        }
        public List<WellWorkover> GetAllOrderedWellWorkoversByMonth(int searchMonth, int searchYear)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (searchYear < 1)
                    {
                        return new List<WellWorkover>();
                    }

                    var myObjList = db.WellWorkovers.Where(m => m.Year.Equals(searchYear) && m.Month.Equals(searchMonth)).Include("Equipment").Include("Well").Include("WellWorkOverReason").ToList();
                    if (!myObjList.Any())
                    {
                        return new List<WellWorkover>();
                    }
                    return myObjList.OrderBy(m => m.WellWorkOverId).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellWorkover>();
            }
        }
        public List<WellWorkoverReportObject> GetMoreWellWorkoverReport(int itemsPerPage, int pageNumber, int searchMonth, int searchYear)
        {
            try
            {
                //var newList = new List<WellWorkoverReportObject>();

                if (searchYear < 1)
                {
                    return new List<WellWorkoverReportObject>();
                }

                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (searchMonth > 0 && searchYear > 0)
                    {
                        //var year = searchYear.ToString(CultureInfo.InvariantCulture);
                        //var month = searchMonth.ToString(CultureInfo.InvariantCulture);
                        var myObjList =
                            (from ww in
                        db.WellWorkovers.Where(m => m.Year.Equals(searchYear) && m.Month.Equals(searchMonth))
                            .Skip((pageNumber - 1) * itemsPerPage)
                            .Take(itemsPerPage)
                            .Include("Equipment")
                            .Include("Well")
                            .Include("WellWorkOverReason")

                             join wt in db.WellTypes on ww.Well.WellTypeId equals wt.WellTypeId
                             join f in db.Fields on ww.Well.FieldId equals f.FieldId
                             join b in db.Blocks on f.BlockId equals b.BlockId
                             join c in db.Companies on f.CompanyId equals c.CompanyId
                             join t in db.Terrains on f.TerrainId equals t.TerrainId
                             join z in db.Zones on f.ZoneId equals z.ZoneId
                             select new WellWorkoverReportObject
                             {
                                 WellName = ww.Well.Name,
                                 ZoneName = z.Name,
                                 TerrainName = t.Name,
                                 WellTypeName = wt.Title,
                                 CompanyName = c.Name,
                                 EquipmentName = ww.Equipment.Name,
                                 DatecomPletedString = ww.DateCompleted,
                                 Reason = ww.WellWorkOverReason.Title
                             }).ToList();
                        if (!myObjList.Any())
                        {
                            return new List<WellWorkoverReportObject>();
                        }
                        return myObjList;
                    }

                    if (searchMonth < 1 && searchYear > 0)
                    {
                        var year = searchYear;
                        var myObjList =
                            (from ww in
                        db.WellWorkovers.Where(m => m.Year.Equals(year))
                            .Skip((pageNumber - 1) * itemsPerPage)
                            .Take(itemsPerPage)
                            .Include("Equipment")
                            .Include("Well")
                            .Include("WellWorkOverReason")
                            join wt in db.WellTypes on ww.Well.WellTypeId equals wt.WellTypeId
                             join f in db.Fields on ww.Well.FieldId equals f.FieldId
                             join b in db.Blocks on f.BlockId equals b.BlockId
                             join c in db.Companies on f.CompanyId equals c.CompanyId
                             join t in db.Terrains on f.TerrainId equals t.TerrainId
                             join z in db.Zones on f.ZoneId equals z.ZoneId
                             select new WellWorkoverReportObject
                             {
                                 WellName = ww.Well.Name,
                                 ZoneName = z.Name,
                                 TerrainName = t.Name,
                                 WellTypeName = wt.Title,
                                 CompanyName = c.Name,
                                 EquipmentName = ww.Equipment.Name,
                                 DatecomPletedString = ww.DateCompleted,
                                 Reason = string.IsNullOrEmpty(ww.WellWorkOverReason.Title) ? " " : ww.WellWorkOverReason.Title
                             }).ToList();
                        if (!myObjList.Any())
                        {
                            return new List<WellWorkoverReportObject>();
                        }
                        return myObjList;
                    }

                    return new List<WellWorkoverReportObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellWorkoverReportObject>();
            }
        }
        public List<long> GetWellWorkoverYears()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = (from x in db.WellWorkovers
                        where x.Year >0
                        select x.Year).ToList();
                    if (!myObjList.Any())
                    {
                        return new List<long>();
                    }
                    var newList = new List<long>();
                    myObjList.ForEach(m =>
                    {
                        if (!newList.Exists(x => x == m))
                        {
                            newList.Add(m);
                        }
                    });
                    return newList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<long>();
            }
        }  
        public List<WellWorkoverReportObject> GetOrderedWellWorkoverReportObjects(WellWorkoverQuery queryBuilder)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {

                    if (queryBuilder != null)
                    {
                        if (queryBuilder.StartDate != null && queryBuilder.StartDate.Value.Year > 0001 && ((queryBuilder.EndDate != null && queryBuilder.EndDate.Value.Year == 0001) || (queryBuilder.EndDate == null)))
                        {
                            var startYear = queryBuilder.StartDate.Value.Year;
                            var startMonth = queryBuilder.StartDate.Value.Month;
                           

                            var query = (
                                  
                                    from
                                        q in db.WellWorkovers.Where(m =>
                                         (queryBuilder.WellId > 0 && m.WellId == queryBuilder.WellId && queryBuilder.WorkoverReasonId < 1 && queryBuilder.EquipmentId < 1 && (m.Year == startYear && m.Month == startMonth)) ||
                                         (queryBuilder.EquipmentId > 0 && queryBuilder.WellId < 1 && queryBuilder.WorkoverReasonId < 1 && m.EquipmentId == queryBuilder.EquipmentId && (m.Year == startYear && m.Month == startMonth)) ||
                                         (queryBuilder.WorkoverReasonId > 0 && queryBuilder.WellId < 1 && queryBuilder.EquipmentId < 1 && m.WellWorkOverReasonId == queryBuilder.WorkoverReasonId && (m.Year == startYear && m.Month == startMonth)) ||
                                         (queryBuilder.WellId > 0 && queryBuilder.EquipmentId > 0 && queryBuilder.WorkoverReasonId < 1 && m.WellId == queryBuilder.WellId && m.EquipmentId == queryBuilder.EquipmentId && (m.Year == startYear && m.Month == startMonth)) ||
                                         (queryBuilder.WellId > 0 && queryBuilder.EquipmentId < 1 && queryBuilder.WorkoverReasonId > 0 && m.WellId == queryBuilder.WellId && m.WellWorkOverReasonId == queryBuilder.WorkoverReasonId && (m.Year == startYear && m.Month == startMonth)) ||
                                         (queryBuilder.WellId < 1 && queryBuilder.EquipmentId > 0 && queryBuilder.WorkoverReasonId > 0 && m.EquipmentId == queryBuilder.EquipmentId && m.WellWorkOverReasonId == queryBuilder.WorkoverReasonId && (m.Year == startYear && m.Month == startMonth)) ||
                                         (queryBuilder.WellId < 1 && queryBuilder.EquipmentId < 1 && queryBuilder.WorkoverReasonId < 1 && (m.Year == startYear && m.Month == startMonth))).Include("Equipment").Include("Well").Include("WellWorkOverReason")

                                        join
                                        t in db.Fields on q.Well.FieldId equals t.FieldId
                                        join i in db.Blocks on t.BlockId equals i.BlockId
                                        join c in db.WellClassifications on  q.Well.WellId equals c.WellId
                                        join
                                        p in db.WellClasses on c.WellClassId equals p.WellClassId
                                        where ((p.WellClassId == queryBuilder.WellClassId && queryBuilder.WellClassId > 0 && c.WellClassId == p.WellClassId) || (p.WellClassId == c.WellClassId && queryBuilder.WellClassId < 1))
                                         
                                        join
                                        x in db.WellTypes on q.Well.WellTypeId equals x.WellTypeId
                                        where
                                        ((queryBuilder.WellTypeId > 0 && x.WellTypeId == queryBuilder.WellTypeId && q.Well.WellTypeId == x.WellTypeId) ||
                                        (q.Well.WellTypeId == x.WellTypeId && queryBuilder.WellTypeId < 1))
                                        
                                        join
                                        y in db.Terrains on t.TerrainId equals y.TerrainId
                                        where ((queryBuilder.TerrainId > 0 && y.TerrainId == queryBuilder.TerrainId) || (y.TerrainId == t.TerrainId && queryBuilder.TerrainId < 1)) 

                                         join
                                         s in db.Zones on t.ZoneId equals s.ZoneId
                                         where
                                         ((queryBuilder.ZoneId > 0 && s.ZoneId == queryBuilder.ZoneId)
                                         ||
                                         (s.ZoneId == t.ZoneId && queryBuilder.ZoneId < 1))
                                         
                                        join
                                          h in db.Companies on t.CompanyId equals h.CompanyId
                                        where ((queryBuilder.CompanyId > 0 && h.CompanyId == queryBuilder.CompanyId ) || (queryBuilder.CompanyId < 1 && t.CompanyId == h.CompanyId))
                                        
                                
                                select new WellWorkoverReportObject
                                {
                                    EquipmentName = q.Equipment.Name,
                                    CompanyName = h.Name,
                                    WellTypeName = x.Title,
                                    WellClassName = p.Name,
                                    WellName = q.Well.Name,
                                    ZoneName = s.Name,
                                    TerrainName = y.Name,
                                    Reason = q.WellWorkOverReason.Title,
                                    Year = q.Year,
                                    Month = q.Month,
                                    DateCompleted = q.DateCompleted
                                }).ToList();
                              
                            if (!query.Any())
                            {
                                return new List<WellWorkoverReportObject>();
                            }
                            query.ForEach(j =>
                            {
                                
                                if (j.Month > 0)
                                {
                                    j.WorkoverPeriod = Enum.GetName(typeof(Months), j.Month) + "/" + j.Year;

                                }

                                if (j.Month < 1)
                                {
                                    j.WorkoverPeriod = Enum.GetName(typeof(Months), 1) + "/" + j.Year;

                                }


                            });
                            return query.OrderBy(v => v.WellName).ToList();
                        }

                        if (queryBuilder.StartDate != null && queryBuilder.StartDate.Value.Year > 0001 && queryBuilder.EndDate != null && queryBuilder.EndDate.Value.Year > 0001)
                        {
                            var startYear = queryBuilder.StartDate.Value.Year;
                            var startMonth = queryBuilder.StartDate.Value.Month;
                            var endYear = queryBuilder.EndDate.Value.Year;
                            var endMonth = queryBuilder.EndDate.Value.Month;

                            var query = (

                                    from
                                        q in db.WellWorkovers.Where(m =>
                                         (queryBuilder.WellId > 0 && m.WellId == queryBuilder.WellId && queryBuilder.WorkoverReasonId < 1 && queryBuilder.EquipmentId < 1 && ((m.Year >= startYear && m.Month >= startMonth) && (m.Year <= endYear && m.Month <= endMonth))) ||
                                         (queryBuilder.EquipmentId > 0 && queryBuilder.WellId < 1 && queryBuilder.WorkoverReasonId < 1 && m.EquipmentId == queryBuilder.EquipmentId && ((m.Year >= startYear && m.Month >= startMonth) && (m.Year <= endYear && m.Month <= endMonth))) ||
                                         (queryBuilder.WorkoverReasonId > 0 && queryBuilder.WellId < 1 && queryBuilder.EquipmentId < 1 && m.WellWorkOverReasonId == queryBuilder.WorkoverReasonId && ((m.Year >= startYear && m.Month >= startMonth) && (m.Year <= endYear && m.Month <= endMonth))) ||
                                         (queryBuilder.WellId > 0 && queryBuilder.EquipmentId > 0 && queryBuilder.WorkoverReasonId < 1 && m.WellId == queryBuilder.WellId && m.EquipmentId == queryBuilder.EquipmentId && ((m.Year >= startYear && m.Month >= startMonth) && (m.Year <= endYear && m.Month <= endMonth))) ||
                                         (queryBuilder.WellId > 0 && queryBuilder.EquipmentId < 1 && queryBuilder.WorkoverReasonId > 0 && m.WellId == queryBuilder.WellId && m.WellWorkOverReasonId == queryBuilder.WorkoverReasonId && ((m.Year >= startYear && m.Month >= startMonth) && (m.Year <= endYear && m.Month <= endMonth))) ||
                                         (queryBuilder.WellId < 1 && queryBuilder.EquipmentId > 0 && queryBuilder.WorkoverReasonId > 0 && m.EquipmentId == queryBuilder.EquipmentId && m.WellWorkOverReasonId == queryBuilder.WorkoverReasonId && ((m.Year >= startYear && m.Month >= startMonth) && (m.Year <= endYear && m.Month <= endMonth))) ||
                                         (queryBuilder.WellId < 1 && queryBuilder.EquipmentId < 1 && queryBuilder.WorkoverReasonId < 1 && ((m.Year >= startYear && m.Month >= startMonth) && (m.Year <= endYear && m.Month <= endMonth)))).Include("Equipment").Include("Well").Include("WellWorkOverReason")

                                    join
                                        t in db.Fields on q.Well.FieldId equals t.FieldId
                                    join i in db.Blocks on t.BlockId equals i.BlockId
                                    join c in db.WellClassifications on q.Well.WellId equals c.WellId
                                    join
                                     p in db.WellClasses on c.WellClassId equals p.WellClassId
                                    where ((p.WellClassId == queryBuilder.WellClassId && queryBuilder.WellClassId > 0 && c.WellClassId == p.WellClassId) || (p.WellClassId == c.WellClassId && queryBuilder.WellClassId < 1))

                                    join
                                    x in db.WellTypes on q.Well.WellTypeId equals x.WellTypeId
                                    where
                                    ((queryBuilder.WellTypeId > 0 && x.WellTypeId == queryBuilder.WellTypeId && q.Well.WellTypeId == x.WellTypeId) ||
                                    (q.Well.WellTypeId == x.WellTypeId && queryBuilder.WellTypeId < 1))

                                    join
                                    y in db.Terrains on t.TerrainId equals y.TerrainId
                                    where ((queryBuilder.TerrainId > 0 && y.TerrainId == queryBuilder.TerrainId) || (y.TerrainId == t.TerrainId && queryBuilder.TerrainId < 1))

                                    join
                                    s in db.Zones on t.ZoneId equals s.ZoneId
                                    where
                                    ((queryBuilder.ZoneId > 0 && s.ZoneId == queryBuilder.ZoneId)
                                    ||
                                    (s.ZoneId == t.ZoneId && queryBuilder.ZoneId < 1))

                                    join
                                      h in db.Companies on t.CompanyId equals h.CompanyId
                                    where ((queryBuilder.CompanyId > 0 && h.CompanyId == queryBuilder.CompanyId) || (queryBuilder.CompanyId < 1 && t.CompanyId == h.CompanyId))


                                    select new WellWorkoverReportObject
                                    {
                                        EquipmentName = q.Equipment.Name,
                                        CompanyName = h.Name,
                                        WellTypeName = x.Title,
                                        WellClassName = p.Name,
                                        WellName = q.Well.Name,
                                        ZoneName = s.Name,
                                        TerrainName = y.Name,
                                        Reason = q.WellWorkOverReason.Title,
                                        Year = q.Year,
                                        Month = q.Month,
                                        DateCompleted = q.DateCompleted
                                    }).ToList();

                            if (!query.Any())
                            {
                                return new List<WellWorkoverReportObject>();
                            }
                            query.ForEach(j =>
                            {

                                if (j.Month > 0)
                                {
                                    j.WorkoverPeriod = Enum.GetName(typeof(Months), j.Month) + "/" + j.Year;

                                }

                                if (j.Month < 1)
                                {
                                    j.WorkoverPeriod = Enum.GetName(typeof(Months), 1) + "/" + j.Year;

                                }


                            });
                            return query.OrderBy(v => v.WellName).ToList();
                        }

                        if (((queryBuilder.StartDate != null && queryBuilder.StartDate.Value.Year == 0001) || (queryBuilder.StartDate == null)) && ((queryBuilder.EndDate != null && queryBuilder.EndDate.Value.Year == 0001) || queryBuilder.EndDate == null))
                        {
                            var query = (

                                from
                                    q in db.WellWorkovers.Where(m =>
                                         (queryBuilder.WellId > 0 && m.WellId == queryBuilder.WellId && queryBuilder.WorkoverReasonId < 1 && queryBuilder.EquipmentId < 1 ) ||
                                         (queryBuilder.EquipmentId > 0 && queryBuilder.WellId < 1 && queryBuilder.WorkoverReasonId < 1 && m.EquipmentId == queryBuilder.EquipmentId) ||
                                         (queryBuilder.WorkoverReasonId > 0 && queryBuilder.WellId < 1 && queryBuilder.EquipmentId < 1 && m.WellWorkOverReasonId == queryBuilder.WorkoverReasonId) ||
                                         (queryBuilder.WellId > 0 && queryBuilder.EquipmentId > 0 && queryBuilder.WorkoverReasonId < 1 && m.WellId == queryBuilder.WellId && m.EquipmentId == queryBuilder.EquipmentId) ||
                                         (queryBuilder.WellId > 0 && queryBuilder.EquipmentId < 1 && queryBuilder.WorkoverReasonId > 0 && m.WellId == queryBuilder.WellId && m.WellWorkOverReasonId == queryBuilder.WorkoverReasonId) ||
                                         (queryBuilder.WellId < 1 && queryBuilder.EquipmentId > 0 && queryBuilder.WorkoverReasonId > 0 && m.EquipmentId == queryBuilder.EquipmentId && m.WellWorkOverReasonId == queryBuilder.WorkoverReasonId) ||
                                         (queryBuilder.WellId < 1 && queryBuilder.EquipmentId < 1 && queryBuilder.WorkoverReasonId < 1 )).Include("Equipment").Include("Well").Include("WellWorkOverReason")

                                join
                                t in db.Fields on q.Well.FieldId equals t.FieldId
                                join i in db.Blocks on t.BlockId equals i.BlockId
                                join c in db.WellClassifications on q.Well.WellId equals c.WellId
                                join
                                 p in db.WellClasses on c.WellClassId equals p.WellClassId
                                where ((p.WellClassId == queryBuilder.WellClassId && queryBuilder.WellClassId > 0 && c.WellClassId == p.WellClassId) || (p.WellClassId == c.WellClassId && queryBuilder.WellClassId < 1))

                                join
                                x in db.WellTypes on q.Well.WellTypeId equals x.WellTypeId
                                where
                                ((queryBuilder.WellTypeId > 0 && x.WellTypeId == queryBuilder.WellTypeId && q.Well.WellTypeId == x.WellTypeId) ||
                                (q.Well.WellTypeId == x.WellTypeId && queryBuilder.WellTypeId < 1))

                                join
                                y in db.Terrains on t.TerrainId equals y.TerrainId
                                where ((queryBuilder.TerrainId > 0 && y.TerrainId == queryBuilder.TerrainId) || (y.TerrainId == t.TerrainId && queryBuilder.TerrainId < 1))

                                join
                                s in db.Zones on t.ZoneId equals s.ZoneId
                                where
                                ((queryBuilder.ZoneId > 0 && s.ZoneId == queryBuilder.ZoneId)
                                ||
                                (s.ZoneId == t.ZoneId && queryBuilder.ZoneId < 1))

                                join
                                  h in db.Companies on t.CompanyId equals h.CompanyId
                                where ((queryBuilder.CompanyId > 0 && h.CompanyId == queryBuilder.CompanyId) || (queryBuilder.CompanyId < 1 && t.CompanyId == h.CompanyId))


                                select new WellWorkoverReportObject
                                {
                                    EquipmentName = q.Equipment.Name,
                                    CompanyName = h.Name,
                                    WellTypeName = x.Title,
                                    WellClassName = p.Name,
                                    WellName = q.Well.Name,
                                    ZoneName = s.Name,
                                    TerrainName = y.Name,
                                    Reason = q.WellWorkOverReason.Title,
                                    Year = q.Year,
                                    Month = q.Month,
                                    DateCompleted = q.DateCompleted
                                }).ToList();

                            if (!query.Any())
                            {
                                return new List<WellWorkoverReportObject>();
                            }
                            query.ForEach(j =>
                            {
                                var monthOut = j.Month;

                                if (monthOut > 0)
                                {
                                    j.WorkoverPeriod = Enum.GetName(typeof(Months), monthOut) + "/" + j.Year;

                                }

                                if (monthOut < 1)
                                {
                                    j.WorkoverPeriod = Enum.GetName(typeof(Months), 1) + "/" + j.Year;

                                }
                                
                            });
                            return query.OrderBy(v => v.WellName).ToList();
                        }
                    }
                    return new List<WellWorkoverReportObject>();
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellWorkoverReportObject>();
            }
        }
	}
	
}
