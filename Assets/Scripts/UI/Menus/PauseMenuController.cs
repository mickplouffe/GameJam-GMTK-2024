using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PauseMenuController : BaseMenu
{
    [SerializeField] private MenuEventChannel _menuEventChannel;
    [SerializeField] private GameManagerEventChannel gameManagerEventChannel;
        
    private UIDocument _ui;
    
    private Button _resumeButton;
    private Button _optionsButton;
    private Button _exitButton;

    private VisualElement _pauseMenuContainer;
    
    private void OnEnable()
    {
        _ui = GetComponent<UIDocument>();

        _resumeButton = _ui.rootVisualElement.Q<Button>("ResumeButton");
        _optionsButton = _ui.rootVisualElement.Q<Button>("OptionsButton");
        _exitButton = _ui.rootVisualElement.Q<Button>("ExitButton");

        _pauseMenuContainer = _ui.rootVisualElement.Q<VisualElement>("PauseMenuContainer");

        _resumeButton.clicked += HandleResumeButtonPressed;
        _optionsButton.clicked += HandleOptionsButtonPressed;
        _exitButton.clicked += HandleExitButtonPressed;

        _menuEventChannel.OnBackButtonPressed += HandleBackButtonPressed;
        _menuEventChannel.OnPauseGame += HandlePauseGame;

        gameManagerEventChannel.OnGameOver += HandleGameOver;
        
        _pauseMenuContainer.visible = _isVisible;
    }

    private void OnDisable()
    {
        _resumeButton.clicked -= HandleResumeButtonPressed;
        _optionsButton.clicked -= HandleOptionsButtonPressed;
        _exitButton.clicked -= HandleExitButtonPressed;

        _menuEventChannel.OnBackButtonPressed -= HandleBackButtonPressed;
        _menuEventChannel.OnPauseGame -= HandlePauseGame;

        gameManagerEventChannel.OnGameOver -= HandleGameOver;
    }

    private void HandlePauseGame(bool pause)
    {
        _pauseMenuContainer.visible = pause;
    }

    private void HandleGameOver()
    {
        _pauseMenuContainer.visible = false;
    }
    
    private void HandleResumeButtonPressed()
    {
        _pauseMenuContainer.visible = false;
        _menuEventChannel.RaiseResumeButtonPressed();
    }

    private void HandleOptionsButtonPressed()
    {
        _pauseMenuContainer.visible = false;
        _menuEventChannel.RaiseOptionsButtonPressedEvent(_pauseMenuContainer);
    }

    private void HandleBackButtonPressed(VisualElement prevContainer)
    {
        if (_pauseMenuContainer != prevContainer)
            return;
        _pauseMenuContainer.visible = true;
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
