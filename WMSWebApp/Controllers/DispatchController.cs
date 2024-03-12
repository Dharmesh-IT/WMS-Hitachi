using Application.Services.Invoice;
using Application.Services.Master;
using Application.Services.PO;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Model.Invoice;
using Domain.Model.Masters;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WMS.Data;
using WMS.Web.Framework.Infrastructure.Extentsion;
using WMSWebApp.HitachiProvider;
using WMSWebApp.HitachiProvider.HitachiModel;
using WMSWebApp.ViewModels.Dispatch;
using WMSWebApp.ViewModels.Invoice;
using WMSWebApp.ViewModels.PO;
using static WMSWebApp.Controllers.IntrasitController;

namespace WMSWebApp.Controllers
{
    public class DispatchController : BaseAdminController
    {

        #region Fields
        private readonly IInvoiceService _invoiceService;
        private readonly IDispatchService _dispatchService;
        private readonly IWorkContext _workContext;
        private readonly ILogger<DispatchController> _logger;
        private readonly IHitachiConnection _hitachiConnection;
        private readonly ISalePo _saleService;
        IFormatProvider culture = new System.Globalization.CultureInfo("fr-FR", true);
        #endregion

        #region Ctor
        public DispatchController(IInvoiceService invoiceService, IDispatchService dispatchService, IWorkContext workContext, ILogger<DispatchController> logger,
            IHitachiConnection hitachiConnection,ISalePo saleService)
        {
            _invoiceService = invoiceService;
            _dispatchService = dispatchService;
            _workContext = workContext;
            _logger = logger;
            _hitachiConnection = hitachiConnection;
            _saleService = saleService;
        }
        #endregion

        #region Methods

        public IActionResult Index()
        {
            return RedirectToAction("List");

        }

        public IActionResult List()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> List(DataSourceRequest request)
        {
            var branch = await _workContext.GetCurrentBranch();
            var dispatches = _dispatchService.GetAllDispatch(branch.BranchCode, 0, int.MaxValue);
            DataSourceResult dataSource = new DataSourceResult
            {
                Data = dispatches.Select(x =>
                {
                    DispatchList m = new DispatchList();
                    m.DispatchDate = x.DispatchDate;
                    m.CreateOn = DateTime.Now;
                    m.InvoiceId = x.InvoiceId;
                    m.PO = x.PO;
                    m.VendorName = x.VendorName;
                    m.VehicleNumber = x.VehicleNumber;
                    m.Location = x.Location;
                    m.Id = x.Id;
                    m.BranchCode = x.BranchCode;
                    var invoice = _invoiceService.GetById(x.InvoiceId);
                    m.InvoiceDate = invoice.CreateOn;
                    return m;
                }),
                Total = dispatches.TotalCount
            };
            return Json(dataSource);
        }

        public async Task<IActionResult> Create()
        {
            DispatchModel model = new DispatchModel();
            var branch = await _workContext.GetCurrentBranch();
            var invoice = _invoiceService.GetAllMaster(branch.BranchCode, 0, int.MaxValue, "NDP");
            model.InvoiceList = invoice.ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(DispatchModel model)
        {
            
            var branch = await _workContext.GetCurrentBranch();
            Dispatch dispatch = new Dispatch();

            var salesItemList = _saleService.GetAllItemSalePos(model.PO).ToList();
            SaleNotificationModel saleNotificationModel = new();
            SaleOrder saleOrder = new();
            saleOrder.fulfillment_center = branch.BranchCode;
            saleOrder.status = "DLV";
            saleOrder.order_number = model.PO;
            var saleDifferentItem = salesItemList.GroupBy(x => x.orderline_number).Select(y => new SaleOrderLine()
            {

                order_line_number = y.Key.ToString(),
                products = y.ToList().Select(z => new SaleProduct() { prod_sku = z.product_sku, prod_qty = z.quantity }).ToList()
            });
            saleOrder.shipments.AddRange( new List<SaleShipment>() { new () {  courier = model.VendorName,
                created_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+" IST",
                dispatched_at = model.DispatchDate.ToString("yyyy-MM-dd HH:mm:ss")+" IST",
                delivered_at = model.DeliveredDate.ToString("yyyy-MM-dd HH:mm:ss")+" IST",
                waybill = model.LRNo,
                shipment_number = salesItemList[0].shipments_number.ToString(),
                order_lines = saleDifferentItem.ToList() } });

            saleNotificationModel.orders.Add(saleOrder);
            var jsonObject = JsonConvert.SerializeObject(saleNotificationModel);
            var resultOfSalesNotification = await _hitachiConnection.SubmitOrderNotification(jsonObject);
            if (resultOfSalesNotification.Status.ToLower() == "success")
            {
                dispatch.DispatchDate = Convert.ToDateTime(model.DispatchDate, culture);
                dispatch.CreateOn = DateTime.Now;
                dispatch.InvoiceId = model.InvoiceId;
                dispatch.PO = model.PO;
                dispatch.VendorName = model.VendorName;
                dispatch.VehicleNumber = model.VehicleNumber;
                dispatch.Location = model.Location;
                dispatch.BranchCode = branch.BranchCode;
                dispatch.DocketNo = model.DocketNo;
                dispatch.LRNo = model.LRNo;
                //string json = JsonConvert.SerializeObject(dispatch);
                //SentrySdk.CaptureMessage(json);
                _dispatchService.Insert(dispatch);
                var invoice = _invoiceService.GetById(model.InvoiceId);
                invoice.DispatchDone = true;
                _invoiceService.Update(invoice);

                SuccessNotification("Dispatch created successfully and Sale Notification Done Successfully.");
                return RedirectToAction("List");

            }
            else if(resultOfSalesNotification.Status.ToLower() == "failure")
            {
                ErrorNotification(resultOfSalesNotification.Message);
            }
            return RedirectToAction("Create", "Dispatch");
        }

        [HttpGet]
        public IActionResult GetInvoice(int id)
        {
            var invoice = _invoiceService.GetById(id);
            List<ItemModel> items = new List<ItemModel>();
            foreach (var item in invoice.InvoiceDetails)
            {
                ItemModel details = new ItemModel();
                details.Amt = item.Amt;
                details.AreaCode = item.AreaCode;
                details.AreaId = item.AreaId;
                details.AreaName = item.AreaName;
                details.Id = item.Id;
                details.InvoiceMasterId = item.InvoiceMasterId;
                details.MaterialDescription = item.MaterialDescription;
                details.PoCategory = item.PoCategory;
                details.Qty = item.Qty;
                details.SerialNo = item.SerialNo;
                details.SubItem = item.SubItem;
                details.SubItemCode = item.SubItemCode;
                details.SubItemName = item.SubItemName;
                details.WarehouseCode = item.WarehouseCode;
                details.Warehouse = item.Warehouse;
                details.ZoneCode = item.ZoneCode;
                details.ZoneName = item.ZoneName;
                details.PONumber = invoice.PoNumber;
                details.Address = invoice.Address1;
                items.Add(details);
            }
            return Json(items);

        }


        #endregion



    }
}
