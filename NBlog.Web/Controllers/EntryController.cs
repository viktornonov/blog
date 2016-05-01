using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBlog.Web.Application.Core;
using NBlog.Web.Models;

namespace NBlog.Web.Controllers
{
    public partial class EntryController : Controller
    {
        protected readonly Config configOptions;
        protected readonly BlogRepository<Entry> blogRepository;

        public EntryController()
        {
            blogRepository = new BlogRepository<Entry>();
            var config = new NBlog.Web.Application.Core.Config();
            var jsonString = System.IO.File.ReadAllText(Config.path);
            configOptions = JsonConvert.DeserializeObject<Config>(jsonString);
        }

        [HttpGet]
        public ViewResult Index()
        {
            var entries = blogRepository.GetList();

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

            ViewBag.Config = configOptions;

            return View();
        }

        [HttpGet]
        public ActionResult Show([Bind(Prefix = "id")] string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentNullException("slug");

            Entry entry;
            try
            {
                entry = blogRepository.GetBySlug(slug);
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
            ViewBag.Config = configOptions;

            return View(model);
        }
    }
}