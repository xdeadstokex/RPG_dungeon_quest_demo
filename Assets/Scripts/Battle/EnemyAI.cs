using Assets.Scripts.Controller;
using Assets.Scripts.Data;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Battle
{
    //basic ai for now
    public static class EnemyAI
    {
        //simple for now (cast the strongest skill to the weakest enemy)
        public static ActionChoice Decide(BattleUnit self, IReadOnlyList<BattleUnit> all)
        {
            SkillData skill = null;
            foreach (var s in self.Skills)
            {
                if (BattleTargeting.IsSupportSkill(s))
                    continue;
                if (s.MpCost > self.Mp)
                    continue;
                if (skill == null || s.Power > skill.Power)
                    skill = s;
            }

            if (skill == null)
                skill = BattleUnit.DefaultSkill;

            var candidates = BattleTargeting.GetEffectedCandidates(self, skill, all);
            BattleUnit target = candidates.Count > 0 ? candidates.OrderBy(u => u.Hp).First() : null;
            return new ActionChoice { Skill = skill, Target = target };
        }
    }
}
