using System;
using OpenTK.Input;

namespace TizenGameEngine.Services
{
    public interface IKeyEventHandlingService
    {
        void OnEventOccured(KeyboardKeyEventArgs e);
        void OnTick(double time);
    }
}

