using EventTriggerSystem.EventActions;
using UnityEngine;

namespace EventTriggerSystem.EventActions
{
    [CreateAssetMenu(fileName = "PauseGame", menuName = "EventActions/PauseGame")]
    public class AtPauseGame : EventActionSettings
    {
        [SerializeField] private bool pauseGame;
        public override void Trigger()
        {
            // GameManager.Instance.PauseGame(pauseGame);
        }
    
    }
}
