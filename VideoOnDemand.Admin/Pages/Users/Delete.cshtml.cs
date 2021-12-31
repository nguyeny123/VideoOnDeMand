using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VideoOnDemand.Admin.Models;
using VideoOnDemand.Admin.Services;

namespace VideoOnDemand.Admin.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private IUserService _userService;
        [BindProperty]
        public UserPageModel Input { get; set; } =
        new UserPageModel();
        // Used to send a message back to the Index Razor Page.
        [TempData]
        public string StatusMessage { get; set; }
        public DeleteModel (IUserService userService)
        {
            _userService = userService;
        }
        public void OnGet(string userId)
        {
            StatusMessage = string.Empty;
            Input = _userService.GetUser(userId);

        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.DeleteUser(Input.Id);
                if (result)
                {
                    // Message sent back to the Index Razor Page.
                    StatusMessage = $"User {Input.Email} was deleted.";
                    return RedirectToPage("Index");
                }
                
            }
            // Something failed, redisplay the form.
            return Page();
        }
    }
}