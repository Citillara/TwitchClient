using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.Models
{
    public class TwitchRoomState
    {
        //[Debug] [04/01/2021 21:04:15] @emote-only=0;followers-only=30;r9k=1;rituals=0;room-id=135468063;slow=120;subs-only=0 
        //:tmi.twitch.tv ROOMSTATE #antoinedaniellive

        public string Channel;
        public bool IsEmoteOnly = false;
        public bool IsFollowersOnly = false;
        public long FollowersModeMonthAge = -1;
        public bool IsR9KActivated = false;
        public long RoomId = -1;
        public bool IsRituals = false;
        public bool IsSlowModeActive = false;
        public long SlowModeSeconds = -1;
        public bool IsSubOnly = false;

        public TwitchRoomState(Irc.IrcMessage args)
        {
            Channel = args.Parameters[0];
            ParseMainInfo(args.Tags);
        }

        private void ParseMainInfo(Dictionary<string, string> tags)
        {
            if (tags.ContainsKey("emote-only"))
            {
                IsEmoteOnly = tags["emote-only"] == "1";
            }
            if (tags.ContainsKey("room-id"))
            {
                RoomId = long.Parse(tags["room-id"]);
            }
            if (tags.ContainsKey("followers-only"))
            {
                FollowersModeMonthAge = long.Parse(tags["followers-only"]);
                if (FollowersModeMonthAge > 0)
                    IsFollowersOnly = true;
            }
            if (tags.ContainsKey("r9k"))
            {
                IsEmoteOnly = tags["r9k"] == "1";
            }
            if (tags.ContainsKey("rituals"))
            {
                IsEmoteOnly = tags["rituals"] == "1";
            }
            if (tags.ContainsKey("slow"))
            {
                SlowModeSeconds = long.Parse(tags["slow"]);
                if (SlowModeSeconds > 0)
                    IsSlowModeActive = true;
            }
            if (tags.ContainsKey("subs-only"))
            {
                IsEmoteOnly = tags["subs-only"] == "1";
            }
        }


        public string ToShortString()
        {

            StringBuilder sb = new StringBuilder();

            if (IsEmoteOnly)
                sb.AppendFormat("[Emote] ");
            if (IsFollowersOnly)
                sb.AppendFormat("[Follow {0}] ", FollowersModeMonthAge);
            if (IsR9KActivated)
                sb.AppendFormat("[R9K] ");
            if (IsRituals)
                sb.AppendFormat("[Ri] ");
            if (IsSlowModeActive)
                sb.AppendFormat("[Slow {0}] ", SlowModeSeconds);
            if (IsSubOnly)
                sb.AppendFormat("[Sub] ");

            return sb.ToString();
        }
    }
}
