using ProjectInventory.Dto;
using ProjectInventory.Entities;

namespace ProjectInventory.Service.Interface;

public interface IOpeningService
{
    Task<Opening> AddAsync(OpeningDto dto);
}