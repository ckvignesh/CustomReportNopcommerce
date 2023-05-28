using Microsoft.AspNetCore.Http;
using Nop.Web.Framework.Models;


namespace Nop.Plugin.Reports.Merchandise.Models
{
    public record ReturnModel : BaseNopModel
    {
        public IFormCollection Form { get; set; }
    }
}
