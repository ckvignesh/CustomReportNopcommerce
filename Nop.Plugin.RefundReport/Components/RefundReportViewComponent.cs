using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.RefundReport
{
    [ViewComponent(Name = "RefundReport")]
    public class RefundReportViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/RefundReport/Views/PaymentInfo.cshtml");
        }
    }
}
