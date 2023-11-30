using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Core;
using Domain.Model.CompanyMaster;

namespace Domain.Model
{
    public class ItemSubItemDb: BaseEntity
    {
        public string SubItemCode { get; set; }
        public string SubItemName { get; set; }
        public int ItemId { get; set; }
        public string MaterialDescription { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
      
    }
}
