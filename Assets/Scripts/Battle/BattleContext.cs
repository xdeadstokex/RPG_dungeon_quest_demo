using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Battle
{
    public static class BattleContext
    {
        public static List<CharacterData> PlayerParty = new();
        public static List<CharacterData> Enemies = new();
        public static string ReturnScene = "MainScene";

        public static void Set(List<CharacterData> party, List<CharacterData> enemies)
        {
            PlayerParty = party;
            Enemies = enemies;
        }
    }
}
