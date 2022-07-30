using System;
namespace TizenGameEngine.Renderer.Models
{
    public class ReferenceContainer<T>
    {
        T _value;

        public ref T Value {
            get => ref _value;
        }
    }
}

