using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DPR_DataMigrationEngine.BulkUploadManagerManager;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;
using Microsoft.Ajax.Utilities;

namespace DPR_DataMigrationEngine.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class CompanyController : BaseController
    {
        public CompanyController()
		{
			 ViewBag.LoadStatus = "0";
		}

        private const int ItemsPerPage = 50;
        private const int PageNumber = 1;
        public ViewResult Companies(int? itemsPerPage, int? pageNumber)
        {
            int dataCount;
            var companyList =  GetCompanies(itemsPerPage?? ItemsPerPage, pageNumber?? PageNumber, out dataCount);

            if (!companyList.Any())
            {
                ViewBag.Title = "Company SetUp";
                return View(companyList);
            }
            Session["_page"] = 1;
            ViewBag.Title = "Manage Companies";
            return View(companyList);
        }

        public ActionResult GetMoreCompanies()
        {
            int currentPage;

            var o = Session["_page"];
            if (o != null)
            {
                var page = (int)o;
                if (page < 1)
                {
                    currentPage = 1;
                }

                else
                {
                    currentPage = page + 1;
                }
            }
            else
            {
                currentPage = 1;
            }

            var companyList = GetCompanyObjects(50, currentPage);

            if (!companyList.Any())
            {
                return Json(new List<Company>(), JsonRequestBehavior.AllowGet);
            }
            Session["_page"] = currentPage;
            return Json(companyList, JsonRequestBehavior.AllowGet);
        }
        private List<Company> GetCompanies(int itemsPerPage, int pageNumber, out int dataCount)
        {
            try
            {
                var companyList = new CompanyServices().GetAllOrderedCompanies(itemsPerPage, pageNumber, out dataCount) ?? new List<Company>();

                if (!companyList.Any())
                {
                    return new List<Company>();
                }
                
                ViewBag.DataCount = dataCount.ToString(CultureInfo.InvariantCulture);

                var totalPages = dataCount / itemsPerPage;

                // Counting the last page
                if (dataCount % itemsPerPage != 0)
                {
                    totalPages++;
                }

                ViewBag.TotalPages = totalPages;
                ViewBag.Page = pageNumber;
                return companyList;
            }
            catch (Exception ex)
            {
                dataCount = 0;
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<Company>();
            }
        }

        private List<CompanyObject> GetCompanyObjects(int itemsPerPage, int pageNumber)
        {
            try
            {
                var companyList = new CompanyServices().GetCompanyObjects(itemsPerPage, pageNumber) ?? new List<CompanyObject>();

                if (!companyList.Any())
                {
                    return new List<CompanyObject>();
                }
                companyList.Remove(companyList.Find(m => m.CompanyId == (int)OtherNotAvailable.Not_Available));
               
                return companyList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return new List<CompanyObject>();
            }
        }

        [HttpPost]
        public ActionResult AddCompany(Company company, Person person)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    company.Error = "Please supply all required fields and try again";
                    company.ErrorCode = -1;
                    return Json(company, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(company);

                if (wx.Code < 1)
                {
                    company.Error = wx.Error;
                    company.ErrorCode = -1;
                    return Json(company, JsonRequestBehavior.AllowGet);
                }

                var wx2 = ValidateControl(person);

                if (wx2.Code < 1)
                {
                    company.Error = wx2.Error;
                    company.ErrorCode = -1;
                    return Json(company, JsonRequestBehavior.AllowGet);
                }

                company.People = new List<Person> {person};
                var k = new CompanyServices().AddCompanyCheckDuplicate(company);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        company.Error = "Company already exists";
                        company.ErrorCode = 0;
                        return Json(company, JsonRequestBehavior.AllowGet);
                    }

                    company.Error = "Process Failed! Please contact the Admin or try again later";
                    company.ErrorCode = 0;
                    return Json(company, JsonRequestBehavior.AllowGet);
                }
                
                var dct = new CompanyObject
                {
                    RCNumber = company.RCNumber,
                    Name = company.Name,
                    Address = company.Address,
                    CompanyId = k,
                    PersonName = person.FirstName + " " + person.LastName,
                    ErrorCode = 1,
                     Error = "Company was successfully added"
                };
              
                return Json(dct, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                company.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                company.ErrorCode = 0;
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return Json(company, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditCompany(Company company, Person person)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_company"] == null)
                {
                    company.Error = "Session has expired";
                    company.ErrorCode = 0;
                    return Json(company, JsonRequestBehavior.AllowGet);
                }

                var oldCompany = Session["_company"] as CompanyObject;

                if (oldCompany == null || oldCompany.CompanyId < 1)
                {
                    company.Error = "Session has expired";
                    company.ErrorCode = 0;
                    return Json(company, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    company.Error = "Please supply all required fields and try again";
                    company.ErrorCode = -1;
                    return Json(company, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(company);

                if (wx.Code < 1)
                {
                    company.Error = wx.Error;
                    company.ErrorCode = -1;
                    return Json(company, JsonRequestBehavior.AllowGet);
                }

                var newPerson = new Person();
                if (oldCompany.PersonObj != null && oldCompany.PersonObj.PersonId > 0)
               {
                   var oldPerson = oldCompany.PersonObj;
                   newPerson.FirstName = person.FirstName;
                   newPerson.LastName = person.LastName;
                   newPerson.Email = person.Email;
                   newPerson.Designation = person.Designation;
                   newPerson.PhoneNumber = person.PhoneNumber;
                   newPerson.PersonId = oldPerson.PersonId;
                   newPerson.CompanyId = oldCompany.CompanyId;
               }

                else
                {
                    newPerson = new Person
                    {
                        FirstName = person.FirstName,
                        LastName = person.LastName,
                        Email = person.Email,
                        Designation = person.Designation,
                        PhoneNumber = person.PhoneNumber,
                        CompanyId = oldCompany.CompanyId,
                    };

                }
                var newCompany = new Company
                {
                    RCNumber = company.RCNumber,
                    Name = company.Name,
                    Address = company.Address,
                    CompanyId = oldCompany.CompanyId,
                    People = null

                };

                var k = new CompanyServices().UpdateCompanyCheckDuplicate(newCompany, newPerson);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        company.Error = "Company already exists";
                        company.ErrorCode = 0;
                        return Json(company, JsonRequestBehavior.AllowGet);
                    }

                    if (k == -4)
                    {
                        company.Error = "This Contact Person has already be registered for this Company";
                        company.ErrorCode = 0;
                        return Json(company, JsonRequestBehavior.AllowGet);
                    }

                    company.Error = "Process Failed! Please contact the Admin or try again later";
                    company.ErrorCode = 0;
                    return Json(company, JsonRequestBehavior.AllowGet);
                }

                var dct = new CompanyObject
                {
                    RCNumber = company.RCNumber,
                    Name = company.Name,
                    Address = company.Address,
                    CompanyId = k,
                    PersonName = person.FirstName + " " + person.LastName,
                    ErrorCode = 1,
                    Error = "Company Information was successfully updated"
                };

                return Json(dct, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                company.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                company.ErrorCode = 0;
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return Json(company, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("DeleteCompany")]
        public ActionResult DeleteCompany(int id)
        {
            var company = new Company();

            try
            {
                if (id < 1)
                {
                    company.Error = "Invalid Selection";
                    company.ErrorCode = 0;
                    return Json(company, JsonRequestBehavior.AllowGet);
                }
                if (new CompanyServices().DeleteCompanyCheckReferences(id))
                {
                    company.Error = "Company Information was successfully deleted.";
                    company.ErrorCode = 1;
                    return Json(company, JsonRequestBehavior.AllowGet);
                }

                company.Error = "Process Failed! Please try again later";
                company.ErrorCode = 0;
                return Json(company, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                company.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                company.ErrorCode = 0;
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return Json(company, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditCompany(int id)
        {
            var company = new Company();
            try
            {
                if (id < 1)
                {
                    company.Error = "Invalid Selection!";
                    company.ErrorCode = -1;
                    return Json(company, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new CompanyServices().GetCompany(id);

                if (myViewObj == null || myViewObj.CompanyId < 1)
                {
                    company.Error = "Company Information could not be retrieved.";
                    company.ErrorCode = -1;
                    return Json(company, JsonRequestBehavior.AllowGet);
                }
                Session["_company"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.CompanyId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                company.Error = "An unknown error was Company Information could not be retrieved.";
                company.ErrorCode = -1;
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return Json(company, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetContactListByCompany(int id)
        {
            var company = new Company();
            try
            {
                if (id < 1)
                {
                    company.Error = "Invalid Selection!";
                    company.ErrorCode = -1;
                    return Json(company, JsonRequestBehavior.AllowGet);
                }

                var contactList = new PersonServices().GetAllOrderedPersonsByCompanyId(id);

                if (contactList == null || !contactList.Any())
                {
                    company.Error = "Company Contact List is empty.";
                    company.ErrorCode = -1;
                    return Json(company, JsonRequestBehavior.AllowGet);
                }
                return Json(contactList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                company.Error = "An unknown error was Company Information could not be retrieved.";
                company.ErrorCode = -1;
                return Json(company, JsonRequestBehavior.AllowGet);
            }
        }

        private static GenericValidator ValidateControl(Company model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.Name.Trim()))
                {
                    gVal.Error = "Please enter Company Name.";
                    gVal.Code = 0;
                    return gVal;
                }
               
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

        private static GenericValidator ValidateControl(Person model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.FirstName.Trim()))
                {
                    gVal.Error = "Please provide First Name.";
                    gVal.Code = 0;
                    return gVal;
                }

                if (string.IsNullOrEmpty(model.LastName.Trim()))
                {
                    gVal.Error = "Please provide Last Name.";
                    gVal.Code = 0;
                    return gVal;
                }

                if (string.IsNullOrEmpty(model.Designation.Trim()))
                {
                    gVal.Error = "Please provide Person's Designation.";
                    gVal.Code = 0;
                    return gVal;
                }

                //if (model.CompanyId < 1)
                //{
                //    gVal.Error = "Please select the Person's Company.";
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

        [HttpPost]
        public ActionResult SaveToFolder(HttpPostedFileBase file)
        {
           int dataCount;
            try
            {
                if (file.ContentLength > 0)
                {
                    const string folderPath = "~/BulkUploadTemplate";

                    var fileName = file.FileName;
                    var path = folderPath + "/" + fileName;

                    if (System.IO.File.Exists(Server.MapPath(path)))
                    {
                        System.IO.File.Delete(Server.MapPath(path));
                    }

                    file.SaveAs(Server.MapPath(folderPath + "/" + fileName));

                    var mList = new List<Company>();
                    var msg = string.Empty;
                    if (!new CompanyUploadManager().Import(Server.MapPath(path), "company", ref mList, ref msg))
                    {
                        ViewBag.ErrorMessage = msg;
                        return View("Companies", GetCompanies(ItemsPerPage, PageNumber, out dataCount));
                    }

                    if (!mList.Any())
                    {
                        ViewBag.Title = "Manage Companies";
                        var error = msg.Length > 0 ? msg : "Bulk upload Failed! unknown error occurred";
                        ViewBag.ErrorMessage = error;
                        return  View("Companies", GetCompanies(ItemsPerPage, PageNumber, out dataCount));
                    }

                    var errorList = (from companyInfo in mList let processedItem = new CompanyServices().AddCompanyCheckDuplicate(companyInfo) where processedItem < 1 select companyInfo).ToList();
                    if (errorList.Any())
                    {
                        ViewBag.ErrorMessage = errorList.Count + " " + "Companies could not be uploaded due to duplicates or unknown errors encountered.";
                        return View("Companies", GetCompanies(ItemsPerPage, PageNumber, out dataCount));
                    }
                    ViewBag.ErrorMessage = mList.Count + " " + "Companies successfully uploaded.";
                    Session["_page"] = 1;
                    return View("Companies", GetCompanies(ItemsPerPage, PageNumber, out dataCount));
                }
                ViewBag.Title = "Manage Companies";
                ViewBag.ErrorMessage = "The selected file is invalid";
                return View("Companies", GetCompanies(ItemsPerPage, PageNumber, out dataCount));
            }
            catch (Exception ex)
            {
                ViewBag.Title = "Manage Companies";
                ViewBag.ErrorMessage = ex.Message;
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return View("Companies", GetCompanies(ItemsPerPage, PageNumber, out dataCount)); 
            }
        }

        public bool DownloadContentFromFolder(string path)
        {
            try
            {
                Response.Clear();
                var filename = Path.GetFileName(path);
                HttpContext.Response.Buffer = true;
                HttpContext.Response.Charset = "";
                HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = GetMimeType(filename);
                HttpContext.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + filename + "\"");
                Response.WriteFile(Server.MapPath(path));
                Response.End();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            var extension = Path.GetExtension(fileName);
            if (extension != null)
            {
                var ext = extension.ToLower();
                var regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
                if (regKey != null && regKey.GetValue("Content Type") != null)
                    mimeType = regKey.GetValue("Content Type").ToString();
            }
            return mimeType;
        }
    }
}