using Application.GenericServices;
using Application.Services.Master;
using Domain.Model.PO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WMS.Core.Data;
using WMS.Data;
using WMSAPI.Filters;
using WMSAPI.Models;
using WMSAPI.ViewModels;
using Data = WMSAPI.Models.Data;
using Root = WMSAPI.Models.Root;

namespace WMSAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
    public class SaleOrderController : BaseAdminController
    {
        private readonly IRepository<SalePoDb> _salePoRepository;
        private IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserProfileService _userProfileService;
        public SaleOrderController(IRepository<SalePoDb> salePoRepository, IConfiguration iconfig, SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, IUserProfileService userProfileService)
        {
            _salePoRepository = salePoRepository;
            _configuration = iconfig;
            _signInManager = signInManager;
            _userManager = userManager;
            _userProfileService = userProfileService;
        }
        [NonAction]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [TypeFilter(typeof(CustomAuthorization))]
        public IActionResult Post(string salesordercreationdata)
        {
            string authKey = _configuration.GetValue<string>("Credential:authKey");
            string authValue = _configuration.GetValue<string>("Credential:authValue");
            SaleOrderViewModel saleOrderViewModel = new SaleOrderViewModel();
            // HttpResponseMessage response = new HttpResponseMessage();
            dynamic response;
            if (saleOrderViewModel == null)
            {
                response = new
                {
                    status = "FAILURE",
                    request_id = "",
                    message = "Operation Unsuccessful. Please Check status of request id after 20 minutes.",
                    status_code = HttpStatusCode.InternalServerError
                };
            }
            else
            {
                var requestGuid = Guid.NewGuid();
                var data = GenericMethods.DecryptClassObject(salesordercreationdata, authKey, authValue);
                var saleOrderDetails = JsonSerializer.Deserialize<SaleOrderViewModel>(data);
                if (saleOrderDetails.data.orders.Count > 0)
                {
                   
                    foreach (var item in saleOrderDetails.data.orders)
                    {
                        foreach (var i in item.shipments)
                        {
                            foreach (var j in i.order_lines)
                            {
                                SalePoDb salePoDb = new SalePoDb();
                                salePoDb.request_id = requestGuid.ToString();
                                salePoDb.order_number = item.order_number;
                                salePoDb.order_date = item.order_date;
                                salePoDb.order_type = item.order_type;
                                salePoDb.channel = item.channel;
                                salePoDb.shipments_number = i.number;
                                salePoDb.shipments_fc = i.fc;
                                //salePoDb.invoice_number = i.invoice.invoice_number;
                                salePoDb.invoiceNumber = i.invoice.invoice_number;
                                salePoDb.payment_mode = i.invoice.payment_mode;
                                salePoDb.total_price = i.invoice.total_price;
                                salePoDb.cod_amount = i.invoice.cod_amount;
                                salePoDb.invoice_url = i.invoice.invoice_url;
                                salePoDb.orderline_number = j.number;
                                salePoDb.product_sku = j.product_sku;
                                salePoDb.orderline_bucket = j.orderline_bucket;
                                salePoDb.quantity = j.quantity;
                                salePoDb.client_id = j.client_id;
                                salePoDb.invoice_payment_mode = j.invoice.payment_mode;
                                salePoDb.invoice_total_price = j.invoice.total_price;
                                salePoDb.invoice_cod_amount = j.invoice.cod_amount;
                                salePoDb.consignee_name = item.consignee.name;
                                salePoDb.consignee_address_line1 = item.consignee.address_line1;
                                salePoDb.consignee_pin_code = item.consignee.pin_code;
                                salePoDb.consignee_city = item.consignee.city;
                                salePoDb.consginee_state = item.consignee.state;
                                salePoDb.consginee_country = item.consignee.country;
                                salePoDb.consginee_primary_phone_number = item.consignee.primary_phone_number;
                                _salePoRepository.Insert(salePoDb);
                            }
                        }

                    }
                }

                response = new
                {
                    status = "SUCCESS",
                    request_id = requestGuid.ToString(),
                    message = "Operation Successful. Please Check status of request id after 20 minutes.",
                    status_code = HttpStatusCode.OK
                };
            }

            return Ok(response);
            // rest of the code
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("CheckExistingUser")]
        public async Task<string> CheckExistingUser(string userauthenticationdata)
        {
            string userauthenticationresponsedata = string.Empty;
            //string test = "{ username = NWSKR, password = NWSKR@23 }";
            //string userName = test.Split(",")[0].Split("=")[1].Trim();
            //string passWord = test.Split(",")[1].Split("=")[1].Split("}")[0].Trim();
            DataAuth dataAuth = new DataAuth();
            Root root = new Root();
            Data data1 = new Data();
            string authKey = _configuration.GetValue<string>("Credential:authKey");
            string authValue = _configuration.GetValue<string>("Credential:authValue");

            try
            {
              
                byte[] keybytes = Encoding.UTF8.GetBytes(authKey);// 4e57534b52403132
                byte[] iv = Encoding.UTF8.GetBytes(authKey);//
                var data = GenericMethods.DecryptClassObject(userauthenticationdata, authKey, authValue);
                string username = data.Split(",")[0].Split("=")[1].Trim();
                string password = data.Split(",")[1].Split("=")[1].Split("}")[0].Trim();


                var result = await _signInManager.PasswordSignInAsync(username, password, false, false);
               
                if (result.Succeeded)
                {
                    var loggedinUser = await _userManager.FindByEmailAsync(username);
                    var userProfile = _userProfileService.GetByUserId(loggedinUser.Id);
                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userProfile.FirstName),
                    new Claim(ClaimTypes.NameIdentifier, userProfile.UserId),
                    new Claim("name", userProfile.FirstName+" "+userProfile.LastName),
                    new Claim("id", userProfile.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                    var token = GetToken(authClaims);
                    data1.access_token = new JwtSecurityTokenHandler().WriteToken(token);
                    root.data = data1;
                    root.success = true;
                    root.message= "Login successful";
                    dataAuth.Root = root;
                    var jsonValue = System.Text.Json.JsonSerializer.Serialize(dataAuth);
                    var result1 = EncryptDecrypt.Encrypt(jsonValue, authKey, authValue);
                    userauthenticationresponsedata = result1;
                }
                else
                {
                    // Root root = new Root();
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
            catch(Exception ex)
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
            return userauthenticationresponsedata;
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
    }

}

