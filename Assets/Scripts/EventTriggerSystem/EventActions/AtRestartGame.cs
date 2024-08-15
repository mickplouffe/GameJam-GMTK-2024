using UnityEngine;

namespace EventTriggerSystem.EventActions
{
    // [CreateAssetMenu(fileName = "RestartGame", menuName = "EventActions/RestartGame")]
    public class AtRestartGame : EventActionSettings
    {
        public override void Trigger()
        {
            //GameManager.Instance.RestartGame();
        }
    
    }
}
