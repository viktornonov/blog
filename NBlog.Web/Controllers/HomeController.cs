using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBlog.Web.Application;
using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;
using NBlog.Web.Application.Service.Entity;
using NBlog.Web.Models;

namespace NBlog.Web.Controllers
{
    public partial class HomeController : Controller
    {
        protected readonly IServices Services;

        public HomeController(IServices services)
        {
            Services = services;
        }

        [HttpGet]
        public ViewResult Index()
        {
            var entries = Services.Entry.GetList();

            ViewBag.Entries = entries
                                .OrderByDescending(e => e.DateCreated)
                                .Select(e => new Entry
                                {
                                    Slug = e.Slug,
                                    Markdown = e.HtmlByMarkdown,
                                    Title = e.Title,
                                    IsFromGithub = e.IsFromGithub,
                                    DateCreated = e.DateCreated,
                                    IsPublished = e.IsPublished
                                });

            ViewBag.Config = Services.Config.Current;

            return View();
        }

        [HttpGet]
        public ViewResult Throw()
        {
            throw new NotImplementedException("Example exception for testing error handling.");
        }
    }
}
