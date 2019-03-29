using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;

namespace DPR_DataMigrationEngine.Services.ServiceManager
{
	public class ProductServices
	{
        public List<Product> GetAllOrderedProducts()
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObjList = db.Products.ToList();
                    if (!myObjList.Any())
                    {
                        return new List<Product>();
                    }
                    return myObjList.OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Product>();
            }
        }
        public int AddProductCheckDuplicate(Product product)
        {
            try
            {
                if (product == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Products.Any())
                    {
                        if (db.Products.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == product.Name.ToLower().Replace(" ", string.Empty)) > 0)
                        {
                            return -3;
                        }
                    }

                    var processedItem = db.Products.Add(product);
                    db.SaveChanges();
                    return processedItem.ProductId;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateProductCheckDuplicate(Product product)
        {
            try
            {
                if (product == null)
                { return -2; }
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Products.Any())
                    {
                        if (db.Products.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == product.Name.ToLower().Replace(" ", string.Empty) && m.ProductId != product.ProductId) > 0)
                        {
                            return -3;
                        }
                    }

                    db.Products.Attach(product);
                    db.Entry(product).State = EntityState.Modified;
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public bool DeleteProductCheckReferences(int productId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    if (db.Productions.Count(m => m.ProductId == productId) > 0)
                    {
                        return false;
                    }

                    var myObj = db.Products.Where(s => s.ProductId == productId).ToList();
                    if (!myObj.Any())
                    {
                        return false;
                    }
                    db.Products.Remove(myObj[0]);
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
        public Product GetProduct(int productId)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Products.Where(s => s.ProductId == productId).ToList();
                    if (!myObj.Any())
                    {
                        return new Product();
                    }

                    return myObj[0];
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new Product();
            }
        }

        public int GetProductIdId(string productName)
        {
            try
            {
                using (var db = new DPRDataMigrationEngineDBEntities())
                {
                    var myObj = db.Products.Where(s => s.Name.ToLower().Replace(" ", string.Empty).Trim() == productName.ToLower().Replace(" ", string.Empty).Trim()).ToList();
                    if (!myObj.Any())
                    {
                        if (db.Products.Any())
                        {
                            if (db.Products.Count(m => m.Name.ToLower().Replace(" ", string.Empty) == productName.ToLower().Replace(" ", string.Empty)) > 0)
                            {
                                return -3;
                            }
                        }
                        var product = new Product { Name = productName};
                        var processedProduct = db.Products.Add(product);
                        db.SaveChanges();
                        return processedProduct.ProductId;
                    }
                    var ttd = myObj[0];
                    return ttd.ProductId;
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
