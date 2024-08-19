using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class OptionsMenuController : BaseMenu
{
    [SerializeField] private MenuEventChannel _menuEventChannel;
    [SerializeField] private GameManagerEventChannel gameManagerEventChannel;
    
    private UIDocument _ui;
    private DropdownField _displayResolution;
    private DropdownField _quality;
    private Toggle _fullscreenToggle;
    private VisualElement _optionsMenuContainer;
    private Button _backButton;
    private Resolution[] _allowResolutions;
    private VisualElement _prevContainer;
    
    private void OnEnable()
    {
        _allowResolutions = Screen.resolutions
            .Where(resolution => resolution is
                { width: 720,  height: 480 } or
                { width: 1280, height: 720 } or
                { width: 1920, height: 1080 } or
                { width: 2560, height: 1440 }).ToArray();
        
        _ui = GetComponent<UIDocument>();
        
        _fullscreenToggle = _ui.rootVisualElement.Q<Toggle>("FullscreenToggle");
        _optionsMenuContainer = _ui.rootVisualElement.Q<VisualElement>("OptionsMenuContainer");
        _backButton = _ui.rootVisualElement.Q<Button>("BackButton");
        
        _backButton.clicked += HandleBackButtonPressed;

        InitDisplayResolution();
        InitQualitySettings();
        
        _displayResolution.RegisterValueChangedCallback(_ => OnApplyResolution());
        _quality.RegisterValueChangedCallback(_ => OnApplyQualitySettings());
        _fullscreenToggle.RegisterValueChangedCallback(_ => OnApplyResolution());
        _fullscreenToggle.value = true;

        _menuEventChannel.OnOptionsButtonPressed += HandleOptionsButtonPressed;
        _menuEventChannel.OnPauseGame += HandlePauseGame;

        gameManagerEventChannel.OnGameOver += HandleGameOver;
        
        _optionsMenuContainer.visible = _isVisible;
    }


    private void OnDisable()
    {
        _displayResolution.UnregisterValueChangedCallback(_ => OnApplyResolution());
        _quality.UnregisterValueChangedCallback(_ => OnApplyQualitySettings());
        _fullscreenToggle.UnregisterValueChangedCallback(_ => OnApplyResolution());
        
        _backButton.clicked -= HandleBackButtonPressed;
        
        _menuEventChannel.OnOptionsButtonPressed -= HandleOptionsButtonPressed;
        
        _menuEventChannel.OnPauseGame -= HandlePauseGame;
        
        gameManagerEventChannel.OnGameOver -= HandleGameOver;
    }

    private void HandleGameOver()
    {
        _optionsMenuContainer.visible = false;
    }

    private void HandlePauseGame(bool pause)
    {
        if (_optionsMenuContainer.visible)
            _optionsMenuContainer.visible = pause;
    }
    private void HandleOptionsButtonPressed(VisualElement prevContainer)
    {
        _prevContainer = prevContainer;
        _optionsMenuContainer.visible = true;
    }
    
    private void HandleBackButtonPressed()
    {
        _optionsMenuContainer.visible = false;
        _menuEventChannel.RaiseBackButtonPressed(_prevContainer);
    }
    
    private void InitDisplayResolution()
    {
        _displayResolution = _ui.rootVisualElement.Q<DropdownField>("ResolutionDropdown");
        _displayResolution.choices = _allowResolutions
                .Select(resolution => $"{resolution.width}x{resolution.height}")
                .ToList();
        _displayResolution.index = _allowResolutions
            .Select((resolution, index) => (resolution, index))
            .First(value => value.resolution.width == Screen.currentResolution.width &&
                            value.resolution.height == Screen.currentResolution.height)
            .index;
    }

    private void InitQualitySettings()
    {
        _quality = _ui.rootVisualElement.Q<DropdownField>("GraphicsDropdown");
        _quality.choices = QualitySettings.names.ToList();
        _quality.index = QualitySettings.GetQualityLevel();
    }

    private void OnApplyResolution()
    {
        Resolution resolution = _allowResolutions[_displayResolution.index];
        Screen.SetResolution(resolution.width, resolution.height, _fullscreenToggle.value);
        Debug.Log("Applying new resolution settings.");
    }

    private void OnApplyQualitySettings()
    {
        QualitySettings.SetQualityLevel(_quality.index, true);
        Debug.Log("Applying new quality settings.");
    }
}
