using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.ComplexTypes;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Concrete
{
    public class MusteriManager : IMusteriService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MusteriManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IResult> Add(MUSTERI musteri)
        {
            await _unitOfWork.MusteriDal.AddAsync(musteri);
            await _unitOfWork.SaveAsync();
         
            return new Result(ResultStatus.Success,Messages.Added);
            
        }
        public async Task<IDataResult<MusteriUpdateDto>> GetMusteriUpdateDtoAsync(string cariKod)
        {
            var result = await _unitOfWork.MusteriDal.AnyAsync(c => c.CARIKODU == cariKod);
            if (result)
            {
                var musteri = await _unitOfWork.MusteriDal.GetAsync(c => c.CARIKODU == cariKod);
                var musteriUpdateDto = _mapper.Map<MusteriUpdateDto>(musteri);
                return new DataResult<MusteriUpdateDto>(ResultStatus.Success, musteriUpdateDto);
            }
            else
            {
                return new DataResult<MusteriUpdateDto>(ResultStatus.Error, Messages.NotFound, null);
            }
        }


        public async Task<DataResult<MusteriListDto>> GetAll()
        {
            var result= await _unitOfWork.MusteriDal.GetAllAsync();
            if (result.Count>-1)
            {
                return new DataResult<MusteriListDto>(ResultStatus.Success,Messages.Listed,new MusteriListDto
                {
                    Messages = Messages.Listed,
                    Musteris = result,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<MusteriListDto>(ResultStatus.Error, Messages.NotFound, new MusteriListDto
            {
                Messages = Messages.Listed,
                Musteris = null,
                ResultStatus = ResultStatus.Success
            });
        }

        public async Task<DataResult<MusteriDto>> GetById(string cariKod)
        {
            var result= await _unitOfWork.MusteriDal.GetAsync(m => m.CARIKODU == cariKod);
            if (result!=null)
            {
                return new DataResult<MusteriDto>(ResultStatus.Success,Messages.Listed,new MusteriDto
                {
                    Musteri = result,
                    Messages = Messages.Listed,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<MusteriDto>(ResultStatus.Error, Messages.Failed, new MusteriDto
            {
                Musteri = null,
                Messages = Messages.NotFound,
                ResultStatus = ResultStatus.Error
            });
        }
      
        public async Task<DataResult<MusteriDto>> Login(string email, string password)
        {
            var result= await _unitOfWork.MusteriDal.GetAsync(m => m.WEBMAIL == email && m.WEBSIFRE == password);
            if (result != null)
            {
                if (String.IsNullOrEmpty(result.DOVIZ))
                {
                    result.DOVIZ = "TRL";
                }
                return new DataResult<MusteriDto>(ResultStatus.Success, Messages.Listed, new MusteriDto
                {
                    Musteri = result,
                    Messages = Messages.Listed,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<MusteriDto>(ResultStatus.Error, Messages.Failed, new MusteriDto
            {
                Musteri = null,
                Messages = Messages.NotFound,
                ResultStatus = ResultStatus.Error
            });
        }


       

        public async Task<DataResult<MusteriListDto>> PersonelList(string kategori)
        {
            var result= await _unitOfWork.MusteriDal.GetAllAsync(p => p.KATAGORI == "PERSONEL");
            if (result.Count > -1)
            {
                return new DataResult<MusteriListDto>(ResultStatus.Success, Messages.Listed, new MusteriListDto
                {
                    Messages = Messages.Listed,
                    Musteris = result,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<MusteriListDto>(ResultStatus.Error, Messages.NotFound, new MusteriListDto
            {
                Messages = Messages.Listed,
                Musteris = null,
                ResultStatus = ResultStatus.Success
            });
        }

        public async Task<DataResult<MusteriDto>> GetEmail(string email)
        {
            var result = await _unitOfWork.MusteriDal.GetAsync(m => m.WEBMAIL == email);
            if (result != null)
            {
                return new DataResult<MusteriDto>(ResultStatus.Success, Messages.Listed, new MusteriDto
                {
                    Musteri = result,
                    Messages = Messages.Listed,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<MusteriDto>(ResultStatus.Error, Messages.Failed, new MusteriDto
            {
                Musteri = null,
                Messages = Messages.NotFound,
                ResultStatus = ResultStatus.Error
            });
        }

       

        public async Task<DataResult<MusteriDto>> Update(MusteriUpdateDto musteriUpdateDto)
        {
            var oldMusteri = await _unitOfWork.MusteriDal.GetAsync(c => c.SIRANO == musteriUpdateDto.SIRANO);
            var musteri = _mapper.Map<MusteriUpdateDto, MUSTERI>(musteriUpdateDto, oldMusteri);
           var result=await _unitOfWork.MusteriDal.UpdateAsync(musteri);
            await _unitOfWork.SaveAsync();
           return new DataResult<MusteriDto>(ResultStatus.Success,Messages.Updated,new MusteriDto
           {
               Musteri = result,
               ResultStatus = ResultStatus.Success,
               Messages = Messages.Updated
           });
           
        }

        public async Task<DataResult<MusteriDto>> GetB2CMail(string email)
        {
            var result = await _unitOfWork.MusteriDal.GetAsync(m => m.EMAIL == email);
            if (result != null)
            {
                return new DataResult<MusteriDto>(ResultStatus.Success, Messages.Listed, new MusteriDto
                {
                    Musteri = result,
                    Messages = Messages.Listed,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<MusteriDto>(ResultStatus.Error, Messages.Failed, new MusteriDto
            {
                Musteri = null,
                Messages = Messages.NotFound,
                ResultStatus = ResultStatus.Error
            });
        }
        public async Task<DataResult<MusteriDto>> GetMail(string email)
        {
            var result = await _unitOfWork.MusteriDal.GetAsync(m => m.EMAIL == email);
            if (result != null)
            {
                return new DataResult<MusteriDto>(ResultStatus.Success, Messages.Listed, new MusteriDto
                {
                    Musteri = result,
                    Messages = Messages.Listed,
                    ResultStatus = ResultStatus.Success
                });
            }
            return new DataResult<MusteriDto>(ResultStatus.Error, Messages.Failed, new MusteriDto
            {
                Musteri = null,
                Messages = Messages.NotFound,
                ResultStatus = ResultStatus.Error
            });
        }
        //public async Task Delete(int musteriId)
        //{
        //    var result = await _unitOfWork.MusteriDal.AnyAsync(a => a.SIRANO == musteriId);
        //    if (result)
        //    {
        //        var musteri = await _unitOfWork.MusteriDal.GetAsync(x => x.SIRANO == musteriId);

        //        await _unitOfWork.MusteriDal.DeleteAsync(musteri).ContinueWith(t => _unitOfWork.SaveAsync());

        //    }

        //}
        //public async Task<MUSTERI> CheckToEmailAndPhone(string email, string phone)
        //{
        //    return await _unitOfWork.MusteriDal.GetAsync(m => m.WEBMAIL == email && m.TEL1 == phone);
        //}

    }
}
