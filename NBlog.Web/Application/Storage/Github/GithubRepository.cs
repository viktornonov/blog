using NBlog.Web.Application.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace NBlog.Web.Application.Storage.Github
{
    public class GithubRepository
    {
        private readonly RepositoryKeys _keys;

        public string DataPath
        {
            get
            {
                return "https://raw.githubusercontent.com/viktornonov/blog/master/NBlog.Web/App_Data/localhost/Entry/";
            }
        }

        public GithubRepository(RepositoryKeys keys)
        {
            _keys = keys;
        }

        public TEntity Single<TEntity>(object key) where TEntity : class, new()
        {
            var filename = key.ToString();
            var location = filename;
            var recordPath = Path.Combine(DataPath, typeof(TEntity).Name, location, filename + ".json");
            var json = File.ReadAllText(recordPath);
            var item = JsonConvert.DeserializeObject<TEntity>(json);

            return item;
        }

        public IEnumerable<TEntity> All<TEntity>() where TEntity : class, new()
        {
            var folderPath = Path.Combine(DataPath, typeof(TEntity).Name);
            var filePaths = Directory.GetFiles(folderPath, "*/*.json", SearchOption.TopDirectoryOnly);

            var list = new List<TEntity>();
            foreach (var path in filePaths)
            {
                var jsonString = File.ReadAllText(path);
                var entity = JsonConvert.DeserializeObject<TEntity>(jsonString);
                list.Add(entity);
            }

            return list;
        }


        public void Save<TEntity>(TEntity item) where TEntity : class, new()
        {
            var json = JsonConvert.SerializeObject(item, Formatting.Indented);
            var folderPath = GetEntityPath<TEntity>();
            if (!Directory.Exists(folderPath)) { Directory.CreateDirectory(folderPath); }

            var filename = _keys.GetKeyValue(item);
            var recordPath = Path.Combine(folderPath, filename + ".json");

            File.WriteAllText(recordPath, json);
        }


        public bool Exists<TEntity>(object key) where TEntity : class, new()
        {
            var folderPath = GetEntityPath<TEntity>();
            var recordPath = Path.Combine(folderPath, key + ".json");
            return File.Exists(recordPath);
        }

        // todo: not in IRepository? should be?
        public void DeleteAll<TEntity>()
        {
            var folderPath = GetEntityPath<TEntity>();
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }
        }


        public void Delete<TEntity>(object key) where TEntity : class, new()
        {
            var folderPath = GetEntityPath<TEntity>();
            var recordPath = Path.Combine(folderPath, key + ".json");
            File.Delete(recordPath);
        }


        // todo: need a TEntity + TKey version of this too that does Path.Combine(folderPath, key + ".json");
        private string GetEntityPath<TEntity>()
        {
            return Path.Combine(DataPath, typeof(TEntity).Name);
        }
    }
}