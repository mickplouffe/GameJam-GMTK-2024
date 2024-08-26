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
        public UnityAction OnGameOver = delegate { }; // Game Lose condition met
    
        public UnityAction OnDialogueStart = delegate { }; // When a dialogue starts, sending the dialogue object as a parameter
        public UnityAction OnDialogueEnd = delegate { }; // When a dialogue ends, sending the dialogue object as a parameter
        
        
        
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