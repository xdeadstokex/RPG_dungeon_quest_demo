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
        private const string SystemName = "BattleSystem";

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

            if (GameObject.Find(SystemName) != null) //avoid duplicate
                return;

            if (!BattleContext.HasData())
            {
                Debug.LogWarning("BattleContext is empty -> starting a demo battle");
                FillDemo();
            }

            EnsureCamera();

            var go = new GameObject(SystemName);
            var battle = go.AddComponent<BattleController>();
            var ui = go.AddComponent<BattleHandler>();
            ui.Bind(battle);
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

        private static void FillDemo()
        {
            BattleContext.Set(
                new List<CharacterData>
                {
                    CharacterData.CreateRuntime("Hero", 100, 3, 18, 14, 4, 0.15f),
                    CharacterData.CreateRuntime("Mage", 70, 5, 22, 12, 2, 0.20f),
                },
                new List<CharacterData>
                {
                    CharacterData.CreateRuntime("Goblin", 60, 0, 14, 10, 2, 0.10f),
                    CharacterData.CreateRuntime("Orc", 90, 0, 20, 8, 5, 0.10f),
                });
            BattleContext.ReturnScene = SceneNames.MainScene.ToString();
            BattleContext.CurrentId = null;
        }
    }
}
