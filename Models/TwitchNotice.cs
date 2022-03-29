using Irc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.Models
{
    public class TwitchNotice : TwitchExtra
    {

        /*
         * [27/03/2022 11:34:26] @badge-info=founder/19;
badges=moderator/1,founder/0,sub-gift-leader/1;
color=#005FCC;
display-name=Citillara;
emotes=;
flags=;
id=afc01a8e-942b-4bf3-b087-b9821d5e3be0;
login=citillara;
mod=1;
msg-id=raid;
msg-param-displayName=Citillara;
msg-param-login=citillara;
msg-param-profileImageURL=https://static-cdn.jtvnw.net/jtv_user_pictures/62919e71-3996-46d0-bed9-d168cdb03a98-profile_image-70x70.png;
msg-param-viewerCount=1;
room-id=49807019;
subscriber=1;
system-msg=1\sraiders\sfrom\sCitillara\shave\sjoined!;
tmi-sent-ts=1648373666270;
user-id=51064914;
user-type=mod :tmi.twitch.tv USERNOTICE #kaguyanicky
        */


        public string Channel;

        public bool IsRaid = false;
        public string RaiderDisplayName;
        public string RaiderLogin;
        public int RaiderViewerCount = 0;

        public TwitchNotice(IrcMessage message) : base (message.Tags, false)
        {
            Channel = message.Parameters[0]; 
            ParseMainInfo(message.Tags);
        }

        private void ParseMainInfo(Dictionary<string, string> tags)
        {
            if (tags.ContainsKey("msg-id"))
            {
                IsRaid = tags["msg-id"] == "raid";
            }
            if (tags.ContainsKey("msg-param-displayName"))
            {
                RaiderDisplayName = tags["msg-param-displayName"];
            }
            if (tags.ContainsKey("msg-param-login"))
            {
                RaiderLogin = tags["msg-param-login"];
            }
            if (tags.ContainsKey("msg-param-viewerCount"))
            {
                int.TryParse(tags["msg-param-viewerCount"], out RaiderViewerCount);
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("[{0}] ", Channel);
            sb.Append(base.ToString());


            return sb.ToString();
        }
    }
}
