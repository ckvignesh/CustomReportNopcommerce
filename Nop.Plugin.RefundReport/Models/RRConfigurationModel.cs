using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.RefundReport.Models
{
    public record RRConfigurationModel : BaseSearchModel
    {
        public string OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string Customer { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMode { get; set; }
        public decimal GST { get; set; }
        public decimal RefundedAmount { get; set; }
        public decimal RefundedAmountExclTax { get; set; }

        public string RefundedDate { get; set; }
        [UIHint("DateNullable")]
        public DateTime? FromDate { get; set; }
        [UIHint("DateNullable")]
        public DateTime? ToDate { get; set; }
        [UIHint("DateNullable")]
        public DateTime? OToDate { get; set; }
        [UIHint("DateNullable")]
        public DateTime? OFromDate { get; set; }
    }
}
