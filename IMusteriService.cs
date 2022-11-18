using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Abstract
{
    public interface IMusteriService
    {
        Task<DataResult<MusteriListDto>> GetAll();
        Task<IResult> Add(MUSTERI musteri);
        Task<DataResult<MusteriDto>> Update(MusteriUpdateDto musteriUpdateDto);
        Task<IDataResult<MusteriUpdateDto>> GetMusteriUpdateDtoAsync(string cariKod);
        Task<DataResult<MusteriDto>> GetById(string cariKod);
        Task<DataResult<MusteriDto>> Login(string email, string password);
        Task<DataResult<MusteriListDto>> PersonelList(string kategori);
        Task<DataResult<MusteriDto>> GetEmail(string email);
       Task<DataResult<MusteriDto>> GetB2CMail(string email);
        Task<DataResult<MusteriDto>> GetMail(string email);
    }
}
