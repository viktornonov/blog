using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using NBlog.Web.Application.Extensions;
using NBlog.Web.Application.Core;
using NBlog.Web.Models;
using Newtonsoft.Json;

namespace NBlog.Web.Controllers
{
    public class FeedController : Controller
    {
        private static readonly Regex _linkRegex = new Regex(@"(?<=\[[^\]]*\]\()(?<url>[^\)]*)(?=\))");

        public FeedController()
        {
        }

        public ActionResult Index()
        {
            var blogRepository = new BlogRepository<Entry>();
            var config = new NBlog.Web.Application.Core.Config();
            var jsonString = System.IO.File.ReadAllText(Config.path);
            var configOptions = JsonConvert.DeserializeObject<Config>(jsonString);

            var markdown = new MarkdownSharp.Markdown();
            var baseUri = new Uri(Request.Url.GetLeftPart(UriPartial.Authority));
            
            var entries =
                blogRepository.GetList()
                .Where(e => e.IsPublished)
                .OrderByDescending(e => e.DateCreated)
                .Take(10)
                .Select(e => new SyndicationItem(
                    e.Title,
                    e.HtmlByMarkdown,
                    new Uri(baseUri, Url.Action("Show", "Entry", new { id = e.Slug }, null))));

            var feed = new SyndicationFeed(
                title: configOptions.Heading,
                description: configOptions.Tagline,
                feedAlternateLink: new Uri(baseUri, Url.Action("Index", "Feed")),
                items: entries);

            return new RssResult(feed);
        }

        private string UrlsToAbsolute(string markdown)
        {
            return _linkRegex.Replace(markdown, ToAbsoluteUrl);
        }

        private string ToAbsoluteUrl(Match match)
        {
            var url = match.Groups["url"].Value;
            if (!(Uri.IsWellFormedUriString(url, UriKind.Absolute)))
            {
                return ToPublicUrl(Request.RequestContext.HttpContext, new Uri(url, UriKind.Relative));
            }
            return url;
        }

        private static string ToPublicUrl(HttpContextBase httpContext, Uri relativeUri)
        {
            var uriBuilder = new UriBuilder
            {
                Host = httpContext.Request.Url.Host,
                Path = "/",
                Port = 80,
                Scheme = "http",
            };

            if (httpContext.Request.IsLocal)
            {
                uriBuilder.Port = httpContext.Request.Url.Port;
            }

            return new Uri(uriBuilder.Uri, relativeUri).AbsoluteUri;
        }
    }
}
