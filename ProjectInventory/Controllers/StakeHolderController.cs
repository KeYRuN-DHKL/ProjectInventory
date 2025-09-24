using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using ProjectInventory.Dto;
using ProjectInventory.Models;
using ProjectInventory.Repository.Interface;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Controllers;

public class StakeHolderController(
    IStakeHolderRepository repository,
    IStakeHolderService service,
    IToastNotification toastNotification)
    : Controller
{
    public async Task<IActionResult> Index()
    {
            var items = await repository.GetAllAsync();
            return View(items);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StakeHolderVm vm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var items = new StakeHolderDto
            {
                Id = Guid.NewGuid(),
                Name = vm.Name,
                Address = vm.Address,
                Email = vm.Email,
                PhoneNumber = vm.PhoneNumber,
                Type = vm.Type,
                VatNo = vm.VatNo,
                IsActive = vm.IsActive
            };
            bool isCreated = await service.AddAsync(items);
            if (isCreated)
                toastNotification.AddSuccessToastMessage("StakeHolder Added Successfully");
            else
                toastNotification.AddErrorToastMessage("Unable to add StakeHolder");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Unable to add stakeholders {ex.Message}");
            return View(vm);
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var item = await repository.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        var itemList = new StakeHolderEditVm
        {
            Name = item.Name,
            Address = item.Address,
            Email = item.Email,
            PhoneNumber = item.PhoneNumber,
            Type = item.Type,
            VatNo = item.VatNo,
            IsActive = item.IsActive
        };
        return View(itemList);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, StakeHolderEditVm vm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var items = new StakeHolderDto
            {
                Name = vm.Name,
                Address = vm.Address,
                Email = vm.Email,
                PhoneNumber = vm.PhoneNumber,
                Type = vm.Type,
                VatNo = vm.VatNo,
                IsActive = vm.IsActive
            };
            var isUpdated = await service.UpdateAsync(id, items);
            if (isUpdated)
                toastNotification.AddSuccessToastMessage("Stakeholder Updated Successfully");
            else
                toastNotification.AddErrorToastMessage("Unable to update an stakeholder");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Unable to Add item: {ex.Message}");
            return View(vm);
        }
    }

    public async Task<IActionResult> Remove(Guid id)
    {
        var item = await repository.GetByIdAsync(id);
        if (item == null)
            return NotFound();
        var stakeHolder = new StakeHolderDeleteVm
        {
            Name = item.Name,
            Address = item.Address,
            Email = item.Email,
            PhoneNumber = item.PhoneNumber,
            Type = item.Type,
            VatNo = item.VatNo,
            IsActive = item.IsActive
        };
        return View(stakeHolder);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveConfirmed(Guid id)
    {
        var isDeleted = await service.DeleteAsync(id);
        if (!isDeleted)
            toastNotification.AddSuccessToastMessage("StakeHolder Deleted Successfully");
        else
            toastNotification.AddErrorToastMessage("Unable to delete an stakeholder");
        return RedirectToAction(nameof(Index));
    }
    
}