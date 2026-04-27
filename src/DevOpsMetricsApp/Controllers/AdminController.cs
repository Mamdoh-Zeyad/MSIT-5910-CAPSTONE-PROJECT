using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevOpsMetricsApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(DevOpsMetricsApp.Models.EditUserViewModel model)
        {
            // If the data is missing or invalid, bounce them back to the index
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            // Update basic info
            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Update the role
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            // Refresh the dashboard
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // If they are locked out, unlock them. If active, lock them out for 100 years.
                if (await _userManager.IsLockedOutAsync(user))
                {
                    await _userManager.SetLockoutEndDateAsync(user, null);
                }
                else
                {
                    await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Prevent admin from deleting themselves
                if (user.Email != User.Identity.Name)
                {
                    await _userManager.DeleteAsync(user);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}