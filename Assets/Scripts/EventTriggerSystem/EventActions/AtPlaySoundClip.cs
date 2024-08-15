using EventTriggerSystem.EventActions;
using UnityEngine;

namespace EventTriggerSystem.EventActions
{
    // [CreateAssetMenu(fileName = "PlaySoundClip", menuName = "EventActions/PlaySoundClip")]
    public class AtPlaySoundClip : EventActionSettings
    {
        [SerializeField] private AudioClip clip;
        public override void Trigger()
        {
            AudioSource.PlayClipAtPoint(clip, Vector3.zero);
        }
    
    }
}

