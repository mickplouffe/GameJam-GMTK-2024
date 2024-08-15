using EventTriggerSystem.EventActions;
using UnityEngine;

namespace EventTriggerSystem.EventActions
{
    [CreateAssetMenu(fileName = "PlayVfx", menuName = "EventActions/PlayVfx")]
    public class AtPlayVfx : EventActionSettings
    {
        [SerializeField] private GameObject vfx;
        public override void Trigger()
        {
            Instantiate(vfx);
        }
    
    }
}