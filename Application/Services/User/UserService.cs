using DatabaseLibrary.SQL;
using System.Collections.Generic;
using System.Data.SqlClient;
using Application.Common;
using System.Data;
using DatabaseLibrary;
using System.Linq;
using System;
namespace Application.Services.User
{
    public class UserService : IUserService
    {
        private readonly IAdoConnection _adoConnection;

        public UserService(IAdoConnection adoConnection)
        {
            _adoConnection = adoConnection;
        }
        public List<Domain.Model.User> VerifyUser(string username, string password)
        {
            List<SqlParameter> sqlParameters =  new List<SqlParameter>()
                {
                    //new SqlParameter("@CompanyId", Item.CompanyId),
                    new SqlParameter("@Username", username),
                    new SqlParameter("@Password", password),
                };
            var  result = _adoConnection.GetDatatableFromSqlWithSP(Constants.VerifyUser, sqlParameters);
            var emp = (from DataRow row in result.Rows
                       select new Domain.Model.User
                       {
                           id = row["Id"].ToString(),
                           firstName = row["FirstName"].ToString(),
                           lastName = row["LastName"].ToString(),
                           userId = row["UserId"].ToString()


                       }).ToList();
            return emp;
        }

        public void InsertHitachiData(int type,string data)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>()
                {
                    //new SqlParameter("@CompanyId", Item.CompanyId),
                    new SqlParameter("@type", type),
                    new SqlParameter("@value", data),
                     new SqlParameter("@createdDateTime", DateTime.Now)
                };
            var result = _adoConnection.InsertUpdateWithSP(Constants.insertRecord, sqlParameters);
          }
        public string GetHitachiData(int id)
        {
            List<SqlParameter> sqlParameters = new List<SqlParameter>()
                {
                    //new SqlParameter("@CompanyId", Item.CompanyId),
                    new SqlParameter("@id", id)
                };
            var result = _adoConnection.GetDatatableFromSqlWithSP("[WMS].[GetHitachiData]", sqlParameters);
            var emp = (from DataRow row in result.Rows
                       select new 
                       {
                           id = row["Hitachidata"].ToString(),
                          


                       }).ToList();
            return emp[0].id;
        }

    }
}
