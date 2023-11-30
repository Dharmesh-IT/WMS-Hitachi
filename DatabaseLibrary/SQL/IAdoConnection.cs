using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseLibrary.SQL;
public interface IAdoConnection
{
    DataTable GetDatatableFromSqlWithSP(string SPName, List<SqlParameter> parameterlist = null);
    DataSet GetMutipleDatatableFromSqlWithSP(string SPName, List<SqlParameter> parameterlist = null);
    int InsertUpdateWithSP(string spName, List<SqlParameter> parameterlist);

    DataTable bulkImport(string SPName, DataTable dataTable, DateTime recvDate, string loginBranch);
    DataTable bulkImport(string SPName, DataTable dataTable, DataTable dtSerialMapping, DateTime recvDate, string loginBranch, string companyName);
    int Delete(string SPName, List<SqlParameter> parameterlist);
}
