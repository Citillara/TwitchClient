using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.Models
{
    public class TwitchExtra
    {
        public Color UserColor = SystemColors.WindowText;
        public string DisplayName;
        public string Login;
        public bool IsSubscriber = false;
        public long SubscriberLevel = -1;
        public bool IsTurbo = false;
        public bool IsPrime = false;
        public long UserId = -1;
        public long RoomId = -1;
        public long BitsLevel = -1;
        public long BitsSent = 0;
        public bool IsVerified = false;
        public long PartnerLevel = -1;
        public TwitchUserTypes UserType = TwitchUserTypes.None;
        public string SystemMessage;

        /// <summary>
        /// Indicates if message is a subscription event
        /// </summary>
        public bool IsSub = false;
        public bool IsResub = false;
        public long SubMonths = -1;
        public string SubPlanName;
        public long SubPlan = -1;

        public TwitchExtra(Dictionary<string, string> tags, bool isBroadcaster)
        {
            ParseTags(tags);

            if (isBroadcaster)
            {
                UserType |= TwitchUserTypes.Broadcaster;
            }
        }

        private void ParseTags(Dictionary<string, string> tags)
        {
            if (tags == null)
                return;

            //if (tags.ContainsKey("emotes"))

            ParseMainInfo(tags);
            ParseTwitchOtherSubsAndBits(tags);
            ParseTwitchSubs(tags);
            ParseMisc(tags);
            ParseBagdes(tags);
        }

        private void ParseMainInfo(Dictionary<string, string> tags)
        {
            if (tags.ContainsKey("display-name"))
            {
                DisplayName = tags["display-name"];
            }
            if (tags.ContainsKey("login"))
            {
                DisplayName = tags["login"];
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
        }
        private void ParseTwitchSubs(Dictionary<string, string> tags)
        {
            if (tags.ContainsKey("msg"))
            {
                if (tags["msg"] == "sub")
                {
                    IsSub = true;
                }
                else if (tags["msg"] == "resub")
                {
                    IsResub = true;
                }
                if (IsSub || IsResub)
                {
                    if (tags.ContainsKey("msg-param-month"))
                    {
                        SubMonths = long.Parse(tags["msg-param-month"]);
                    }
                    if (tags.ContainsKey("msg-param-sub-plan-name"))
                    {
                        SubPlanName = tags["msg-param-sub-plan-name"];
                    }
                    if (tags.ContainsKey("msg-param-sub-plan"))
                    {
                        SubPlan = long.Parse(tags["msg-param-sub-plan"]);
                    }
                }
            }
        }
        private void ParseTwitchOtherSubsAndBits(Dictionary<string, string> tags)
        {
            if (tags.ContainsKey("subscriber"))
            {
                if (tags["subscriber"] == "1")
                {
                    IsSubscriber = true;
                    UserType |= TwitchUserTypes.Subscriber;
                }
            }
            if (tags.ContainsKey("turbo"))
            {
                if (tags["turbo"] == "1")
                    IsTurbo = true;
            }
            if (tags.ContainsKey("bits"))
            {
                if (!string.IsNullOrEmpty(tags["bits"]))
                {
                    long.TryParse(tags["bits"], out BitsSent);
                }
            }
        }
        private void ParseMisc(Dictionary<string, string> tags)
        {
            if (tags.ContainsKey("color"))
            {
                if (!string.IsNullOrWhiteSpace(tags["color"]))
                {
                    int RGB = int.Parse(tags["color"].Replace("#", ""), NumberStyles.HexNumber);
                    UserColor = Color.FromArgb(RGB);
                }
            }
            if (tags.ContainsKey("system-msg"))
            {
                SystemMessage = tags["system-msg"];
            }
        }

        private void ParseBagdes(Dictionary<string, string> tags)
        {

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
                            case "partner":
                                long.TryParse(s[1], out PartnerLevel);
                                if (PartnerLevel > 1)
                                    IsVerified = true;
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
