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
            sb.Append(base.ToString());


            return sb.ToString();
        }
    }
}
