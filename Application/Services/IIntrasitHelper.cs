using Domain.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IIntrasitHelper
    {
        bool DeleteIntrasitById(int Id);
        List<IntrasitDb> GetAllIntrasit();
        IntrasitDb GetIntrasitById(int Id);
        bool UpdateIntrasitById(IntrasitDb intrasit);
        bool CreateNewIntrasit(IntrasitDb intrasit);
        void blukUpload(DataTable ds, DateTime recvDate, string loginBranch);
        void blukUpload(DataTable dtFinalData, DataTable dtSerialMapping, DateTime recvDate, string loginBranch, string companyName);

        List<Branch> GetAllBranches();

        List<CompanyDb> GetAllCompany();

        List<ItemDb> GetAllItem();
        List<SubItemDb> GetSubItem(int subItemId);

        string GetSubItemTitle(string subName);

        List<ItemSubItemDb> GetItemSubItemDetails();
        List<ItemSubItemDb> GetItemSubItemDetails(string subItemCode);

        List<InventoryStockItemDetails> GetItemDetailsForPickSlip(string subItemCode, int warehouseId);
        bool CreateNewIntrasitSPFromAGN(IntrasitDb intrasit);

    }
}
