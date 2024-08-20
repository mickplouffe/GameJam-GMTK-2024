using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviourSingleton<DialogueManager>
{
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
        WaveConfig currentWaveConfig = EnemySpawner.Instance.GetCurrentWaveConfig();

        if (currentWaveConfig == null || currentWaveConfig.dialogue == null)
        {
            gameManagerEventChannel.RaiseDialogueEnd();
            return;
        }

        GameObject dialogueObject = Instantiate(currentWaveConfig.dialogue);
        dialogueObject.GetComponentInChildren<DialogueController>().IsPlaying = true;
        dialogueObject.GetComponentInChildren<DialogueController>().StartDialogue();
    }
}
