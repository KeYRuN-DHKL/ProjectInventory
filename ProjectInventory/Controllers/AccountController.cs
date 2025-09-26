using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectInventory.Entities;
using ProjectInventory.Models;

namespace ProjectInventory.Controllers;

[AllowAnonymous]
public class AccountController(SignInManager<ApplicationUser> signinManager, UserManager<ApplicationUser> userManager)
    : Controller
{
    private readonly UserManager<ApplicationUser> UserManager = userManager;
    private readonly SignInManager<ApplicationUser> SigninManager = signinManager;

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(UserRegisterVm vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var user = new ApplicationUser
        {
            Email = vm.Email,
            UserName = vm.Email,
            PhoneNumber = vm.PhoneNumber,
        };

        var result = await UserManager.CreateAsync(user, vm.Password);
        {
            if (result.Succeeded)
            {
                return Redirect("/");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(vm);
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserLoginVm vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var result =
            await SigninManager.PasswordSignInAsync(vm.Email, vm.Password, vm.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            return Redirect("/");
        }

        ModelState.AddModelError("", "Incorrect E-mail password combination");
        return View(vm);
    }
}