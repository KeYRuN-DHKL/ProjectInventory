using Microsoft.AspNetCore.Mvc;
using ProjectInventory.Dto;
using ProjectInventory.Enum;
using ProjectInventory.Models;
using ProjectInventory.Repository.Interface;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Controllers;

public class PurchaseController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly IStakeHolderRepository _stakeholderRepository;
    private readonly IPurchaseService _purchaseService;
    private readonly IStockMovementService _stockMovementService;

    public PurchaseController(IProductRepository productRepository, IStakeHolderRepository stakeholderRepository, IPurchaseService purchaseService, IStockMovementService stockMovementService)
    {
        _productRepository = productRepository;
        _stakeholderRepository = stakeholderRepository;
        _purchaseService = purchaseService;
        _stockMovementService = stockMovementService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Create()
    {
        try
        {
            var vm = new PurchaseVm();
            vm.Products = await _productRepository.GetAllSelectListAsync();
            vm.StakeHolders = await _stakeholderRepository.GetAllSelectListAsync();
            vm.TransactionDate = DateOnly.FromDateTime(DateTime.Now);
            var products = await _productRepository.GetAllAsync();
            vm.ProductUnitMap = products.ToDictionary(
                p => p.Id.ToString(),
                p => p.Unit.Symbol
            );
            return View(vm);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An error occured while Purchasing the item {ex.Message}");
            return View();
        }
    }
   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PurchaseVm vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Products = await _productRepository.GetAllSelectListAsync();
            vm.StakeHolders = await _stakeholderRepository.GetAllSelectListAsync();
            vm.TransactionDate = DateOnly.FromDateTime(DateTime.Now);
            var products = await _productRepository.GetAllAsync();
            vm.ProductUnitMap = products.ToDictionary(
                p => p.Id.ToString(),
                p => p.Unit.Symbol
                );
            return View(vm);
        }

        try
        {
            var purchaseDto = new PurchaseDto
            {
                InvoiceNumber = vm.InvoiceNumber,
                Description = vm.Description ?? string.Empty,
                TransactionDate = vm.TransactionDate,
                StakeHolderId = vm.StakeHolderId,
                TotalAmount = vm.TotalAmount,
                DiscountAmount = vm.DiscountAmount,
                TaxAmount = vm.TaxAmount,
                TaxableAmount = vm.TaxableAmount,
            };

            var purchase = await _purchaseService.CreateAsync(purchaseDto);
            var stockMovementsDto = vm.StockMovements.Select(sm => new StockMovementDto
            {

                ProductId = sm.ProductId,
                Quantity = sm.Quantity,
                MovementType = MovementType.Purchase,
                InvoiceNumber = vm.InvoiceNumber,
                Rate = sm.Rate,
                Date = vm.TransactionDate,
                VatPercentage = sm.VatPercentage,
                TypeId = purchase.Id,
                Stock = Stock.In,
            }).ToList();
            await _stockMovementService.AddAsync(stockMovementsDto);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "An unexpected error occured {ex.Message}");
            vm.Products= await _productRepository.GetAllSelectListAsync();
            vm.StakeHolders = await _stakeholderRepository.GetAllSelectListAsync();
            vm.TransactionDate = DateOnly.FromDateTime(DateTime.Now);
            var products = await _productRepository.GetAllAsync();
            vm.ProductUnitMap = products.ToDictionary(
            p=> p.Id.ToString(),
            p=> p.Unit.Symbol
        ); 
            return View(vm);
        }
    }
}