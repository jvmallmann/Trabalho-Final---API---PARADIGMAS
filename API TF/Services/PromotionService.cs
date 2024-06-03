using API_TF.DataBase.Models;
using API_TF.DataBase;
using API_TF.Services.DTOs;
using API_TF.Services.Exceptions;
using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API_TF.Services
{
    public class PromotionService
    {
        private readonly TfDbContext _dbCDbContext;
        private readonly IValidator<PromotionDTO> _validator;
        private readonly IMapper _mapper;

        public PromotionService(TfDbContext dbCDbContext, IValidator<PromotionDTO> validator, IMapper mapper)
        {
            _dbCDbContext = dbCDbContext;
            _validator = validator;
            _mapper = mapper;
        }


        public TbPromotion Insert(PromotionDTO dto)
        {
            ValidatePromotion(dto);

            var entity = _mapper.Map<TbPromotion>(dto);

            _dbCDbContext.Add(entity);
            _dbCDbContext.SaveChanges();

            return entity;
        }


        public TbPromotion GetById(int id)
        {
            var existingEntity = _dbCDbContext.TbPromotions.FirstOrDefault(c => c.Id == id);

            if (existingEntity == null)
            {
                throw new NotFoundException("Registro não existe");
            }
            return existingEntity;
        }


        public IEnumerable<TbPromotion> GetPromotionsByProductAndPeriod(int productId, DateTime startDate, DateTime endDate)
        {
            var productExists = _dbCDbContext.TbProducts.Any(p => p.Id == productId);
            if (!productExists)
            {
                throw new NotFoundException("Produto não encontrado.");
            }

            var promotions = _dbCDbContext.TbPromotions
                .Where(p => p.Productid == productId &&
                            p.Startdate >= startDate &&
                            p.Enddate <= endDate)
                .ToList();

            if (promotions == null || promotions.Count == 0)
            {
                throw new NotFoundException("Nenhuma promoção encontrada para o período especificado.");
            }

            return promotions;
        }


        public TbPromotion Update(PromotionDTO dto, int id)
        {
            ValidatePromotion(dto);

            var promotion = GetById(id);

            _mapper.Map(dto, promotion);

            _dbCDbContext.Update(promotion);
            _dbCDbContext.SaveChanges();

            return promotion;
        }


        public List<TbPromotion> GetActivePromotions(int productId)
        {
            var currentDate = DateTime.Now;

            return _dbCDbContext.TbPromotions
                     .Where(p => p.Productid == productId
                                 && p.Startdate <= currentDate
                                 && p.Enddate >= currentDate)
                     .OrderByDescending(p => p.Enddate)
                     .ThenByDescending(p => p.Startdate)
                     .ToList();
        }


        private void ValidatePromotion(PromotionDTO dto)
        {
            var validationResult = _validator.Validate(dto);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new InvalidEntity(errorMessages);
            }
        }
    }
}
