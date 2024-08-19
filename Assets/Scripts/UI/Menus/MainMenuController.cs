using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenuController : BaseMenu
{
    [SerializeField] private MenuEventChannel _menuEventChannel; 
        
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
    
    private void HandlePlayButtonPressed()
    {
        throw new NotImplementedException();
    }

    private void HandleOptionsButtonPressed()
    {
        _mainMenuContainer.visible = false;
        _menuEventChannel.RaiseOptionsButtonPressedEvent(_mainMenuContainer);
    }

    private void HandleBackButtonPressed(VisualElement prevContainer)
    {
        if (_mainMenuContainer != prevContainer)
            return;
        
        _mainMenuContainer.visible = true;
    }
    
    private void HandleCreditsButtonPressed()
    {
        _mainMenuContainer.visible = false;
        _menuEventChannel.RaiseCreditsButtonPressed(_mainMenuContainer);
    }
    
    
    private void HandleExitButtonPressed()
    {
#if UNITY_EDITOR
        // Exit play mode in the editor
        EditorApplication.isPlaying = false;
#else
            // Quit the application
            Application.Quit();
#endif
    }


}