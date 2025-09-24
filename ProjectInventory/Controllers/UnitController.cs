using Microsoft.AspNetCore.Mvc;
using ProjectInventory.Dto;
using ProjectInventory.Models;
using ProjectInventory.Repository.Interface;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Controllers;

public class UnitController(IUnitRepository repository, IUnitService service) : Controller
{
    public async Task<IActionResult> Index()
    {
        var items =await  repository.GetAllAsync();
        return View(items);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UnitVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var units = new UnitDto
        {
            Name = vm.Name,
            Symbol = vm.Symbol,
            Description = vm.Description,
        };
        await service.AddAsync(units);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var units = await repository.GetByIdAsync(id);
        if (units == null)
            return NotFound();
        
        var listOfUnits = new UnitEditVm
        {
            Name = units.Name,
            Symbol = units.Symbol,
            Description = units.Description,
            IsActive = units.IsActive
        };
        return View(listOfUnits);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UnitEditVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        try
        {
            var items = new UnitDto
            {
                Name = vm.Name,
                Symbol = vm.Symbol,
                Description = vm.Description,
                IsActive = vm.IsActive
            };

            await service.UpdateAsync(id, items);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            //ModelState only works with the current context. Not during the RedirectToAction
            ModelState.AddModelError("", $"Something Went Wrong: {ex.Message}");
            return View(vm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(Guid id)
    {
        try
        {
            await service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
