using System.Collections.Generic;
using System.Linq;
using System.Web;
using NBlog.Web.Models;
using Newtonsoft.Json;
using System.IO;


namespace NBlog.Web.Application.Core
{
    public class BlogRepository<T> : IRepository<T> where T : class, IEntity
    {
        public BlogRepository()
        {

        }

        public string DataPath
        {
            get
            {
                return HttpContext.Current.Server.MapPath("~/App_Data/localhost");// + _tenantSelector.Name);
            }
        }

        public T Single<T>(object key) where T: class, new()
        {
            var filename = key.ToString();
            var recordPath = Path.Combine(DataPath, typeof(T).Name, filename + ".json");
            var json = File.ReadAllText(recordPath);
            var item = JsonConvert.DeserializeObject<T>(json);
            
            return item;
        }

        public IEnumerable<T> All<T>() where T: class, new()
        {
            var folderPath = Path.Combine(DataPath, typeof(T).Name);
            var filePaths = Directory.GetFiles(folderPath, "*.json", SearchOption.TopDirectoryOnly);

            var list = new List<T>();
            foreach (var path in filePaths)
            {
                var jsonString = File.ReadAllText(path);
                var entity = JsonConvert.DeserializeObject<T>(jsonString);
                list.Add(entity);
            }

            return list;
        }

        public Entry GetBySlug(string slug)
        {
            return Single<Entry>(slug);
        }

        public List<Entry> GetList()
        {
            return All<Entry>().ToList();
        }
    }
}