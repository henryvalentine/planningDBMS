using System.Web.Mvc;
using System.Web.Routing;

namespace DPR_DataMigrationEngine
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "EquipmentTypes",
               url: "EquipmentType/EquipmentTypes",
               defaults: new { controller = "EquipmentType", action = "EquipmentTypes"}
           );

            routes.MapRoute(
            name: "WellBulkUpload",
            url: "Well/SaveToFolder",
            defaults: new { controller = "Well", action = "SaveToFolder"}
        );

           // routes.MapRoute(
           //name: "Project",
           //url: "Project/EditProject",
           //defaults: new { controller = "Well", action = "SaveToFolder" }
       //);


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
