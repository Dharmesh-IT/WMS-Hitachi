using System.Collections.Generic;

namespace WMSAPI.ViewModels
{
    public class SaleOrderViewModel
    {
        public Data data { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Consignee
    {
        public string name { get; set; }
        public string address_line1 { get; set; }
        public string pin_code { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string primary_phone_number { get; set; }
    }

    public class Data
    {
        public List<Order> orders { get; set; }
    }

    public class InvoiceView
    {
        public string invoice_number { get; set; }
        public string payment_mode { get; set; }
        public int total_price { get; set; }
        public int cod_amount { get; set; }
        public string invoice_url { get; set; }
    }

    public class Order
    {
        public string order_number { get; set; }
        public string order_date { get; set; }
        public string order_type { get; set; }
        public string channel { get; set; }
        public List<Shipment> shipments { get; set; }
        public Consignee consignee { get; set; }
    }

    public class OrderLine
    {
        public string number { get; set; }
        public string product_sku { get; set; }
        public string orderline_bucket { get; set; }
        public int quantity { get; set; }
        public string client_id { get; set; }
        public InvoiceView invoice { get; set; }
    }

    public class DataRoot
    {
        public Data data { get; set; }
    }

    public class Shipment
    {
        public int number { get; set; }
        public string fc { get; set; }
        public InvoiceView invoice { get; set; }
        public List<OrderLine> order_lines { get; set; }
    }

}
