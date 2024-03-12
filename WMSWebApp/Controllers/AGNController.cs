using Application.GenericServices;
using Application.Services;
using Application.Services.Master;
using Application.Services.PO;
using Application.Services.User;
using Domain.Model;
using Domain.Model.PO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WMS.Data;
using WMSWebApp.Filters;
using WMSWebApp.Models;
using WMSWebApp.ViewModels;
using Data = WMSWebApp.Models.Data;
using Root = WMSWebApp.Models.Root;

namespace WMSWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AGNController : BaseAdminController
    {
        private readonly IIntrasitHelper _intrasitHelper;
        private IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserProfileService _userProfileService;
        private readonly IPurchaseOrder _purchaseOrder;
        private readonly IUserService _userservice;
        public AGNController(IIntrasitHelper intrasitHelper, IConfiguration iconfig, SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, IUserProfileService userProfileService, IPurchaseOrder purchaseOrder, IUserService userService)
        {
            _configuration = iconfig;
            _signInManager = signInManager;
            _userManager = userManager;
            _userProfileService = userProfileService;
            _purchaseOrder = purchaseOrder;
            _userservice = userService;
            _intrasitHelper = intrasitHelper;
        }
        [NonAction]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [TypeFilter(typeof(CustomAuthorization))]
        public async Task<IActionResult> Post([FromBody] AGNRequestData request)
        {

            string authKey = "4e57534b52403132";
            string authValue = "4e57534b52403132";
            dynamic response = null;
            try
            {
                _userservice.InsertHitachiData(2, request.agncreationdata);
                if (request.agncreationdata == null)
                {
                    response = new
                    {
                        status = "FAILURE",
                        request_id = "",
                        message = "Operation Unsuccessful. AGN created data is null",
                        status_code = HttpStatusCode.OK
                    };
                }
                else
                {
                    var data = GenericMethods.DecryptClassObject(request.agncreationdata, authKey, authValue);
                    var agnDetails = JsonConvert.DeserializeObject<AGNViewModel>(data);
                    var requestGuid = Guid.NewGuid();
                    if (agnDetails != null)
                    {
                        if (agnDetails.products != null && agnDetails.products.Count > 0)
                        {
                            foreach (var item in agnDetails.products)
                            {
                                if(agnDetails.source_number > 0)
                                {
                                    var intransit = new IntrasitDb();
                                    intransit.Source_Number = agnDetails.source_number;
                                    intransit.client_uuid = agnDetails.client_uuid;
                                    intransit.fulfillment_center_uuid = agnDetails.fulfillment_center_uuid;
                                    intransit.mode_of_transport = agnDetails.mode_of_transport;
                                    intransit.ETA = agnDetails.expected_arrival_date;
                                    intransit.agn_type = agnDetails.agn_type;
                                    intransit.Qty = item.quantity;
                                    intransit.PendingToProcessQuantity = item.quantity;
                                    intransit.SubItem_Code = item.sku;
                                    intransit.Line_Item_id = item.extras!.line_itemid;
                                    intransit.request_id = requestGuid.ToString();

                                    _intrasitHelper.CreateNewIntrasitSPFromAGN(intransit);
                                }
                            }
                            //_purchaseOrder.Insert(new PurchaseOrderDb
                            //{
                            //    POCategory = "StockTransfer PO",
                            //    PODate = agnDetails.expected_arrival_date,
                            //    PONumber = agnDetails.source_number.ToString(),
                            //    BranchCode = agnDetails.fulfillment_center_uuid,
                            //    ProcessStatus = false
                            //});
                            response = new
                            {
                                status = "SUCCESS",
                                request_id = requestGuid.ToString(),
                                message = "Operation Successful. Please Check status of request id after 20 minutes.",
                                status_code = HttpStatusCode.OK
                            };
                        }
                        else
                        {
                            response = new
                            {
                                status = "Failure",
                                request_id = "",
                                message = "Operation Not Successful. Please Check AGN Details. AGN products details is null.",
                                status_code = HttpStatusCode.OK
                            };
                        }
                    }
                    else
                    {
                        response = new
                        {
                            status = "Failure",
                            request_id = "",
                            message = "Operation Not Successful. Please Check AGN Details. AGN Details is null.",
                            status_code = HttpStatusCode.OK
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                response = new
                {
                    status = "FAILURE",
                    request_id = "",
                    message = "Operation Unsuccessful. Please Check status of request id after 20 minutes.",
                    status_code = HttpStatusCode.InternalServerError
                };
            }
            return Ok(response);
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
                    root.message = "Login successful";
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
            catch (Exception ex)
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
