using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMSWebApp.ViewModels
{
    public class Intrasitc
    {
        public int Id { get; set; }
        public string LoginBranch { get; set; }
        [Display(Name = "Company")]
        public string SenderCompany { get; set; }
        [Display(Name = "Fulfillment Center")]
        public string Branch { get; set; }
        [Display(Name = "Invoice Number")]
        public string PurchaseOrder { get; set; }
        public string ItemCode { get; set; }
        public string SubItemCode { get; set; }
        public string SubItemName { get; set; }
        [Display(Name = "Material Description")]
        public string MaterialDescription { get; set; }
        public int Qty { get; set; }
        public string Unit { get; set; }
        public decimal Amt { get; set; }
        public DateTime ETA { get; set; }
        public bool IsDeleted { get; set; }
        public int Sno { get; set; }
        public bool AllowGRN { get; set; }

        public string subItemCodeVal { get; set; }
        public DateTime Recv_Date { get; set; }
        public string Way_Bill_Number { get; set; }
        public string Line_Item_id { get; set; }
        public int Source_Number { get; set; }

        public string SerialNumbers { get; set; }
        public string Bucket { get; set; }

    }
}
