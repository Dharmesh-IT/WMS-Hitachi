using Application.GenericServices;
using Application.Services.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WMSAPI.Filters;
using WMSAPI.Models;
using WMSAPI.ViewModels;

namespace WMSAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private IConfiguration _configuration;
        private IUserService _userService;
        public AuthenticateController(IConfiguration iconfig, IUserService userService)
        {
            _configuration = iconfig;
            _userService = userService;
        }


        [HttpGet]
        [Route("CustomAuth")]
        [TypeFilter(typeof(CustomAuthorization))]
        public IActionResult CustomAuth()
        {
            return Ok("");
        }

        [HttpPost]

        public IActionResult Index([FromBody]UserAuthenticationData userauthenticationdata)
        {
           // var dataAuth = new DataAuth();
            var root = new Models.Root();
            var data1 = new Models.Data();
            string authKey = "4e57534b52403132";
            string authValue = "4e57534b52403132";
            var keybytes = Encoding.UTF8.GetBytes(authKey);// 4e57534b52403132
            var iv = Encoding.UTF8.GetBytes(authKey);//
            string userauthenticationresponsedata;
            try
            {
                _userService.InsertHitachiData(1, userauthenticationdata.userauthenticationdata);
                var data = GenericMethods.DecryptClassObject(userauthenticationdata.userauthenticationdata, authKey, authValue);
                var login = JsonConvert.DeserializeObject<Login>(data);
                var result = _userService.VerifyUser(login.username, login.password);

                if (result.Count > 0)
                {
                    var authClaims = new List<Claim>
                    {
                        new(ClaimTypes.Name, result[0].firstName),
                        new(ClaimTypes.NameIdentifier, result[0].userId),
                        new("name", result[0].firstName+" "+result[0].lastName),
                        new("id", result[0].id),
                        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };
                    var token = GetToken(authClaims);
                    data1.access_token = new JwtSecurityTokenHandler().WriteToken(token);
                    root.data = data1;
                    root.success = true;
                    root.message = "Login successful";
                   
                    var jsonValue = JsonConvert.SerializeObject(root);
                    string encryprtedResponse = Convert.ToBase64String(Encryption.EncryptStringToBytes(jsonValue, keybytes, iv));
                    userauthenticationresponsedata = encryprtedResponse;
                }
                else
                {
                    // Root root = new Root();
                    data1.access_token = "";
                    root.data = data1;
                    root.success = false;
                    root.message = "Login failed";
                    var jsonValue = JsonConvert.SerializeObject(root);
                    string encryprtedResponse = Convert.ToBase64String(Encryption.EncryptStringToBytes(jsonValue, keybytes, iv));
                    userauthenticationresponsedata = encryprtedResponse;
                }
            }
            catch (Exception ex)
            {
                data1.access_token = "";
                root.data = data1;
                root.success = false;
                root.message = "Login failed";
                var jsonValue = JsonConvert.SerializeObject(root);
                string encryprtedResponse = Convert.ToBase64String(Encryption.EncryptStringToBytes(jsonValue, keybytes, iv));
                userauthenticationresponsedata = encryprtedResponse;
            }
            return Ok(new { userauthenticationresponsedata });
        }

        protected JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMonths(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }
        [HttpPost]
        [Route("GetEncryptedString")]
        public string GetEncryptedString([FromBody] object login,int type,string tempText)
        {
            string jsonAuth = string.Empty;
            Login loginObj = null;
            AGNViewModel agnObj = null;
            SaleOrderViewModel saleOrderObj = null;
            switch (type)
            {
                case 1:
                    loginObj = JsonConvert.DeserializeObject<Login>(login.ToString());
                    jsonAuth = JsonConvert.SerializeObject(loginObj);
                    break;
                case 2:
                    agnObj = JsonConvert.DeserializeObject<AGNViewModel>(login.ToString());
                    jsonAuth = JsonConvert.SerializeObject(agnObj);
                    break;
                case 3:
                    saleOrderObj = JsonConvert.DeserializeObject<SaleOrderViewModel>(login.ToString());
                    jsonAuth = JsonConvert.SerializeObject(saleOrderObj);
                    break;
                case 4:
                    jsonAuth = tempText;
                    break;

            }
            byte[] keybytes = Encoding.UTF8.GetBytes("4e57534b52403132");
            // byte[] iv = Encoding.UTF8.GetBytes("4e57534b52403132");
           
            byte[] encrypted = Encryption.EncryptStringToBytes(jsonAuth.ToString(), keybytes, keybytes);
            string encryprtedRequest = Convert.ToBase64String(encrypted);
            return encryprtedRequest;
        }


        [HttpPost]
        [Route("GetDecryptedString")]
        public string GetDecryptedString(string key)
        {
            return GenericMethods.Decrypt(key, "4e57534b52403132", "4e57534b52403132");
        }


    }
}
