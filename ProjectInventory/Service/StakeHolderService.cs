using Microsoft.EntityFrameworkCore;
using ProjectInventory.Data;
using ProjectInventory.Dto;
using ProjectInventory.Entities;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Service;

public class StakeHolderService(ApplicationDbContext context) : IStakeHolderService
{
    public async Task<bool> AddAsync(StakeHolderDto dto)
    {
        var isExisted = await context.StakeHolders.AnyAsync(c =>
            c.VatNo == dto.VatNo && c.PhoneNumber == dto.PhoneNumber && c.Email == dto.Email);
        if (isExisted)
            throw new InvalidOperationException("Person Already Exists");
        
        var items = new StakeHolder
        {
            Id = dto.Id,
            Name = dto.Name,
            Address = dto.Address,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Type = dto.Type,
            VatNo = dto.VatNo,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };
        context.StakeHolders.Add(items);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Guid id,StakeHolderDto dto)
    {
        var items = await context.StakeHolders.FindAsync(id);
        
        if (items == null)
            throw new Exception($"Item with the Id : {id} not available...");
        
        items.Name = dto.Name;
        items.Address = dto.Address;
        items.Email = dto.Email;
        items.PhoneNumber = dto.PhoneNumber;
        items.Type = dto.Type;
        items.VatNo = dto.VatNo;
        items.IsActive = dto.IsActive;
        items.UpdatedAt = DateTime.UtcNow;

        context.StakeHolders.Update(items);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var item = await context.StakeHolders.FindAsync(id);
        if (item == null)
            throw new InvalidOperationException($"Item with the Id: {id} not found");
        context.StakeHolders.Remove(item);
        return await context.SaveChangesAsync() > 0;
    }
}