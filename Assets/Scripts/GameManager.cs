using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using UnityEngine.Events;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [SerializeField] private GameManagerEventChannel gameManagerEventChannel;
    [SerializeField] private EnemyEventChannel enemyEventChannel;
    [SerializeField] private UIEventChannel uiEventChannel;
    [SerializeField] private MenuEventChannel menuEventChannel;

    public bool IsGamePaused { get; set; }
    public bool IsGameOver { get; set; }
    
    private void OnEnable()
    {
        gameManagerEventChannel.OnDialogueEnd += HandleStartWave;
        gameManagerEventChannel.OnGameOver += HandleGameOver;
        enemyEventChannel.OnWaveCompleted += StartNextCycle;
        menuEventChannel.OnResumeButtonPressed += HandleGameResume;
        gameManagerEventChannel.OnGameRestart += HandleGameRestart;
    }

    private void OnDisable()
    {
        gameManagerEventChannel.OnDialogueEnd -= HandleStartWave;
        enemyEventChannel.OnWaveCompleted -= StartNextCycle;
        gameManagerEventChannel.OnGameOver -= HandleGameOver;
        menuEventChannel.OnResumeButtonPressed -= HandleGameResume;
        gameManagerEventChannel.OnGameRestart -= HandleGameRestart;
    }
        
    private void Start()
    {
        StartNextCycle();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape) || IsGameOver) 
            return;
        
        IsGamePaused = !IsGamePaused;
        HandleGamePause();
    }

    private void StartNextCycle()
    {
        uiEventChannel.RaiseActivateBuildMenu(false);
        gameManagerEventChannel.RaiseDialogueStart();
    }

    private void HandleStartWave()
    {
        uiEventChannel.RaiseActivateBuildMenu(true);
        enemyEventChannel.RaiseStartNextWave();
    }
    
    private void HandleGameResume()
    {
        Time.timeScale = 1.0f;
    }
    
    private void HandleGamePause()
    {
        Time.timeScale = IsGamePaused ? 0.0f : 1.0f;
        menuEventChannel.RaisePauseGame(IsGamePaused);
    }
    
    private void HandleGameOver()
    {
        Time.timeScale = 0.0f;
        IsGameOver = true;
    }

    private void HandleGameRestart()
    {
        uiEventChannel.RaiseActivateBuildMenu(false);
        IsGamePaused = false;
        HandleGamePause();
        IsGameOver = false;
        //
        // Scene scene = SceneManager.GetActiveScene();
        // SceneManager.LoadScene(scene.name);
        StartNextCycle();
    }
}
