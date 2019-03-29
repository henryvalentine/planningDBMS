using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;

namespace DPR_DataMigrationEngine.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        public ProductController()
		{
			 ViewBag.LoadStatus = "0";
		}
        public ViewResult Products()
        {

            var productList = new ProductServices().GetAllOrderedProducts() ?? new List<Product>();

            if (!productList.Any())
            {
                ViewBag.Title = "Product SetUp";
                return View(productList);
            }
            
            ViewBag.Title = "Manage Products";
            return View(productList);
        }
        
        [HttpPost]
        public ActionResult AddProduct(Product product)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    product.Error = "Please supply all required fields and try again";
                    product.ErrorCode = -1;
                    return Json(product, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(product);

                if (wx.Code < 1)
                {
                    product.Error = wx.Error;
                    product.ErrorCode = -1;
                    return Json(product, JsonRequestBehavior.AllowGet);
                }

                product.Name = product.Name;
                var k = new ProductServices().AddProductCheckDuplicate(product);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        product.Error = "Product already exists";
                        product.ErrorCode = -3;
                        return Json(product, JsonRequestBehavior.AllowGet);
                    }

                    product.Error = "Process Failed! Please contact the Admin or try again later";
                    product.ErrorCode = 0;
                    return Json(product, JsonRequestBehavior.AllowGet);
                }

                product.Error = "Record was added successfully";
                product.ErrorCode = 1;
                product.ProductId = k;
                return Json(product, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                product.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                product.ErrorCode = 0;
                return Json(product, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditProduct(Product product)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_product"] == null)
                {
                    product.Error = "Session has expired";
                    product.ErrorCode = 0;
                    return Json(product, JsonRequestBehavior.AllowGet);
                }

                var oldProduct = Session["_product"] as Product;

                if (oldProduct == null || oldProduct.ProductId < 1)
                {
                    product.Error = "Session has expired";
                    product.ErrorCode = 0;
                    return Json(product, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    product.Error = "Please supply all required fields and try again";
                    product.ErrorCode = -1;
                    return Json(product, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(product);

                if (wx.Code < 1)
                {
                    product.Error = wx.Error;
                    product.ErrorCode = -1;
                    return Json(product, JsonRequestBehavior.AllowGet);
                }

                oldProduct.Name = product.Name;
                oldProduct.Description = product.Description;

                var k = new ProductServices().UpdateProductCheckDuplicate(oldProduct);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        product.Error = "Product already exists";
                        product.ErrorCode = 0;
                        return Json(product, JsonRequestBehavior.AllowGet);
                    }

                    product.Error = "Process Failed! Please contact the Admin or try again later";
                    product.ErrorCode = 0;
                    return Json(product, JsonRequestBehavior.AllowGet);
                }
                
                    product.Error = "Product Information was successfully updated";
                    product.ErrorCode = 1;
                    return Json(product, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                product.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                product.ErrorCode = 0;
                return Json(product, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("DeleteProduct")]
        public ActionResult DeleteProduct(int id)
        {
            var product = new Product();

            try
            {
                if (id < 1)
                {
                    product.Error = "Invalid Selection";
                    product.ErrorCode = 0;
                    return Json(product, JsonRequestBehavior.AllowGet);
                }
                if (new ProductServices().DeleteProductCheckReferences(id))
                {
                    product.Error = "Product Information was successfully deleted.";
                    product.ErrorCode = 1;
                    return Json(product, JsonRequestBehavior.AllowGet);
                }

                product.Error = "Process Failed! Please try again later";
                product.ErrorCode = 0;
                return Json(product, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                product.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                product.ErrorCode = 0;
                return Json(product, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditProduct(int id)
        {
            var product = new Product();
            try
            {
                if (id < 1)
                {
                    product.Error = "Invalid Selection!";
                    product.ErrorCode = -1;
                    return Json(product, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new ProductServices().GetProduct(id);

                if (myViewObj == null || myViewObj.ProductId < 1)
                {
                    product.Error = "Product Information could not be retrieved.";
                    product.ErrorCode = -1;
                    return Json(product, JsonRequestBehavior.AllowGet);
                }
                Session["_product"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.ProductId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                product.Error = "An unknown error was Product Information could not be retrieved.";
                product.ErrorCode = -1;
                return Json(product, JsonRequestBehavior.AllowGet);
            }
        }

        private static GenericValidator ValidateControl(Product model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.Name.Trim()))
                {
                    gVal.Error = "Please enter Product Name.";
                    gVal.Code = 0;
                    return gVal;
                }

                //if (string.IsNullOrEmpty(model.Description.Trim()))
                //{
                //    gVal.Error = "Please provide Product Description.";
                //    gVal.Code = 0;
                //    return gVal;
                //}
               
                gVal.Code = 1;
                return gVal;
            }
            catch (Exception )
            {
                gVal.Error = "Process validation failed. Please supply all required fields and try again.";
                gVal.Code = 0;
                return gVal;
            }
        }

        public int GetLoggedOnUserId()
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser != null)
            {
                return Convert.ToInt32(membershipUser.ProviderUserKey);
            }
            return 0;
        }
    }
}