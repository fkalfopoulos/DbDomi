using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DbDomi.Models;
using Microsoft.AspNetCore.Identity;
using DbDomi.Data;
using Microsoft.AspNetCore.Authorization;
using PagedList;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace DbDomi.Controllers
{
    public class HomeController : Controller
    { 

    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> signInManager;
        private readonly ApplicationDbContext _context;



    public HomeController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        this.signInManager = signInManager;
        _context = context;
    }


      [Authorize]
        public  async Task<IActionResult> Index()
        {
            var role = await _userManager.GetUsersInRoleAsync("Admin");

            var query = _context.Users
                                 .Where(x => x.Email != "administrator@gmail.com")
                                 .Select(y => new ClientViewModel
                                 {
                                     FirstName = y.FirstName,
                                     LastName = y.LastName,
                                     Email = y.Email,
                                     UserAvatar = y.UserAvatar,
                                     PhoneNumber = y.PhoneNumber,
                                     Comment = y.Comment
                                 })
                                 .ToList();
                                

            return View(query);

        }
      
    


            public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
