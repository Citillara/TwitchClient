using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.Models
{
    public class TwitchExtra
    {
        /*
         * @badge-info=subscriber/20	badges=subscriber/12,bits/100	client-nonce=f730e7597ce30423be56e87a0e3bda04	color=#D2691E	display-name=OrangeJinjo	emotes=	first-msg=0	flags=0-8:P.3	id=3b84b2a5-401d-48b4-8b5d-fd265bf2df27	mod=0	returning-chatter=0	room-id=26261471	subscriber=1	tmi-sent-ts=1655840318180	turbo=0	user-id=38449705	user-type= :orangejinjo!orangejinjo@orangejinjo.tmi.twitch.tv PRIVMSG #asmongold :holy shit LuL
[Debug] [21/06/2022 21:38:38] */

        public Color UserColor = SystemColors.WindowText;
        public string DisplayName;
        public string Login;
        public bool IsSubscriber = false;
        public bool IsVIP = false;
        public long SubscriberIconLevel = -1;
        public long SuscriberNumberOfMonths = -1;
        public bool IsTurbo = false;
        public bool IsPrime = false;
        public long UserId = -1;
        public long RoomId = -1;
        public long BitsLevel = -1;
        public long BitsSent = 0;
        public long TwitchTimestamp = 0;
        public DateTime TwitchTimestampDate { get { return UnixTimeStampToDateTime(TwitchTimestamp); } }
        public bool IsVerified = false;
        public bool IsFirstMessage = false;
        public long PartnerLevel = -1;
        public TwitchUserTypes UserType = TwitchUserTypes.None;
        public string SystemMessage;
        public bool IsHighlighted = false;
        public bool IsReply = false;

        public SubMessageDataContainer SubMessageData = new SubMessageDataContainer();
        public class SubMessageDataContainer
        {
            public bool IsSub = false;
            public bool IsResub = false;
            public long SubMonths = -1;
            public string SubPlanName;
            public long SubPlan = -1;
            public long SubCumulativeMonths = -1;
            public long SubMultiMonthsDuration = -1;
            public long SubMultiMonthsTenure = -1;
            public bool SubShouldShareStreak = false;
            public bool SubWasGifted = false;
        }

        public ReplyMessageDataContainer ReplyMessageData = new ReplyMessageDataContainer();
        public class ReplyMessageDataContainer
        {
            public string ParentDisplayName;
            public string ParentMessageBody;
            public string ParentMessageId;
            public long ParentUserId;
            public string ParentUserLogin;
        }

        public TwitchExtra(Dictionary<string, string> tags, bool isBroadcaster)
        {
            ParseTags(tags);

            if (isBroadcaster)
            {
                UserType |= TwitchUserTypes.Broadcaster;
            }
        }

        private void ParseTags(Dictionary<string, string> tags)
        {
            if (tags == null)
                return;

            //if (tags.ContainsKey("emotes"))

            ParseMainInfo(tags);
            ParseTwitchOtherSubsAndBits(tags);
            ParseTwitchSubs(tags);
            ParseMisc(tags);
            ParseBagdes(tags);
            ParseBagdesInfo(tags);
        }

        private void ParseMainInfo(Dictionary<string, string> tags)
        {
            if (tags.ContainsKey("display-name"))
            {
                DisplayName = tags["display-name"];
            }
            if (tags.ContainsKey("login"))
            {
                DisplayName = tags["login"];
            }
            if (tags.ContainsKey("user-id"))
            {
                UserId = long.Parse(tags["user-id"]);
            }
            if (tags.ContainsKey("room-id"))
            {
                RoomId = long.Parse(tags["room-id"]);
            }
            if (tags.ContainsKey("tmi-sent-ts"))
            {
                TwitchTimestamp = long.Parse(tags["tmi-sent-ts"]);
            }
            if (tags.ContainsKey("user-type"))
            {
                UserType = ParseType(tags["user-type"]);
            }
            if (tags.ContainsKey("first-msg"))
            {
                IsFirstMessage = tags["first-msg"] == "1";
            }

            if (tags.ContainsKey("msg"))
            {
                switch (tags["msg"])
                {
                    case "sub":
                        SubMessageData.IsSub = true;
                        break;
                    case "resub":
                        SubMessageData.IsResub = true;
                        break;
                    case "highlighted-message":
                        IsHighlighted = true;
                        break;
                    default: break;
                }
            }
        }

        private void ParseTwitchSubs(Dictionary<string, string> tags)
        {
            if (SubMessageData.IsSub || SubMessageData.IsResub)
            {
                if (tags.ContainsKey("msg-param-month"))
                {
                    SubMessageData.SubMonths = long.Parse(tags["msg-param-month"]);
                }
                if (tags.ContainsKey("msg-param-sub-plan-name"))
                {
                    SubMessageData.SubPlanName = tags["msg-param-sub-plan-name"];
                }
                if (tags.ContainsKey("msg-param-sub-plan"))
                {
                    SubMessageData.SubPlan = long.Parse(tags["msg-param-sub-plan"]);
                }
                if (tags.ContainsKey("msg-param-cumulative-month"))
                {
                    SubMessageData.SubCumulativeMonths = long.Parse(tags["msg-param-cumulative-month"]);
                }
                if (tags.ContainsKey("msg-param-multimonth-duration"))
                {
                    SubMessageData.SubMultiMonthsDuration = long.Parse(tags["msg-param-multimonth-duration"]);
                }
                if (tags.ContainsKey("msg-param-multimonth-tenure"))
                {
                    SubMessageData.SubMultiMonthsTenure = long.Parse(tags["msg-param-multimonth-tenure"]);
                }
                if (tags.ContainsKey("msg-param-should-share-streak"))
                {
                    SubMessageData.SubShouldShareStreak = tags["msg-param-should-share-streak"] == "1";
                }
                if (tags.ContainsKey("msg-param-was-gifted"))
                {
                    SubMessageData.SubWasGifted = tags["msg-param-was-gifted"] == "1";
                }
            }
        }
        private void ParseReplyData(Dictionary<string, string> tags)
        {
            if (tags.ContainsKey("reply-parent-display-name"))
            {
                ReplyMessageData.ParentDisplayName = tags["reply-parent-display-name"];
            }
            if (tags.ContainsKey("reply-parent-msg-body"))
            {
                ReplyMessageData.ParentMessageBody = Irc.IrcMessage.UnescapeTag(tags["reply-parent-msg-body"]);
            }
            if (tags.ContainsKey("reply-parent-msg-id"))
            {
                IsReply = true;
                ReplyMessageData.ParentMessageId = tags["reply-parent-msg-id"];
            }
            if (tags.ContainsKey("reply-parent-user-id"))
            {
                ReplyMessageData.ParentUserId = long.Parse(tags["reply-parent-user-id"]);
            }
            if (tags.ContainsKey("reply-parent-user-login"))
            {
                ReplyMessageData.ParentUserLogin = tags["reply-parent-user-login"];
            }
        }
        private void ParseTwitchOtherSubsAndBits(Dictionary<string, string> tags)
        {
            if (tags.ContainsKey("subscriber"))
            {
                if (tags["subscriber"] == "1")
                {
                    IsSubscriber = true;
                    UserType |= TwitchUserTypes.Subscriber;
                }
            }
            if (tags.ContainsKey("turbo"))
            {
                if (tags["turbo"] == "1")
                    IsTurbo = true;
            }
            if (tags.ContainsKey("bits"))
            {
                if (!string.IsNullOrEmpty(tags["bits"]))
                {
                    long.TryParse(tags["bits"], out BitsSent);
                }
            }
        }
        private void ParseMisc(Dictionary<string, string> tags)
        {
            if (tags.ContainsKey("color"))
            {
                if (!string.IsNullOrWhiteSpace(tags["color"]))
                {
                    int RGB = int.Parse(tags["color"].Replace("#", ""), NumberStyles.HexNumber);
                    UserColor = Color.FromArgb(RGB);
                }
            }
            if (tags.ContainsKey("system-msg"))
            {
                SystemMessage = tags["system-msg"];
            }
        }

        private void ParseBagdes(Dictionary<string, string> tags)
        {

            // Parsing badges
            if (tags.ContainsKey("badges"))
            {
                var split = tags["badges"].Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var spl in split)
                {
                    var s = spl.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    if (s.Length == 2)
                    {
                        switch (s[0])
                        {
                            case "subscriber":
                                long.TryParse(s[1], out SubscriberIconLevel);
                                break;
                            case "bits":
                                long.TryParse(s[1], out BitsLevel);
                                break;
                            case "premium":
                                IsPrime = true;
                                break;
                            case "vip":
                                IsVIP = true;
                                break;
                            case "partner":
                                long.TryParse(s[1], out PartnerLevel);
                                if (PartnerLevel > 1)
                                    IsVerified = true;
                                break;
                            default: break;
                        }
                    }
                }
            }
        }


        private void ParseBagdesInfo(Dictionary<string, string> tags)
        {

            // Parsing badges
            if (tags.ContainsKey("badge-info"))
            {
                var split = tags["badge-info"].Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var spl in split)
                {
                    var s = spl.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    if (s.Length == 2)
                    {
                        switch (s[0])
                        {
                            case "subscriber":
                                long.TryParse(s[1], out SuscriberNumberOfMonths);
                                break;
                            default: break;
                        }
                    }
                }
            }
        }

        private TwitchUserTypes ParseType(string type)
        {
            switch (type)
            {
                case "mod": return TwitchUserTypes.Mod;
                case "global_mod": return TwitchUserTypes.GlobalMod;
                case "admin": return TwitchUserTypes.Admin;
                case "staff": return TwitchUserTypes.Staff;
                default: return TwitchUserTypes.None;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (UserType.HasFlag(TwitchUserTypes.Founder))
                sb.AppendFormat("[C] ");
            if (UserType.HasFlag(TwitchUserTypes.Developer))
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
                sb.AppendFormat("[Sub {0}/{1}] ", SuscriberNumberOfMonths, SubscriberIconLevel);
            if (IsVerified)
                sb.AppendFormat("[Verified] ");
            if (PartnerLevel != -1)
                sb.AppendFormat("[Partner {0}] ", PartnerLevel);
            if (IsVIP)
                sb.AppendFormat("[VIP] ");
            if (IsTurbo)
                sb.AppendFormat("[Turbo] ");
            if (IsPrime)
                sb.AppendFormat("[Prime] ");
            if (IsReply)
                sb.AppendFormat("[Reply] ");
            if (IsHighlighted)
                sb.AppendFormat("[Highlight] ");
            if (BitsLevel > 0)
                sb.AppendFormat("[Bit Lv {0}] ", BitsLevel);
            if (BitsSent > 0)
                sb.AppendFormat("[Bit Sent {0}] ", BitsSent);

            return sb.ToString();
        }

        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

    }


    public static class Extensions
    {
        public static string[] Split(this string str, char c, StringSplitOptions op)
        {
            return str.Split(new char[] { c }, op);
        }
    }
}
