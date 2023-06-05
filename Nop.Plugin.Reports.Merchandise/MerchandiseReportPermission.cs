using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;

namespace Nop.Plugin.Reports.Merchandise
{
    public partial class MerchandiseReportPermission : IPermissionProvider
    {
        public static readonly PermissionRecord MerchandiseReport = new()
        {
            Name = "Admin area. Merchandise Report",
            SystemName = "Reports.Merchandise",
            Category = "Configuration"
        };
        /// <summary>
        /// Get permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
            MerchandiseReport
        };
        }
        /// <summary>
        /// Get default permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            return new() { (NopCustomerDefaults.AdministratorsRoleName, new[] { MerchandiseReport }) };
        }
    }
}
