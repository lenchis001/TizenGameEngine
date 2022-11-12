using System;
using System.Timers;
using Newtonsoft.Json;
using OpenTK.Input;
using TizenGameEngine.Logger;

namespace TizenGameEngine.Services
{
    public class KeyEventHandlingService : IKeyEventHandlingService
    {
        private readonly Action<KeyboardKeyEventArgs> _destination;
        private readonly double _thresshold;
        private double _elapsedTime;

        private KeyboardKeyEventArgs _currentEvent;

        public KeyEventHandlingService(TimeSpan timeout, Action<KeyboardKeyEventArgs> destination)
        {
            _thresshold = timeout.TotalSeconds;
            _destination = destination;
            _elapsedTime = 0;
        }

        public void OnEventOccured(KeyboardKeyEventArgs e)
        {
            if (_currentEvent != null && _currentEvent.Key != e.Key)
            {
                _destination(_currentEvent);
            }

            _currentEvent = e;
            _elapsedTime = 0;
        }

        public void OnTick(double time)
        {
            if (_currentEvent != null)
            {
                _elapsedTime += time;

                if (_elapsedTime > _thresshold)
                {
                    _elapsedTime = 0;
                    _destination(_currentEvent);
                    _currentEvent = null;
                }
            }
        }
    }
}

