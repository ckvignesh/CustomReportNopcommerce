using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.RefundReport.Models
{
    public record ReturnModel : BaseNopModel
    {
        public IFormCollection Form { get; set; }
    }
}
