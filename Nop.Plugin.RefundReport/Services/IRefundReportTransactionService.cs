using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.RefundReport.Domain;

namespace Nop.Plugin.RefundReport.Services
{
    public partial interface IRefundReportTransactionService
    {
        Task<IPagedList<RefundReportTransactionlog>> GetTransactionLogAsync(int pageIndex = 0, int pageSize = int.MaxValue, string orderNumber = null, string customer = null, string paymentStatus = null, string orderStatus = null, string paymentMode = null, DateTime? fromDate = null, DateTime? toDate = null, DateTime? oFromDate = null, DateTime? oToDate = null);

        Task<IList<RefundReportTransactionlog>> GetAllAsync(string merchantTxnRef = null, string orderId = null);
        RefundReportTransactionlog GetByReference(string refNo);

        Task InsertTransactionAsync(RefundReportTransactionlog transactionLog);

        Task UpdateTransactionAsync(RefundReportTransactionlog transactionLog);
    }
}
