using API_TF.DataBase.Models;
using API_TF.Services.DTOs;
using System;

namespace API_TF.Services.Parser
{
    public class PromotionParser
    {
        public static TbPromotion ToEntity(PromotionDTO dto)
        {
            return new TbPromotion
            {
                Startdate = DateTime.SpecifyKind(dto.Startdate, DateTimeKind.Unspecified),
                Enddate = DateTime.SpecifyKind(dto.Enddate, DateTimeKind.Unspecified),
                Promotiontype = dto.Promotiontype,
                Productid = dto.Productid,
                Value = dto.Value
            };
        }
    }
}
