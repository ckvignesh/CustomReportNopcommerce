using System.Threading.Tasks;
using DocumentFormat.OpenXml.Bibliography;
using Nop.Core;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Reports.Merchandise.Domain;

namespace Nop.Plugin.Reports.Merchandise.Services
{
    /// <summary>
    /// Represents service shipping by weight service
    /// </summary>
    public partial interface IReportsMerchandiseTransactionService
    {
        Task<IPagedList<MerchandiseTransactionLog>> GetTransactionLogAsync(int pageIndex = 0, int pageSize = int.MaxValue, string productName = null, string sKU = null, string categories = null, string brands = null, string vendorName = null, string productLaunchDate = null);

        MerchandiseTransactionLog GetByReference(string refNo);

        Task InsertTransactionAsync(MerchandiseTransactionLog transactionLog);       

        Task UpdateTransactionAsync(MerchandiseTransactionLog transactionLog);
    }
}
