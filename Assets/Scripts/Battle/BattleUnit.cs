using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    public class BattleUnit
    {
        private const float BASE_CRIT_SCALE = 1.1f;
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
        public int MaxHp => Data.MaxHp;
        public int MaxMp => CharacterData.MaxMp;
        public bool IsDead => Hp <= 0;
        public IReadOnlyList<SkillData> Skills { get; }
        //public Action<int, int> OnHpChanged;
        //public Action<int, int> OnMpChanged;
        public event Action<BattleUnit> OnStatChanged;


        public string DisplayName() => string.IsNullOrEmpty(Data.CharacterName) ? Data.name : Data.CharacterName;

        public void RefreshStats() => OnStatChanged?.Invoke(this);

        public BattleUnit(CharacterData data, Team team)
        {
            Data = data;
            Team = team;
            Hp = data.MaxHp;
            Mp = data.StartMp;
            //OnHpChanged?.Invoke(Hp, Hp);
            //OnMpChanged?.Invoke(Mp, Mp);
            Skills = SkillsInit(data);
        }


        public int TakeDamage(int rawDamage)
        {
            int damage = Mathf.Max(0, rawDamage - Data.Defense);
            Hp = Mathf.Max(0, Hp - damage);
            OnStatChanged?.Invoke(this);
            return damage;
        }


        public void Heal(int amount)
        {
            Hp = Mathf.Min(Data.MaxHp, Hp + amount);
            OnStatChanged?.Invoke(this);
        }


        public void GainMp(int amount)
        {
            Mp = Mathf.Min(CharacterData.MaxMp, Mp + amount);
            OnStatChanged?.Invoke(this);
        }


        public bool TrySpendMp(int cost)
        {
            if (Mp < cost)
                return false;
            Mp -= cost;
            OnStatChanged?.Invoke(this);
            return true;
        }


        public int RollAttackDamage(out bool isCrit)
        {
            isCrit = UnityEngine.Random.value < Data.CritRate;
            return isCrit ? Mathf.RoundToInt(Data.BaseDamage * BASE_CRIT_SCALE) : Data.BaseDamage;
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
