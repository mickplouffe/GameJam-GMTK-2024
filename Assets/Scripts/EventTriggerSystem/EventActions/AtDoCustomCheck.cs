using EventTriggerSystem.EventActions;
using UnityEngine;

namespace EventTriggerSystem.EventActions
{
    [CreateAssetMenu(fileName = "DoCustomCheck", menuName = "EventActions/DoCustomCheck")]
    public class AtDoCustomCheck : EventActionSettings
    {
        public override void Trigger()
        {
            // GameManager.Instance.DoCustomCheck();
        }
    
    }
}
