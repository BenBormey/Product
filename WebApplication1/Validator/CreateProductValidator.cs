using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Models;

namespace WebApplication1.Validator
{
    public class CreateProductValidator : AbstractValidator<Product>
    {
        private readonly AppDbContext context;
     
        public CreateProductValidator( AppDbContext context)
        {
            this.context = context;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is .")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
         
            RuleFor(temp => temp.Price)
        .InclusiveBetween(0,
        int.MaxValue).WithMessage($"prinec shoulder between  0 to {int.MaxValue}");
            RuleFor(x => x.CodeOrBarcode)
                .NotEmpty().WithMessage("Code or barcode is required.")
                .MaximumLength(50).WithMessage("Code or barcode cannot exceed 50 characters.");
            RuleFor(temp => temp.QtyInStock)
          .InclusiveBetween(0,
          int.MaxValue).WithMessage($"Quantity In Stock shoulder between  0 to {int.MaxValue}");
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Category is required.");

            RuleFor(x => x.CodeOrBarcode)
      .NotEmpty().WithMessage("Barcode is required.")
      .MustAsync(async (barcode, cancellation) =>
      {
          // check DB for duplicates
          // example using EF Core
          return !await context.products
              .AnyAsync(p => p.CodeOrBarcode == barcode, cancellation);
      })
      .WithMessage("Barcode already exists.");


      


            
        }
    }
}
