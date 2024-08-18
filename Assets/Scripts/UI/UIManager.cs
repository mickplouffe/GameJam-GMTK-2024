using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviourSingletonPersistent<UIManager>
{
    [SerializeField] private GameObject buildMenu;
    [SerializeField] private GameObject actionsMenu;

    [SerializeField] private UIEventChannel uitEventChannel;
    [SerializeField] private EnemyEventChennl enemyEventChannel;
    
    private UIDocument _ui;
    private Label _waveTimerLabel;
    private Label _coinsLabel;

    private Coroutine _timerCoroutine;

    private int _coins;

    private void OnEnable()
    {
        uitEventChannel.OnActivateBuildMenu += HandleActivateBuildMenu;
        uitEventChannel.OnActivateActionsMenu += HandleActivateActionsMenu;
        uitEventChannel.OnCoinsValueChanged += HandleCoinsValueChanged;

        enemyEventChannel.OnWaveStart += HandleWaveStart;
    }

    private void OnDisable()
    {
        uitEventChannel.OnActivateBuildMenu -= HandleActivateBuildMenu;
        uitEventChannel.OnActivateActionsMenu -= HandleActivateActionsMenu;
        uitEventChannel.OnCoinsValueChanged -= HandleCoinsValueChanged;
        
        enemyEventChannel.OnWaveStart -= HandleWaveStart;
    }

    public override void Awake()
    {
        base.Awake();
        _ui = GetComponent<UIDocument>();
        _waveTimerLabel = _ui.rootVisualElement.Q<Label>("WaveDelayLabel");
        _coinsLabel = _ui.rootVisualElement.Q<Label>("CoinsLabel");
    }

    private void HandleCoinsValueChanged(int value)
    {
        _coinsLabel.text = $"Coins: {value}";
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void HandleWaveStart(HexTileController tileController, float waveDelay)
    {
        // Start a new countdown timer
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
        }
        _timerCoroutine = StartCoroutine(StartCountdown(waveDelay));
    }
    
    private IEnumerator StartCountdown(float duration)
    {
        _waveTimerLabel.visible = true;
        float remainingTime = duration;
        while (remainingTime > 0.0f)
        {
            UpdateTimerLabel(remainingTime);
            yield return new WaitForSeconds(1.0f/duration);
            remainingTime -= 1.0f/duration;
        }

        // Ensure the timer hits 0 when finished
        UpdateTimerLabel(0.0f);
        _waveTimerLabel.visible = false;
    }
    
    private void UpdateTimerLabel(float remainingTime)
    {
        TimeSpan time = TimeSpan.FromSeconds(remainingTime);
        _waveTimerLabel.text = $"Next Wave In: {time:mm\\:ss}";
    }
    
    
    
    private void HandleActivateActionsMenu()
    {
        buildMenu.SetActive(false);
        actionsMenu.SetActive(true);
    }

    private void HandleActivateBuildMenu()
    {
        buildMenu.SetActive(true);
        actionsMenu.SetActive(false);
    }
}
