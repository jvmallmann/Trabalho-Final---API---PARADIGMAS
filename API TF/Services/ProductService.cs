using API_TF.DataBase;
using API_TF.DataBase.Models;
using API_TF.Services.DTOs;
using API_TF.Services.Exceptions;
using API_TF.Services.Validate;
using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InvalidDataException = API_TF.Services.Exceptions.InvalidDataException;

namespace API_TF.Services
{
    public class ProductService
    {
        private readonly TfDbContext _dbCDbContext;
        private readonly ProductValidate _validator;
        public readonly IValidator<ProductUpDTO> _validatorUpdateProduct;
        private readonly IMapper _mapper;
        private readonly LogService _LogService;

        public ProductService(TfDbContext dbCDbContext, IValidator<ProductUpDTO> validatorUpProduct, ProductValidate validator, IMapper mapper, LogService LogService)
        {
            _dbCDbContext = dbCDbContext;
            _validator = validator;
            _mapper = mapper;
            _LogService = LogService;
            _validatorUpdateProduct = validatorUpProduct;
    }


        public TbProduct Insert(ProductDTO dto)
        {
            var validationResult = _validator.Validate(dto);
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
                throw new InvalidEntity(string.Join(", ", errorMessages));
            }

            var entity = _mapper.Map<TbProduct>(dto);

            _dbCDbContext.Add(entity);
            _dbCDbContext.SaveChanges();

            _LogService.InsertLog(new StockLogDTO
            {
                Productid = entity.Id,
                Qty = entity.Stock,
                Createdat = DateTime.Now
            });

            return entity;
        }


        public TbProduct GetById(int id)
        {
            var existingEntity = _dbCDbContext.TbProducts.FirstOrDefault(c => c.Id == id);

            if (existingEntity == null)
            {
                throw new NotFoundException("Registro não existe");
            }
            return existingEntity;
        }


        public TbProduct GetByBarcode(string barcode)
        {
            var existingEntity = _dbCDbContext.TbProducts.FirstOrDefault(c => c.Barcode == barcode);

            if (existingEntity == null)
            {
                throw new NotFoundException("Registro não existe");
            }
            return existingEntity;
        }


        public IEnumerable<TbProduct> GetByDescription(string description)
        {
            var existingEntities = _dbCDbContext.TbProducts
                                                 .Where(c => c.Description.Contains(description))
                                                 .ToList();

            if (existingEntities == null || existingEntities.Count == 0)
            {
                throw new NotFoundException("Nenhum registro encontrado");
            }

            return existingEntities;
        }


        public TbProduct Update(ProductUpDTO dto, int id)
        {

            var product = GetById(id);

            var validationResult = _validatorUpdateProduct.Validate(dto);

            if (!validationResult.IsValid)
            {
                throw new InvalidDataException("Dados inválidos", validationResult.Errors);
            }

            int oldStock = product.Stock;

            if (product == null)
            {
                throw new NotFoundException("Registro não existe");
            }

            product.Description = dto.Description;
            product.Barcode = dto.Barcode;
            product.Barcodetype = dto.Barcodetype;
            product.Price = dto.Price;
            product.Costprice = dto.Costprice;

            _dbCDbContext.Update(product);
            _dbCDbContext.SaveChanges();

            _LogService.InsertLog(new StockLogDTO
            {
                Productid = product.Id,
                Qty = product.Stock - oldStock,
                Createdat = DateTime.Now
            });

            return product;
        }


        public void AjustarStock(int productId, int quantity)
        {
            var product = GetById(productId);
            if (product == null)
            {
                throw new NotFoundException("Produto não encontrado.");
            }

            if (quantity == 0)
            {
                throw new InvalidEntity("A quantidade a atualizar deve ser diferente de zero.");
            }

            if (product.Stock + quantity < 0)
            {
                throw new InsufficientStockException("Estoque insuficiente para realizar a operação.");
            }

            int oldStock = product.Stock;

            product.Stock += quantity;
            _dbCDbContext.Update(product);
            _dbCDbContext.SaveChanges();

            _LogService.InsertLog(new StockLogDTO
            {
                Productid = product.Id,
                Qty = product.Stock - oldStock,
                Createdat = DateTime.Now
            });
        }
    }
}
