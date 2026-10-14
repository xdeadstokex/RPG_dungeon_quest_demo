using Assets.Scripts.Core;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Infrastructure
{
    public class SceneLoader : ISceneLoader
    {
        public const string TransitionSceneName = "TransitionScene";
        public static SceneLoader Shared { get; } = new SceneLoader();
        public string PendingScene { get; private set; }
        private bool _busy;


        private SceneLoader()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void Load(string sceneName)
        {
            if (_busy) 
                return;
            _busy = true;
            PendingScene = sceneName;
            SceneManager.LoadScene(sceneName);
        }

        public void LoadWithTransition(string sceneName)
        {
            if (_busy) 
                return;
            _busy = true;
            PendingScene = sceneName;
            TransitionScene.nextSceneName = sceneName;
            SceneManager.LoadScene(TransitionSceneName);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == TransitionSceneName) 
                return;
            _busy = false;
            PendingScene = null;
        }
    }
}