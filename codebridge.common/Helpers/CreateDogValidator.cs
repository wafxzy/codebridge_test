using codebridge.Common.DTO_s;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codebridge.Common.Helpers
{
    public class CreateDogDtoValidator : AbstractValidator<CreateDogDto>
    {
        public CreateDogDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .Length(1, 50).WithMessage("Name must be between 1 and 50 characters");

            RuleFor(x => x.Color)
                .NotEmpty().WithMessage("Color is required")
                .Length(1, 100).WithMessage("Color must be between 1 and 100 characters");

            RuleFor(x => x.TailLength)
                .GreaterThanOrEqualTo(0).WithMessage("Tail length must be a positive number or zero");

            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("Weight must be a positive number");
        }
    }

    public class DogQueryParamsValidator : AbstractValidator<DogQueryParams>
    {
        private readonly string[] _validAttributes = { "name", "color", "tail_length", "taillength", "weight" };
        private readonly string[] _validOrders = { "asc", "desc" };

        public DogQueryParamsValidator()
        {
            RuleFor(x => x.Attribute)
                .Must(attr => string.IsNullOrEmpty(attr) || _validAttributes.Contains(attr.ToLower()))
                .WithMessage($"Attribute must be one of: {string.Join(", ", _validAttributes)}");

            RuleFor(x => x.Order)
                .Must(order => string.IsNullOrEmpty(order) || _validOrders.Contains(order.ToLower()))
                .WithMessage($"Order must be one of: {string.Join(", ", _validOrders)}");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");
        }
    }
}
