using Microsoft.AspNetCore.Mvc;
using ProjectInventory.Dto;
using ProjectInventory.Enum;
using ProjectInventory.Models;
using ProjectInventory.Repository.Interface;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Controllers;

public class OpeningController(
    IOpeningService openingService,
    IProductRepository productRepository,
    IStockMovementService stockMovementService)
    : Controller
{
    public async Task<IActionResult> Create()
    {
        OpeningVm vm = new OpeningVm();
        vm.Products = await productRepository.GetAllSelectListAsync();
        vm.Date = DateOnly.FromDateTime(DateTime.Now);
        var products = await productRepository.GetAllAsync();
        vm.ProductUnitMap = products.ToDictionary(
            p => p.Id.ToString(),
            p => p.Unit.Symbol
        );
        return View(vm);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OpeningVm vm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                vm.Products = await productRepository.GetAllSelectListAsync();
                vm.Date = DateOnly.FromDateTime(DateTime.Now);
                var products = await productRepository.GetAllAsync();
                vm.ProductUnitMap = products.ToDictionary(
                    p => p.Id.ToString(),
                    p => p.Unit.Symbol
                );
                return View(vm);
            }

            var dto = new OpeningDto()
            {
                Id = Guid.NewGuid(),
                Date = vm.Date,
                Amount = vm.Amount,
                Description = vm.Description
            };

            var opening = await openingService.AddAsync(dto);
            var stockMovementsDto = vm.StockMovements.Select(sm => new StockMovementDto
            {

                ProductId = sm.ProductId,
                Quantity = sm.Quantity,
                MovementType = MovementType.Opening,
                Rate = sm.Rate,
                Date = vm.Date,
                TypeId = opening.Id,
                Stock = sm.Stock,
            }).ToList();
            await stockMovementService.AddAsync(stockMovementsDto);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An unexpected error occured {ex.Message}");
            vm.Products = await productRepository.GetAllSelectListAsync();
            vm.Date = DateOnly.FromDateTime(DateTime.Now);
            var products = await productRepository.GetAllAsync();
            vm.ProductUnitMap = products.ToDictionary(
                p => p.Id.ToString(),
                p => p.Unit.Symbol
            );
            return View(vm);
        }
    }
}