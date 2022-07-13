using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using ChatApp.Hubs;
using ChatApp.Database;
using System.Threading.Tasks;
using ChatApp.Models;
using System;

namespace ChatApp.Controllers
{
    [Authorize(Roles = "Admin, Normal")]//如果权限验证失败会调用一个叫accessdenied的action
    [Route("[controller]")]
    //[Route("api/[controller]")]//写webapi用到这俩
    //[ApiController]
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IHubContext<ChatHub> hubContext)
        {
            this._hubContext = hubContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("[action]/{connectionId}/{roomName}")]
        public async Task<IActionResult> JoinRoom(string connectionId, string roomName)
        {
            Console.WriteLine(connectionId + "====JoinRoom=====" + roomName);
            await _hubContext.Groups.AddToGroupAsync(connectionId, roomName);
            return Ok();
        }
        [HttpPost("[action]/{connectionId}/{roomName}")]
        public async Task<IActionResult> LeaveRoom(string connectionId, string roomName)
        {
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, roomName);
            return Ok();
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessage(string mess, string roomName, int chatId, [FromServices] AppDbContext appDbContext)
        {
            Console.WriteLine(mess + "====SendMessage=====" + roomName);
            var Message = new Message
            {
                ChatId = chatId,
                Name = User.Identity.Name,
                Text = mess,
                Timetamp = DateTime.Now
            };
            if (ModelState.IsValid)
            {
                appDbContext.Messages.Add(Message);
                await appDbContext.SaveChangesAsync();
            }
            await _hubContext.Clients.Group(roomName).SendAsync("RecieveMessage", Message);
            return Ok();
        }
    }
}
