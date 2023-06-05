using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.RefundReport.Domain;
using Nop.Plugin.RefundReport.Services;

namespace Nop.Plugin.RefundReport.Services
{
    public partial class RefundReportTransactionService : IRefundReportTransactionService
    {
        #region Fields

        private readonly IRepository<RefundReportTransactionlog> _refundRepository;
        private readonly INopDataProvider _dataProvider;

        #endregion

        #region Ctor

        public RefundReportTransactionService(INopDataProvider dataProvider, IRepository<RefundReportTransactionlog> enetsRepository)
        {
            _refundRepository = enetsRepository;
            _dataProvider = dataProvider;
        }

        #endregion

        #region Methods

        public virtual async Task<IPagedList<RefundReportTransactionlog>> GetTransactionLogAsync(int pageIndex = 0, int pageSize = int.MaxValue, string orderNumber = null, string customer = null, string paymentStatus = null, string orderStatus = null, string paymentMode = null)
        {
            //var res = (await GetAllAsync(merchantTxnRef: merchantTxnRef, orderId: orderId)).ToList();
            //var res = await _dataProvider.QueryAsync<ENETSTransactionLog>("SELECT [OrderNumber], [OrderDate], [Customer], [OrderStatus], [PaymentStatus], [OrderTotal], [OrderDiscount], [ShippingAddressId], [OrderShippingExclTax], [OrderTax], [OrderSubtotalExclTax], [OrderSubtotalInclTax], [PaymentMode], [CountryName] FROM [dbo].[Finance_Report]\r\n", null);
            //            var records = new PagedList<ENETSTransactionLog>(res, pageIndex, pageSize);

            var sqlQuery = "SELECT * FROM [dbo].[Refund_Report] WHERE 1=1";
            if (!string.IsNullOrEmpty(orderNumber))
                sqlQuery = sqlQuery + " AND OrderNumber like '%" + orderNumber + "%'";
            if (!string.IsNullOrEmpty(customer))
                sqlQuery = sqlQuery + " AND Customer like '%" + customer + "%'";
            if (!string.IsNullOrEmpty(paymentStatus))
                sqlQuery = sqlQuery + " AND PaymentStatus like '%" + paymentStatus + "%'";
            //if (!string.IsNullOrEmpty(orderStatus))
            //    sqlQuery = sqlQuery + " AND OrderStatus like '%" + orderStatus + "%'";
            if (!string.IsNullOrEmpty(paymentMode))
                sqlQuery = sqlQuery + " AND PaymentMode like '%" + paymentMode + "%'";

            var res = await _dataProvider.QueryAsync<RefundReportTransactionlog>(sqlQuery, null);

            var records = new PagedList<RefundReportTransactionlog>(res, pageIndex, pageSize);

            return records;
        }


        public virtual async Task<IList<RefundReportTransactionlog>> GetAllAsync(string merchantTxnRef = null, string orderId = null)
        {
            var res = await _refundRepository.GetAllAsync(query =>
            {
                
                return from data in query
                       orderby data.Id descending
                       select data;
            });

            return res; //records;
        }

        public virtual async Task InsertTransactionAsync(RefundReportTransactionlog transactionLog)
        {
            await _refundRepository.InsertAsync(transactionLog, false);
        }

        public virtual async Task UpdateTransactionAsync(RefundReportTransactionlog transactionLog)
        {
            await _refundRepository.UpdateAsync(transactionLog, false);
        }


        public virtual RefundReportTransactionlog GetByReference(string refNo)
        {
            var res = _refundRepository.Table.FirstOrDefault();

            return res;
        }

        #endregion
    }
}
