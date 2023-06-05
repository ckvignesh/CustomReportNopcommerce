using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Reports.Merchandise.Domain;

namespace Nop.Plugin.Reports.Merchandise.Services
{
    public interface IMerchandiseReport
    {
        Task<byte[]> ExportMerchadiseReportToXlsxAsync(IEnumerable<MerchandiseTransactionLog> report);
    }
}
