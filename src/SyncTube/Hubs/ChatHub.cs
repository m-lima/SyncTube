using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace SignalRChat
{
    [Authorize]
    public class ChatHub : Hub
    {
        private static readonly List<string> Lobbiers = new List<string>();

        public void Send(string roomId, string message)
        {
            var name = Context.User.Identity.Name;
            Clients.Group(roomId).sendMessage(name, message);
            Console.WriteLine("Message has been sent from " + name);
        }

        public void JoinRoom(string roomId)
        {
            Groups.Add(Context.ConnectionId, roomId);
            Console.WriteLine(Context.User.Identity.Name + " has joined the chat.");
            Clients.Group(roomId).addChatMessage(Context.User.Identity.Name + " has joined the chat.");
            Lobbiers.Add(Context.ConnectionId);
        }

        public override Task OnConnected()
        {
            Console.WriteLine("User has connected. ");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Lobbiers.Remove(Context.ConnectionId);
            Console.WriteLine("User has disconnected. ");
            return base.OnDisconnected(stopCalled);
        }
    }
}