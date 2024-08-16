using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct CreditEntry
{
    public string role;
    public List<string> names;
}

public class CreditsMenuController : MonoBehaviour
{
    [SerializeField] private MenuEventChannel _menuEventChannel; 
    
    public VisualTreeAsset roleTemplate;
    public List<CreditEntry> credits;

    private VisualElement _creditsMenuContainer;
    private Button _backButton;
    
    private void OnEnable()
    {
        InitCreditsUI();
        _menuEventChannel.OnCreditsButtonPressed += HandleCreditsButtonPressed;
        _backButton.clicked += HandleBackButtonPressed;
        
        _creditsMenuContainer.visible = false;
    }
    
    private void OnDisable()
    {
        _menuEventChannel.OnCreditsButtonPressed -= HandleCreditsButtonPressed;
        _backButton.clicked -= HandleBackButtonPressed;
    }
    
    private void HandleCreditsButtonPressed()
    {
        _creditsMenuContainer.visible = true;
    }
    
    private void HandleBackButtonPressed()
    {
        _creditsMenuContainer.visible = false;
        _menuEventChannel.RaiseBackButtonPressed();
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
