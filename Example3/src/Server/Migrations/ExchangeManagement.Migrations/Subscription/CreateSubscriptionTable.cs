using FluentMigrator;
using FluentMigrator.Runner.Extensions;

namespace ExchangeManagement.Migrations.Subscription
{
    [Migration(20170522120733)]
    public class CreateSubscriptionTable : AutoReversingMigration
    {
        public static string TableName = "subscription";
        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("id").AsInt64().PrimaryKey().Identity(1, 1)
                .WithColumn("url").AsString().NotNullable()
                .WithColumn("query").AsString().NotNullable()
                .WithColumn("token_endpoint").AsString().NotNullable()
                .WithColumn("client_id").AsString().NotNullable()
                .WithColumn("client_secret").AsString().NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime)
                .WithColumn("updated_at").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime);

        }
    }
}
