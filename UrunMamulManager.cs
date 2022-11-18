using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.ComplexTypes;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.ComplexType;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.Extensions.Caching.Memory;
using X.PagedList;


namespace Business.Concrete
{
    public class UrunMamulManager : IUrunMamulService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKurlarService _kurlarService;
        private readonly IMemoryCache _memoryCache;
       

        public UrunMamulManager( IUnitOfWork unitOfWork, IKurlarService kurlarService, IMemoryCache memoryCache)
        {
          
            _unitOfWork = unitOfWork;
            _kurlarService = kurlarService;
            _memoryCache = memoryCache;
        }
        public async Task<DataResult<UrunMamulDto>> GetUrunmamul(string kod)
        {
            var result = await _unitOfWork.UrunMamulDal.GetAsync(x => x.KOD == kod );
            if (result != null)
            {
                return new DataResult<UrunMamulDto>(ResultStatus.Success, Messages.Exist, new UrunMamulDto
                {
                    Urunmamul = result,
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Listed
                });
            }
            return new DataResult<UrunMamulDto>(ResultStatus.Error, Messages.NotFound, new UrunMamulDto
            {
                Urunmamul = null,
                ResultStatus = ResultStatus.Error,
                Messages = Messages.NotFound
            });
        }


        private async Task<IList<URUNMAMUL>> CacheUrunmamul()
        {
            const string keyCollection = "Collection";
            IList<URUNMAMUL> urunmamuls;
            
            if (_memoryCache.TryGetValue(keyCollection, out object list))
            {
                urunmamuls = (IList<URUNMAMUL>)list;
            }
            else
            {   
                urunmamuls = await _unitOfWork.UrunMamulDal.GetAllAsync(u => u.ETIKET > 0 && u.WEB2 == "Y"   );


                 _memoryCache.Set(keyCollection, urunmamuls, new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(20),
                    Priority = CacheItemPriority.Normal,
                    
                });
            }

            return urunmamuls;
        }

        public async Task<DataResult<UrunMamulListDto>> GetAll()
        {
            var result = await CacheUrunmamul();


            if (result.Count > -1)
            {
                return new DataResult<UrunMamulListDto>(ResultStatus.Success, Messages.Listed, new UrunMamulListDto
                {
                    UrunMamulList = result,
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Listed
                });
            }
            return new DataResult<UrunMamulListDto>(ResultStatus.Error, Messages.NotFound, new UrunMamulListDto
            {
                UrunMamulList = null,
                ResultStatus = ResultStatus.Error,
                Messages = Messages.NotFound
            });
        }

        //public async Task<DataResult<UrunmamulwithTurListDto>> TurList(bool inStock)
        //{
        //    var urunmamul = await CacheUrunmamul();

        //    var stokdurdal = await _unitOfWork.StokDurDal.GetAllAsync();
        //    if (inStock == false)
        //    {

        //        var resulturun = (from u in urunmamul
        //                          where u.TUR != ""
        //                          group u by u.TUR
        //            into tur
        //                          select new UrunMamulwithTur
        //                          {
        //                              TUR = tur.Key,
        //                              RESIM = tur.Key + ".jpg",
        //                              TOTALCOUNT = tur.Count(),
        //                          }).OrderByDescending(t => t.TOTALCOUNT).ToList();

        //        if (resulturun.Count > -1)
        //        {
        //            return new DataResult<UrunmamulwithTurListDto>(ResultStatus.Success, Messages.Listed, new UrunmamulwithTurListDto
        //            {
        //                UrunMamulwithTurs = resulturun,
        //                ResultStatus = ResultStatus.Success,
        //                Messages = Messages.Listed
        //            });
        //        }
        //        return new DataResult<UrunmamulwithTurListDto>(ResultStatus.Error, Messages.NotFound, new UrunmamulwithTurListDto
        //        {
        //            UrunMamulwithTurs = null,
        //            ResultStatus = ResultStatus.Error,
        //            Messages = Messages.NotFound
        //        });

        //    }

        //    var resultstokurun = (from u in urunmamul
        //                          join stk in stokdurdal on new { urunkod = u.KOD, parti = (int)u.PARTI } equals
        //                              new { urunkod = stk.URUNKOD, parti = stk.PARTI }
        //                          select new
        //                          {
        //                              stk,
        //                              u
        //                          }
        //                          into us
        //                          where us.stk.AGIREN - us.stk.ACIKAN > 0
        //                          group us by us.u.TUR
        //                          into tur
        //                          select new UrunMamulwithTur
        //                          {
        //                              TUR = tur.Key,
        //                              RESIM = tur.Key + ".jpg",
        //                              TOTALCOUNT = tur.Count(),
        //                          }).OrderByDescending(t => t.TOTALCOUNT).ToList();
        //    if (resultstokurun.Count > -1)
        //    {
        //        return new DataResult<UrunmamulwithTurListDto>(ResultStatus.Success, Messages.Listed, new UrunmamulwithTurListDto
        //        {
        //            UrunMamulwithTurs = resultstokurun,
        //            ResultStatus = ResultStatus.Success,
        //            Messages = Messages.Listed
        //        });
        //    }
        //    return new DataResult<UrunmamulwithTurListDto>(ResultStatus.Error, Messages.NotFound, new UrunmamulwithTurListDto
        //    {
        //        UrunMamulwithTurs = null,
        //        ResultStatus = ResultStatus.Error,
        //        Messages = Messages.NotFound
        //    });



        //}

        public async Task<DataResult<CollectionListDto>> GetAllCollectionInStockAsync()
        {
            var urunmamul = await CacheUrunmamul();

            var stokdurdal = await _unitOfWork.StokDurDal.GetAllAsync();
            var resultstokaile = (from u in urunmamul
                                  join stk in stokdurdal on new { urunkod = u.KOD, parti = (int)u.PARTI } equals
                                      new { urunkod = stk.URUNKOD, parti = stk.PARTI }
                                  select new
                                  {
                                      stk,
                                      u
                                  }
                into us
                                  where us.stk.AGIREN - us.stk.ACIKAN > 0 && us.u.AILE != "" 
                                  group us by us.u.AILE
                into aile
                                  select new Collection
                                  {
                                      AILE = aile.Key,
                                      RESIM = aile.Key + ".jpg",
                                      TOTALCOUNT = aile.Count()
                                  }).OrderByDescending(u => u.TOTALCOUNT);

            var result = await resultstokaile.ToListAsync();
            if (result.Count > -1)
            {
                return new DataResult<CollectionListDto>(ResultStatus.Success, Messages.Listed, new CollectionListDto
                {
                    CollectionList = result,
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Listed
                });
            }
            return new DataResult<CollectionListDto>(ResultStatus.Error, Messages.NotFound, new CollectionListDto
            {
                CollectionList = null,
                ResultStatus = ResultStatus.Error,
                Messages = Messages.NotFound
            });
        }
        public async Task<DataResult<CollectionListDto>> GetAllCollectionAsync()
        {
            var urunmamul = await CacheUrunmamul();

            var res = (from u in urunmamul.Where(x => x.AILE != "")
                       group u by u.AILE
                into aile
                       select new Collection
                       {
                           AILE = aile.Key,
                           RESIM = aile.Key + ".jpg",
                           TOTALCOUNT = aile.Count()
                       }).OrderByDescending(u => u.TOTALCOUNT);
            var result = await res.ToListAsync();
            if (result.Count > -1)
            {
                return new DataResult<CollectionListDto>(ResultStatus.Success, Messages.Listed, new CollectionListDto
                {
                    CollectionList = result,
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Listed
                });
            }
            return new DataResult<CollectionListDto>(ResultStatus.Error, Messages.NotFound, new CollectionListDto
            {
                CollectionList = null,
                ResultStatus = ResultStatus.Error,
                Messages = Messages.NotFound
            });
        }

        public async Task<DataResult<UrunMamulWithCinsListDto>> CinsListByCollection(string aile)
        {
            var urunmamuls = await CacheUrunmamul();

            var result = await (from u in urunmamuls
                                where u.AILE == aile
                                group u by u.CINS
                into cins
                                select new UrunMamulWithCins
                                {
                                    CINS = cins.Key,
                                    TOTALCOUNT = cins.Count()
                                }).ToListAsync();
            if (result.Count > -1)
            {
                return new DataResult<UrunMamulWithCinsListDto>(ResultStatus.Success, Messages.Listed, new UrunMamulWithCinsListDto
                {
                    UrunMamulWithCins = result,
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Listed
                });
            }
            return new DataResult<UrunMamulWithCinsListDto>(ResultStatus.Error, Messages.NotFound, new UrunMamulWithCinsListDto
            {
                UrunMamulWithCins = null,
                ResultStatus = ResultStatus.Error,
                Messages = Messages.NotFound
            });
        }

        public async Task<double> IndirimHesaplama(double urunFiyat, double? indirimOran, string musteriDoviz, string urunDoviz)
        {
            var musteriDovizFiyat = await _kurlarService.MusteriDovizFiyat(musteriDoviz);
            var urunDovizFiyat = await _kurlarService.UrunDovizFiyat(urunDoviz);
            var fiyat = (urunDovizFiyat / musteriDovizFiyat) * urunFiyat;
            var indirim = urunFiyat * indirimOran / 100;
            var indirimliFiyat = fiyat - indirim;
            return Convert.ToDouble(indirimliFiyat);
        }
        public async Task<double> UrunFiyat(double urunFiyat, string musteriDoviz, string urunDoviz)
        {
            var musteriDovizFiyat = await _kurlarService.MusteriDovizFiyat(musteriDoviz);
            var urunDovizFiyat = await _kurlarService.UrunDovizFiyat(urunDoviz);
            var fiyat = (urunDovizFiyat / musteriDovizFiyat) * urunFiyat;
            fiyat = Math.Round(fiyat);
            return fiyat;
        }
        public async Task<DataResult<UrunMamulListDto>> GetByCins(string cins)
        {
            var urunmamuls = await CacheUrunmamul();
            var result = await urunmamuls.Where(u => u.CINS == cins).ToListAsync();
            if (result.Count > -1)
            {
                return new DataResult<UrunMamulListDto>(ResultStatus.Success, Messages.Listed, new UrunMamulListDto
                {
                    UrunMamulList = result,
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Listed
                });
            }
            return new DataResult<UrunMamulListDto>(ResultStatus.Error, Messages.NotFound, new UrunMamulListDto
            {
                UrunMamulList = null,
                ResultStatus = ResultStatus.Error,
                Messages = Messages.NotFound
            });
        }
        public async Task<DataResult<UrunMamulWithCinsListDto>> CinsList(string tur)
        {
            var urunmamul = await CacheUrunmamul();

            var result = await (
                from u in urunmamul
                where u.TUR == tur
                group u by u.CINS 
                into cs
                select new UrunMamulWithCins
                {
                    CINS = cs.Key,
                    TOTALCOUNT = cs.Count()
                }).ToListAsync();
            if (result.Count > -1)
            {
                return new DataResult<UrunMamulWithCinsListDto>(ResultStatus.Success, Messages.Listed, new UrunMamulWithCinsListDto
                {
                    UrunMamulWithCins = result,
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Listed
                });
            }
            return new DataResult<UrunMamulWithCinsListDto>(ResultStatus.Error, Messages.NotFound, new UrunMamulWithCinsListDto
            {
                UrunMamulWithCins = null,
                ResultStatus = ResultStatus.Error,
                Messages = Messages.NotFound
            });
        }
        public async Task<DataResult<UrunMamulDto>> GetById(int id)
        {
            var result = await _unitOfWork.UrunMamulDal.GetAsync(u => u.ID == id);
            if (result != null)
            {
                return new DataResult<UrunMamulDto>(ResultStatus.Success, Messages.Listed, new UrunMamulDto
                {
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Listed,
                    Urunmamul = result
                });
            }
            return new DataResult<UrunMamulDto>(ResultStatus.Error, Messages.NotFound, new UrunMamulDto
            {
                ResultStatus = ResultStatus.Error,
                Messages = Messages.NotFound,
                Urunmamul = null
            });
        }
        public async Task<double> UrunFiyatDoviz(double urunFiyat, string urunDoviz, string parametreDoviz)
        {
            var musteriDovizFiyat = await _kurlarService.MusteriDovizFiyat(urunDoviz);
            var urunDovizFiyat = await _kurlarService.UrunDovizFiyat(parametreDoviz);
            var fiyat = (urunDovizFiyat / musteriDovizFiyat) * urunFiyat;
           // fiyat = Math.Round(fiyat);
            return fiyat;
        }

        public async Task<DataResult<UrunMamulModelListDto>> UrunListesi()
        {
           
            var results = await CacheUrunmamul();
            if (results.Count >= 0)
            { 
                var urunler = from urunMamul in results 
                              where !String.IsNullOrEmpty(urunMamul.ACIKLAMA1) && !String.IsNullOrEmpty(urunMamul.AILE)
                              select new URUNMAMULMODEL()
                              {
                                  ID = urunMamul.ID,
                                  ACIKLAMA1 = urunMamul.ACIKLAMA1,
                                  ACIKLAMA2 = urunMamul.ACIKLAMA2,
                                  ACIKLAMA3 = urunMamul.ACIKLAMA3,
                                  ACIKLAMA4 = urunMamul.ACIKLAMA4,
                                  DOVIZ = "TRL",
                                  CINS = urunMamul.CINS,
                                  PARTI = 0,
                                  ETIKET = urunMamul.ETIKET,
                                  ETIKET2 = urunMamul.ETIKET2,
                                  KOD = urunMamul.KOD,
                                  RESIMX = urunMamul.RESIMX,
                                  MARKA = urunMamul.MARKA,
                                  TUR = urunMamul.TUR,
                                  TURID = urunMamul.TURID,
                                  GRUP2 = urunMamul.GRUP2,
                                  GRUP5 = urunMamul.GRUP5,
                                  GRUP6 = urunMamul.GRUP6,
                                  GRUP7 = urunMamul.GRUP7,
                                  GRUP8 = urunMamul.GRUP8,
                                  GRUP9 = urunMamul.GRUP9,
                                  GRUP10 = urunMamul.GRUP10,
                                  GRUP11 = urunMamul.GRUP11,
                                  GRUP12 = urunMamul.GRUP12,
                                  GRUP16 = urunMamul.GRUP16,
                                  WEB2 = urunMamul.WEB2,
                                  AILE = urunMamul.AILE, 
                                  MODEL = urunMamul.MODEL,
                                  SEOACIKLAMA = urunMamul.SEOACIKLAMA,
                                  E_FIYAT1 = urunMamul.E_FIYAT1,
                                  E_FIYAT2 = urunMamul.E_FIYAT2,
                                  TITLE = urunMamul.WEB1_ACIKLAMA,
                                  WEB1_ACIKLAMA = urunMamul.WEB1_ACIKLAMA,
                                  BARCODE = urunMamul.BARCODE,
                                  BITMISGRAM = 0
                              };
                return new DataResult<UrunMamulModelListDto>(ResultStatus.Success, Messages.Listed, new UrunMamulModelListDto
                {
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Listed,
                    Urunler = urunler.ToList()
                });
            }
            return new DataResult<UrunMamulModelListDto>(ResultStatus.Error, Messages.NotFound, new UrunMamulModelListDto
            {
                ResultStatus = ResultStatus.Error,
                Messages = Messages.NotFound,
                Urunler = null
            });
        }
        public async Task<IResult> Add(URUNMAMUL Urunmamul)
        {
            await _unitOfWork.UrunMamulDal.AddAsync(Urunmamul);
            await _unitOfWork.SaveAsync();
            return new Result(ResultStatus.Success, Messages.Added);
        }
        public async Task<IResult> Update(URUNMAMUL Urunmamul)
        {
            var result = await _unitOfWork.UrunMamulDal.GetAsync(a => a.ID == Urunmamul.ID);
            if (result != null)
            {
                await _unitOfWork.UrunMamulDal.UpdateAsync(Urunmamul);
                await _unitOfWork.SaveAsync();
                return new Result(ResultStatus.Success, Messages.Updated);
            }
            return new Result(ResultStatus.Error, Messages.Failed);
        }

        public async Task<DataResult<UrunMamulDto>> Delete(int id)
        {
            var result = await _unitOfWork.UrunMamulDal.GetAsync(z => z.ID == id);
            if (result != null)
            {
                await _unitOfWork.UrunMamulDal.DeleteAsync(result);
                await _unitOfWork.SaveAsync();
                return new DataResult<UrunMamulDto>(ResultStatus.Success, Messages.Deleted, new UrunMamulDto
                {
                    Urunmamul = result,
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Deleted
                });
            }
            return new DataResult<UrunMamulDto>(ResultStatus.Error, Messages.NotFound, new UrunMamulDto
            {
                ResultStatus = ResultStatus.Success,
                Messages = Messages.NotFound,
                Urunmamul = null
            });
        }

        public async Task<DataResult<UrunMamulModelListDto>> LatestList()
        {
            var results = await UrunListesi();
            if (results.Data.Urunler.Count > 0)
            {
                return new DataResult<UrunMamulModelListDto>(ResultStatus.Success, Messages.Listed, new UrunMamulModelListDto
                {
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Listed,
                    Urunler = results.Data.Urunler.OrderByDescending(u => u.FIRSTDATE).Take(7).ToList()
                });
            }
            return new DataResult<UrunMamulModelListDto>(ResultStatus.Error, Messages.NotFound, new UrunMamulModelListDto
            {
                ResultStatus = ResultStatus.Error,
                Messages = Messages.NotFound,
                Urunler = null
            });
        }

        public async Task<IDataResult<UrunMamulModelListDto>> YeniUrunler()
        {
            var Grup = await _unitOfWork.Grup10Dal.GetAsync(null);
            var results = UrunListesi().Result.Data.Urunler.ToList();
            if (results.Count > 0)
            {
                return new DataResult<UrunMamulModelListDto>(ResultStatus.Success, Messages.Listed, new UrunMamulModelListDto
                {
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Listed,
                    Urunler = results
                });
            }
            return new DataResult<UrunMamulModelListDto>(ResultStatus.Error, Messages.NotFound, new UrunMamulModelListDto
            {
                ResultStatus = ResultStatus.Error,
                Messages = Messages.NotFound,
                Urunler = null
            });
        }

        public async Task<DataResult<UrunMamulModelListDto>> SearchList(string search)
        {
            var urumamulList = await CacheUrunmamul();
            var parametre = await _unitOfWork.ParametreDal.GetAsync(null);
            var result = await urumamulList.Where(u => (u.ACIKLAMA1.ToLower().Contains(search.ToLower()) || u.KOD.ToLower().Contains(search.ToLower())))
                .Select(x => new URUNMAMULMODEL
                {
                    ID = x.ID,
                    ACIKLAMA1 = x.ACIKLAMA1,
                    ACIKLAMA2 = x.ACIKLAMA2,
                    ACIKLAMA3 = x.ACIKLAMA3,
                    ACIKLAMA4 = x.ACIKLAMA4,
                    DOVIZ = "TRL",
                    CINS = x.CINS,
                 
                    ETIKET = x.ETIKET,
                    ETIKET2 = x.ETIKET2,
                    KOD = x.KOD,
                    RESIMX = x.RESIMX,
                    MARKA = x.MARKA,
                    TUR = x.TUR,
                    TURID = x.TURID,
                    GRUP2 = x.GRUP2,
                    GRUP5 = x.GRUP5,
                    GRUP6 = x.GRUP6,
                    GRUP7 = x.GRUP7,
                    GRUP8 = x.GRUP8,
                    GRUP9 = x.GRUP9,
                    GRUP10 = x.GRUP10,
                    GRUP11 = x.GRUP11,
                    GRUP12 = x.GRUP12,
                    GRUP16 = x.GRUP16,
                    WEB2 = x.WEB2,
                    AILE = x.AILE,
                    MODEL = x.MODEL,
                    SEOACIKLAMA = x.SEOACIKLAMA,
                    E_FIYAT1 = x.E_FIYAT1,
                    E_FIYAT2 = x.E_FIYAT2,
                    TITLE = x.WEB1_ACIKLAMA,
                    WEB1_ACIKLAMA = x.WEB1_ACIKLAMA,
                    BARCODE = x.BARCODE
                })
                .OrderByDescending(u => u.FIRSTDATE).ToListAsync();
            if (result.Count > -1)
            {
                return new DataResult<UrunMamulModelListDto>(ResultStatus.Success, Messages.Listed, new UrunMamulModelListDto
                {
                    Urunler = result,
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Listed
                });
            }
            return new DataResult<UrunMamulModelListDto>(ResultStatus.Error, Messages.NotFound, new UrunMamulModelListDto
            {
                Urunler = null,
                ResultStatus = ResultStatus.Error,
                Messages = Messages.NotFound
            });
        }

        public async Task<DataResult<UrunMamulModelListDto>> SearchListInStock(string search)
        {
            var result = await UrunmamulsInStock(u => (u.ACIKLAMA1.ToLower().Contains(search.ToLower()) || u.KOD.ToLower().Contains(search.ToLower())));

            if (result.Count > -1)
            {
                return new DataResult<UrunMamulModelListDto>(ResultStatus.Success, Messages.Listed, new UrunMamulModelListDto
                {
                    Urunler = result,
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Listed
                });
            }
            return new DataResult<UrunMamulModelListDto>(ResultStatus.Error, Messages.NotFound, new UrunMamulModelListDto
            {
                Urunler = null,
                ResultStatus = ResultStatus.Error,
                Messages = Messages.NotFound
            });
        }
        //private async Task<List<URUNMAMULMODEL>> UrunmamulsInStock( Func<URUNMAMULMODEL, bool> filter = null)
        //{
        //    var urumamulList = await CacheUrunmamul();
        //    var parametre = await _unitOfWork.ParametreDal.GetAsync(null);
        //    var stokdurdal = await _unitOfWork.StokDurDal.GetStokDurInStock();
        //    Func<URUNMAMULMODEL, bool> VALUE = x => true;
        //    List<URUNMAMULMODEL> result;

        //        result = await (from stk in stokdurdal
        //                        join urmm in urumamulList 
        //                            on new { urunkod = stk.URUNKOD, parti = stk.PARTI } equals
        //                            new { urunkod = urmm.KOD, parti = (int)urmm.PARTI }
        //                        select new
        //                        {
        //                            stk,
        //                            urmm
        //                        }
        //                            into u
        //                        group u by new
        //                        {
        //                            u.urmm.ID,
        //                            u.urmm.AILE,
        //                            u.urmm.ACIKLAMA2,
        //                            u.urmm.CINS,
        //                            u.urmm.FIRSTDATE,
        //                            u.urmm.KATALOG,
        //                            u.urmm.KOD,
        //                            u.urmm.MARKA,
        //                            u.urmm.MODEL,
        //                            u.urmm.PARTI,
        //                            u.urmm.RESIMX,
        //                            u.urmm.TUR,
        //                            u.urmm.TURID,
        //                            u.urmm.REFNO,
        //                            u.urmm.ACIKLAMA1,
        //                            u.urmm.ACIKLAMA3,
        //                            u.urmm.ACIKLAMA4,
        //                            u.urmm.DOVIZ,
                                   
        //                            u.urmm.ETIKET,
        //                            u.urmm.ETIKET2,
        //                            u.urmm.GRUP2,
        //                            u.urmm.GRUP5,
        //                            u.urmm.GRUP6,
        //                            u.urmm.GRUP7,
        //                            u.urmm.GRUP8,
        //                            u.urmm.GRUP9,
        //                            u.urmm.GRUP10,
        //                            u.urmm.GRUP11,
        //                            u.urmm.GRUP12,
        //                            u.urmm.GRUP16,
        //                            u.urmm.WEB2,
        //                            u.urmm.SEOACIKLAMA,
        //                            u.urmm.E_FIYAT1,
        //                            u.urmm.E_FIYAT2,
        //                            u.urmm.WEB1_ACIKLAMA,
        //                            u.urmm.BARCODE
        //                        }
        //                       into product
        //                        select new URUNMAMULMODEL()
        //                        {
        //                            ID = product.Key.ID,
        //                            ACIKLAMA1 = product.Key.ACIKLAMA1,
        //                            ACIKLAMA2 = product.Key.ACIKLAMA2,
        //                            ACIKLAMA3 = product.Key.ACIKLAMA3,
        //                            ACIKLAMA4 = product.Key.ACIKLAMA4,
        //                            DOVIZ = "TRL",
        //                            CINS = product.Key.CINS,
                                  
        //                            ETIKET = product.Key.ETIKET,
        //                            ETIKET2 = product.Key.ETIKET2,
        //                            KOD = product.Key.KOD,
        //                            RESIMX = product.Key.RESIMX,
        //                            MARKA = product.Key.MARKA,
        //                            TUR = product.Key.TUR,
        //                            TURID = product.Key.TURID,
        //                            GRUP2 = product.Key.GRUP2,
        //                            GRUP5 = product.Key.GRUP5,
        //                            GRUP6 = product.Key.GRUP6,
        //                            GRUP7 = product.Key.GRUP7,
        //                            GRUP8 = product.Key.GRUP8,
        //                            GRUP9 = product.Key.GRUP9,
        //                            GRUP10 = product.Key.GRUP10,
        //                            GRUP11 = product.Key.GRUP11,
        //                            GRUP12 = product.Key.GRUP12,
        //                            GRUP16 = product.Key.GRUP16,
        //                            WEB2 = product.Key.WEB2,
        //                            AILE = product.Key.AILE,
        //                            MODEL = product.Key.MODEL,
        //                            SEOACIKLAMA = product.Key.SEOACIKLAMA,
        //                            E_FIYAT1 = product.Key.E_FIYAT1,
        //                            E_FIYAT2 = product.Key.E_FIYAT2,
        //                            TITLE = product.Key.WEB1_ACIKLAMA,
        //                            WEB1_ACIKLAMA = product.Key.WEB1_ACIKLAMA,
        //                            BARCODE = product.Key.BARCODE
        //                        }).Where(filter ?? VALUE).OrderByDescending(u => u.FIRSTDATE).ToListAsync();

        //        return result;
        //}

        public async Task<DataResult<ProductdetailListDto>> UrunMamulFiltrele(UrunMamulFiltre Filtre)
        {
            var results = await _unitOfWork.UrunMamulDal.UrunMamulFiltrele(Filtre);
            if (results.Count > -1)
            {
                return new DataResult<ProductdetailListDto>(ResultStatus.Success, Messages.Listed, new ProductdetailListDto
                {
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Listed,
                    ProductDetails = results
                });
            }
            return new DataResult<ProductdetailListDto>(ResultStatus.Error, Messages.NotFound, new ProductdetailListDto
            {
                ResultStatus = ResultStatus.Error,
                Messages = Messages.NotFound,
                ProductDetails = null
            });
        }

        public async Task<DataResult<UrunMamulListDto>> GetAllSirali(string cinsId, string tur)
        {
            var result = await CacheUrunmamul();
            var urunler = result.Where(x => x.GRUP11 == cinsId  && x.TUR == tur).ToList();

            if (result.Count > -1)
            {
                return new DataResult<UrunMamulListDto>(ResultStatus.Success, Messages.Listed, new UrunMamulListDto
                {
                    UrunMamulList = urunler,
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Listed
                });
            }
            return new DataResult<UrunMamulListDto>(ResultStatus.Error, Messages.NotFound, new UrunMamulListDto
            {
                UrunMamulList = null,
                ResultStatus = ResultStatus.Error,
                Messages = Messages.NotFound
            });
        }

        public async Task<DataResult<UrunMamulModelListDto>> GetAllTur(string tur)
        {
            var urunlistesi = await UrunListesi();
            var turlistesi = urunlistesi.Data.Urunler.Where(x => x.TUR == tur).ToList();
            if (turlistesi.Count>-1)
            {
                return new DataResult<UrunMamulModelListDto>(ResultStatus.Success, Messages.Listed, new UrunMamulModelListDto
                {
                    ResultStatus = ResultStatus.Success,
                    Messages = Messages.Listed,
                    Urunler = turlistesi
                });
            }
            return new DataResult<UrunMamulModelListDto>(ResultStatus.Error, Messages.NotFound, new UrunMamulModelListDto
            {
                ResultStatus = ResultStatus.Error,
                Messages = Messages.NotFound,
                Urunler = null
            }); 

        }

        public async Task<List<URUNMAMULMODEL>> UrunmamulsInStock(Func<URUNMAMULMODEL, bool> filter = null)
        {
            var urumamulList = await CacheUrunmamul();
            var parametre = await _unitOfWork.ParametreDal.GetAsync(null);
            List<StokDurInStock> stokdurdal;
            if (parametre.STOKGOSTER=="Y")
            {
                stokdurdal = await _unitOfWork.StokDurDal.GetStokDurInStockY();
            }
            else
            { 
            stokdurdal = await _unitOfWork.StokDurDal.GetStokDurInStock();
            }
            Func<URUNMAMULMODEL, bool> VALUE = x => true;
            List<URUNMAMULMODEL> result;
            
            result = await(from stk in stokdurdal
                             join  urmm in urumamulList 
                          
                               on new { urunkod = stk.URUNKOD, parti = stk.PARTI } equals
                               new { urunkod = urmm.KOD, parti = (int)urmm.PARTI } 
                               
                           select new
                           {
                               stk,
                               urmm
                           }
                                into u
                          
                           group u by new
                           {
                               u.urmm.ID,
                               u.urmm.AILE,
                               u.urmm.ACIKLAMA2,
                               u.urmm.CINS,
                               u.urmm.FIRSTDATE,
                               u.urmm.KATALOG,
                               u.urmm.KOD,
                               u.urmm.MARKA,
                               u.urmm.MODEL,
                               u.urmm.PARTI,
                               u.urmm.RESIMX,
                               u.urmm.TUR,
                               u.urmm.TURID,
                               u.urmm.REFNO,
                               u.urmm.ACIKLAMA1,
                               u.urmm.ACIKLAMA3,
                               u.urmm.ACIKLAMA4,
                               u.urmm.DOVIZ,

                               u.urmm.ETIKET,
                               u.urmm.ETIKET2,
                               u.urmm.GRUP2,
                               u.urmm.GRUP5,
                               u.urmm.GRUP6,
                               u.urmm.GRUP7,
                               u.urmm.GRUP8,
                               u.urmm.GRUP9,
                               u.urmm.GRUP10,
                               u.urmm.GRUP11,
                               u.urmm.GRUP12,
                               u.urmm.GRUP16,
                               u.urmm.WEB2,
                               u.urmm.SEOACIKLAMA,
                               u.urmm.E_FIYAT1,
                               u.urmm.E_FIYAT2,
                               u.urmm.WEB1_ACIKLAMA,
                               u.urmm.BARCODE
                           }
                           into product
                           
                           select new URUNMAMULMODEL()
                           {
                               ID = product.Key.ID,
                               ACIKLAMA1 = product.Key.ACIKLAMA1,
                               ACIKLAMA2 = product.Key.ACIKLAMA2,
                               ACIKLAMA3 = product.Key.ACIKLAMA3,
                               ACIKLAMA4 = product.Key.ACIKLAMA4,
                               DOVIZ = "TRL",
                               CINS = product.Key.CINS,
                               PARTI=product.Key.PARTI,
                               ETIKET = product.Key.ETIKET,
                               ETIKET2 = product.Key.ETIKET2,
                               KOD = product.Key.KOD,
                               RESIMX = product.Key.RESIMX,
                               MARKA = product.Key.MARKA,
                               TUR = product.Key.TUR,
                               TURID = product.Key.TURID,
                               GRUP2 = product.Key.GRUP2,
                               GRUP5 = product.Key.GRUP5,
                               GRUP6 = product.Key.GRUP6,
                               GRUP7 = product.Key.GRUP7,
                               GRUP8 = product.Key.GRUP8,
                               GRUP9 = product.Key.GRUP9,
                               GRUP10 = product.Key.GRUP10,
                               GRUP11 = product.Key.GRUP11,
                               GRUP12 = product.Key.GRUP12,
                               GRUP16 = product.Key.GRUP16,
                               WEB2 = product.Key.WEB2,
                               AILE = product.Key.AILE,
                               MODEL = product.Key.MODEL,
                               SEOACIKLAMA = product.Key.SEOACIKLAMA,
                               E_FIYAT1 = product.Key.E_FIYAT1,
                               E_FIYAT2 = product.Key.E_FIYAT2,
                               TITLE = product.Key.WEB1_ACIKLAMA,
                               WEB1_ACIKLAMA = product.Key.WEB1_ACIKLAMA,
                               BARCODE = product.Key.BARCODE,
                               
                               
                           }).Where(filter ?? VALUE).OrderByDescending(u => u.FIRSTDATE).ToListAsync();

            return result;
        }

        //public async Task<DataResult<IEnumerable<URUNMAMUL>>> GetByAileList()
        //{
        

        //    var urunmamul = await CacheUrunmamul();
        //    var AileList = from u in urunmamul.Where(x => x.AILE != "")
        //                   group u by u.AILE
        //                     into aile
        //                   select new URUNMAMUL
        //                   {
        //                       AILE = aile.Key

        //                   };


        //    var result = AileList;
        //    if (result !=null)
        //    {
        //        return new DataResult<URUNMAMUL>(ResultStatus.Success, Messages.Listed, new URUNMAMUL
        //        {
        //             = result,
        //            ResultStatus = ResultStatus.Success,
        //            Messages = Messages.Listed
        //        });
        //    }
        //    return new DataResult<URUNMAMUL>(ResultStatus.Error, Messages.NotFound, new URUNMAMUL
        //    {
        //        CollectionList = null,
        //        ResultStatus = ResultStatus.Error,
        //        Messages = Messages.NotFound
        //    });
        //}
    }
}