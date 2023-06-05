using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;

namespace Nop.Plugin.RefundReport
{
    public partial class RefundReportPermission : IPermissionProvider
    {
        public static readonly PermissionRecord RefundReport = new()
        {
            Name = "Admin area. Refund Report",
            SystemName = "RefundReport",
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
            RefundReport
        };
        }
        /// <summary>
        /// Get default permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
        {
            return new() { (NopCustomerDefaults.AdministratorsRoleName, new[] { RefundReport }) };
        }
    }
}
