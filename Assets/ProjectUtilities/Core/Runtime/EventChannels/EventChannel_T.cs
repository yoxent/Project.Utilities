using System;
using UnityEngine;

namespace ProjectUtilities.Core.EventChannels
{
    /// <summary>
    /// Generic ScriptableObject-based event channel with payload.
    /// </summary>
    /// <typeparam name="T">Payload type.</typeparam>
    public abstract class EventChannel<T> : ScriptableObject
    {
        private event Action<T> _onEventRaised;

        public void Raise(T value)
        {
            _onEventRaised?.Invoke(value);
        }

        public void Subscribe(Action<T> handler)
        {
            _onEventRaised += handler;
        }

        public void Unsubscribe(Action<T> handler)
        {
            _onEventRaised -= handler;
        }
    }
}

