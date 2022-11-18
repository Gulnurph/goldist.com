using System;
using System.Linq;
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
    public class SiparisManager : ISiparisService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGeciciSiparisService _geciciSiparisService;

        public SiparisManager(IMapper mapper, IUnitOfWork unitOfWork, IGeciciSiparisService geciciSiparisService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _geciciSiparisService = geciciSiparisService;
        }
        public async Task<DataResult<double?>> TotalPrice(int id)
        {
            var result = await _unitOfWork.SiparisDal.GetAllAsync(s => s.SIPARISNO == id);
            if (result.Count > -1)
            {
                return new DataResult<double?>(ResultStatus.Success, Messages.Added,
                    result.Select(c => c.FIYAT * c.ADET).Sum());
            }

            return new DataResult<double?>(ResultStatus.Error, Messages.Failed, null);

        }


        public async Task<DataResult<SiparisListDto>> OrderDetailList(int siparisId)
        {
            var result = await _unitOfWork.SiparisDal.GetAllAsync(s => s.SIPARISNO == siparisId);
            if (result.Count > -1)
            {
                return new DataResult<SiparisListDto>(ResultStatus.Success, Messages.Added, new SiparisListDto
                {
                    Siparises = result,
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Listed
                });
            }
            return new DataResult<SiparisListDto>(ResultStatus.Error, Messages.NotFound, new SiparisListDto
            {
                Siparises = null,
                ResultStatus = ResultStatus.Success,
                Messages = Messages.Listed
            });
        }


        public async Task<IResult> AddOrders(MUSTERI customer, int siparisNo)
        {
            var shoppingCart = await _geciciSiparisService.CartList(customer);

            if (shoppingCart.ResultStatus == ResultStatus.Success || shoppingCart.ResultStatus == ResultStatus.Info)
            {
                foreach (var cartItem in shoppingCart.Data.GeciciSiparisList)
                {
                    var orderAddDto = new SiparisAddDto
                    {
                        SIPARISNO = siparisNo,
                        CARIKOD = customer.CARIKODU,
                        CARIADI = customer.KISAUNVAN,
                        KOD = cartItem.KOD,
                        PARTI = cartItem.PARTI,
                        GRAM = cartItem.GRAM,
                        ADET = cartItem.ADET,
                        ACIKLA1 = cartItem.ACIKLAMA2,
                        TARIH = DateTime.Now,
                        RESIMX = cartItem.RESIM,
                        FIYAT = cartItem.FIYAT,
                        TUTAR = cartItem.TUTAR,
                        TUR = cartItem.TUR,
                        DOVIZ = cartItem.DOVIZ,
                        TESLIMTARIHI = DateTime.Now.AddDays(3),
                        URUNTUR = cartItem.TURU,
                        USERID = customer.KISAUNVAN,
                        URUNID = cartItem.URUNID,
                        ORIJINALDOVIZ = cartItem.DOVIZ,
                        ORIJINALTUTAR = cartItem.TUTAR,
                        MILYEM = cartItem.MILYEM,
  
                    };
                    
                    var order = _mapper.Map<SIPARIS>(orderAddDto);
                    await _unitOfWork.SiparisDal.AddAsync(order);
                    await _unitOfWork.SaveAsync();

                }
                var result = await _geciciSiparisService.DeleteAll(customer);
                if (result.ResultStatus == ResultStatus.Error)
                {
                    return new Result(ResultStatus.Error, Messages.Failed);
                }
                return new Result(ResultStatus.Success, "Siparişler eklendi ve Geçici Siparişler silindi");
            }
            return new Result(ResultStatus.Success, Messages.NotFound);
        }
     
        public async Task<DataResult<SiparisListDto>> GetByCari(string carikod)
        {
            var results = await _unitOfWork.SiparisDal.GetAllAsync(s => s.CARIKOD == carikod);
            if (results.Count > 0)
            {
                return new DataResult<SiparisListDto>(ResultStatus.Success, Messages.Listed, new SiparisListDto
                {
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Listed,
                    Siparises = results
                });

            }
            return new DataResult<SiparisListDto>(ResultStatus.Error, Messages.NotFound, new SiparisListDto
            {
                ResultStatus = ResultStatus.Error,
                Messages = Messages.NotFound,
                Siparises = null

            });
        }

        public async Task<DataResult<SiparisListDto>> GetAll()
        {
            var results = await _unitOfWork.SiparisDal.GetAllAsync();
            if (results.Count>0)
            {
                return new DataResult<SiparisListDto>(ResultStatus.Success, Messages.Listed, new SiparisListDto
                {
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Listed,
                    Siparises = results
                });
            }
           
            return new DataResult<SiparisListDto>(ResultStatus.Error, Messages.NotFound, new SiparisListDto
            {
                ResultStatus = ResultStatus.Error,
                Messages = Messages.NotFound,
                Siparises = null
            });
        }








        //public async Task Delete(int siparisId)
        //{
        //    var result = await _unitOfWork.SiparisDal.AnyAsync(a => a.SIRANO == siparisId);
        //    if (result)
        //    {
        //        var siparis = await _unitOfWork.SiparisDal.GetAsync(x => x.SIRANO == siparisId);

        //        await _unitOfWork.SiparisDal.DeleteAsync(siparis).ContinueWith(t => _unitOfWork.SaveAsync());

        //    }

        //}

        //public async Task<IList<SIPARIS>> GetAll()
        //{
        //    return await _unitOfWork.SiparisDal.GetAllAsync();
        //}
        //public async Task<SIPARIS> GetById(int siparisId)
        //{
        //    return await _unitOfWork.SiparisDal.GetAsync(s => s.SIRANO == siparisId);
        //}

        //public async Task Update(SIPARIS siparis)
        //{
        //    await _unitOfWork.SiparisDal.UpdateAsync(siparis).ContinueWith(t => _unitOfWork.SaveAsync()); ;
        //}

    }
}
