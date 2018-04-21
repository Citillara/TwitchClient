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
        private ManualResetEvent m_KeepAliveToken = new ManualResetEvent(false);
        public static readonly string Version = "2";
        private IrcClient m_Client;
        private TwitchChatManager m_TwitchChatManager = new TwitchChatManager();
        private string m_Name;
        private bool hasBeenDisconnected = false;
        public string Name { get { return m_Name; } }

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
            get { return (Twitch.Models.MessageLevel)m_Client.LogLevel; } 
            set { m_Client.LogLevel = (Irc.MessageLevel) value; } 
        }
        public bool LogToConsole { get { return m_Client.LogToConsole; } set { m_Client.LogToConsole = value; } }
        public bool LogEnabled { get { return m_Client.LogEnabled; } set { m_Client.LogEnabled = value; } }

        public TwitchClient(string nickname, string password)
        {
            m_Client = new IrcClient("irc.chat.twitch.tv", 80, nickname);
            m_Name = nickname;
            m_Client.Password = password;
            m_Client.OnChannelNickListRecived += m_client_OnChannelNickListRecived;
            m_Client.OnJoin += m_client_OnJoin;
            if(KeepAlive)
                m_Client.OnJoin += m_client_OnJoinKeepAlive;
            m_Client.OnLog += m_client_OnLog;
            m_Client.OnMode += m_client_OnMode;
            m_Client.OnNotice += m_client_OnNotice;
            m_Client.OnPart += m_client_OnPart;
            m_Client.OnPerform += m_client_OnPerform;
            m_Client.OnPrivateMessage += m_client_OnPrivateMessage;
            m_Client.OnQuit += m_client_OnQuit;
            m_Client.OnUnknownCommand += m_client_OnUnknownCommand;
            m_Client.OnDisconnect += m_client_OnDisconnect;
            m_Client.LogLevel = Irc.MessageLevel.Info;
            
        }

        void m_client_OnJoinKeepAlive(IrcClient sender, IrcClientOnJoinEventArgs args)
        {
            //Console.WriteLine("Checking keep alive");
            string channel = "#" + m_Name.ToLowerInvariant();
            if (!string.IsNullOrEmpty(KeepAliveChannel))
                channel = KeepAliveChannel;

            if (args.IsMyself && args.Channel == channel)
            {
                //Console.WriteLine("Starting keep alive");
                m_Client.OnJoin -= m_client_OnJoinKeepAlive;
                new Thread(new ThreadStart(KeepAliveLoop)).Start();
            }
        }

        private void m_client_OnDisconnect(IrcClient sender, bool wasManualDisconnect)
        {
            KeepAlive = false;
            m_KeepAliveToken.Set();
            if (OnDisconnect != null)
                OnDisconnect(this, wasManualDisconnect);
        }

        public void Connect()
        {
            if (hasBeenDisconnected)
                throw new ObjectDisposedException("Cannot reconnect after a disconnection. Create a new instance of the class");
            m_Client.Connect();
        }

        public void SendWhisper(string channel, string message)
        {
            if (m_Client.IsConnected)
                m_Client.PrivMsg("#jtv", "/w {0} {1}", channel, message);
        }
        public void SendWhisper(string destination, string format, params object[] arg)
        {
            this.SendWhisper(destination, string.Format(Thread.CurrentThread.CurrentCulture, format, arg));
        }

        public void SendAction(string channel, string action)
        {
            if(m_Client.IsConnected)
                m_Client.PrivMsg(channel, "\x0001ACTION {0}\x01", action);


            //m_client.PrivMsg(channel, "\x01 ACTION {0}\x01", action);
        }
        public void SendAction(string destination, string format, params object[] arg)
        {
            this.SendAction(destination, string.Format(Thread.CurrentThread.CurrentCulture, format, arg));
        }

        public void SendMessage(string channel, string message)
        {
            if (m_Client.IsConnected)
                if (AutoDetectSendWhispers && !channel.StartsWith("#"))
                    SendWhisper(channel, message);
                else 
                    m_Client.PrivMsg(channel, message);
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
            if (m_Client != null && m_Client.IsConnected)
                m_Client.Disconnect();

            KeepAlive = false;
            m_KeepAliveToken.Set();
            if (OnDisconnect != null)
                OnDisconnect(this, true);
        }

        public void Join(string channel)
        {
            if (m_Client.IsConnected)
                m_Client.Join(channel);
        }
        public void Part(string channel)
        {
            if (m_Client.IsConnected)
                m_Client.Part(channel);
        }

        private void KeepAliveLoop()
        {
            Console.WriteLine("[{0}] Started keep alive", DateTime.Now.ToString());
            Random r = new Random();
            string channel = "#" + m_Name.ToLowerInvariant();
            if (!string.IsNullOrEmpty(KeepAliveChannel))
                channel = KeepAliveChannel;

            while (KeepAlive)
            {
                SendMessage(channel, "Keep alive : " + DateTime.UtcNow.ToString());
                int wait = m_KeepAliveInterval * 1000;
                m_KeepAliveToken.Reset();
                m_KeepAliveToken.WaitOne(wait);
            }
        }

        private DateTime m_LastKeepAlive = DateTime.Now;
        private int m_KeepAliveInterval = 2 * 3600;

        public DateTime LastKeepAlive
        {
            get { return m_LastKeepAlive; }
        }

        public int KeepAliveInterval
        {
            get
            {
                return m_KeepAliveInterval;
            }
            set
            {
                m_KeepAliveInterval = value;
                TriggerNextKeepAlive();
            }
        }

        public DateTime NextKeepAlive
        {
            get
            {
                return m_LastKeepAlive.AddSeconds(m_KeepAliveInterval);
            }
        }

        public void TriggerNextKeepAlive()
        {
            m_KeepAliveToken.Set();
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
            var message = m_TwitchChatManager.ParseTwitchMessageFromIrc(args);
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
                OnPart(this, new TwitchClientOnPartEventArgs(args.Name, args.Channel, m_Name.Equals(args.Name)));
        }

        void m_client_OnNotice(IrcClient sender, IrcMessage args)
        {
            var notice = m_TwitchChatManager.ParseTwitchNoticeFromIrc(args);
            if (OnNotice != null)
                OnNotice(this, notice);
        }

        void m_client_OnMode(IrcClient sender, IrcClientOnModeEventArgs args)
        {
            m_TwitchChatManager.OnModeChange(args);
        }

        void m_client_OnLog(IrcClient sender, IrcClientOnLogEventArgs args)
        {
            if (OnLog != null)
                OnLog(this, args);
        }

        void m_client_OnJoin(IrcClient sender, IrcClientOnJoinEventArgs args)
        {
            if(OnJoin != null)
                OnJoin(this, new TwitchClientOnJoinEventArgs(args.Name, args.Channel, args.Name.ToLowerInvariant().Equals(m_Name.ToLowerInvariant())));
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
