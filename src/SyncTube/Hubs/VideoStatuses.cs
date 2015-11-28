using System.Collections.Generic;

namespace SyncTube.Hubs
{
    public enum Status
    {
        Ended,
        Playing,
        Paused,
        Unstarted
    }

    public class VideoStatus
    {
        public int Time { get; set; } = 0;
        public List<string> UserList { get; } = new List<string>();
        public Status Status { get; set; } = Status.Unstarted;
    }

    public class VideoStatusLightweight
    {
        public VideoStatusLightweight(string roomId, VideoStatus status)
        {
            RoomId = roomId;
            Time = status.Time;
            UserCount = status.UserList.Count;
            switch (status.Status)
            {
                case Status.Playing:
                    CurrentStatus = "play";
                    break;
                case Status.Paused:
                    CurrentStatus = "pause";
                    break;
                case Status.Ended:
                    CurrentStatus = "end";
                    break;
                default:
                    CurrentStatus = "unstarted";
                    break;
            }
        }

        public string RoomId { get; set; }
        public int Time { get; set; }
        public int UserCount { get; set; }
        public string CurrentStatus { get; set; }
    }
}