using UnityEngine;
using UnityEngine.Events;

namespace CristiEventSystem.EventChannels
{
    [CreateAssetMenu(menuName = "EventChannels/GameManager Event Channel")]
    public class GameManagerEventChannel : EventChannelObject
    {
        public UnityAction OnGameStart; // Start Game
        public UnityAction OnGamePause; // Pause Game
        public UnityAction OnGameResume; // Resume Game
        public UnityAction OnGameRestart; // Restart Game
        public UnityAction OnGameQuit; // Quit Application 
    
        public UnityAction OnGameWon; // Game Win condition met
        public UnityAction OnGameOver; // Game Lose condition met
    
        public UnityAction OnDialogueStart; // When a dialogue starts, sending the dialogue object as a parameter
        public UnityAction OnDialogueEnd; // When a dialogue ends, sending the dialogue object as a parameter

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