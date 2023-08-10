using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WHO_Survey_System.HelpingClasses;

namespace WHO_Survey_System
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               "Survey",                                           // Route name
               "survey/{Company}/{action}/{id}",                   // URL with parameters
               new
               {
                   controller = "Company",
                   action = "Index",
                   company = "{Company}",
                   id = UrlParameter.Optional
               }  // Parameter defaults
           );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "LandingPage", id = UrlParameter.Optional }
            );



        }
    }
}
