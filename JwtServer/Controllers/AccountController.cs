using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JwtServer.Models;
using JwtServer.Services;

namespace JwtServer.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController:ControllerBase
    {
        private readonly SignInManager<User> _signManager;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public AccountController(UserManager<User> userManager, 
            SignInManager<User> signManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signManager = signManager;
            _tokenService = tokenService;
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return NoContent();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(Register register)
        {
            if (ModelState.IsValid)
            {
                var user = new User {
                    UserName = register.Username,
                    Email = register.Email
                };
                var result = await _userManager.CreateAsync(user, register.Password);

                if (result.Succeeded)
                {
                    await _signManager.SignInAsync(user, false);
                    string token = _tokenService.GenerateToken(user);
                    return CreatedAtAction("register",token);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        //ModelState.AddModelError("", error.Description);
                        Console.WriteLine("创建用户时发生错误"+ error.Description);
                    }
                }
            }
            return NoContent();
        }
        [HttpGet("login")]
        public IActionResult Login()
        {
            return NoContent();
        }
        [HttpPost("login")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                var result = await _signManager.
                    PasswordSignInAsync(login.Username,login.Password,false, false);
                if (result.Succeeded)
                {
                    //if (!string.IsNullOrEmpty(login.ReturnUrl) && Url.IsLocalUrl(login.ReturnUrl))
                    //{
                    //    return Redirect(login.ReturnUrl);
                    //}
                    //else
                    //{
                    //    return RedirectToAction("Index", "Home");
                    //}
                    //var encryptValue = _userService.LoginEncrypt(model.UserName, ApplicationKeys.User_Cookie_Encryption_Key);
                    //HttpContext.Response.Cookies.Append(ApplicationKeys.User_Cookie_Key, encryptValue);
                    var user = new User()
                    {
                        UserName = login.Username
                    };
                    string token = _tokenService.GenerateToken(user);
                    return CreatedAtAction("login", token);
                }
            }
            //ModelState.AddModelError("", "Invalid login attempt");
            //return View(login);
            return CreatedAtAction("login", "登录失败");
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signManager.SignOutAsync();
            return CreatedAtAction("logout", "注销成功");
        }
    }
}
