using System;
using Nop.Core;
using Nop.Plugin.Reports.Merchandise.Domain;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Reports.Merchandise.Models
{
    public record TransactionListModel : BasePagedListModel<TransactionModel>
    {

    }
}