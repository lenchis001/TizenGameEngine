using System;
using System.Collections.Generic;

namespace TizenGameEngine.LevelLoader.Models
{
    public class Object
    {
        public ObjectType Type { get; set; }

        public ICollection<Object> Children { get; set; }
    }
}

