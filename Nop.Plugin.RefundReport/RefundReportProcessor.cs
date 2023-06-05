using Nop.Core.Domain.Payments;
using Nop.Services.Payments;
using System.Threading.Tasks;
using Nop.Services.Plugins;
using Nop.Core;
using System;
using Nop.Services.Localization;
using Nop.Plugin.RefundReport.Services;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Services.Security;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Menu;
using Nop.Web.Framework;
using System.Linq;
using Nop.Plugin.RefundReport.Domain;

namespace Nop.Plugin.RefundReport
{
    public class RefundReportProcessor : BasePlugin ,IAdminMenuPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IRefundReportTransactionService _transactionService;
        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public RefundReportProcessor(
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IRefundReportTransactionService transactionService,
            IWebHelper webHelper,
            IPermissionService permissionService    
            )
        {
            _localizationService = localizationService;
            _storeContext = storeContext;
            _transactionService = transactionService;
            _webHelper = webHelper;
            _permissionService = permissionService;
        }
        #endregion
        #region Methods

        public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.NewPaymentStatus = PaymentStatus.Pending;
            return Task.FromResult(result);
        }

        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();

            var amount = (Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2) * 100).ToString("#.##");

            var dateTime = DateTime.Now;
            var date = dateTime.ToString("yyyyMMdd HH:mm:ss.fff");

            var random = new Random();

            //Save transaction Log
            var transactionLog = new RefundReportTransactionlog()
            {
                //OrderId = postProcessPaymentRequest.Order.Id,
            };
            await _transactionService.InsertTransactionAsync(transactionLog);

        }

        public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return Task.FromResult(result);
        }

        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return Task.FromResult(result);
        }

        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return Task.FromResult(result);
        }
        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return Task.FromResult(result);
        }
        public Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //payment status should be Pending
            if (order.PaymentStatus != PaymentStatus.Pending)
                return Task.FromResult(false);

            //let's ensure that at least 1 minute passed after order is placed
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes < 1)
                return Task.FromResult(false);

            return Task.FromResult(true);
        }

        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            var warnings = new List<string>();
            return Task.FromResult<IList<string>>(new List<string>());
        }
        public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            return Task.FromResult(new ProcessPaymentRequest());
        }


        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/RefundReport/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <returns>View component name</returns>
        public string GetPublicViewComponentName()
        {
            return "RefundReport";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            //var settings = new FinanceReportPaymentSettings
            //{
            //    TransactMode = TransactMode.Pending
            //};
            //await _settingService.SaveSettingAsync(settings);

            //add permission
            await _permissionService.InstallPermissionsAsync(new RefundReportPermission());

            ////locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.RefundReport.PaymentMethodDescription"] = " ",
            });

            await base.InstallAsync();
        }
        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && !await _permissionService.AuthorizeAsync(RefundReportPermission.RefundReport))
                return;

            var config = rootNode.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Reports"));
            if (config == null)
                return;

            var plugins = config.ChildNodes.FirstOrDefault(node => node.SystemName.Equals("Sales summary"));

            if (plugins == null)
                return;

            var index = config.ChildNodes.IndexOf(plugins);

            if (index < 0)
                return;

            config.ChildNodes.Insert(index, new SiteMapNode
            {
                SystemName = "RefundReport",
                Title = "Refund Report",
                ControllerName = "RefundReport",
                ActionName = "Configure",
                IconClass = "far fa-dot-circle",
                Visible = true,
                RouteValues = new RouteValueDictionary { { "area", AreaNames.Admin } }
            });

        }
        public override async Task UninstallAsync()
        {
            
            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.RefundReport");

            await base.UninstallAsync();
        }
        #endregion

        #region Properies

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture => false;


        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund => false;


        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund => false;


        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid => false;


        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;


        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public bool SkipPaymentInfo => false;

        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            return await _localizationService.GetResourceAsync("Plugins.RefundReport.PaymentMethodDescription");
        }

        #endregion
    }
}