using API_TF.DataBase.Models;using AutoMapper;
using API_TF.Services.DTOs;

namespace API_TF.MappingProfiles
{

    using AutoMapper;
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PromotionDTO, TbPromotion>().ReverseMap();
            CreateMap<ProductDTO, TbProduct>().ReverseMap();
            CreateMap<SaleDTO, TbSale>().ReverseMap();
            CreateMap<StockLogDTO, TbStockLog>().ReverseMap();
            CreateMap<ProductUpDTO, TbProduct>().ReverseMap();
        }
    }



}
