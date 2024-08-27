using UnityEngine;
using UnityEngine.Events;

namespace CristiEventSystem.EventChannels
{
    [CreateAssetMenu(menuName = "EventChannels/UI Event Channel")]
    public class UiEventChannel : ScriptableObject
    {
        public UnityAction<bool> OnActivateBuildMenu;
        public UnityAction<bool> OnActivateActionsMenu;
        public UnityAction<int> OnCoinsValueChanged;
        public UnityAction<float, float> OnHealthChanged;
        public UnityAction OnCantBuy;

        public void RaiseActivateBuildMenu(bool value)
        {
            OnActivateBuildMenu?.Invoke(value);
        }
    
        public void RaiseActivateActionsMenu(bool value)
        {
            OnActivateActionsMenu?.Invoke(value);
        }

        public void RaiseCoinsValueChanged(int value)
        {
            OnCoinsValueChanged?.Invoke(value);
        }

        public void RaiseCantBuy()
        {
            OnCantBuy?.Invoke();
        }

        public void RaiseHealthChanged(float health, float maxHealth)
        {
            OnHealthChanged?.Invoke(health, maxHealth);
        }
    }
}
