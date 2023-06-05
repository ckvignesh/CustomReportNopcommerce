using LinqToDB.Common;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.FinanceReport.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.FinanceReport.Services
{
    /// <summary>
    /// Represents service shipping by weight service implementation
    /// </summary>
    public partial class FinanceReportTransactionService : IFinanceReportTransactionService
    {

        #region Fields

        private readonly IRepository<FinanceReportransactionLog> _enetsRepository;
        private readonly INopDataProvider _dataProvider;

        #endregion

        #region Ctor

        public FinanceReportTransactionService(INopDataProvider dataProvider, IRepository<FinanceReportransactionLog> enetsRepository)
        {
            _enetsRepository = enetsRepository;
            _dataProvider = dataProvider;
        }

        #endregion

        #region Methods

        public virtual async Task<IPagedList<FinanceReportransactionLog>> GetTransactionLogAsync(int pageIndex = 0, int pageSize = int.MaxValue, string orderNumber = null, string customer = null, string paymentStatus = null, string orderStatus = null, string paymentMode = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            //var res = (await GetAllAsync(merchantTxnRef: merchantTxnRef, orderId: orderId)).ToList();
            //var res = await _dataProvider.QueryAsync<ENETSTransactionLog>("SELECT [OrderNumber], [OrderDate], [Customer], [OrderStatus], [PaymentStatus], [OrderTotal], [OrderDiscount], [ShippingAddressId], [OrderShippingExclTax], [OrderTax], [OrderSubtotalExclTax], [OrderSubtotalInclTax], [PaymentMode], [CountryName] FROM [dbo].[Finance_Report]\r\n", null);
            //            var records = new PagedList<ENETSTransactionLog>(res, pageIndex, pageSize);

            var sqlQuery = "SELECT * FROM [dbo].[Finance_Report] WHERE 1=1";
            if (!string.IsNullOrEmpty(orderNumber))
                sqlQuery = sqlQuery + " AND OrderNumber like '%" + orderNumber + "%'";
            if (!string.IsNullOrEmpty(customer))
                sqlQuery = sqlQuery + " AND Customer like '%" + customer + "%'";
            if (!string.IsNullOrEmpty(paymentStatus))
                sqlQuery = sqlQuery + " AND PaymentStatus like '%" + paymentStatus + "%'";
            if (!string.IsNullOrEmpty(orderStatus))
                sqlQuery = sqlQuery + " AND OrderStatus like '%" + orderStatus + "%'";
            if (!string.IsNullOrEmpty(paymentMode))
                sqlQuery = sqlQuery + " AND PaymentMode like '%" + paymentMode + "%'";
            // has both SD ED
            if ((!string.IsNullOrEmpty(fromDate.ToString())) && (!string.IsNullOrEmpty(toDate.ToString())))
                sqlQuery = sqlQuery + " AND OrderDate BETWEEN CONVERT(datetime, '" + fromDate + "', 103) AND CONVERT(datetime, '" + toDate+ "', 103)";
            // has only SD
            if ((string.IsNullOrEmpty(fromDate.ToString())) && (!string.IsNullOrEmpty(toDate.ToString())))
                sqlQuery = sqlQuery + " AND OrderDate <= CONVERT(datetime, '" + toDate + "', 103)";
            // has only ED
            if ((!string.IsNullOrEmpty(fromDate.ToString())) && (string.IsNullOrEmpty(toDate.ToString())))
                sqlQuery = sqlQuery + " AND OrderDate >= CONVERT(datetime, '" + fromDate + "', 103)";


            var res = await _dataProvider.QueryAsync<FinanceReportransactionLog>(sqlQuery, null);

            var records = new PagedList<FinanceReportransactionLog>(res, pageIndex, pageSize);

            return records;
        }

        
        public virtual async Task<IList<FinanceReportransactionLog>> GetAllAsync(string merchantTxnRef = null, string orderId = null)
        {
            var res = await _enetsRepository.GetAllAsync(query =>
            {
                //if (!string.IsNullOrEmpty(merchantTxnRef))
                //    query = query.Where(c => c.MerchantTxnRef.Contains(merchantTxnRef));
                //if (!string.IsNullOrEmpty(orderId))
                //{
                //    var isNumeric = int.TryParse(orderId, out int n);
                //    if (isNumeric)
                //        query = query.Where(c => c.OrderId.Equals(Convert.ToInt32(orderId)));
                //}

                return from data in query
                       orderby data.Id descending
                       select data;
            });

            //var records = new PagedList<ENETSTransactionLog>(res, pageIndex, pageSize);

            return res; //records;
        }

        public virtual async Task InsertTransactionAsync(FinanceReportransactionLog transactionLog)
        {
            await _enetsRepository.InsertAsync(transactionLog, false);
        }

        public virtual async Task UpdateTransactionAsync(FinanceReportransactionLog transactionLog)
        {
            await _enetsRepository.UpdateAsync(transactionLog, false);
        }


        public virtual FinanceReportransactionLog GetByReference(string refNo)
        {
            var res = _enetsRepository.Table.FirstOrDefault();

            return res;
        }

        #endregion
    }
}
