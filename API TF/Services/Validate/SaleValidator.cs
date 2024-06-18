using API_TF.DataBase;
using API_TF.Services.DTOs;
using FluentValidation;
using System.Linq;

namespace API_TF.Services.Validate
{
    public class SaleValidator : AbstractValidator<SaleDTO>
    {
        private readonly TfDbContext _dbContext;
        public SaleValidator()
        {
            RuleFor(sale => sale.Productid)
                .GreaterThan(0).WithMessage("O ID do produto associado à venda é obrigatório.");

            RuleFor(sale => sale.Qty)
                .GreaterThan(0).WithMessage("A quantidade vendida deve ser maior que zero.");
        }
    }
}
