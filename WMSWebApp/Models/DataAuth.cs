﻿using System.Collections.Generic;

namespace WMSWebApp.Models
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
        public object _metadata { get; set; } = new object();
        public Data data { get; set; }
        public List<object> errors { get; set; } = new List<object>{ };
        public string message { get; set; }
        public bool success { get; set; }
    }

   
}