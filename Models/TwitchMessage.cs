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

        public bool IsWhisper = false;
        public string WhisperChannel;
        public string Channel;
        public string Message;
        public string SenderName;
        public string SenderDisplayName;
        public bool IsSubscriber = false;
        public long SubscriberLevel = -1;
        public bool IsTurbo = false;
        public long UserId = -1;
        public bool IsBits = false;
        public long BitsSent = 0;
        public long BitsLevel = -1;

        public TwitchUserTypes UserType = TwitchUserTypes.None;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("[{0}] " , Channel);

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
            if (IsBits)
                sb.AppendFormat("[Bits({0} {1})] ", BitsLevel, BitsSent);


            sb.AppendFormat("<{0}> " , SenderDisplayName);
            sb.Append(Message);
            return sb.ToString();
        }

        private void ParseCommandAndArgs()
        {
            m_args = Message.Split(' ');
            m_command = m_args[0];
            m_isCommand = m_command.StartsWith("!");
        }

        private string m_command;
        public string Command
        {
            get
            {
                if (m_command == null)
                    ParseCommandAndArgs();
                return m_command;
            }
        }

        private string[] m_args;
        public string[] Args
        {
            get
            {
                if (m_args == null)
                    ParseCommandAndArgs();
                return m_args;
            }
        }

        private bool? m_isCommand;
        public bool IsCommand
        {
            get
            {
                if (!m_isCommand.HasValue)
                    ParseCommandAndArgs();
                return m_isCommand.Value;
            }
        }
    }
}
