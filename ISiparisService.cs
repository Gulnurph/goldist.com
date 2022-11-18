using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Abstract
{
    public interface ISiparisService
    {

   
        Task<IResult> AddOrders(MUSTERI customer, int siparisNo);
        Task<DataResult<double?>> TotalPrice(int id);
        Task<DataResult<SiparisListDto>> GetByCari(string carikod);
        Task<DataResult<SiparisListDto>> OrderDetailList(int siparisId);
        Task<DataResult<SiparisListDto>> GetAll();

    }
}
