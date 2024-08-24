using System;
using System.Collections.Generic;
using CristiEventSystem.EventChannels;
using NaughtyAttributes;
using UnityEngine;

namespace EventTriggerSystem.EventActions
{
    // [CreateAssetMenu(fileName = "ResetLevel", menuName = "EventActions/ResetLevel")]
    public class AtRaiseEvent : EventActionSettings
    {
        [OnValueChanged("UpdateDropdown")] public EventChannelObject eventChannel;
        private bool _isEventChannelObject;
        [ShowIf("_isEventChannelObject")]
        [Dropdown("GetEventNames")]
        public string eventName;
        
        public override void Trigger()
        {
            Debug.Log("Triggered AtRaiseEvent");
            if (eventChannel)
            {
                
                eventChannel.RaiseEvent(eventName);
            }
            
        }

        private void OnEnable()
        {
            UpdateDropdown();
        }

        private void UpdateDropdown()
        {
            _isEventChannelObject = eventChannel;
        }
        
        private List<string> GetEventNames()
        {
            if (eventChannel != null)
            {
                return eventChannel.GetUnityActionNames();
            }
            return new List<string>();
        }
        
        
        
    
    }
}