using System;
using UnityEngine;

namespace ProjectUtilities.Core.EventChannels
{
    /// <summary>
    /// ScriptableObject-based event channel for decoupled communication.
    /// </summary>
    [CreateAssetMenu(fileName = "EventChannel", menuName = "ProjectUtilities/Event Channel/No Payload")]
    public class EventChannel : ScriptableObject
    {
        private event Action _onEventRaised;

        public void Raise()
        {
            _onEventRaised?.Invoke();
        }

        public void Subscribe(Action handler)
        {
            _onEventRaised += handler;
        }

        public void Unsubscribe(Action handler)
        {
            _onEventRaised -= handler;
        }
    }
}

