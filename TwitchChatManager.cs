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
        List<string> botmasters = new List<string>();
        List<string> opslist = new List<string>();

        public TwitchChatManager()
        {
            botmasters.Add("lam0r_");
        }

        public TwitchMessage ParseTwitchMessageFromIrc(IrcClientOnPrivateMessageEventArgs args)
        {
            string key = string.Concat(args.Channel, args.Name);

            TwitchMessage retval = new TwitchMessage(args);

            if (opslist.Contains(key))
            {
                retval.UserType |= TwitchUserTypes.Mod;
            }
            if (botmasters.Contains(args.Name))
            {
                retval.UserType |= TwitchUserTypes.Developer;
            }
            if (args.Name.Equals("citillara"))
            {
                retval.UserType |= TwitchUserTypes.Citillara;
            }


            return retval;
        }

        public TwitchNotice ParseTwitchNoticeFromIrc(IrcMessage args)
        {
            TwitchNotice retval = new TwitchNotice(args);

            string key = string.Concat(args.Parameters[0], retval.Login ?? "********");



            if (retval.Login != null && opslist.Contains(key))
            {
                retval.UserType |= TwitchUserTypes.Mod;
            }
            if (retval.Login != null &&  botmasters.Contains(retval.Login))
            {
                retval.UserType |= TwitchUserTypes.Developer;
            }
            if (retval.Login != null && retval.Login.Equals("citillara"))
            {
                retval.UserType |= TwitchUserTypes.Citillara;
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
                        if (!opslist.Contains(name))
                            opslist.Add(name);
                    }
                    else
                    {
                        if (opslist.Contains(name))
                            opslist.Remove(name);
                    }
                }
            }
        }
    }
}
