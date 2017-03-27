using Irc;
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
        public bool IsPrime = false;
        public long UserId = -1;
        public long BitsSent = 0;
        public long BitsLevel = -1;

        public TwitchUserTypes UserType = TwitchUserTypes.None;


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

        public TwitchMessage(IrcClientOnPrivateMessageEventArgs args)
        {
            TwitchExtra extra = new TwitchExtra(args.Tags);

            Channel = args.Channel;
            Message = args.Message;
            SenderName = args.Name;
            UserType = extra.UserType;
            IsSubscriber = extra.IsSubscriber;
            SubscriberLevel = extra.SubscriberLevel;
            IsTurbo = extra.IsTurbo;
            IsPrime = extra.IsPrime;
            UserId = extra.UserId;
            BitsLevel = extra.BitsLevel;
            BitsSent = extra.BitsSent;

            if (!args.Channel.StartsWith("#"))
            {
                IsWhisper = true;
                Channel = args.Name;
                WhisperChannel = args.Channel;
            }

            if (!string.IsNullOrEmpty(extra.DisplayName))
                SenderDisplayName = extra.DisplayName;
            else
                SenderDisplayName = args.Name;


            if (extra.IsSubscriber)
            {
                UserType |= TwitchUserTypes.Subscriber;
            }
            if (args.Name.Equals(args.Channel.Substring(1)))
            {
                UserType |= TwitchUserTypes.Broadcaster;
            }

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
