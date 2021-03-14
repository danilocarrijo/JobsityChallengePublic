using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ChatRoomChallenge.Models;
using ChatRoomChallenge.Data;
using System.Data.Entity;
using Microsoft.AspNetCore.SignalR;
using ChatTask.Hubs;
using ChatRoomChallenge.Hubs.ChatCenter;
using ServicesInterface;
using Entities;

namespace ChatRoomChallenge.Controllers
{
    public class HomeController : Controller
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMessageService _messageService;
        private readonly IChatCenterController _chatCenterController;

        public HomeController(
                   UserManager<AppUser> userManager,
                   SignInManager<AppUser> signInManager,
                   IChatCenterController chatCenterController,
                   IMessageService messageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _messageService = messageService;
            _chatCenterController = chatCenterController;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user =
                await _userManager.GetUserAsync(HttpContext.User).ConfigureAwait(false);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var messages = _messageService.GetAll();



            ViewBag.MessageHIstory = messages.OrderBy(x => x.MessageDateMessage)
                                            .Skip(Math.Max(0, messages.Count() - 50))
                                            .Take(50);


            return View(user);
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

        [Authorize]
        public async Task<IActionResult> UserInfo()
        {
            var user =
                await _userManager.GetUserAsync(HttpContext.User).ConfigureAwait(false);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AppUser appUser)
        { 
            var user = await _userManager.FindByNameAsync(appUser.UserName);

            if (user != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync
                                   (user, appUser.Password, false, false);

                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Register");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public List<string>  GetUsers()
        {
            var res = _chatCenterController.GetUsers();
            return res;
        }

        [HttpPost]
        public async Task<IActionResult> Register(AppUser appUser)
        { 

            var user = new AppUser
            {
                UserName = appUser.UserName,
                Email = appUser.Email,
                Name = appUser.Name,
                Password = appUser.Password
            };

            var result = await _userManager.CreateAsync(user, user.Password);


            if (result.Succeeded)
            { 
                var signInResult = await _signInManager.PasswordSignInAsync
                                   (user, user.Password, false, false);

                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }

            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}
