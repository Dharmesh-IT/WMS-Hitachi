using Application.GenericServices;
using Application.Services.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMSAPI.Models;

namespace WMSAPI.Filters
{
    public class CustomAuthorization : IAsyncAuthorizationFilter
    {
        private IUserService _userService;
        public CustomAuthorization(IUserService userService) {
            _userService = userService;
        }
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            string userauthenticationresponsedata = string.Empty;
            try
            {
                var header = context.HttpContext.Request.Headers;
                if (header.Any(x => x.Key.Equals(HeaderNames.Authorization)))
                {
                    var authHeader = header[HeaderNames.Authorization].FirstOrDefault(x => x.StartsWith("Bearer "));
                    if (authHeader != null)
                    {
                        var accessToken = authHeader.Split("Bearer ")[1];
                        if (accessToken != null)
                        {
                            var keybytes = Encoding.UTF8.GetBytes("4e57534b52403132");// 4e57534b52403132
                            var iv = Encoding.UTF8.GetBytes("4e57534b52403132");//
                            var token = GenericMethods.DecryptClassObject(accessToken, "4e57534b52403132", "4e57534b52403132");
                            var handler = new JwtSecurityTokenHandler();
                            var jsonToken = handler.ReadToken(token);
                            var tokenS = jsonToken as JwtSecurityToken;
                            var id = tokenS.Claims.FirstOrDefault(x => x.Type == "id").Value;
                            var name = tokenS.Claims.FirstOrDefault(x => x.Type == "name").Value;
                            var resultUser = _userService.VerifyUser(name.Split(" ")[0], name.Split(" ")[1]);
                            if (resultUser == null || resultUser.Count <= 0)
                            {
                                context.Result = new UnauthorizedObjectResult("Username or Password is not correct.");
                            }

                            return Task.CompletedTask;
                        }
                        else
                        {
                            context.Result = new UnauthorizedObjectResult("Username or Password is not correct.");
                            return Task.CompletedTask;
                        }
                    }
                    else
                    {
                        context.Result = new UnauthorizedObjectResult("Access Token not Found.");
                        return Task.CompletedTask;
                    }
                }
                else
                {
                    context.Result = new UnauthorizedObjectResult("No Header Found.");
                    return Task.CompletedTask;
                }
            }
            catch (System.Exception ex)
            {
                context.Result = new UnauthorizedObjectResult("Something went wrong!!! Please try again later.");
                return Task.CompletedTask;
            }
        }
    }
}
