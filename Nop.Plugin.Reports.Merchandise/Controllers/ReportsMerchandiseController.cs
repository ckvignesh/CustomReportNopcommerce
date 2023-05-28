using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using LinqToDB.DataProvider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Plugin.Reports.Merchandise.Domain;
using Nop.Plugin.Reports.Merchandise.Models;
using Nop.Plugin.Reports.Merchandise.Services;
using Nop.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Reports.Merchandise.Controllers
{
    public class ReportsMerchandiseController : BasePaymentController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPaymentService _paymentService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly ICustomerService _customerService;
        private readonly IReportsMerchandiseTransactionService _transactionService;
        //private readonly IWorkflowMessageService _workflowMessageService;
        //private readonly IPdfService _pdfService;
        //private readonly LocalizationSettings _localizationSettings;
        //private readonly IVendorService _vendorService;
        private readonly INopDataProvider _dataProvider;
        private readonly IMerchandiseReport _merchandiseReport;
        #endregion

        #region Ctor

        public ReportsMerchandiseController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            IPaymentService paymentService,
            IWebHelper webHelper,
             IWorkContext workContext,
            OrderSettings orderSettings,
            ICustomerService customerService,
            IReportsMerchandiseTransactionService transactionService,
            INopDataProvider dataProvider,
            IMerchandiseReport merchandiseReport
            //IWorkflowMessageService workflowMessageService,
            //IPdfService pdfService,
            //LocalizationSettings localizationSettings,
            //IVendorService vendorService
            )
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _paymentService = paymentService;
            _webHelper = webHelper;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _customerService = customerService;
            _transactionService = transactionService;
            _dataProvider = dataProvider;
            _merchandiseReport = merchandiseReport;
            //_workflowMessageService = workflowMessageService;
            //_pdfService= pdfService;
            //_localizationSettings = localizationSettings;
            //_vendorService = vendorService;
        }

        #endregion

        #region Methods

        //[HttpPost]
        //[AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [AutoValidateAntiforgeryToken]
        public IActionResult Configure()
        {
            //if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
            //    return AccessDeniedView();

            //load settings for a chosen store scope
            //var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            //var eNETSPaymentSettings = await _settingService.LoadSettingAsync<ENETSPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                //TransactModeId = Convert.ToInt32(eNETSPaymentSettings.TransactMode),
                //AdditionalFee = eNETSPaymentSettings.AdditionalFee,
                //AdditionalFeePercentage = eNETSPaymentSettings.AdditionalFeePercentage,
                //TransactModeValues = await eNETSPaymentSettings.TransactMode.ToSelectListAsync(),
                //ActiveStoreScopeConfiguration = storeScope,

                //ApiKey = eNETSPaymentSettings.ApiKey,
                //NetsMid = eNETSPaymentSettings.NetsMid,
                //SecretKey = eNETSPaymentSettings.SecretKey,
                //B2STxnEndURL = eNETSPaymentSettings.B2STxnEndURL,
                //RequestUrl = eNETSPaymentSettings.RequestUrl
            };
            ////if (storeScope > 0)
            ////{
            ////    //model.TransactModeId_OverrideForStore = await _settingService.SettingExistsAsync(eNETSPaymentSettings, x => x.TransactMode, storeScope);
            ////    //model.AdditionalFee_OverrideForStore = await _settingService.SettingExistsAsync(eNETSPaymentSettings, x => x.AdditionalFee, storeScope);
            ////    //model.AdditionalFeePercentage_OverrideForStore = await _settingService.SettingExistsAsync(eNETSPaymentSettings, x => x.AdditionalFeePercentage, storeScope);

            ////    //model.ApiKey_OverrideForStore = await _settingService.SettingExistsAsync(eNETSPaymentSettings, x => x.ApiKey, storeScope);
            ////    //model.NetsMid_OverrideForStore = await _settingService.SettingExistsAsync(eNETSPaymentSettings, x => x.NetsMid, storeScope);
            ////    //model.SecretKey_OverrideForStore = await _settingService.SettingExistsAsync(eNETSPaymentSettings, x => x.SecretKey, storeScope);
            ////    //model.B2STxnEndURL_OverrideForStore = await _settingService.SettingExistsAsync(eNETSPaymentSettings, x => x.B2STxnEndURL, storeScope);
            ////    //model.RequestUrl_OverrideForStore = await _settingService.SettingExistsAsync(eNETSPaymentSettings, x => x.RequestUrl, storeScope);

            ////}

            model.SetGridPageSize();

            return View("~/Plugins/Reports.Merchandise/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> ConfigureAsync(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            ////load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var eNETSPaymentSettings = await _settingService.LoadSettingAsync<ReportsMerchandisePaymentSettings>(storeScope);

            ////save settings
            //eNETSPaymentSettings.TransactMode = (TransactMode)model.TransactModeId;
            //eNETSPaymentSettings.AdditionalFee = model.AdditionalFee;
            //eNETSPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;

            //eNETSPaymentSettings.ApiKey = model.ApiKey;
            //eNETSPaymentSettings.SecretKey = model.SecretKey;
            //eNETSPaymentSettings.NetsMid = model.NetsMid;
            //eNETSPaymentSettings.B2STxnEndURL = model.B2STxnEndURL;
            //eNETSPaymentSettings.RequestUrl = model.RequestUrl;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */

            //await _settingService.SaveSettingOverridablePerStoreAsync(eNETSPaymentSettings, x => x.TransactMode, model.TransactModeId_OverrideForStore, storeScope, false);
            //await _settingService.SaveSettingOverridablePerStoreAsync(eNETSPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            //await _settingService.SaveSettingOverridablePerStoreAsync(eNETSPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);

            //await _settingService.SaveSettingOverridablePerStoreAsync(eNETSPaymentSettings, x => x.ApiKey, model.ApiKey_OverrideForStore, storeScope, false);
            //await _settingService.SaveSettingOverridablePerStoreAsync(eNETSPaymentSettings, x => x.SecretKey, model.SecretKey_OverrideForStore, storeScope, false);
            //await _settingService.SaveSettingOverridablePerStoreAsync(eNETSPaymentSettings, x => x.NetsMid, model.NetsMid_OverrideForStore, storeScope, false);
            //await _settingService.SaveSettingOverridablePerStoreAsync(eNETSPaymentSettings, x => x.B2STxnEndURL, model.B2STxnEndURL_OverrideForStore, storeScope, false);
            //await _settingService.SaveSettingOverridablePerStoreAsync(eNETSPaymentSettings, x => x.RequestUrl, model.RequestUrl_OverrideForStore, storeScope, false);

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
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            try
            {
                var bytes = await _merchandiseReport.ExportMerchadiseReportToXlsxAsync(await _transactionService.GetTransactionLogAsync());
                return File(bytes, MimeTypes.TextXlsx, "MerchandiseReport.xlsx");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                //return RedirectToAction("List");
                return Configure();

            }
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> TransactionListAsync(ConfigurationModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return await AccessDeniedDataTablesJson();

            var transactions = await _transactionService.GetTransactionLogAsync(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize, productName: searchModel.ProductName, sKU: searchModel.SKU, categories: searchModel.Categories, brands:searchModel.Brands, vendorName:searchModel.VendorName, productLaunchDate:searchModel.ProductLaunchDate);

            var gridModel = new TransactionListModel().PrepareToGrid(searchModel, transactions, () =>
            {
                return transactions.Select(trans => new TransactionModel
                {
                    ProductName = trans.ProductName,
                    SKU = trans.SKU,
                    ProductID = trans.ProductID,
                    StockQuantity = trans.StockQuantity,
                    RetailPrice = trans.RetailPrice,
                    OldPrice = trans.OldPrice,
                    ProductCost = trans.ProductCost,
                    DiscountAmount = trans.DiscountAmount,
                    DiscountName = trans.DiscountName,
                    DiscountPercentage = trans.DiscountPercentage,
                    DiscountType = trans.DiscountType,
                    PromotionEndDate = trans.PromotionEndDate,
                    PromotionStartDate = trans.PromotionStartDate,
                    Categories = trans.Categories,
                    Brands = trans.Brands,
                    VendorName = trans.VendorName,
                    ProductLaunchDate = trans.ProductLaunchDate,
                    ProductStatus = trans.ProductStatus,
                    InventoryMethod = trans.InventoryMethod,
                });
            });
            
            return Json(gridModel);
            
        }
    }

    //////[HttpPost, ActionName("ExportToExcel")]
    //////[FormValueRequired("exportexcel-all")]
    //////public async Task<IActionResult> ExportExcelAll(ConfigurationModel model)
    //////{
    //////    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
    //////        return AccessDeniedDataTablesJson();

    //////    var categoryIds = new List<int> { model.SearchCategoryId };
    //////    //include subcategories
    //////    if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
    //////        categoryIds.AddRange(await _categoryService.GetChildCategoryIdsAsync(parentCategoryId: model.SearchCategoryId, showHidden: true));

    //////    //0 - all (according to "ShowHidden" parameter)
    //////    //1 - published only
    //////    //2 - unpublished only
    //////    bool? overridePublished = null;
    //////    if (model.SearchPublishedId == 1)
    //////        overridePublished = true;
    //////    else if (model.SearchPublishedId == 2)
    //////        overridePublished = false;

    //////    var products = await _productService.SearchProductsAsync(0,
    //////        categoryIds: categoryIds,
    //////        manufacturerIds: new List<int> { model.SearchManufacturerId },
    //////        storeId: model.SearchStoreId,
    //////        vendorId: model.SearchVendorId,
    //////        warehouseId: model.SearchWarehouseId,
    //////        productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
    //////        keywords: model.SearchProductName,
    //////        showHidden: true,
    //////        overridePublished: overridePublished);

    //////    try
    //////    {
    //////        var bytes = await _exportManager.ExportProductsToXlsxAsync(products);

    //////        return File(bytes, MimeTypes.TextXlsx, "FinanceReport.xlsx");
    //////    }
    //////    catch (Exception exc)
    //////    {
    //////        await _notificationService.ErrorNotificationAsync(exc);

    //////        return RedirectToAction("List");
    //////    }
    //////}
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