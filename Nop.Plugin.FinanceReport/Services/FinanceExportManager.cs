using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.FinanceReport.Domain;
using Nop.Services.ExportImport.Help;
using Services;

namespace Nop.Plugin.FinanceReport.Services
{
    public class FinanceExportManager : IFinanceExportManager
    {
        private readonly CatalogSettings _catalogSettings;

        #region Ctor
        public FinanceExportManager(CatalogSettings catalogSettings)
        {
            _catalogSettings = catalogSettings;
        }
        #endregion
        public virtual async Task<byte[]> ExportFinanceReportToXlsxAsync(IList<FinanceReportransactionLog> report)
        {

            //property array
            //var manager = new PropertyManager(new[]
            var properties = new[]
            {
                new PropertyByName<FinanceReportransactionLog>("Receipt Date (UTC)", p => p.OrderDate),
                new PropertyByName<FinanceReportransactionLog>("Receipt Number", p => p.OrderNumber),
                new PropertyByName<FinanceReportransactionLog>("Order Status", p => p.OrderStatus),
                new PropertyByName<FinanceReportransactionLog>("Payment Status", p => p.PaymentStatus),
                new PropertyByName<FinanceReportransactionLog>("Customer Name", p => p.Customer),
                new PropertyByName<FinanceReportransactionLog>("Shipping City", p => p.CountryName),
                new PropertyByName<FinanceReportransactionLog>("Amt b4 disc excl. GST", p => p.OrderSubtotalExclTax),
                new PropertyByName<FinanceReportransactionLog>("Disc excl. GST", p => p.OrderDiscount),
                new PropertyByName<FinanceReportransactionLog>("Amt after disc excl. GST", p => p.OrderSubtotalInclTax),
                new PropertyByName<FinanceReportransactionLog>("Delivery Charge excl. GST", p => p.OrderShippingExclTax),

                new PropertyByName<FinanceReportransactionLog>("GST", p => p.OrderTax),
                new PropertyByName<FinanceReportransactionLog>("Redeemed Reward Points Amount", p => p.RedeemedRewardPointsAmount),

                new PropertyByName<FinanceReportransactionLog>("Total incl. GST", p => p.OrderTotal),
                new PropertyByName<FinanceReportransactionLog>("Payment Mode", p => p.PaymentMode),
                new PropertyByName<FinanceReportransactionLog>("Redeemed Reward Points", p => p.RedeemedRewardPoints),
            };

            return await new PropertyManager<FinanceReportransactionLog>(properties, _catalogSettings).ExportToXlsxAsync(report);

        }
    }
}
