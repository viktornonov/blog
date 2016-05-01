using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NBlog.Web.Models;

namespace NBlog.Web.Application.Core
{
    public interface IRepository<T>
    {
        T Single<T>(object key) where T : class, new();
        IEnumerable<T> All<T>() where T : class, new();
        Entry GetBySlug(string slug);
        List<Entry> GetList();
    }
}