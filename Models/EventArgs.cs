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

    public class TwitchClientOnLogEventArgs : EventArgs
    {
        private string m_message;
        public string Message { get { return m_message; } }
        private MessageLevel m_level;
        public MessageLevel Level { get { return m_level; } }

        public TwitchClientOnLogEventArgs(string message, MessageLevel level)
        {
            m_message = message;
            m_level = level;
        }
    }

    public enum MessageLevel
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        Critical = 4
    }

}
