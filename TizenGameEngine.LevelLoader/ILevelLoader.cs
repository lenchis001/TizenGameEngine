using System;
using System.Threading.Tasks;
using TizenGameEngine.LevelLoader.Models;

namespace TizenGameEngine.LevelLoader
{
    public interface ILevelLoader
    {
        public Level LoadFile(String path);
        public Level LoadContent(String path);
    }
}

