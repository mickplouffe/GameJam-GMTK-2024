using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using UnityEngine.Events;

public class GameManager : MonoBehaviourSingletonPersistent<GameManager>
{
    [SerializeField] [ReadOnly] private string persistentSceneName = "PersistentScene";
    [SerializeField] [Tooltip("The String name of all scenes that should be loaded in PlayMode")] List<string> debugScenes;
    [SerializeField] private bool isPaused;
    [SerializeField] private bool isGameOver;
    [SerializeField] private bool isGameWon;
    [SerializeField] private bool isGameLost;
    [SerializeField] private bool isGameRestarting;
    [SerializeField] private bool isGameResuming;
    [SerializeField] private bool isGameStarting;
    [SerializeField] private bool isGameQuitting;
    [SerializeField] private bool isGameLoading;
    [SerializeField] private bool isGameSaving;
    [SerializeField] private bool isGameLoadingScene;
    
    public bool IsPaused => isPaused;
    public bool IsGameOver => isGameOver;
    public bool IsGameWon => isGameWon;
    public bool IsGameLost => isGameLost;
    public bool IsGameRestarting => isGameRestarting;
    public bool IsGameResuming => isGameResuming;
    public bool IsGameStarting => isGameStarting;
    public bool IsGameQuitting => isGameQuitting;
    public bool IsGameLoading => isGameLoading;
    public bool IsGameSaving => isGameSaving;
    public bool IsGameLoadingScene => isGameLoadingScene;
    

    public static UnityAction OnGameAwake; // Awake. Called before Start. Following Unity's Order of Execution Logic
    public static UnityAction OnGameStart; // Start Game
    public static UnityAction OnGamePause; // Pause Game
    public static UnityAction OnGameResume; // Resume Game
    public static UnityAction OnGameRestart; // Restart Game
    public static UnityAction OnGameSave; // Save Game
    public static UnityAction OnGameLoadSave; // Load Save File
    public static UnityAction OnGameQuit; // Quit Application 
    
    public static UnityAction OnGameLoadScene; // Load "System" Scene
    public static UnityAction OnGameUnloadScene; // Unload "System" Scene
    
    public static UnityAction OnGameWon; // Game Win condition met
    public static UnityAction OnGameLost; // Game Lose condition met
    
    // public static UnityAction OnGameStartLoading;
    // public static UnityAction OnGameEndLoading; 
    
    public static UnityAction OnGameStartSaving; // Starting to save to disk
    public static UnityAction OnGameEndSaving; // Finished saving to disk
    
    public static UnityAction OnGameStartSaveLoading; // Starting to load from disk
    public static UnityAction OnGameEndSaveLoading; // Finished loading from disk
    
    public static UnityAction OnGameStartQuitting;
    // public static UnityAction OnGameEndQuitting;
    
    public static UnityAction OnLoadLevel; // Load Gameplay Level Scene
    public static UnityAction OnUnloadLevel; // Unload Gameplay Level Scene
    
    
    public static UnityAction OnPlayerSpawn; // Player Spawned (First Time)
    public static UnityAction OnPlayerDeath; // Player Died
    public static UnityAction OnPlayerRespawn; // Player Respawned / Retry / Revive
    
    public static UnityAction OnPlayerLoseControl; // Disable Player Input Gameplay Wise. Still allow for UI Input (Pause Menu)
    public static UnityAction OnPlayerRegainControl; // Enable Player Input Gameplay Wise.
    public static UnityAction OnPlayerHealthChange; // Player Health Changed (Damage, Heal, etc)
    // public static UnityAction OnPlayerScoreChange; 
    
    public static UnityAction OnEnemyDeath; // When any enemy dies, sending the enemy object as a parameter
    public static UnityAction OnEnemySpawn; // When any enemy spawns, sending the enemy object as a parameter
    public static UnityAction OnEnemyDespawn; // When any enemy despawn, sending the enemy object as a parameter
    public static UnityAction OnEnemyHealthChange; // When any enemy health changes, sending the enemy object as a parameter
    
    public static UnityAction OnQuestComplete; // When a quest is completed, sending the quest object as a parameter
    public static UnityAction OnQuestFailed; // When a quest is failed, sending the quest object as a parameter
    public static UnityAction OnQuestUpdate; // When a quest is updated, sending the quest object as a parameter
    public static UnityAction OnQuestStart; // When a quest is started, sending the quest object as a parameter
    public static UnityAction OnQuestEnd; // When a quest is ended, sending the quest object as a parameter

    public static UnityAction OnDialogueStart; // When a dialogue starts, sending the dialogue object as a parameter
    public static UnityAction OnDialogueEnd; // When a dialogue ends, sending the dialogue object as a parameter
    public static UnityAction OnDialogueUpdate; // When a dialogue is updated, sending the dialogue object as a parameter
    public static UnityAction OnDialogueChoice; // When a dialogue choice is made, sending the dialogue object and choice object as a parameter
    public static UnityAction OnDialogueNext; // When a dialogue is advanced to the next line, sending the dialogue object as a parameter
    
    public static UnityAction OnCutsceneStart; // When a cutscene starts, sending the cutscene object as a parameter
    public static UnityAction OnCutsceneEnd; // When a cutscene ends, sending the cutscene object as a parameter
    public static UnityAction OnCutsceneUpdate; // When a cutscene is updated, sending the cutscene object as a parameter
    public static UnityAction OnCutsceneSkip; // When a cutscene is skipped, sending the cutscene object as a parameter
    
    public static UnityAction OnItemPickup; // When an item is picked up, sending the item object as a parameter
    public static UnityAction OnItemDrop; // When an item is dropped, sending the item object as a parameter
    public static UnityAction OnItemUse; // When an item is used, sending the item object as a parameter
    public static UnityAction OnItemEquip; // When an item is equipped, sending the item object as a parameter
    public static UnityAction OnItemUnequip; // When an item is unequipped, sending the item object as a parameter
    public static UnityAction OnItemDestroy; // When an item is destroyed, sending the item object as a parameter
    
    public static UnityAction OnInventoryOpen; // When the inventory is opened
    public static UnityAction OnInventoryClose; // When the inventory is closed
    public static UnityAction OnInventoryUpdate; // When the inventory is updated
    public static UnityAction OnInventoryAdd; // When an item is added to the inventory
    public static UnityAction OnInventoryRemove; // When an item is removed from the inventory
    
    public static UnityAction OnShopOpen; 
    public static UnityAction OnShopClose;
    public static UnityAction OnShopUpdate;
    public static UnityAction OnShopBuy;
    public static UnityAction OnShopSell;
    
    public static UnityAction OnSettingsOpen; // When the settings / Options menu is opened
    public static UnityAction OnSettingsClose; // When the settings / Options menu is closed
    public static UnityAction OnSettingsUpdate; // When the settings / Options menu is updated
    public static UnityAction OnSettingsApply; // When the settings / Options menu is applied
    public static UnityAction OnSettingsCancel; // When the settings / Options menu is canceled
    public static UnityAction OnSettingsReset; // When the settings / Options menu is reset to default
    
    public static UnityAction OnAudioChange; // When any audio setting is changed
    public static UnityAction OnAudioMute; // When audio is muted
    public static UnityAction OnAudioUnmute; // When audio is unmuted
    public static UnityAction OnAudioVolumeChange; 
    public static UnityAction OnAudioVolumeIncrease;
    public static UnityAction OnAudioVolumeDecrease;
    
    public static UnityAction OnGraphicsChange;
    public static UnityAction OnGraphicsQualityChange;
    public static UnityAction OnGraphicsResolutionChange;
    public static UnityAction OnGraphicsFullscreenChange;
    public static UnityAction OnGraphicsVSyncChange;
    
    public static UnityAction OnInputOpen;
    public static UnityAction OnInputClose;
    public static UnityAction OnInputUpdate;
    public static UnityAction OnInputApply;
    public static UnityAction OnInputReset;
    public static UnityAction OnInputLoseControl; // Disable Player Input Application Wide
    public static UnityAction OnInputRegainControl; // Enable Player Input Application Wide
    public static UnityAction OnInputForceRegainControl; // Force Enable Player Input In Application and Game
    

    void Start()
    {
        // MarkPersistentSceneObjectsDontDestroyOnLoad();
        foreach (string scene in debugScenes)
        {

            try
            {
                SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }

    public string GetPersistentSceneName()
    {
        return persistentSceneName;
    }



    private void HandleGameResume()
    {

    }
    private void HandleGamePause()
    {

    }

    private void HandleGameRestart()
    {

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
