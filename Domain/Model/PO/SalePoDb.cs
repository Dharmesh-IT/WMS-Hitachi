using System;
using WMS.Core;

namespace Domain.Model.PO
{
    public  class SalePoDb : BaseEntity
    {
        public string PONumber { get; set; }
        public string SalePOCategory { get; set; }
        public string SalePOSendingTo { get; set; }
        public string SalePOItem { get; set; }
        public string SalePOSubItem { get; set; }
        public int SalePOQty { get; set; }
        public string SalePOAmt { get; set; }
        public string SalePOSerialNumber { get; set; }
        public string SubItemCode { get; set; }
        public bool IsProcessed { get; set; }
        public string request_id { get; set; }
        public int order_number { get; set; }
        public string order_date { get; set; }

        public string order_type { get; set; }

        public string channel { get; set; }
        public int shipments_number { get; set; }
        public string shipments_fc { get; set; }

        public string invoiceNumber { get; set; }
        public string payment_mode { get; set; }
        public decimal total_price { get; set; }

        public decimal cod_amount { get; set; }

        public string invoice_url { get; set; }

        public string orderline_number { get; set; }

        public string product_sku { get; set; }

        public string orderline_bucket { get; set; }

        public int quantity { get; set; }
        public string client_id { get; set; }

        public string invoice_payment_mode { get; set; }

        public decimal invoice_total_price { get; set; }

        public decimal invoice_cod_amount { get; set; }

        public string consignee_name { get; set; }
        public string consignee_address_line1 { get; set; }
        public string consignee_pin_code { get; set; }
        public string consignee_city { get; set; }
        public string consignee_state { get; set; }
        public string consignee_country { get; set; }
        public string consignee_primary_phone_number { get; set; }
        public DateTime SaleRequestInsertedDateTime { get; set; }
    }
}
