using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly UserManager<IdentityUser> userManager;

        public AdminUserController(IUserRepository userRepository, UserManager<IdentityUser> userManager)
        {
            this.userRepository = userRepository;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var users = await userRepository.GetAll();
            var usersViewModel = new UserViewModel();
            usersViewModel.Users = new List<User>();

            foreach (var user in users)
            {
                usersViewModel.Users.Add(new User
                {
                    Id = Guid.Parse(user.Id),
                    Username = user.UserName,
                    EmailAddress = user.Email
                });
            }

            return View(usersViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserViewModel request)
        {
            var identityUser = new IdentityUser
            {
                UserName = request.Username,
                Email = request.Email
            };

            var identityResult = await userManager.CreateAsync(identityUser, request.Password);

            if (identityResult != null)
            {
                if (identityResult.Succeeded)
                {
                    //assign roles to this user
                    var roles = new List<string>() { "User" };   
                    
                        if (request.AdminRoleCheckBox)
                        {
                            roles.Add("Admin");
                        }

                        identityResult = await userManager.AddToRolesAsync(identityUser, roles);

                        if(identityResult != null && identityResult.Succeeded)
                        {
                            return RedirectToAction("List");
                        }                   
                }
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());

            if(user != null)
            {
                var identityResult = await userManager.DeleteAsync(user);

                if (identityResult != null && identityResult.Succeeded)
                {
                    return RedirectToAction("List");
                }
            }

            return View();
        }
    }
}
