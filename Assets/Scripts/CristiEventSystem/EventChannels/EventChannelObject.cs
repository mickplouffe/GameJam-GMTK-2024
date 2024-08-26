using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CristiEventSystem.EventChannels
{
    public abstract class EventChannelObject : ScriptableObject
    {
        private Dictionary<string, UnityAction> _unityActions;
        
        private void OnEnable()
        {
            _unityActions = new Dictionary<string, UnityAction>();
            
            var fields = GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(UnityAction))
                {
                    var action = (UnityAction)field.GetValue(this);
                    if (action != null)
                    {
                        _unityActions.Add(field.Name, action);
                    }
                }
            }
            // Debug.Log("Unity Actions count: " + unityActions.Count);
        }
        
        public void RaiseEvent(string eventNameToRaise)
        {
            Debug.Log($"Event {eventNameToRaise} Selected to be raised.");
            if (_unityActions.TryGetValue(eventNameToRaise, out UnityAction action))
            {
                Debug.Log("INVOKING ACTION: " + action.Method.Name);
                action.Invoke();
            }
            else
            {
                Debug.LogWarning($"Event {eventNameToRaise} not found.");
            }
        }
        
        public List<string> GetUnityActionNames()
        {
            return new List<string>(_unityActions.Keys);
        }

    }
}
