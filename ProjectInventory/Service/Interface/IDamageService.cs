using ProjectInventory.Dto;
using ProjectInventory.Entities;

namespace ProjectInventory.Service.Interface;

public interface IDamageService
{
    Task<Damage> AddAsync(DamageDto dto);
}