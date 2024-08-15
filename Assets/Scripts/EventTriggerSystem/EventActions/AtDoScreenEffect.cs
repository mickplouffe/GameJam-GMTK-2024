using EventTriggerSystem.EventActions;
using UnityEngine;

namespace EventTriggerSystem.EventActions
{
    [CreateAssetMenu(fileName = "DoScreenEffect", menuName = "EventActions/DoScreenEffect")]
    public class AtDoScreenEffect : EventActionSettings
    {
        [SerializeField] private bool doScreenEffect;
        public override void Trigger()
        {
            // GameManager.Instance.DoScreenEffect(doScreenEffect);
        }
    
    }
}
