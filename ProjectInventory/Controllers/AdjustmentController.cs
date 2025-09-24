using Microsoft.AspNetCore.Mvc;
using ProjectInventory.Dto;
using ProjectInventory.Entities;
using ProjectInventory.Enum;
using ProjectInventory.Models;
using ProjectInventory.Repository.Interface;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Controllers;

public class AdjustmentController(
    IAdjustmentService adjustmentService,
    IProductRepository productRepository,
    IStockMovementService stockmovementService)
    : Controller
{
    public async Task<IActionResult> Create()
    {
        try
        {
            var vm = new AdjustmentVm();
            vm.Products = await productRepository.GetAllSelectListAsync();
            vm.Date = DateOnly.FromDateTime(DateTime.Now);
            vm.InvoiceNo = await adjustmentService.GetInvoiceNumber();
            var products = await productRepository.GetAllAsync();
            vm.ProductUnitMap = products.ToDictionary(
                p => p.Id.ToString(),
                p => p.Unit.Symbol
            );
            return View(vm);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An unexpected error occured {ex.Message}");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdjustmentVm vm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                vm.Products = await productRepository.GetAllSelectListAsync();
                vm.Date = DateOnly.FromDateTime(DateTime.Now);
                vm.InvoiceNo = await adjustmentService.GetInvoiceNumber();
                var products = await productRepository.GetAllAsync();
                vm.ProductUnitMap = products.ToDictionary(
                    p => p.Id.ToString(),
                    p => p.Unit.Symbol
                );
                return View(vm);
            }

            var dto = new AdjustmentDto()
            {
                InvoiceNo = vm.InvoiceNo,
                Date = vm.Date,
                Amount = vm.Amount,
                Description = vm.Description
            };

            var adjustment = await adjustmentService.AddAsync(dto);
            var stockMovementsDto = vm.StockMovements.Select(sm => new StockMovementDto
            {

                ProductId = sm.ProductId,
                Quantity = sm.Quantity,
                MovementType = MovementType.Adjustment,
                InvoiceNumber = vm.InvoiceNo,
                Rate = sm.Rate,
                Date = vm.Date,
                TypeId = adjustment.Id,
                Stock = sm.Stock,
            }).ToList();
            await stockmovementService.AddAsync(stockMovementsDto);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An unexpected error occured {ex.Message}");
            vm.Products = await productRepository.GetAllSelectListAsync();
            vm.Date = DateOnly.FromDateTime(DateTime.Now);
            vm.InvoiceNo = await adjustmentService.GetInvoiceNumber();
            var products = await productRepository.GetAllAsync();
            vm.ProductUnitMap = products.ToDictionary(
                p => p.Id.ToString(),
                p => p.Unit.Symbol
            );
            return View(vm);
        }
    }
}