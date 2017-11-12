using FluentMigrator;

namespace ExchangeManagement.Migrations.Subscription
{
    [Migration(20171102175200)]
    public class AddActiveField : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table(CreateSubscriptionTable.TableName)
                .AddColumn("active").AsBoolean().NotNullable().SetExistingRowsTo(1);
        }
    }
}