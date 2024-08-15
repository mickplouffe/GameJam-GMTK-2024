using UnityEngine;

namespace EventTriggerSystem.EventActions
{
    [CreateAssetMenu(fileName = "ResetLevel", menuName = "EventActions/ResetLevel")]
    public class AtResetLevel : EventActionSettings
    {
        public override void Trigger()
        {
            //GameManager.Instance.ResetLevel();
        }
    
    }
}