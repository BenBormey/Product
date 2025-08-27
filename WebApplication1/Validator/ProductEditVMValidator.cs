using FluentValidation;
using WebApplication1.Data;
using WebApplication1.Models.Product;

namespace WebApplication1.Validator
{
    public class ProductEditVMValidator : AbstractValidator<ProductEditVM>
    {
        public ProductEditVMValidator(AppDbContext db)
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
            RuleFor(x => x.qty).GreaterThanOrEqualTo(0);
            RuleFor(x => x.CategoryId).GreaterThan(0);
        }
        }
}
