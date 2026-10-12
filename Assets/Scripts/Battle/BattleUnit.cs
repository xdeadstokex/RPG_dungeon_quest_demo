using UnityEngine;

namespace Assets.Scripts.Battle
{
    public class BattleUnit
    {
        public CharacterData Data { get; }
        public int Hp {  get; private set; }
        public int Mp { get; private set; }
        public bool IsDead => Hp <= 0;


        public BattleUnit(CharacterData data)
        {
            Data = data;
            Hp = data.MaxHp;
            Mp = data.StartMp;
        }

        public int TakeDamage(int rawDamage)
        {
            int damage = Mathf.Max(1, rawDamage - Data.Defense);
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
    }
}
