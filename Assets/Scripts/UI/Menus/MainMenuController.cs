using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenuController : BaseMenu
{
    [SerializeField] private MenuEventChannel _menuEventChannel;
    
    [SerializeField] private AK.Wwise.Event mainThemeEvent;
    [SerializeField] private AK.Wwise.Event startLevelEvent;
    
    private UIDocument _ui;
    
    private Button _playButton;
    private Button _optionsButton;
    private Button _creditsButton;
    private Button _exitButton;

    private VisualElement _mainMenuContainer;
    
    private void OnEnable()
    {
        _ui = GetComponent<UIDocument>();

        _playButton = _ui.rootVisualElement.Q<Button>("PlayButton");
        _optionsButton = _ui.rootVisualElement.Q<Button>("OptionsButton");
        _creditsButton = _ui.rootVisualElement.Q<Button>("CreditsButton");
        _exitButton = _ui.rootVisualElement.Q<Button>("ExitButton");

        _mainMenuContainer = _ui.rootVisualElement.Q<VisualElement>("MainMenuContainer");
        
        _playButton.clicked += HandlePlayButtonPressed;
        _optionsButton.clicked += HandleOptionsButtonPressed;
        _creditsButton.clicked += HandleCreditsButtonPressed;
        _exitButton.clicked += HandleExitButtonPressed;

        _menuEventChannel.OnBackButtonPressed += HandleBackButtonPressed;
        
        _mainMenuContainer.visible = _isVisible;
    }

    private void OnDisable()
    {
        _playButton.clicked -= HandlePlayButtonPressed;
        _optionsButton.clicked -= HandleOptionsButtonPressed;
        _creditsButton.clicked -= HandleCreditsButtonPressed;
        _exitButton.clicked -= HandleExitButtonPressed;

        _menuEventChannel.OnBackButtonPressed -= HandleBackButtonPressed;
    }

    private void Awake()
    {
        mainThemeEvent.Post(gameObject);
    }

    private void HandlePlayButtonPressed()
    {
        startLevelEvent.Post(gameObject);
        uiClickAudioEvent.Post(gameObject);
        SceneManager.LoadScene("GameScene");
    }

    private void HandleOptionsButtonPressed()
    {
        uiClickAudioEvent.Post(gameObject);
        _mainMenuContainer.visible = false;
        _menuEventChannel.RaiseOptionsButtonPressedEvent(_mainMenuContainer);
    }

    private void HandleBackButtonPressed(VisualElement prevContainer)
    {
        uiClickAudioEvent.Post(gameObject);
        if (_mainMenuContainer != prevContainer)
            return;
        
        _mainMenuContainer.visible = true;
    }
    
    private void HandleCreditsButtonPressed()
    {
        uiClickAudioEvent.Post(gameObject);
        _mainMenuContainer.visible = false;
        _menuEventChannel.RaiseCreditsButtonPressed(_mainMenuContainer);
    }
    
    
    private void HandleExitButtonPressed()
    {
        uiClickAudioEvent.Post(gameObject);
#if UNITY_EDITOR
        // Exit play mode in the editor
        EditorApplication.isPlaying = false;
#else
            // Quit the application
            Application.Quit();
#endif
    }


}