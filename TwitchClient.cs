using Irc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Twitch.Models;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;

namespace Twitch
{
    public class TwitchClient
    {
        private ManualResetEvent m_keep_alive_token = new ManualResetEvent(false);
        public static readonly string Version = "2";
        private IrcClient m_client;
        private TwitchChatManager m_twitch_chat_manager = new TwitchChatManager();
        private string m_name;
        private bool hasBeenDisconnected = false;
        public string Name { get { return m_name; } }

        public delegate void TwitchClientOnPartEventHandler(TwitchClient sender, TwitchClientOnPartEventArgs args);
        public event TwitchClientOnPartEventHandler OnPart;

        public delegate void TwitchClientOnJoinEventHandler(TwitchClient sender, TwitchClientOnJoinEventArgs args);
        public event TwitchClientOnJoinEventHandler OnJoin;

        public delegate void TwitchClientOnMessageEventHandler(TwitchClient sender, TwitchMessage args);
        public event TwitchClientOnMessageEventHandler OnMessage;

        public delegate void TwitchClientOnNoticeEventHandler(TwitchClient sender, TwitchNotice args);
        public event TwitchClientOnNoticeEventHandler OnNotice;

        public delegate void TwitchClientOnLogEventHandler(TwitchClient sender, IrcClientOnLogEventArgs args);
        public event TwitchClientOnLogEventHandler OnLog;

        public delegate void TwitchClientOnPerformEventHandler(TwitchClient sender);
        public event TwitchClientOnPerformEventHandler OnPerform;

        public delegate void TwitchClientOnDisconnectEventHandler(TwitchClient sender, bool wasManualDisconnect);
        public event TwitchClientOnDisconnectEventHandler OnDisconnect;

        /// <summary>
        /// If sending to a user as a message, it will automatically be converted to whisper
        /// </summary>
        public bool AutoDetectSendWhispers = false;
        public bool KeepAlive = true;
        public string KeepAliveChannel = "";

        public Twitch.Models.MessageLevel LogLevel 
        { 
            get { return (Twitch.Models.MessageLevel)m_client.LogLevel; } 
            set { m_client.LogLevel = (Irc.MessageLevel) value; } 
        }
        public bool LogToConsole { get { return m_client.LogToConsole; } set { m_client.LogToConsole = value; } }
        public bool LogEnabled { get { return m_client.LogEnabled; } set { m_client.LogEnabled = value; } }

        public TwitchClient(string nickname, string password)
        {
            m_client = new IrcClient("irc.chat.twitch.tv", 80, nickname);
            m_name = nickname;
            m_client.Password = password;
            m_client.OnChannelNickListRecived += m_client_OnChannelNickListRecived;
            m_client.OnDebug += m_client_OnDebug;
            m_client.OnJoin += m_client_OnJoin;
            if(KeepAlive)
                m_client.OnJoin += m_client_OnJoinKeepAlive;
            m_client.OnLog += m_client_OnLog;
            m_client.OnMode += m_client_OnMode;
            m_client.OnNotice += m_client_OnNotice;
            m_client.OnPart += m_client_OnPart;
            m_client.OnPerform += m_client_OnPerform;
            m_client.OnPrivateMessage += m_client_OnPrivateMessage;
            m_client.OnQuit += m_client_OnQuit;
            m_client.OnUnknownCommand += m_client_OnUnknownCommand;
            m_client.OnDisconnect += m_client_OnDisconnect;
            m_client.LogLevel = Irc.MessageLevel.Info;
            
        }

        void m_client_OnJoinKeepAlive(IrcClient sender, IrcClientOnJoinEventArgs args)
        {
            //Console.WriteLine("Checking keep alive");
            string channel = "#" + m_name.ToLowerInvariant();
            if (!string.IsNullOrEmpty(KeepAliveChannel))
                channel = KeepAliveChannel;

            if (args.IsMyself && args.Channel == channel)
            {
                //Console.WriteLine("Starting keep alive");
                m_client.OnJoin -= m_client_OnJoinKeepAlive;
                new Thread(new ThreadStart(KeepAliveLoop)).Start();
            }
        }

        private void m_client_OnDisconnect(IrcClient sender, bool wasManualDisconnect)
        {
            KeepAlive = false;
            m_keep_alive_token.Set();
            if (OnDisconnect != null)
                OnDisconnect(this, wasManualDisconnect);
        }

        public void Connect()
        {
            if (hasBeenDisconnected)
                throw new Exception("Cannot reconnect after a disconnection. Create a new instance of the class");
            m_client.Connect();
        }

        public void SendWhisper(string channel, string message)
        {
            if (m_client.IsConnected)
                m_client.PrivMsg("#jtv", "/w {0} {1}", channel, message);
        }
        public void SendWhisper(string destination, string format, params object[] arg)
        {
            this.SendWhisper(destination, string.Format(Thread.CurrentThread.CurrentCulture, format, arg));
        }

        public void SendAction(string channel, string action)
        {
            if(m_client.IsConnected)
                m_client.PrivMsg(channel, "\x0001ACTION {0}\x01", action);


            //m_client.PrivMsg(channel, "\x01 ACTION {0}\x01", action);
        }
        public void SendAction(string destination, string format, params object[] arg)
        {
            this.SendAction(destination, string.Format(Thread.CurrentThread.CurrentCulture, format, arg));
        }

        public void SendMessage(string channel, string message)
        {
            if (m_client.IsConnected)
                if (AutoDetectSendWhispers && !channel.StartsWith("#"))
                    SendWhisper(channel, message);
                else 
                    m_client.PrivMsg(channel, message);
        }
        public void SendMessage(string destination, string format, params object[] arg)
        {
            this.SendMessage(destination, string.Format(Thread.CurrentThread.CurrentCulture, format, arg));
        }

        /// <summary>
        /// Disconnects the client and close all connection. Cannot be reused afterwards
        /// </summary>
        public void Disconnect()
        {
            hasBeenDisconnected = true;
            if (m_client != null && m_client.IsConnected)
                m_client.Disconnect();

            KeepAlive = false;
            m_keep_alive_token.Set();
            if (OnDisconnect != null)
                OnDisconnect(this, true);
        }

        public void Join(string channel)
        {
            if (m_client.IsConnected)
                m_client.Join(channel);
        }
        public void Part(string channel)
        {
            if (m_client.IsConnected)
                m_client.Part(channel);
        }

        private void KeepAliveLoop()
        {
            Console.WriteLine("Started keep alive");
            Random r = new Random();
            string channel = "#" + m_name.ToLowerInvariant();
            if (!string.IsNullOrEmpty(KeepAliveChannel))
                channel = KeepAliveChannel;

            while (KeepAlive)
            {
                SendMessage(channel, "Keep alive : " + DateTime.UtcNow.Ticks);
                int wait = 24 * 3600 * 1000;
                m_keep_alive_token.WaitOne(wait);
            }
        }

        void m_client_OnUnknownCommand(IrcClient sender, IrcMessage message)
        {
            /*
            switch (message.Command)
            {
                default:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(message);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }*/
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
            var notice = m_twitch_chat_manager.ParseTwitchNoticeFromIrc(args);
            if (OnNotice != null)
                OnNotice(this, notice);
        }

        void m_client_OnMode(IrcClient sender, IrcClientOnModeEventArgs args)
        {
            m_twitch_chat_manager.OnModeChange(args);
        }

        void m_client_OnLog(IrcClient sender, IrcClientOnLogEventArgs args)
        {
            if (OnLog != null)
                OnLog(this, args);
        }

        void m_client_OnJoin(IrcClient sender, IrcClientOnJoinEventArgs args)
        {
            if(OnJoin != null)
                OnJoin(this, new TwitchClientOnJoinEventArgs(args.Name, args.Channel, args.Name.ToLowerInvariant().Equals(m_name.ToLowerInvariant())));
        }

        void m_client_OnDebug(int debug)
        {
            Console.WriteLine(debug);
        }

        void m_client_OnChannelNickListRecived(IrcClient sender, IrcClientOnChannelNickListReceivedEventArgs args)
        {
            //if (OnJoin != null)
            //{
            //    foreach(string user in args.NameList)
            //        OnJoin(this, new TwitchClientOnJoinEventArgs(user, args.Channel, user.ToLowerInvariant().Equals(m_name.ToLowerInvariant())));
            //}
        }

    }
}
