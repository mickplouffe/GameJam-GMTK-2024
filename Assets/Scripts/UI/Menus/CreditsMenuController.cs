using System;
using System.Collections.Generic;
using System.Linq;
using CristiEventSystem.EventChannels;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public struct CreditEntry
{
    public string role;
    public List<string> names;
}

[RequireComponent(typeof(UIDocument))]
public class CreditsMenuController : BaseMenu
{
    [SerializeField] private MenuEventChannel _menuEventChannel; 
    
    public VisualTreeAsset roleTemplate;
    public List<CreditEntry> credits;

    private VisualElement _creditsMenuContainer;
    private Button _backButton;
    private VisualElement _prevContainer;
    
    private void OnEnable()
    {
        InitCreditsUI();
        _menuEventChannel.OnCreditsButtonPressed += HandleCreditsButtonPressed;
        _backButton.clicked += HandleBackButtonPressed;
        
        _creditsMenuContainer.visible = _isVisible;
    }
    
    private void OnDisable()
    {
        _menuEventChannel.OnCreditsButtonPressed -= HandleCreditsButtonPressed;
        _backButton.clicked -= HandleBackButtonPressed;
    }
    
    private void HandleCreditsButtonPressed(VisualElement prevContainer)
    {
        uiClickAudioEvent.Post(gameObject);
        _prevContainer = prevContainer;
        _creditsMenuContainer.visible = true;
    }
    
    private void HandleBackButtonPressed()
    {
        uiClickAudioEvent.Post(gameObject);
        _creditsMenuContainer.visible = false;
        _menuEventChannel.RaiseBackButtonPressed(_prevContainer);
    }
    
    private void InitCreditsUI()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Create the main container
        _creditsMenuContainer = new VisualElement { name = "CreditsMenuContainer" };
        _creditsMenuContainer.AddToClassList("menuContainer");

        var titleLabel = new Label("Credits".ToUpper());
        titleLabel.AddToClassList("titleLabel");
        _creditsMenuContainer.Add(titleLabel);
        
        root.Add(_creditsMenuContainer);

        foreach (var credit in credits)
        {
            // Create Role Container
            var roleContainer = new VisualElement
            {
                name = $"Role{credit.role}Container"
            };
            roleContainer.AddToClassList("roleContainer");

            // Create Role Label
            var roleLabel = new Label(credit.role.ToUpper());
            roleLabel.AddToClassList("roleLabel");
            roleContainer.Add(roleLabel);

            // Create Names Container
            var namesContainer = new VisualElement
            {
                name = "NamesContainer"
            };
            namesContainer.AddToClassList("namesContainer");

            foreach (var nameLabel in credit.names.Select(n => new Label(n)))
            {
                nameLabel.AddToClassList("nameLabel");
                namesContainer.Add(nameLabel);
            }

            roleContainer.Add(namesContainer);
            _creditsMenuContainer.Add(roleContainer);

        }

        _backButton = new Button{name = "BackButton", text = "Back"};
        _backButton.AddToClassList("menuButton");
        _creditsMenuContainer.Add(_backButton);
    }
}
