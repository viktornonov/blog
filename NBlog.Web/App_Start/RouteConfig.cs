﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NBlog.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.RouteExistingFiles = false;
//            routes.LowercaseUrls = true;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.ico");

            // homepage
            routes.MapRoute("", "", new { controller = "Entry", action = "Index" });

            // combined scripts
            routes.MapRoute("", "min.css", new { controller = "Resource", action = "Css" });
            routes.MapRoute("", "min.js", new { controller = "Resource", action = "JavaScript" });

            // feed
            routes.MapRoute("", "feed", new { controller = "Feed", action = "Index" });

            // entry pages
            routes.MapRoute("", "{id}", new { controller = "Entry", action = "Show" });

            // general route
            routes.MapRoute("", "{controller}/{action}/{id}", new { id = UrlParameter.Optional });
        }
    }
}