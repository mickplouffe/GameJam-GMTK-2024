using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tilt/TiltSettings")]
public class TiltSettings : ScriptableObject
{
    [Range(0.0f, 5.0f)]
    public float tiltAmount = 0.1f;
    public float rotationSpeed;
    public float tileWeight = 0.1f;
    public float distanceWeightModifier = 1.0f;
    public float maxAngleTilt = 45.0f;
}
