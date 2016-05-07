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
        public bool IsTurbo = false;
        public long UserId = -1;
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
            if (tags.ContainsKey("user-type"))
            {
                UserType = ParseType(tags["user-type"]);
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
}
