using System;
using System.Linq;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Initialization.AssemblyLoader;
using FluentMigrator.Runner.Processors;

namespace ExchangeManagement.Migrations.Installers
{
    public class CustomTaskExecutor : TaskExecutor
    {
        public CustomTaskExecutor(IRunnerContext runnerContext) : base(runnerContext)
        {
        }

        public CustomTaskExecutor(IRunnerContext runnerContext, AssemblyLoaderFactory assemblyLoaderFactory, MigrationProcessorFactoryProvider processorFactoryProvider) : base(runnerContext, assemblyLoaderFactory, processorFactoryProvider)
        {
        }

        public void CheskForNotAppliedMigrations()
        {
            this.Initialize();
            try
            {
                var versionLoader = new VersionLoader(this.Runner, this.Runner.MigrationAssemblies, ((MigrationRunner)this.Runner).Conventions);
                versionLoader.LoadVersionInfo();

                var migrationsToRun = (from m in this.Runner.MigrationLoader.LoadMigrations()
                                       where !versionLoader.VersionInfo.HasAppliedMigration(m.Key)
                                       select m).ToList();

                if (migrationsToRun.Any())
                {
                    var text = $"Необходимо выполнить миграции:{Environment.NewLine}";
                    text = migrationsToRun.Aggregate(text, (current, keyValuePair) => current + $"{keyValuePair.Value.GetName()}{Environment.NewLine}");
                    throw new Exception(text);

                }
            }
            finally
            {
                this.Runner.Processor.Dispose();
            }
        }
    }
}
