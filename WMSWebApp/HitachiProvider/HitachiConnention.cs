using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WMSWebApp.Models;
using WMSWebApp.Wrapper;
namespace WMSWebApp.HitachiProvider
{
    public class HitachiConnention : IHitachiConnection
    {
        private readonly IMemoryCacheWrapper _memoryCacheWrapper;
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static string accessToken = "accessToken";
        public HitachiConnention(IMemoryCacheWrapper memoryCacheWrapper)
        {
            _memoryCacheWrapper = memoryCacheWrapper;
        }

        public async Task<String> SendGRNToHitachi(string request)
        {


            var login =await Auth();
            // Add Sending GRN data with token

            return "";




        }

        public async Task<LoginResponse> Auth()
        {
            try
            {
                var options = new RestClientOptions("https://hcserve.com/ILogisticWebAPI_UAT/DeliveryDetails.svc/authenticate")
                {
                    MaxTimeout = -1,
                };

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                    SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var client = new RestClient(options);
                RestRequest rest = new RestRequest();


                byte[] keybytes = Encoding.UTF8.GetBytes("4e57534b52403132");
                byte[] iv = Encoding.UTF8.GetBytes("4e57534b52403132");
                var authRequest = new Auth();
                authRequest.password = "NWSKR@23";
                authRequest.username = "NWSKR";
                string jsonAuth = JsonConvert.SerializeObject(authRequest);
                byte[] encrypted = Encryption.EncryptStringToBytes(jsonAuth.ToString(), keybytes, iv);
                string encryprtedRequest = Convert.ToBase64String(encrypted);

                rest.AddHeader("transportername", "NWSKR");
                rest.AddHeader("content-type", "application/json");
                rest.AddParameter("application/json", JsonConvert.SerializeObject(new { userauthenticationdata = encryprtedRequest }), ParameterType.RequestBody);

                var response = await client.ExecutePostAsync(rest);
                var authResponse = JsonConvert.DeserializeObject<AuthResponse>(response.Content);
                var base64EncodedBytes = System.Convert.FromBase64String(authResponse.userauthenticationresponsedata);

                var jsonResponse = Encryption.DecryptStringFromBytes(base64EncodedBytes, keybytes, iv);
                LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(jsonResponse);
                _memoryCacheWrapper.Set(accessToken, loginResponse.data.access_token,
                    (epoch.AddMilliseconds(loginResponse.data.expires_in) - DateTime.UtcNow).TotalMinutes);
                return loginResponse;


            }
            catch (Exception ex)
            {
                string errorMessage = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                Console.WriteLine("error " + errorMessage);
                await Auth();
                throw;
            }
        }

        public async Task<HitachiResponse> SubmitGrnNotification(string grnObject)
        {
            try
            {
                var options = new RestClientOptions("https://hcserve.com/ILogisticWebAPI_UAT/DeliveryDetails.svc/grn-notification")
                {
                    MaxTimeout = -1,
                };

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                    SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var client = new RestClient(options);
                RestRequest rest = new RestRequest();
                rest.AddHeader("authorization", GetEncryptedString(await GetAccessToken()));
                rest.AddHeader("transportername", "NWSKR");
                rest.AddHeader("content-type", "application/json");
                byte[] keybytes = Encoding.UTF8.GetBytes("4e57534b52403132");
                byte[] iv = Encoding.UTF8.GetBytes("4e57534b52403132");
                byte[] encrypted = Encryption.EncryptStringToBytes(grnObject.ToString(), keybytes, iv);
                string encryprtedRequest = Convert.ToBase64String(encrypted);
                rest.AddParameter("application/json", JsonConvert.SerializeObject
                    (
                    new { grnnotificationdata = encryprtedRequest
                    }), ParameterType.RequestBody);

                var response = await client.ExecutePostAsync(rest);
                var hitachiResponse = JsonConvert.DeserializeObject<HitachiResponse>(response.Content);
              
                return hitachiResponse;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                Console.WriteLine("error " + errorMessage);
                throw;
            }
        }

        public async Task<HitachiResponse> SubmitOrderNotification(string orderObject)
        {
            try
            {
                var options = new RestClientOptions("https://hcserve.com/ILogisticWebAPI_UAT/DeliveryDetails.svc/order-notification")
                {
                    MaxTimeout = -1,
                };

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                    SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var client = new RestClient(options);
                RestRequest rest = new RestRequest();
                rest.AddHeader("authorization", GetEncryptedString(await GetAccessToken()));
                rest.AddHeader("content-type", "application/json");
                rest.AddHeader("transportername", "NWSKR");
                byte[] keybytes = Encoding.UTF8.GetBytes("4e57534b52403132");
                byte[] iv = Encoding.UTF8.GetBytes("4e57534b52403132");
                byte[] encrypted = Encryption.EncryptStringToBytes(orderObject.ToString(), keybytes, iv);
                string encryprtedRequest = Convert.ToBase64String(encrypted);
                rest.AddParameter("application/json", JsonConvert.SerializeObject
                    (
                    new
                    {
                        ordernotificationdata = encryprtedRequest
                    }), ParameterType.RequestBody);

                var response = await client.ExecutePostAsync(rest);
                var hitachiResponse = JsonConvert.DeserializeObject<HitachiResponse>(response.Content);

                return hitachiResponse;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
                Console.WriteLine("error " + errorMessage);
                throw;
            }
        }

        private async Task<string> GetAccessToken()
        {
            
            if (!_memoryCacheWrapper.TryGetValue(accessToken, out string tempAccessToken))
            {
              LoginResponse loginResponse =  await Auth();
              tempAccessToken = loginResponse.data.access_token;
            }

            return tempAccessToken;
        }

        private string GetEncryptedString(string objectString)
        {
            byte[] keybytes = Encoding.UTF8.GetBytes("4e57534b52403132");
            byte[] iv = Encoding.UTF8.GetBytes("4e57534b52403132");
            byte[] encrypted = Encryption.EncryptStringToBytes(objectString, keybytes, iv);
            return Convert.ToBase64String(encrypted);
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
        public AccessTokenDetail data { get; set; }
        public object errors { get; set; }
        public string message { get; set; }
        public bool success { get; set; }

    }
    public class AccessTokenDetail
    {
        public string access_token { get; set; }
        public Int64 expires_in { get; set; }
    }

    public class HitachiResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("status_code")]
        public HttpStatusCode StatusCode { get; set; }
    }
}
