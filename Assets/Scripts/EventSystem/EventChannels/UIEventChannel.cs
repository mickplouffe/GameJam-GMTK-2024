using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "EventChannels/UI Event Channel")]
public class UIEventChannel : ScriptableObject
{
    public UnityAction OnActivateBuildMenu;
    public UnityAction OnActivateActionsMenu;
    public UnityAction<int> OnCoinsValueChanged;
    public UnityAction OnCantBuy;

    public void RaiseActivateBuildMenu()
    {
        OnActivateBuildMenu?.Invoke();
    }
    
    public void RaiseActivateActionsMenu()
    {
        OnActivateActionsMenu?.Invoke();
    }

    public void RaiseCoinsValueChanged(int value)
    {
        OnCoinsValueChanged?.Invoke(value);
    }

    public void RaiseCantBuy()
    {
        OnCantBuy?.Invoke();
    }
}
