using ProjectInventory.Dto;
namespace ProjectInventory.Service.Interface;
public interface IStakeHolderService
{
    Task<bool> AddAsync(StakeHolderDto dto);
    
    Task<bool> UpdateAsync(Guid id,StakeHolderDto dto);

    Task<bool> DeleteAsync(Guid id);
}