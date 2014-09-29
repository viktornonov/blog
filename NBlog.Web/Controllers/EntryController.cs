﻿using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;
using NBlog.Web.Application.Service.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace NBlog.Web.Controllers
{
    public partial class EntryController : LayoutController
    {
        public EntryController(IServices services) : base(services) { }

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

            var html = String.Empty;
            if((bool)entry.IsFromGithub)
            {
                var markdown = new MarkdownSharp.Markdown();
                html = markdown.Transform(entry.Markdown);
            }
            else
            {
                // TODO: Extract to another method
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://raw.githubusercontent.com/viktornonov/blog/master/NBlog.Web/App_Data/localhost/Entry/" + slug + ".json");
                request.Method = "GET";
                using (var reader = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    html = reader.ReadToEnd();
                }
                Dictionary<string, string> json = new JavaScriptSerializer().Deserialize<Dictionary<string,string>>(html);
                var markdown = new MarkdownSharp.Markdown();
                html = markdown.Transform(entry.Markdown);
            }

            var model = new ShowModel
            {
                Date = entry.DateCreated.ToString("dddd, dd MMMM yyyy"),
                Slug = entry.Slug,
                Title = entry.Title,
                Html = html,
                IsCodePrettified = entry.IsCodePrettified ?? true
            };

            return View(model);
        }


        [AdminOnly]
        [HttpGet]
        public ActionResult Add()
        {
            return View("Edit", new EditModel());
        }


        [AdminOnly]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Add(EditModel model)
        {
            var slug = model.Title.ToUrlSlug();

            // (these fields are hidden when creating a new entry, so don't validate them)
            ModelState["NewSlug"].Errors.Clear();
            ModelState["Date"].Errors.Clear();

            if (Services.Entry.Exists(slug))
                ModelState.AddModelError("Title", "Sorry, a post already exists with the slug '" + slug + "', please change the title.");

            if (!ModelState.IsValid)
                return View("Edit", model);

            var entry = new Entry
            {
                Title = model.Title,
                Markdown = model.Markdown,
                Slug = slug,
                Author = Services.User.Current.FriendlyName,
                DateCreated = DateTime.Now,
                IsPublished = false
            };

            Services.Entry.Save(entry);

            return RedirectToAction("Show", "Entry", new { id = entry.Slug });
        }


        [AdminOnly]
        [HttpGet]
        public ActionResult Edit([Bind(Prefix = "id")] string slug)
        {
            var entry = Services.Entry.GetBySlug(slug);

            var model = new EditModel
            {
                Title = entry.Title,
                Date = entry.DateCreated.ToString("dd MMM yyyy", Thread.CurrentThread.CurrentCulture),
                Markdown = entry.Markdown,
                Slug = slug,
                NewSlug = slug,
                IsPublished = entry.IsPublished ?? true,
                IsCodePrettified = entry.IsCodePrettified ?? true
            };

            return View(model);
        }


        [AdminOnly]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Edit(EditModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var entry = Services.Entry.GetBySlug(model.Slug);
            entry.Title = model.Title;
            entry.DateCreated = DateTime.ParseExact(model.Date, "dd MMM yyyy", Thread.CurrentThread.CurrentCulture);
            entry.Markdown = model.Markdown;
            entry.IsPublished = model.IsPublished;
            entry.IsCodePrettified = model.IsCodePrettified;
            
            var slugChanged =
                !string.Equals(model.Slug, model.NewSlug, StringComparison.InvariantCultureIgnoreCase)
                && !string.IsNullOrWhiteSpace(model.NewSlug);

            if (slugChanged)
            {
                if (Services.Entry.Exists(model.NewSlug))
                {
                    ModelState.AddModelError("NewSlug", "Sorry, a post with that slug already exists.");
                    return View(model);
                }
                Services.Entry.Delete(model.Slug);
                entry.Slug = model.NewSlug;
            }

            Services.Entry.Save(entry);

            return RedirectToAction("Show", "Entry", new { id = entry.Slug });
        }

        [AdminOnly]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Delete(DeleteModel model)
        {
            Services.Entry.Delete(model.Slug);
            return RedirectToAction("Index", "Home");
        }

        [AdminOnly]
        [HttpGet]
        public ActionResult Delete([Bind(Prefix = "id")] string slug)
        {
            var entry = Services.Entry.GetBySlug(slug);

            var model = new DeleteModel
            {
                Title = entry.Title,
                Slug = slug
            };

            return View(model);
        }
    }
}