using Irc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Twitch.Models;

namespace Twitch.Tools
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
            TwitchMessage retval = new TwitchMessage();
            string key = string.Concat(args.Channel, args.Name);

            TwitchExtra extra = new TwitchExtra(args.Tags);

            retval.Channel = args.Channel;
            retval.Message = args.Message;
            retval.SenderName = args.Name;
            retval.UserType = extra.UserType;
            retval.IsSubscriber = extra.IsSubscriber;
            retval.IsTurbo = extra.IsTurbo;
            retval.UserId = extra.UserId;

            if (!args.Channel.StartsWith("#"))
            {
                retval.IsWhisper = true;
                retval.Channel = args.Name;
                retval.WhisperChannel = args.Channel;
            }


            if (!string.IsNullOrEmpty(extra.DisplayName))
                retval.SenderDisplayName = extra.DisplayName;
            else
                retval.SenderDisplayName = args.Name;

            if(opslist.Contains(key))
            {
                retval.UserType |= TwitchUserTypes.Mod;
            }
            if (extra.IsSubscriber)
            {
                retval.UserType |= TwitchUserTypes.Subscriber;
            }
            if (args.Name.Equals(args.Channel.Substring(1)))
            {
                retval.UserType |= TwitchUserTypes.Broadcaster;
            }
            if (botmasters.Contains(args.Name))
            {
                retval.UserType |= TwitchUserTypes.BotMaster;
            }
            if (args.Name.Equals("citillara"))
            {
                retval.UserType |= TwitchUserTypes.Citillara;
            }

            return retval;
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
