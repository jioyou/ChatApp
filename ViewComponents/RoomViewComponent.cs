using ChatApp.Database;
using ChatApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace ChatApp.ViewComponents
{
    public class RoomViewComponent : ViewComponent
    {
        private readonly AppDbContext _appDbContext;

        public RoomViewComponent(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }
        public IViewComponentResult Invoke(string chattype)
        {
            int type = Convert.ToInt32(chattype);
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;//获取当前登录用户Id
            //var user = await _userManager.FindByIdAsync(userId);//获取当前用户
            var chats = _appDbContext.ChatUsers
                .Where(x => x.UserId == userId && x.Chat.Type == (ChatType)type)
                .Select(x => x.Chat)
                .ToList();
            return View(chats);
        }
    }
}
