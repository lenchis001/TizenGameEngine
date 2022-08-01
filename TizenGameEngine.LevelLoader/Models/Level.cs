using System;
namespace TizenGameEngine.LevelLoader.Models
{
    public class Level: Object
    {
        public new ObjectType Type => ObjectType.LEVEL;

        public string Name { get; set; }
    }
}

