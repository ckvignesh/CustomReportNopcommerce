using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.FinanceReport.Domain;

namespace Nop.Plugin.FinanceReport.Data
{
    public class FinanceReportTransactionLogBuilder : NopEntityBuilder<FinanceReportransactionLog>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            //table
            //    .WithColumn(nameof(ENETSTransactionLog.OrderId))
            //    .AsInt32()
            //    .WithColumn(nameof(ENETSTransactionLog.OrderAmount))
            //    .AsDecimal(18, 2)
            //    .WithColumn(nameof(ENETSTransactionLog.TransactionDate))
            //    .AsDateTime()
            //    .WithColumn(nameof(ENETSTransactionLog.MerchantTxnRef))
            //    .AsString(200)
            //    .WithColumn(nameof(ENETSTransactionLog.RequestPayload))
            //    .AsString(int.MaxValue).Nullable()
            //    .WithColumn(nameof(ENETSTransactionLog.RequestHmac))
            //    .AsString(int.MaxValue).Nullable()
            //    .WithColumn(nameof(ENETSTransactionLog.PaymentMode))
            //    .AsString(100).Nullable()
            //    .WithColumn(nameof(ENETSTransactionLog.ResponseText))
            //    .AsString(int.MaxValue).Nullable()
            //    .WithColumn(nameof(ENETSTransactionLog.ResponseHmac))
            //    .AsString(int.MaxValue).Nullable()
            //    .WithColumn(nameof(ENETSTransactionLog.TransactionStatus))
            //    .AsString(100).Nullable()
            //    .WithColumn(nameof(ENETSTransactionLog.TransactionMsg))
            //    .AsString(500).Nullable();
        }
    }
}