using Domain.Model;
using System;
using System.Linq;
using WMS.Core;
using WMS.Core.Data;
namespace Application.Services.GRN
{
    public class IntrasitService : IIntrasitService
    {
        #region Fields
        private readonly IRepository<IntrasitDb> _intrasitRepository;
        private readonly IRepository<ItemSerialDetailsDb> _itemSerialDetailsRepository;

        #endregion

        #region Ctor
        public IntrasitService(IRepository<IntrasitDb> intrasitRepository, IRepository<ItemSerialDetailsDb> itemSerialDetails)
        {
            _intrasitRepository = intrasitRepository;
            _itemSerialDetailsRepository = itemSerialDetails;
        }
        #endregion

        #region Methods

        public virtual (IPagedList<IntrasitDb> intrasitResult, IPagedList<ItemSerialDetailsDb> itemSerialDetailResult)
            GetPendingPO(string branchCode, string pono, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(pono))
                {
                    var queryItemSerialDetails = from x in _itemSerialDetailsRepository.Table
                                                 select x;
                    var query = from x in _intrasitRepository.Table
                                 select x;
                    query = query.Where(x => x.Login_Branch == branchCode && x.IsGrn == false);

                    if (pono == "0")
                    {

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(pono))
                        {
                            query = query.Where(x => x.PurchaseOrder.Contains(pono));
                        }
                    }
                    var resultItemDetails = new PagedList<ItemSerialDetailsDb>(queryItemSerialDetails,pageIndex,pageSize);
                    var result = new PagedList<IntrasitDb>(query, pageIndex, pageSize);
                    return (result, resultItemDetails);
                }
                else
                {
                    return (null, null);
                }

            }
            catch (Exception e)
            {
                return (null,null);
            }

        }

        public virtual IPagedList<IntrasitDb> GetDonePO(string branchCode, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from x in _intrasitRepository.Table
                        select x;
            query = query.Where(x => x.Login_Branch == branchCode && x.IsGrn == true);

            query = query.OrderByDescending(x => x.Id);
            return new PagedList<IntrasitDb>(query, pageIndex, pageSize);
        }

        public virtual IntrasitDb GetById(int id)
        {
            return _intrasitRepository.GetById(id);
        }

        public virtual void Update(IntrasitDb entitiy)
        {
            _intrasitRepository.Update(entitiy);
        }
        #endregion


    }
}
