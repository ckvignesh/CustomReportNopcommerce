using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.RefundReport.Domain;
using Nop.Services.ExportImport.Help;

namespace Nop.Plugin.RefundReport.Services
{
    public class RefundExportManager : IRefundExportManager
    {
        private readonly CatalogSettings _catalogSettings;

        #region Ctor
        public RefundExportManager(CatalogSettings catalogSettings)
        {
            _catalogSettings = catalogSettings;
        }
        #endregion
        public virtual async Task<byte[]> ExportFinanceReportToXlsxAsync(IList<RefundReportTransactionlog> report)
        {

            //property array
            var properties = new[]
            {
                new PropertyByName<RefundReportTransactionlog>("Order Date",r => r.OrderDate),
                new PropertyByName<RefundReportTransactionlog>("Order Number",r => r.OrderNumber),
                new PropertyByName<RefundReportTransactionlog>("Customer",r => r.Customer),
                new PropertyByName<RefundReportTransactionlog>("Payment Status",r => r.PaymentStatus),
                new PropertyByName<RefundReportTransactionlog>("Refund Date",r => r.RefundedDate),
                new PropertyByName<RefundReportTransactionlog>("Refunded Amount Excl.GST",r => r.RefundedAmountExclTax),
                new PropertyByName<RefundReportTransactionlog>("GST",r => r.GST),
                new PropertyByName<RefundReportTransactionlog>("Refunded Amount",r => r.RefundedAmount),


            };

            return await new PropertyManager<RefundReportTransactionlog>(properties, _catalogSettings).ExportToXlsxAsync(report);

        }
    }
}
