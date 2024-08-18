using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "EventChannels/UI Event Channel")]
public class UIEventChannel : ScriptableObject
{
    public UnityAction OnActivateBuildMenu;
    public UnityAction OnActivateActionsMenu;

    public void RaiseActivateBuildMenu()
    {
        OnActivateBuildMenu?.Invoke();
    }
    
    public void RaiseActivateActionsMenu()
    {
        OnActivateActionsMenu?.Invoke();
    }
}
