using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "EventChannels/Tilt Event Channel")]
public class TiltEventChannel : ScriptableObject
{
   public UnityAction<float, Vector3> OnTiltChanged;

   public void RaiseTiltChanged(float tiltAngle, Vector3 tiltDirection)
   {
      OnTiltChanged?.Invoke(tiltAngle, tiltDirection);
   }
}
