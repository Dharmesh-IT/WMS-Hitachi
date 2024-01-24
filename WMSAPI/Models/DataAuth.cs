using System.Collections.Generic;

namespace WMSAPI.Models
{
    public class DataAuth
    {
       public Root Root { get; set; }
    }

    public class Data
    {
        public string access_token { get; set; }
    }

    public class Root
    {
        public Data data { get; set; }
        public List<object> errors { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
    }
}
