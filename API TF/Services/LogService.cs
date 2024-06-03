using API_TF.DataBase;
using API_TF.DataBase.Models;
using API_TF.Services.DTOs;
using API_TF.Services.Exceptions;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API_TF.Services
{
    public class LogService
    {
        private readonly TfDbContext _dbCDbContext;
        private readonly IMapper _mapper;
        public LogService(TfDbContext dbContext, IMapper mapper)
        {
            _dbCDbContext = dbContext;
            _mapper = mapper;
        }


        public TbStockLog InsertLog(StockLogDTO dto)
        {
            var entity = _mapper.Map<TbStockLog>(dto);

            _dbCDbContext.TbStockLogs.Add(entity);
            _dbCDbContext.SaveChanges();
            return entity;
        }


        public List<StockResultLogDTO> GetLogsByProduct(int productId)
        {
            var productExists = _dbCDbContext.TbProducts.Any(p => p.Id == productId);

            if (!productExists)
            {
                throw new NotFoundException("Produto não encontrado.");
            }

            var logs = from log in _dbCDbContext.TbStockLogs
                       where log.Productid == productId
                       select new StockResultLogDTO
                       {
                           Date = log.Createdat,
                           Barcode = log.Product.Barcode,
                           Description = log.Product.Description,
                           Quantity = log.Qty
                       };

            if (!logs.Any())
            {
                throw new NotFoundException("Nenhum log encontrado para o produto.");
            }

            return logs.ToList();
        }
    }
}
