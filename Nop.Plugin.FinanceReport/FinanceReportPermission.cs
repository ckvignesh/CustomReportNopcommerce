using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;

namespace Nop.Plugin.FinanceReport
{
    public partial class FinanceReportPermission :IPermissionProvider
    {
        public static readonly PermissionRecord FinanceReport = new()
        {
            Name = "Admin area. Finance Report",
            SystemName = "FinanceReport",
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
            FinanceReport
        };
        }
        /// <summary>
        /// Get default permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            return new() { (NopCustomerDefaults.AdministratorsRoleName, new[] { FinanceReport }) };
        }
    }
}
