using UnityEngine.SceneManagement;

namespace Utilities
{
    public static class DefaultScene
    {
        private const string PersistentSceneName = "PersistentScene";
        // private static bool _isInitialized = false;

        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        // private static void Initialize()
        // {
        //     if (!_isInitialized)
        //     {
        //         EnsurePersistentSceneLoaded();
        //         SceneManager.sceneUnloaded += OnSceneUnloaded;
        //         _isInitialized = true;
        //     }
        // }

        public static void LoadSceneAsyncAdditively(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        private static void EnsurePersistentSceneLoaded()
        {
            // Check if the persistent scene is already loaded
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == PersistentSceneName)
                {
                    return; // Persistent scene is already loaded
                }
            }

            // Load the persistent scene if not already loaded
            SceneManager.LoadScene(PersistentSceneName, LoadSceneMode.Additive);
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            if (scene.name == PersistentSceneName)
            {
                SceneManager.LoadScene(PersistentSceneName, LoadSceneMode.Additive);
            }
        }


    }
}