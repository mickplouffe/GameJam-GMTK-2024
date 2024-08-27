using System;
using CristiEventSystem.EventChannels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

// Enum for game states
public enum GameState
{
    Dialogue,
    Wave,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviourSingletonPersistent<GameManager>
{
    // Events for state changes
    public UnityAction<GameState> OnGameStateChanged;

    [SerializeField] private GameManagerEventChannel gameManagerEventChannel;
    [SerializeField] private EnemyEventChannel enemyEventChannel;
    [SerializeField] private UiEventChannel uiEventChannel;
    [SerializeField] private MenuEventChannel menuEventChannel;

    public AK.Wwise.Event scribbleSFX;
    public AK.Wwise.Event gameOverMusic;

    private GameState _currentState;

    private bool waveActive;
    private void OnEnable()
    {
        gameManagerEventChannel.OnDialogueEnd += OnDialogueEnd;
        gameManagerEventChannel.OnGameOver += OnGameOver;
        enemyEventChannel.OnWaveCompleted += OnWaveCompleted;
        menuEventChannel.OnResumeButtonPressed += OnResume;
        gameManagerEventChannel.OnGameRestart += OnGameRestart;

    }
    
    private void OnDisable()
    {
        gameManagerEventChannel.OnDialogueEnd -= OnDialogueEnd;
        gameManagerEventChannel.OnGameOver -= OnGameOver;
        enemyEventChannel.OnWaveCompleted -= OnWaveCompleted;
        menuEventChannel.OnResumeButtonPressed -= OnResume;
        gameManagerEventChannel.OnGameRestart -= OnGameRestart;
    }

    private void Start()
    {
        ChangeState(GameState.Dialogue);
    }

    public bool IsInDialogue()
    {
        return _currentState == GameState.Dialogue;
    }

    private void ChangeState(GameState newState)
    {
        _currentState = newState;
        OnGameStateChanged?.Invoke(newState);

        switch (_currentState)
        {
            case GameState.Dialogue:
                HandleDialogueState();
                break;
            case GameState.Wave:
                HandleWaveState();
                break;
            case GameState.Paused:
                HandlePausedState();
                break;
            case GameState.GameOver:
                HandleGameOverState();
                break;
        }
    }
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) 
            return;
        
        switch (_currentState)
        {
            case GameState.Paused:
                OnResume();
                break;
            case GameState.Wave:
                ChangeState(GameState.Paused);
                break;
        }
    } 
    private void HandleDialogueState()
    {
        uiEventChannel.RaiseActivateBuildMenu(false);
        gameManagerEventChannel.RaiseDialogueStart();
        Time.timeScale = 1.0f;
    }
    
    private void HandleWaveState()
    {
        Time.timeScale = 1.0f;
        
        if(waveActive)
            return;

        waveActive = true;
        
        // Activate build menu, deactivate upgrade menu
        uiEventChannel.RaiseActivateBuildMenu(true);
        enemyEventChannel.RaiseStartNextWave();
    }

    private void HandlePausedState()
    {
        uiEventChannel.RaiseActivateBuildMenu(false);
        menuEventChannel.RaisePauseGame(true);
        Time.timeScale = 0.0f;
    }

    private void HandleGameOverState()
    {
        uiEventChannel.RaiseActivateBuildMenu(false);
        Time.timeScale = 0.0f;
        // Trigger any additional game over logic here
    }


    private void OnWaveCompleted()
    {
        waveActive = false;
        
        if (_currentState == GameState.GameOver)
            return;

        HexGridManager.Instance.AddCircularBlobAtRandomEdgeTile(3);

        ChangeState(GameState.Dialogue);
    }

    private void OnDialogueEnd()
    {
        if (_currentState == GameState.Dialogue)
            ChangeState(GameState.Wave);
    }

    private void OnGameOver()
    {
        ChangeState(GameState.GameOver);
    }

    private void OnResume()
    {
        uiEventChannel.RaiseActivateBuildMenu(true);
        menuEventChannel.RaisePauseGame(false);
        ChangeState(GameState.Wave);
    }

    private void OnGameRestart()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        ChangeState(GameState.Dialogue);
    }
}
