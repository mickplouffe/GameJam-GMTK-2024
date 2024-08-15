using System;
using EventTriggerSystem.EventActions;
using NaughtyAttributes;
using UnityEngine.Serialization;

namespace EventTriggerSystem
{
    [Serializable]
    public struct EventActionType {
        public bool isTriggered;
        public EventActionTypes trigger;
        [FormerlySerializedAs("actionScript")] [AllowNesting, Expandable] public EventActionSettings eventActionScript;
    }
}
