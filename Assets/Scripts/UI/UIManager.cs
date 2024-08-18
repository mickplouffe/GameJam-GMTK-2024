using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIManager : MonoBehaviourSingletonPersistent<UIManager>
{
    [SerializeField] private GameObject buildMenu;
    [SerializeField] private GameObject actionsMenu;

    [SerializeField] private UIEventChannel uitEventChannel;

    private void OnEnable()
    {
        uitEventChannel.OnActivateBuildMenu += HandleActivateBuildMenu;
        uitEventChannel.OnActivateActionsMenu += HandleActivateActionsMenu;
    }

    private void OnDisable()
    {
        uitEventChannel.OnActivateBuildMenu -= HandleActivateBuildMenu;
        uitEventChannel.OnActivateActionsMenu -= HandleActivateActionsMenu;
    }

    private void HandleActivateActionsMenu()
    {
        buildMenu.SetActive(false);
        actionsMenu.SetActive(true);
    }

    private void HandleActivateBuildMenu()
    {
        buildMenu.SetActive(true);
        actionsMenu.SetActive(false);
    }
}
