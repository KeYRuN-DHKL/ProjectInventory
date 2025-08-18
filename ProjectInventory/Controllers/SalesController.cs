using Microsoft.AspNetCore.Mvc;
using ProjectInventory.Dto;
using ProjectInventory.Enum;
using ProjectInventory.Models;
using ProjectInventory.Repository.Interface;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Controllers;

public class SalesController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly IStakeHolderRepository _stakeHolderRepository;
    private readonly ISalesService _salesService;
    private readonly IStockMovementService _stockMovementService;
    private readonly ISalesRepository _salesRepository;

    public SalesController(IProductRepository productRepository, IStakeHolderRepository stakeholderRepository,
        ISalesService salesService, IStockMovementService stockMovementService, ISalesRepository salesRepository)
    {
        _productRepository = productRepository;
        _stakeHolderRepository = stakeholderRepository;
        _salesService = salesService;
        _stockMovementService = stockMovementService;
        _salesRepository = salesRepository;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _salesRepository.GetAllAsync();
        return View(items);
    }

    public async Task<IActionResult> Create()
    {
        try
        {
            var vm = new SalesVm();
            vm.Products = await _productRepository.GetAllSelectListAsync();
            vm.StakeHolders = await _stakeHolderRepository.GetAllSelectListAsync();
            var transactionDate = DateOnly.FromDateTime(DateTime.Now);
            var product = await _productRepository.GetAllAsync();
            vm.ProductUnitMap = product.ToDictionary(
                p => p.Id.ToString(),
                p => p.Unit.Symbol
            );
            return View(vm);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An unexcepted error occured...{ex.Message}");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SalesVm vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Products = await _productRepository.GetAllSelectListAsync();
            vm.StakeHolders = await _stakeHolderRepository.GetAllSelectListAsync();
            var transactionDate = DateOnly.FromDateTime(DateTime.Now);
            var product = await _productRepository.GetAllAsync();
            vm.ProductUnitMap = product.ToDictionary(
                p => p.Id.ToString(),
                p => p.Unit.Symbol
            );
            return View(vm);
        }

        try
        {
            var salesDto = new SalesDto()
            {
                InvoiceNumber = vm.InvoiceNo,
                TransactionDate = vm.TransactionDate,
                StakeHolderId = vm.StakeHolderId,
                TotalAmount = vm.TotalAmount,
                Description = vm.Description,
                DiscountAmount = vm.DiscountAmount,
                TaxableAmount = vm.TaxableAmount,
                TaxAmount = vm.TaxAmount
            };
            var sales = await _salesService.CreateAsync(salesDto);
            var stockMovementsDto = vm.StockMovements.Select(sm => new StockMovementDto
            {

                ProductId = sm.ProductId,
                Quantity = sm.Quantity,
                MovementType = MovementType.Sale,
                InvoiceNumber = vm.InvoiceNo,
                Rate = sm.Rate,
                Date = vm.TransactionDate,
                VatPercentage = sm.VatPercentage,
                TypeId = sales.Id,
                Stock = Stock.Out,
            }).ToList();
            await _stockMovementService.AddAsync(stockMovementsDto);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An unexpected error occured: {ex.Message}");
            vm.Products = await _productRepository.GetAllSelectListAsync();
            vm.StakeHolders = await _stakeHolderRepository.GetAllSelectListAsync();
            var transactionDate = DateOnly.FromDateTime(DateTime.Now);
            var product = await _productRepository.GetAllAsync();
            vm.ProductUnitMap = product.ToDictionary(
                p => p.Id.ToString(),
                p => p.Unit.Symbol
            );
            return View(vm);
        }
    }
}