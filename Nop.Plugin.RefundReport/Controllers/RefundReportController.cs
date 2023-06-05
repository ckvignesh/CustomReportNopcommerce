using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Core.Domain.Orders;
using Nop.Core;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Plugin.RefundReport.Models;
using Nop.Web.Framework.Models.Extensions;
using Nop.Plugin.RefundReport.Services;

namespace Nop.Plugin.RefundReport
{
    public class RefundReportController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly ICustomerService _customerService;      
        private readonly INopDataProvider _dataProvider;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IProductService _productService;
        private readonly IRefundExportManager _refundExportManager;
        private readonly IRefundReportTransactionService _refundReportTransactionService;

        #endregion

        #region Ctor

        public RefundReportController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            IWebHelper webHelper,
             IWorkContext workContext,
            OrderSettings orderSettings,
            ICustomerService customerService,
            INopDataProvider dataProvider,
            IDateTimeHelper dateTimeHelper,
            IProductService productService,
            IRefundExportManager refundExportManager,
            IRefundReportTransactionService refundReportTransactionService
            )
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _webHelper = webHelper;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _customerService = customerService;
            _dataProvider = dataProvider;
            _dateTimeHelper = dateTimeHelper;
            _productService = productService;
            _refundReportTransactionService = refundReportTransactionService;
            _refundExportManager = refundExportManager;
        }

        #endregion

        #region Methods

        //[HttpPost]
        //[AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [AutoValidateAntiforgeryToken]
        public IActionResult Configure()
        {
            
            var model = new RRConfigurationModel
            {
                
            };
           
            model.SetGridPageSize();

            return View("~/Plugins/RefundReport/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> ConfigureAsync(RRConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) || !await _permissionService.AuthorizeAsync(RefundReportPermission.RefundReport))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
            
            return Configure();
        }


        [HttpPost, ActionName("ExportExcel")]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        //[FormValueRequired("exportexcel-all")]
        public virtual async Task<IActionResult> ExportExcelAll()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) || !await _permissionService.AuthorizeAsync(RefundReportPermission.RefundReport))
                return AccessDeniedView();

            try
            {
                var bytes = await _refundExportManager.ExportFinanceReportToXlsxAsync(await _refundReportTransactionService.GetTransactionLogAsync());
                return File(bytes, MimeTypes.TextXlsx, "RefundReport.xlsx");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return Configure();

            }
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> TransactionListAsync(RRConfigurationModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && !await _permissionService.AuthorizeAsync(RefundReportPermission.RefundReport))
                return await AccessDeniedDataTablesJson();

            var transactions = await _refundReportTransactionService.GetTransactionLogAsync(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize, orderNumber: searchModel.OrderNumber, customer: searchModel.Customer, paymentStatus: searchModel.PaymentStatus, paymentMode: searchModel.PaymentMode);

            var gridModel = new TransactionListModel().PrepareToGrid(searchModel, transactions, () =>
            {
                return transactions.Select(trans => new RRTransactionModel
                {

                    OrderNumber = trans.OrderNumber,
                    Customer = trans.Customer,
                    PaymentMode = trans.PaymentMode,
                    PaymentStatus = trans.PaymentStatus,
                    OrderDate = trans.OrderDate,
                    GST = trans.GST,
                    RefundedAmount = trans.RefundedAmount,
                    RefundedAmountExclTax = trans.RefundedAmountExclTax
                });
            });

            return Json(gridModel);

        }
    }

    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            filterContext.HttpContext.Response.Headers["Expires"] = "-1";
            filterContext.HttpContext.Response.Headers["Pragma"] = "no-cache";

            base.OnResultExecuting(filterContext);
        }
    }
}
#endregion
 