using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.RefundReport.Models
{
    public record RRTransactionModel : BaseNopModel
    {
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string Customer { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMode { get; set; }
        public decimal GST { get; set; }
        public decimal RefundedAmount { get; set; }
        public decimal RefundedAmountExclTax { get; set; }
    }
}
