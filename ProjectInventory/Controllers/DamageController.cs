using Microsoft.AspNetCore.Mvc;
using ProjectInventory.Dto;
using ProjectInventory.Enum;
using ProjectInventory.Models;
using ProjectInventory.Repository.Interface;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Controllers;

public class DamageController:Controller
{
    private readonly IDamageService _damageService;
    private readonly IProductRepository _productRepository;
    private readonly IStockMovementService _stockMovementService;

    public DamageController(IDamageService damageService, IProductRepository productRepository, IStockMovementService stockMovementService)
    {
        _damageService = damageService;
        _productRepository = productRepository;
        _stockMovementService = stockMovementService;
    }

    public async Task<IActionResult> Create()
    {
        DamageVm vm = new DamageVm();
        vm.Products = await _productRepository.GetAllSelectListAsync();
        vm.Date = DateOnly.FromDateTime(DateTime.Now);
        var products = await _productRepository.GetAllAsync();
        vm.ProductUnitMap = products.ToDictionary(
            p => p.Id.ToString(),
            p => p.Unit.Symbol
        );
        return View(vm);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DamageVm vm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                vm.Products = await _productRepository.GetAllSelectListAsync();
                vm.Date = DateOnly.FromDateTime(DateTime.Now);
                var products = await _productRepository.GetAllAsync();
                vm.ProductUnitMap = products.ToDictionary(
                    p => p.Id.ToString(),
                    p => p.Unit.Symbol
                );
                return View(vm);
            }

            var dto = new DamageDto()
            {
                Id = Guid.NewGuid(),
                Date = vm.Date,
                Amount = vm.Amount,
                Description = vm.Description
            };

            var damage = await _damageService.AddAsync(dto);
            var stockMovementsDto = vm.StockMovements.Select(sm => new StockMovementDto
            {

                ProductId = sm.ProductId,
                Quantity = sm.Quantity,
                MovementType = MovementType.Damage,
                Rate = sm.Rate,
                Date = vm.Date,
                TypeId = damage.Id,
                Stock = Stock.Out,
            }).ToList();
            await _stockMovementService.AddAsync(stockMovementsDto);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An unexpected error occured {ex.Message}");
            vm.Products = await _productRepository.GetAllSelectListAsync();
            vm.Date = DateOnly.FromDateTime(DateTime.Now);
            var products = await _productRepository.GetAllAsync();
            vm.ProductUnitMap = products.ToDictionary(
                p => p.Id.ToString(),
                p => p.Unit.Symbol
            );
            return View(vm);
        }
    }
}