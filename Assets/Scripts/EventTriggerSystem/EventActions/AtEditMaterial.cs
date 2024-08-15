using EventTriggerSystem.EventActions;
using UnityEngine;

namespace EventTriggerSystem.EventActions
{
    [CreateAssetMenu(fileName = "EditMaterial", menuName = "EventActions/EditMaterial")]
    public class AtEditMaterial : EventActionSettings
    {
        [SerializeField] private Material material;
        [SerializeField] private Material newMaterial;
        public override void Trigger()
        {
            material = newMaterial;
        }
    
    }
}
