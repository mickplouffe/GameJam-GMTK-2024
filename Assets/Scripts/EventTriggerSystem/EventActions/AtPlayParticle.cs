using EventTriggerSystem.EventActions;
using UnityEngine;

namespace EventTriggerSystem.EventActions
{
    // [CreateAssetMenu(fileName = "PlayParticle", menuName = "EventActions/PlayParticle")]
    public class AtPlayParticle : EventActionSettings
    {
        [SerializeField] private ParticleSystem particle;
        public override void Trigger()
        {
            particle.Play();
        }
    
    }
}