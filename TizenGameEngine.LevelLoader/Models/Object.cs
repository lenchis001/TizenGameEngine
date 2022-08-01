using System;
using System.Collections.Generic;

namespace TizenGameEngine.LevelLoader.Models
{
    public class Object
    {
        public ObjectType Type { get; }

        public ICollection<Object> Children { get; set; }
    }
}

