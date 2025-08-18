using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NToastNotify;
using ProjectInventory.Dto;
using ProjectInventory.Models;
using ProjectInventory.Repository.Interface;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Controllers;

public class CategoryController:Controller
{
    private readonly ICategoryRepository _repository;
    private readonly ICategoryService _service;
    private readonly IToastNotification _toastNotification;

    public CategoryController(ICategoryRepository repository,ICategoryService service,IToastNotification toastNotification)
    {
        _repository = repository;
        _service = service;
        _toastNotification = toastNotification;
    }

    public async Task<IActionResult> Index()
    {
            var categoryItems = await _repository.GetAllAsync();
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
            var isCreated = await _service.CreateAsync(categoryList);
            if (isCreated)
            {
                _toastNotification.AddSuccessToastMessage("Item added successfully");
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Unable to add item");
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
        var item = await _repository.GetByIdAsync(id);
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
            var isUpdated = await _service.EditAsync(id,categorylist);
            if (isUpdated)
            {
                _toastNotification.AddSuccessToastMessage("An item has been updated");
                return RedirectToAction(nameof(Index));
            }

            _toastNotification.AddErrorToastMessage("Failed to update an item");
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
        var item = await _repository.GetByIdAsync(id);
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
        var isDeleted = await _service.DeleteAsync(id);
        if (isDeleted)
            _toastNotification.AddSuccessToastMessage("Item deleted successfully");
        else
            _toastNotification.AddErrorToastMessage("Unable to delete the item");
        return RedirectToAction(nameof(Index));
    }
}
