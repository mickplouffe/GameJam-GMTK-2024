using UnityEngine;
using UnityEngine.Events;

namespace CristiEventSystem.EventChannels
{
   [CreateAssetMenu(menuName = "EventChannels/Tilt Event Channel")]
   public class TiltEventChannel : EventChannelObject
   {
      public UnityAction<float, Vector3> OnTiltChanged;

      public void RaiseTiltChanged(float tiltAngle, Vector3 tiltDirection)
      {
         OnTiltChanged?.Invoke(tiltAngle, tiltDirection);
      }
   }
}
