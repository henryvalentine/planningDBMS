using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
    public class WellCompletionServices
	{
        public List<WellCompletion> GetAllOrderedWellCompletions(int itemsPerPage, int pageNumber, out int dataCount)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.WellCompletions.Any())
                    {
                        dataCount = db.WellWorkovers.Count();
                        var myObjList =
                            db.WellCompletions.OrderBy(m => m.WellCompletionId).Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage)
                            .Include("WellCompletionType").Include("Equipment").Include("Well").Include("WellCompletionIntervals")
                                .ToList();
                        if (!myObjList.Any())
                        {
                            dataCount = 0;
                            return new List<WellCompletion>();
                        }
                        myObjList.ForEach(m =>
                        {
                            if (m.DateCompleted != null)
                                m.DatecomPletedString = ((DateTime)m.DateCompleted).ToString("yyyy/MM/dd");
                        });
                        return myObjList;
                    }
                    dataCount = 0;
                    return new List<WellCompletion>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                dataCount = 0;
                return new List<WellCompletion>();
            }
        }
        public List<WellCompletion> GetAllOrderedWellCompletionReportByPeriod(int searchMonth, int searchYear)
        {
            try
            {
                if (searchYear < 1)
                {
                    return new List<WellCompletion>();
                }

                var compList = new List<WellCompletion>();

                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (searchMonth > 0 && searchYear > 0)
                    {
                        //var yearStr = searchYear.ToString(CultureInfo.InvariantCulture);
                        //var monthStr = "";
                        //if (searchMonth < 10)
                        //{
                        //    monthStr = "/0" + searchMonth + "/";
                        //}
                        //else
                        //{
                        //    monthStr = searchMonth.ToString(CultureInfo.InvariantCulture);
                        //}
                          
                        var myObjList = (from wc in
                                             db.WellCompletions.Where(m => m.DateCompleted.Value.Year == searchYear && m.DateCompleted.Value.Month == searchMonth).Include("WellCompletionType")
                                .Include("Well")
                                .Include("Equipment")
                                 .Include("WellCompletionIntervals")

                                         join f in db.Fields on wc.Well.FieldId equals f.FieldId
                                         join b in db.Blocks on f.BlockId equals b.BlockId
                                         join z in db.Zones on f.ZoneId equals z.ZoneId
                                         join t in db.Terrains on f.TerrainId equals t.TerrainId
                                         join c in db.Companies on f.CompanyId equals c.CompanyId
                                         join wct in db.WellCompletionTypes on wc.WellCompletionTypeId equals wct.WellCompletionTypeId
                                         join wt in db.WellTypes on wc.Well.WellTypeId equals wt.WellTypeId
                                         select new { wc, b, f, z, t, c, wct, wt }).ToList();
                        if (!myObjList.Any())
                        {
                            return new List<WellCompletion>();
                        }

                        myObjList.ForEach(m =>
                        {
                            if (m.wc.DateCompleted != null)
                            {
                                var ttd = new WellCompletion
                                {
                                    WellCompletionId = m.wc.WellCompletionId,
                                    WellName = m.wc.Well.Name,
                                    ZoneName = m.z.Name,
                                    TerrainName = m.t.Name,
                                    WellTypeName = m.wt.Title,
                                    CompanyName = m.c.Name,
                                    EquipmentName = m.wc.Equipment.Name,
                                    WellCompletionTypeName = m.wct.Type,
                                    DatecomPletedString = ((DateTime)m.wc.DateCompleted).ToString("yyyy/MM/dd"),
                                    WellCompletionTypeId = m.wc.WellCompletionTypeId
                                };


                                if (m.wc.WellCompletionIntervals.Any())
                                {
                                    var intervals = m.wc.WellCompletionIntervals.ToList();

                                    if (ttd.WellCompletionTypeId == (int)WellCompletionTypeEnum.Dual)
                                    {
                                        var upperInt = intervals.Max(x => x.UpperInterval);
                                        var lowerInt = intervals.Min(x => x.UpperInterval);

                                        if (lowerInt > 0)
                                        {
                                            var interval1 = intervals.Find(r => r.UpperInterval.Equals(lowerInt));
                                            if (interval1 != null && interval1.WellCompletionIntervalId > 0)
                                            {
                                                ttd.CompletionIntervalStr = "LI1 : " + interval1.LowerInterval + ", UI1 : " + interval1.UpperInterval;
                                            }
                                        }

                                        if (upperInt > 0)
                                        {
                                            var interval2 = intervals.Find(y => y.UpperInterval.Equals(upperInt));
                                            if (interval2 != null)
                                            {
                                                ttd.CompletionIntervalStr += "\n" + "LI2 : " + interval2.LowerInterval + ", UI2 : " + interval2.UpperInterval;
                                            }

                                        }

                                    }

                                    if (ttd.WellCompletionTypeId == (int)WellCompletionTypeEnum.Single)
                                    {
                                        ttd.CompletionIntervalStr = "LI : " + intervals[0].LowerInterval + ", UI : " + intervals[0].UpperInterval;
                                    }
                                }

                                compList.Add(ttd);
                            }
                        });

                        return compList;
                    }

                    if (searchMonth < 1 && searchYear > 0)
                    {
                        var yearStr = searchYear.ToString(CultureInfo.InvariantCulture);
                      
                        var myObjList = (from wc in
                                             db.WellCompletions.Where(m => m.DateCompleted.Value.Year == searchYear && m.DateCompleted.Value.Month == searchMonth).Include("WellCompletionType")
                                .Include("Well")
                                .Include("Equipment")
                                 .Include("WellCompletionIntervals")

                                 join f in db.Fields on wc.Well.FieldId equals f.FieldId
                                 join b in db.Blocks on f.BlockId equals b.BlockId
                                 join z in db.Zones on f.ZoneId equals z.ZoneId
                                 join t in db.Terrains on f.TerrainId equals  t.TerrainId
                                 join c in db.Companies on f.CompanyId equals c.CompanyId
                                 join wct in db.WellCompletionTypes on wc.WellCompletionTypeId equals wct.WellCompletionTypeId
                                 join wt in db.WellTypes on wc.Well.WellTypeId equals wt.WellTypeId
                                 select new {wc, b, f, z, t, c, wct, wt}).ToList();
                        if (!myObjList.Any())
                        {
                            return new List<WellCompletion>();
                        }

                        myObjList.ForEach(m =>
                        {
                            var ttd = new WellCompletion
                            {
                                WellCompletionId = m.wc.WellCompletionId,
                                WellName = m.wc.Well.Name,
                                ZoneName = m.z.Name,
                                TerrainName = m.t.Name,
                                WellTypeName = m.wt.Title,
                                CompanyName = m.c.Name,
                                EquipmentName = m.wc.Equipment.Name,
                                WellCompletionTypeName = m.wct.Type,
                                DatecomPletedString = ((DateTime)m.wc.DateCompleted).ToString("yyyy/MM/dd"),
                                WellCompletionTypeId = m.wc.WellCompletionTypeId
                            };

                            
                            if (m.wc.WellCompletionIntervals.Any())
                            {
                                var intervals = m.wc.WellCompletionIntervals.ToList();

                                if (ttd.WellCompletionTypeId == (int)WellCompletionTypeEnum.Dual)
                                {
                                    var upperInt = intervals.Max(x => x.UpperInterval);
                                    var lowerInt = intervals.Min(x => x.UpperInterval);

                                    if (lowerInt > 0)
                                    {
                                        var interval1 = intervals.Find(r => r.UpperInterval.Equals(lowerInt));
                                        if (interval1 != null && interval1.WellCompletionIntervalId > 0)
                                        {
                                            ttd.CompletionIntervalStr = "LI1 : " + interval1.LowerInterval + ", UI1 : " + interval1.UpperInterval;
                                        }
                                    }

                                    if (upperInt > 0)
                                    {
                                        var interval2 = intervals.Find(y => y.UpperInterval.Equals(upperInt));
                                        if (interval2 != null)
                                        {
                                            ttd.CompletionIntervalStr += "\n" + "LI2 : " + interval2.LowerInterval + ", UI2 : " + interval2.UpperInterval;
                                        }

                                    }

                                }

                                if (ttd.WellCompletionTypeId == (int)WellCompletionTypeEnum.Single)
                                {
                                    ttd.CompletionIntervalStr = "LI : " + intervals[0].LowerInterval + ", UI : " + intervals[0].UpperInterval;
                                }
                            }

                            compList.Add(ttd);
                        });

                        return compList;
                    }
                    return new List<WellCompletion>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellCompletion>();
            }
        }
        public List<WellCompletionObject> GetWellObjects(int itemsPerPage, int pageNumber)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.WellCompletions.OrderBy(m => m.WellCompletionId).Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage)
                                .Include("WellCompletionType")
                                .Include("Equipment")
                                .Include("Well")
                                .ToList();

                    if (!myObjList.Any())
                    {
                        return new List<WellCompletionObject>();
                    }

                    var ttsdfg = new List<WellCompletionObject>();

                    myObjList.ForEach(m => ttsdfg.Add(new WellCompletionObject
                    {
                        WellCompletionId = m.WellCompletionId,
                        WellName = m.Well.Name,
                        EquipmentName = m.Equipment.Name,
                        WellCompletionTypeName = m.WellCompletionType.Type,
                        IsInitial = m.IsInitial
                    }));

                    if (!ttsdfg.Any())
                    {
                        return new List<WellCompletionObject>();
                    }

                    return ttsdfg;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellCompletionObject>();
            }
        }
        public List<string> GetWellCompletionYears()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = (from x in db.WellCompletions
                                     where x.DateCompleted.Value.Year > 0
                                     select x.DateCompleted.Value.Year).ToList();
                    if (!myObjList.Any())
                    {
                        return new List<string>();
                    }
                    var newList = new List<string>();
                    myObjList.ForEach(m =>
                    {
                        
                        
                            if (!newList.Exists(x => x == m.ToString(CultureInfo.InvariantCulture)))
                            {
                                newList.Add(m.ToString(CultureInfo.InvariantCulture));
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
        public List<WellCompletionReportObject> GetCompletionReportByPeriod(int searchYear)
        {
            try
            {
                var fxg = new List<WellCompletionReportObject>();
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.WellCompletions.Any())
                    {
                        var myObjList =( from wc in
                                             db.WellCompletions.Where(m => m.DateCompleted.Value.Year == searchYear).Include("WellCompletionType")
                                .Include("Well")
                                .Include("Equipment")
                                 .Include("WellCompletionIntervals")

                                 join f in db.Fields on wc.Well.FieldId equals f.FieldId
                                 join b in db.Blocks on f.BlockId equals b.BlockId
                                 join z in db.Zones on f.ZoneId equals z.ZoneId
                                 join t in db.Terrains on f.TerrainId equals  t.TerrainId
                                 join c in db.Companies on f.CompanyId equals c.CompanyId
                                 join wct in db.WellCompletionTypes on wc.WellCompletionTypeId equals wct.WellCompletionTypeId
                                 join wt in db.WellTypes on wc.Well.WellTypeId equals wt.WellTypeId
                                 select new {wc, b, f, z, t, c, wct, wt}).ToList();
                        if (!myObjList.Any())
                        {
                            return new List<WellCompletionReportObject>();
                        }

                        myObjList.ForEach(m =>
                        {
                            if (m.wc.DateCompleted != null)
                            {
                                var ttd = new WellCompletionReportObject
                                {
                                    WellCompletionId = m.wc.WellCompletionId,
                                    WellName = m.wc.Well.Name,
                                    ZoneName = m.z.Name,
                                    TerrainName = m.t.Name,
                                    WellTypeName = m.wt.Title,
                                    CompanyName = m.c.Name,
                                    EquipmentName = m.wc.Equipment.Name,
                                    WellCompletionTypeName = m.wct.Type,
                                    DatecomPletedString = ((DateTime)m.wc.DateCompleted).ToString("yyyy/MM/dd"),
                                    WellCompletionTypeId = m.wc.WellCompletionTypeId
                                };

                            
                                if (m.wc.WellCompletionIntervals.Any())
                                {
                                    var intervals = m.wc.WellCompletionIntervals.ToList();

                                    if (ttd.WellCompletionTypeId == (int)WellCompletionTypeEnum.Dual)
                                    {
                                        var upperInt = intervals.Max(x => x.UpperInterval);
                                        var lowerInt = intervals.Min(x => x.UpperInterval);

                                        if (lowerInt > 0)
                                        {
                                            var interval1 = intervals.Find(r => r.UpperInterval.Equals(lowerInt));
                                            if (interval1 != null && interval1.WellCompletionIntervalId > 0)
                                            {
                                                ttd.CompletionIntervalStr = "LI1 : " + interval1.LowerInterval + ", UI1 : " + interval1.UpperInterval;
                                            }
                                        }

                                        if (upperInt > 0)
                                        {
                                            var interval2 = intervals.Find(y => y.UpperInterval.Equals(upperInt));
                                            if (interval2 != null)
                                            {
                                                ttd.CompletionIntervalStr += "\n" + "LI2 : " + interval2.LowerInterval + ", UI2 : " + interval2.UpperInterval;
                                            }

                                        }

                                    }

                                    if (ttd.WellCompletionTypeId == (int)WellCompletionTypeEnum.Single)
                                    {
                                        ttd.CompletionIntervalStr = "LI : " + intervals[0].LowerInterval + ", UI : " + intervals[0].UpperInterval;
                                    }
                                }

                                fxg.Add(ttd);
                            }
                        });

                        return fxg;
                    }
                    return new List<WellCompletionReportObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellCompletionReportObject>();
            }
        }
        public List<WellCompletionReportObject> GetWellStaticCompletionReport(int itemsPerPage, int pageNumber, int searchMonth, int searchYear)
        {
            try
            {
                var fxg = new List<WellCompletionReportObject>();
                if (searchYear < 1)
                {
                    return new List<WellCompletionReportObject>();
                }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (searchMonth > 0 && searchYear > 0)
                    {
                        var myObjList =( from wc in db.WellCompletions.Where(m => m.DateCompleted.Value.Year == searchYear && m.DateCompleted.Value.Month == searchMonth).OrderBy(m => m.WellCompletionId).Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage)
                                .Include("Well")
                                .Include("Equipment")
                                .Include("WellCompletionIntervals")
                                         join f in db.Fields on wc.Well.FieldId equals f.FieldId
                                         join b in db.Blocks on f.BlockId equals b.BlockId
                                         join z in db.Zones on f.ZoneId equals z.ZoneId
                                         join t in db.Terrains on f.TerrainId equals t.TerrainId
                                         join c in db.Companies on f.CompanyId equals c.CompanyId
                                         join wct in db.WellCompletionTypes on wc.WellCompletionTypeId equals wct.WellCompletionTypeId
                                         join wt in db.WellTypes on wc.Well.WellTypeId equals wt.WellTypeId
                                         select new { wc, b, f, z, t, c, wct, wt }).ToList();
                        if (!myObjList.Any())
                        {
                            return new List<WellCompletionReportObject>();
                        }

                        myObjList.ForEach(m =>
                        {
                            if (m.wc.DateCompleted != null)
                            {
                                var ttd = new WellCompletionReportObject
                                {
                                    WellCompletionId = m.wc.WellCompletionId,
                                    WellName = m.wc.Well.Name,
                                    ZoneName = m.z.Name,
                                    TerrainName = m.t.Name,
                                    WellTypeName = m.wt.Title,
                                    CompanyName = m.c.Name,
                                    EquipmentName = m.wc.Equipment.Name,
                                    WellCompletionTypeName = m.wct.Type,
                                    DatecomPletedString = ((DateTime)m.wc.DateCompleted).ToString("yyyy/MM/dd"),
                                    WellCompletionTypeId = m.wc.WellCompletionTypeId
                                };


                                if (m.wc.WellCompletionIntervals.Any())
                                {
                                    var intervals = m.wc.WellCompletionIntervals.ToList();

                                    if (ttd.WellCompletionTypeId == (int)WellCompletionTypeEnum.Dual)
                                    {
                                        var upperInt = intervals.Max(x => x.UpperInterval);
                                        var lowerInt = intervals.Min(x => x.UpperInterval);

                                        if (lowerInt > 0)
                                        {
                                            var interval1 = intervals.Find(r => r.UpperInterval.Equals(lowerInt));
                                            if (interval1 != null && interval1.WellCompletionIntervalId > 0)
                                            {
                                                ttd.CompletionIntervalStr = "LI1 : " + interval1.LowerInterval + ", UI1 : " + interval1.UpperInterval;
                                            }
                                        }

                                        if (upperInt > 0)
                                        {
                                            var interval2 = intervals.Find(y => y.UpperInterval.Equals(upperInt));
                                            if (interval2 != null)
                                            {
                                                ttd.CompletionIntervalStr += "\n" + "LI2 : " + interval2.LowerInterval + ", UI2 : " + interval2.UpperInterval;
                                            }

                                        }

                                    }

                                    if (ttd.WellCompletionTypeId == (int)WellCompletionTypeEnum.Single)
                                    {
                                        ttd.CompletionIntervalStr = "LI : " + intervals[0].LowerInterval + ", UI : " + intervals[0].UpperInterval;
                                    }
                                }

                                fxg.Add(ttd);
                            }
                        });
                       

                        return fxg;
                    }

                    if (searchMonth < 1 && searchYear > 0)
                    {
                        var myObjList = ( from wc in
                                              db.WellCompletions.Where(m => m.DateCompleted.Value.Year == searchYear).OrderBy(m => m.WellCompletionId).Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage)
                            .Include("WellCompletionType")
                                .Include("Well")
                                .Include("Equipment")
                                .Include("WellCompletionIntervals")

                                          join f in db.Fields on wc.Well.FieldId equals f.FieldId
                                          join b in db.Blocks on f.BlockId equals b.BlockId
                                          join z in db.Zones on f.ZoneId equals z.ZoneId
                                          join t in db.Terrains on f.TerrainId equals t.TerrainId
                                          join c in db.Companies on f.CompanyId equals c.CompanyId
                                          join wct in db.WellCompletionTypes on wc.WellCompletionTypeId equals wct.WellCompletionTypeId
                                          join wt in db.WellTypes on wc.Well.WellTypeId equals wt.WellTypeId
                                          select new { wc, b, f, z, t, c, wct, wt }).ToList();
                        if (!myObjList.Any())
                        {
                            return new List<WellCompletionReportObject>();
                        }

                        myObjList.ForEach(m =>
                        {
                            if (m.wc.DateCompleted != null)
                            {
                                var ttd = new WellCompletionReportObject
                                {
                                    WellCompletionId = m.wc.WellCompletionId,
                                    WellName = m.wc.Well.Name,
                                    ZoneName = m.z.Name,
                                    TerrainName = m.t.Name,
                                    WellTypeName = m.wt.Title,
                                    CompanyName = m.c.Name,
                                    EquipmentName = m.wc.Equipment.Name,
                                    WellCompletionTypeName = m.wct.Type,
                                    DatecomPletedString = ((DateTime)m.wc.DateCompleted).ToString("yyyy/MM/dd"),
                                    WellCompletionTypeId = m.wc.WellCompletionTypeId
                                };


                                if (m.wc.WellCompletionIntervals.Any())
                                {
                                    var intervals = m.wc.WellCompletionIntervals.ToList();

                                    if (ttd.WellCompletionTypeId == (int)WellCompletionTypeEnum.Dual)
                                    {
                                        var upperInt = intervals.Max(x => x.UpperInterval);
                                        var lowerInt = intervals.Min(x => x.UpperInterval);

                                        if (lowerInt > 0)
                                        {
                                            var interval1 = intervals.Find(r => r.UpperInterval.Equals(lowerInt));
                                            if (interval1 != null && interval1.WellCompletionIntervalId > 0)
                                            {
                                                ttd.CompletionIntervalStr = "LI1 : " + interval1.LowerInterval + ", UI1 : " + interval1.UpperInterval;
                                            }
                                        }

                                        if (upperInt > 0)
                                        {
                                            var interval2 = intervals.Find(y => y.UpperInterval.Equals(upperInt));
                                            if (interval2 != null)
                                            {
                                                ttd.CompletionIntervalStr += "\n" + "LI2 : " + interval2.LowerInterval + ", UI2 : " + interval2.UpperInterval;
                                            }

                                        }

                                    }

                                    if (ttd.WellCompletionTypeId == (int)WellCompletionTypeEnum.Single)
                                    {
                                        ttd.CompletionIntervalStr = "LI : " + intervals[0].LowerInterval + ", UI : " + intervals[0].UpperInterval;
                                    }
                                }

                                fxg.Add(ttd);
                            }
                        });

                        return fxg;
                    }
                    return new List<WellCompletionReportObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellCompletionReportObject>();
            }
        }

        public List<WellCompletionReportObject> GetWellStaticCompletionReport23(int itemsPerPage, int pageNumber, int searchMonth, int searchYear)
        {
            try
            {
                var fxg = new List<WellCompletionReportObject>();
                if (searchYear < 1)
                {
                    return new List<WellCompletionReportObject>();
                }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (searchMonth > 0 && searchYear > 0)
                    {
                       var myObjList = (from wc in
                                             db.WellCompletions.Where(m => m.DateCompleted.Value.Year == searchYear && m.DateCompleted.Value.Month == searchMonth).OrderBy(m => m.WellCompletionId).Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage)
                               .Include("Well")
                               .Include("Equipment")
                               .Include("WellCompletionIntervals")

                                        join f in db.Fields on wc.Well.FieldId equals f.FieldId
                                        join b in db.Blocks on f.BlockId equals b.BlockId
                                         join z in db.Zones on f.ZoneId equals z.ZoneId
                                         join t in db.Terrains on f.TerrainId equals t.TerrainId
                                         join c in db.Companies on f.CompanyId equals c.CompanyId
                                         join wct in db.WellCompletionTypes on wc.WellCompletionTypeId equals wct.WellCompletionTypeId
                                         join wt in db.WellTypes on wc.Well.WellTypeId equals wt.WellTypeId
                                         select new { wc, b, f, z, t, c, wct, wt }).ToList();
                        if (!myObjList.Any())
                        {
                            return new List<WellCompletionReportObject>();
                        }

                        myObjList.ForEach(m =>
                        {
                            if (m.wc.DateCompleted != null)
                            {
                                var ttd = new WellCompletionReportObject
                                {
                                    WellCompletionId = m.wc.WellCompletionId,
                                    WellName = m.wc.Well.Name,
                                    ZoneName = m.z.Name,
                                    TerrainName = m.t.Name,
                                    WellTypeName = m.wt.Title,
                                    CompanyName = m.c.Name,
                                    EquipmentName = m.wc.Equipment.Name,
                                    WellCompletionTypeName = m.wct.Type,
                                    DatecomPletedString = ((DateTime)m.wc.DateCompleted).ToString("yyyy/MM/dd"),
                                    WellCompletionTypeId = m.wc.WellCompletionTypeId
                                };


                                if (m.wc.WellCompletionIntervals.Any())
                                {
                                    var intervals = m.wc.WellCompletionIntervals.ToList();

                                    if (ttd.WellCompletionTypeId == (int)WellCompletionTypeEnum.Dual)
                                    {
                                        var upperInt = intervals.Max(x => x.UpperInterval);
                                        var lowerInt = intervals.Min(x => x.UpperInterval);

                                        if (lowerInt > 0)
                                        {
                                            var interval1 = intervals.Find(r => r.UpperInterval.Equals(lowerInt));
                                            if (interval1 != null && interval1.WellCompletionIntervalId > 0)
                                            {
                                                ttd.CompletionIntervalStr = "LI1 : " + interval1.LowerInterval + ", UI1 : " + interval1.UpperInterval;
                                            }
                                        }

                                        if (upperInt > 0)
                                        {
                                            var interval2 = intervals.Find(y => y.UpperInterval.Equals(upperInt));
                                            if (interval2 != null)
                                            {
                                                ttd.CompletionIntervalStr += "\n" + "LI2 : " + interval2.LowerInterval + ", UI2 : " + interval2.UpperInterval;
                                            }

                                        }

                                    }

                                    if (ttd.WellCompletionTypeId == (int)WellCompletionTypeEnum.Single)
                                    {
                                        ttd.CompletionIntervalStr = "LI : " + intervals[0].LowerInterval + ", UI : " + intervals[0].UpperInterval;
                                    }
                                }

                                fxg.Add(ttd);
                            }
                        });

                        return fxg;
                    }

                    if (searchMonth < 1 && searchYear > 0)
                    {
                        var myObjList = (from wc in
                                             db.WellCompletions.Where(m => m.DateCompleted.Value.Year == searchYear).OrderBy(m => m.WellCompletionId).Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage)
                           .Include("WellCompletionType")
                               .Include("Well")
                               .Include("Equipment")
                               .Include("WellCompletionIntervals")

                                         join f in db.Fields on wc.Well.FieldId equals f.FieldId
                                         join b in db.Blocks on f.BlockId equals b.BlockId
                                         join z in db.Zones on f.ZoneId equals z.ZoneId
                                         join t in db.Terrains on f.TerrainId equals t.TerrainId
                                         join c in db.Companies on f.CompanyId equals c.CompanyId
                                         join wct in db.WellCompletionTypes on wc.WellCompletionTypeId equals wct.WellCompletionTypeId
                                         join wt in db.WellTypes on wc.Well.WellTypeId equals wt.WellTypeId
                                         select new { wc, b, f, z, t, c, wct, wt }).ToList();
                        if (!myObjList.Any())
                        {
                            return new List<WellCompletionReportObject>();
                        }

                        myObjList.ForEach(m =>
                        {
                            var ttd = new WellCompletionReportObject
                            {
                                WellCompletionId = m.wc.WellCompletionId,
                                WellName = m.wc.Well.Name,
                                ZoneName = m.z.Name,
                                TerrainName = m.t.Name,
                                WellTypeName = m.wt.Title,
                                CompanyName = m.c.Name,
                                EquipmentName = m.wc.Equipment.Name,
                                WellCompletionTypeName = m.wct.Type,
                                DatecomPletedString = ((DateTime)m.wc.DateCompleted).ToString("yyyy/MM/dd"),
                                WellCompletionTypeId = m.wc.WellCompletionTypeId
                            };


                            if (m.wc.WellCompletionIntervals.Any())
                            {
                                var intervals = m.wc.WellCompletionIntervals.ToList();

                                if (ttd.WellCompletionTypeId == (int)WellCompletionTypeEnum.Dual)
                                {
                                    var upperInt = intervals.Max(x => x.UpperInterval);
                                    var lowerInt = intervals.Min(x => x.UpperInterval);

                                    if (lowerInt > 0)
                                    {
                                        var interval1 = intervals.Find(r => r.UpperInterval.Equals(lowerInt));
                                        if (interval1 != null && interval1.WellCompletionIntervalId > 0)
                                        {
                                            ttd.CompletionIntervalStr = "LI1 : " + interval1.LowerInterval + ", UI1 : " + interval1.UpperInterval;
                                        }
                                    }

                                    if (upperInt > 0)
                                    {
                                        var interval2 = intervals.Find(y => y.UpperInterval.Equals(upperInt));
                                        if (interval2 != null)
                                        {
                                            ttd.CompletionIntervalStr += "\n" + "LI2 : " + interval2.LowerInterval + ", UI2 : " + interval2.UpperInterval;
                                        }

                                    }

                                }

                                if (ttd.WellCompletionTypeId == (int)WellCompletionTypeEnum.Single)
                                {
                                    ttd.CompletionIntervalStr = "LI : " + intervals[0].LowerInterval + ", UI : " + intervals[0].UpperInterval;
                                }
                            }

                            fxg.Add(ttd);
                        });

                        return fxg;
                    }
                    return new List<WellCompletionReportObject>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellCompletionReportObject>();
            }
        }

        public List<WellCompletion> GetAllOrderedWellCompletions()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.WellCompletions.Any())
                    {
                        var myObjList =
                            db.WellCompletions.Include("WellCompletionType")
                                .Include("Equipment")
                                .Include("Well")
                                 .Include("WellCompletionIntervals")
                                .ToList();
                        if (!myObjList.Any())
                        {
                            return new List<WellCompletion>();
                        }
                        return myObjList;
                    }
                    return new List<WellCompletion>();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellCompletion>();
            }
        }
        public List<WellCompletion> GetAllOrderedCompletionsByMonth(int searchMonth, int searchYear)
        {
            try
            {
                if (searchMonth < 1 || searchYear < 1)
                {
                    return new List<WellCompletion>();
                }
                
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                   var myObjList = db.WellCompletions.Where(m => m.DateCompleted.Value.Year == searchYear && m.DateCompleted.Value.Month == searchMonth).Include("Equipment").Include("Well").Include("WellWorkOverReason").Include("WellCompletionIntervals").ToList();
                   
                    if (!myObjList.Any())
                    {
                        return new List<WellCompletion>();
                    }
                    return myObjList.OrderBy(m => m.DateCompleted).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellCompletion>();
            }
        }
        public int AddWellCompletionCheckDuplicate(WellCompletion wellCompletion)
        {
            try
            {
                if (wellCompletion == null)
                {
                    return -2;
                }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var txx = db.WellCompletions.Add(wellCompletion);
                    db.SaveChanges();
                    var newCompletionInterval = new WellCompletionInterval
                    {
                        LowerInterval = wellCompletion.L1,
                        UpperInterval = wellCompletion.U1 ,
                        DateCompleted = wellCompletion.DateCompleted.ToString(),
                        LastUpdatedTime = DateTime.Now.ToString("g"),
                        WellCompletionId = txx.WellCompletionId
                    };
                   
                 var k = new WellCompletionIntervalServices().AddWellCompletionIntervalCheckDuplicate(newCompletionInterval);
                if (k < 1)
                {
                    DeleteWellCompletionCheckReferences(txx.WellCompletionId);
                    return -2;
                }

                if (wellCompletion.WellCompletionTypeId > 1)
                {
                    var newCompletionInterval2 = new WellCompletionInterval
                    {
                        LowerInterval = wellCompletion.L2,
                        UpperInterval = wellCompletion.U2 ,
                        DateCompleted = wellCompletion.DateCompleted.ToString(),
                        LastUpdatedTime = DateTime.Now.ToString("g"),
                        WellCompletionId = txx.WellCompletionId
                    };

                   var x = new WellCompletionIntervalServices().AddWellCompletionIntervalCheckDuplicate(newCompletionInterval2);
                    if (x < 1)
                    {
                        DeleteWellCompletionCheckReferences(txx.WellCompletionId);
                        return -2;
                    }      
                  }
                    return txx.WellCompletionId;
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int AddWellCompletionCheckDuplicate2(WellCompletion wellCompletion)
        {
            try
            {
                if (wellCompletion == null)
                {
                    return -2;
                }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var txx = db.WellCompletions.Add(wellCompletion);
                    db.SaveChanges();
                    var newCompletionInterval = new WellCompletionInterval
                    {
                        LowerInterval = 0,
                        UpperInterval = 1,
                        DateCompleted = wellCompletion.DateCompleted.ToString(),
                        LastUpdatedTime = DateTime.Now.ToString("g"),
                        WellCompletionId = txx.WellCompletionId
                    };

                    var k = db.WellCompletionIntervals.Add(newCompletionInterval);
                    db.SaveChanges();
                    if (k.WellCompletionIntervalId < 1)
                    {
                        DeleteWellCompletionCheckReferences(txx.WellCompletionId);
                        return -2;
                    }
                    return txx.WellCompletionId;
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int AddWellCompletionCheckDuplicate3(WellCompletion wellCompletion)
        {
            try
            {
                if (wellCompletion == null)
                {
                    return -2;
                }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.WellCompletions.Count(m => m.WellId == wellCompletion.WellId && m.EquipmentId == wellCompletion.EquipmentId && m.IsInitial == wellCompletion.IsInitial && m.WellCompletionTypeId == wellCompletion.WellCompletionTypeId && m.DateCompleted == wellCompletion.DateCompleted) > 0)
                    {
                        return -3;
                    }
                    var txx = db.WellCompletions.Add(wellCompletion);
                    db.SaveChanges();
                    return txx.WellCompletionId;
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateWellCompletionCheckDuplicate(WellCompletion wellCompletion)
        {
            try
            {
                if (wellCompletion == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    db.WellCompletions.Attach(wellCompletion);
                    db.Entry(wellCompletion).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public bool DeleteWellCompletionCheckReferences(int wellCompletionId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.WellCompletions.Where(s => s.WellCompletionId == wellCompletionId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.WellCompletions.Remove(myObj[0]);
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
        public WellCompletion GetWellCompletion(int wellCompletionId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.WellCompletions.Where(s => s.WellCompletionId == wellCompletionId).Include("WellCompletionIntervals").ToList();
                    if (!myObj.Any())
                    {
                        return new WellCompletion();
                    }

                    if (myObj[0].DateCompleted != null)
                    {
                        myObj[0].DatecomPletedString = ((DateTime)myObj[0].DateCompleted).ToString("yyyy/MM/dd");
                    }
                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new WellCompletion();
            }
        }
        public List<WellCompletionReportObject> GetWellCompletionReports(WellCompletionQuery queryBuilder)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {

                    if (queryBuilder != null)
                    {
                        if (((queryBuilder.StartDate != null && queryBuilder.StartDate.Value.Year == 0001) || (queryBuilder.StartDate == null)) && ((queryBuilder.EndDate != null && queryBuilder.EndDate.Value.Year == 0001) || queryBuilder.EndDate == null))
                        {
                            var query = (from

                                         q in db.WellCompletions.Where(m =>
                                         (queryBuilder.WellId > 0 && m.WellId == queryBuilder.WellId && queryBuilder.CompletionTypeId < 1 && queryBuilder.EquipmentId < 1) ||
                                         (queryBuilder.EquipmentId > 0 && queryBuilder.WellId < 1 && queryBuilder.CompletionTypeId < 1 && m.EquipmentId == queryBuilder.EquipmentId) ||
                                         (queryBuilder.CompletionTypeId > 0 && queryBuilder.WellId < 1 && queryBuilder.EquipmentId < 1 && m.WellCompletionTypeId == queryBuilder.CompletionTypeId) ||
                                         (queryBuilder.WellId > 0 && queryBuilder.EquipmentId > 0 && queryBuilder.CompletionTypeId < 1 && m.WellId == queryBuilder.WellId && m.EquipmentId == queryBuilder.EquipmentId) ||
                                         (queryBuilder.WellId > 0 && queryBuilder.EquipmentId < 1 && queryBuilder.CompletionTypeId > 0 && m.WellId == queryBuilder.WellId && m.WellCompletionTypeId == queryBuilder.CompletionTypeId) ||
                                         (queryBuilder.WellId < 1 && queryBuilder.EquipmentId > 0 && queryBuilder.CompletionTypeId > 0 && m.EquipmentId == queryBuilder.EquipmentId && m.WellCompletionTypeId == queryBuilder.CompletionTypeId) ||
                                         (queryBuilder.WellId < 1 && queryBuilder.EquipmentId < 1 && queryBuilder.CompletionTypeId < 1)).Include("WellCompletionType").Include("Equipment").Include("Well").Include("WellCompletionIntervals")

                                         join
                                         c in db.WellClassifications on q.WellId equals c.WellId
                                         join
                                          p in db.WellClasses on c.WellClassId equals p.WellClassId
                                         where ((p.WellClassId == queryBuilder.WellClassId && queryBuilder.WellClassId > 0) || (p.WellClassId == c.WellClassId && queryBuilder.WellClassId < 1))

                                         join
                                         x in db.WellTypes on q.Well.WellTypeId equals x.WellTypeId
                                         where
                                         queryBuilder.WellTypeId > 0 && x.WellTypeId == queryBuilder.WellTypeId ||
                                         (q.Well.WellTypeId == x.WellTypeId && queryBuilder.WellTypeId < 1)

                                         join
                                          t in db.Fields on q.Well.FieldId equals t.FieldId

                                         join i in db.Blocks on t.BlockId equals i.BlockId
                                         
                                         join
                                         s in db.Zones on t.ZoneId equals s.ZoneId
                                         where
                                         ((queryBuilder.ZoneId > 0 && s.ZoneId == queryBuilder.ZoneId)
                                         ||
                                         (s.ZoneId == t.ZoneId && queryBuilder.ZoneId < 1))

                                         join 
                                         y in db.Terrains on t.TerrainId equals y.TerrainId where((queryBuilder.TerrainId > 0 && y.TerrainId == queryBuilder.TerrainId) || (y.TerrainId == t.TerrainId && queryBuilder.TerrainId < 1))

                                         join
                                           h in db.Companies on t.CompanyId equals h.CompanyId
                                         where ((queryBuilder.CompanyId > 0 && h.CompanyId == queryBuilder.CompanyId) || (queryBuilder.CompanyId < 1 && t.CompanyId == h.CompanyId))


                                         select new WellCompletionReportObject
                                         {
                                             Intervals = q.WellCompletionIntervals.ToList(),
                                             WellName = q.Well.Name,
                                             WellClassName = p.Name,
                                             EquipmentName = q.Equipment.Name,
                                             WellCompletionTypeName = q.WellCompletionType.Type,
                                             DatecomPleted = (DateTime)q.DateCompleted,
                                             CompanyName = h.Name,
                                             WellTypeName = x.Title,
                                             TerrainName = y.Name,
                                             WellCompletionTypeId = q.WellCompletionTypeId,
                                             ZoneName = s.Name
                                         }).ToList();

                            if (!query.Any())
                            {
                                return new List<WellCompletionReportObject>();
                            }
                            query.ForEach(m =>
                            {
                                m.DatecomPletedString = m.DatecomPleted.ToString("yyyy/MM/dd");
                                var intervalStr = "";
                                var intervals = m.Intervals;
                                if (intervals.Any())
                                {
                                    if (m.WellCompletionTypeId > 1)
                                    {
                                        var upperInt = intervals.Max(x => x.UpperInterval);
                                        var lowerInt = intervals.Min(x => x.UpperInterval);

                                        if (lowerInt != null)
                                        {
                                            var interval1 = intervals.Find(p => p.UpperInterval.Equals(lowerInt));
                                            if (interval1 != null && interval1.WellCompletionIntervalId > 0)
                                            {
                                                intervalStr = "LI1: " + interval1.LowerInterval + "; UI1: " +
                                                              interval1.UpperInterval;

                                            }
                                        }

                                        if (upperInt != null)
                                        {
                                            var interval2 = intervals.Find(t => t.UpperInterval.Equals(upperInt));
                                            if (interval2 != null)
                                            {
                                                intervalStr += "; LI2: " + interval2.LowerInterval + "; UI2: " +
                                                               interval2.UpperInterval;
                                            }

                                        }

                                    }

                                    if (m.WellCompletionTypeId < 2)
                                    {
                                        intervalStr = "LI: " + intervals[0].LowerInterval + "; UI: " +
                                                      intervals[0].UpperInterval;

                                    }
                                }
                                m.CompletionIntervalStr = intervalStr;
                            });
                            return query.OrderBy(v => v.WellName).ToList();
                        }

                        if (queryBuilder.StartDate != null && queryBuilder.StartDate.Value.Year > 0001 && queryBuilder.EndDate != null && queryBuilder.EndDate.Value.Year > 0001)
                        {
                            var startYear = queryBuilder.StartDate.Value.Year;
                            var startMonth = queryBuilder.StartDate.Value.Month;
                            var endYear = queryBuilder.EndDate.Value.Year;
                            var endMonth = queryBuilder.EndDate.Value.Month;

                            var query = (from

                                          q in db.WellCompletions.Where(m =>
                                         m.DateCompleted != null && ((queryBuilder.WellId > 0 && m.WellId == queryBuilder.WellId && queryBuilder.CompletionTypeId < 1 && queryBuilder.EquipmentId < 1 && ((m.DateCompleted.Value.Year >= startYear && m.DateCompleted.Value.Month >= startMonth) && (m.DateCompleted.Value.Year <= endYear && m.DateCompleted.Value.Month <= endMonth))) ||
                                                                     (queryBuilder.EquipmentId > 0 && queryBuilder.WellId < 1 && queryBuilder.CompletionTypeId < 1 && m.EquipmentId == queryBuilder.EquipmentId && ((m.DateCompleted.Value.Year >= startYear && m.DateCompleted.Value.Month >= startMonth) && (m.DateCompleted.Value.Year <= endYear && m.DateCompleted.Value.Month <= endMonth))) ||
                                                                     (queryBuilder.CompletionTypeId > 0 && queryBuilder.WellId < 1 && queryBuilder.EquipmentId < 1 && m.WellCompletionTypeId == queryBuilder.CompletionTypeId && ((m.DateCompleted.Value.Year >= startYear && m.DateCompleted.Value.Month >= startMonth) && (m.DateCompleted.Value.Year <= endYear && m.DateCompleted.Value.Month <= endMonth))) ||
                                                                     (queryBuilder.WellId > 0 && queryBuilder.EquipmentId > 0 && queryBuilder.CompletionTypeId < 1 && m.WellId == queryBuilder.WellId && m.EquipmentId == queryBuilder.EquipmentId && ((m.DateCompleted.Value.Year >= startYear && m.DateCompleted.Value.Month >= startMonth) && (m.DateCompleted.Value.Year <= endYear && m.DateCompleted.Value.Month <= endMonth))) ||
                                                                     (queryBuilder.WellId < 1 && queryBuilder.EquipmentId > 0 && queryBuilder.CompletionTypeId > 0 && m.EquipmentId == queryBuilder.EquipmentId && m.WellCompletionTypeId == queryBuilder.CompletionTypeId && ((m.DateCompleted.Value.Year >= startYear && m.DateCompleted.Value.Month >= startMonth) && (m.DateCompleted.Value.Year <= endYear && m.DateCompleted.Value.Month <= endMonth))) ||
                                                                     (queryBuilder.WellId < 1 && queryBuilder.EquipmentId < 1 && queryBuilder.CompletionTypeId < 1 && ((m.DateCompleted.Value.Year >= startYear && m.DateCompleted.Value.Month >= startMonth) && (m.DateCompleted.Value.Year <= endYear && m.DateCompleted.Value.Month <= endMonth))))).Include("WellCompletionType").Include("Equipment").Include("Well").Include("WellCompletionIntervals")

                                         join
                                         c in db.WellClassifications on q.WellId equals c.WellId
                                         join
                                          p in db.WellClasses on c.WellClassId equals p.WellClassId
                                         where ((p.WellClassId == queryBuilder.WellClassId && queryBuilder.WellClassId > 0) || (p.WellClassId == c.WellClassId && queryBuilder.WellClassId < 1))

                                         join
                                         x in db.WellTypes on q.Well.WellTypeId equals x.WellTypeId
                                         where
                                         queryBuilder.WellTypeId > 0 && x.WellTypeId == queryBuilder.WellTypeId ||
                                         (q.Well.WellTypeId == x.WellTypeId && queryBuilder.WellTypeId < 1)

                                         join
                                          t in db.Fields on q.Well.FieldId equals t.FieldId

                                         join i in db.Blocks on t.BlockId equals i.BlockId

                                         join
                                         s in db.Zones on t.ZoneId equals s.ZoneId
                                         where
                                         ((queryBuilder.ZoneId > 0 && s.ZoneId == queryBuilder.ZoneId)
                                         ||
                                         (s.ZoneId == t.ZoneId && queryBuilder.ZoneId < 1))

                                         join
                                         y in db.Terrains on t.TerrainId equals y.TerrainId
                                         where ((queryBuilder.TerrainId > 0 && y.TerrainId == queryBuilder.TerrainId) || (y.TerrainId == t.TerrainId && queryBuilder.TerrainId < 1))

                                         join
                                           h in db.Companies on t.CompanyId equals h.CompanyId
                                         where ((queryBuilder.CompanyId > 0 && h.CompanyId == queryBuilder.CompanyId) || (queryBuilder.CompanyId < 1 && t.CompanyId == h.CompanyId))


                                         select new WellCompletionReportObject
                                         {
                                             Intervals = q.WellCompletionIntervals.ToList(),
                                             WellName = q.Well.Name,
                                             WellClassName = p.Name,
                                             EquipmentName = q.Equipment.Name,
                                             WellCompletionTypeName = q.WellCompletionType.Type,
                                             DatecomPleted = (DateTime)q.DateCompleted,
                                             CompanyName = h.Name,
                                             WellTypeName = x.Title,
                                             WellCompletionTypeId = q.WellCompletionTypeId,
                                             TerrainName = y.Name,
                                             ZoneName = s.Name
                                         }).ToList();

                            if (!query.Any())
                            {
                                return new List<WellCompletionReportObject>();
                            }
                            query.ForEach(m =>
                            {
                                m.DatecomPletedString = m.DatecomPleted.ToString("yyyy/MM/dd");
                                var intervalStr = "";
                                var intervals = m.Intervals;
                                if (intervals.Any())
                                {
                                    if (m.WellCompletionTypeId > 1)
                                    {
                                        var upperInt = intervals.Max(x => x.UpperInterval);
                                        var lowerInt = intervals.Min(x => x.UpperInterval);

                                        if (lowerInt != null)
                                        {
                                            var interval1 = intervals.Find(p => p.UpperInterval.Equals(lowerInt));
                                            if (interval1 != null && interval1.WellCompletionIntervalId > 0)
                                            {
                                                intervalStr = "LI1: " + interval1.LowerInterval + "; UI1: " +
                                                              interval1.UpperInterval;

                                            }
                                        }

                                        if (upperInt != null)
                                        {
                                            var interval2 = intervals.Find(t => t.UpperInterval.Equals(upperInt));
                                            if (interval2 != null)
                                            {
                                                intervalStr += "; LI2: " + interval2.LowerInterval + "; UI2: " +
                                                               interval2.UpperInterval;
                                            }

                                        }

                                    }

                                    if (m.WellCompletionTypeId < 2)
                                    {
                                        intervalStr = "LI: " + intervals[0].LowerInterval + "; UI: " +
                                                      intervals[0].UpperInterval;

                                    }
                                }
                                m.CompletionIntervalStr = intervalStr;
                            });
                            return query.OrderBy(v => v.WellName).ToList();
                        }

                        //if (queryBuilder.Year > 0 && queryBuilder.Year2 < 1 && queryBuilder.Month < 1)
                        //{
                        //   // var yearStr = queryBuilder.Year.ToString();

                        //    var query = (from

                        //                 q in db.WellCompletions.Where(m =>
                        //                 m.DateCompleted != null && ((queryBuilder.WellId > 0 && m.WellId == queryBuilder.WellId && queryBuilder.CompletionTypeId < 1 && queryBuilder.EquipmentId < 1 && m.DateCompleted.Value.Year == queryBuilder.Year) ||
                        //                                             (queryBuilder.EquipmentId > 0 && queryBuilder.WellId < 1 && queryBuilder.CompletionTypeId < 1 && m.EquipmentId == queryBuilder.EquipmentId && m.DateCompleted.Value.Year == queryBuilder.Year) ||
                        //                                             (queryBuilder.CompletionTypeId > 0 && queryBuilder.WellId < 1 && queryBuilder.EquipmentId < 1 && m.WellCompletionTypeId == queryBuilder.CompletionTypeId && m.DateCompleted.Value.Year == queryBuilder.Year) ||
                        //                                             (queryBuilder.WellId > 0 && queryBuilder.EquipmentId > 0 && queryBuilder.CompletionTypeId < 1 && m.WellId == queryBuilder.WellId && m.EquipmentId == queryBuilder.EquipmentId && m.DateCompleted.Value.Year == queryBuilder.Year) ||
                        //                                             (queryBuilder.WellId > 0 && queryBuilder.EquipmentId < 1 && queryBuilder.CompletionTypeId > 0 && m.WellId == queryBuilder.WellId && m.WellCompletionTypeId == queryBuilder.CompletionTypeId && m.DateCompleted.Value.Year == queryBuilder.Year) ||
                        //                                             (queryBuilder.WellId < 1 && queryBuilder.EquipmentId > 0 && queryBuilder.CompletionTypeId > 0 && m.EquipmentId == queryBuilder.EquipmentId && m.WellCompletionTypeId == queryBuilder.CompletionTypeId && m.DateCompleted.Value.Year == queryBuilder.Year) ||
                        //                                             (queryBuilder.WellId < 1 && queryBuilder.EquipmentId < 1 && queryBuilder.CompletionTypeId < 1 && m.DateCompleted.Value.Year == queryBuilder.Year))).Include("WellCompletionType").Include("Equipment").Include("Well").Include("WellCompletionIntervals")

                        //                 join
                        //                 c in db.WellClassifications on q.WellId equals c.WellId
                        //                 join
                        //                  p in db.WellClasses on c.WellClassId equals p.WellClassId
                        //                 where ((p.WellClassId == queryBuilder.WellClassId && queryBuilder.WellClassId > 0) || (p.WellClassId == c.WellClassId && queryBuilder.WellClassId < 1))

                        //                 join
                        //                 x in db.WellTypes on q.Well.WellTypeId equals x.WellTypeId
                        //                 where
                        //                 queryBuilder.WellTypeId > 0 && x.WellTypeId == queryBuilder.WellTypeId ||
                        //                 (q.Well.WellTypeId == x.WellTypeId && queryBuilder.WellTypeId < 1)

                        //                 join
                        //                  t in db.Fields on q.Well.FieldId equals t.FieldId

                        //                 join i in db.Blocks on t.BlockId equals i.BlockId

                        //                 join
                        //                 s in db.Zones on t.ZoneId equals s.ZoneId
                        //                 where
                        //                 ((queryBuilder.ZoneId > 0 && s.ZoneId == queryBuilder.ZoneId)
                        //                 ||
                        //                 (s.ZoneId == t.ZoneId && queryBuilder.ZoneId < 1))

                        //                 join
                        //                 y in db.Terrains on t.TerrainId equals y.TerrainId
                        //                 where ((queryBuilder.TerrainId > 0 && y.TerrainId == queryBuilder.TerrainId) || (y.TerrainId == t.TerrainId && queryBuilder.TerrainId < 1))

                        //                 join
                        //                   h in db.Companies on t.CompanyId equals h.CompanyId
                        //                 where ((queryBuilder.CompanyId > 0 && h.CompanyId == queryBuilder.CompanyId) || (queryBuilder.CompanyId < 1 && t.CompanyId == h.CompanyId))


                        //                 select new WellCompletionReportObject
                        //                 {
                        //                     Intervals = q.WellCompletionIntervals.ToList(),
                        //                     WellName = q.Well.Name,
                        //                     WellClassName = p.Name,
                        //                     EquipmentName = q.Equipment.Name,
                        //                     WellCompletionTypeName = q.WellCompletionType.Type,
                        //                     DatecomPletedString = ((DateTime)q.DateCompleted).ToString("yyyy/MM/dd"),
                        //                     CompanyName = h.Name,
                        //                     WellCompletionTypeId = q.WellCompletionTypeId,
                        //                     WellTypeName = x.Title,
                        //                     TerrainName = y.Name,
                        //                     ZoneName = s.Name
                        //                 }).ToList();

                        //    if (!query.Any())
                        //    {
                        //        return new List<WellCompletionReportObject>();
                        //    }
                        //    query.ForEach(m =>
                        //    {
                        //        var intervalStr = "";
                        //        var intervals = m.Intervals;
                        //        if (intervals.Any())
                        //        {
                        //            if (m.WellCompletionTypeId > 1)
                        //            {
                        //                var upperInt = intervals.Max(x => x.UpperInterval);
                        //                var lowerInt = intervals.Min(x => x.UpperInterval);

                        //                if (lowerInt != null)
                        //                {
                        //                    var interval1 = intervals.Find(p => p.UpperInterval.Equals(lowerInt));
                        //                    if (interval1 != null && interval1.WellCompletionIntervalId > 0)
                        //                    {
                        //                        intervalStr = "LI1: " + interval1.LowerInterval + "; UI1: " +
                        //                                      interval1.UpperInterval;

                        //                    }
                        //                }

                        //                if (upperInt != null)
                        //                {
                        //                    var interval2 = intervals.Find(t => t.UpperInterval.Equals(upperInt));
                        //                    if (interval2 != null)
                        //                    {
                        //                        intervalStr += "; LI2: " + interval2.LowerInterval + "; UI2: " +
                        //                                       interval2.UpperInterval;
                        //                    }

                        //                }

                        //            }

                        //            if (m.WellCompletionTypeId < 2)
                        //            {
                        //                intervalStr = "LI: " + intervals[0].LowerInterval + "; UI: " +
                        //                              intervals[0].UpperInterval;

                        //            }
                        //        }
                        //        m.CompletionIntervalStr = intervalStr;
                        //    });
                        //     return query.OrderBy(v => v.WellName).ToList();
                        // }

                        if (queryBuilder.StartDate != null && queryBuilder.StartDate.Value.Year > 0001 && ((queryBuilder.EndDate != null && queryBuilder.EndDate.Value.Year == 0001) || (queryBuilder.EndDate == null)))
                        {
                            var startYear = queryBuilder.StartDate.Value.Year;
                            var startMonth = queryBuilder.StartDate.Value.Month;
                           
                           
                            var query = (from

                                         q in db.WellCompletions.Where(m =>
                                         m.DateCompleted != null && ((queryBuilder.WellId > 0 && m.WellId == queryBuilder.WellId && queryBuilder.CompletionTypeId < 1 && queryBuilder.EquipmentId < 1 && (m.DateCompleted.Value.Year == startYear && m.DateCompleted.Value.Month == startMonth)) ||
                                                                     (queryBuilder.EquipmentId > 0 && queryBuilder.WellId < 1 && queryBuilder.CompletionTypeId < 1 && m.EquipmentId == queryBuilder.EquipmentId && (m.DateCompleted.Value.Year == startYear && m.DateCompleted.Value.Month == startMonth)) ||
                                                                     (queryBuilder.CompletionTypeId > 0 && queryBuilder.WellId < 1 && queryBuilder.EquipmentId < 1 && m.WellCompletionTypeId == queryBuilder.CompletionTypeId && (m.DateCompleted.Value.Year == startYear && m.DateCompleted.Value.Month == startMonth)) ||
                                                                     (queryBuilder.WellId > 0 && queryBuilder.EquipmentId > 0 && queryBuilder.CompletionTypeId < 1 && m.WellId == queryBuilder.WellId && m.EquipmentId == queryBuilder.EquipmentId && (m.DateCompleted.Value.Year == startYear && m.DateCompleted.Value.Month == startMonth)) ||
                                                                     (queryBuilder.WellId > 0 && queryBuilder.EquipmentId < 1 && queryBuilder.CompletionTypeId > 0 && m.WellId == queryBuilder.WellId && m.WellCompletionTypeId == queryBuilder.CompletionTypeId && (m.DateCompleted.Value.Year == startYear && m.DateCompleted.Value.Month == startMonth)) ||
                                                                     (queryBuilder.WellId < 1 && queryBuilder.EquipmentId > 0 && queryBuilder.CompletionTypeId > 0 && m.EquipmentId == queryBuilder.EquipmentId && m.WellCompletionTypeId == queryBuilder.CompletionTypeId && (m.DateCompleted.Value.Year == startYear && m.DateCompleted.Value.Month == startMonth)) ||
                                                                     (queryBuilder.WellId < 1 && queryBuilder.EquipmentId < 1 && queryBuilder.CompletionTypeId < 1 && (m.DateCompleted.Value.Year == startYear && m.DateCompleted.Value.Month == startMonth)))).Include("WellCompletionType").Include("Equipment").Include("Well").Include("WellCompletionIntervals")

                                         join
                                         c in db.WellClassifications on q.WellId equals c.WellId
                                         join
                                          p in db.WellClasses on c.WellClassId equals p.WellClassId
                                         where ((p.WellClassId == queryBuilder.WellClassId && queryBuilder.WellClassId > 0) || (p.WellClassId == c.WellClassId && queryBuilder.WellClassId < 1))

                                         join
                                         x in db.WellTypes on q.Well.WellTypeId equals x.WellTypeId
                                         where
                                         queryBuilder.WellTypeId > 0 && x.WellTypeId == queryBuilder.WellTypeId ||
                                         (q.Well.WellTypeId == x.WellTypeId && queryBuilder.WellTypeId < 1)

                                         join
                                          t in db.Fields on q.Well.FieldId equals t.FieldId

                                         join i in db.Blocks on t.BlockId equals i.BlockId

                                         join
                                         s in db.Zones on t.ZoneId equals s.ZoneId
                                         where
                                         ((queryBuilder.ZoneId > 0 && s.ZoneId == queryBuilder.ZoneId)
                                         ||
                                         (s.ZoneId == t.ZoneId && queryBuilder.ZoneId < 1))

                                         join
                                         y in db.Terrains on t.TerrainId equals y.TerrainId
                                         where ((queryBuilder.TerrainId > 0 && y.TerrainId == queryBuilder.TerrainId) || (y.TerrainId == t.TerrainId && queryBuilder.TerrainId < 1))

                                         join
                                           h in db.Companies on t.CompanyId equals h.CompanyId
                                         where ((queryBuilder.CompanyId > 0 && h.CompanyId == queryBuilder.CompanyId) || (queryBuilder.CompanyId < 1 && t.CompanyId == h.CompanyId))


                                         select new WellCompletionReportObject
                                         {
                                             Intervals = q.WellCompletionIntervals.ToList(),
                                             WellName = q.Well.Name,
                                             WellClassName = p.Name,
                                             EquipmentName = q.Equipment.Name,
                                             WellCompletionTypeName = q.WellCompletionType.Type,
                                             DatecomPleted = (DateTime) q.DateCompleted,
                                             CompanyName = h.Name,
                                             WellCompletionTypeId = q.WellCompletionTypeId,
                                             WellTypeName = x.Title,
                                             TerrainName = y.Name,
                                             ZoneName = s.Name
                                         }).ToList();

                            if (!query.Any())
                            {
                                return new List<WellCompletionReportObject>();
                            }
                            query.ForEach(m =>
                            {
                                m.DatecomPletedString = m.DatecomPleted.ToString("yyyy/MM/dd");
                                var intervalStr = "";
                                var intervals = m.Intervals;
                                if (intervals.Any())
                                {
                                    if (m.WellCompletionTypeId > 1)
                                    {
                                        var upperInt = intervals.Max(x => x.UpperInterval);
                                        var lowerInt = intervals.Min(x => x.UpperInterval);

                                        if (lowerInt != null)
                                        {
                                            var interval1 = intervals.Find(p => p.UpperInterval.Equals(lowerInt));
                                            if (interval1 != null && interval1.WellCompletionIntervalId > 0)
                                            {
                                                intervalStr = "LI1: " + interval1.LowerInterval + "; UI1: " +
                                                              interval1.UpperInterval;

                                            }
                                        }

                                        if (upperInt != null)
                                        {
                                            var interval2 = intervals.Find(t => t.UpperInterval.Equals(upperInt));
                                            if (interval2 != null)
                                            {
                                                intervalStr += "; LI2: " + interval2.LowerInterval + "; UI2: " +
                                                               interval2.UpperInterval;
                                            }

                                        }

                                    }

                                    if (m.WellCompletionTypeId < 2)
                                    {
                                        intervalStr = "LI: " + intervals[0].LowerInterval + "; UI: " +
                                                      intervals[0].UpperInterval;

                                    }
                                }
                                m.CompletionIntervalStr = intervalStr;
                            });
                            return query.OrderBy(v => v.WellName).ToList();
                        }


                        //if (queryBuilder.Year > 0 && queryBuilder.Year2 < 1 && queryBuilder.Month > 0 && queryBuilder.Month2 < 1)
                        //{

                        //    var query = (from

                        //                 q in db.WellCompletions.Where(m =>
                        //                 m.DateCompleted != null && ((queryBuilder.WellId > 0 && m.WellId == queryBuilder.WellId && queryBuilder.CompletionTypeId < 1 && queryBuilder.EquipmentId < 1 && m.DateCompleted.Value.Year == queryBuilder.Year && m.DateCompleted.Value.Month == queryBuilder.Month) ||
                        //                                             (queryBuilder.EquipmentId > 0 && queryBuilder.WellId < 1 && queryBuilder.CompletionTypeId < 1 && m.EquipmentId == queryBuilder.EquipmentId && m.DateCompleted.Value.Year == queryBuilder.Year && m.DateCompleted.Value.Month == queryBuilder.Month) ||
                        //                                             (queryBuilder.CompletionTypeId > 0 && queryBuilder.WellId < 1 && queryBuilder.EquipmentId < 1 && m.WellCompletionTypeId == queryBuilder.CompletionTypeId && m.DateCompleted.Value.Year == queryBuilder.Year && m.DateCompleted.Value.Month == queryBuilder.Month) ||
                        //                                             (queryBuilder.WellId > 0 && queryBuilder.EquipmentId > 0 && queryBuilder.CompletionTypeId < 1 && m.WellId == queryBuilder.WellId && m.EquipmentId == queryBuilder.EquipmentId && m.DateCompleted.Value.Year == queryBuilder.Year && m.DateCompleted.Value.Month == queryBuilder.Month) ||
                        //                                             (queryBuilder.WellId > 0 && queryBuilder.EquipmentId < 1 && queryBuilder.CompletionTypeId > 0 && m.WellId == queryBuilder.WellId && m.WellCompletionTypeId == queryBuilder.CompletionTypeId && m.DateCompleted.Value.Year == queryBuilder.Year && m.DateCompleted.Value.Month == queryBuilder.Month) ||
                        //                                             (queryBuilder.WellId < 1 && queryBuilder.EquipmentId > 0 && queryBuilder.CompletionTypeId > 0 && m.EquipmentId == queryBuilder.EquipmentId && m.WellCompletionTypeId == queryBuilder.CompletionTypeId && m.DateCompleted.Value.Year == queryBuilder.Year && m.DateCompleted.Value.Month == queryBuilder.Month) ||
                        //                                             (queryBuilder.WellId < 1 && queryBuilder.EquipmentId < 1 && queryBuilder.CompletionTypeId < 1 && m.DateCompleted.Value.Year == queryBuilder.Year && m.DateCompleted.Value.Month == queryBuilder.Month))).Include("WellCompletionType").Include("Equipment").Include("Well").Include("WellCompletionIntervals")

                        //                 join
                        //                 c in db.WellClassifications on q.WellId equals c.WellId
                        //                 join
                        //                  p in db.WellClasses on c.WellClassId equals p.WellClassId
                        //                 where ((p.WellClassId == queryBuilder.WellClassId && queryBuilder.WellClassId > 0) || (p.WellClassId == c.WellClassId && queryBuilder.WellClassId < 1))

                        //                 join
                        //                 x in db.WellTypes on q.Well.WellTypeId equals x.WellTypeId
                        //                 where
                        //                 queryBuilder.WellTypeId > 0 && x.WellTypeId == queryBuilder.WellTypeId ||
                        //                 (q.Well.WellTypeId == x.WellTypeId && queryBuilder.WellTypeId < 1)

                        //                 join
                        //                  t in db.Fields on q.Well.FieldId equals t.FieldId

                        //                 join i in db.Blocks on t.BlockId equals i.BlockId

                        //                 join
                        //                 s in db.Zones on t.ZoneId equals s.ZoneId
                        //                 where
                        //                 ((queryBuilder.ZoneId > 0 && s.ZoneId == queryBuilder.ZoneId)
                        //                 ||
                        //                 (s.ZoneId == t.ZoneId && queryBuilder.ZoneId < 1))

                        //                 join
                        //                 y in db.Terrains on t.TerrainId equals y.TerrainId
                        //                 where ((queryBuilder.TerrainId > 0 && y.TerrainId == queryBuilder.TerrainId) || (y.TerrainId == t.TerrainId && queryBuilder.TerrainId < 1))

                        //                 join
                        //                   h in db.Companies on t.CompanyId equals h.CompanyId
                        //                 where ((queryBuilder.CompanyId > 0 && h.CompanyId == queryBuilder.CompanyId) || (queryBuilder.CompanyId < 1 && t.CompanyId == h.CompanyId))


                        //                 select new WellCompletionReportObject
                        //                 {
                        //                     Intervals = q.WellCompletionIntervals.ToList(),
                        //                     WellName = q.Well.Name,
                        //                     WellClassName = p.Name,
                        //                     EquipmentName = q.Equipment.Name,
                        //                     WellCompletionTypeName = q.WellCompletionType.Type,
                        //                     DatecomPletedString = ((DateTime)q.DateCompleted).ToString("yyyy/MM/dd"),
                        //                     CompanyName = h.Name,
                        //                     WellCompletionTypeId = q.WellCompletionTypeId,
                        //                     WellTypeName = x.Title,
                        //                     TerrainName = y.Name,
                        //                     ZoneName = s.Name
                        //                 }).ToList();

                        //    if (!query.Any())
                        //    {
                        //        return new List<WellCompletionReportObject>();
                        //    }
                        //    query.ForEach(m =>
                        //    {
                        //        var intervalStr = "";
                        //        var intervals = m.Intervals;
                        //        if (intervals.Any())
                        //        {
                        //            if (m.WellCompletionTypeId > 1)
                        //            {
                        //                var upperInt = intervals.Max(x => x.UpperInterval);
                        //                var lowerInt = intervals.Min(x => x.UpperInterval);

                        //                if (lowerInt != null)
                        //                {
                        //                    var interval1 = intervals.Find(p => p.UpperInterval.Equals(lowerInt));
                        //                    if (interval1 != null && interval1.WellCompletionIntervalId > 0)
                        //                    {
                        //                        intervalStr = "LI1: " + interval1.LowerInterval + "; UI1: " +
                        //                                      interval1.UpperInterval;

                        //                    }
                        //                }

                        //                if (upperInt != null)
                        //                {
                        //                    var interval2 = intervals.Find(t => t.UpperInterval.Equals(upperInt));
                        //                    if (interval2 != null)
                        //                    {
                        //                        intervalStr += "; LI2: " + interval2.LowerInterval + "; UI2: " +
                        //                                       interval2.UpperInterval;
                        //                    }

                        //                }

                        //            }

                        //            if (m.WellCompletionTypeId < 2)
                        //            {
                        //                intervalStr = "LI: " + intervals[0].LowerInterval + "; UI: " +
                        //                              intervals[0].UpperInterval;

                        //            }
                        //        }
                        //        m.CompletionIntervalStr = intervalStr;
                        //    });
                        //    return query.OrderBy(v => v.WellName).ToList();
                        //}

                        }

                    }
                    return new List<WellCompletionReportObject>();
                }
            
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<WellCompletionReportObject>();
            }
        }
      
	}
}
