using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BLVisual
{
    public static class ConstVars
    {
        public static readonly Dictionary<string, object>[] DefaultRooms = new Dictionary<string, object>[]
        {
            new Dictionary<string,object>(){ ["showStr"] = "Custom", ["putStr"] = "", },
            new Dictionary<string,object>(){ ["showStr"] = "Ava", ["putStr"] = "22625025", },
            new Dictionary<string,object>(){ ["showStr"] = "Bella",  ["putStr"] = "22632424", },
            new Dictionary<string,object>(){ ["showStr"] = "Carol",  ["putStr"] = "22634198", },
            new Dictionary<string,object>(){ ["showStr"] = "Diana",  ["putStr"] = "22637261", },
            new Dictionary<string,object>(){ ["showStr"] = "Eileen", ["putStr"] = "22625027", },
        };
    }
}
