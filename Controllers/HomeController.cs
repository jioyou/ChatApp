using ChatApp.Database;
using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ChatApp.Hubs;

namespace ChatApp.Controllers
{
    [Authorize(Roles = "Admin, Normal")]//如果权限验证失败会调用一个叫accessdenied的action
    public class HomeController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public HomeController(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }
        public IActionResult Index()
        {
            var chats = _appDbContext.Chats
                .Include(x => x.Users)
                .Where(x => x.Type == ChatType.Room && !x.Users
                    .Any(y => y.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value))
                .ToList();
            return View(chats);
        }
        public IActionResult Find()
        {
            var users = _appDbContext.Users
                .Where(x => x.Id != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                .ToList();
            return View(users);
        }
        //[HttpPost]这玩意TM的是get
        public async Task<IActionResult> CreatePrivateRoom(string userId)
        {
            var user = _appDbContext.Users.FirstOrDefault(x => x.Id == userId);
            var chat = new Chat
            {
                Name = "由" + user.UserName + "发起的私聊",
                Type = ChatType.Private
            };
            chat.Users.Add(new ChatUser
            {
                UserId = userId,
                Role = UserRole.Member,
                
            });
            chat.Users.Add(new ChatUser
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Role = UserRole.Admin
                
            });
             _appDbContext.Chats.Add(chat);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Chat), new { Id = chat.Id});
        }
        public IActionResult Chat(int Id)
        {
            var chat = _appDbContext.Chats.Include(x => x.Messages).FirstOrDefault(x => x.Id == Id);
            return View(chat);
        }
        /// <summary>
        /// 可以注释掉
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="mess"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateMessage(int chatId, string mess)
        {
            Console.WriteLine("form调用onsubmit后没走这块了");
            var message = new Message {
                ChatId = chatId,
                Name = User.Identity.Name,
                Text = mess,
                Timetamp = DateTime.Now
            };
            if (ModelState.IsValid)
            {
                _appDbContext.Messages.Add(message);
                await _appDbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Chat), new { Id = chatId });
        }
        [HttpPost]
        public async Task<IActionResult> CreateRoom(string name)
        {
            var chat = new Chat
            {
                Name = name,
                Type = ChatType.Room
            };
            chat.Users.Add(new ChatUser
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Role = UserRole.Admin
            });
             _appDbContext.Chats.Add(chat);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> JoinChat(int Id)
        {
            var chatUser = new ChatUser
            {
                ChatId = Id,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Role = UserRole.Member
            };
            _appDbContext.ChatUsers.Add(chatUser);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Chat),new { Id = Id});
        }
    }
}
