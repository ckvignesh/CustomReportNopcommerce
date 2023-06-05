using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Reports.Merchandise.Controllers;
using Nop.Plugin.Reports.Merchandise.Domain;
using Nop.Plugin.Reports.Merchandise.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Reports.Merchandise
{
    /// <summary>
    /// ENETS payment processor
    /// </summary>
    public class ReportsMerchandisePaymentProcessor : BasePlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
       // private readonly ReportsMerchandisePaymentSettings _eNETSPaymentSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoreContext _storeContext;
        private readonly IReportsMerchandiseTransactionService _transactionService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public ReportsMerchandisePaymentProcessor(ILocalizationService localizationService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ISettingService settingService,
            IWebHelper webHelper,
           // ReportsMerchandisePaymentSettings eNETSPaymentSettings,
            IHttpContextAccessor httpContextAccessor,
            IStoreContext storeContext,
            IReportsMerchandiseTransactionService transactionService,
            IPermissionService permissionService)
        {
            _localizationService = localizationService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _settingService = settingService;
            _webHelper = webHelper;
           // _eNETSPaymentSettings = eNETSPaymentSettings;
            _httpContextAccessor = httpContextAccessor;
            _storeContext = storeContext;
            _transactionService = transactionService;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods


        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.NewPaymentStatus = PaymentStatus.Pending;
            return Task.FromResult(result);
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        /// 

        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            //var eNETSPaymentSettings = await _settingService.LoadSettingAsync<ReportsMerchandisePaymentSettings>(storeScope);

            var amount = (Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2) * 100).ToString("#.##");

            var dateTime = DateTime.Now;
            var date = dateTime.ToString("yyyyMMdd HH:mm:ss.fff");

            var random = new Random();

            //var merchantTxnRef = DateTime.Now.ToString("MMdd") + random.Next(10, 99).ToString() + postProcessPaymentRequest.Order.Id.ToString();  //DateTime.Now.ToString("yyMMddHHmmss") + postProcessPaymentRequest.Order.Id.ToString();
            //var req = "{\"ss\":\"1\",\"msg\":{\"netsMid\":\"" + _eNETSPaymentSettings.NetsMid + "\",\"tid\":\"\",\"submissionMode\":\"B\",\"txnAmount\":\"" + amount + "\",\"merchantTxnRef\":\"" + merchantTxnRef + "\",\"merchantTxnDtm\":\"" + date + "\",\"paymentType\":\"SALE\",\"currencyCode\":\"SGD\",\"paymentMode\":\"\",\"merchantTimeZone\":\"+8:00\",\"b2sTxnEndURL\":\"" + _eNETSPaymentSettings.B2STxnEndURL + "\",\"b2sTxnEndURLParam\":\"\",\"s2sTxnEndURL\":\"" + _eNETSPaymentSettings.B2STxnEndURL + "\",\"s2sTxnEndURLParam\":\"\",\"clientType\":\"W\",\"supMsg\":\"\",\"netsMidIndicator\":\"U\",\"ipAddress\":\"" + postProcessPaymentRequest.Order.CustomerIp + "\",\"language\":\"en\"}}";

            //string hmac = GenerateSignature(req, _eNETSPaymentSettings.SecretKey);

            //Save transaction Log
            var transactionLog = new MerchandiseTransactionLog()
            {
                //OrderId = postProcessPaymentRequest.Order.Id,
                //OrderAmount = postProcessPaymentRequest.Order.OrderTotal,
                //TransactionDate = dateTime,
                //MerchantTxnRef = merchantTxnRef,
                //RequestPayload = req,
                //RequestHmac = hmac
            };
            await _transactionService.InsertTransactionAsync(transactionLog);

            //var remotePostHelper = new RemotePost(_httpContextAccessor, _webHelper)
            //{
            //    FormName = "Merchandise",
            //    Url = _eNETSPaymentSettings.RequestUrl
            //};
            //remotePostHelper.Add("payload", req);
            //remotePostHelper.Add("apiKey", _eNETSPaymentSettings.ApiKey);
            //remotePostHelper.Add("hmac", hmac);
            //remotePostHelper.Post();
        }

        public static string GenerateSignature(string txnReq, string secretKey)
        {

            var concatPayloadAndSecretKey = txnReq + secretKey;
            var hmac = EncodeBase64(HashSHA256ToBytes(Encoding.UTF8.GetBytes(concatPayloadAndSecretKey)));
            //string hmac = Base64Encode(sha256_hash(concatPayloadAndSecretKey));
            return hmac;
        }

        public static byte[] HashSHA256ToBytes(byte[] input)
        {
            var sha1 = SHA256.Create();
            var outputBytes = sha1.ComputeHash(input);

            return outputBytes;

        }
        public static string EncodeBase64(byte[] data)
        {
            return Convert.ToBase64String(data);
        }


        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return Task.FromResult(true);
        }

        //hide payment ends

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        //public async Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        //{
        //    return await _orderTotalCalculationService.CalculatePaymentAdditionalFeeAsync(cart,
        //       _eNETSPaymentSettings.AdditionalFee, _eNETSPaymentSettings.AdditionalFeePercentage);
        //}

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");


            return Task.FromResult(result);
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return Task.FromResult(result);
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return Task.FromResult(result);
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>

        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return Task.FromResult(result);
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return Task.FromResult(result);
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //eNETS is the redirection payment method
            //It also validates whether order is also paid (after redirection) so customers will not be able to pay twice

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
            return $"{_webHelper.GetStoreLocation()}Admin/ReportsMerchandise/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <returns>View component name</returns>
        public string GetPublicViewComponentName()
        {
            return "ReportsMerchandise";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
          //  var settings = new ReportsMerchandisePaymentSettings
            //{
            //    TransactMode = TransactMode.Pending
            //};
            //await _settingService.SaveSettingAsync(settings);

            //NEW CODE
            //add permission for customer role
            await _permissionService.InstallPermissionsAsync(new MerchandiseReportPermission());
            //-----

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.ReportsMerchandise.Instructions"] = "",
                ["Plugins.ReportsMerchandise.Fields.AdditionalFee"] = "Additional fee",
                ["Plugins.ReportsMerchandise.Fields.AdditionalFee.Hint"] = "Enter additional fee to charge your customers.",
                ["Plugins.ReportsMerchandise.Fields.AdditionalFeePercentage"] = "Additional fee. Use percentage",
                ["Plugins.ReportsMerchandise.Fields.AdditionalFeePercentage.Hint"] = "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.",
                ["Plugins.ReportsMerchandise.TransactMode"] = "After checkout mark payment as",
                ["Plugins.ReportsMerchandise.Fields.TransactMode.Hint"] = "Specify transaction mode.",
                ["Plugins.ReportsMerchandise.PaymentMethodDescription"] = "You will be redirected to eNETS site to complete the order.",
                ["Plugins.ReportsMerchandise.PaymentModeCC"] = "Credit Card",
                ["Plugins.ReportsMerchandise.PaymentModeDD"] = "Direct Debit",
                ["Plugins.ReportsMerchandise.PaymentModeQR"] = "QR Code",
                ["Plugins.ReportsMerchandise.Company"] = "SingHealth"
            });

            await base.InstallAsync();
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && !await _permissionService.AuthorizeAsync(MerchandiseReportPermission.MerchandiseReport))
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
                SystemName = "Reports.Merchandise",
                Title = "Merchandise Reports",
                ControllerName = "ReportsMerchandise",
                ActionName = "Configure",
                IconClass = "far fa-dot-circle",
                Visible = true,
                RouteValues = new RouteValueDictionary { { "area", AreaNames.Admin } }
            });

        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            //await _settingService.DeleteSettingAsync<ReportsMerchandisePaymentSettings>();

            //delete permission
            //if (await _permissionService.AuthorizeAsync(MerchandiseReportPermission.MerchandiseReport))
            //{
            //    var permissionRecord = (await _permissionService.GetAllPermissionRecordsAsync())
            //        .FirstOrDefault(x => x.SystemName == MerchandiseReportPermission.MerchandiseReport.SystemName);
            //    var listMappingCustomerRolePermissionRecord = await _permissionService.GetMappingByPermissionRecordIdAsync(permissionRecord.Id);
            //    foreach (var mappingCustomerPermissionRecord in listMappingCustomerRolePermissionRecord)
            //        await _permissionService.DeletePermissionRecordCustomerRoleMappingAsync(
            //            mappingCustomerPermissionRecord.PermissionRecordId,
            //            mappingCustomerPermissionRecord.CustomerRoleId);

            //    await _permissionService.DeletePermissionRecordAsync(permissionRecord);
            //}

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Reports.Merchandise");

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
            return await _localizationService.GetResourceAsync("Plugins.Reports.Merchandise.PaymentMethodDescription");
        }

        #endregion
    }
}