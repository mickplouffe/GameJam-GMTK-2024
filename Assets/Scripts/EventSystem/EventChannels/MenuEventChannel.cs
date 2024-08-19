using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "EventChannels/Menu Event Channel")]
public class MenuEventChannel: ScriptableObject
{
    public UnityAction OnPlayButtonPressed;
    public UnityAction<VisualElement> OnOptionsButtonPressed;
    public UnityAction<VisualElement> OnCreditsButtonPressed;
    public UnityAction<VisualElement> OnBackButtonPressed;
    public UnityAction OnExitButtonPressed;
    
    // Pause Menu Events
    public UnityAction<bool> OnPauseGame;
    public UnityAction OnResumeButtonPressed;

    public UnityAction OnRestartButtonPressed;
    
    
    [Button]
    public void RaisePlayButtonPressed()
    {
        OnPlayButtonPressed?.Invoke();
    } 
    
    public void RaiseOptionsButtonPressedEvent(VisualElement prevContainer)
    {
        OnOptionsButtonPressed?.Invoke(prevContainer);
    }
    
    public void RaiseCreditsButtonPressed(VisualElement prevContainer)
    {
        OnCreditsButtonPressed?.Invoke(prevContainer);
    }
    
    public void RaiseBackButtonPressed(VisualElement prevContainer)
    {
        OnBackButtonPressed?.Invoke(prevContainer);
    }
    
    public void RaiseExitButtonPressed()
    {
        OnExitButtonPressed?.Invoke();
    }

    public void RaisePauseGame(bool pause)
    {
        OnPauseGame?.Invoke(pause);
    }

    public void RaiseResumeButtonPressed()
    {
        OnResumeButtonPressed?.Invoke();
    }

    public void RaiseRestartButtonPressed()
    {
        OnRestartButtonPressed?.Invoke();
    }
}
