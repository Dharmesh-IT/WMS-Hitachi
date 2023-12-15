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

        public async Task SendGRNToHitachi(string request)
        {
            try
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                binding.MaxBufferSize = int.MaxValue;
                binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.AllowCookies = true;
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                binding.TransferMode = System.ServiceModel.TransferMode.Buffered;
               // binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.;
                var endpoint = new EndpointAddress("https://hcserve.com/ILogisticWebAPI_UAT/DeliveryDetails.svc/authenticate");

                DeliveryDetailsClient client = new DeliveryDetailsClient(binding, endpoint);
                DeliverySerializationcl_UserAuthentication auth = new DeliverySerializationcl_UserAuthentication();
                var authRequest = new
                {
                    username = "NWSKR",
                    password = "NWSKR@23",
                };
                byte[] keybytes = Encoding.UTF8.GetBytes("4e57534b52403132");// 4e57534b52403132
                byte[] iv = Encoding.UTF8.GetBytes("4e57534b52403132");//

                byte[] encrypted = Encryption.EncryptStringToBytes(authRequest.ToString(), keybytes, iv);
                auth.userauthenticationdata = Convert.ToBase64String(encrypted);
                var authToekn = await client.getUserAuthenticateAsync(auth);
            }
            catch (Exception ex)
            {

            }


        }
    }
}
