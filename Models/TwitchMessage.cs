using Irc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Twitch.Models
{
    public class TwitchMessage : TwitchExtra
    {
        static readonly Regex r = new Regex(@"[^\u0000-\u007F]", RegexOptions.Compiled);

        public bool IsWhisper = false;
        public string WhisperChannel;
        public string Channel;
        public string Message;
        public string SenderName;
        public string SenderDisplayName;

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

        public TwitchMessage(IrcClientOnPrivateMessageEventArgs args) :
            base(args.Tags, args.Name.Equals(args.Channel.Substring(1)))
        {

            Channel = args.Channel;
            Message = args.Message;
            SenderName = args.Name;

            if (!args.Channel.StartsWith("#"))
            {
                IsWhisper = true;
                Channel = args.Name;
                WhisperChannel = args.Channel;
            }

            if (!string.IsNullOrEmpty(DisplayName))
                SenderDisplayName = DisplayName;
            else
                SenderDisplayName = args.Name;


        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("[{0}] " , Channel);

            if (UserType.HasFlag(TwitchUserTypes.Citillara))
                sb.AppendFormat("[C] ");
            if (UserType.HasFlag(TwitchUserTypes.Developper))
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
                sb.AppendFormat("[Sub {0}] ", SubscriberLevel);
            if (IsVIP)
                sb.AppendFormat("[VIP] ");
            if (IsTurbo)
                sb.AppendFormat("[Turbo] ");
            if (IsPrime)
                sb.AppendFormat("[Prime] ");
            if(BitsLevel > 0)
                sb.AppendFormat("[Bit Lv {0}] ", BitsLevel);
            if(BitsSent > 0)
                sb.AppendFormat("[Bit Sent {0}] ", BitsSent);

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

    }
}
