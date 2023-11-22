using Bloggie.Web.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ILogger<AccountController> logger;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager
            , ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                var identityUser = new IdentityUser()
                {
                    UserName = registerViewModel.Username,
                    Email = registerViewModel.Email
                };
                var identityResult = await userManager.CreateAsync(identityUser, registerViewModel.Password);

                if (identityResult.Succeeded)
                {
                    // Assign this user the "User" Role
                    var roleIdentityResult = await userManager.AddToRoleAsync(identityUser, "User");

                    if (roleIdentityResult.Succeeded)
                    {
                        //Show success notification
                        return RedirectToAction("Register");
                    }
                }
            }

            //Show error notification
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }
            
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["error2"] = "Username or Password is incorrect!";
                return View();
            }

            try 
            {
                var signInResult = await signInManager.PasswordSignInAsync(loginViewModel.Username, loginViewModel.Password, false, false);

                if (signInResult != null && signInResult.Succeeded)
                {
                    TempData["success"] = "Login successfully!";
                    if (!string.IsNullOrWhiteSpace(loginViewModel.ReturnUrl))
                    {
                        return Redirect(loginViewModel.ReturnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            catch(Exception ex) 
            {
                var errorId = Guid.NewGuid();
                logger.LogError(ex, $"{errorId} : {ex.Message}");
            }
           

            //Show errors;
            TempData["error"] = "Login failed!";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
