using API_TF.DataBase;
using API_TF.DataBase.Models;
using API_TF.Services.DTOs;
using API_TF.Services.Exceptions;
using API_TF.Services.Validate;
using ApiWebDB.Services.Exceptions;
using System.Collections.Generic;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace API_TF.Services
{
    public class SaleService
    {
        private readonly TfDbContext _dbCDbContext;
        private readonly PromotionService _promotionService;
        private readonly ProductService _productService;
        private readonly IMapper _mapper;
        private readonly LogService _LogService;
        public readonly IValidator<SaleDTO> _validator;


        public SaleService(TfDbContext dbCDbContext, PromotionService promotionService, ProductService productService, IValidator<SaleDTO> validator, IMapper mapper, LogService LogService)
        {
            _dbCDbContext = dbCDbContext;
            _promotionService = promotionService;
            _productService = productService;
            _mapper = mapper;
            _LogService = LogService;
            _validator = validator;
        }


        public IEnumerable<TbSale> Insert(List<SaleDTO> dtoList)
        {
            var sales = new List<TbSale>();
            var currentTime = DateTime.Now;
            var code = Guid.NewGuid().ToString();

            foreach (var dto in dtoList)
            {
                var validationResult = _validator.Validate(dto);

                if (!validationResult.IsValid)
                {
                    throw new InvalidDataException("Dados inválidos", validationResult.Errors);
                }

                var product = _productService.GetById(dto.Productid);

                if (product == null)
                    throw new NotFoundException("Produto não existe");

                if (product.Stock < dto.Qty)
                    throw new InsufficientStockException("Estoque insuficiente para a movimentação");

                var promotions = _promotionService.GetActivePromotions(dto.Productid);

                decimal unitPrice = product.Price;

                decimal originalPrice = unitPrice;

                foreach (var promotion in promotions)
                {
                    unitPrice = ApplyPromotion(unitPrice, promotion);
                }

                decimal totalDiscount = originalPrice - unitPrice;

                _productService.AjustarStock(product.Id, -dto.Qty);

                var stockLogDto = new StockLogDTO
                {
                    Productid = dto.Productid,
                    Qty = -dto.Qty,
                    Createdat = DateTime.Now
                };

                _LogService.InsertLog(stockLogDto);

                var sale = _mapper.Map<TbSale>(dto);

                sale.Code = code;
                sale.Price = product.Price;
                sale.Discount = totalDiscount;
                sale.Createat = currentTime;

                _dbCDbContext.Add(sale);
                sales.Add(sale);
            }

            _dbCDbContext.SaveChanges();

            return sales;
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

            if (startDate == default || endDate == default)
            {
                throw new BadRequestException("As datas de início e fim são obrigatórias.");
            }

            var query = from sale in _dbCDbContext.TbSales join product in _dbCDbContext.TbProducts on sale.Productid equals product.Id
                        where sale.Createat >= startDate && sale.Createat < endDate.AddDays(1) select new SalesReportDTO
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
