using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Framework.Logging;
using SyncTube.Logging;

namespace SyncTube.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private static readonly List<string> Lobbiers = new List<string>();
        private readonly ILogger _logger;

        public ChatHub(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("ChatHub");
        }

        public void Send(string roomId, string message)
        {
            var name = Context.User.Identity.Name;
            Clients.Group(roomId).sendMessage(name, message);
            _logger.LogVerbose("Message has been sent from [" + name + "] in room [" + roomId + "]\n" + message);
        }

        public void JoinRoom(string roomId)
        {
            Groups.Add(Context.ConnectionId, roomId);
            _logger.LogInformation(Context.User.Identity.Name + " has joined the chat in room [" + roomId + ']');
            Clients.Group(roomId).addChatMessage(Context.User.Identity.Name + " has joined the chat.");
            Lobbiers.Add(Context.ConnectionId);
        }

        public override Task OnConnected()
        {
            _logger.LogVerbose(Context.User.Identity.Name + " has connected to ChatHub");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Lobbiers.Remove(Context.ConnectionId);
            _logger.LogInformation(Context.User.Identity.Name + " has disconnected from ChatHub");
            return base.OnDisconnected(stopCalled);
        }
    }
}