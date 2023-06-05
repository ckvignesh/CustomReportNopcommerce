using Nop.Core;
using Nop.Plugin.FinanceReport.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.FinanceReport.Services
{
    /// <summary>
    /// Represents service shipping by weight service
    /// </summary>
    public partial interface IFinanceReportTransactionService
    {
        Task<IPagedList<FinanceReportransactionLog>> GetTransactionLogAsync(int pageIndex = 0, int pageSize = int.MaxValue, string orderNumber = null, string customer = null, string paymentStatus = null, string orderStatus = null, string paymentMode = null, DateTime? fromDate = null, DateTime? toDate = null);

        Task<IList<FinanceReportransactionLog>> GetAllAsync(string merchantTxnRef = null, string orderId = null);
        FinanceReportransactionLog GetByReference(string refNo);

        Task InsertTransactionAsync(FinanceReportransactionLog transactionLog);

        Task UpdateTransactionAsync(FinanceReportransactionLog transactionLog);
    }
}
