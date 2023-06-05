﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.Reports.Merchandise.Domain;
using Nop.Web.Framework.Models;
using static ClosedXML.Excel.XLPredefinedFormat;
using DateTime = System.DateTime;

namespace Nop.Plugin.Reports.Merchandise.Models
{

    public record TransactionModel : BaseNopModel
    {
        //Merchandise Reports

        public string ProductName { get; set; }
        public string SKU { get; set; }
        public int ProductID { get; set; }
        public int StockQuantity { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal OldPrice { get; set; }
        public decimal ProductCost { get; set; }
        public string DiscountName { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
        public string DiscountType { get; set; }
        public string PromotionStartDate { get; set; }
        public string PromotionEndDate { get; set; }
        public string Categories { get; set; }
        public string Brands { get; set; }
        public string VendorName { get; set; }
        public string ProductLaunchDate { get; set; }
        public string ProductStatus { get; set; }
        public string InventoryMethod { get; set; }

    }
}