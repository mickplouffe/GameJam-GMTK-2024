using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviourSingleton<UIManager>
{
    [SerializeField] private GameObject buildMenu;
    [SerializeField] private GameObject actionsMenu;

    [SerializeField] private UIEventChannel uitEventChannel;
    [SerializeField] private EnemyEventChannel enemyEventChannel;
    [SerializeField] private GameManagerEventChannel gameManagerEventChannel;
    
    [SerializeField] private Color textFlashColor = Color.red;
    [SerializeField] private float textFlashSpeed = 1.0f;
    [SerializeField] private float _flashDuration = 2.0f;
    
    [SerializeField] private AK.Wwise.Event waveCountdown;
    
    private bool _isFlashing;
    private Coroutine _flashCoroutine; // To manage the coroutine
    
    private Color _originalTextColor = Color.black;
    
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
        uitEventChannel.OnCantBuy += HandleCantBuy;

        enemyEventChannel.OnWaveStart += HandleWaveStart;

        gameManagerEventChannel.OnGameOver += HandleGameOver;
        gameManagerEventChannel.OnGameRestart += HandleGameRestart;
    }

    private void OnDisable()
    {
        uitEventChannel.OnActivateBuildMenu -= HandleActivateBuildMenu;
        uitEventChannel.OnActivateActionsMenu -= HandleActivateActionsMenu;
        uitEventChannel.OnCoinsValueChanged -= HandleCoinsValueChanged;
        uitEventChannel.OnCantBuy -= HandleCantBuy;
        
        enemyEventChannel.OnWaveStart -= HandleWaveStart;
        
        gameManagerEventChannel.OnGameOver -= HandleGameOver;
        gameManagerEventChannel.OnGameRestart -= HandleGameRestart;

    }

    private void HandleGameRestart()
    {
        StopAllCoroutines();
        _waveTimerLabel.visible = false;
    }


    public void Awake()
    {
        _ui = GetComponent<UIDocument>();
        _waveTimerLabel = _ui.rootVisualElement.Q<Label>("WaveDelayLabel");
        _coinsLabel = _ui.rootVisualElement.Q<Label>("CoinsLabel");
        _waveTimerLabel.visible = false;
    }

    private void HandleGameOver()
    {
        HandleActivateBuildMenu(false);
    }
    private void HandleCantBuy()
    {
        // If a flash is already ongoing, stop it
        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
        }

        // Start the flashing coroutine
        _flashCoroutine = StartCoroutine(FlashText());
    }
    
    private void HandleCoinsValueChanged(int value)
    {
        _coinsLabel.text = $"Coins: {value}";
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void HandleWaveStart(float waveDelay)
    {
        _waveTimerLabel.visible = true;
        // Start a new countdown timer
        if (_timerCoroutine != null)
            StopCoroutine(_timerCoroutine);

        _timerCoroutine = StartCoroutine(StartCountdown(waveDelay));
    }
    
// Coroutine to handle the flashing effect
    private IEnumerator FlashText()
    {
        float elapsedTime = 0f;
        bool isFlashing = true;

        while (elapsedTime < _flashDuration)
        {
            // Alternate between the original color and the flash color
            _coinsLabel.style.color = isFlashing ? textFlashColor : _originalTextColor;

            // Wait for the next flash change
            yield return new WaitForSeconds(textFlashSpeed);

            // Toggle the flashing state
            isFlashing = !isFlashing;

            // Increment the elapsed time
            elapsedTime += textFlashSpeed;
        }

        // Ensure the label color returns to the original color after flashing
        _coinsLabel.style.color = _originalTextColor;

        // Clear the coroutine reference
        _flashCoroutine = null;
    }
    
    private IEnumerator StartCountdown(float duration)
    {
        _waveTimerLabel.visible = true;
        float remainingTime = duration;
        while (remainingTime > 0.0f)
        {
            waveCountdown.Post(gameObject);
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
    
    private void HandleActivateActionsMenu(bool value)
    {
        actionsMenu.SetActive(value);
        buildMenu.SetActive(false);
    }

    private void HandleActivateBuildMenu(bool value)
    {
        buildMenu.SetActive(value);
        actionsMenu.SetActive(false);
    }
}
