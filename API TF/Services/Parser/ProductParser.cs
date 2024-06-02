using API_TF.DataBase.Models;
using API_TF.Services.DTOs;
using System;

namespace API_TF.Services.Parser
{
    public class ProductParser
    {
        public static TbProduct ToEntity(ProductDTO dto)
        {
            return new TbProduct
            {
                Description = dto.Description,
                Barcode = dto.Barcode,
                Barcodetype = dto.Barcodetype,
                Stock = dto.Stock,
                Price = dto.Price,
                Costprice = dto.Costprice

            };
        }
    }
}
