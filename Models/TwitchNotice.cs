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
    }
}
