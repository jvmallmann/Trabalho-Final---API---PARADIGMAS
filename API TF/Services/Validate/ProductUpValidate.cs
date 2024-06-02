using API_TF.Services.DTOs;
using FluentValidation;
using System.Text.RegularExpressions;

namespace API_TF.Services.Validate
{
    public class ProductUpValidate : AbstractValidator<ProductUpDTO>
    {
        public ProductUpValidate()
        {
            RuleFor(product => product.Description)
                 .NotEmpty().WithMessage("A descrição do produto é obrigatória.")
                 .MaximumLength(255).WithMessage("A descrição do produto não pode exceder 255 caracteres.");

            RuleFor(product => product.Barcode)
                .NotEmpty().WithMessage("O código de barras do produto é obrigatório.")
                .MaximumLength(40).WithMessage("O código de barras do produto não pode exceder 40 caracteres.");

            RuleFor(product => product.Barcodetype)
                .NotEmpty().WithMessage("O tipo de código de barras do produto é obrigatório.")
                .MaximumLength(10).WithMessage("O tipo de código de barras do produto não pode exceder 10 caracteres.")
                .Must(BarCodeTypeValidate).WithMessage("O código de barras não é válido para o tipo especificado.");

            RuleFor(product => product.Price)
                .GreaterThan(0).WithMessage("O preço do produto deve ser maior que zero.");

            RuleFor(product => product.Costprice)
                .GreaterThan(0).WithMessage("O preço de custo do produto deve ser maior que zero.");
        }

        private bool BarCodeTypeValidate(ProductUpDTO dto, string barcodetype)
        {
            switch (barcodetype.ToUpper())
            {
                case "EAN-13":
                    return Regex.IsMatch(dto.Barcode, @"^\d{13}$");

                case "DUN-14":
                    return Regex.IsMatch(dto.Barcode, @"^\d{14}$");

                case "UPC":
                    return Regex.IsMatch(dto.Barcode, @"^\d{12}$");

                case "CODE 11":
                    return Regex.IsMatch(dto.Barcode, @"^[0-9\-\*]+$");

                case "CODE 39":
                    return Regex.IsMatch(dto.Barcode, @"^[A-Z0-9\-\.\/\+\%\*\s]+$");

                default:
                    return false;
            }
        }
    }
}
