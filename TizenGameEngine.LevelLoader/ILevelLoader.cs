using System;
using System.Threading.Tasks;
using TizenGameEngine.LevelLoader.Models;

namespace TizenGameEngine.LevelLoader
{
    public interface ILevelLoader
    {
        Level LoadFile(String path);
        Level LoadContent(String path);
    }
}

