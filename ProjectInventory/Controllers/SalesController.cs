using Microsoft.AspNetCore.Mvc;
using NToastNotify;
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
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IToastNotification _toastNotification;

    public SalesController(IProductRepository productRepository, IStakeHolderRepository stakeholderRepository,
        ISalesService salesService, IStockMovementService stockMovementService, ISalesRepository salesRepository, IStockMovementRepository stockMovementRepository, IToastNotification toastNotification)
    {
        _productRepository = productRepository;
        _stakeHolderRepository = stakeholderRepository;
        _salesService = salesService;
        _stockMovementService = stockMovementService;
        _salesRepository = salesRepository;
        _stockMovementRepository = stockMovementRepository;
        _toastNotification = toastNotification;
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
            vm.TransactionDate = DateOnly.FromDateTime(DateTime.Now);
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
    
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var sales = await _salesRepository.FindById(id);
            var salesItems = await _stockMovementRepository.FindByIdAsync(id);
            var stakeHolders = await _stakeHolderRepository.GetAllSelectListAsync();
            var products = await _productRepository.GetAllSelectListAsync();
            var vm = new SalesEditVm
            {
                Id = sales.Id,
                InvoiceNumber = sales.InvoiceNo,
                Description = sales.Description ?? string.Empty,
                DiscountAmount = sales.DiscountAmount,
                StakeHolderId = sales.StakeHolderId,
                TotalAmount = sales.TotalAmount,
                TaxableAmount = sales.TaxableAmount,
                TaxAmount = sales.TaxAmount,
                TransactionDate = sales.TransactionDate,

                StockMovements = salesItems.Select(sm => new StockMovementEditVm
                {
                    Id = sm.Id,
                    ProductId = sm.ProductId,
                    Quantity = sm.Quantity,
                    Rate = sm.Rate,
                    VatPercentage = sm.VatPercentage,
                    UnitName = sm.Product.Unit.Name
                }).ToList(),

                Products = products,
                StakeHolders = stakeHolders,
            };
            var allProducts = await _productRepository.GetAllAsync();
            vm.ProductUnitMap = allProducts.ToDictionary(
                p => p.Id.ToString(),
                p => p.Unit.Symbol
            );
            return View(vm);
        }
        catch (Exception ex)
        {
            _toastNotification.AddErrorToastMessage("An unexpected error occured..." + ex.Message);
            return RedirectToAction("Index");
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SalesEditVm vm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                vm.Products = await _productRepository.GetAllSelectListAsync();
                vm.StakeHolders = await _stakeHolderRepository.GetAllSelectListAsync();
                vm.TransactionDate = DateOnly.FromDateTime(DateTime.Now);
                var products = await _productRepository.GetAllAsync();
                vm.ProductUnitMap = products.ToDictionary(
                    p => p.Id.ToString(),
                    p => p.Unit.Symbol
                );
                return View(vm);
            }
            
            var salesDto = new SalesDto
            {
                Id = vm.Id,
                InvoiceNumber = vm.InvoiceNumber,
                Description = vm.Description,
                DiscountAmount = vm.DiscountAmount,
                StakeHolderId = vm.StakeHolderId,
                TaxableAmount = vm.TaxableAmount,
                TaxAmount = vm.TaxAmount,
                TransactionDate = vm.TransactionDate,
                TotalAmount = vm.TotalAmount,
            };

            var updatedSales = await _salesService.UpdateAsync(salesDto);
            var existingStockMovement = await _stockMovementRepository.FindByIdAsync(vm.Id);

            foreach (var item in existingStockMovement)
            {
                if (vm.StockMovements.All(x => x.Id != item.Id))
                {
                    await _stockMovementService.DeleteAsync(item.Id);
                }

                if (vm.StockMovements.All(x => x.Id == item.Id))
                {
                    var newStockMovement = vm.StockMovements.First(x => x.Id == item.Id);
                    var stockMovementDto = new StockMovementDto
                    {
                        Id = item.Id,
                        Date = vm.TransactionDate,
                        InvoiceNumber = vm.InvoiceNumber,
                        ProductId = newStockMovement.ProductId,
                        Rate = newStockMovement.Rate,
                        Quantity = newStockMovement.Quantity,
                        TypeId = updatedSales.Id,
                        VatPercentage = newStockMovement.VatPercentage,
                    };
                    await _stockMovementService.UpdateAsync(stockMovementDto);
                }
            }

            foreach (var item in vm.StockMovements)
            {
                if (existingStockMovement.All(x => x.Id != item.Id) && item.Id == Guid.Empty)
                {
                    var stockMovementDto = new StockMovementDto
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Date = vm.TransactionDate,
                        InvoiceNumber = vm.InvoiceNumber,
                        MovementType = MovementType.Purchase,
                        Stock = Stock.In,
                        Rate = item.Rate,
                        TypeId = updatedSales.Id,
                        VatPercentage = item.VatPercentage,
                    };
                    await _stockMovementService.AddAsync(stockMovementDto);
                }
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _toastNotification.AddErrorToastMessage(ex.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Return(Guid id)
    {
        try
        {
            var sales= await _salesRepository.FindById(id);
            var salesItems = await _stockMovementRepository.FindByIdAsync(id);
            var vm = new SalesReturnVm
            {
                Id = sales.Id,
                InvoiceNumber = sales.InvoiceNo,
                Description = sales.Description ?? "",
                DiscountAmount = sales.DiscountAmount,
                StakeHolderName = await _stakeHolderRepository.GetStakeHolderName(sales.StakeHolderId),
                StakeHolderId = sales.StakeHolderId,
                TotalAmount = sales.TotalAmount,
                TaxableAmount = sales.TaxableAmount,
                TaxAmount = sales.TaxAmount,
                TransactionDate = sales.TransactionDate,

                StockMovements = salesItems.Select(sm => new StockMovementReturnVm
                {
                    Id = sm.Id,
                    ProductId = sm.ProductId,
                    Quantity = sm.Quantity,
                    ProductName = sm.Product.Name,
                    Rate = sm.Rate,
                    VatPercentage = sm.VatPercentage,
                    UnitName = sm.Product.Unit.Name,
                }).ToList(),
            };
            var allProducts = await _productRepository.GetAllAsync();
            vm.ProductUnitMap = allProducts.ToDictionary(
                p => p.Id.ToString(),
                p => p.Unit.Symbol
            );
            return View(vm);
        }
        catch (Exception ex)
        {
            _toastNotification.AddErrorToastMessage("An unexpected error occured..." + ex.Message);
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(PurchaseReturnVm viewModel)
    {
            try
            {
                if (!ModelState.IsValid)
                {
                    var salesData = await _salesRepository.FindById(viewModel.Id);
                    var salesItems = await _stockMovementRepository.FindByIdAsync(viewModel.Id);
                    var vm = new SalesReturnVm
                    {
                    Id = salesData.Id,
                    InvoiceNumber = salesData.InvoiceNo,
                    Description = salesData.Description ?? "",
                    DiscountAmount = salesData.DiscountAmount,
                    StakeHolderName = await _stakeHolderRepository.GetStakeHolderName(salesData.StakeHolderId),
                    TotalAmount = salesData.TotalAmount,
                    TaxableAmount = salesData.TaxableAmount,
                    TaxAmount = salesData.TaxAmount,
                    TransactionDate = salesData.TransactionDate,

                    StockMovements = salesItems.Select(sm => new StockMovementReturnVm
                    {
                        Id = sm.Id,
                        ProductId = sm.ProductId,
                        Quantity = sm.Quantity,
                        ProductName = sm.Product.Name,
                        Rate = sm.Rate,
                        VatPercentage = sm.VatPercentage,
                        UnitName = sm.Product.Unit.Name,
                    }).ToList(),
                    };
                    var allProducts = await _productRepository.GetAllAsync();
                    vm.ProductUnitMap = allProducts.ToDictionary(
                        p => p.Id.ToString(),
                        p => p.Unit.Symbol
                    );
                    return View(vm);
                } 
                
                var salesDto = new SalesDto
                {
                InvoiceNumber = viewModel.InvoiceNumber,
                Description = viewModel.Description ?? string.Empty,
                TransactionDate = viewModel.TransactionDate,
                StakeHolderId = viewModel.StakeHolderId,
                TotalAmount = viewModel.TotalAmount,
                DiscountAmount = viewModel.DiscountAmount,
                TaxAmount = viewModel.TaxAmount,
                TaxableAmount = viewModel.TaxableAmount,
                Status = Status.Returned,
                };

            var sales = await _salesService.CreateAsync(salesDto);
            var stockMovementsDto = viewModel.StockMovements.Select(sm => new StockMovementDto
            {

                ProductId = sm.ProductId,
                Quantity = sm.Quantity,
                MovementType = MovementType.SalesReturn,
                InvoiceNumber = viewModel.InvoiceNumber,
                Rate = sm.Rate,
                Date = viewModel.TransactionDate,
                VatPercentage = sm.VatPercentage,
                TypeId = sales.Id,
                Stock = Stock.In,
            }).ToList();
            await _stockMovementService.AddAsync(stockMovementsDto);
            return RedirectToAction("Index"); 
            }
            catch (Exception ex)
            {
                var sales = await _salesRepository.FindById(viewModel.Id);
                var salesItems = await _stockMovementRepository.FindByIdAsync(viewModel.Id);
                var vm = new SalesReturnVm
            {
                Id = sales.Id,
                InvoiceNumber = sales.InvoiceNo,
                Description = sales.Description ?? string.Empty,
                DiscountAmount = sales.DiscountAmount,
                StakeHolderName = await _stakeHolderRepository.GetStakeHolderName(sales.StakeHolderId),
                TotalAmount = sales.TotalAmount,
                TaxableAmount = sales.TaxableAmount,
                TaxAmount = sales.TaxAmount,
                TransactionDate = sales.TransactionDate,

                StockMovements = salesItems.Select(sm => new StockMovementReturnVm
                {
                    Id = sm.Id,
                    ProductId = sm.ProductId,
                    Quantity = sm.Quantity,
                    ProductName = sm.Product.Name,
                    Rate = sm.Rate,
                    VatPercentage = sm.VatPercentage,
                    UnitName = sm.Product.Unit.Name,
                }).ToList(),
            };
            var allProducts = await _productRepository.GetAllAsync();
            vm.ProductUnitMap = allProducts.ToDictionary(
                p => p.Id.ToString(),
                p => p.Unit.Symbol
            );
            return View(vm);
            }
    }
}
