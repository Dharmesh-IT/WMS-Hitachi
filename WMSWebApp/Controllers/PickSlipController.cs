using Microsoft.AspNetCore.Mvc;
using Application.Services.PS;
using Application.Services.GRN;
using Microsoft.AspNetCore.Authorization;
using Domain.Model.PS;
using WMS.Web.Framework.Infrastructure.Extentsion;
using System;
using System.Linq;
using Application.Services.WarehouseMaster;
using WMSWebApp.ViewModels.Pickslip;
using WMS.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using WMSWebApp.ViewModels.GRN;
using Application.Services.PO;
using Application.Services.StockMgnt;
using Domain.Model;
using Domain.Model.Masters;
using Domain.Model.StockManagement;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Application.Services;
using WMSWebApp.ViewModels.Invoice;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Model.Invoice;
using Application.Services.Invoice;

namespace WMSWebApp.Controllers
{
    [Authorize]
    public class PickSlipController : BaseAdminController
    {

        #region Fields
        // private readonly ITempPickSlipDetailsService _tempPickSlipDetailsService;
        private readonly IIntrasitHelper _intrasitHelper;
        private readonly IPickSlipService _pickSlipService;
        private readonly IWarehouseService _warehouseService;
        private readonly IGoodReceivedNoteMasterService _goodReceivedNoteMasterService;
        private readonly IWorkContext _workContext;
        private readonly IItemStockService _itemStockService;
        private readonly IPurchaseOrder _purchaseOrder;
        private readonly ISalePo _salePoService;
        private readonly IServiceOrderPo _serviceOrderPoService;
        private readonly IStockTransferPo _stockTransferPoService;
        private readonly ISubItemHelper _subItemService;
        private readonly IInvoiceService _invoiceService;


        #endregion

        #region Ctor
        public PickSlipController(IIntrasitHelper  intrasitHelper, IPickSlipService pickSlipService, 
            IWarehouseService warehouseService, IGoodReceivedNoteMasterService goodReceivedNoteMasterService, 
            IWorkContext workContext, IPurchaseOrder purchaseOrder, ISalePo salePoService, IServiceOrderPo serviceOrderPoService,
            IStockTransferPo stockTransferPoService, IItemStockService itemStockService, ISubItemHelper subItemService,
            IInvoiceService invoiceService)
        {
            _intrasitHelper = intrasitHelper;
            _pickSlipService = pickSlipService;
            _warehouseService = warehouseService;
            _goodReceivedNoteMasterService = goodReceivedNoteMasterService;
            _workContext = workContext;
            _purchaseOrder = purchaseOrder;
            _salePoService = salePoService;
            _serviceOrderPoService = serviceOrderPoService;
            _stockTransferPoService = stockTransferPoService;
            _itemStockService = itemStockService;
            _subItemService = subItemService;
            _invoiceService = invoiceService;
        }
        #endregion

        #region Methods
        public IActionResult Index()
        {
            return View();
        }




        public virtual IActionResult List(DataSourceRequest request, string guid)
        {
            List<PickslipDetailModel> result = new List<PickslipDetailModel>();
            // var result = _tempPickSlipDetailsService.GetAllTemp(guid, request.Page - 1, request.PageSize);
            var gridData = new DataSourceResult()
            {
                Data = result,

                Total = result.Count,
            };

            return Json(gridData);
        }

        [HttpGet]
        public virtual IActionResult GrnList()
        {
            var branch = _workContext.GetCurrentBranch().Result;
            var pos = _goodReceivedNoteMasterService.GetAllMaster(branch.BranchCode).ToList();
            List<GrnListModel> list = new List<GrnListModel>();
            list = pos.Select(x => new GrnListModel() { GRNId = x.Id }).ToList();
            return Json(list);
        }


        [HttpGet]
        public virtual IActionResult PoList(string docType)
        {
            var branch = _workContext.GetCurrentBranch().Result;
            var pos = _purchaseOrder.GetPurchaseOrders(branch.BranchCode, docType);
            List<GrnListModel> list = new List<GrnListModel>();
            list = pos.Select(x => new GrnListModel() { PoNumber = x.PONumber, GRNId = x.Id }).ToList();
            return Json(list);
        }

        //[HttpGet]
        //public virtual IActionResult GetGrnProduct(int id,string category)
        //{
        //    var grnitems = _purchaseOrder.GetById(id);
        //    if(category=="")
        //    var items = grnitems.GoodReceivedNoteDetails.ToList();
        //    List<GrnItemListModel> model = new List<GrnItemListModel>();
        //    foreach (var item in items)
        //    {
        //        GrnItemListModel m = new GrnItemListModel()
        //        {
        //            Amount = item.Amount,
        //            SubItemName = item.SubItemName,
        //            AreaId = item.AreaId,
        //            GRNId = item.GRNId,
        //            Id = item.Id,
        //            ItemCode = item.ItemCode,
        //            MaterialDescription = item.MaterialDescription,
        //            Qty = item.Qty,
        //            SubItemCode = item.SubItemCode,
        //            Unit = item.Unit,
        //        };
        //        model.Add(m);
        //    }
        //    return Json(model);
        //}


        [HttpGet]
        public virtual IActionResult GetPoProduct(string id, string docType)
        {
            var branch = _workContext.GetCurrentBranch().Result;
            List<GrnItemListModel> model = new List<GrnItemListModel>();
            if (docType == "StockTransfer PO")
            {
                var items = _stockTransferPoService.GetStockTransferPos(id);

                foreach (var item in items)
                {
                    GrnItemListModel m = new GrnItemListModel();
                    var itemDetails = _intrasitHelper.GetItemSubItemDetails(item.sku).FirstOrDefault();
                    if(itemDetails != null)
                    {
                        m.Amount = 0;
                        m.SubItemName = itemDetails.SubItemName;
                        m.POId = id;
                        m.Id = item.Id;
                        m.ItemCode = itemDetails.ItemCode;
                        m.MaterialDescription = itemDetails.MaterialDescription;
                        m.Qty = item.quantity;
                        m.SubItemCode = itemDetails.SubItemCode;
                        m.Location = "";
                        m.Unit = "";
                        m.line_itemId = item.line_itemid;
                        model.Add(m);
                    }
                  
                }
            }
            else if (docType == "Sale PO")
            {
                var items = _salePoService.GetSalePos(id);
                foreach (var item in items)
                {
                    var itemDetails = _intrasitHelper.GetItemSubItemDetails(item.product_sku).FirstOrDefault();
                    if(itemDetails != null)
                    {
                        GrnItemListModel m = new GrnItemListModel();
                        m.Amount = Convert.ToInt32(item.SalePOAmt);
                        m.SubItemName = itemDetails.SubItemName;
                        m.POId = id;
                        m.Id = item.Id;
                        m.ItemCode = itemDetails.ItemCode;
                        var location = _itemStockService.ItemsByCode(itemDetails.SubItemCode, branch.BranchWiseWarehouses.FirstOrDefault().WarehouseId);
                        if (location != null && location.Count > 0)
                        {
                            var warehouse = _warehouseService.GetWarehouseZoneAreaById(location.FirstOrDefault().AreaId);
                            m.Location = warehouse.AreaName;
                            m.AreaId = warehouse.Id;
                            m.InventoryId = location.FirstOrDefault().Id;
                        }
                        m.MaterialDescription = itemDetails.MaterialDescription;
                        m.Qty = item.quantity;
                        m.SubItemCode = itemDetails.SubItemCode;
                        m.Unit = "";
                        m.invoiceNumber = item.invoiceNumber;
                        m.Address1 = "Name : " + item.consignee_name + ".\n" + "Address : " + item.consignee_address_line1 + ".\n" + item.consignee_city + " - " + item.consignee_pin_code +
                            "," + item.consignee_state + "," + item.consignee_country;
                        model.Add(m);
                    }
                }
            }
            else

            {
                var items = _serviceOrderPoService.GetServicePos(id);
                foreach (var item in items)
                {
                    GrnItemListModel m = new GrnItemListModel()
                    {
                        Amount = item.ServiceOrderPOQty,
                        SubItemName = item.ServiceOrderPOSubitem,
                        Id = item.Id,
                        ItemCode = item.SubItemCode,
                        MaterialDescription = "",
                        Qty = item.ServiceOrderPOQty,
                        SubItemCode = item.SubItemCode,
                        Location = "",
                        POId = id,
                    };
                    model.Add(m);
                }
            }
            return Json(model);

        }
        [HttpGet]
        public virtual IActionResult GetLocation(string SubItemCode)
        {
            var branch = _workContext.GetCurrentBranch().Result;
            var location = _itemStockService.ItemsByCode(SubItemCode, branch.BranchWiseWarehouses.FirstOrDefault().WarehouseId);
            List<LocationList> locations = new List<LocationList>();
            if (location != null)
            {
                foreach (var item in location)
                {
                    LocationList locationList = new LocationList();
                    var warehouse = _warehouseService.GetWarehouseZoneAreaById(item.AreaId);

                    locationList.Location = warehouse.AreaName;
                    locationList.AreaId = warehouse.Id;
                    locationList.InventoryId = item.Id;
                    locations.Add(locationList);
                }

            }
            return Json(locations);
        }


        [HttpGet]
        public virtual IActionResult GetLocationForGrid(string SubItemCode)
        {
            var branch = _workContext.GetCurrentBranch().Result;
            var location = _intrasitHelper.GetItemDetailsForPickSlip(SubItemCode,branch.BranchWiseWarehouses.FirstOrDefault().WarehouseId);
            location = location.DistinctBy(x => new { x.AreaId, x.InventoryId }).ToList() ;
            var gridData = new DataSourceResult()
            {
                Data = location,
                Total = location.Count,
            };

            return Json(gridData);
        }

        [HttpPost]
        public virtual IActionResult Save([FromBody] List<GrnItemListModel> model)
        {
            var pickSlipId = 0;
            
            if (model.Count > 0)
            {
                var branch = _workContext.GetCurrentBranch().Result;
                PickSlipMaster master = new PickSlipMaster();
                master.PickSlipName = model.FirstOrDefault().POId + "_" + "PickSlip";
                master.CreateOn = DateTime.Now;
                master.BranchCode = branch.BranchCode;
                master.POID = Convert.ToInt32(model.FirstOrDefault().POId);
                master.DockType = model.FirstOrDefault().DockType;
                InvoiceMaster invoiceMaster = new InvoiceMaster();
                foreach (var item in model)
                {
                    PickSlipDetails details = new PickSlipDetails();
                    details.Qty = item.Qty;
                    details.SubItemName = item.SubItemName;
                    details.AreaId = item.AreaId;
                    details.Amount = item.Amount;
                    details.InventoryId = item.InventoryId;
                    details.ItemCode = item.ItemCode;
                    details.PickSlipMaster = master;
                    details.SubItemCode = item.SubItemCode;
                    details.Unit = item.Unit;
                    master.PickSlipDetails.Add(details);
              
                    var pos = _salePoService.GetById(item.Id);
                    pos.IsProcessed = true;
                    _salePoService.Update(pos);

                    InvoiceDetails m = new InvoiceDetails();

                    m.Amt = item.Amount;
                    m.SubItemName = item.SubItemName;
                    m.AreaId = item.AreaId;
                    //m.Id = item.Id;
                    m.SubItemCode = item.SubItemCode;
                    m.MaterialDescription = item.MaterialDescription;
                    var area = _warehouseService.GetWarehouseZoneAreaById(item.AreaId);
                    if (area != null)
                    {
                        m.AreaCode = area.AreaCode;
                        m.AreaName = area.AreaName;
                        var zone = _warehouseService.GetZoneById(area.ZoneId);
                        m.ZoneCode = zone.ZoneCode;
                        m.ZoneName = zone.ZoneName;
                        var warehouse = _warehouseService.GetById(zone.WarehouseId);
                        m.Warehouse = warehouse.WarehouseName;
                        m.WarehouseCode = warehouse.WarehouseCode;
                    }

                    m.Qty = item.Qty;
                    m.InvoiceMaster = invoiceMaster;
                    invoiceMaster.InvoiceDetails.Add(m);

                }
                pickSlipId = _pickSlipService.InsertAndReturnId(master);

                invoiceMaster.PoNumber = model.FirstOrDefault().POId;
                invoiceMaster.PoCategory = model.FirstOrDefault().DockType;
                invoiceMaster.CreateOn = DateTime.Now;
                invoiceMaster.BilledTo = "";
                invoiceMaster.Address1 = model.FirstOrDefault().Address1;
                invoiceMaster.PickSlipId = pickSlipId;
                invoiceMaster.InvoiceNumber = model.FirstOrDefault().invoiceNumber;
                invoiceMaster.BranchCode = branch.BranchCode;
                invoiceMaster.DispatchDone = false;

                var allItem = _salePoService.GetAllItemSalePos(model.FirstOrDefault().POId.ToString());
                var processItems = allItem.Where(x => x.IsProcessed == true).ToList();
                if (allItem.Count > 0 && allItem.Count == processItems.Count)
                {
                    var purchaseOrder = _purchaseOrder.GetAllPurchaseByPoNumber(allItem.FirstOrDefault().order_number.ToString());
                    foreach (var poItem in purchaseOrder)
                    {
                        var poMaster = _purchaseOrder.GetById(poItem.Id);
                                               List<InvoiceDetails> invoiceDetails = new List<InvoiceDetails>();

                        _invoiceService.InsertMaster(invoiceMaster);
                        var pickslipDetails = _pickSlipService.GetPickSlipMasters(branch.BranchCode, master.PickSlipName).ToList()[0];
                        pickslipDetails.IsProcessed = true;
                        _pickSlipService.Update(pickslipDetails);

                        poMaster.ProcessStatus = true;
                        poMaster.ProcessStatusUpdateDateTime = DateTime.Now;
                        _purchaseOrder.Update(poMaster);
                    }
                }
                // Update Inventory
                foreach (var item in model)
                {
                    AddUpdateStock(item.InventoryId, item.Qty);
                }
               
                return Json(true);
            }
            else
            {
                return Json(false);
            }

        }


        public virtual IActionResult PickSlipList()
        {
            return View();

        }

        [HttpPost]
        public IActionResult PickSlipList(DataSourceRequest request)
        {
            var branch = _workContext.GetCurrentBranch().Result;
            var result = _pickSlipService.GetPickSlipMasters(branch.BranchCode, "", request.Page - 1, request.PageSize);
            var gridData = new DataSourceResult()
            {
                Data = result.Select(x =>
                {
                    WMSWebApp.ViewModels.Pickslip.PickSlipListModel m = new WMSWebApp.ViewModels.Pickslip.PickSlipListModel();
                    m.PickSlipName = x.PickSlipName;
                    m.BranchCode = x.BranchCode;
                    m.Branch = x.BranchCode;
                    m.DockType = x.DockType;
                    m.CreateOn = x.CreateOn;
                    m.Id = x.Id;
                    m.Item = x.PickSlipDetails.Count;

                    return m;
                }),

                Total = result.TotalCount,
            };

            return Json(gridData);
        }

        public IActionResult pickSlipPrint(int id)
        {
            //var pickslip = _pickSlipService.GetbyId(id);
            //if (pickslip.DockType == "Sale PO")
            //{

            //}
            return View(id);
        }
        public virtual IActionResult Test()
        {
            return View();
        }

        #endregion

        #region Utilities
        protected void AddUpdateStock(int inventoryId, int qty)
        {
            var stock = _itemStockService.GetById(inventoryId);
            if (stock != null)
            {
                stock.Qty = stock.Qty - qty;
                stock.LastUpdate = DateTime.Now;
                _itemStockService.Update(stock);
            }

        }
        #endregion


    }
}
