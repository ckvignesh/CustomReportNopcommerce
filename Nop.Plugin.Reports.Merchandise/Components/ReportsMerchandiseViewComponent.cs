using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.Reports.Merchandise.Models;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Reports.Merchandise.Components
{
    [ViewComponent(Name = "ReportsMerchandise")]
    public class ReportsMerchandiseViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Reports.Merchandise/Views/PaymentInfo.cshtml");
        }
    }
}
