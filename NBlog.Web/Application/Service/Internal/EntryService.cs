using System;
using System.Collections.Generic;
using System.Linq;
using NBlog.Web.Application.Storage;
using NBlog.Web.Models;

namespace NBlog.Web.Application.Service.Internal
{
    public class EntryService : IEntryService
    {
        private readonly IRepository _repository;

        public EntryService(IRepository repository)
        {
            _repository = repository;
        }

        public Entry GetBySlug(string slug)
        {
            return _repository.Single<Entry>(slug);
        }

        public List<Entry> GetList()
        {
            return _repository.All<Entry>().ToList();
        }

        public bool Exists(string slug)
        {
            return _repository.Exists<Entry>(slug);
        }
    }
}