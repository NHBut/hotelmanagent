using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QuanLyKhachSan
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
             name: "index page",
             url: "PublicRoom/Index/{page}",
             defaults: new { controller = "PublicRoom", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
            name: "index citi page",
            url: "PublicCity/Index/{page}",
            defaults: new { controller = "PublicCity", action = "Index", page = UrlParameter.Optional }
           );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "PublicHome", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
             name: "details2",
             url: "PublicHotel/DetailHotel/{id}/{page}",
             defaults: new { controller = "PublicHotel", action = "DetailHotel", id = UrlParameter.Optional, page = UrlParameter.Optional }
            );
            routes.MapRoute(
            name: "details",
            url: "PublicHotel/DetailHotel/{id}",
            defaults: new { controller = "PublicRoom", action = "Index", id = UrlParameter.Optional }
           );
           

        }
    }
}
