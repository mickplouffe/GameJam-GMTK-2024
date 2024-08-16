using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GameOverMenuController : BaseMenu
{
    [SerializeField] private MenuEventChannel _menuEventChannel; 
        
    private UIDocument _ui;
    
    private Button _restartButton;
    private Button _optionsButton;
    private Button _exitButton;

    private VisualElement _gameOverMenuContainer;
    
    private void OnEnable()
    {
        _ui = GetComponent<UIDocument>();

        _restartButton = _ui.rootVisualElement.Q<Button>("RestartButton");
        _optionsButton = _ui.rootVisualElement.Q<Button>("OptionsButton");
        _exitButton = _ui.rootVisualElement.Q<Button>("ExitButton");

        _gameOverMenuContainer = _ui.rootVisualElement.Q<VisualElement>("GameOverMenuContainer");

        _restartButton.clicked += HandleRestartButtonPressed;
        _optionsButton.clicked += HandleOptionsButtonPressed;
        _exitButton.clicked += HandleExitButtonPressed;

        _menuEventChannel.OnBackButtonPressed += HandleBackButtonPressed;
        
        _gameOverMenuContainer.visible = _isVisible;
    }

    private void OnDisable()
    {
        _restartButton.clicked -= HandleRestartButtonPressed;
        _optionsButton.clicked -= HandleOptionsButtonPressed;
        _exitButton.clicked -= HandleExitButtonPressed;

        _menuEventChannel.OnBackButtonPressed -= HandleBackButtonPressed;
    }
    
    private void HandleRestartButtonPressed()
    {
        _gameOverMenuContainer.visible = false;
        _menuEventChannel.RaiseRestartButtonPressed();
    }

    private void HandleOptionsButtonPressed()
    {
        _gameOverMenuContainer.visible = false;
        _menuEventChannel.RaiseOptionsButtonPressedEvent(_gameOverMenuContainer);
    }

    private void HandleBackButtonPressed(VisualElement prevContainer)
    {
        if (_gameOverMenuContainer != prevContainer)
            return;
        _gameOverMenuContainer.visible = true;
    }
    
    private void HandleExitButtonPressed()
    {
        // TODO: For not this exits the game but it should transition to the Main Menu UI in some way
#if UNITY_EDITOR
        // Exit play mode in the editor
        EditorApplication.isPlaying = false;
#else
            // Quit the application
            Application.Quit();
#endif
    }
}
