using System;

namespace WMSWebApp.ViewModels.PO
{
    public class PolistViewModel
    {
        public int Id { get; set; }
        public string PoNumber { get; set; }
        public string stockTransferPOCatagry { get; set; }
        public string stockTransferPoSendingTo { get; set; }
        public string stockTransferPoItem { get; set; }
        public string subItemCode { get; set; }
        public string stockTransferPoSubitem { get; set; }
        public string stockTransferPoQty { get; set; }
        public string stockTransferPoAmt { get; set; }
        public string stockTransferPoSerialNumber { get; set; }
        public string serviceCategory { get; set; }
        public string salePo { get; set; }
        public string saleDate { get; set; }
        public string ServiceRequestNumber { get; set; }
        public string invNumber { get; set; }

        //new changes
        public int source_number { get; set; }
        public string client_uuid { get; set; }
        public string fulfillment_center_uuid { get; set; }
        public string mode_of_transport { get; set; }
        public DateTime expected_arrival_date { get; set; }
        public string agn_type { get; set; }
        public int quantity { get; set; }
        public string sku { get; set; }
        public string line_itemid { get; set; }
    }
    public class SaleViewModel
    {
        public int Id { get; set; }
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

        public string orderline_number { get; set; }

        public string product_sku { get; set; }

        public string orderline_bucket { get; set; }

        public int quantity { get; set; }
        public string client_id { get; set; }
        public bool isInvoice { get; set; }

        public string invoice_payment_mode { get; set; }

        public decimal invoice_total_price { get; set; }

        public decimal invoice_cod_amount { get; set; }
        public string consigneeAddress { get; set; }
        public string consignee_name { get; set; }
        public string consignee_address_line1 { get; set; }
        public string consignee_pin_code { get; set; }
        public string consignee_city { get; set; }
        public string consignee_state { get; set; }
        public string consignee_country { get; set; }
        public string consignee_primary_phone_number { get; set; }
    }
}
