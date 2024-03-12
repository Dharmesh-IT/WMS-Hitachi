using Domain.Model.PO;
using WMS.Core;
namespace Application.Services.PO
{
    public interface ISalePo
    {
        void Insert(SalePoDb salePoDb);
        void Update(SalePoDb salePoDb);

        SalePoDb GetSalesPoForPrint(string poNumber);
        IPagedList<SalePoDb> GetSalePos(string poNumber = "0", bool isProcessed = false, int pageIndex = 0, int pageSize = int.MaxValue);

        IPagedList<SalePoDb> GetDetails(string category, string branchCode, int pageIndex = 0, int pageSize = int.MaxValue);
        SalePoDb GetById(int id);
        IPagedList<SalePoDb> GetAllSalePo(string status, string branchCode, int pageIndex = 0, int pageSize = int.MaxValue);
        IPagedList<SalePoDb> GetAllItemSalePos(string poNumber = "0", int pageIndex = 0, int pageSize = int.MaxValue);
        IPagedList<SalePoDb> GetAllSalePoForDispatch(string branchCode, int pageIndex = 0, int pageSize = int.MaxValue);
        dynamic GetInvoiceBase64Data(string invoiceNumber);
    }
}