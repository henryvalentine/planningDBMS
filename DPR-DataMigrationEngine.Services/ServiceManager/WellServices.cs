using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class WellServices
	{
        public List<Well> GetAllOrderedWells(int itemsPerPage, int pageNumber, out int dataCount)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    dataCount = db.Wells.Count();
                        var myObjList = (from w in db.Wells.OrderBy(m => m.Name)
                               .Skip((pageNumber - 1) * itemsPerPage)
                               .Take(itemsPerPage)
                               join wt in db.WellTypes on w.WellTypeId equals wt.WellTypeId
                               join wc in db.WellClassifications on w.WellId equals wc.WellId
                               join wcc in db.WellClasses on wc.WellClassId equals wcc.WellClassId
                               join f in db.Fields on w.FieldId equals f.FieldId
                               join b in db.Blocks on f.BlockId equals b.BlockId
                               join c in db.Companies on f.CompanyId equals c.CompanyId
                               join t in db.Terrains on f.TerrainId equals t.TerrainId
                               join z in db.Zones on f.ZoneId equals z.ZoneId
                               select new {w, wt, b, wc, wcc, f, c, t, z}).ToList();
                        if (!myObjList.Any())
                        {
                            return new List<Well>();
                        }
                        var newList = new List<Well>();
                        myObjList.ForEach(m =>
                        {
                            var w = m.w;
                            var b = m.b;
                            var z = m.z;
                            var wcc = m.wcc;
                            var t = m.t;
                            var c = m.c;
                            var f = m.f;
                            var wt = m.wt;

                            newList.Add(new Well
                            {
                                Name = w.Name,
                                WellId = w.WellId,
                                SpudDate = w.SpudDate,
                                TechnicalAllowable = w.TechnicalAllowable,
                                TotalDept = w.TotalDept,
                                Remarks = w.Remarks,
                                BlockName = b.Name,
                                ZoneName = z.Name,
                                WellClassName = wcc.Name,
                                TerrainName = t.Name,
                                FieldName = f.Name,
                                CompanyName = c.Name,
                                WellTypeName = wt.Title,
                            Date = (w.SpudDate != null) ? ((DateTime)w.SpudDate).ToString("yyyy/MM/dd") : DateTime.Now.ToString("yyyy/MM/dd")
                        });
                    });

                    return !newList.Any() ? new List<Well>() : newList;
                }
            }
            catch (Exception ex)
            {
                dataCount = 0;
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Well>();
            }
        }
        public List<Well> GetAllOrderedWells()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {

                    var myObjList = (from w in db.Wells.OrderBy(m => m.Name)
                        join wt in db.WellTypes on w.WellTypeId equals wt.WellTypeId
                        join wc in db.WellClassifications on w.WellId equals wc.WellId
                        join wcc in db.WellClasses on wc.WellClassId equals wcc.WellClassId
                        join f in db.Fields on w.FieldId equals f.FieldId
                        join b in db.Blocks on f.BlockId equals b.BlockId
                        join c in db.Companies on f.CompanyId equals c.CompanyId
                        join t in db.Terrains on f.TerrainId equals t.TerrainId
                        join z in db.Zones on f.ZoneId equals z.ZoneId
                        select new {w, wt, b, wc, wcc, f, c, t, z}).ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Well>();
                    }
                    var newList = new List<Well>();
                    myObjList.ForEach(m =>
                    {
                        var w = m.w;
                        var z = m.z;
                        var wcc = m.wcc;
                        var t = m.t;
                        var c = m.c;
                        var f = m.f;
                        var wt = m.wt;

                        newList.Add(new Well
                        {
                            Name = w.Name,
                            SpudDate = w.SpudDate,
                            TechnicalAllowable = w.TechnicalAllowable,
                            TotalDept = w.TotalDept,
                            Remarks = w.Remarks,
                            ZoneName = z.Name,
                            WellClassName = wcc.Name,
                            TerrainName = t.Name,
                            FieldName = f.Name,
                            CompanyName = c.Name,
                            WellTypeName = wt.Title,
                            Date = (w.SpudDate != null) ? ((DateTime)w.SpudDate).ToString("yyyy/MM/dd") : DateTime.Now.ToString("yyyy/MM/dd")
                        });
                    });

                    return !newList.Any() ? new List<Well>() : newList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Well>();
            }
        }
        public List<Well> GetWells()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = (from w in db.Wells.OrderBy(m => m.Name)
                                     join wt in db.WellTypes on w.WellTypeId equals wt.WellTypeId
                                     join wc in db.WellClassifications on w.WellId equals wc.WellId
                                     join wcc in db.WellClasses on wc.WellClassId equals wcc.WellClassId
                                     join f in db.Fields on w.FieldId equals f.FieldId
                                     join b in db.Blocks on f.BlockId equals b.BlockId
                                     join c in db.Companies on f.CompanyId equals c.CompanyId
                                     join t in db.Terrains on f.TerrainId equals t.TerrainId
                                     join z in db.Zones on f.ZoneId equals z.ZoneId
                                     select new { w, wt, b, wc, wcc, f, c, t, z }).ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Well>();
                    }
                    var newList = new List<Well>();
                    myObjList.ForEach(m =>
                    {
                        var w = m.w;
                        var b = m.b;
                        var z = m.z;
                        var wcc = m.wcc;
                        var t = m.t;
                        var c = m.c;
                        var f = m.f;
                        var wt = m.wt;

                        newList.Add(new Well
                        {
                            Name = w.Name,
                            WellId = w.WellId,
                            SpudDate = w.SpudDate,
                            TechnicalAllowable = w.TechnicalAllowable,
                            TotalDept = w.TotalDept,
                            Remarks = w.Remarks,
                            BlockName = b.Name,
                            ZoneName = z.Name,
                            WellClassName = wcc.Name,
                            TerrainName = t.Name,
                            FieldName = f.Name,
                            CompanyName = c.Name,
                            WellTypeName = wt.Title,
                            Date = (w.SpudDate != null)? ((DateTime)w.SpudDate).ToString("yyyy/MM/dd") : DateTime.Now.ToString("yyyy/MM/dd")
                        });
                    });

                    return !newList.Any() ? new List<Well>() : newList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Well>();
            }
        }
        public List<WellReportObject> GetWellObjects(int itemsPerPage, int pageNumber)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = (from w in db.Wells.OrderBy(m => m.Name)
                               .Skip((pageNumber - 1) * itemsPerPage)
                               .Take(itemsPerPage)
                               join wt in db.WellTypes on w.WellTypeId equals wt.WellTypeId
                               join wc in db.WellClassifications on w.WellId equals wc.WellId
                               join wcc in db.WellClasses on wc.WellClassId equals wcc.WellClassId
                               join f in db.Fields on w.FieldId equals f.FieldId
                               join b in db.Blocks on f.BlockId equals b.BlockId
                               join c in db.Companies on f.CompanyId equals c.CompanyId
                               join t in db.Terrains on f.TerrainId equals t.TerrainId
                               join z in db.Zones on f.ZoneId equals z.ZoneId
                               select new WellReportObject
                               {
                                    Name = w.Name,
                                    WellId = w.WellId,
                                    SpudDate = w.SpudDate,
                                    TechnicalAllowable = w.TechnicalAllowable > 0 ? (double)w.TechnicalAllowable : 0,
                                    TotalDept = w.TotalDept > 0 ? (double)w.TotalDept : 0,
                                    Remarks = w.Remarks,
                                    ZoneName = z.Name,
                                    BlockName = b.Name,
                                    WellClassName = wcc.Name,
                                    TerrainName = t.Name,
                                    FieldName = f.Name,
                                    CompanyName = c.Name,
                                    WellTypeName = wt.Title,
                               }).ToList();

                   if(!myObjList.Any())
                    {
                        return new List<WellReportObject>();
                    }
                    myObjList.ForEach(m =>
                    {
                        if (m.SpudDate != null)
                        {
                            m.Date = ((DateTime)m.SpudDate).ToString("yyyy/MM/dd");
                        }
                    });
                    
                   return myObjList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellReportObject>();
            }
        }
	    public List<WellReportObject> GetWellObjects(int itemsPerPage, int pageNumber, long companyId)
	    {
	        try
	        {
	            using (var db = new DPRDataMigrationEngineDBEntities())
	            {
                    var myObjList = (from w in db.Wells.OrderBy(m => m.Name)
                               .Skip((pageNumber - 1) * itemsPerPage)
                               .Take(itemsPerPage)
                                     join wt in db.WellTypes on w.WellTypeId equals wt.WellTypeId
                                     join wc in db.WellClassifications on w.WellId equals wc.WellId
                                     join wcc in db.WellClasses on wc.WellClassId equals wcc.WellClassId
                                     join f in db.Fields on w.FieldId equals f.FieldId
                                     join b in db.Blocks on f.BlockId equals b.BlockId
                                     join c in db.Companies on f.CompanyId equals c.CompanyId
                                     join t in db.Terrains on f.TerrainId equals t.TerrainId
                                     join z in db.Zones on f.ZoneId equals z.ZoneId
                                     select new WellReportObject
                                     {
                                         Name = w.Name,
                                         WellId = w.WellId,
                                         SpudDate = w.SpudDate,
                                         TechnicalAllowable = w.TechnicalAllowable > 0 ? (double)w.TechnicalAllowable : 0,
                                         TotalDept = w.TotalDept > 0 ? (double)w.TotalDept : 0,
                                         Remarks = w.Remarks,
                                         ZoneName = z.Name,
                                         BlockName = b.Name,
                                         WellClassName = wcc.Name,
                                         TerrainName = t.Name,
                                         FieldName = f.Name,
                                         CompanyName = c.Name,
                                         WellTypeName = wt.Title
                                     }).ToList();

                    if (!myObjList.Any())
                    {
                        return new List<WellReportObject>();
                    }
                    myObjList.ForEach(m =>
                    {
                        if (m.SpudDate != null)
                        {
                            m.Date = ((DateTime)m.SpudDate).ToString("yyyy/MM/dd");
                        }
                    });

                    return myObjList;
	            }
	        }
	        catch (Exception ex)
	        {
                ErrorLogger.LogEror(ex.StackTrace,ex.Source,ex.Message);
                return new List<WellReportObject>();
	        }
	    }
        public List<WellReportObject> GetOrderedWellReportObjects(WellQuery queryBuilder)
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
                                                 from w in db.Wells.Where(x => (x.SpudDate != null && x.SpudDate.Value.Year == startYear && x.SpudDate.Value.Month == startMonth)).OrderBy(m => m.Name)
                                                 join wt in db.WellTypes on w.WellTypeId equals wt.WellTypeId where ((queryBuilder.WellTypeId > 0 && queryBuilder.WellTypeId == wt.WellTypeId) || (queryBuilder.WellTypeId < 1 && w.WellTypeId == wt.WellTypeId))
                                                 join wc in db.WellClassifications on w.WellId equals wc.WellId
                                                 join wcc in db.WellClasses on wc.WellClassId equals wcc.WellClassId
                                                 join f in db.Fields on w.FieldId equals f.FieldId  where ((queryBuilder.FieldId > 0 && queryBuilder.FieldId == f.FieldId) || (queryBuilder.FieldId < 1 && w.FieldId == f.FieldId))
                                                 join b in db.Blocks on f.BlockId equals b.BlockId
                                                 where ((queryBuilder.BlockId > 0 && queryBuilder.BlockId == b.BlockId) || (queryBuilder.BlockId < 1 && f.BlockId == b.BlockId))
                                                 join c in db.Companies on f.CompanyId equals c.CompanyId  where ((queryBuilder.CompanyId > 0 && queryBuilder.CompanyId == c.CompanyId) || (queryBuilder.CompanyId < 1 && f.CompanyId == c.CompanyId))
                                                 join t in db.Terrains on f.TerrainId equals t.TerrainId  where ((queryBuilder.TerrainId > 0 && queryBuilder.TerrainId == t.TerrainId) || (queryBuilder.TerrainId < 1 && f.TerrainId == t.TerrainId))
                                                 join z in db.Zones on f.ZoneId equals z.ZoneId
                                                 select new WellReportObject
                                                 {
                                                     Name = w.Name,
                                                     WellId = w.WellId,
                                                     SpudDate = w.SpudDate,
                                                     TechnicalAllowable = w.TechnicalAllowable > 0 ? (double)w.TechnicalAllowable : 0,
                                                     TotalDept = w.TotalDept > 0 ? (double)w.TotalDept : 0,
                                                     Remarks = w.Remarks,
                                                     ZoneName = z.Name,
                                                     BlockName = b.Name,
                                                     WellClassName = wcc.Name,
                                                     TerrainName = t.Name,
                                                     FieldName = f.Name,
                                                     CompanyName = c.Name,
                                                     WellTypeName = wt.Title
                                                 }).ToList();

                                if (!query.Any())
                                {
                                    return new List<WellReportObject>();
                                }
                                query.ForEach(m =>
                                {
                                    if (m.SpudDate != null)
                                    {
                                        m.Date = ((DateTime)m.SpudDate).ToString("dd/MM/yyyy");
                                    }
                                });

                                return query;
                            }

                        if (((queryBuilder.StartDate != null && queryBuilder.StartDate.Value.Year == 0001) || (queryBuilder.StartDate == null)) && ((queryBuilder.EndDate != null && queryBuilder.EndDate.Value.Year == 0001) || queryBuilder.EndDate == null))
                            {
                                var query = (
                                                 from w in db.Wells.OrderBy(m => m.Name)
                                                 join wt in db.WellTypes on w.WellTypeId equals wt.WellTypeId
                                                 where ((queryBuilder.WellTypeId > 0 && queryBuilder.WellTypeId == wt.WellTypeId) || (queryBuilder.WellTypeId < 1 && w.WellTypeId == wt.WellTypeId))
                                                 join wc in db.WellClassifications on w.WellId equals wc.WellId
                                                 join wcc in db.WellClasses on wc.WellClassId equals wcc.WellClassId
                                                 join f in db.Fields on w.FieldId equals f.FieldId
                                                 where ((queryBuilder.FieldId > 0 && queryBuilder.FieldId == f.FieldId) || (queryBuilder.FieldId < 1 && w.FieldId == f.FieldId))
                                                 join b in db.Blocks on f.BlockId equals b.BlockId
                                                 where ((queryBuilder.BlockId > 0 && queryBuilder.BlockId == b.BlockId) || (queryBuilder.BlockId < 1 && f.BlockId == b.BlockId))
                                                 join c in db.Companies on f.CompanyId equals c.CompanyId
                                                 where ((queryBuilder.CompanyId > 0 && queryBuilder.CompanyId == c.CompanyId) || (queryBuilder.CompanyId < 1 && f.CompanyId == c.CompanyId))
                                                 join t in db.Terrains on f.TerrainId equals t.TerrainId
                                                 where ((queryBuilder.TerrainId > 0 && queryBuilder.TerrainId == t.TerrainId) || (queryBuilder.TerrainId < 1 && f.TerrainId == t.TerrainId))
                                                 join z in db.Zones on f.ZoneId equals z.ZoneId
                                                 select new WellReportObject
                                                 {
                                                     Name = w.Name,
                                                     WellId = w.WellId,
                                                     SpudDate = w.SpudDate,
                                                     TechnicalAllowable = w.TechnicalAllowable > 0 ? (double)w.TechnicalAllowable : 0,
                                                     TotalDept = w.TotalDept > 0 ? (double)w.TotalDept : 0,
                                                     Remarks = w.Remarks,
                                                     ZoneName = z.Name,
                                                     BlockName = b.Name,
                                                     WellClassName = wcc.Name,
                                                     TerrainName = t.Name,
                                                     FieldName = f.Name,
                                                     CompanyName = c.Name,
                                                     WellTypeName = wt.Title
                                                 }).ToList();


                                if (!query.Any())
                                {
                                    return new List<WellReportObject>();
                                }
                                query.ForEach(m =>
                                {
                                    if (m.SpudDate != null)
                                    {
                                        m.Date = ((DateTime)m.SpudDate).ToString("dd/MM/yyyy");
                                    }
                                });

                                return query;
                            }

                            if (queryBuilder.StartDate != null && queryBuilder.StartDate.Value.Year > 0001 && queryBuilder.EndDate != null && queryBuilder.EndDate.Value.Year > 0001)
                            {
                                var startYear = queryBuilder.StartDate.Value.Year;
                                var startMonth = queryBuilder.StartDate.Value.Month;
                                var endYear = queryBuilder.EndDate.Value.Year;
                                var endMonth = queryBuilder.EndDate.Value.Month;

                                    var query = (
                                                 from w in db.Wells.Where(x => ((x.SpudDate != null && x.SpudDate.Value.Year >= startYear && x.SpudDate.Value.Month >= startMonth) && (x.SpudDate.Value.Year <= endYear && x.SpudDate.Value.Month <= endMonth))).OrderBy(m => m.Name)
                                                 join wt in db.WellTypes on w.WellTypeId equals wt.WellTypeId
                                                 where ((queryBuilder.WellTypeId > 0 && queryBuilder.WellTypeId == wt.WellTypeId) || (queryBuilder.WellTypeId < 1 && w.WellTypeId == wt.WellTypeId))
                                                 join wc in db.WellClassifications on w.WellId equals wc.WellId
                                                 join wcc in db.WellClasses on wc.WellClassId equals wcc.WellClassId
                                                 join f in db.Fields on w.FieldId equals f.FieldId
                                                 where ((queryBuilder.FieldId > 0 && queryBuilder.FieldId == f.FieldId) || (queryBuilder.FieldId < 1 && w.FieldId == f.FieldId))
                                                 join b in db.Blocks on f.BlockId equals b.BlockId
                                                 where ((queryBuilder.BlockId > 0 && queryBuilder.BlockId == b.BlockId) || (queryBuilder.BlockId < 1 && f.BlockId == b.BlockId))
                                                 join c in db.Companies on f.CompanyId equals c.CompanyId
                                                 where ((queryBuilder.CompanyId > 0 && queryBuilder.CompanyId == c.CompanyId) || (queryBuilder.CompanyId < 1 && f.CompanyId == c.CompanyId))
                                                 join t in db.Terrains on f.TerrainId equals t.TerrainId
                                                 where ((queryBuilder.TerrainId > 0 && queryBuilder.TerrainId == t.TerrainId) || (queryBuilder.TerrainId < 1 && f.TerrainId == t.TerrainId))
                                                 join z in db.Zones on f.ZoneId equals z.ZoneId
                                                 select new WellReportObject
                                                 {
                                                     Name = w.Name,
                                                     WellId = w.WellId,
                                                     SpudDate = w.SpudDate,
                                                     TechnicalAllowable = w.TechnicalAllowable > 0 ? (double)w.TechnicalAllowable : 0,
                                                     TotalDept = w.TotalDept > 0 ? (double)w.TotalDept : 0,
                                                     Remarks = w.Remarks,
                                                     ZoneName = z.Name,
                                                     WellClassName = wcc.Name,
                                                     TerrainName = t.Name,
                                                     BlockName = b.Name,
                                                     FieldName = f.Name,
                                                     CompanyName = c.Name,
                                                     WellTypeName = wt.Title,
                                                 }).ToList();

                                    if (!query.Any())
                                    {
                                        return new List<WellReportObject>();
                                    }
                                    query.ForEach(m =>
                                    {
                                        if (m.SpudDate != null)
                                        {
                                            m.Date = ((DateTime)m.SpudDate).ToString("dd/MM/yyyy");
                                        }
                                    });

                                    return query;
                                
                            }
                            
                        }
                    return new List<WellReportObject>();
                    }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellReportObject>();
            }
        }
        public List<string> GetSpudYears()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = (from x in db.Wells
                                     where x.SpudDate != null
                                     select x.SpudDate.Value.Year).ToList();
                    if (!myObjList.Any())
                    {
                        return new List<string>();
                    }

                    var fd = new List<string>();

                    myObjList.ForEach(m =>
                    {
                        if (!fd.Exists(l => l == m.ToString(CultureInfo.InvariantCulture)))
                        {
                            fd.Add(m.ToString(CultureInfo.InvariantCulture));
                        }
                        
                    });

                    return fd;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<string>();
            }
        }
        public List<Well> GetAllOrderedWellsByWellType()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Wells.Where(m => m.WellTypeId == 3).ToList();
                    
                    if (!myObjList.Any())
                    {
                        return new List<Well>();
                    }

                    //var filteredList = new List<Well>();

                    //myObjList.ForEach(m =>
                    //{
                    //    if (m.WellTypeId == 3)
                    //    {
                    //        filteredList.Add(m);
                    //    }

                    //});
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Well>();
            }
        }
        public int AddWellCheckDuplicate(Well well)
        {
            try
            {
                if (well == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Wells.Any())
                    {
                        if (db.Wells.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == well.Name.ToLower().Replace(" ", string.Empty)) > 0)
                        {
                            return -3;
                        }
                    }
                    
                    var processedItem = db.Wells.Add(well);
                    db.SaveChanges();

                    if (db.WellClassifications.Any())
                    {
                        if (db.WellClassifications.Count(m => m.WellId == processedItem.WellId) > 0)
                        {
                            return -4;
                        }
                    }
                    var newWellClassification = new WellClassification
                    {
                        WellClassId = well.WellClassId,
                        WellId = processedItem.WellId
                    };
                    var classidication = new WellClassificationServices().AddWellClassificationCheckDuplicate(newWellClassification);

                    if (classidication < 1)
                        {
                            return -2;
                        }
                    
                    return processedItem.WellId;

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int AddWellCheckDuplicate2(Well well)
        {
            try
            {
                if (well == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {

                    var edx = db.Wells.Where(m => m.Name.Trim().Replace(" ", string.Empty).Replace("-", "").ToLower() == well.Name.Trim().Replace(" ", string.Empty).Replace("-", "").ToLower()).ToList();
                        
                    if (edx.Any())
                    {
                        return edx[0].WellId;
                    }

                    var processedItem = db.Wells.Add(well);
                    db.SaveChanges();
                    return processedItem.WellId;

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateWellCheckDuplicate(Well well)
        {
            try
            {
                if (well == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Wells.Any())
                    {
                        if (db.Wells.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == well.Name.ToLower().Replace(" ", string.Empty) && m.WellId != well.WellId && m.FieldId == well.FieldId) > 0)
                        {
                            return -3;
                        }
                    }

                    var newWellClassification = new WellClassification
                    {
                        WellClassId = well.WellClassId,
                        WellId = well.WellId,
                        WellClassificationId = well.WellClassificationId
                    };
                    
                    var classidicationUpdate = new WellClassificationServices().UpdateWellClassification2(newWellClassification);

                    if (classidicationUpdate < 1)
                    {
                        return -2;
                    }

                    db.Wells.Attach(well);
                    db.Entry(well).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public bool DeleteWellCheckReferences(int wellId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.EquipmentUsageHistories.Count(m => m.WellId == wellId) > 0)
                    {
                        return false;
                    }

                    if (db.WellWorkovers.Count(m => m.WellId == wellId) > 0)
                    {
                        return false;
                    }

                    if (db.WellClassifications.Count(m => m.WellId == wellId) > 0)
                    {
                        return false;
                    }

                    if (db.WellCompletions.Count(m => m.WellId == wellId) > 0)
                    {
                        return false;
                    }

                    var myObj = db.Wells.Where(s => s.WellId == wellId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.Wells.Remove(myObj[0]);
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
        public Well GetWell(int wellId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = (from w in db.Wells.Where(m => m.WellId == wellId)
                                 join wc in db.WellClassifications on w.WellId equals wc.WellId
                                 join wcc in db.WellClasses on wc.WellClassId equals wcc.WellClassId
                                 join f in db.Fields on w.FieldId equals f.FieldId
                                 join b in db.Blocks on f.BlockId equals b.BlockId
                                 select new { w, wc.WellClassificationId, wcc.WellClassId, f.FieldId}).ToList();
                    if (!myObj.Any())
                    {
                        return new Well();
                    }

                    var tdd = new Well
                    {
                        WellId = myObj[0].w.WellId,
                        WellClassId = myObj[0].WellClassId,
                        WellClassificationId = myObj[0].WellClassificationId,
                        Name = myObj[0].w.Name,
                        WellTypeId = myObj[0].w.WellTypeId,
                        FieldId = myObj[0].FieldId,
                        SpudDate = myObj[0].w.SpudDate,
                        TechnicalAllowable = myObj[0].w.TechnicalAllowable,
                        TotalDept = myObj[0].w.TotalDept,
                        Remarks = myObj[0].w.Remarks,
                        Date =
                            (myObj[0].w.SpudDate != null)
                                ? ((DateTime) myObj[0].w.SpudDate).ToString("yyyy/MM/dd")
                                : DateTime.Now.ToString("yyyy/MM/dd")
                    };

                    return tdd;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Well();
            }
        }
        public Well GetWellForReport(int wellId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Wells.Where(s => s.WellId == wellId).Include("Company").Include("Field").Include("WellType").ToList();
                    if (!myObj.Any())
                    {
                        return new Well();
                    }
                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Well();
            }
        }
        public int GetWellId(Well well)
        {
            try
            {
                if (well == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Wells.Any())
                    {
                        var edx = db.Wells.Where(m => m.Name.ToLower().Replace(" ", string.Empty) == well.Name.ToLower().Replace(" ", string.Empty)).ToList();

                        if (edx.Any())
                        {
                            return edx[0].WellId;
                        }

                        var processedWell = db.Wells.Add(well);
                        db.SaveChanges();
                        return processedWell.WellId;
                     }

                    var processedItem = db.Wells.Add(well);
                    db.SaveChanges();
                    return processedItem.WellId;

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 1;
            }
        }
        public int GetWellIdByName(string wellName)
        {
            try
            {
                if (wellName == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var edx = db.Wells.Where(m => m.Name.ToLower().Replace(" ", string.Empty) == wellName.ToLower().Replace(" ", string.Empty)).ToList();

                    if (edx.Any())
                    {
                        return edx[0].WellId;
                    }

                    return 0;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public List<WellObject> GetWells(int itemsPerPage, int pageNumber)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var dfx = new List<WellObject>();
                    
                    var myObjList = db.Wells.OrderBy(m => m.Name)
                           .Skip((pageNumber - 1) * itemsPerPage)
                           .Take(itemsPerPage).ToList();
                    if (!myObjList.Any())
                    {
                        return new List<WellObject>();
                    }
                    myObjList.ForEach(m => dfx.Add(new WellObject
                    {
                        Name = m.Name,
                        WellId = m.WellId
                    }));
                    return dfx;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellObject>();
            }
        }
        public List<Well> GetNormWells(int itemsPerPage, int pageNumber)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Wells.OrderBy(m => m.Name)
                           .Skip((pageNumber - 1) * itemsPerPage)
                           .Take(itemsPerPage).ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Well>();
                    }
                    return myObjList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Well>();
            }
        }

	}
	
}
