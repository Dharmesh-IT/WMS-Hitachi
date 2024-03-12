using System;
using WMS.Core;

namespace Domain.Model.PO
{
    public class PurchaseOrderDb : BaseEntity
    {
        public DateTime PODate { get; set; }

        public string PONumber { get; set; }

        public string POCategory { get; set; }
        public string BranchCode { get; set; }
        public bool ProcessStatus { get; set; } = false;
        public int shipments_number { get; set; }
        public DateTime POInsertedDateTime { get; set; }
        public DateTime? ProcessStatusUpdateDateTime { get; set; }

    }
}
