using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;
using NBlog.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace NBlog.Web.Controllers
{
    public partial class EntryController : Controller
    {
        protected readonly IServices Services;

        public EntryController(IServices services)
        {
            Services = services;
        }

        [HttpGet]
        public ActionResult Show([Bind(Prefix = "id")] string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentNullException("slug");

            Entry entry;
            try
            {
                entry = Services.Entry.GetBySlug(slug);
            }
            catch (Exception ex)
            {
                throw new HttpException(404, "Entry not found", ex);
            }

            var html = entry.HtmlByMarkdown;

            var model = new Entry
            {
                Date = entry.DateCreated.ToString("MMMM dd, yyyy"),
                Slug = entry.Slug,
                Title = entry.Title,
                Html = html,
                IsCodePrettified = entry.IsCodePrettified ?? true
            };
//            model.Config = Services.Config.Current;
            ViewBag.Config = Services.Config.Current;

            return View(model);
        }
    }
}