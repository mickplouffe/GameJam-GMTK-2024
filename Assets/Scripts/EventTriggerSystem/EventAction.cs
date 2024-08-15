using UnityEngine;

namespace EventTriggerSystem
{
    public sealed class EventAction : MonoBehaviour
    {
        public void Trigger()
        {
            Debug.LogWarning(gameObject.name + " Action Trigger as been Triggered, but no logic is implemented."); 
        }
    }

    public enum EventActionTypes
    {
        PlaySoundClip,
        PlayAnimation,
        PlayParticle,
        PlayVFX,
        SendTriggerToWwise,
        PlayDialogue,
        DoScreenEffect,
        DoCustomCheck, // If object or player placed on the right spot, call a sub Trigger. Like a Chair on the door giving +10 seconds. Being in water of the bath when electricity come back, kill player.
        EditMaterial,
        ControlsLock,
        // Some are added for the sake of the SubTriggers
        ChangeScene,
        EndGame,
        RestartGame,
        PauseGame,
        ResumeGame,
        QuitGame,
        ResetLevel
    
    

    
    
    }
}