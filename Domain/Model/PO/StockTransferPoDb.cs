using System;
using WMS.Core;

namespace Domain.Model.PO
{
    public class StockTransferPoDb: BaseEntity
    {
        public string PONumber { get; set; }
        public string StockTransferPOCategory { get; set; }
        public string StockTransferPOSendingTo { get; set; }
        public string StockTransferPOItem { get; set; }
        public string StockTransferPOSubItem { get; set; }
        public int StockTransferPOQty { get; set; }
        public string StockTransferPOAmt { get; set; }
        public string StockTransferPOSerialNumber { get; set; }
        public string SubItemCode { get; set; }
        public bool IsProcessed { get; set; } = false;

        public string request_id { get; set; }

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
}
