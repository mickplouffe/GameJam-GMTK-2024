using EventTriggerSystem.EventActions;
using UnityEngine;

namespace EventTriggerSystem.EventActions
{
    [CreateAssetMenu(fileName = "ControlsLock", menuName = "EventActions/ControlsLock")]
    public class AtControlsLock : EventActionSettings
    {
        [SerializeField] private bool lockControls;
        public override void Trigger()
        {
            // GameManager.Instance.LockControls(lockControls);
        }
    
    }
}

