using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.FinanceReport.Domain;

namespace Services
{
    public interface IFinanceExportManager
    {
        Task<byte[]> ExportFinanceReportToXlsxAsync(IList<FinanceReportransactionLog> report);
    }
}
