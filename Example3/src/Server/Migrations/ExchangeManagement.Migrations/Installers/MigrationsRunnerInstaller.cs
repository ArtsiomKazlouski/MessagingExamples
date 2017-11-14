using System.Data;
using Autofac;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;

namespace ExchangeManagement.Migrations.Installers
{
    public class MigrationsRunnerInstaller : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new RunnerContext(new NullAnnouncer())
            {
                Database = "sqlserver",
                Connection = c.Resolve<IDbConnection>().ConnectionString,
                Targets = new[] { ThisAssembly.FullName }
            }).AsSelf();

            base.Load(builder);
        }
    }
}
