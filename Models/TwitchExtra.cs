using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.Models
{
    class TwitchExtra
    {
        public Color UserColor = SystemColors.WindowText;
        public string DisplayName;
        public bool IsSubscriber = false;
        public long SubscriberLevel = -1;
        public bool IsTurbo = false;
        public bool IsPrime = false;
        public long UserId = -1;
        public long RoomId = -1;
        public long BitsLevel = -1;
        public long BitsSent = 0;
        public TwitchUserTypes UserType = TwitchUserTypes.None;

        

        public TwitchExtra(Dictionary<string, string> tags)
        {
            if (tags == null)
                return;
            if (tags.ContainsKey("color"))
            {
                if (!string.IsNullOrWhiteSpace(tags["color"]))
                {
                    int RGB = int.Parse(tags["color"].Replace("#", ""), NumberStyles.HexNumber);
                    UserColor = Color.FromArgb(RGB);
                }
            }
            if (tags.ContainsKey("display-name"))
            {
                DisplayName = tags["display-name"];
            }
            //if (tags.ContainsKey("emotes"))
            if (tags.ContainsKey("subscriber"))
            {
                if (tags["subscriber"] == "1")
                    IsSubscriber = true;
            }
            if (tags.ContainsKey("turbo"))
            {
                if (tags["turbo"] == "1")
                    IsTurbo = true;
            }
            if (tags.ContainsKey("user-id"))
            {
                UserId = long.Parse(tags["user-id"]);
            }
            if (tags.ContainsKey("room-id"))
            {
                RoomId = long.Parse(tags["room-id"]);
            }
            if (tags.ContainsKey("user-type"))
            {
                UserType = ParseType(tags["user-type"]);
            }
            if (tags.ContainsKey("bits"))
            {
                if (!string.IsNullOrEmpty(tags["bits"]))
                {
                    long.TryParse(tags["bits"], out BitsSent);
                }
            }

            // Parsing badges
            if (tags.ContainsKey("badges"))
            {
                var split = tags["badges"].Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var spl in split)
                {
                    var s = spl.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    if (s.Length == 2)
                    {
                        switch (s[0])
                        {
                            case "subscriber":
                                long.TryParse(s[1], out SubscriberLevel);
                                break;
                            case "bits":
                                long.TryParse(s[1], out BitsLevel);
                                break;
                            case "premium":
                                IsPrime = true;
                                break;
                            default: break;
                        }
                    }
                }
            }

        }

        private TwitchUserTypes ParseType(string type)
        {
            switch (type)
            {
                case "mod": return TwitchUserTypes.Mod;
                case "global_mod": return TwitchUserTypes.GlobalMod;
                case "admin" : return TwitchUserTypes.Admin;
                case "staff": return TwitchUserTypes.Staff;
                default: return TwitchUserTypes.None;
            }
        }


    }

    public static class Extensions
    {
        public static string[] Split(this string str, char c, StringSplitOptions op)
        {
            return str.Split(new char[] { c }, op);
        }
    }
}
