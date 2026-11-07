using Assets.Scripts.Controller;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameData;

namespace Assets.Scripts.Battle
{
    /// <summary>
    ///just use for demo now, entry-point for combat system
    /// </summary>
    public static class BattleBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }


        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != SceneNames.FightScene.ToString())
                return;

            EnsureCamera();
        }


        private static void EnsureCamera()
        {
            if (Camera.main != null)
                return;

            var go = new GameObject("BattleCamera");
            go.tag = "MainCamera";
            var cam = go.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.07f, 0.08f, 0.13f);
        }


       
    }
}
