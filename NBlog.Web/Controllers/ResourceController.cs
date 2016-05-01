using NBlog.Web.Application;
using NBlog.Web.Application.Infrastructure;
using SquishIt.Framework;
using SquishIt.Framework.Minifiers.CSS;
using System.Web.Mvc;
using JavaScriptMinifiers = SquishIt.Framework.Minifiers.JavaScript;

namespace NBlog.Web.Controllers
{
    public class ResourceController : Controller
    {
        public ResourceController()
        {
        }

        [HttpGet]
        public ActionResult Css()
        {
            var cacheKey = "yo-yo-marafara-css";

            Bundle.Css()
                //.Add(Services.Theme.Current.Css("style"))
                .Add("~/scripts/prettify/prettify.css")
                .ForceRelease()
                .WithMinifier(new MsMinifier())
                .AsCached(cacheKey, "");

            var css = Bundle.Css().RenderCached(cacheKey);

            return Content(css, "text/css");
        }

        [HttpGet]
        public ActionResult JavaScript()
        {
            const string cacheKey = "nblog-js";

            Bundle.JavaScript()
                .Add("~/scripts/prism.js")
                .Add("~/scripts/prettify/prettify.js").WithMinifier(new JavaScriptMinifiers.NullMinifier())
                .ForceRelease().WithMinifier(new JavaScriptMinifiers.MsMinifier()).AsCached(cacheKey, "");

            var js = Bundle.JavaScript().RenderCached(cacheKey);

            return Content(js, "text/javascript");
        }
    }
}