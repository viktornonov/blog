﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace NBlog.Web.Application.Service.Entity
{
    public class Config
    {
        public List<string> Admins { get; set; }

        public ContactFormConfig ContactForm { get; set; }

        public string Crossbar { get; set; }

        public DisqusConfig Disqus { get; set; }

        public string GoogleAnalyticsId { get; set; }

        public string Heading { get; set; }

        public string MetaDescription { get; set; }

        public string Site { get; set; }

        public string Tagline { get; set; }

        public string Theme { get; set; }

        public string Title { get; set; }

        public class ContactFormConfig
        {
            public string RecipientEmail { get; set; }

            public string RecipientName { get; set; }

            public string Subject { get; set; }
        }

        public class DisqusConfig
        {
            public bool DevelopmentMode { get; set; }

            public string Shortname { get; set; }
        }
    }
}