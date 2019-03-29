using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPR_DataMigrationEngine.BulkUploadManagerManager;
using DPR_DataMigrationEngine.EF.CustomizedModels;
using DPR_DataMigrationEngine.EF.Models;
using DPR_DataMigrationEngine.GenericHelpers;
using DPR_DataMigrationEngine.Services.ServiceManager;

namespace DPR_DataMigrationEngine.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class PersonController : Controller
    {
        public PersonController()
		{
			 ViewBag.LoadStatus = "0";
		}
        public ViewResult Persons()
        {
            var companyList = new CompanyServices().GetAllOrderedCompanies() ?? new List<Company>();

            if (!companyList.Any())
            {
                ViewBag.Title = "Persons' Information Set Up";
                return View(Tuple.Create(new List<Company>(), new List<Person>()));
            }

            var personList = new PersonServices().GetAllOrderedPersons() ?? new List<Person>();

            if (!personList.Any())
            {
                ViewBag.Title = "Persons' Information Set Up";
                return View(Tuple.Create(companyList, new List<Person>()));
            }

            personList.ForEach(m =>
            {
                m.CompanyName = m.Company.Name;
            });
            
            ViewBag.Title = "Manage Persons' Information";
            return View(Tuple.Create(companyList, personList));
        }
        
        [HttpPost]
        public ActionResult AddPerson(Person person)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    person.Error = "Please supply all required fields and try again";
                    person.ErrorCode = -1;
                    return Json(person, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(person);

                if (wx.Code < 1)
                {
                    person.Error = wx.Error;
                    person.ErrorCode = -1;
                    return Json(person, JsonRequestBehavior.AllowGet);
                }
             
                var k = new PersonServices().AddPersonCheckDuplicate(person);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        person.Error = "Person's Information already exists";
                        person.ErrorCode = 0;
                        return Json(person, JsonRequestBehavior.AllowGet);
                    }

                    person.Error = "Process Failed! Please contact the Admin or try again later";
                    person.ErrorCode = 0;
                    return Json(person, JsonRequestBehavior.AllowGet);
                }
                person.Error = "Record was added successfully";
                person.ErrorCode = 1;
                person.PersonId = k;
                return Json(person, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                person.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                person.ErrorCode = 0;
                return Json(person, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditPerson(Person person)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_person"] == null)
                {
                    person.Error = "Session has expired";
                    person.ErrorCode = 0;
                    return Json(person, JsonRequestBehavior.AllowGet);
                }

                var oldPerson = Session["_person"] as Person;

                if (oldPerson == null || oldPerson.PersonId < 1)
                {
                    person.Error = "Session has expired";
                    person.ErrorCode = 0;
                    return Json(person, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    person.Error = "Please supply all required fields and try again";
                    person.ErrorCode = -1;
                    return Json(person, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(person);

                if (wx.Code < 1)
                {
                    person.Error = wx.Error;
                    person.ErrorCode = -1;
                    return Json(person, JsonRequestBehavior.AllowGet);
                }

                oldPerson.LastName = person.LastName;
                oldPerson.FirstName = person.FirstName;
                oldPerson.CompanyId = person.CompanyId;
                oldPerson.CompanyName = person.CompanyName;
                oldPerson.Designation = person.Designation;

                var k = new PersonServices().UpdatePersonCheckDuplicate(oldPerson);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        person.Error = "Person's Information already exists";
                        person.ErrorCode = 0;
                        return Json(person, JsonRequestBehavior.AllowGet);
                    }

                    person.Error = "Process Failed! Please contact the Admin or try again later";
                    person.ErrorCode = 0;
                    return Json(person, JsonRequestBehavior.AllowGet);
                }
                
                    person.Error = "Person's Information was successfully updated";
                    person.ErrorCode = 1;
                    return Json(person, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                person.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                person.ErrorCode = 0;
                return Json(person, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("DeletePerson")]
        public ActionResult DeletePerson(int id)
        {
            var person = new Person();

            try
            {
                if (id < 1)
                {
                    person.Error = "Invalid Selection";
                    person.ErrorCode = 0;
                    return Json(person, JsonRequestBehavior.AllowGet);
                }
                if (new PersonServices().DeletePersonCheckReferences(id))
                {
                    person.Error = "Person's Information was successfully deleted.";
                    person.ErrorCode = 1;
                    return Json(person, JsonRequestBehavior.AllowGet);
                }

                person.Error = "Process Failed! Please try again later";
                person.ErrorCode = 0;
                return Json(person, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                person.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                person.ErrorCode = 0;
                return Json(person, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult EditPerson(int id)
        {
            var person = new Person();
            try
            {
                if (id < 1)
                {
                    person.Error = "Invalid Selection!";
                    person.ErrorCode = -1;
                    return Json(person, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new PersonServices().GetPerson(id);

                if (myViewObj == null || myViewObj.PersonId < 1)
                {
                    person.Error = "Person's Information could not be retrieved.";
                    person.ErrorCode = -1;
                    return Json(person, JsonRequestBehavior.AllowGet);
                }
                Session["_person"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.PersonId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                person.Error = "An unknown error was encountered. The Person's Information could not be retrieved.";
                person.ErrorCode = -1;
                return Json(person, JsonRequestBehavior.AllowGet);
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

                if (model.CompanyId < 1)
                {
                    gVal.Error = "Please select the Person's Company.";
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


        [HttpPost]
        public ActionResult SaveToFolder(HttpPostedFileBase file, int companyId)
        {
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

                    var mList = new List<Person>();
                    var msg = string.Empty;
                    if (!new PersonUploadManager().Import(Server.MapPath(path), "Person", ref mList, ref msg))
                    {
                        return RedirectToAction("Persons");
                    }

                    if (!mList.Any())
                    {
                        var error = msg.Length > 0 ? msg : "Bulk upload Failed! unknown error occurred";

                        return RedirectToAction("Persons");
                    }

                    var errorList = new List<Person>();
                    mList.ForEach(m =>
                    {
                        m.CompanyId = companyId;
                        var k = new PersonServices().AddPersonCheckDuplicate(m);
                        if (k < 1)
                        {
                            errorList.Add(m);
                        }
                    });
                    return RedirectToAction("Persons");
                }
                return RedirectToAction("Persons");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Persons");
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