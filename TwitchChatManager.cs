using Irc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Twitch.Models;

namespace Twitch
{
    class TwitchChatManager
    {
        readonly List<string> m_developers;
        readonly List<string> m_founders;
        List<string> m_oplist = new List<string>();

        public TwitchChatManager(string[] founders, string[] developers)
        {
            if (founders == null)
                m_founders = new List<string>();
            else 
                m_founders = new List<string>(founders);

            if(m_developers == null)
                m_developers = new List<string>();
            else
                m_developers = new List<string>(developers);
        }

        public TwitchMessage ParseTwitchMessageFromIrc(IrcClientOnPrivateMessageEventArgs args)
        {
            string key = string.Concat(args.Channel, args.Name);

            TwitchMessage retval = new TwitchMessage(args);

            if (m_oplist.Contains(key))
            {
                retval.UserType |= TwitchUserTypes.Mod;
            }
            if (m_developers.Contains(args.Name))
            {
                retval.UserType |= TwitchUserTypes.Developer;
            }
            if (m_founders.Contains(args.Name))
            {
                retval.UserType |= TwitchUserTypes.Founder;
            }


            return retval;
        }

        public TwitchNotice ParseTwitchNoticeFromIrc(IrcMessage args)
        {
            TwitchNotice retval = new TwitchNotice(args);

            string key = string.Concat(args.Parameters[0], retval.Login ?? "********");



            if (retval.Login != null && m_oplist.Contains(key))
            {
                retval.UserType |= TwitchUserTypes.Mod;
            }
            if (retval.Login != null && m_developers.Contains(retval.Login))
            {
                retval.UserType |= TwitchUserTypes.Developer;
            }
            if (retval.Login != null && m_founders.Contains(retval.Login))
            {
                retval.UserType |= TwitchUserTypes.Founder;
            }


            return retval;
        }

        public TwitchRoomState ParseTwitchRoomStateFromIrc(IrcMessage args)
        {
            return new TwitchRoomState(args);
        }

        public void OnModeChange(IrcClientOnModeEventArgs args)
        {

            foreach (ModeChange md in args.ModeChanges)
            {
                //Console.WriteLine([{3}] {0} gets {1}{2}, md.Name, md.IsAdded ? + : -, md.Mode, args.Channel);
                if (!md.IsGlobalMode && md.Mode == 'o')
                {
                    string name = string.Concat(args.Channel, md.Name);
                    if (md.IsAdded)
                    {
                        if (!m_oplist.Contains(name))
                            m_oplist.Add(name);
                    }
                    else
                    {
                        if (m_oplist.Contains(name))
                            m_oplist.Remove(name);
                    }
                }
            }
        }
    }
}
