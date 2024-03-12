using System;

namespace WMSWebApp.ViewModels.GRN
{
    public class GrnItemListModel
    {
        public int Id { get; set; }
        public string POId { get; set; }

        public string ItemCode { get; set; }
        public string SubItemCode { get; set; }
        public string SubItemName { get; set; }
        public string MaterialDescription { get; set; }
        public int Qty { get; set; }
        public string Unit { get; set; }
        public decimal Amount { get; set; }
        public int AreaId { get; set; }
        public string Location { get; set; }
        public int InventoryId { get; set; }
        public string Address1 { get; set; }
        public string DockType { get; set; }
        public string mode_of_transport { get; set; }
        public string agn_type { get; set; }
        public string line_itemId { get; set; }
        public string invoiceNumber { get; set; }
        public DateTime expected_arrival_date { get; set; }
    }
}
