using API_TF.Services.DTOs;
using FluentValidation;

namespace API_TF.Services.Validate
{
    public class ProductValidate : AbstractValidator<ProductDTO>
    {
        public ProductValidate()
        {
            RuleFor(dto => dto.Barcode)
                .NotEmpty()
                .WithMessage("O campo 'Barcode' é obrigatório.");

            RuleFor(dto => dto.Description)
                .NotEmpty()
                .WithMessage("O campo 'Description' é obrigatório.");

            RuleFor(dto => dto.Barcodetype)
                .NotEmpty()
                .WithMessage("O campo 'Barcodetype' é obrigatório.");

            RuleFor(dto => dto.Price)
                .GreaterThan(0)
                .WithMessage("O campo 'Price' é obrigatório e deve ser maior que zero.");

            RuleFor(dto => dto.Costprice)
                .GreaterThan(0)
                .WithMessage("O campo 'Costprice' é obrigatório e deve ser maior que zero.");
        }
    }
}