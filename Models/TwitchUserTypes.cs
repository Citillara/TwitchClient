﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.Models
{
    [Flags]
    public enum TwitchUserTypes
    {
        None = 0,
        Subscriber = 1 << 0,
        Mod = 1 << 1,
        Staff = 1 << 2,
        GlobalMod = 1 << 3,
        Broadcaster = 1 << 4,
        Admin = 1 << 5,
        Developer = 1 << 6,
        Founder = 1 << 7,
    }
}
