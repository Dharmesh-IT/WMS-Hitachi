using System;
using System.Collections.Generic;

namespace WMSAPI.ViewModels
{
    
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Extras
    {
        public string line_itemid { get; set; }
    }

    public class Product
    {
        public int quantity { get; set; }
        public string sku { get; set; }
        public Extras extras { get; set; }
    }

    public class AGNViewModel
    {
        public string source_number { get; set; }
        public string client_uuid { get; set; }
        public string fulfillment_center_uuid { get; set; }
        public string mode_of_transport { get; set; }
        public DateTime expected_arrival_date { get; set; }
        public string agn_type { get; set; }
        public List<Product> products { get; set; }
    }


}
