using Irc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Twitch.Tools;
using Twitch.Models;

namespace Twitch
{
    public class TwitchClient
    {
        
        private IrcClient m_client;
        private TwitchChatManager m_twitch_chat_manager = new TwitchChatManager();
        private string m_name;

        public delegate void TwitchClientOnPartEventHandler(TwitchClient sender, TwitchClientOnPartEventArgs args);
        public event TwitchClientOnPartEventHandler OnPart;

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

        public void SendAction(string channel, string action)
        {
            if(m_client.IsConnected)
                m_client.PrivMsg(channel, "\x01ACTION {1}\x01", action);
        }

        public void SendMessage(string channel, string message)
        {
            if (m_client.IsConnected)
                m_client.PrivMsg(channel, message);
        }


        void m_client_OnUnknownCommand(IrcClient sender, string message)
        {
            throw new NotImplementedException();
        }

        void m_client_OnQuit(IrcClient sender, IrcClientOnQuitEventArgs args)
        {
            throw new NotImplementedException();
        }

        void m_client_OnPrivateMessage(IrcClient sender, IrcClientOnPrivateMessageEventArgs args)
        {
            var message = m_twitch_chat_manager.ParseTwitchMessage(args);
        }

        void m_client_OnPerform(IrcClient sender)
        {
            throw new NotImplementedException();
        }

        void m_client_OnPart(IrcClient sender, IrcClientOnPartEventArgs args)
        {
            if(OnPart != null)
                OnPart(this, new TwitchClientOnPartEventArgs(args.Name, args.Channel, m_name.Equal(args.Name));
        }

        void m_client_OnNotice(IrcClient sender, IrcMessage args)
        {
            throw new NotImplementedException();
        }

        void m_client_OnMode(IrcClient sender, IrcClientOnModeEventArgs args)
        {
            throw new NotImplementedException();
        }

        void m_client_OnLog(IrcClient sender, IrcClientOnLogEventArgs args)
        {
            throw new NotImplementedException();
        }

        void m_client_OnJoin(IrcClient sender, IrcClientOnJoinEventArgs args)
        {
            throw new NotImplementedException();
        }

        void m_client_OnDebug(int debug)
        {
            throw new NotImplementedException();
        }

        void m_client_OnChannelNickListRecived(IrcClient sender, IrcClientOnChannelNickListReceivedEventArgs args)
        {
            throw new NotImplementedException();
        }


    }
}
