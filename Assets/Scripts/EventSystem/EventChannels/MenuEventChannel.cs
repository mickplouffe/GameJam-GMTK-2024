using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "EventChannels/Menu Event Channel")]
public class MenuEventChannel: ScriptableObject
{
    public UnityAction OnPlayButtonPressed;
    public UnityAction OnOptionsButtonPressed;
    public UnityAction OnCreditsButtonPressed;
    public UnityAction OnBackButtonPressed;
    public UnityAction OnExitButtonPressed;
    
    public void RaisePlayButtonPressed()
    {
        OnPlayButtonPressed?.Invoke();
    } 
    public void RaiseOptionsButtonPressedEvent()
    {
        OnOptionsButtonPressed?.Invoke();
    }
    
    public void RaiseCreditsButtonPressed()
    {
        OnCreditsButtonPressed?.Invoke();
    }
    
    public void RaiseBackButtonPressed()
    {
        OnBackButtonPressed?.Invoke();
    }
    
    public void RaiseExitButtonPressed()
    {
        OnExitButtonPressed?.Invoke();
    }

}
