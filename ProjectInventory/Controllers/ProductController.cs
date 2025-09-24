using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using ProjectInventory.Dto;
using ProjectInventory.Models;
using ProjectInventory.Repository.Interface;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Controllers;

public class ProductController(
    IProductRepository repository,
    IProductService service,
    ICategoryRepository categoryRepository,
    IUnitRepository unitRepository,
    IToastNotification toastNotification)
    : Controller
{
    public async Task<IActionResult> Index()
    {
        var items = await repository.GetAllAsync();
        return View(items);
    }

    public async Task<IActionResult> Create()
    {
        var vm = new ProductVm
        {
            Categories = await categoryRepository.GetAllSelectListAsync(),
            Units=await unitRepository.GetAllSelectListAsync()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductVm vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Units = await unitRepository.GetAllSelectListAsync();
            vm.Categories = await categoryRepository.GetAllSelectListAsync();
            return View(vm);
        }

        try
        {
            var items = new ProductDto
            {
                Id = Guid.NewGuid(),
                Name = vm.Name,
                Code = vm.Code,
                CostPrice = vm.CostPrice,
                Description = vm.Description,
                UnitId = vm.UnitId,
                CategoryId = vm.CategoryId,
                IsActive = vm.IsActive
            };
            var isSaved = await service.AddAsync(items);
            if (isSaved)
                toastNotification.AddSuccessToastMessage("Items Added Successfully");
            else
                toastNotification.AddErrorToastMessage("Unable to Add Items");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Unable to add items {ex.Message}");
            return View(vm);
        }
    }
    public async Task<IActionResult> Edit(Guid id)
    {
        var items = await repository.GetByIdAsync(id);
        if (items == null)
            return NotFound();
        var viewModel = new ProductEditVm
        {
            Name = items.Name,
            Code = items.Code,
            CostPrice = items.CostPrice,
            Description = items.Description,
            Units = await unitRepository.GetAllSelectListAsync(),
            Categories = await categoryRepository.GetAllSelectListAsync(),
            IsActive = items.IsActive
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ProductEditVm vm)
    {
        vm.Units = await unitRepository.GetAllSelectListAsync();
        vm.Categories = await categoryRepository.GetAllSelectListAsync();
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        try
        {
            var productDto = new ProductDto
            {
                Name = vm.Name,
                Code = vm.Code,
                CostPrice = vm.CostPrice,
                Description = vm.Description,
                UnitId = vm.UnitId,
                CategoryId = vm.CategoryId,
                IsActive = vm.IsActive,
            };
            var isUpdated = await service.UpdateAsync(id, productDto);
            if (isUpdated)
            {
                toastNotification.AddSuccessToastMessage("Item updated successfully");
                return RedirectToAction(nameof(Index));
            }
            toastNotification.AddErrorToastMessage("Failed to update an item...");
            return View(vm);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Unable to Update the operation {ex.Message}");
            return View(vm);
        }
    }
    public async Task<IActionResult> Remove(Guid id)
    {
        var items = await repository.GetByIdAsync(id);
       if (items == null)
       {
           return NotFound();
       }

       var vm = new ProductDeleteVm
       {
           Name = items.Name,
           Code = items.Code,
           CostPrice = items.CostPrice,
           Description = items.Description,
           IsActive = items.IsActive
       };
       return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveConfirmed(Guid id)
    {
            var isDeleted = await service.DeleteAsync(id);
            if (isDeleted)
                toastNotification.AddSuccessToastMessage("The item has been deleted successfully");
            else
                toastNotification.AddErrorToastMessage("Failed to delete an item");
            return RedirectToAction(nameof(Index));
    }
}