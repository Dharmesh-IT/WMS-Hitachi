using System.Collections.Generic;

namespace Application.Services.User
{
    public interface IUserService
    {
        public List<Domain.Model.User> VerifyUser (string username, string password);
        public void InsertHitachiData(int type, string data);

        public string GetHitachiData(int id);
    }
}
