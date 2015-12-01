using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Framework.Logging;
using Newtonsoft.Json;

namespace SyncTube.Hubs
{
    [Authorize]
    public class StreamHub : Hub
    {
        private const string LobbyRoomName = "LOBBY_ROOM_FUN_TIMES!!";
        private static readonly Dictionary<string, VideoStatus> RoomStatuses = new Dictionary<string, VideoStatus>();
        private static Thread _roomLoopThread;
        private static readonly List<string> Lobbyers = new List<string>();
        private static Thread _loobyLoopThread;

        private readonly ILogger _logger;

        public StreamHub(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("StreamHub");
        }

        //Thread methods
        private void LobbyLoop()
        {
            _logger.LogInformation("Starting lobby thread");

            var statuses = new List<VideoStatusLightweight>(RoomStatuses.Count);
            do
            {
                BroadcastLobbyStatus(statuses);
                Thread.Sleep(2000);
            } while (Lobbyers.Count > 0);

            _logger.LogInformation("Stopping lobby thread");

            _loobyLoopThread = null;
        }

        private void LoopRooms()
        {
            _logger.LogInformation("Starting rooms thread");

            int userCount;
            do
            {
                userCount = 0;
                var keyCollection = RoomStatuses.Keys;
                foreach (var roomId in keyCollection)
                {
                    var videoStatus = RoomStatuses[roomId];
                    if (videoStatus.UserList.Count < 1)
                    {
                        videoStatus.Status = Status.Unstarted;
                        videoStatus.Time = 0;
                        continue;
                    }
                    if (videoStatus.Status == Status.Playing)
                    {
                        videoStatus.Time += 2;
                    }
                    userCount += videoStatus.UserList.Count;
                    SendAll(roomId);
                }

                Thread.Sleep(2000);
            } while (userCount > 0);

            _logger.LogInformation("Stopping rooms thread");

            _roomLoopThread = null;
        }

        // Room management
        public override Task OnConnected()
        {
            _logger.LogVerbose(Context.User.Identity.Name + " has connected to StreamHub");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var keyCollection = RoomStatuses.Keys;
            var connectionId = Context.ConnectionId;
            string userStatus = "Current user count:\n";
            foreach (var roomId in keyCollection)
            {
                if (RoomStatuses[roomId].UserList.Remove(connectionId))
                    _logger.LogInformation(Context.User.Identity.Name + " is leaving stream room [" + roomId + ']');
               userStatus += RoomStatuses[roomId].UserList.Count + " on room [" + roomId + "]\n";
            }
            _logger.LogInformation(userStatus);
            Lobbyers.Remove(connectionId);
            return base.OnDisconnected(stopCalled);
        }

        public void JoinLobby()
        {
            Lobbyers.Add(Context.ConnectionId);
            Groups.Add(Context.ConnectionId, LobbyRoomName);
            _logger.LogInformation(Context.User.Identity.Name + " has joined the lobby; " + Lobbyers.Count + " users in the lobby");

            if (_loobyLoopThread == null)
            {
                _loobyLoopThread = new Thread(LobbyLoop);
                _loobyLoopThread.Start();
            }
        }

        public void CreateOrJoinRoom(string roomId)
        {
            if (!RoomStatuses.ContainsKey(roomId))
            {
                RoomStatuses[roomId] = new VideoStatus();
            }

            if (_roomLoopThread == null)
            {
                _roomLoopThread = new Thread(LoopRooms);
                _roomLoopThread.Start();
            }

            RoomStatuses[roomId].UserList.Add(Context.ConnectionId);
            _logger.LogInformation(Context.User.Identity.Name + " has joined stream room [" + roomId + ']');
            _logger.LogInformation(RoomStatuses[roomId].UserList.Count + " users connected to room [" + roomId + ']');
            Groups.Add(Context.ConnectionId, roomId);
        }

// Broadcasting methods
        private VideoStatus SendAll(string roomId, string connectionId = null)
        {
            string command = null;

            var roomStatus = RoomStatuses[roomId];
            switch (roomStatus.Status)
            {
                case Status.Playing:
                    command = "play";
                    break;
                case Status.Paused:
                    command = "pause";
                    break;
                case Status.Ended:
                    command = "end";
                    break;
                case Status.Unstarted:
                    command = "unstarted";
                    break;
            }

            if (command != null)
            {
                if (connectionId == null) Clients.Group(roomId).broadcastStatus(command, roomStatus.Time);
                else Clients.User(connectionId).broadcastStatus(command, roomStatus.Time);
            }
            return roomStatus;
        }

        private void BroadcastLobbyStatus(List<VideoStatusLightweight> statuses)
        {
            statuses.Clear();
            var keyCollection = RoomStatuses.Keys;
            foreach (var roomId in keyCollection)
            {
                var videoStatus = RoomStatuses[roomId];
                statuses.Add(new VideoStatusLightweight(roomId, videoStatus));
            }
            var jsonRooms = JsonConvert.SerializeObject(statuses);
            Clients.Group(LobbyRoomName).broadcastLobbyStatus(jsonRooms);
        }

//View invocatable methods
        public VideoStatus RequestStatus(string roomId)
        {
            return SendAll(roomId, Context.ConnectionId);
        }

        public void RequestLobbyStatus()
        {
            BroadcastLobbyStatus(new List<VideoStatusLightweight>(RoomStatuses.Count));
        }

        public void SetPaused(string roomId, int time)
        {
            var roomStatuse = RoomStatuses[roomId];
            roomStatuse.Time = time;
            roomStatuse.Status = Status.Paused;
        }

        public void SetPlaying(string roomId, int time)
        {
            var roomStatuse = RoomStatuses[roomId];
            roomStatuse.Time = time;
            roomStatuse.Status = Status.Playing;
        }

        public void SetEnded(string roomId)
        {
            var roomStatuse = RoomStatuses[roomId];
            roomStatuse.Status = Status.Ended;
            roomStatuse.Time = 0;
        }
    }
}