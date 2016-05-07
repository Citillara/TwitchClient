using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Twitch.Models
{
    public class TwitchMessage
    {
        static readonly Regex r = new Regex(@"[^\u0000-\u007F]", RegexOptions.Compiled);

        public string Channel;
        public string Message;
        public string SenderName;
        public string SenderDisplayName;
        public bool IsSubscriber = false;
        public bool IsTurbo = false;
        public long UserId = -1;

        public TwitchUserTypes UserType = TwitchUserTypes.None;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("[{0}]" , Channel);

            if (UserType.HasFlag(TwitchUserTypes.Citillara))
                sb.AppendFormat("[C] ");
            if (UserType.HasFlag(TwitchUserTypes.BotMaster))
                sb.AppendFormat("[BotMaster] ");
            if (UserType.HasFlag(TwitchUserTypes.Admin))
                sb.AppendFormat("[Admin] ");
            if (UserType.HasFlag(TwitchUserTypes.Broadcaster))
                sb.AppendFormat("[Broadcaster] ");
            if (UserType.HasFlag(TwitchUserTypes.GlobalMod))
                sb.AppendFormat("[GlobalMod] ");
            if (UserType.HasFlag(TwitchUserTypes.Staff))
                sb.AppendFormat("[Staff] ");
            if (UserType.HasFlag(TwitchUserTypes.Mod))
                sb.AppendFormat("[Mod] ");
            if (UserType.HasFlag(TwitchUserTypes.Subscriber))
                sb.AppendFormat("[Sub] ");
            if (IsTurbo)
                sb.AppendFormat("[Turbo] ");

            sb.AppendFormat("<{0}> " , SenderDisplayName);
            sb.Append(Message);
            return sb.ToString();
        }
    }
}
