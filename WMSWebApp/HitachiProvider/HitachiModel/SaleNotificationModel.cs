using System;
using System.Collections.Generic;

namespace WMSWebApp.HitachiProvider.HitachiModel
{
    public class SaleNotificationModel
    {
        public List<SaleOrder> orders { get; set; } = new List<SaleOrder>();
    }
    public class SaleOrder
    {
        public string order_number { get; set; }
        public string fulfillment_center { get; set; }
        public string status { get; set; }

        public List<SaleShipment> shipments { get; set; } = new List<SaleShipment>();
        public object extras { get; set; } = new object();

    }
    public class SaleShipment
    {
        public string shipment_number { get; set; }
        public string waybill { get; set; }
        public string created_at { get; set; }
        public string courier { get; set; }
        public string dispatched_at { get; set; }
        public string delivered_at { get; set; }
        public object extras { get; set; } = new object();
        public List<SaleOrderLine> order_lines { get; set; } = new List<SaleOrderLine>();
    }

    public class SaleOrderLine
    {
        public string order_line_number { get; set; }

        public List<SaleProduct> products { get; set; } = new List<SaleProduct>();
    }

    public class SaleProduct
    {
        public string prod_sku { get; set; }
        public int prod_qty { get; set; }

        public object extras { get; set; } = new object();
    }
}
