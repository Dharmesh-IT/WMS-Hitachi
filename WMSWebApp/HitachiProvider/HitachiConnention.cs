using Newtonsoft.Json;
using RestSharp;
using ServiceReference1;
using System;
using System.Security.Policy;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WMSWebApp.Models;
namespace WMSWebApp.HitachiProvider
{
    public class HitachiConnention
    {

        public async Task<String> SendGRNToHitachi(string request)
        {
           

            var login = Auth();
            // Add Sending GRN data with token

            return "";




        }

        async Task<LoginResponse> Auth()
        {
            var options = new RestClientOptions("https://hcserve.com")
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            RestRequest rest = null;


            byte[] keybytes = Encoding.UTF8.GetBytes("4e57534b52403132");
            byte[] iv = Encoding.UTF8.GetBytes("4e57534b52403132");
            var authRequest = new Auth();
            authRequest.password = "NWSKR@23";
            authRequest.username = "NWSKR";
            string jsonAuth = JsonConvert.SerializeObject(authRequest);
            byte[] encrypted = Encryption.EncryptStringToBytes(jsonAuth.ToString(), keybytes, iv);
            string encryprtedRequest = Convert.ToBase64String(encrypted);
            rest = new RestRequest("/ILogisticWebAPI_UAT/DeliveryDetails.svc/authenticate", Method.Post);
            Request body = new Request();
            body.userauthenticationdata = encryprtedRequest;
            string jsonRequest = JsonConvert.SerializeObject(body);


            rest.AddHeader("transportername", "NWSKR");
            rest.AddHeader("Content-Type", "application/json");

            rest.AddStringBody(jsonRequest, DataFormat.Json);
            RestResponse response = await client.ExecuteAsync(rest);

            var authResponse = JsonConvert.DeserializeObject<AuthResponse>(response.Content);
            var base64EncodedBytes = System.Convert.FromBase64String(authResponse.userauthenticationresponsedata);

            var jsonResponse = Encryption.DecryptStringFromBytes(base64EncodedBytes, keybytes, iv);
            LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(jsonResponse);
            return loginResponse;
        }
    }
    public class Request
    {
        public string userauthenticationdata { get; set; }
    }
    public class Auth
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    public class AuthResponse
    {
        public string userauthenticationresponsedata { get; set; }
    }
    public class LoginResponse
    {
        public string data { get; set; }
        public string access_token { get; set; }

        public int expires_in { get; set; }
        public object errors { get; set; }
        public string message { get; set; }
        public bool success { get; set; }

    }
}
