using Irc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Twitch.Tools;
using Twitch.Models;
using System.Text.RegularExpressions;
using System.Threading;

namespace Twitch
{
    public class TwitchClient
    {
        
        private IrcClient m_client;
        private TwitchChatManager m_twitch_chat_manager = new TwitchChatManager();
        private string m_name;

        public delegate void TwitchClientOnPartEventHandler(TwitchClient sender, TwitchClientOnPartEventArgs args);
        public event TwitchClientOnPartEventHandler OnPart;

        public delegate void TwitchClientOnJoinEventHandler(TwitchClient sender, TwitchClientOnJoinEventArgs args);
        public event TwitchClientOnJoinEventHandler OnJoin;

        public delegate void TwitchClientOnMessageEventHandler(TwitchClient sender, TwitchMessage args);
        public event TwitchClientOnMessageEventHandler OnMessage;

        public delegate void TwitchClientPerformEventHandler(TwitchClient sender);
        public event TwitchClientPerformEventHandler OnPerform;

        public TwitchClient(string nickname, string password)
        {
            m_client = new IrcClient("irc.chat.twitch.tv", 80, nickname);
            m_client.Password = password;
            m_client.OnChannelNickListRecived += m_client_OnChannelNickListRecived;
            m_client.OnDebug += m_client_OnDebug;
            m_client.OnJoin += m_client_OnJoin;
            m_client.OnLog += m_client_OnLog;
            m_client.OnMode += m_client_OnMode;
            m_client.OnNotice += m_client_OnNotice;
            m_client.OnPart += m_client_OnPart;
            m_client.OnPerform += m_client_OnPerform;
            m_client.OnPrivateMessage += m_client_OnPrivateMessage;
            m_client.OnQuit += m_client_OnQuit;
            m_client.OnUnknownCommand += m_client_OnUnknownCommand;
        }

        public void Connect()
        {
            m_client.Connect();
        }

        public void SendWhisper(string channel, string message)
        {
            if (m_client.IsConnected)
                m_client.PrivMsg("#jtv", "/w {0} {1}", channel, message);
        }

        public void SendAction(string channel, string action)
        {
            if(m_client.IsConnected)
                m_client.PrivMsg(channel, "\x01ACTION {0}\x01", action);
        }
        public void SendAction(string destination, string format, params object[] arg)
        {
            this.SendAction(destination, string.Format(Thread.CurrentThread.CurrentCulture, format, arg));
        }

        public void SendMessage(string channel, string message)
        {
            if (m_client.IsConnected)
                m_client.PrivMsg(channel, message);
        }
        public void SendMessage(string destination, string format, params object[] arg)
        {
            this.SendMessage(destination, string.Format(Thread.CurrentThread.CurrentCulture, format, arg));
        }


        public void Join(string channel)
        {
            if (m_client.IsConnected)
                m_client.Join(channel);
        }


        void m_client_OnUnknownCommand(IrcClient sender, IrcMessage message)
        {
            switch (message.Command)
            {
                default:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(message);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }
        }

        void m_client_OnQuit(IrcClient sender, IrcClientOnQuitEventArgs args)
        {
            throw new NotImplementedException();
        }

        void m_client_OnPrivateMessage(IrcClient sender, IrcClientOnPrivateMessageEventArgs args)
        {
            var message = m_twitch_chat_manager.ParseTwitchMessageFromIrc(args);
            if(OnMessage != null)
                OnMessage(this, message);
        }

        void m_client_OnPerform(IrcClient sender)
        {
            sender.CapabilityRequest("twitch.tv/membership");
            sender.CapabilityRequest("twitch.tv/commands");
            sender.CapabilityRequest("twitch.tv/tags");

            if (OnPerform != null)
                OnPerform(this);
        }

        void m_client_OnPart(IrcClient sender, IrcClientOnPartEventArgs args)
        {
            if(OnPart != null)
                OnPart(this, new TwitchClientOnPartEventArgs(args.Name, args.Channel, m_name.Equals(args.Name)));
        }

        void m_client_OnNotice(IrcClient sender, IrcMessage args)
        {
            throw new NotImplementedException();
        }

        void m_client_OnMode(IrcClient sender, IrcClientOnModeEventArgs args)
        {
            m_twitch_chat_manager.OnModeChange(args);
        }

        Regex r = new Regex(@"[^\u0000-\u007F]", RegexOptions.Compiled);
        void m_client_OnLog(IrcClient sender, IrcClientOnLogEventArgs args)
        {
            Console.WriteLine(r.Replace(args.Message, string.Empty));
        }

        void m_client_OnJoin(IrcClient sender, IrcClientOnJoinEventArgs args)
        {
            if(OnJoin != null)
                OnJoin(this, new TwitchClientOnJoinEventArgs(args.Name, args.Channel, args.Name.Equals(m_name)));
        }

        void m_client_OnDebug(int debug)
        {
            Console.WriteLine(debug);
        }

        void m_client_OnChannelNickListRecived(IrcClient sender, IrcClientOnChannelNickListReceivedEventArgs args)
        {
            if (OnJoin != null)
            {
                foreach(string user in args.NameList)
                    OnJoin(this, new TwitchClientOnJoinEventArgs(user, args.Channel, user.Equals(m_name)));
            }
        }


    }
}
