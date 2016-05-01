using System;
using System.Collections.Generic;
using System.Linq;
using NBlog.Web.Application.Service.Entity;
using NBlog.Web.Application.Storage;

namespace NBlog.Web.Application.Service.Internal
{
    public class EntryService : IEntryService
    {
        private readonly IRepository _repository;

        public EntryService(IRepository repository)
        {
            _repository = repository;
        }

        public void Save(Entry entry)
        {
            if (string.IsNullOrWhiteSpace(entry.Slug))
                throw new ArgumentNullException("entry", "Entry must have a Slug value to Save()");

            entry.Slug = entry.Slug.ToLowerInvariant();

            if (entry.DateCreated == default(DateTime))
            {
                var isUpdate = _repository.Exists<Entry>(entry.Slug);
                if (isUpdate)
                {
                    var oldEntry = _repository.Single<Entry>(entry.Slug);
                    entry.DateCreated = oldEntry.DateCreated;                    
                }
                else
                {
                    entry.DateCreated = DateTime.Now;
                }
            }

            _repository.Save(entry);
        }

        public Entry GetBySlug(string slug)
        {
            return _repository.Single<Entry>(slug);
        }

        public List<Entry> GetList()
        {
            return _repository.All<Entry>().ToList();
        }

        public void Delete(string slug)
        {
            _repository.Delete<Entry>(slug);
        }

        public bool Exists(string slug)
        {
            return _repository.Exists<Entry>(slug);
        }
    }
}