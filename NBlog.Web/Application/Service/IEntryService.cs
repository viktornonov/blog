using System.Collections.Generic;
using NBlog.Web.Application.Service.Entity;
using NBlog.Web.Models;

namespace NBlog.Web.Application.Service
{
    public interface IEntryService
    {
        Entry GetBySlug(string slug);
        List<Entry> GetList();
        bool Exists(string slug);
    }
}