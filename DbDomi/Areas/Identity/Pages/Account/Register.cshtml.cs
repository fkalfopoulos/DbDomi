using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DbDomi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace DbDomi.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IHostingEnvironment _env;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IHostingEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _env = env;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [PersonalData]
            [Display(Name = "Firstname")]
            public string Firstname { get; set; }

            [Required]
           [PersonalData ]
            [Display(Name = "LastName")]
            public string Lastname { get; set; }

            [Required]
            [DataType(DataType.PhoneNumber)]
            [Display(Name = "Telephone Number")]
            [RegularExpression(@"^([0-9]{3})[- ]?([0-9]{3})[- ]?([0-9]{4})$", ErrorMessage = "Please enter a valid telephone")]
            public string Phone { get; set; }


            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }


            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [DataType(DataType.Upload)]
            [Display(Name = "User Avatar")]
            public IFormFile UserAvatar { get; set; }

            [PersonalData]
            [Display(Name = "Comments")]
            public string Comment { get; set; }


        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new User
                {

                    FirstName = Input.Firstname,
                    LastName = Input.Lastname,                    
                    UserName =  Input.Email.Split('@')[0],
                    Email = Input.Email,
                    PhoneNumber = Input.Phone, 
                    Comment= Input.Comment,
                    UserAvatar = "defUserAvatar.jpg"
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    await SaveUserAvatar(Input.UserAvatar, user);
                    _logger.LogInformation("User created a new account with password.");

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { userId = user.Id, code = code },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        public async Task SaveUserAvatar(IFormFile image, User newUser)
        {
            // full path to file in temp location
            if (image != null && image.Length > 0)
            {
                if (image.ContentType.Contains("png") || image.ContentType.Contains("jpg") || image.ContentType.Contains("jpeg"))
                {
                    string userID = newUser.Id;
                    var filePath = _env.WebRootPath + @"/images/userAvatars/" + userID + '.' + image.ContentType.Split('/')[1];

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    newUser.UserAvatar = userID + '.' + image.ContentType.Split('/')[1];
                    await _userManager.UpdateAsync(newUser);
                }
            }
        }
    }
}
