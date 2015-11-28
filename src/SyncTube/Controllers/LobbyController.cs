using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using SyncTube.Models;
using SyncTube.Services;
using SyncTube.ViewModels.Room;
using System.Linq;
using System.Security.Claims;

namespace SyncTube.Controllers
{
    [Authorize]
    public class LobbyController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IEmailSender _emailSender;

        public LobbyController(ApplicationDbContext applicationDbContext, IEmailSender emailSender)
        {
            _applicationDbContext = applicationDbContext;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View(_applicationDbContext.Rooms);
        }

        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateRoomViewModel model)
        {
            var user = _applicationDbContext.Users.First(u => u.Id.Equals(User.GetUserId()));
            var room = new Room { VideoId = model.VideoId, Name = model.Name, Description = model.Description, Duration = model.Duration, CreatedBy = user };
            _applicationDbContext.Rooms.Add(room);
            _applicationDbContext.SaveChanges();
            _emailSender.SendEmailAsync(user.Email, "New room", "Congratulations! You have created a new room for syncronizing videoes. What a time to be alive!");
            return RedirectToAction("Show", new { id = room.Id });
        }

        public IActionResult Show(long id)
        {
            return View(_applicationDbContext.Rooms.FirstOrDefault(x => x.Id == id));
        }
    }
}
