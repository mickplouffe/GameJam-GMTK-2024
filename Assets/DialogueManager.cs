using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
{
    [SerializeField] private List<DialogueController> dialogueControllers;
    [SerializeField] private GameManagerEventChannel gameManagerEventChannel;
    
    private void OnEnable()
    {
        gameManagerEventChannel.OnDialogueStart += HandleDialogueStart;
    }

    private void OnDisable()
    {
        gameManagerEventChannel.OnDialogueStart -= HandleDialogueStart;
    }

    private void HandleDialogueStart()
    {
        if (dialogueControllers.Count <= 0)
        {
            gameManagerEventChannel.RaiseDialogueEnd();
            return;
        }

        dialogueControllers[EnemySpawner.Instance.CurrentWaveIndex % dialogueControllers.Count]
            .transform.parent
            .gameObject.SetActive(true);
        
        dialogueControllers[EnemySpawner.Instance.CurrentWaveIndex % dialogueControllers.Count].StartDialogue();
    }
}
