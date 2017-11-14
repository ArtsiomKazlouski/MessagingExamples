using System;
using FluentMigrator;
using FluentMigrator.Runner.Extensions;

namespace ExchangeManagement.Migrations.Subscription
{
    [Migration(20170911150800)]
    public class DeleteTable : ForwardOnlyMigration
    {
        public override void Up()
        {
            Delete.Table(CreateSubscriptionTable.TableName);
        }
    }

    [Migration(20170912092800)]
    public class RecreateTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table(CreateSubscriptionTable.TableName)
                .WithColumn("id").AsInt64().PrimaryKey().Identity(1, 1)
                .WithColumn("url").AsString().NotNullable()
                .WithColumn("query").AsString(Int32.MaxValue).NotNullable()
                .WithColumn("token_endpoint").AsString().NotNullable()
                .WithColumn("client_id").AsString().NotNullable()
                .WithColumn("client_secret").AsString().NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime)
                .WithColumn("updated_at").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime)
                .WithColumn("download_resource_file").AsBoolean().NotNullable().WithDefaultValue(0);
        }
    }
}