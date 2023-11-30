using ServiceReference1;
using System;
using System.Security.Policy;
using System.ServiceModel;
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
                var endpoint = new EndpointAddress("https://hcserve.com/ILogisticWebAPI_UAT/DeliveryDetails.svc/authentication");

                DeliveryDetailsClient client = new DeliveryDetailsClient(binding, endpoint);
                DeliverySerializationcl_UserAuthentication auth = new DeliverySerializationcl_UserAuthentication();
                var authRequest = new
                {
                    username = "NWSKR",
                    password = "NWSKR@23",
                };
                string encReques = EncryptDecrypt.Encrypt(authRequest.ToString(), "4e57534b5240313233", "4e57534b5240313233");
                auth.userauthenticationdata = encReques;
                var authToekn = await client.getUserAuthenticateAsync(auth);
            }
            catch (Exception ex)
            {

            }


        }
    }
}
