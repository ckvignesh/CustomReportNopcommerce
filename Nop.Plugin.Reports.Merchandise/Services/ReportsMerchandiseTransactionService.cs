using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LinqToDB.DataProvider;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Plugin.Reports.Merchandise.Domain;

namespace Nop.Plugin.Reports.Merchandise.Services
{
    /// <summary>
    /// Represents service shipping by weight service implementation
    /// </summary>
    public partial class ReportsMerchandiseTransactionService : IReportsMerchandiseTransactionService
    {

        #region Fields

        private readonly IRepository<MerchandiseTransactionLog> _enetsRepository;
        private readonly INopDataProvider _dataProvider;

        #endregion

        #region Ctor

        public ReportsMerchandiseTransactionService(INopDataProvider dataProvider, IRepository<MerchandiseTransactionLog> enetsRepository)
        {
            _enetsRepository = enetsRepository;
            _dataProvider = dataProvider;
        }

        #endregion

        #region Methods

        public virtual async Task<IPagedList<MerchandiseTransactionLog>> GetTransactionLogAsync(int pageIndex = 0, int pageSize = int.MaxValue, string productName = null, string sKU = null, string categories=null, string brands = null, string vendorName = null, string productLaunchDate = null)
        {
            //var res = (await GetAllAsync(merchantTxnRef: merchantTxnRef, orderId: orderId)).ToList();
            //var res = await _dataProvider.QueryAsync<ENETSTransactionLog>("SELECT * FROM [dbo].[Merchandise_Report]", null);
            //            var records = new PagedList<ENETSTransactionLog>(res, pageIndex, pageSize);
            //ProductLaunchDate
            // return records;
            var sqlQuery = "SELECT * FROM [dbo].[Merchandise_Report] WHERE 1=1";
            if (!string.IsNullOrEmpty(sKU))
                sqlQuery = sqlQuery + " AND SKU like '%" + sKU + "%'";
            if (!string.IsNullOrEmpty(productName))
                ////productName = Regex.Replace(productName, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
                /////ProductName = '"+ productName +"' or ProductName LIKE '% "+productName+"%'
                //sqlQuery = sqlQuery + " AND ProductName like '%" + productName + "%'";
                sqlQuery = sqlQuery + " AND ProductName = '" + productName.Replace("'", "''") + "' or ProductName LIKE '% " + productName.Replace("'", "''") + "%'";
            if (!string.IsNullOrEmpty(categories))
                sqlQuery = sqlQuery + " AND Categories like '%" + categories + "%'";
            if (!string.IsNullOrEmpty(brands))
                sqlQuery = sqlQuery + " AND Brands like '%" + brands + "%'";
            if (!string.IsNullOrEmpty(vendorName))
                sqlQuery = sqlQuery + " AND VendorName like '%" + vendorName + "%'";
            if (!string.IsNullOrEmpty(productLaunchDate))
                sqlQuery = sqlQuery + " AND ProductLaunchDate like '%" + productLaunchDate + "%'";
            //if (!string.IsNullOrEmpty(orderStatus))
                //    sqlQuery = sqlQuery + " AND OrderStatus like '%" + orderStatus + "%'";
                //if (!string.IsNullOrEmpty(paymentMode))
                //    sqlQuery = sqlQuery + " AND PaymentMode like '%" + paymentMode + "%'";

                var res = await _dataProvider.QueryAsync<MerchandiseTransactionLog>(sqlQuery, null);

            var records = new PagedList<MerchandiseTransactionLog>(res, pageIndex, pageSize);

            return records;
        }

        public virtual async Task<IList<MerchandiseTransactionLog>> GetAllAsync(string merchantTxnRef = null, string orderId = null)
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

        public virtual async Task InsertTransactionAsync(MerchandiseTransactionLog transactionLog)
        {
            await _enetsRepository.InsertAsync(transactionLog, false);
        }

        public virtual async Task UpdateTransactionAsync(MerchandiseTransactionLog transactionLog)
        {
            await _enetsRepository.UpdateAsync(transactionLog, false);
        }


        public virtual MerchandiseTransactionLog GetByReference(string refNo)
        {
            var res = _enetsRepository.Table.FirstOrDefault();

            return res;
        }

        #endregion
    }
}
