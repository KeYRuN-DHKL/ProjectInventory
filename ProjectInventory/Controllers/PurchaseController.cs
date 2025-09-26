using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using ProjectInventory.Dto;
using ProjectInventory.Enum;
using ProjectInventory.Models;
using ProjectInventory.Repository.Interface;
using ProjectInventory.Service.Interface;

namespace ProjectInventory.Controllers;

public class PurchaseController(
    IProductRepository productRepository,
    IStakeHolderRepository stakeholderRepository,
    IPurchaseService purchaseService,
    IStockMovementService stockMovementService,
    IPurchaseRepository purchaseRepository,
    IToastNotification toastNotification,
    IStockMovementRepository stockMovementRepository)
    : Controller
{
    public IActionResult Index()
    {
        var items = purchaseRepository.GetQueryAbleData();
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
                Products = await productRepository.GetAllSelectListAsync(),
                StakeHolders = await stakeholderRepository.GetAllSelectListAsync(),
                TransactionDate = DateOnly.FromDateTime(DateTime.Now)
            };
            var products = await productRepository.GetAllAsync();
            vm.ProductUnitMap = products.ToDictionary(
                p => p.Id.ToString(),
                p => new ProductUnitVm{Symbol = p.Unit.Symbol,CostPrice=p.CostPrice}
            );
            return View(vm);
        }
        catch (Exception ex)
        {
            toastNotification.AddErrorToastMessage("An error occured...");
            return View();
        }
    }
   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PurchaseVm vm)
    {
        if (!ModelState.IsValid)
        {
            toastNotification.AddErrorToastMessage("An model state not valid...");
            vm.Products = await productRepository.GetAllSelectListAsync();
            vm.StakeHolders = await stakeholderRepository.GetAllSelectListAsync();
            vm.TransactionDate = DateOnly.FromDateTime(DateTime.Now);
            var products = await productRepository.GetAllAsync();
            vm.ProductUnitMap = products.ToDictionary(
                p => p.Id.ToString(),
                p => new ProductUnitVm{Symbol = p.Unit.Symbol,CostPrice = p.CostPrice}
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

            var purchase = await purchaseService.CreateAsync(purchaseDto);
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
            await stockMovementService.AddAsync(stockMovementsDto);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            // ModelState.AddModelError("", $"An unexpected error occured {ex.Message}");
            toastNotification.AddErrorToastMessage("An error occured...");
            vm.Products= await productRepository.GetAllSelectListAsync();
            vm.StakeHolders = await stakeholderRepository.GetAllSelectListAsync();
            vm.TransactionDate = DateOnly.FromDateTime(DateTime.Now);
            var products = await productRepository.GetAllAsync();
            vm.ProductUnitMap = products.ToDictionary(
            p=> p.Id.ToString(),
            p=> new ProductUnitVm{Symbol = p.Unit.Symbol,CostPrice = p.CostPrice}
        ); 
            return View(vm);
        }
    }
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var purchase = await purchaseRepository.FindById(id);
            var purchaseItems = await stockMovementRepository.FindByIdAsync(id);
            var stakeHolders = await stakeholderRepository.GetAllSelectListAsync();
            var products = await productRepository.GetAllSelectListAsync();
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
            var allProducts = await productRepository.GetAllAsync();
            vm.ProductUnitMap = allProducts.ToDictionary(
                p => p.Id.ToString(),
                p => new ProductUnitVm{Symbol = p.Unit.Symbol,CostPrice = p.CostPrice}
            );
            return View(vm);
        }
        catch (Exception ex)
        {
            toastNotification.AddErrorToastMessage("An unexpected error occured..." + ex.Message);
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
                vm.Products = await productRepository.GetAllSelectListAsync();
                vm.StakeHolders = await stakeholderRepository.GetAllSelectListAsync();
                vm.TransactionDate = DateOnly.FromDateTime(DateTime.Now);
                var products = await productRepository.GetAllAsync();
                vm.ProductUnitMap = products.ToDictionary(
                    p => p.Id.ToString(),
                    p => new ProductUnitVm{Symbol = p.Unit.Symbol,CostPrice = p.CostPrice}
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

            var updatedPurchase = await purchaseService.UpdateAsync(purchaseDto);
            var existingStockMovement = await stockMovementRepository.FindByIdAsync(vm.Id);

            foreach (var item in existingStockMovement)
            {
                if (vm.StockMovements.All(x => x.Id != item.Id))
                {
                    await stockMovementService.DeleteAsync(item.Id);
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
                    await stockMovementService.UpdateAsync(stockMovementDto);
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
                    await stockMovementService.AddAsync(stockMovementDto);
                }
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            toastNotification.AddErrorToastMessage(ex.Message);
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Return(Guid id)
    {
        try
        {
            var purchase = await purchaseRepository.FindById(id);
            var purchaseItems = await stockMovementRepository.FindByIdAsync(id);
            var vm = new PurchaseReturnVm
            {
                Id = purchase.Id,
                InvoiceNumber = purchase.InvoiceNumber,
                Description = purchase.Description,
                DiscountAmount = purchase.DiscountAmount,
                StakeHolderName = await stakeholderRepository.GetStakeHolderName(purchase.StakeHolderId),
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
            var allProducts = await productRepository.GetAllAsync();
            vm.ProductUnitMap = allProducts.ToDictionary(
                p => p.Id.ToString(),
                p => new ProductUnitVm{Symbol = p.Unit.Symbol,CostPrice = p.CostPrice}
            );
            return View(vm);
        }
        catch (Exception ex)
        {
            toastNotification.AddErrorToastMessage("An unexpected error occured..." + ex.Message);
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
                    var purchaseData = await purchaseRepository.FindById(viewModel.Id);
                    var purchaseItems = await stockMovementRepository.FindByIdAsync(viewModel.Id);
                    var vm = new PurchaseReturnVm
                    {
                    Id = purchaseData.Id,
                    InvoiceNumber = purchaseData.InvoiceNumber,
                    Description = purchaseData.Description,
                    DiscountAmount = purchaseData.DiscountAmount,
                    StakeHolderName = await stakeholderRepository.GetStakeHolderName(purchaseData.StakeHolderId),
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
                    var allProducts = await productRepository.GetAllAsync();
                    vm.ProductUnitMap = allProducts.ToDictionary(
                        p => p.Id.ToString(),
                        p => new ProductUnitVm{Symbol = p.Unit.Symbol,CostPrice = p.CostPrice}
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

            var purchase = await purchaseService.CreateAsync(purchaseDto);
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
            await stockMovementService.AddAsync(stockMovementsDto);
            return RedirectToAction("Index"); 
            }
            catch (Exception ex)
            {
                var purchase = await purchaseRepository.FindById(viewModel.Id);
                var purchaseItems = await stockMovementRepository.FindByIdAsync(viewModel.Id);
                var vm = new PurchaseReturnVm
            {
                Id = purchase.Id,
                InvoiceNumber = purchase.InvoiceNumber,
                Description = purchase.Description,
                DiscountAmount = purchase.DiscountAmount,
                StakeHolderName = await stakeholderRepository.GetStakeHolderName(purchase.StakeHolderId),
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
            var allProducts = await productRepository.GetAllAsync();
            vm.ProductUnitMap = allProducts.ToDictionary(
                p => p.Id.ToString(),
                p => new ProductUnitVm{Symbol = p.Unit.Symbol,CostPrice = p.CostPrice}
            );
            return View(vm);
            }
    }
}