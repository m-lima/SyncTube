using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using SyncTube.Hubs;

namespace SignalRStream
{
    [Authorize]
    public class StreamHub : Hub
    {
        private const string LobbyRoomName = "LOBBY_ROOM_FUN_TIMES!!";
        private static readonly Dictionary<string, VideoStatus> RoomStatuses = new Dictionary<string, VideoStatus>();
        private static Thread _roomLoopThread;
        private static readonly List<string> Lobbiers = new List<string>();
        private static Thread _loobyLoopThread;


//Thread methods
        private void LobbyLoop()
        {
            Console.WriteLine("Starting lobby thread");

            var statuses = new List<VideoStatusLightweight>(RoomStatuses.Count);
            do
            {
                BroadcastLobbyStatus(statuses);
                Thread.Sleep(2000);
            } while (Lobbiers.Count > 0);

            Console.WriteLine("Stopping lobby thread");

            _loobyLoopThread = null;
        }

        private void LoopRooms()
        {
            Console.WriteLine("Starting rooms thread");

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

            Console.WriteLine("Stopping rooms thread");

            _roomLoopThread = null;
        }

        // Room management
        public override Task OnDisconnected(bool stopCalled)
        {
            var keyCollection = RoomStatuses.Keys;
            var connectionId = Context.ConnectionId;
            foreach (var roomId in keyCollection)
            {
                if (RoomStatuses[roomId].UserList.Remove(connectionId))
                    Console.WriteLine("User leaving room " + roomId);
                Console.WriteLine(RoomStatuses[roomId].UserList.Count + " users connected to room " + roomId);
            }
            Lobbiers.Remove(connectionId);
            return base.OnDisconnected(stopCalled);
        }

        public void JoinLobby()
        {
            Lobbiers.Add(Context.ConnectionId);
            Groups.Add(Context.ConnectionId, LobbyRoomName);

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
            Console.WriteLine(RoomStatuses[roomId].UserList.Count + " users connected to room " + roomId);
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