using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.FinanceReport.Components
{
    [ViewComponent(Name = "FinanceREport")]
    public class FinanceReportViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/FinanceReport/Views/PaymentInfo.cshtml");
        }
    }
}
