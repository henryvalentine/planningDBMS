using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using System.Data.Entity.Validation;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class ProductionServices
	{
        public List<Production> GetAllOrderedProductions(int itemsPerPage, int pageNumber, out int dataCount)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    dataCount = db.Productions.Count();
                    var myObjList = db.Productions.OrderBy(m => m.Product.Name).Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).Include("Field").Include("Product").ToList();

                    //.OrderBy(sortColumn).Skip((page - 1) *  pageSize).Take(pageSize + 1).ToList()

                    if (!myObjList.Any())
                    {
                        return new List<Production>();
                    }
                        
                    return myObjList;
                }
                
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                dataCount = 0;
                return new List<Production>();
            }
        }
        public List<ProductionObject> GetMoreProductionObjects(int itemsPerPage, int pageNumber)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Productions.OrderBy(m => m.Product.Name).Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).Include("Field").Include("Product").ToList();

                        
                    if (!myObjList.Any())
                    {
                        return new List<ProductionObject>();
                    }

                    var fddd = new List<ProductionObject>();
                    myObjList.ForEach(m => fddd.Add(new ProductionObject
                    {
                        ProductionId = m.ProductionId,
                        ProductName = m.Product.Name,
                        FieldName = m.Field.Name,
                        Quantity = Convert.ToDecimal(m.Quantity).ToString("#,##0"),
                        TechnicalAllowable = m.TechnicalAllowable,
                        MonthName = Enum.GetName(typeof(Months), m.Month),
                        Remark = string.IsNullOrEmpty(m.Remark) ? " " : m.Remark,
                        Year = m.Year,
                    }));
                    return fddd;
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<ProductionObject>();
            }
        }
	    public List<Production> GetAllOrderedProductionsByMonth(int searchMonth , int searchYear )
        {
            try
            {
                if (searchYear < 1)
                {
                    return new List<Production>();
                }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {

                    if (searchMonth > 0)
                    {
                        //var year = searchYear.ToString(CultureInfo.InvariantCulture);
                        //var month = searchMonth.ToString(CultureInfo.InvariantCulture);

                        var myObjList = db.Productions.Where(m => m.Year == searchYear && m.Month == searchMonth).Include("Field").Include("Product").ToList();

                        if (!myObjList.Any())
                        {
                            return new List<Production>();
                        }

                        return myObjList.OrderBy(m => m.Product.Name).ToList();
                  
                    }

                    else
                    {
                        var year = searchYear.ToString(CultureInfo.InvariantCulture);
                      
                        var myObjList = db.Productions.Where(m => m.Year == searchYear).Include("Field").Include("Product").ToList();

                        if (!myObjList.Any())
                        {
                            return new List<Production>();
                        }

                        return myObjList.OrderBy(m => m.Product.Name).ToList();
                  
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
               return new List<Production>();
            }
        }
        public List<Production> GetAllOrderedProductions()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                        var myObjList = db.Productions.Include("Field").Include("Product").ToList();

                        //.OrderBy(sortColumn).Skip((page - 1) *  pageSize).Take(pageSize + 1).ToList()

                        if (!myObjList.Any())
                        {
                            return new List<Production>();
                        }

                        return myObjList.OrderBy(m => m.Product.Name).ToList();
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Production>();
            }
        }
        public int AddProduction(Production production)
        {
            try
            {
                if (production == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var processedItem = db.Productions.Add(production);
                    db.SaveChanges();
                    return processedItem.ProductionId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateProductionCheckDuplicate(Production production)
        {
            try
            {
                if (production == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    db.Productions.Attach(production);
                    db.Entry(production).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public bool DeleteProductionCheckReferences(int productionId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Productions.Where(s => s.ProductionId == productionId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.Productions.Remove(myObj[0]);
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
        public Production GetProduction(int productionId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Productions.Where(s => s.ProductionId == productionId).ToList();
                    if (!myObj.Any())
                    {
                        return new Production();
                    }
                    myObj[0].ProductionDateString = myObj[0].Year + "/" + myObj[0].Month + "/01";
                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Production();
            }
        }
        public List<Production> GetAllOrderedProductionsByFieldId(int field)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Productions.Where(s => s.FieldId == field).ToList();
                    return !myObjList.Any() ? new List<Production>() : myObjList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Production>();
            }
        }
        public List<Production> GetAllOrderedProductionReportByPeriod(int pageNumber, int itemsPerPage)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                        var myObjList = (from p in
                            db.Productions
                                .OrderBy(m => m.Product.Name)
                                .Skip((pageNumber - 1)*itemsPerPage)
                                .Take(itemsPerPage)
                                join f in db.Fields on p.FieldId equals f.FieldId
                                join c in db.Companies on f.CompanyId equals c.CompanyId
                                join z in db.Zones on f.ZoneId equals z.ZoneId
                                join t in db.Terrains on f.TerrainId equals t.TerrainId
                                join pd in db.Products on p.ProductId equals pd.ProductId
                                join b in db.Blocks on f.BlockId equals b.BlockId
                                select new Production
                                {
                                    BlockName = b.Name,
                                    ZoneName = z.Name,
                                    TerrainName = t.Name,
                                    CompanyName = c.Name,
                                    ProductName = pd.Name,
                                    FieldName = f.Name
                                })
                                .ToList();
                        if (!myObjList.Any())
                        {
                            return new List<Production>();
                        }
                        myObjList.ForEach(m =>
                        {
                            m.Periodstr = Enum.GetName(typeof (Months), m.Month) + "/" + m.Year;
                            m.Quantity = Convert.ToDecimal(m.Quantity).ToString("#,##0");
                        });

                        return myObjList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Production>();
            }
        }
        public List<ProductionObject> GetProductionStaticReportByPeriod(int pageNumber, int itemsPerPage, int searchMonth, int searchYear)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var prodObjList = new List<ProductionObject>();

                    if (searchMonth < 1)
                    {
                        var myObjList = (from p in
                            db.Productions.Where(m => m.Year == searchYear)
                                .OrderBy(x => x.Product.Name)
                                .Skip((pageNumber - 1)*itemsPerPage)
                                .Take(itemsPerPage)
                                join f in db.Fields on p.FieldId equals f.FieldId
                                join c in db.Companies on f.CompanyId equals c.CompanyId
                                join z in db.Zones on f.ZoneId equals z.ZoneId
                                join t in db.Terrains on f.TerrainId equals t.TerrainId
                                join pd in db.Products on p.ProductId equals pd.ProductId
                                join b in db.Blocks on f.BlockId equals b.BlockId
                                select new ProductionObject
                                {
                                    BlockName = b.Name,
                                    ZoneName = z.Name,
                                    TerrainName = t.Name,
                                    CompanyName = c.Name,
                                    ProductName = pd.Name,
                                    FieldName = f.Name
                                    
                                })
                                .ToList();
                        if (!myObjList.Any())
                        {
                            return new List<ProductionObject>();
                        }
                        myObjList.ForEach(m =>
                        {
                            m.Periodstr = Enum.GetName(typeof(Months), m.Month) + "/" + m.Year;
                            m.Quantity = Convert.ToDecimal(m.Quantity).ToString("#,##0");
                        });

                        return prodObjList;
                    }

                    else
                    {
                        var myObjList = (from p in
                            db.Productions.Where(m => m.Year == searchYear && m.Month == searchMonth)
                                .OrderBy(m => m.Product.Name)
                                .Skip((pageNumber - 1) * itemsPerPage)
                                .Take(itemsPerPage)
                                join f in db.Fields on p.FieldId equals f.FieldId
                                join c in db.Companies on f.CompanyId equals c.CompanyId
                                join z in db.Zones on f.ZoneId equals z.ZoneId
                                join t in db.Terrains on f.TerrainId equals t.TerrainId
                                join pd in db.Products on p.ProductId equals pd.ProductId
                                join b in db.Blocks on f.BlockId equals b.BlockId
                                select new ProductionObject
                                {
                                    BlockName = b.Name,
                                    ZoneName = z.Name,
                                    TerrainName = t.Name,
                                    CompanyName = c.Name,
                                    ProductName = pd.Name,
                                    FieldName = f.Name
                                    
                                })
                                .ToList();
                        if (!myObjList.Any())
                        {
                            return new List<ProductionObject>();
                        }

                        myObjList.ForEach(m =>
                        {
                            m.Periodstr = Enum.GetName(typeof(Months), m.Month) + "/" + m.Year;
                            m.Quantity = Convert.ToDecimal(m.Quantity).ToString("#,##0");
                        });

                        return myObjList;
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<ProductionObject>();
            }
        }
        public List<long> GetProductionYears()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = (from x in db.Productions
                                     where x.Year > 0
                                     select x.Year).ToList();
                    if (!myObjList.Any())
                    {
                        return new List<long>();
                    }
                    
                    return myObjList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<long>();
            }
        }
        public List<ProductionObject> GetOrderedProductionReportObjects(ProductionQuery queryBuilder)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {

                    if (queryBuilder != null)
                    {
                        if (queryBuilder.StartDate != null &&  queryBuilder.StartDate.Value.Year > 0001 && queryBuilder.EndDate != null &&  queryBuilder.EndDate.Value.Year > 0001)
                        {
                            var startYear = queryBuilder.StartDate.Value.Year;
                            var startMonth = queryBuilder.StartDate.Value.Month;
                            var endYear = queryBuilder.EndDate.Value.Year;
                            var endMonth = queryBuilder.EndDate.Value.Month;
                            
                            var query = (

                                  from
                                    p in db.Productions.Where(m =>
                                         (queryBuilder.FieldId > 0 && m.FieldId == queryBuilder.FieldId && queryBuilder.ProductId < 1 && ((m.Year >= startYear && m.Month >= startMonth) && (m.Year <= endYear && m.Month <= endMonth))) ||
                                         (queryBuilder.ProductId > 0 && queryBuilder.FieldId < 1 && m.ProductId == queryBuilder.ProductId && ((m.Year >= startYear && m.Month >= startMonth) && (m.Year <= endYear && m.Month <= endMonth))) ||
                                         (queryBuilder.FieldId > 0 && queryBuilder.ProductId > 0 && queryBuilder.ProductId > 0 && queryBuilder.FieldId > 0 && m.FieldId == queryBuilder.FieldId && m.ProductId == queryBuilder.ProductId && ((m.Year >= startYear && m.Month >= startMonth) && (m.Year <= endYear && m.Month <= endMonth))) ||
                                         (queryBuilder.FieldId < 1 && queryBuilder.ProductId < 1 && ((m.Year >= startYear && m.Month >= startMonth) && (m.Year <= endYear && m.Month <= endMonth))))
                                         
                                  join f in db.Fields on p.FieldId equals f.FieldId
                                  where ((queryBuilder.FieldId > 0 && f.FieldId == queryBuilder.FieldId) || (f.FieldId == p.FieldId && queryBuilder.FieldId < 1))
                                  join c in db.Companies on f.CompanyId equals c.CompanyId
                                  where
                                      ((queryBuilder.CompanyId > 0 && c.CompanyId == queryBuilder.CompanyId) || (queryBuilder.CompanyId < 1 && c.CompanyId == f.CompanyId))
                                  join z in db.Zones on f.ZoneId equals z.ZoneId
                                  //where ((queryBuilder.ZoneId > 0 && z.ZoneId == queryBuilder.ZoneId && z.ZoneId == f.ZoneId) || (z.ZoneId == f.ZoneId && queryBuilder.ZoneId < 1))
                                  join t in db.Terrains on f.TerrainId equals t.TerrainId
                                  //where ((queryBuilder.TerrainId > 0 &&
                                  //    t.TerrainId == queryBuilder.TerrainId) || (queryBuilder.TerrainId < 1 && t.TerrainId == f.TerrainId))
                                  join pd in db.Products on p.ProductId equals pd.ProductId
                                  where
                                    ((queryBuilder.ProductId > 0 && pd.ProductId == queryBuilder.ProductId) ||
                                     (queryBuilder.ProductId < 1 && p.ProductId == pd.ProductId))
                                  join b in db.Blocks on f.BlockId equals b.BlockId
                                  where
                                     ((queryBuilder.BlockId > 0 && b.BlockId == queryBuilder.BlockId) ||
                                     (b.BlockId == f.BlockId && queryBuilder.BlockId < 1))

                                  select new ProductionObject()
                                  {
                                      ProductName = pd.Name,
                                      BlockName = b.Name,
                                      FieldName = f.Name,
                                      CompanyName = c.Name,
                                      ZoneName = z.Name,
                                      TerrainName = t.Name,
                                      Quantity = p.Quantity,
                                      Year = p.Year,
                                      Month = p.Month,
                                      Remark = p.Remark,
                                      APIGravity = p.APIGravity
                                  }).ToList();

                            if (!query.Any())
                            {
                                return new List<ProductionObject>();
                            }
                            query.ForEach(j =>
                            {
                                var quantityOut = 0;
                                var result2 = int.TryParse(j.Quantity, out quantityOut);

                                if (result2)
                                {
                                    j.Quantity = Convert.ToDecimal(j.Quantity).ToString("#,##0");
                                }
                                else
                                {
                                    j.Quantity = "0";
                                }
                                var monthOut = j.Month;

                                if (monthOut > 0)
                                {
                                    j.Period = Enum.GetName(typeof(Months), monthOut) + "/" + j.Year;

                                }

                                if (monthOut < 1)
                                {
                                    j.Period = j.Year.ToString(CultureInfo.InvariantCulture);

                                }


                            });
                            return query.OrderBy(v => v.ProductName).ToList();
                        }

                        if (queryBuilder.StartDate != null && queryBuilder.StartDate.Value.Year > 0001 && ((queryBuilder.EndDate != null && queryBuilder.EndDate.Value.Year == 0001) || (queryBuilder.EndDate == null)))
                        {
                            var startYear = queryBuilder.StartDate.Value.Year;
                            var startMonth = queryBuilder.StartDate.Value.Month;
                           
                            var query = (
                                
                                from
                                    p in db.Productions.Where(m =>
                                         (queryBuilder.FieldId > 0 && m.FieldId == queryBuilder.FieldId && queryBuilder.ProductId < 1 && (m.Year == startYear && m.Month == startMonth)) ||
                                         (queryBuilder.ProductId > 0 && queryBuilder.FieldId < 1 && m.ProductId == queryBuilder.ProductId && (m.Year == startYear && m.Month == startMonth)) ||
                                         (queryBuilder.FieldId > 0 && queryBuilder.ProductId > 0 && queryBuilder.ProductId > 0 && queryBuilder.FieldId > 0 && m.FieldId == queryBuilder.FieldId && m.ProductId == queryBuilder.ProductId && (m.Year == startYear && m.Month == startMonth)) ||
                                         (queryBuilder.FieldId < 1 && queryBuilder.ProductId < 1 && (m.Year == startYear && m.Month == startMonth)))


                                join f in db.Fields on p.FieldId equals f.FieldId
                                where ((queryBuilder.FieldId > 0 && f.FieldId == queryBuilder.FieldId) || (f.FieldId == p.FieldId && queryBuilder.FieldId < 1))
                                join c in db.Companies on f.CompanyId equals c.CompanyId
                                where
                                    ((queryBuilder.CompanyId > 0 && c.CompanyId == queryBuilder.CompanyId) || (queryBuilder.CompanyId < 1 && c.CompanyId == f.CompanyId))
                                join z in db.Zones on f.ZoneId equals z.ZoneId
                                //where ((queryBuilder.ZoneId > 0 && z.ZoneId == queryBuilder.ZoneId && z.ZoneId == f.ZoneId) || (z.ZoneId == f.ZoneId && queryBuilder.ZoneId < 1))
                                join t in db.Terrains on f.TerrainId equals t.TerrainId
                                //where ((queryBuilder.TerrainId > 0 &&
                                //    t.TerrainId == queryBuilder.TerrainId) || (queryBuilder.TerrainId < 1 && t.TerrainId == f.TerrainId))
                                join pd in db.Products on p.ProductId equals pd.ProductId
                                where
                                  ((queryBuilder.ProductId > 0 && pd.ProductId == queryBuilder.ProductId) ||
                                   (queryBuilder.ProductId < 1 && p.ProductId == pd.ProductId))
                                join b in db.Blocks on f.BlockId equals b.BlockId
                                where
                                   ((queryBuilder.BlockId > 0 && f.BlockId == queryBuilder.BlockId) ||
                                   (b.BlockId == f.BlockId && queryBuilder.BlockId < 1))

                                select new
                                {
                                    pp = p,
                                    pdd = pd,
                                    bb = b,
                                    ff = f,
                                    cc = c,
                                    zz = z,
                                    tt = t,
                                }).ToList();

                            if (!query.Any())
                            {
                                return new List<ProductionObject>();
                            }

                            var xlt = new List<ProductionObject>();

                            query.ForEach(m =>
                            {
                                var p = m.pp;
                                var pd = m.pdd;
                                var b = m.bb;
                                var f = m.ff;
                                var c = m.cc;
                                var z = m.zz;
                                var t = m.tt;

                                var j =
                                new ProductionObject()
                                {
                                    ProductName = pd.Name,
                                    BlockName = b.Name,
                                    FieldName = f.Name,
                                    CompanyName = c.Name,
                                    ZoneName = z.Name,
                                    TerrainName = t.Name,
                                    Quantity = p.Quantity,
                                    Year = p.Year,
                                    Month = p.Month,
                                    Remark = p.Remark,
                                    APIGravity = p.APIGravity
                                };
                                var quantityOut = 0;
                                var result2 = int.TryParse(j.Quantity, out quantityOut);

                                if (result2)
                                {
                                    j.Quantity = Convert.ToDecimal(j.Quantity).ToString("#,##0");
                                }
                                else
                                {
                                    j.Quantity = "0";
                                }
                                var monthOut = j.Month;

                                if (monthOut > 0)
                                {
                                    j.Period = Enum.GetName(typeof(Months), monthOut) + "/" + j.Year;

                                }

                                if (monthOut < 1)
                                {
                                    j.Period = j.Year.ToString(CultureInfo.InvariantCulture);

                                }
                                xlt.Add(j);
                            });
                            return xlt.OrderBy(v => v.ProductName).ToList();
                            //return new List<ProductionObject>();
                        }

                        //if (queryBuilder.Year < 1 && queryBuilder.Month > 0)
                        //{
                        //    var query = (

                        //        from
                        //            p in db.Productions.Where(m =>
                        //                 (queryBuilder.FieldId > 0 && m.FieldId == queryBuilder.FieldId && queryBuilder.ProductId < 1 && (m.Month == queryBuilder.Month)) ||
                        //                 (queryBuilder.ProductId > 0 && queryBuilder.FieldId < 1 && m.ProductId == queryBuilder.ProductId && (m.Month == queryBuilder.Month)) ||
                        //                 (queryBuilder.FieldId > 0 && queryBuilder.ProductId > 0 && queryBuilder.ProductId > 0 && queryBuilder.FieldId > 0 && m.FieldId == queryBuilder.FieldId && m.ProductId == queryBuilder.ProductId && (m.Month == queryBuilder.Month)) ||
                        //                 (queryBuilder.FieldId < 1 && queryBuilder.ProductId < 1 && (m.Month == queryBuilder.Month)))


                        //          join f in db.Fields on p.FieldId equals f.FieldId where ((queryBuilder.FieldId > 0 && f.FieldId == queryBuilder.FieldId) || (f.FieldId == p.FieldId && queryBuilder.FieldId < 1))
                        //          join c in db.Companies on f.CompanyId equals c.CompanyId where
                        //              ((queryBuilder.CompanyId > 0 && c.CompanyId == queryBuilder.CompanyId) || (queryBuilder.CompanyId < 1 && c.CompanyId == f.CompanyId))
                        //          join z in db.Zones on f.ZoneId equals z.ZoneId
                        //          where ((queryBuilder.ZoneId > 0 && z.ZoneId == queryBuilder.ZoneId && z.ZoneId == f.ZoneId) || (z.ZoneId == f.ZoneId && queryBuilder.ZoneId < 1))
                        //          join t in db.Terrains on f.TerrainId equals t.TerrainId
                        //          where ((queryBuilder.TerrainId > 0 &&
                        //              t.TerrainId == queryBuilder.TerrainId) || (queryBuilder.TerrainId < 1 && t.TerrainId == f.TerrainId))
                        //          join pd in db.Products on p.ProductId equals pd.ProductId
                        //          where
                        //            ((queryBuilder.ProductId > 0 && pd.ProductId == queryBuilder.ProductId) ||
                        //             (queryBuilder.ProductId < 1 && p.ProductId == pd.ProductId))
                        //        join b in db.Blocks on f.BlockId equals b.BlockId
                        //        where
                        //           ((queryBuilder.BlockId > 0 && f.BlockId == queryBuilder.BlockId) ||
                        //           (b.BlockId == f.BlockId && queryBuilder.BlockId < 1))

                        //        select new ProductionObject()
                        //        {
                        //            ProductName = pd.Name,
                        //            BlockName = b.Name,
                        //            FieldName = f.Name,
                        //            CompanyName = c.Name,
                        //            ZoneName = z.Name,
                        //            TerrainName = t.Name,
                        //            Quantity = p.Quantity,
                        //            Year = p.Year,
                        //            Month = p.Month,
                        //            Remark = p.Remark,
                        //            APIGravity = p.APIGravity
                        //        }).ToList();
                        //    if (!query.Any())
                        //    {
                        //        return new List<ProductionObject>();
                        //    }

                        //    query.ForEach(j =>
                        //    {
                        //         var quantityOut = 0;
                        //        var result2 = int.TryParse(j.Quantity, out quantityOut);
                                
                        //        if (result2)
                        //        {
                        //            j.Quantity = Convert.ToDecimal(j.Quantity).ToString("#,##0");
                        //        }
                        //        else
                        //        {
                        //            j.Quantity = "0";
                        //        }
                        //        var monthOut = j.Month;

                        //        if (monthOut > 0)
                        //        {
                        //        j.Period = Enum.GetName(typeof (Months), monthOut) + "/" + j.Year;
                                    
                        //        }

                        //         if (monthOut < 1)
                        //        {
                        //            j.Period = j.Year.ToString(CultureInfo.InvariantCulture);
                                    
                        //        }
                               
                        //    });
                        //    return query.OrderBy(v => v.ProductName).ToList();
                        //}

                        if (((queryBuilder.StartDate != null && queryBuilder.StartDate.Value.Year == 0001) || (queryBuilder.StartDate == null)) && ((queryBuilder.EndDate != null && queryBuilder.EndDate.Value.Year == 0001) || queryBuilder.EndDate == null))
                        {
                            var query = (

                                from
                                    p in db.Productions.Where(m =>
                                         (queryBuilder.FieldId > 0 && m.FieldId == queryBuilder.FieldId && queryBuilder.ProductId < 1) ||
                                         (queryBuilder.ProductId > 0 && queryBuilder.FieldId < 1 && m.ProductId == queryBuilder.ProductId) ||
                                         (queryBuilder.FieldId > 0 && queryBuilder.ProductId > 0 && queryBuilder.ProductId > 0 && queryBuilder.FieldId > 0 && m.FieldId == queryBuilder.FieldId && m.ProductId == queryBuilder.ProductId) ||
                                         (queryBuilder.FieldId < 1 && queryBuilder.ProductId < 1 ))


                                  join f in db.Fields on p.FieldId equals f.FieldId where ((queryBuilder.FieldId > 0 && f.FieldId == queryBuilder.FieldId) || (f.FieldId == p.FieldId && queryBuilder.FieldId < 1))
                                  join c in db.Companies on f.CompanyId equals c.CompanyId where
                                      ((queryBuilder.CompanyId > 0 && c.CompanyId == queryBuilder.CompanyId) || (queryBuilder.CompanyId < 1 && c.CompanyId == f.CompanyId))
                                  join z in db.Zones on f.ZoneId equals z.ZoneId
                                  //where ((queryBuilder.ZoneId > 0 && z.ZoneId == queryBuilder.ZoneId && z.ZoneId == f.ZoneId) || (z.ZoneId == f.ZoneId && queryBuilder.ZoneId < 1))
                                  join t in db.Terrains on f.TerrainId equals t.TerrainId
                                  //where ((queryBuilder.TerrainId > 0 &&
                                  //    t.TerrainId == queryBuilder.TerrainId) || (queryBuilder.TerrainId < 1 && t.TerrainId == f.TerrainId))
                                  join pd in db.Products on p.ProductId equals pd.ProductId
                                  where
                                    ((queryBuilder.ProductId > 0 && pd.ProductId == queryBuilder.ProductId) ||
                                     (queryBuilder.ProductId < 1 && p.ProductId == pd.ProductId))
                                join b in db.Blocks on f.BlockId equals b.BlockId
                                where
                                   ((queryBuilder.BlockId > 0 && f.BlockId == queryBuilder.BlockId) ||
                                   (b.BlockId == f.BlockId && queryBuilder.BlockId < 1))

                                select new ProductionObject()
                                {
                                    ProductName = pd.Name,
                                    BlockName = b.Name,
                                    FieldName = f.Name,
                                    CompanyName = c.Name,
                                    ZoneName = z.Name,
                                    TerrainName = t.Name,
                                    Quantity = p.Quantity,
                                    Year = p.Year,
                                    Month = p.Month,
                                    Remark = p.Remark,
                                    APIGravity = p.APIGravity
                                }).ToList();

                            if (!query.Any())
                            {
                                return new List<ProductionObject>();
                            }

                            query.ForEach(j =>
                            {
                                var quantityOut = 0;
                                var result2 = int.TryParse(j.Quantity, out quantityOut);

                                if (result2)
                                {
                                    j.Quantity = Convert.ToDecimal(j.Quantity).ToString("#,##0");
                                }
                                else
                                {
                                    j.Quantity = "0";
                                }
                                var monthOut = j.Month;

                                if (monthOut > 0)
                                {
                                    j.Period = Enum.GetName(typeof(Months), monthOut) + "/" + j.Year;

                                }

                                if (monthOut < 1)
                                {
                                    j.Period = j.Year.ToString(CultureInfo.InvariantCulture);

                                }
                                
                            });
                            return query.OrderBy(v => v.ProductName).ToList();
                        }
                    }
                    return new List<ProductionObject>();
                }
                
            }
            catch (Exception e)
            {
                ErrorLogger.LogEror(e.StackTrace, e.Source, e.Message);
                return new List<ProductionObject>();
            }
        }
        
	}
	
}
