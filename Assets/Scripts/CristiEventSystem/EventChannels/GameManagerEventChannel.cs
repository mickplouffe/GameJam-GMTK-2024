using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CristiEventSystem.EventChannels
{
    [CreateAssetMenu(menuName = "EventChannels/GameManager Event Channel")]
    public class GameManagerEventChannel : EventChannelObject
    {
        public UnityAction OnGameStart = delegate { }; // Start Game
        public UnityAction OnGamePause = delegate { }; // Pause Game
        public UnityAction OnGameResume = delegate { }; // Resume Game
        public UnityAction OnGameRestart = delegate { }; // Restart Game
        public UnityAction OnGameQuit = delegate { }; // Quit Application 
    
        public UnityAction OnGameWon = delegate { }; // Game Win condition met
        public static UnityAction OnGameOver = delegate { }; // Game Lose condition met
    
        public UnityAction OnDialogueStart = delegate { }; // When a dialogue starts, sending the dialogue object as a parameter
        public UnityAction OnDialogueEnd = delegate { }; // When a dialogue ends, sending the dialogue object as a parameter

        private Dictionary<string, UnityAction> _unityActions;
        
        private void Awake()
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
            Debug.Log("Unity Actions count: " + _unityActions.Count);
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
        
        public void RaiseGameStart()
        {
            OnGameStart?.Invoke();
        }

        public void RaiseGamePause()
        {
            OnGamePause?.Invoke();
        }

        public void RaiseGameResume()
        {
            OnGameResume?.Invoke();
        }

        public void RaiseGameRestart()
        {
            OnGameRestart?.Invoke();
        }

        public void RaiseGameQuit()
        {
            OnGameQuit?.Invoke();
        }

        public void RaiseGameWon()
        {
            OnGameWon?.Invoke();
        }

        public void RaiseGameOver()
        {
            OnGameOver?.Invoke();
        }

        public void RaiseDialogueStart()
        {
            OnDialogueStart?.Invoke();
        }

        public void RaiseDialogueEnd()
        {
            OnDialogueEnd?.Invoke();
        }
    }
}