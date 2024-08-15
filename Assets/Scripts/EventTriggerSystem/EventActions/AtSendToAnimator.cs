using UnityEngine;

namespace EventTriggerSystem.EventActions
{
    // [CreateAssetMenu(fileName = "SendTriggerToAnimator", menuName = "EventActions/SendTriggerToAnimator")]
    public class AtSendToAnimator : EventActionSettings
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string triggerName;
        public override void Trigger()
        {
            animator.SetTrigger(triggerName);
        }
    
    }
}

