using Assets.Scripts.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    public class BattleUnit
    {
        private static SkillData _defaultSkill;
        public static SkillData DefaultSkill
        {
            get
            {
                if (_defaultSkill == null)
                {
                    _defaultSkill = ScriptableObject.CreateInstance<SkillData>();
                    _defaultSkill.hideFlags = HideFlags.HideAndDontSave;
                }
                return _defaultSkill;
            }
        }

        public CharacterData Data { get; }
        public Team Team { get; }
        public int Hp {  get; private set; }
        public int Mp { get; private set; }
        public bool IsDead => Hp <= 0;
        public IReadOnlyList<SkillData> Skills { get; }


        public string DisplayName() => string.IsNullOrEmpty(Data.CharacterName) ? Data.name : Data.CharacterName;


        public BattleUnit(CharacterData data, Team team)
        {
            Data = data;
            Team = team;
            Hp = data.MaxHp;
            Mp = data.StartMp;
            Skills = SkillsInit(data);
        }


        public int TakeDamage(int rawDamage)
        {
            int damage = Mathf.Max(0, rawDamage - Data.Defense);
            Hp = Mathf.Max(0, Hp - damage);
            return damage;
        }


        public void Heal(int amount)
        {
            Hp = Mathf.Min(Data.MaxHp, Hp + amount);
        }


        public void GainMp(int amount)
        {
            Mp = Mathf.Min(CharacterData.MaxMp, Mp + amount);
        }


        public bool TrySpendMp(int cost)
        {
            if (Mp < cost)
                return false;
            Mp -= cost;
            return true;
        }


        public int RollAttackDamage(out bool isCrit)
        {
            isCrit = Random.value < Data.CritRate;
            return isCrit ? Mathf.RoundToInt(Data.BaseDamage * 1.5f) : Data.BaseDamage;
        }


        private List<SkillData> SkillsInit(CharacterData data)
        {
            var skills = new List<SkillData>();
            if (data.Skills != null)
                foreach (var s in data.Skills)
                    if (s != null)
                        skills.Add(s);
            if (skills.Count == 0)
                skills.Add(DefaultSkill);
            return skills;
        }
    }
}
