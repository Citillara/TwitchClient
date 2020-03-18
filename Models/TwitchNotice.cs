using Irc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.Models
{
    public class TwitchNotice : TwitchExtra
    {
        public string Channel;

        public TwitchNotice(IrcMessage message) : base (message.Tags, false)
        {
            Channel = message.Parameters[0];
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("[{0}] ", Channel);

            if (UserType.HasFlag(TwitchUserTypes.Citillara))
                sb.AppendFormat("[C] ");
            if (UserType.HasFlag(TwitchUserTypes.Developer))
                sb.AppendFormat("[Developper] ");
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
                sb.AppendFormat("[Sub {0}] ", SubscriberIconLevel);
            if (IsTurbo)
                sb.AppendFormat("[Turbo] ");
            if (IsPrime)
                sb.AppendFormat("[Prime] ");
            if (BitsLevel > 0)
                sb.AppendFormat("[Bit Lv {0}] ", BitsLevel);
            if (BitsSent > 0)
                sb.AppendFormat("[Bit Sent {0}] ", BitsSent);



            return sb.ToString();
        }
    }
}
