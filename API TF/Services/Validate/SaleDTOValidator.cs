using API_TF.DataBase;
using API_TF.Services.DTOs;
using FluentValidation;
using System.Linq;

namespace API_TF.Services.Validate
{
    public class SaleDTOValidator : AbstractValidator<SaleDTO>
    {
        private readonly TfDbContext _dbContext;

        public SaleDTOValidator(TfDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(dto => dto.Code)
                .NotEmpty().WithMessage("O campo 'Code' é obrigatório.");

            RuleFor(dto => dto.Createat)
                .NotEmpty().WithMessage("O campo 'Createat' é obrigatório.");

            RuleFor(dto => dto.Productid)
                .GreaterThan(0).WithMessage("O campo 'Productid' é obrigatório e deve ser maior que zero.")
                .Must(ProductExists).WithMessage("Produto com ID especificado não existe.")
                .Must(HaveSufficientStock).WithMessage("Estoque insuficiente para o produto especificado.");

            RuleFor(dto => dto.Price)
                .GreaterThan(0).WithMessage("O campo 'Price' é obrigatório e deve ser maior que zero.");

            RuleFor(dto => dto.Qty)
                .GreaterThan(0).WithMessage("O campo 'Qty' é obrigatório e deve ser maior que zero.");

            RuleFor(dto => dto.Discount)
                .GreaterThanOrEqualTo(0).WithMessage("O campo 'Discount' é obrigatório e deve ser maior ou igual a zero.");
        }

        private bool ProductExists(int productId)
        {
            return _dbContext.TbProducts.Any(p => p.Id == productId);
        }

        private bool HaveSufficientStock(SaleDTO dto, int productId)
        {
            var product = _dbContext.TbProducts.FirstOrDefault(p => p.Id == productId);
            return product != null && product.Stock >= dto.Qty;
        }
    }
}
