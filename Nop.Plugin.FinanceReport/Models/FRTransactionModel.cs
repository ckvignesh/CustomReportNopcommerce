using Nop.Web.Framework.Models;
using System;

namespace Nop.Plugin.FinanceReport.Models
{

    public record FRTransactionModel : BaseNopModel
    {


        // Finance Reports

        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string PaymentMode { get; set; }
        public string Customer { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal OrderDiscount { get; set; }
        public decimal OrderTax { get; set; }
        public decimal OrderSubtotalExclTax { get; set; }
        public decimal OrderSubtotalInclTax { get; set; }
        public decimal OrderShippingExclTax { get; set; }
        public string CountryName { get; set; }

        public int RedeemedRewardPoints { get; set; }
        public decimal RedeemedRewardPointsAmount { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }


    }
}