using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBlog.Web.Application;
using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;
using NBlog.Web.Application.Service.Entity;

namespace NBlog.Web.Controllers
{
    public partial class HomeController : LayoutController
    {
        public HomeController(IServices services) : base(services) { }

        [HttpGet]
        public ViewResult Index()
        {
            var entries = Services.Entry.GetList();

            var model = new IndexModel
            {
                Entries = entries
                    .OrderByDescending(e => e.DateCreated)
                    .Select(e => new Entry
                    {
                        Slug = e.Slug,
                        Markdown = e.HtmlByMarkdown,
                        Title = e.Title,
                        IsFromGithub = e.IsFromGithub,
                        DateCreated = e.DateCreated,
                        IsPublished = e.IsPublished
                    })
            };

            return View(model);
        }

        [HttpGet]
        public ViewResult Throw()
        {
            throw new NotImplementedException("Example exception for testing error handling.");
        }

        [HttpGet]
        public ActionResult WhoAmI()
        {
            return Content(User.Identity.Name.AsNullIfEmpty() ?? "Not logged in");
        }

        [HttpGet]
        public ActionResult WhatTimeIsIt()
        {
            return Content(DateTime.Now.ToString());
        }
        
        public class IndexModel : LayoutModel
        {
            public IEnumerable<Entry> Entries { get; set; }
        }
    }
}
