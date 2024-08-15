using EventTriggerSystem.EventActions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace EventTriggerSystem.EventActions
{
    [CreateAssetMenu(fileName = "ChangeScene", menuName = "EventActions/ChangeScene")]
    public class AtChangeScene : EventActionSettings
    {
        [SerializeField] private string sceneName;
        [SerializeField] private bool isAdditive; 
        [SerializeField] private bool isReloadIfExist;

        public override void Trigger()
        {
            
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("Scene name is empty");
                return;
            }
            
            // Check if the scene with name exist
            if (SceneManager.GetSceneByName(sceneName).name == null)
            {
                Debug.LogError("Scene name is not exist");
                return;
            }
            
            // Check if the scene is already loaded
            if (!isReloadIfExist && SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                Debug.LogError("Scene is already loaded");
                return;
            }
                
                
            if (isAdditive)
            {
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
            else
            {
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            }
        }
    }
}

