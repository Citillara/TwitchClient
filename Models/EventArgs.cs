using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.Models
{
    public class TwitchClientOnJoinEventArgs : EventArgs
    {
        public string Name;
        public string Channel;
        public bool IsMyself;
        public TwitchClientOnJoinEventArgs(string name, string channel, bool isMyself)
        {
            Name = name;
            Channel = channel;
            IsMyself = isMyself;
        }
    }

    public class TwitchClientOnPartEventArgs : EventArgs
    {
        public string Name;
        public string Channel;
        public bool IsMyself;
        public TwitchClientOnPartEventArgs(string name, string channel, bool isMyself)
        {
            Name = name;
            Channel = channel;
            IsMyself = isMyself;
        }
    }

}
