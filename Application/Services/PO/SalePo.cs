﻿using Domain.Model.PO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Core.Data;
using WMS.Core;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
namespace Application.Services.PO
{
    public partial class SalePo : ISalePo
    {
        #region Fields
        private readonly IRepository<SalePoDb> _salePoRepository;
        private readonly IRepository<PurchaseOrderDb> _poRepository;

        #endregion

        #region Ctor
        public SalePo(IRepository<SalePoDb> salePoRepository, IRepository<PurchaseOrderDb> poRepository)
        {
            _salePoRepository = salePoRepository; _poRepository = poRepository;
        }
        #endregion
        public void Insert(SalePoDb salePoDb)
        {
            _salePoRepository.Insert(salePoDb);
        }

        public void Update(SalePoDb salePoDb)
        {
            _salePoRepository.Update(salePoDb);
        }
        public virtual SalePoDb GetById(int id)
        {
            return _salePoRepository.GetById(id);
        }

        public virtual SalePoDb GetSalesPoForPrint(string poNumber)
        {
            var query = from x in _salePoRepository.Table
                        select x;

            if (poNumber != "0")
            {
                query = query.Where(x => x.order_number.ToString() == poNumber);
            }
           
            var result = new PagedList<SalePoDb>(query, 0, int.MaxValue);
            return result.DistinctBy(x=>x.invoiceNumber).FirstOrDefault();
        }
        public virtual IPagedList<SalePoDb> GetSalePos(string poNumber = "0", bool isProcessed = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from x in _salePoRepository.Table
                        select x;

            if (poNumber != "0")
            {
                query = query.Where(x => x.order_number.ToString() == poNumber);
            }
            query = query.Where(x => x.IsProcessed == isProcessed);
            query = query.OrderByDescending(x => x.Id);

            var result = new PagedList<SalePoDb>(query, pageIndex, pageSize);
            return result;
        }
        public virtual IPagedList<SalePoDb> GetAllItemSalePos(string poNumber = "0", int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from x in _salePoRepository.Table
                        select x;

            if (poNumber != "0")
            {
                query = query.Where(x => x.order_number.ToString() == poNumber);
            }

            query = query.OrderByDescending(x => x.Id);

            var result = new PagedList<SalePoDb>(query, pageIndex, pageSize);
            return result;
        }

        public IPagedList<SalePoDb> GetDetails(string category, string branchCode, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from x in _poRepository.Table
                        join y in _salePoRepository.Table
                        on x.PONumber equals y.order_number.ToString()
                        where x.POCategory == category && x.BranchCode == branchCode 
                        select y;
            var result = new PagedList<SalePoDb>(query, pageIndex, pageSize);
            return result;
        }


        public dynamic GetInvoiceBase64Data(string invoiceNumber)
        {
            var query = from x in _salePoRepository.Table
                        where x.invoiceNumber == invoiceNumber
                        select new { x.invoice_url };
            var result = new PagedList<dynamic>(query, 0, int.MaxValue);
            return result.Select(x=>x.invoice_url).ToList().FirstOrDefault();


        }


        public virtual IPagedList<SalePoDb> GetAllSalePo(string status, string branchCode, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from x in _poRepository.Table
                        join y in _salePoRepository.Table
                        on x.PONumber equals y.PONumber
                        where x.POCategory == "Sale PO" && x.BranchCode == branchCode
                        select y;
            if (status == "ALL")
            {

            }
            else if (status == "Done")
            {
                query = query.Where(x => x.IsProcessed == true);
            }
            else
            {
                query = query.Where(x => x.IsProcessed == false);
            }

            query = query.OrderByDescending(x => x.Id);

            return new PagedList<SalePoDb>(query, pageIndex, pageSize);
        }

        public virtual IPagedList<SalePoDb> GetAllSalePoForDispatch(string branchCode, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from x in _salePoRepository.Table
                        join y in _poRepository.Table
                        on x.order_number.ToString() equals y.PONumber
                        where y.POCategory == "Sale PO" && y.BranchCode == branchCode && y.ProcessStatus == true
                        select x;
            return new PagedList<SalePoDb>(query, pageIndex, pageSize);
            
        }
        
    }
}
