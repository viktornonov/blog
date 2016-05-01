using System;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;

namespace NBlog.Web.Models
{
    public class Entry
    {
        [BsonId]
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime DateCreated { get; set; }
        public bool? IsFromGithub { get; set; }
        public string Markdown { get; set; }
        public bool IsPublished { get; set; }
        public bool? IsCodePrettified { get; set; }

        public string Html { get; set; }
        public string Date { get; set; }

        public string HtmlByMarkdown
        {
            get
            {
                string html = String.Empty;
                if(this.IsFromGithub == null || !(bool)this.IsFromGithub)
                {
                    var markdown = new MarkdownSharp.Markdown();
                    html = markdown.Transform(this.Markdown);
                }
                else
                {
                    string markdownText = GetPostContentFromGithub(this.Slug);
                    var markdown = new MarkdownSharp.Markdown();
                    html = markdown.Transform(markdownText);
                    Match matchTitle = Regex.Match(html, @"<h1>(.*)<\/h1>");
                    if (matchTitle.Success)
                    {
                        
                        this.Title = matchTitle.Groups[1].ToString();
                        html = Regex.Replace(html, @"<h1>.*<\/h1>", "");
                    }
                }

                return html;
            }
        }
        public override string ToString()
        {
            return Title;
        }

        private string GetPostContentFromGithub(string slug)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://raw.githubusercontent.com/viktornonov/blog-posts/master/" + slug + "/" + slug + ".md");
            request.Method = "GET";
            string html = String.Empty;
            using (var reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                html = reader.ReadToEnd();
            }

            return html;
        }
    }
}