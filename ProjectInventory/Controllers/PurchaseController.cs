using Microsoft.AspNetCore.Mvc;
using NToastNotify;
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
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IStockMovementService _stockMovementService;
    private readonly IToastNotification _toastNotification;
    private readonly IStockMovementRepository _stockMovementRepository;

    public PurchaseController(IProductRepository productRepository, IStakeHolderRepository stakeholderRepository, IPurchaseService purchaseService, IStockMovementService stockMovementService, IPurchaseRepository purchaseRepository, IToastNotification toastNotification, IStockMovementRepository stockMovementRepository)
    {
        _productRepository = productRepository;
        _stakeholderRepository = stakeholderRepository;
        _purchaseService = purchaseService;
        _stockMovementService = stockMovementService;
        _purchaseRepository = purchaseRepository;
        _toastNotification = toastNotification;
        _stockMovementRepository = stockMovementRepository;
    }

    public IActionResult Index()
    {
        var items = _purchaseRepository.GetQueryAbleData();
        var vm = items.Select(p => new PurchaseIndexVm
        {
            Id = p.Id,
            StakeHolderName = p.StakeHolder.Name,
            Description = p.Description,
            DiscountAmount = p.DiscountAmount,
            InvoiceNumber = p.InvoiceNumber,
            TaxableAmount = p.TaxableAmount,
            TaxAmount = p.TaxAmount,
            TotalAmount = p.TotalAmount,
            Status = p.Status,
            TransactionDate = p.TransactionDate
        }).ToList();
        return View(vm);
    }

    public async Task<IActionResult> Create()
    {
        try
        {
            var vm = new PurchaseVm
            {
                Products = await _productRepository.GetAllSelectListAsync(),
                StakeHolders = await _stakeholderRepository.GetAllSelectListAsync(),
                TransactionDate = DateOnly.FromDateTime(DateTime.Now)
            };
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
            ModelState.AddModelError("", $"An unexpected error occured {ex.Message}");
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
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var purchase = await _purchaseRepository.FindById(id);
            var purchaseItems = await _stockMovementRepository.FindByIdAsync(id);
            var stakeHolders = await _stakeholderRepository.GetAllSelectListAsync();
            var products = await _productRepository.GetAllSelectListAsync();
            var vm = new PurchaseEditvm
            {
                Id = purchase.Id,
                InvoiceNumber = purchase.InvoiceNumber,
                Description = purchase.Description,
                DiscountAmount = purchase.DiscountAmount,
                StakeHolderId = purchase.StakeHolderId,
                TotalAmount = purchase.TotalAmount,
                TaxableAmount = purchase.TaxableAmount,
                TaxAmount = purchase.TaxAmount,
                TransactionDate = purchase.TransactionDate,

                StockMovements = purchaseItems.Select(sm => new StockMovementEditVm
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
    public async Task<IActionResult> Edit(PurchaseEditvm vm)
    {
        try
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

            // var purchase = await _purchaseRepository.FindById(id);
            var purchaseDto = new PurchaseDto
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

            var updatedPurchase = await _purchaseService.UpdateAsync(purchaseDto);
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
                        TypeId = updatedPurchase.Id,
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
                        TypeId = updatedPurchase.Id,
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
            var purchase = await _purchaseRepository.FindById(id);
            var purchaseItems = await _stockMovementRepository.FindByIdAsync(id);
            var vm = new PurchaseReturnVm
            {
                Id = purchase.Id,
                InvoiceNumber = purchase.InvoiceNumber,
                Description = purchase.Description,
                DiscountAmount = purchase.DiscountAmount,
                StakeHolderName = await _stakeholderRepository.GetStakeHolderName(purchase.StakeHolderId),
                StakeHolderId = purchase.StakeHolderId,
                TotalAmount = purchase.TotalAmount,
                TaxableAmount = purchase.TaxableAmount,
                TaxAmount = purchase.TaxAmount,
                TransactionDate = purchase.TransactionDate,

                StockMovements = purchaseItems.Select(sm => new StockMovementReturnVm
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
                    var purchaseData = await _purchaseRepository.FindById(viewModel.Id);
                    var purchaseItems = await _stockMovementRepository.FindByIdAsync(viewModel.Id);
                    var vm = new PurchaseReturnVm
                    {
                    Id = purchaseData.Id,
                    InvoiceNumber = purchaseData.InvoiceNumber,
                    Description = purchaseData.Description,
                    DiscountAmount = purchaseData.DiscountAmount,
                    StakeHolderName = await _stakeholderRepository.GetStakeHolderName(purchaseData.StakeHolderId),
                    TotalAmount = purchaseData.TotalAmount,
                    TaxableAmount = purchaseData.TaxableAmount,
                    TaxAmount = purchaseData.TaxAmount,
                    TransactionDate = purchaseData.TransactionDate,

                    StockMovements = purchaseItems.Select(sm => new StockMovementReturnVm
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
                
                var purchaseDto = new PurchaseDto
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

            var purchase = await _purchaseService.CreateAsync(purchaseDto);
            var stockMovementsDto = viewModel.StockMovements.Select(sm => new StockMovementDto
            {

                ProductId = sm.ProductId,
                Quantity = sm.Quantity,
                MovementType = MovementType.PurchaseReturn,
                InvoiceNumber = viewModel.InvoiceNumber,
                Rate = sm.Rate,
                Date = viewModel.TransactionDate,
                VatPercentage = sm.VatPercentage,
                TypeId = purchase.Id,
                Stock = Stock.Out,
            }).ToList();
            await _stockMovementService.AddAsync(stockMovementsDto);
            return RedirectToAction("Index"); 
            }
            catch (Exception ex)
            {
                var purchase = await _purchaseRepository.FindById(viewModel.Id);
                var purchaseItems = await _stockMovementRepository.FindByIdAsync(viewModel.Id);
                var vm = new PurchaseReturnVm
            {
                Id = purchase.Id,
                InvoiceNumber = purchase.InvoiceNumber,
                Description = purchase.Description,
                DiscountAmount = purchase.DiscountAmount,
                StakeHolderName = await _stakeholderRepository.GetStakeHolderName(purchase.StakeHolderId),
                TotalAmount = purchase.TotalAmount,
                TaxableAmount = purchase.TaxableAmount,
                TaxAmount = purchase.TaxAmount,
                TransactionDate = purchase.TransactionDate,

                StockMovements = purchaseItems.Select(sm => new StockMovementReturnVm
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