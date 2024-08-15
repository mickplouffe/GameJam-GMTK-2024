using EventTriggerSystem.EventActions;
using UnityEngine;

namespace EventTriggerSystem.EventActions
{
    // [CreateAssetMenu(fileName = "PlayDialogue", menuName = "EventActions/PlayDialogue")]
    public class AtPlayDialogue : EventActionSettings
    {
        [SerializeField] private AudioClip dialogue;
        public override void Trigger()
        {
            // GameManager.Instance.PlayDialogue(dialogue);
        }
    
    }
}