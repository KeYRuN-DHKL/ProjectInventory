using Microsoft.EntityFrameworkCore;
using ProjectInventory.Data;
using ProjectInventory.Dto;
using ProjectInventory.Entities;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Service;

public class ProductService(ApplicationDbContext context) : IProductService
{
    public async Task<bool> AddAsync(ProductDto dto)
    {
        var product = await context.Products.AnyAsync(p => p.Code == dto.Code);
        if (product)
            throw new InvalidOperationException("Product is already available....");
        var productEntity = new Product
        {
            Id = dto.Id,
            Name = dto.Name,
            Code = dto.Code,
            CostPrice = dto.CostPrice,
            Description = dto.Description,
            UnitId = dto.UnitId,
            CategoryId = dto.CategoryId,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };
        context.Products.Add(productEntity);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Guid id, ProductDto dto)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Product not available");
        
        var isProductExisted = await context.Products.AnyAsync(p => p.Code == dto.Code && p.Name == dto.Name);
        if (isProductExisted)
            throw new InvalidOperationException($"Product Already Exists");

        product.Name = dto.Name;
        product.Code = dto.Code;
        product.CostPrice = dto.CostPrice;
        product.Description = dto.Description;
        product.UnitId = dto.UnitId;
        product.CategoryId = dto.CategoryId;
        product.IsActive = dto.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        context.Products.Update(product);
        return await context.SaveChangesAsync() > 0;
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var item = await context.Products.FindAsync(id);
        if (item == null)
            throw new InvalidOperationException("Item not found");
        context.Products.Remove(item);
        return await context.SaveChangesAsync() > 0;
    }
}