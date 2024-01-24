using System.Collections.Generic;

namespace Application.Services.User
{
    public interface IUserService
    {
        public List<Domain.Model.User> VerifyUser (string username, string password);
    }
}
