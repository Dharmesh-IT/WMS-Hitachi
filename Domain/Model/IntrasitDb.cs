using System;
using WMS.Core;
namespace Domain.Model
{
    public class IntrasitDb : BaseEntity
    {

        public string Login_Branch { get; set; }
        public string Sender_Company { get; set; }
        public string Sender_Branch { get; set; }
        public string PurchaseOrder { get; set; }
        public string Item_Code { get; set; }
        public string SubItem_Code { get; set; }
        public string SubItem_Name { get; set; }
        public string Material_Description { get; set; }
        public string Way_Bill_Number { get; set; }
        public string Line_Item_id { get; set; }
        public int Source_Number { get; set; }
        public string Bucket { get; set; }
        public int Qty { get; set; }
        public string Unit { get; set; }
        public decimal Amt { get; set; }
        public DateTime ETA { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsGrn { get; set; }
        public DateTime Recv_Date { get; set; }

        public bool IsProcessed { get; set; } = false;
        public int PendingToProcessQuantity { get; set; }
        public string request_id { get; set; }
        public string client_uuid { get; set; }
        public string mode_of_transport { get; set; }
        public string agn_type { get; set; }
        public string fulfillment_center_uuid { get; set; }
        public DateTime CreatedDateTimeStamp { get; set; } = DateTime.Now;
    }

    public class ItemSerialDetailsDb :BaseEntity
    {
        public new long Id { get; set; }
        public string Serial_Number { get; set; }
        public int IdInTrasit { get; set; }
    }
}
