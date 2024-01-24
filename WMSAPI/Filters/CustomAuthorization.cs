using Application.GenericServices;
using Application.Services.User;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMSAPI.Models;

namespace WMSAPI.Filters
{
    public class CustomAuthorization : IAsyncAuthorizationFilter
    {
        private IConfiguration _configuration;
        private IUserService _userService;
        public CustomAuthorization(IConfiguration iconfig, IUserService userService) {
            _configuration = iconfig;
            _userService = userService;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            string userauthenticationresponsedata = string.Empty;
            try
            {
                string authKey = _configuration.GetValue<string>("Credential:authKey");
                string authValue = _configuration.GetValue<string>("Credential:authValue");
                DataAuth dataAuth = new DataAuth();
                Root root = new Root();
                Data data1 = new Data();
                var header = context.HttpContext.Request.Headers;
                if (header.Any(x => x.Key.Equals(HeaderNames.Authorization)))
                {
                    var accessToken = header[HeaderNames.Authorization].FirstOrDefault(x=>x.StartsWith("Bearer ")).Split("Bearer ")[1];
                    if (accessToken != null)
                    {
                        var keybytes = Encoding.UTF8.GetBytes(authKey);// 4e57534b52403132
                        var iv = Encoding.UTF8.GetBytes(authKey);//
                        var data = GenericMethods.Decrypt(accessToken, authKey, authValue);
                        var result = JsonConvert.DeserializeObject<DataAuth>(data);
                        var handler = new JwtSecurityTokenHandler();
                        var jsonToken = handler.ReadToken(result.Root.data.access_token);
                        var tokenS = jsonToken as JwtSecurityToken;
                        var id = tokenS.Claims.FirstOrDefault(x => x.Type == "id").Value;
                        var name = tokenS.Claims.FirstOrDefault(x => x.Type == "name").Value;
                        var resultUser = _userService.VerifyUser(name.Split(" ")[0], name.Split(" ")[1]);
                        return;
                    }
                    else
                    {
                        data1.access_token = "";
                        root.data = data1;
                        root.success = false;
                        root.message = "Login failed";
                        dataAuth.Root = root;
                        var jsonValue = System.Text.Json.JsonSerializer.Serialize(dataAuth);
                        var result1 = EncryptDecrypt.Encrypt(jsonValue, authKey, authValue);
                        userauthenticationresponsedata = result1;
                    }
                }
                else
                {
                    data1.access_token = "";
                    root.data = data1;
                    root.success = false;
                    root.message = "Login failed";
                    dataAuth.Root = root;
                    var jsonValue = System.Text.Json.JsonSerializer.Serialize(dataAuth);
                    var result1 = EncryptDecrypt.Encrypt(jsonValue, authKey, authValue);
                    userauthenticationresponsedata = result1;
                }
            }
            catch (System.Exception ex)
            {

                throw;
            }

            context.Result = new ContentResult() {StatusCode = 200, Content = userauthenticationresponsedata };
            return;
        }
    }
}
