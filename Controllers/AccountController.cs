using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ChatApp.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using NETCore.MailKit.Core;

namespace ChatApp.Controllers
{

    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailService emailService)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._emailService = emailService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null && user.EmailConfirmed)
            {
                //isPersistent(指示在关闭浏览器后登录 cookie 是否应保持不变的标志。)
                //lockoutOnFailure(指示登录失败时是否应锁定用户帐户的标志。)
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index), nameof(Home));
                }
            }
            return RedirectToAction(nameof(Login));
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string username, string phonenumber, string email, string password)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Register));
            }
            var user = new User
            {
                UserName = username,
                Email = email,
                PhoneNumber = phonenumber,
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var roleresult = await _userManager.AddToRoleAsync(user, "NORMAL");
                if (roleresult.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //Request.Scheme => "http" or "https"
                    var link = Url.Action(nameof(VerifyEmail), nameof(Account), new { userId = user.Id, code }, Request.Scheme, Request.Host.ToString());
                    _emailService.Send(email, "邮箱验证", $"<a href=\"{link}\">完成验证</a>", true);
                    return RedirectToAction(nameof(EmailVerification));
                }
            }
            return View(nameof(Register));
        }
        //如果是从邮箱的链接进来的把数据库的ConfirmEmail字段改成ture，然后显示前往登录链接
        public async Task<IActionResult> VerifyEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, code);
                if (result.Succeeded)
                {
                    return View();
                }
            }
            return BadRequest();//400
        }
        //注册成功后提示登录邮箱进行验证
        public IActionResult EmailVerification() => View();

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
