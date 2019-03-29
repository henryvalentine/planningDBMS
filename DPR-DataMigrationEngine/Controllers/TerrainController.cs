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
    [CustomAuthorize]
    public class TerrainController : Controller
    {
        public TerrainController()
		{
			 ViewBag.LoadStatus = "0";
		}
        public ActionResult Terrains()
        {
           
            var terrainList = new TerrainServices().GetAllOrderedTerrains() ?? new List<Terrain>();

            if (!terrainList.Any())
            {
                ViewBag.Edit = 1;
                ViewBag.Title = "Terrain SetUp";
                return View(new List<Terrain>());
            }

            terrainList.Remove(terrainList.Find(m => m.TerrainId == (int)OtherNotAvailable.Not_Available));
            ViewBag.Title = "Manage Terrains";
            return View(terrainList);
        }

        [HttpPost]
        public ActionResult AddTerrain(Terrain terrain)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (!ModelState.IsValid)
                {
                    terrain.Error = "Please supply all required fields and try again";
                    terrain.ErrorCode = -1;
                    return Json(terrain, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(terrain);

                if (wx.Code < 1)
                {
                    terrain.Error = wx.Error;
                    terrain.ErrorCode = -1;
                    return Json(terrain, JsonRequestBehavior.AllowGet);
                }
              
                var k = new TerrainServices().AddTerrainCheckDuplicate(terrain);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        terrain.Error = "Terrain  already exists";
                        terrain.ErrorCode = -3;
                        return Json(terrain, JsonRequestBehavior.AllowGet);
                    }

                    terrain.Error = "Process Failed! Please contact the Admin or try again later";
                    terrain.ErrorCode = 0;
                    return Json(terrain, JsonRequestBehavior.AllowGet);
                }

                terrain.Error = "Record was added successfully";
                terrain.ErrorCode = 1;
                terrain.TerrainId = k;
                return Json(terrain, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                terrain.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                terrain.ErrorCode = 0;
                return Json(terrain, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditTerrain(Terrain terrain)
        {
            ModelState.Clear();
            ViewBag.LoadStatus = "0";
            try
            {
                if (Session["_terrain"] == null)
                {
                    terrain.Error = "Session has expired";
                    terrain.ErrorCode = 0;
                    return Json(terrain, JsonRequestBehavior.AllowGet);
                }

                var oldTerrain = Session["_terrain"] as Terrain;

                if (oldTerrain == null || oldTerrain.TerrainId < 1)
                {
                    terrain.Error = "Session has expired";
                    terrain.ErrorCode = 0;
                    return Json(terrain, JsonRequestBehavior.AllowGet);
                }

                if (!ModelState.IsValid)
                {
                    terrain.Error = "Please supply all required fields and try again";
                    terrain.ErrorCode = -1;
                    return Json(terrain, JsonRequestBehavior.AllowGet);
                }

                var wx = ValidateControl(terrain);

                if (wx.Code < 1)
                {
                    terrain.Error = wx.Error;
                    terrain.ErrorCode = -1;
                    return Json(terrain, JsonRequestBehavior.AllowGet);
                }

                oldTerrain.Name = terrain.Name;
                oldTerrain.ZoneName = terrain.ZoneName;
                
                var k = new TerrainServices().UpdateTerrainCheckDuplicate(oldTerrain);
                if (k < 1)
                {
                    if (k == -3)
                    {
                        terrain.Error = "Terrain  already exists";
                        terrain.ErrorCode = 0;
                        return Json(terrain, JsonRequestBehavior.AllowGet);
                    }

                    terrain.Error = "Process Failed! Please contact the Admin or try again later";
                    terrain.ErrorCode = 0;
                    return Json(terrain, JsonRequestBehavior.AllowGet);
                }
                
                    terrain.Error = "Terrain  Information was successfully updated";
                    terrain.ErrorCode = 1;
                    return Json(terrain, JsonRequestBehavior.AllowGet);
              
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                terrain.Error = "An unknown error was encountered. Request could not be serviced. Please try again later.";
                terrain.ErrorCode = 0;
                return Json(terrain, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ActionName("DeleteTerrain")]
        public ActionResult DeleteTerrain(int id)
        {
            var terrain = new Terrain();

            try
            {
                if (id < 1)
                {
                    terrain.Error = "Invalid Selection";
                    terrain.ErrorCode = 0;
                    return Json(terrain, JsonRequestBehavior.AllowGet);
                }
                if (new TerrainServices().DeleteTerrainCheckReferences(id))
                {
                    terrain.Error = "Terrain  Information was successfully deleted.";
                    terrain.ErrorCode = 1;
                    return Json(terrain, JsonRequestBehavior.AllowGet);
                }

                terrain.Error = "Process Failed! Please try again later";
                terrain.ErrorCode = 0;
                return Json(terrain, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                terrain.Error = "An unknown error was encountered. Please contact the Administrator or try again later.";
                terrain.ErrorCode = 0;
                return Json(terrain, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditTerrain(int id)
        {
            var terrain = new Terrain();
            try
            {
                if (id < 1)
                {
                    terrain.Error = "Invalid Selection!";
                    terrain.ErrorCode = -1;
                    return Json(terrain, JsonRequestBehavior.AllowGet);
                }

                var myViewObj = new TerrainServices().GetTerrain(id);

                if (myViewObj == null || myViewObj.TerrainId < 1)
                {
                    terrain.Error = "Terrain  Information could not be retrieved.";
                    terrain.ErrorCode = -1;
                    return Json(terrain, JsonRequestBehavior.AllowGet);
                }
                Session["_terrain"] = myViewObj;
                myViewObj.ErrorCode = myViewObj.TerrainId;
                return Json(myViewObj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogEror(ex.StackTrace, ex.Source, ex.Message);
                terrain.Error = "An unknown error was Terrain  Information could not be retrieved.";
                terrain.ErrorCode = -1;
                return Json(terrain, JsonRequestBehavior.AllowGet);
            }
        }


        private static GenericValidator ValidateControl(Terrain model)
        {
            var gVal = new GenericValidator();

            try
            {
                if (string.IsNullOrEmpty(model.Name.Trim()))
                {
                    gVal.Error = "Please enter Terrain  Name.";
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