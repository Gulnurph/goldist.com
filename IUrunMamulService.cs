using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using Entities.ComplexType;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Abstract
{
    public interface IUrunMamulService
    {
        Task<DataResult<UrunMamulDto>> GetById(int id);
        Task<IDataResult<UrunMamulModelListDto>> YeniUrunler();
        Task<DataResult<UrunMamulDto>> GetUrunmamul(string kod);
        Task<DataResult<CollectionListDto>> GetAllCollectionInStockAsync();
        Task<DataResult<UrunMamulListDto>> GetAll();
        Task<DataResult<UrunMamulListDto>> GetAllSirali(string cinsId, string tur);
        Task<DataResult<UrunMamulModelListDto>> GetAllTur(string tur);
        //Task<DataResult<UrunmamulwithTurListDto>> TurList(bool inStock);
        Task<DataResult<CollectionListDto>> GetAllCollectionAsync();
        Task<DataResult<UrunMamulWithCinsListDto>> CinsListByCollection(string aile);
        Task<double> IndirimHesaplama(double urunFiyat, double? indirimOran, string musteriDoviz, string urunDoviz);
        Task<double> UrunFiyat(double urunFiyat, string musteriDoviz, string urunDoviz);
        Task<DataResult<UrunMamulListDto>> GetByCins(string cins);
        Task<DataResult<UrunMamulWithCinsListDto>> CinsList(string tur);
        Task<DataResult<ProductdetailListDto>> UrunMamulFiltrele(UrunMamulFiltre Filtre);
        Task<double> UrunFiyatDoviz(double urunFiyat, string urunDoviz, string parametreDoviz);
      
        Task<List<URUNMAMULMODEL>> UrunmamulsInStock(Func<URUNMAMULMODEL, bool> filter = null);
        Task<DataResult<UrunMamulModelListDto>> UrunListesi();
        Task<DataResult<UrunMamulModelListDto>> LatestList();
        Task<IResult> Add(URUNMAMUL Urunmamul);
        Task<IResult> Update(URUNMAMUL Urunmamul);
        Task<DataResult<UrunMamulDto>> Delete(int id);
        Task<DataResult<UrunMamulModelListDto>> SearchList(string search);
        Task<DataResult<UrunMamulModelListDto>> SearchListInStock(string search);
        //Task<DataResult<IEnumerable<URUNMAMUL>>> GetByAileList();


    }
}