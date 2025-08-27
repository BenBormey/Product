using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Models.Product;

namespace WebApplication1.Validator
{
    public class CreateProductValidator : AbstractValidator<CreateProductVM>
    {
        private readonly AppDbContext context;
     

          public CreateProductValidator(AppDbContext db)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be >= 0.");

            RuleFor(x => x.QtyInStock)
                .GreaterThanOrEqualTo(0).WithMessage("Qty must be >= 0.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category is required.");

            RuleFor(x => x.CodeOrBarcode)
                .NotEmpty().WithMessage("Barcode is required.")
                .MaximumLength(50)
                .MustAsync(async (barcode, ct) =>
                    !await db.products.AnyAsync(p => p.CodeOrBarcode == barcode, ct))
                .WithMessage("Barcode already exists.");
        }



    }
    
}
