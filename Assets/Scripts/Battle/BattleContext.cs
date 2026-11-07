using System.Collections.Generic;
using UnityEngine;
using static GameData;

namespace Assets.Scripts.Battle
{
    public static class BattleContext
    {
        public static string CurrentId = null;
        public static float FightCooldownUntil = 0f;
        public static string ReturnScene = "MainScene";
        public static List<CharacterData> PlayerParty = new();
        public static List<CharacterData> Enemies = new();
        public static HashSet<string> ClearedZones = new();
        public static bool IsDemo { get; private set; }

        public static bool HasData() => PlayerParty.Count > 0 && Enemies.Count > 0;
        public static string ZoneId(string sceneName, string zoneName) => sceneName + "/" + zoneName;


        public static void Set(List<CharacterData> party, List<CharacterData> enemies)
        {
            PlayerParty = party;
            Enemies = enemies;
            IsDemo = false;
        }


        public static void EnsureData()
        {
            if (HasData())
                return;

            Debug.Log("No data loaded, use demo");
            FillDemo();
        }


        public static void Finish(Team winner)
        {
            if (winner == Team.Player && CurrentId != null)
                ClearedZones.Add(CurrentId);
            CurrentId = null;
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
