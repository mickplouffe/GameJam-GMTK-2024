using EventTriggerSystem.EventActions;
using UnityEngine;

namespace EventTriggerSystem.EventActions
{
    // [CreateAssetMenu(fileName = "PlayAnimation", menuName = "EventActions/PlayAnimation")]
    public class AtPlayAnimation : EventActionSettings
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string animationName;
        public override void Trigger()
        {
            animator.Play(animationName);
        }
    
    }
}