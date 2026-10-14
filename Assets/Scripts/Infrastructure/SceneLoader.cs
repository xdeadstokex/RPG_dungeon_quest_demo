using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Infrastructure
{
    public class SceneLoader : ISceneLoader
    {
        public string PendingScene { get; private set; }

        public void Load(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void LoadWithTransition(string sceneName)
        {
            PendingScene = sceneName;
            SceneManager.LoadScene("TransitionScene");
        }
    }
}
