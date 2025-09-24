using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NToastNotify;
using ProjectInventory.Dto;
using ProjectInventory.Models;
using ProjectInventory.Repository.Interface;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Controllers;

public class CategoryController(
    ICategoryRepository repository,
    ICategoryService service,
    IToastNotification toastNotification)
    : Controller
{
    public async Task<IActionResult> Index()
    {
            var categoryItems = await repository.GetAllAsync();
            return View(categoryItems);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryVm vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }
        try
        {
            var categoryList = new CategoryDto
            {
                Id = Guid.NewGuid(),
                Name = vm.Name,
                Description = vm.Description,
                IsActive = true
            };
            var isCreated = await service.CreateAsync(categoryList);
            if (isCreated)
            {
                toastNotification.AddSuccessToastMessage("Item added successfully");
            }
            else
            {
                toastNotification.AddErrorToastMessage("Unable to add item");
            }
        }
        catch (DbUpdateException ex1)
        {
            if (ex1.InnerException is PostgresException pgex)
            {
                Console.WriteLine("A postgres error occured: " + pgex.SqlState);
            }

            Console.WriteLine("An unexpected error occured: " + ex1.Message);
        }
        catch(Exception ex)
        {
            Console.WriteLine("An unexpected error occured..." + ex.Message);
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await repository.GetByIdAsync(id);
        var vm = new CategoryEditVm
        {
            Name = item.Name,
            Description = item.Description,
            IsActive = item.IsActive
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id,CategoryEditVm vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }
    
        try
        {
            var categorylist = new CategoryDto
            {
                Id = vm.Id,
                Name = vm.Name,
                Description = vm.Description,
                IsActive = vm.IsActive
            };
            var isUpdated = await service.EditAsync(id,categorylist);
            if (isUpdated)
            {
                toastNotification.AddSuccessToastMessage("An item has been updated");
                return RedirectToAction(nameof(Index));
            }

            toastNotification.AddErrorToastMessage("Failed to update an item");
            return View(vm);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Unable to Update an item {ex.Message}");
            return View(vm);
        }
    }

    public async Task<IActionResult> Remove(Guid id)
    {
        var item = await repository.GetByIdAsync(id);
        if (item == null)
            return NotFound();
        var category = new CategoryDeleteVm
        {
            Name = item.Name,
            Description = item.Description,
            IsActive = item.IsActive
        };
        return View(category);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveConfirmed(Guid id)
    {
        var isDeleted = await service.DeleteAsync(id);
        if (isDeleted)
            toastNotification.AddSuccessToastMessage("Item deleted successfully");
        else
            toastNotification.AddErrorToastMessage("Unable to delete the item");
        return RedirectToAction(nameof(Index));
    }
}
