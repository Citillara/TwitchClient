using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.API
{
    [DataContract]
    class ChatProperties
    {

        [DataMember(Name = "_id")]
        public long Id { get; set; }
        [DataMember(Name = "hide_chat_links")]
        public bool HideChatLinks { get; set; }
        [DataMember(Name = "devchat")]
        public bool DevChat { get; set; }
        [DataMember(Name = "game")]
        public string Game { get; set; }
        [DataMember(Name = "require_verified_account")]
        public bool RequireVerifiedAccount { get; set; }
        [DataMember(Name = "subsonly")]
        public bool SubsOnly { get; set; }
        [DataMember(Name = "eventchat")]
        public bool EventChat { get; set; }
        [DataMember(Name = "cluster")]
        public string Cluster { get; set; }

        static T GetData<T>(string url)
        {
            Console.WriteLine(url);
            HttpWebRequest request = WebRequest.CreateHttp(url);
            T result = default(T);
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.116 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers.Add("Accept-Language", "fr,en-US;q=0.8,en;q=0.6");
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            request.KeepAlive = true;

            using (Stream jsonSource = request.GetResponse().GetResponseStream())
            {
                var s = new DataContractJsonSerializer(typeof(T));
                result = (T)s.ReadObject(jsonSource);

            }
            return result;
        }

    }
}
