using API_TF.DataBase.Models;
using API_TF.Services.DTOs;
using System;

namespace API_TF.Services.Parser
{
    public class SaleParser
    {
        public static TbSale ToEntity(SaleDTO dto)
        {
            return new TbSale
            {
                Code = dto.Code,
                Createat = dto.Createat,
                Productid = dto.Productid,
                Price = dto.Price,
                Qty = dto.Qty,
                Discount = dto.Discount

            };
        }
    }
}
