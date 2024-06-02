using API_TF.Services.DTOs;
using API_TF.Services.Exceptions;
using ApiWebDB.Services.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using System;

namespace API_TF.Services.Validate
{
    public class PromotionValidate : AbstractValidator<PromotionDTO>
    {
        public PromotionValidate()
        {
            RuleFor(promotion => promotion.Startdate)
                .NotEmpty().WithMessage("A data de início da promoção é obrigatória.");

            RuleFor(promotion => promotion.Enddate)
                .NotEmpty().WithMessage("A data fim da promoção é obrigatória.");

            RuleFor(promotion => promotion.Promotiontype)
                .Must(p => p == 0 || p == 1)
                .WithMessage("O tipo de promoção deve ser 0 (desconto percentual) ou 1 (desconto fixo)");

            RuleFor(promotion => promotion.Value)
                .GreaterThan(0)
                .WithMessage("O valor da promoção deve ser maior que zero");

            When(promotion => promotion.Promotiontype == 0, () =>
            {
                RuleFor(promotion => promotion.Value)
                    .InclusiveBetween(0, 100)
                    .WithMessage("Para desconto percentual, o valor deve estar entre 0 e 100");
            });

            When(promotion => promotion.Promotiontype == 1, () =>
            {
                RuleFor(promotion => promotion.Value)
                    .GreaterThan(0)
                    .WithMessage("Para desconto fixo, o valor deve ser positivo");
            });
        }

        public ValidationResult ValidatePromotion(PromotionDTO promotion)
        {
            return Validate(promotion);
        }
    }
}
