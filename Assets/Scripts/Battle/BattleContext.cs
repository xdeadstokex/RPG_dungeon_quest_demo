using System.Collections.Generic;

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


        public static bool HasData() => PlayerParty.Count > 0 && Enemies.Count > 0;
        public static string ZoneId(string sceneName, string zoneName) => sceneName + "/" + zoneName;


        public static void Set(List<CharacterData> party, List<CharacterData> enemies)
        {
            PlayerParty = party;
            Enemies = enemies;
        }


        public static void Finish(Team winner)
        {
            if (winner == Team.Player && CurrentId != null)
                ClearedZones.Add(CurrentId);
            CurrentId = null;
        }
    }
}
