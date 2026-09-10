using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace BookStoreApi.Data.NHibernate;

public class NHibernateSessionFactory
{
    private readonly ISessionFactory _sessionFactory;

    public NHibernateSessionFactory(IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection");

        var fluentConfiguration = Fluently.Configure()
            .Database(
                MsSqlConfiguration.MsSql2012
                    .ConnectionString(connectionString)
                    .Driver<global::NHibernate.Driver.MicrosoftDataSqlClientDriver>())
            .Mappings(m =>
                m.FluentMappings.AddFromAssemblyOf<BookMap>());

        _sessionFactory =
            fluentConfiguration.BuildSessionFactory();
    }

    public global::NHibernate.ISession OpenSession()
    {
        return _sessionFactory.OpenSession();
    }
}