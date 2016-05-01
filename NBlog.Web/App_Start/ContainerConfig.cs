using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;
using NBlog.Web.Application.Service.Entity;
using NBlog.Web.Application.Service.Internal;
using NBlog.Web.Application.Storage;
using NBlog.Web.Application.Storage.Json;
using Quartz;
using Quartz.Impl;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace NBlog.Web
{
    public class ContainerConfig
    {
        private const string DefaultRepositoryName = "json";

        public static void SetUpContainer()
        {
            var container = RegisterDependencies();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // replace the default FilterAttributeFilterProvider with one that has Autofac property
            // injection
            FilterProviders.Providers.Remove(FilterProviders.Providers.Single(f => f is FilterAttributeFilterProvider));

            //InitialiseJobScheduler(container);
        }

        private static ResolvedParameter GetResolvedParameterByName<T>(string key)
        {
            return new ResolvedParameter(
                (pi, c) => pi.ParameterType == typeof(T),
                (pi, c) => c.ResolveNamed<T>(key));
        }

        private static IContainer RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            var repositoryKeys = new RepositoryKeys();
            repositoryKeys.Add<Entry>(e => e.Slug);
            repositoryKeys.Add<Config>(c => c.Site);
            repositoryKeys.Add<User>(u => u.Username);

            builder.RegisterType<JsonRepository>().Named<IRepository>("json").InstancePerLifetimeScope().WithParameters(new[] {
                new NamedParameter("keys", repositoryKeys)
            });

            builder.RegisterControllers(typeof(ContainerConfig).Assembly)
                .WithParameter(GetResolvedParameterByName<IRepository>(DefaultRepositoryName));

            builder.RegisterType<ConfigService>().As<IConfigService>().InstancePerLifetimeScope()
                .WithParameter(GetResolvedParameterByName<IRepository>(DefaultRepositoryName));

            builder.RegisterType<EntryService>().As<IEntryService>().InstancePerLifetimeScope()
                .WithParameter(GetResolvedParameterByName<IRepository>(DefaultRepositoryName));

            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<MessageService>().As<IMessageService>().InstancePerLifetimeScope();
            builder.RegisterType<Services>().As<IServices>().InstancePerLifetimeScope();

            var container = builder.Build();
            return container;
        }
    }
}