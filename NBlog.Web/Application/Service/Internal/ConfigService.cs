using System.Web;
using NBlog.Web.Application.Service.Entity;
using NBlog.Web.Application.Storage;

namespace NBlog.Web.Application.Service.Internal
{
    public class ConfigService : IConfigService
    {
        private readonly IRepository _repository;

        public ConfigService(IRepository repository)
        {
            /*
             * mongodb connection
            var repositoryKeys = new RepositoryKeys();
            repositoryKeys.Add<Entry>(e => e.Slug);
            repositoryKeys.Add<Config>(c => c.Site);
            repositoryKeys.Add<User>(u => u.Username);
            NBlog.Web.Application.Storage.Mongo.MongoRepository mr = new Storage.Mongo.MongoRepository(repositoryKeys, "mongodb://username:password@mongoserver/dbname", "dbname");
            repository = mr;
            */
            _repository = repository;
        }

        public Config Current { get { return _repository.Single<Config>("settings"); } }
    }
}