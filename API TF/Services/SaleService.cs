using API_TF.DataBase;
using API_TF.DataBase.Models;
using API_TF.Services.DTOs;
using API_TF.Services.Exceptions;
using API_TF.Services.Parser;
using API_TF.Services.Validate;
using ApiWebDB.Services.Exceptions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API_TF.Services
{
    public class SaleService
    {
        private readonly TfDbContext _dbCDbContext;
        private readonly SaleDTOValidator _validator;
        private readonly PromotionService _promotionService;
        private readonly ProductService _productService;
        private readonly IMapper _mapper;
        private readonly LogService _LogService;

        public SaleService(TfDbContext dbCDbContext, SaleDTOValidator validator, PromotionService promotionService, ProductService productService, IMapper mapper, LogService LogService)
        {
            _dbCDbContext = dbCDbContext;
            _validator = validator;
            _promotionService = promotionService;
            _productService = productService;
            _mapper = mapper;
            _LogService = LogService;
        }


        public TbSale Insert(SaleDTO dto)
        {
            var product = _productService.GetById(dto.Productid);

            if (product == null)
            {
                throw new NotFoundException("Produto não encontrado.");
            }

            if (product.Stock < dto.Qty)
            {
                throw new InsufficientStockException("Estoque insuficiente para o produto: " + product.Description);
            }

            var promotions = _promotionService.GetActivePromotions(dto.Productid);


            decimal unitPrice = product.Price;

            foreach (var promotion in promotions)
            {
                unitPrice = ApplyPromotion(unitPrice, promotion);
            }

            decimal totalPrice = unitPrice * dto.Qty;

            var entity = _mapper.Map<TbSale>(dto);

            entity.Price = totalPrice;

            product.Stock -= dto.Qty;

            entity.Createat = DateTime.Now;

            _dbCDbContext.Update(product);
            _dbCDbContext.Add(entity);
            _dbCDbContext.SaveChanges();

            _LogService.InsertLog(new StockLogDTO
            {
                Productid = entity.Productid,
                Qty = -entity.Qty,
                Createdat = DateTime.Now
            });

            return entity;
        }


        public decimal ApplyPromotion(decimal price, TbPromotion promotion)
        {
            switch (promotion.Promotiontype)
            {
                case 0:
                    return price * (1 - promotion.Value / 100);
                case 1:
                    return price - promotion.Value;
                default:
                    return price;
            }
        }


        public TbSale GetSaleByCode(string code)
        {
            var sale = _dbCDbContext.TbSales
                .Include(s => s.Product)
                .FirstOrDefault(s => s.Code == code);

            if (sale == null)
            {
                throw new NotFoundException("Venda não encontrada.");
            }

            return sale;
        }


        public List<SalesReportDTO> GetSalesReportByPeriod(DateTime startDate, DateTime endDate)
        {
            var query = from sale in _dbCDbContext.TbSales
                        join product in _dbCDbContext.TbProducts on sale.Productid equals product.Id
                        where sale.Createat >= startDate && sale.Createat < endDate.AddDays(1)
                        select new SalesReportDTO
                        {
                            SaleCode = sale.Code,
                            ProductDescription = product.Description,
                            Price = sale.Price,
                            Quantity = sale.Qty,
                            SaleDate = sale.Createat
                        };

            if (!query.Any())
            {
                throw new NotFoundException("Nenhuma venda encontrada para o período.");
            }

            return query.ToList();
        }
    }
}
