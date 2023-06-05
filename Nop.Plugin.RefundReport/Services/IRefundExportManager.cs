using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.RefundReport.Domain;

namespace Nop.Plugin.RefundReport.Services
{
    public interface IRefundExportManager
    {
        Task<byte[]> ExportFinanceReportToXlsxAsync(IList<RefundReportTransactionlog> report);

    }
}
