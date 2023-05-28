using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Reports.Merchandise.Domain;
using Nop.Services.ExportImport.Help;

namespace Nop.Plugin.Reports.Merchandise.Services
{
    public class MerchandiseReport : IMerchandiseReport
    {
        private readonly CatalogSettings _catalogSettings;

        #region Ctor
        public MerchandiseReport(CatalogSettings catalogSettings)
        {
            _catalogSettings = catalogSettings;
        }
        #endregion
        public virtual async Task<byte[]> ExportMerchadiseReportToXlsxAsync(IEnumerable<MerchandiseTransactionLog> report)
        {

            //property array
            //var manager = new PropertyManager(new[]
            var properties = new[]
            {
                new PropertyByName<MerchandiseTransactionLog>("Product Code",p => p.ProductID),
                new PropertyByName<MerchandiseTransactionLog>("Product Name",p => p.ProductName),
                new PropertyByName<MerchandiseTransactionLog>("Product Status",p => p.ProductStatus),
                new PropertyByName<MerchandiseTransactionLog>("Inventory",p => p.StockQuantity),
                new PropertyByName<MerchandiseTransactionLog>("Vendor",p => p.VendorName),
                new PropertyByName<MerchandiseTransactionLog>("Brand",p => p.Brands),
                new PropertyByName<MerchandiseTransactionLog>("Category",p => p.Categories),
                new PropertyByName<MerchandiseTransactionLog>("Retail Price",p => p.RetailPrice),
                new PropertyByName<MerchandiseTransactionLog>("Product Cost",p => p.ProductCost),
                new PropertyByName<MerchandiseTransactionLog>("Product Launch Date",p => p.ProductLaunchDate),
                new PropertyByName<MerchandiseTransactionLog>("Product Type",p => p.InventoryMethod),
                new PropertyByName<MerchandiseTransactionLog>("Promotion Price",p => p.DiscountAmount),
                new PropertyByName<MerchandiseTransactionLog>("Promotion Start Date",p => p.PromotionStartDate),
                new PropertyByName<MerchandiseTransactionLog>("Promotion End Date",p => p.PromotionStartDate),

            };
            return await new PropertyManager<MerchandiseTransactionLog>(properties, _catalogSettings).ExportToXlsxAsync(report);

        }
    }
}
